#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Saro {

    public abstract partial class TweenAnimation {
        public static readonly Dictionary<Type, TweenAnimationAttribute> allTypeDic = new Dictionary<Type, TweenAnimationAttribute> ();

        [SerializeField] bool m_Foldout = true;

        [InitializeOnLoadMethod]
        private static void Init () {
            var types = ReflectionEx.GetAllAssemblyTypes.Where (
                t => t.IsSubclassOf (typeof (TweenAnimation)) &&
                t.IsDefined (typeof (TweenAnimationAttribute), false) &&
                !t.IsAbstract
            );

            foreach (var type in types) {
                allTypeDic.Add (type, type.GetCustomAttributes (typeof (TweenAnimationAttribute), false) [0] as TweenAnimationAttribute);
            }
        }

        public abstract void Record ();
        public abstract void Restore ();
        public virtual void Reset () {
            enabled = true;
            m_MinNormalizedTime = 0f;
            m_MaxNormalizedTime = 1f;
            m_Interpolator = new CustomizableInterpolator ();
            m_Foldout = true;
        }

        /// <summary>
        /// float type, binding From and To
        /// </summary>
        /// <param name="label"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        protected static void FromToFieldLayout (string label, SerializedProperty from, SerializedProperty to) {
            float fromValue = from.floatValue;
            float toValue = to.floatValue;

            FromToFieldLayout (label, ref fromValue, ref toValue, out bool fromChanged, out bool toChanged);

            if (fromChanged) from.floatValue = fromValue;
            if (toChanged) to.floatValue = toValue;
        }

        protected static void FromToFieldLayout (string label, ref float from, ref float to, out bool fromChanged, out bool toChanged) {
            var rect = EditorGUILayout.GetControlRect ();
            float labelWidth = EditorGUIUtility.labelWidth;

            var fromRect = new Rect (rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
            var toRect = new Rect (rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
            rect.width = labelWidth - 8;

            // new GUIContent
            var content = new GUIContent ();
            content.text = label;
            EditorGUI.LabelField (rect, content);

            rect.width = EditorStyles.label.CalcSize (content).x;
            float delta = EditorEx.DragValue (rect, 0, 0.01f);

            using (new LabelWidthScope (14)) {
                float newFrom = EditorGUI.FloatField (fromRect, "F", from + delta);
                float newTo = EditorGUI.FloatField (toRect, "T", to + delta);

                fromChanged = from != newFrom;
                from = newFrom;

                toChanged = to != newTo;
                to = newTo;
            }
        }

        /// <summary>
        /// binding From, To and Toggle
        /// </summary>
        /// <param name="label"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="toggle"></param>
        protected static void FromToFIeldLayout (string label, SerializedProperty from, SerializedProperty to, SerializedProperty toggle) {
            float fromValue = from.floatValue;
            float toValue = to.floatValue;
            bool toggleValue = toggle.boolValue;

            FromToFIeldLayout (label, ref fromValue, ref toValue, ref toggleValue, out bool fromChanged, out bool toChanged, out bool toggleChanged);

            if (fromChanged) from.floatValue = fromValue;
            if (toChanged) to.floatValue = toValue;
            if (toggleChanged) toggle.boolValue = toggleValue;
        }

        protected static void FromToFIeldLayout (string label, ref float from, ref float to, ref bool toggle, out bool fromChanged, out bool toChanged, out bool toggleChanged) {
            var rect = EditorGUILayout.GetControlRect ();
            float labelWidth = EditorGUIUtility.labelWidth;

            var fromRect = new Rect (rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
            var toRect = new Rect (rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
            var toggleRect = new Rect (rect.x, rect.y, rect.height, rect.height);
            rect.width = labelWidth - 8 - toggleRect.width;
            rect.x = toggleRect.xMax;

            bool newToggle = EditorGUI.Toggle (toggleRect, toggle);
            using (new DisabledScope (!newToggle)) {

                // new GUIContent
                var content = new GUIContent ();
                content.text = label;
                EditorGUI.LabelField (rect, content);

                rect.width = EditorStyles.label.CalcSize (content).x;
                float delta = EditorEx.DragValue (rect, 0, 0.01f);

                using (new LabelWidthScope (14)) {
                    float newFrom = EditorGUI.FloatField (fromRect, "F", from + delta);
                    float newTo = EditorGUI.FloatField (toRect, "T", to + delta);

                    fromChanged = from != newFrom;
                    from = newFrom;

                    toChanged = to != newTo;
                    to = newTo;

                    toggleChanged = toggle != newToggle;
                    toggle = newToggle;
                }
            }
        }

        public abstract class Editor : UnityEditor.Editor {
            public abstract void OnInsPectorGUI (Tween tween);
        }

        protected abstract class Editor<T> : Editor where T : TweenAnimation {
            protected new T target => base.target as T;
            SerializedProperty m_InterpolatorPro;
            GenericMenu m_OptionsMenu;

            protected virtual void OnEnable () {
                m_InterpolatorPro = serializedObject.FindProperty ("m_Interpolator");
            }

            protected virtual void InitOptionsMenu (GenericMenu menu, Tween tween) {
                menu.AddItem (new GUIContent ("Reset"), false, () => { Undo.RecordObject (target, "Reset"); target.Reset (); });
                menu.AddItem (new GUIContent ("Remove"), false, () => tween.UndoRemoveAnimation (target));
            }

            public sealed override void OnInsPectorGUI (Tween tween) {
                var rect = EditorGUILayout.GetControlRect ();
                var rect1 = rect;

                // --------------check(button checkbox option)-----------------
                // foldout button
                using (var scope = new ChangeCheckScope (target)) {
                    rect1.width = rect.height;
                    bool result = GUI.Toggle (rect1, target.m_Foldout, GUIContent.none, EditorStyles.foldout);
                    if (scope.Changed) target.m_Foldout = result;
                }

                // endabled checkbox
                using (var scope = new ChangeCheckScope (target)) {
                    rect1.x = rect1.xMax;
                    bool result = EditorGUI.ToggleLeft (rect1, GUIContent.none, target.enabled);
                    if (scope.Changed) target.enabled = result;
                }

                // load light skin
                var optionsIcon = (Texture2D) EditorGUIUtility.Load ("Builtin Skins/LightSkin/Images/pane options.png");

                // name
                rect1.x = rect1.xMax;
                rect1.xMax = rect.xMax - optionsIcon.width;
                EditorGUI.LabelField (rect1, allTypeDic[typeof (T)].name, EditorStyles.boldLabel);

                // options droplist
                rect.Set (rect1.xMax, rect.y + 4, optionsIcon.width, optionsIcon.height);
                var content = new GUIContent ();
                content.image = optionsIcon;
                if (GUI.Button (rect, content, GUIStyle.none)) {
                    if (m_OptionsMenu == null) {
                        m_OptionsMenu = new GenericMenu ();
                        InitOptionsMenu (m_OptionsMenu, tween);
                    }

                    m_OptionsMenu.DropDown (rect);
                }

                rect = EditorGUILayout.GetControlRect (false, 3);
                rect.xMin += EditorGUIUtility.singleLineHeight * 2;
                rect.xMax -= EditorGUIUtility.singleLineHeight * 2;

                // ----------------progress bar-------------------
                // set bg
                EditorGUI.DrawRect (rect, Tween.Editor.ProgressBackgroundInvalid);

                rect1.Set (rect.x + target.MinNormalizedTime * rect.width, rect.y,
                    Mathf.Max (1, rect.width * (target.MaxNormalizedTime - target.MinNormalizedTime)), rect.height);

                // set play progress
                if (target.enabled) {
                    rect.width = Mathf.Round (rect.width * tween.NormalizedTime);
                    EditorGUI.DrawRect (rect, Tween.Editor.ProgressForegroundInvalid);
                }

                EditorGUI.DrawRect (rect1, Tween.Editor.ProgressBackgroundValid);

                // set tween range
                if (target.enabled) {
                    // get intersection
                    if (rect.xMin > rect1.xMin) rect1.xMin = rect.xMin;
                    if (rect.xMax < rect1.xMax) rect1.xMax = rect.xMax;
                    if (rect.xMin > rect1.xMin) rect1.xMin = rect.xMin;
                    if (rect.xMax < rect1.xMax) rect1.xMax = rect.xMax;

                    if (rect1.width > 0) EditorGUI.DrawRect (rect1, Tween.Editor.ProgressForegroundValid);
                }

                GUILayout.Space (4);

                // ------------------foldout------------------------
                if (target.m_Foldout) {
                    // config range
                    using (var scope = new ChangeCheckScope (target)) {
                        float min = target.MinNormalizedTime * tween.Duration;
                        float max = target.MaxNormalizedTime * tween.Duration;

                        FromToFieldLayout ("Time Range", ref min, ref max, out bool fromChanged, out bool toChanged);

                        if (scope.Changed) {
                            if (fromChanged) target.MinNormalizedTime = Mathf.Min (min / tween.Duration, target.MaxNormalizedTime);
                            if (toChanged) target.MaxNormalizedTime = Mathf.Max (max / tween.Duration, target.MinNormalizedTime);
                        }
                    }

                    serializedObject.Update ();
                    EditorGUILayout.PropertyField (m_InterpolatorPro);
                    // show properties of children class
                    OnPropertiesGUI (tween);
                    GUILayout.Space (4);
                    serializedObject.ApplyModifiedProperties ();
                }

                EditorGUI.DrawRect (EditorGUILayout.GetControlRect (false, 1), EditorStyles.centeredGreyMiniLabel.normal.textColor);
            }

            protected abstract void OnPropertiesGUI (Tween tween);
        }

    }
}
#endif