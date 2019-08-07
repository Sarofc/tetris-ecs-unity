using UnityEngine;

namespace Saro {
    [System.Serializable]
    public struct CustomizableInterpolator {
        public enum Type {
            Linear = 0,
            Accelerate,
            Decelerate,
            AccelerateDecelerate,
            Anticipate,
            Overshoot,
            AnticipateOvershoot,
            Bounce,
            Parabolic,
            Sine,
            CustomCurve = -1
        }

        public Type type;
        [Range (0, 1)]
        public float strength;
        public AnimationCurve customCurve;

        public float this [float t] {
            get {
                return type == Type.CustomCurve ?
                    customCurve.Evaluate (t) :
                    Interpolator.Interpolators[(int) type] (t, strength);
            }
        }

        public CustomizableInterpolator (Type type, float strength, AnimationCurve customCurve) {
            this.type = type;
            this.strength = strength;
            this.customCurve = customCurve;
        }
    }
}