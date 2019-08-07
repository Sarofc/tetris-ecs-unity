using UnityEditor;
using UnityEngine;

namespace Saro {
    [TweenAnimation ("Rendering/3D Light Intensity", "Light Intensity")]
    public partial class Tween3DLightIntensity : TweenFloat {
        public Light targetLight;
        public override float Current {
            get {
                if (targetLight) return targetLight.intensity;
                return 1f;
            }
            set {
                if (targetLight) targetLight.intensity = value;
            }
        }

    }
}