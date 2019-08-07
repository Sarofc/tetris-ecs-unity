namespace Saro {
    using UnityEditor;
    using UnityEngine;

    [TweenAnimation ("Transform/Rotation", "Transfrom Rotation")]
    public partial class TweenTransformRotation : TweenQuaternion {
        public Transform targetTransform;
        public Space coordinate = Space.Self;

        public override Quaternion Current {
            get {
                if (targetTransform) return coordinate == Space.Self ?
                    targetTransform.localRotation :
                    targetTransform.rotation;

                return Quaternion.identity;
            }

            set {
                if (targetTransform) {
                    if (coordinate == Space.Self) {
                        targetTransform.localRotation = value;
                    } else {
                        targetTransform.rotation = value;
                    }
                }
            }
        }

    }
}