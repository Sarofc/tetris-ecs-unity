using UnityEditor;
using UnityEngine;
namespace Saro {

    [TweenAnimation ("Transform/Scale", "Transfrom Scale")]
    public partial class TweenTransformScale : TweenVector3 {

        public Transform targetTransform;
        public override Vector3 Current {
            get {
                if (targetTransform) return targetTransform.localPosition;
                return Vector3.one;
            }
            set {
                if (targetTransform) targetTransform.localPosition = value;
            }
        }
    }
}