#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saro {
    public enum MenuItemState {
        Disabled,
        Selected,
        Normal
    }

    public static class EditorEx {

        public static float DragValue (Rect rect, float value, float step) {
            return DragValue (rect, GUIContent.none, value, step, GUIStyle.none);
        }

        /// <summary>
        /// Drag mouse to change filed
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="content"></param>
        /// <param name="value"></param>
        /// <param name="step"></param>
        /// <param name="style"></param>
        /// <param name="horizontal"></param>
        /// <returns></returns>
        public static float DragValue (Rect rect, GUIContent content, float value, float step, GUIStyle style, bool horizontal = true) {
            int controllID = GUIUtility.GetControlID (FocusType.Passive);

            switch (Event.current.GetTypeForControl (controllID)) {
                case EventType.Repaint:
                    GUI.Label (rect, content, style);
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 0 && rect.Contains (Event.current.mousePosition))
                        GUIUtility.hotControl = controllID;
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controllID) {
                        GUIUtility.hotControl = 0;
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
                    }
                    break;
                default:
                    break;
            }

            if (GUIUtility.hotControl == controllID) {
                if (Event.current.isMouse) {
                    if (Event.current.type == EventType.MouseDrag) {
                        if (horizontal) value += Event.current.delta.x * step;
                        else value -= Event.current.delta.y * step;

                        // 保留后小数点后多少位
                        value = (float) System.Math.Round (value, 4);

                        GUI.changed = true;
                    }
                    Event.current.Use ();
                }
                rect.size = new Vector2 (1000, 1000);
                rect.center = Event.current.mousePosition;
            }

            EditorGUIUtility.AddCursorRect (rect, horizontal?MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
            return value;
        }

        public static GenericMenu CreateMenu<T> (
            IEnumerable<T> items,
            Func<T, GUIContent> getItemContent,
            Func<T, MenuItemState> getItemState,
            Action<T> onSelect
        ) {
            GenericMenu menu = new GenericMenu ();
            GUIContent content;
            MenuItemState state;

            foreach (var item in items) {
                content = getItemContent (item);
                if (content.text.EndsWith ("/")) {
                    menu.AddSeparator (content.text.Substring (0, content.text.Length - 1));
                } else {
                    state = getItemState (item);
                    if (state == MenuItemState.Disabled) {
                        menu.AddDisabledItem (content);
                    } else {
                        T current = item;
                        menu.AddItem (content, state == MenuItemState.Selected, () => onSelect (current));
                    }
                }
            }
            return menu;
        }

        /// <summary>
        /// draw progress bar
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="value"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="foregroundColor"></param>
        /// <returns></returns>
        public static float ProgressBar (
            Rect rect,
            float value,
            Color backgroundColor,
            Color foregroundColor
        ) {
            int controlID = GUIUtility.GetControlID (FocusType.Passive);

            switch (Event.current.GetTypeForControl (controlID)) {
                case EventType.Repaint:
                    using (new GUIColorScope (backgroundColor)) {
                        GUI.DrawTexture (rect, EditorGUIUtility.whiteTexture);

                        var progressRect = rect;
                        progressRect.width = Mathf.Round (progressRect.width * value);
                        GUI.color = foregroundColor;
                        GUI.DrawTexture (progressRect, EditorGUIUtility.whiteTexture);
                    }
                    break;

                case EventType.MouseDown:
                    if (Event.current.button == 0 && rect.Contains (Event.current.mousePosition)) {
                        GUIUtility.hotControl = controlID;
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID) {
                        GUIUtility.hotControl = 0;
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
                    }
                    break;
            }

            if (GUIUtility.hotControl == controlID) {
                if (Event.current.isMouse) {
                    float offset = Event.current.mousePosition.x - rect.x + 1f;
                    value = Mathf.Clamp01 (offset / rect.width);

                    GUI.changed = true;
                    Event.current.Use ();
                }

                rect.size = new Vector2 (1000, 1000);
                rect.center = Event.current.mousePosition;
            }

            EditorGUIUtility.AddCursorRect (rect, MouseCursor.SlideArrow);

            return value;
        }
    }
}
#endif