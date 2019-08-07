#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Saro {

    public partial class TweenTMPTextFontSize {

        public override void Reset () {
            base.Reset ();
            targetText = GetComponent<TMP_Text> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (TweenTMPTextFontSize))]
        new class Editor : Editor<TweenTMPTextFontSize> {
            SerializedProperty targetTextProp;
            protected override void OnEnable () {
                base.OnEnable ();
                targetTextProp = serializedObject.FindProperty ("targetText");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (targetTextProp);

                base.OnPropertiesGUI (tween);
            }
        }

    }
}
#endif