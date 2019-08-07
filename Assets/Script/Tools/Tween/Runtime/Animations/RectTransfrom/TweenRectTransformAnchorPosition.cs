using UnityEngine;

namespace Saro {
    [TweenAnimation ("RectTransform/RectTransform AnchorPostion", "RectTransform AnchorPostion")]
    public partial class TweenRectTransformAnchorPosition : TweenVector2 {
        public RectTransform targetRectTransform;

        public override Vector2 Current {
            get {
                if (targetRectTransform) return targetRectTransform.anchoredPosition;
                return default;
            }
            set {
                if (targetRectTransform) targetRectTransform.anchoredPosition = value;
            }

        }
    }
}