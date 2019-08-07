using UnityEditor;
using UnityEngine;
namespace Saro {
    [TweenAnimation ("Transform/EulerAngles", "Transfrom EulerAngles")]
    public partial class TweenTransformEulerAngles : TweenVector3 {
        public Transform targetTransform;
        public Space coordinate = Space.Self;
        public override Vector3 Current {
            get {
                if (targetTransform) return coordinate == Space.Self ?
                    targetTransform.localEulerAngles :
                    targetTransform.eulerAngles;

                return default;
            }
            set {
                if (targetTransform) {
                    if (coordinate == Space.Self)
                        targetTransform.localEulerAngles = value;
                    else
                        targetTransform.eulerAngles = value;
                }
            }
        }
    }
}