using UnityEngine;

namespace Saro {
    [TweenAnimation ("2D/CanvasGroup Alpha", "CanvasGroup Alpha")]
    public partial class TweenCanvasGroupAlpha : TweenFloat {

        public CanvasGroup targetCanvasGroup;

        public override float Current {
            get {
                if (targetCanvasGroup) return targetCanvasGroup.alpha;
                return 1f;
            }
            set {
                if (targetCanvasGroup) targetCanvasGroup.alpha = value;
            }
        }

    }
}