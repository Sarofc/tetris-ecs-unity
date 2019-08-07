using UnityEditor;
using UnityEngine;

namespace Saro {
    public abstract partial class TweenColor : TweenFormTo<Color> {
        public bool useGradient;
        public Gradient gradient;

        protected override void OnInterpolate (float factor) {
            var t = Current;

            if (useGradient) {
                var c = gradient.Evaluate (factor);

                t.r = c.r;
                t.g = c.g;
                t.b = c.b;
                t.a = c.a;
            } else {
                t.r = (to.r - from.r) * factor + from.r;
                t.g = (to.g - from.g) * factor + from.g;
                t.b = (to.b - from.b) * factor + from.b;
                t.a = (to.a - from.a) * factor + from.a;
            }

            Current = t;
        }



    }
}