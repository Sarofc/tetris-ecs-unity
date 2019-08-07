#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public partial class Tween3DLightColor {

        // public override void Record () {
        //     base.Record ();
        //     m_Cache = targetLight.color;
        // }

        // public override void Restore () {
        //     base.Restore ();
        //     var t = targetLight.color;
        //     targetLight.color = m_Cache;

        //     targetLight.color = t;
        // }

        public override void Reset () {
            base.Reset ();
            targetLight = GetComponent<Light> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (Tween3DLightColor))]
        protected new class Editor : Editor<Tween3DLightColor> {
            private SerializedProperty m_TargetLightProp;

            protected override void OnEnable () {
                base.OnEnable ();
                m_TargetLightProp = serializedObject.FindProperty ("targetLight");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();

                EditorGUILayout.PropertyField (m_TargetLightProp);

                base.OnPropertiesGUI (tween);
            }
        }

    }
}
#endif