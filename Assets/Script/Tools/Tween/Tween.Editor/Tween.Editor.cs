#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saro {
    public partial class Tween {
        private static List<Tween> m_TweenList = new List<Tween> ();
        private static List<TweenAnimation> m_AnimationList = new List<TweenAnimation> ();
        private static List<GameObject> m_ObjectList = new List<GameObject> ();

        [SerializeField] bool m_FoldoutController = true;
        [SerializeField] bool m_Foldoutevents = false;

        private bool m_Preview;
        private bool m_Dragging;
        private bool m_EnableRecord;
        private float m_NormalizedTimeRecord;
        private PlayDirection m_DirectionRecord;

        private void RecordAll () {
            m_EnableRecord = enabled;
            m_NormalizedTimeRecord = NormalizedTime;
            m_DirectionRecord = playDirection;

            if (m_Animations == null) return;

            for (int i = 0; i < m_Animations.Count; i++)
                m_Animations[i]?.Record ();
        }

        private void RestoreAll () {
            enabled = m_EnableRecord;
            NormalizedTime = m_NormalizedTimeRecord;
            playDirection = m_DirectionRecord;

            if (m_Animations == null) return;
            for (int i = 0; i < m_Animations.Count; i++) {
                m_Animations[i]?.Restore ();
            }
        }

        private bool Playing {
            get {
                if (Application.isPlaying) return enabled;
                else return m_Preview;
            }
            set {
                if (Application.isPlaying) enabled = value;
                else {
                    if (m_Preview != value) {
                        m_Preview = value;

                        if (value) {
                            EditorApplication.update += OnUpdate;
                            EditorApplication.quitting += StopPreview;
                            EditorApplication.playModeStateChanged += StopPreviewInEditMode;

                            RecordAll ();
                        } else {
                            EditorApplication.update -= OnUpdate;
                            EditorApplication.quitting -= StopPreview;
                            EditorApplication.playModeStateChanged -= StopPreviewInEditMode;

                            RestoreAll ();
                        }
                    }
                }

                void StopPreview () { Playing = false; }
                void StopPreviewInEditMode (PlayModeStateChange msg) { if (msg == PlayModeStateChange.ExitingEditMode) Playing = false; }
            }
        }

        private bool Dragging {
            get { return m_Dragging; }
            set {
                if (m_Dragging != value) {
                    m_Dragging = value;

                    if (value) {
                        if (Playing) RecordAll ();
                    } else {
                        if (!Playing) RestoreAll ();
                    }
                }
            }
        }

        private static bool AnyContainsInTweenList (TweenAnimation target) {
            for (int i = 0; i < m_TweenList.Count; i++) {
                if (m_TweenList[i].m_Animations != null && m_TweenList[i].m_Animations.Contains (target)) {
                    return true;
                }
            }
            return false;
        }

        private static void RemoveAllUnusedAnimations (GameObject target) {
            target.GetComponents (m_TweenList);
            target.GetComponents (m_AnimationList);

            for (int i = 0; i < m_AnimationList.Count; i++) {
                if (!AnyContainsInTweenList (m_AnimationList[i])) {
                    // TODO Logger
                    Debug.LogError("Remove Unused TweenAnimation : " + m_AnimationList[i]);
                    Undo.DestroyObjectImmediate (m_AnimationList[i]);
                }
            }

            m_AnimationList.Clear ();
            m_TweenList.Clear ();
        }

        [InitializeOnLoadMethod]
        private static void ClearUnusedAnimationInEditor () {
            EditorApplication.update += () => {
                for (int i = 0; i < m_ObjectList.Count; i++) {
                    if (m_ObjectList[i]) RemoveAllUnusedAnimations (m_ObjectList[i]);
                }
                m_ObjectList.Clear ();
            };
        }

        private void Reset () {
            RemoveAllUnusedAnimations (gameObject);
        }

        protected override void OnValidate () {
            base.OnValidate ();

            if (m_Animations != null && m_Animations.Count != 0) {
                GetComponents (m_TweenList);
                m_TweenList.Remove (this);
                for (int i = 0; i < m_Animations.Count; i++) {
                    if (m_Animations[i] && m_Animations[i].gameObject == gameObject && !AnyContainsInTweenList (m_Animations[i])) {
                        continue;
                    }
                    m_Animations.RemoveAt (i--);
                }
                m_TweenList.Clear ();
            }
            m_ObjectList.Add (gameObject);
        }

        public void UndoRemoveAnimation (TweenAnimation anim) {
            Undo.RecordObject (this, "RemoveAnimation");
            m_Animations.Remove (anim);
            Undo.DestroyObjectImmediate (anim);
        }

        [CustomEditor (typeof (Tween))]
        [CanEditMultipleObjects]
        public class Editor : BaseEditor<Tween> {
            private static GUIStyle m_ImageButtonSytle;
            public static GUIStyle ImageButtonSytle {
                get {
                    if (m_ImageButtonSytle == null) {
                        m_ImageButtonSytle = new GUIStyle (EditorStyles.miniButton);
                        m_ImageButtonSytle.padding = new RectOffset (0, 0, 0, 0);
                    }
                    return m_ImageButtonSytle;
                }
            }

            public static Color ProgressBackgroundInvalid {
                get { return EditorGUIUtility.isProSkin ? new Color () : new Color (0, 0, 0, 0.6f); }
            }

            public static Color ProgressBackgroundValid {
                get { return EditorGUIUtility.isProSkin ? new Color () : new Color (0, 1, 0.2f, 0.5f); }
            }

            public static Color ProgressForegroundInvalid {
                get { return EditorGUIUtility.isProSkin ? new Color () : new Color (0.8f, 0.8f, 1, 0.6f); }
            }

            public static Color ProgressForegroundValid {
                get { return EditorGUIUtility.isProSkin ? new Color () : new Color (0.2f, 1f, 0, 1); }
            }

            private SerializedProperty m_DurationProp;
            private SerializedProperty m_UpdateModeProp;
            private SerializedProperty m_TimeModePorp;
            private SerializedProperty m_WrapModePorp;
            private SerializedProperty m_ArrivedActionProp;
            private SerializedProperty m_OnForwardArriedProp;
            private SerializedProperty m_OnBackwardArrivedProp;

            private List<UnityEditor.Editor> m_Editors = new List<UnityEditor.Editor> ();
            private GenericMenu m_AddMenu;

            // use refection to get TweenAnimations, and Add them to m_Animations collection
            private void ShowAddMenu (Rect rect) {
                if (m_AddMenu == null) {
                    m_AddMenu = EditorEx.CreateMenu (
                        TweenAnimation.allTypeDic.Keys,
                        t => new GUIContent (TweenAnimation.allTypeDic[t].menu),
                        t => MenuItemState.Normal,
                        t => {
                            var tmp = Undo.AddComponent (target.gameObject, t) as TweenAnimation;
                            Undo.RecordObject (tmp, "AddComponent");
                            tmp.Reset ();

                            Undo.RecordObject (target, "AddComponent");
                            target.AddAnimationInternal (tmp);
                        }
                    );
                }

                m_AddMenu.DropDown (rect);
            }

            private void OnEnable () {
                m_DurationProp = serializedObject.FindProperty ("m_Duration");
                m_UpdateModeProp = serializedObject.FindProperty ("m_UpdateMode");
                m_TimeModePorp = serializedObject.FindProperty ("m_TimeMode");
                m_WrapModePorp = serializedObject.FindProperty ("m_WrapMode");
                m_ArrivedActionProp = serializedObject.FindProperty ("m_ArrivedAction");

                m_OnForwardArriedProp = serializedObject.FindProperty ("m_OnForwardArrived");
                m_OnBackwardArrivedProp = serializedObject.FindProperty ("m_OnBackwardArrived");
            }

            private void OnDisable () {
                if (!Application.isPlaying) {
                    if (target) target.Playing = false;
                }
            }

            private void OnDestroy () {

                for (int i = 0; i < m_Editors.Count; i++) {
                    if (m_Editors[i] != null) {

                        if (!Application.isPlaying) {
                            DestroyImmediate (m_Editors[i]);
                        } else

                            Destroy (m_Editors[i]);
                    }
                }
            }

            public override bool RequiresConstantRepaint () {
                if (Application.isPlaying) return target.isActiveAndEnabled;
                else return target.m_Preview;
            }

            public override void OnInspectorGUI () {
                EditorGUILayout.BeginVertical (EditorStyles.helpBox);

                var rect = EditorGUILayout.GetControlRect ();
                var rect1 = rect;

                // controll foldout
                using (var scope = new ChangeCheckScope (target)) {
                    rect1.width = rect1.height;
                    bool result = GUI.Toggle (rect1, target.m_FoldoutController, GUIContent.none, EditorStyles.foldout);
                    if (scope.Changed) target.m_FoldoutController = result;
                }

                // controll label
                rect.xMin = rect1.xMax;
                EditorGUI.LabelField (rect, "Controller", EditorStyles.boldLabel);

                serializedObject.Update ();

                // controll settings
                if (target.m_FoldoutController) {
                    EditorGUILayout.PropertyField (m_DurationProp);
                    EditorGUILayout.PropertyField (m_UpdateModeProp);
                    EditorGUILayout.PropertyField (m_TimeModePorp);
                    EditorGUILayout.PropertyField (m_WrapModePorp);
                    EditorGUILayout.PropertyField (m_ArrivedActionProp);

                    GUILayout.Space (4);
                }

                EditorGUI.DrawRect (EditorGUILayout.GetControlRect (false, 1), EditorStyles.centeredGreyMiniLabel.normal.textColor);
                GUILayout.Space (4);

                rect = EditorGUILayout.GetControlRect ();
                rect1 = rect;

                // play button
                rect1.width = EditorGUIUtility.singleLineHeight * 2 - 4;

                var content = new GUIContent ();
                content.text = "Play";

                using (new GUIContentColorScope (target.Playing? ProgressForegroundValid : EditorStyles.label.normal.textColor)) {

                    target.Playing = GUI.Toggle (rect1, target.Playing, content, ImageButtonSytle);
                }

                // direction button
                rect1.x = rect.xMax - rect1.width;

                content.text = target.playDirection == PlayDirection.Forward ? "->" : "<-";

                using (new DisabledScope (!target.Playing)) {
                    using (new GUIContentColorScope (EditorStyles.label.normal.textColor)) {
                        if (GUI.Button (rect1, content, ImageButtonSytle)) {
                            target.ReverseDirection ();
                        }
                    }
                }

                rect.xMin += EditorGUIUtility.singleLineHeight * 2;
                rect.xMax -= EditorGUIUtility.singleLineHeight * 2;

                // start of dragging
                if (Event.current.type == EventType.MouseDown) {
                    if (rect.Contains (Event.current.mousePosition)) target.Dragging = true;
                }

                // end of dragging
                if (Event.current.rawType == EventType.MouseUp) {
                    if (target.Dragging) {
                        target.Dragging = false;
                        Repaint ();
                    }
                }

                // progress bar
                using (var scope = new ChangeCheckScope (null)) {
                    float progress = EditorEx.ProgressBar (rect, target.NormalizedTime, ProgressBackgroundInvalid, ProgressForegroundValid);
                    if (scope.Changed && target.Dragging) {
                        target.NormalizedTime = progress;
                    }
                }

                GUILayout.Space (4);
                EditorGUILayout.EndVertical ();
                GUILayout.Space (4);

                EditorGUILayout.BeginVertical (EditorStyles.helpBox);

                rect = EditorGUILayout.GetControlRect ();
                rect1 = rect;

                // events foldout
                using (var scope = new ChangeCheckScope (target)) {
                    rect1.width = rect1.height;
                    bool result = GUI.Toggle (rect1, target.m_Foldoutevents, GUIContent.none, EditorStyles.foldout);
                    if (scope.Changed) target.m_Foldoutevents = result;
                }

                // events label
                rect.xMin = rect1.xMax;
                EditorGUI.LabelField (rect, "Events", EditorStyles.boldLabel);

                // events
                if (target.m_Foldoutevents) {
                    EditorGUILayout.PropertyField (m_OnForwardArriedProp);
                    GUILayout.Space (2);
                    EditorGUILayout.PropertyField (m_OnBackwardArrivedProp);
                    GUILayout.Space (2);
                }

                serializedObject.ApplyModifiedProperties ();

                EditorGUILayout.EndVertical ();
                GUILayout.Space (4);
                EditorGUILayout.BeginVertical (EditorStyles.helpBox);

                // draw animation list
                if (target.m_Animations != null && target.m_Animations.Count != 0) {
                    int editorIndex = 0;
                    for (int i = 0; i < target.m_Animations.Count; i++) {
                        var anim = target.m_Animations[i];
                        if (anim) {
                            if (m_Editors.Count <= editorIndex) m_Editors.Add (null);

                            if (!m_Editors[editorIndex] || m_Editors[editorIndex].target != anim) {

                                if (!Application.isPlaying)
                                    Object.DestroyImmediate (m_Editors[editorIndex]);
                                else

                                    Object.Destroy (m_Editors[editorIndex]);

                                m_Editors[editorIndex] = CreateEditor (anim);
                            }

                            (m_Editors[editorIndex] as TweenAnimation.Editor).OnInsPectorGUI (target);
                            editorIndex++;
                        } else {
                            target.m_Animations.RemoveAt (i--);
                        }
                    }

                    for (; editorIndex < m_Editors.Count; editorIndex++) {


                        if (!Application.isPlaying)
                            Object.DestroyImmediate (m_Editors[editorIndex]);
                        else

                            Object.Destroy (m_Editors[editorIndex]);

                        m_Editors.RemoveAt (editorIndex--);

                    }

                }

                // add button
                GUILayout.Space (4);
                var btnRect = EditorGUILayout.GetControlRect ();
                if (GUI.Button (btnRect, "Add Animation..", EditorStyles.miniButton)) {
                    ShowAddMenu (btnRect);
                }
                GUILayout.Space (4);

                EditorGUILayout.EndVertical ();
            }
        }

    }
}
#endif