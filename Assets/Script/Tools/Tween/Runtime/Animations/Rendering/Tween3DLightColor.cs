using UnityEngine;

namespace Saro
{
    [TweenAnimation ("Rendering/3D Light Color", "Light Color")]
    public partial class Tween3DLightColor : TweenColor {
        public override Color Current {
            get {
                if (targetLight) return targetLight.color;
                return new Color (1, 1, 1);
            }
            set {
                if (targetLight) targetLight.color = value;
            }
        }


        public Light targetLight;



    }
}