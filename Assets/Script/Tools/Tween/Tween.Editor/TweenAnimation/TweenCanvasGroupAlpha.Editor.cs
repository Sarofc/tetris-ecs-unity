#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public partial class TweenCanvasGroupAlpha {

        public override void Reset () {
            base.Reset ();
            targetCanvasGroup = GetComponent<CanvasGroup> ();
        }

        [CustomEditor (typeof (TweenCanvasGroupAlpha))]
        protected new class Editor : Editor<TweenCanvasGroupAlpha> {
            private SerializedProperty m_TargetCanvasGroup;

            protected override void OnEnable () {
                base.OnEnable ();
                m_TargetCanvasGroup = serializedObject.FindProperty ("targetCanvasGroup");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (m_TargetCanvasGroup);
                base.OnPropertiesGUI (tween);
            }
        }
    }
}
#endif