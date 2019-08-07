#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saro {

    [CustomPropertyDrawer (typeof (Interpolator))]
    [CustomPropertyDrawer (typeof (CustomizableInterpolator))]
    public class InterpolatorDrawer : PropertyDrawer {
        private int m_LastType;
        private float m_LastStrength;
        private float m_MinValue;
        private float m_MaxValue;

        private List<Vector3> m_Samples = new List<Vector3> ();

        private static GUIStyle m_ButtonStyle;

        public static GUIStyle ButtonStyle {
            get {
                if (m_ButtonStyle == null) {
                    m_ButtonStyle = new GUIStyle (GUIStyle.none);
                    m_ButtonStyle.clipping = TextClipping.Clip;
                }
                return m_ButtonStyle;
            }
        }

        private void Sample (int type, float strength, int maxSegments, float maxError) {
            if (m_Samples.Count == 0 || type != m_LastType || strength != m_LastStrength) {
                m_LastType = type;
                m_LastStrength = strength;
                m_Samples.Clear ();

                var interpolator = new Interpolator ((Interpolator.Type) type, strength);

                // add next node
                Vector3 point = new Vector3 (0, interpolator[0]);
                m_Samples.Add (point);

                // add more
                Vector3 lastSample = point;
                Vector3 lastEvaluate = point;
                m_MinValue = m_MaxValue = point.y;

                float minSlope = float.MinValue;
                float maxSlope = float.MaxValue;

                for (int i = 1; i < maxSegments; i++) {
                    point.x = i / (float) maxSegments;
                    point.y = interpolator[point.x];

                    if (m_MinValue > point.y) m_MinValue = point.y;
                    if (m_MaxValue < point.y) m_MaxValue = point.y;

                    maxSlope = Mathf.Min ((point.y - lastSample.y + maxError) / (point.x - lastSample.x), maxSlope);
                    minSlope = Mathf.Max ((point.y - lastSample.y - maxError) / (point.x - lastSample.x), minSlope);

                    if (minSlope >= maxSlope) {
                        m_Samples.Add (lastSample = lastEvaluate);
                        maxSlope = (point.y - lastSample.y + maxError) / (point.x - lastSample.x);
                        minSlope = (point.y - lastSample.y - maxError) / (point.x - lastSample.x);
                    }

                    lastEvaluate = point;
                }

                // add last node
                m_Samples.Add (point);
                if (m_MinValue > point.y) m_MinValue = point.y;
                if (m_MaxValue < point.y) m_MaxValue = point.y;

                //
                if (m_MaxValue - m_MinValue < 1f) {
                    if (m_MinValue < 0f) m_MaxValue = m_MinValue + 1f;
                    else if (m_MaxValue > 1f) m_MinValue = m_MaxValue - 1f;
                    else {
                        m_MinValue = 0f;
                        m_MaxValue = 1f;
                    }
                }
            }
        }

        // draw curve
        private void DrawCurve (Rect rect, bool drawStrength) {
            EditorGUI.DrawRect (rect, new Color (.3f, .3f, .3f));

            if (drawStrength) {
                EditorGUI.DrawRect (new Rect (rect.x + (rect.width - 1) * m_LastStrength, rect.y, 1, rect.height), new Color (1f, .3f, 0));
            }

            Vector2 origin = new Vector2 (rect.x + 1, rect.y + 1);
            Vector2 scale = new Vector2 (rect.width - 2, (rect.height - 2) / (m_MaxValue - m_MinValue));

            Vector3 last = m_Samples[0];
            last.x = origin.x + last.x * scale.x;
            last.y = origin.y + (m_MaxValue - last.y) * scale.y;

            using (new HandlesColorScope (new Color (1f, 1f, 1f, .8f))) {
                Vector3 point;

                for (int i = 1; i < m_Samples.Count; i++) {
                    point = m_Samples[i];
                    point.x = origin.x + point.x * scale.x;
                    point.y = origin.y + (m_MaxValue - point.y) * scale.y;

                    // draw AA Line
                    Handles.DrawAAPolyLine (last, point);
                    last = point;
                }

                // draw wire rect
                var color = new Color (0, 0, 0, .4f);
                Rect draw = new Rect (rect.x, rect.y, rect.width, 1f);
                EditorGUI.DrawRect (draw, color);
                draw.y = rect.yMax - 1f;
                EditorGUI.DrawRect (draw, color);
                draw.yMax = draw.yMin;
                draw.yMin = rect.yMin + 1f;
                draw.width = 1f;
                EditorGUI.DrawRect (draw, color);
                draw.x = rect.xMax - 1f;
                EditorGUI.DrawRect (draw, color);
            }
        }

        public override bool CanCacheInspectorGUI (SerializedProperty property) {
            return false;
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            position = EditorGUI.PrefixLabel (position, label);

            var typeProp = property.FindPropertyRelative ("type");
            int type = typeProp.intValue;
            var strengthProp = property.FindPropertyRelative ("strength");

            var image = (Texture2D) EditorGUIUtility.Load ("Builtin Skins/DarkSkin/Images/pane options.png");

            var btnRect = new Rect (position.x + 1, position.y + 2, image.width, image.height);

            using (var scope = new ChangeCheckScope (null)) {
                EditorGUIUtility.AddCursorRect (btnRect, MouseCursor.Arrow);

                System.Enum newType;
                if (fieldInfo.FieldType == typeof (CustomizableInterpolator)) {
                    newType = EditorGUI.EnumPopup (btnRect, GUIContent.none, (CustomizableInterpolator.Type) type, ButtonStyle);
                } else {
                    newType = EditorGUI.EnumPopup (btnRect, GUIContent.none, (Interpolator.Type) type, ButtonStyle);
                }

                if (scope.Changed) {
                    typeProp.intValue = type = (int) (CustomizableInterpolator.Type) newType;
                    strengthProp.floatValue = .5f;
                }
            }

            if ((CustomizableInterpolator.Type) type == CustomizableInterpolator.Type.CustomCurve) {
                EditorGUIUtility.AddCursorRect (position, MouseCursor.Zoom);
                EditorGUI.PropertyField (position, property.FindPropertyRelative ("customCurve"), GUIContent.none);
            } else {
                bool drawStrength;

                switch ((CustomizableInterpolator.Type) type) {
                    case CustomizableInterpolator.Type.Linear:
                    case CustomizableInterpolator.Type.Parabolic:
                    case CustomizableInterpolator.Type.Sine:
                        drawStrength = false;
                        break;
                    default:
                        strengthProp.floatValue = Mathf.Clamp01 (EditorEx.DragValue (position, strengthProp.floatValue, .01f));
                        drawStrength = true;
                        break;
                }

                if (Event.current.type == EventType.Repaint) {
                    Sample (type, strengthProp.floatValue, Mathf.Min ((int) position.width, 256), .002f);
                    DrawCurve (position, drawStrength);
                }
            }

            var content = new GUIContent ();
            content.image = image;
            EditorGUI.LabelField (btnRect, content, GUIStyle.none);
        }
    }
}
#endif