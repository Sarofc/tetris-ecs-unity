using UnityEngine;
namespace Saro {

    public abstract partial class TweenVector2 : TweenFormTo<Vector2> {
        protected override void OnInterpolate (float f) {

            var t = Current;
            t.x = (to.x - from.x) * f + from.x;
            t.y = (to.y - from.y) * f + from.y;

            Current = t;
        }
    }
}