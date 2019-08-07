#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {

    public partial class TweenTransformScale : TweenVector3 {

        public override void Reset () {
            base.Reset ();
            targetTransform = GetComponent<Transform> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (TweenTransformScale))]
        protected new class Editor : Editor<TweenTransformScale> {
            private SerializedProperty m_TargetTransformProp;

            protected override void OnEnable () {
                base.OnEnable ();

                m_TargetTransformProp = serializedObject.FindProperty ("targetTransform");
            }

            protected override void OnPropertiesGUI (Tween tween) {

                EditorGUILayout.Space ();

                EditorGUILayout.PropertyField (m_TargetTransformProp);

                base.OnPropertiesGUI (tween);
            }
        }
    }
}
#endif