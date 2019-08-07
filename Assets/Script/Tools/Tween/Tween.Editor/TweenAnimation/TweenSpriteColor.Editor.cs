#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {

    public partial class TweenSpriteColor {
        public override void Reset () {
            base.Reset ();
            targetRenderer = GetComponent<SpriteRenderer> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (TweenSpriteColor))]
        protected new class Editor : Editor<TweenSpriteColor> {
            private SerializedProperty m_TargetRendererProp;

            protected override void OnEnable () {
                base.OnEnable ();
                m_TargetRendererProp = serializedObject.FindProperty ("targetRenderer");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (m_TargetRendererProp);
                base.OnPropertiesGUI (tween);
            }
        }
    }
}
#endif