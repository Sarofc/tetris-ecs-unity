using System;
using UnityEngine;

namespace Saro {
    [System.Serializable]
    public partial struct Interpolator {
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
            Sine
        }

        public Type type;
        [Range (0, 1)] public float strength;

        public static readonly Func<float, float, float>[] Interpolators = {
            (t, s) => t,
            Accelerate,
            Decelerate,
            AccelerateDecelerate,
            Anticipate,
            Overshoot,
            AnticipateOvershoot,
            Bounce,
            (t, s) => Parabolic (t),
            (t, s) => Sine (t)
        };

        public float this [float t] {
            get { return Interpolators[(int) type] (t, strength); }
        }

        public Interpolator (Type type, float strength = .5f) {
            this.type = type;
            this.strength = strength;
        }
    }
}