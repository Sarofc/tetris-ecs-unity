using UnityEngine;

namespace Saro {
    [TweenAnimation ("RectTransform/RectTransform SizeDelta", "RectTransform SizeDelta")]
    public partial class TweenRectTransformSizeDelta : TweenVector2 {
        public RectTransform targetRectTransform;

        public override Vector2 Current {
            get {
                if (targetRectTransform) return targetRectTransform.sizeDelta;
                return default;
            }
            set {
                if (targetRectTransform) targetRectTransform.sizeDelta = value;
            }
        }
    }
}