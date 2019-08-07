#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Saro {
    public partial class TweenTMPTextColor {
        public override void Reset () {
            base.Reset ();

            targetText = GetComponent<TMP_Text> ();
        }

        [CustomEditor (typeof (TweenTMPTextColor))]
        protected new class Editor : Editor<TweenTMPTextColor> {
            private SerializedProperty m_TargetTextProp;

            protected override void OnEnable () {
                base.OnEnable ();
                m_TargetTextProp = serializedObject.FindProperty ("targetText");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();

                EditorGUILayout.PropertyField (m_TargetTextProp);

                base.OnPropertiesGUI (tween);
            }
        }
    }
}
#endif