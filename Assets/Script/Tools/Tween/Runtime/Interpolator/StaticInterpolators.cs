using UnityEngine;

namespace Saro {
    public partial struct Interpolator {
        public static float Linear (float t) {
            return t;
        }

        public static float Accelerate (float t) {
            return t * t;
        }

        public static float AccelerateWeakly (float t) {
            return t * t * (2f - t);
        }

        public static float AccelerateStrongly (float t) {
            return t * t * t;
        }

        public static float Accelerate (float t, float strength) {
            return t * t * ((2f - t) * (1f - strength) + t * strength);
        }

        public static float Decelerate (float t) {
            t = 1f - t;
            return 1f - t * t * (2f - t);
        }

        public static float DecelerateWeakly (float t) {
            //TODO
            return 0f;
        }

        public static float DecelerateStrongly (float t) {
            //TODO
            return 0f;
        }

        public static float Decelerate (float t, float strength) {
            t = 1f - t;
            return 1f - t * t * ((2 - t) * (1f - strength) + t * strength);
        }

        public static float AccelerateDecelerate (float t, float strength) {
            float tt = t * t;
            float ttt6_15tt = (6f * t - 15f) * tt;
            return ((6f - ttt6_15tt - 14f * t) * (1f - strength) + (ttt6_15tt + 10f * t) * strength) * tt;
        }

        public static float Anticipate (float t, float strength = .5f) {
            float a = 2f + strength * 2f;
            return (a * t - a + 1f) * t * t;
        }

        public static float Overshoot (float t, float strength = .5f) {
            t = 1f - t;
            float a = 2f + strength * 2f;
            return 1f - (a * t - a + 1f) * t * t;
        }

        public static float AnticipateOvershoot (float t, float strength) {
            float d = -6f - 12f * strength;
            return ((((6f - d - d) * t + (5f * d - 15f)) * t + (10f - 4f * d)) * t + d) * t * t;
        }

        public static float Bounce (float t, float strength = 0.5f) {
            float k = 0.3f + 0.4f * strength;
            float kk = k * k;
            float a = 1f + (k + k) * (1f + k + kk);

            float tmp;

            if (t < 1f / a) {
                tmp = a * t;
                return tmp * tmp;
            }
            if (t < (1f + k + k) / a) {
                tmp = a * t - 1f - k;
                return 1f - kk + tmp * tmp;
            }
            if (t < (1f + (k + kk) * 2f) / a) {
                tmp = a * t - 1f - k - k - kk;
                return 1f - kk * kk + tmp * tmp;
            }

            tmp = a * t - 1f - 2 * (k + kk) - kk * k;
            return 1f - kk * kk * kk + tmp * tmp;
        }

        public static float Parabolic (float t) {
            return 4f * t * (1f - t);
        }

        public static float Sine (float t) {
            return Mathf.Sin ((t + t + 1.5f) * Mathf.PI) * 0.5f + 0.5f;
        }
    }
}