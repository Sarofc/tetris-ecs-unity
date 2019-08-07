using UnityEngine;
namespace Saro {

    public abstract partial class TweenQuaternion : TweenFormTo<Quaternion> {
        protected override void OnInterpolate (float f) {
            Current = Quaternion.SlerpUnclamped (from, to, f);
        }
    }
}