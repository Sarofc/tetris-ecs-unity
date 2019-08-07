#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {

    public partial class TweenTransformRotation : TweenQuaternion {

        public override void Reset () {
            base.Reset ();
            targetTransform = GetComponent<Transform> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (TweenTransformRotation))]
        protected new class Editor : Editor<TweenTransformRotation> {
            private SerializedProperty m_TargetTransformProp;
            private SerializedProperty m_CoordinateProp;

            protected override void OnEnable () {
                base.OnEnable ();

                m_TargetTransformProp = serializedObject.FindProperty ("targetTransform");
                m_CoordinateProp = serializedObject.FindProperty ("coordinate");
            }

            protected override void OnPropertiesGUI (Tween tween) {

                EditorGUILayout.Space ();

                EditorGUILayout.PropertyField (m_TargetTransformProp);
                EditorGUILayout.PropertyField (m_CoordinateProp);

                base.OnPropertiesGUI (tween);
            }

        }
    }
}
#endif