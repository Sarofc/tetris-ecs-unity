using UnityEditor;
using UnityEngine;

namespace Saro {
    public abstract  partial class TweenVector3 : TweenFormTo<Vector3> {
        protected override void OnInterpolate (float f) {
            var t = Current;

            t.x = (to.x - from.x) * f + from.x;
            t.y = (to.y - from.y) * f + from.y;
            t.z = (to.z - from.z) * f + from.z;

            Current = t;
        }


    }
}