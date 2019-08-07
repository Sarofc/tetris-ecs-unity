#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public partial class TweenRectTransformAnchorPosition {
        public override void Reset () {
            base.Reset ();
            targetRectTransform = GetComponent<RectTransform> ();
        }

        [CustomEditor (typeof (TweenRectTransformAnchorPosition))]
        protected new class Editor : Editor<TweenRectTransformAnchorPosition> {
            private SerializedProperty m_TargetRectTransfromProp;

            protected override void OnEnable () {
                base.OnEnable ();
                m_TargetRectTransfromProp = serializedObject.FindProperty ("targetRectTransform");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (m_TargetRectTransfromProp);
                base.OnPropertiesGUI (tween);
            }
        }
    }
}
#endif