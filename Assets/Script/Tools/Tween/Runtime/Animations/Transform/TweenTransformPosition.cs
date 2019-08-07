using UnityEditor;
using UnityEngine;
namespace Saro {

    [TweenAnimation ("Transform/Position", "Transfrom Position")]
    public partial class TweenTransformPosition : TweenVector3 {

        public Transform targetTransform;
        public Space coordinate = Space.Self;
        public override Vector3 Current {
            get {
                if (targetTransform) return coordinate == Space.Self ?
                    targetTransform.localPosition :
                    targetTransform.position;

                return default;
            }
            set {
                if (targetTransform) {
                    if (coordinate == Space.Self)
                        targetTransform.localPosition = value;
                    else
                        targetTransform.position = value;
                }
            }
        }
    }
}