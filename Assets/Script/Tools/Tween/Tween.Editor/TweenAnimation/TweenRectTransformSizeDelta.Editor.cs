#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public partial class TweenRectTransformSizeDelta {
        public override void Reset () {
            base.Reset ();
            targetRectTransform = GetComponent<RectTransform> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (TweenRectTransformSizeDelta))]
        protected new class Editor : Editor<TweenRectTransformSizeDelta> {
            private SerializedProperty m_TargetRectTransformProp;

            protected override void OnEnable () {
                base.OnEnable ();
                m_TargetRectTransformProp = serializedObject.FindProperty ("targetRectTransform");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (m_TargetRectTransformProp);
                base.OnPropertiesGUI (tween);
            }
        }
    }
}
#endif