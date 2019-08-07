using UnityEditor;

namespace Saro {
    public abstract partial class TweenFloat : TweenFormTo<float> {
        protected override void OnInterpolate (float f) {
            Current = (to - from) * f + from;
        }

    }
}