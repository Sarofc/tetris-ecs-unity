#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {

    public partial class Tween3DLightIntensity {

        // public override void Record () {
        //     m_Cache = targetLight.intensity;
        //     base.Record ();
        // }

        // public override void Restore () {
        //     var t = targetLight.intensity;
        //     targetLight.intensity = m_Cache;
        //     targetLight.intensity = t;
        //     base.Restore ();
        // }

        public override void Reset () {
            base.Reset ();
            targetLight = GetComponent<Light> ();
            from = Current;
            to = Current;
        }

        [CustomEditor (typeof (Tween3DLightIntensity))]
        new class Editor : Editor<Tween3DLightIntensity> {
            SerializedProperty targetLightProp;
            protected override void OnEnable () {
                base.OnEnable ();
                targetLightProp = serializedObject.FindProperty ("targetLight");
            }

            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (targetLightProp);

                base.OnPropertiesGUI (tween);
            }
        }

    }
}
#endif