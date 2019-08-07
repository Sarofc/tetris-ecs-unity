using UnityEngine;

namespace Saro {
    [TweenAnimation ("2D/Sprite Color", "Sprite Color")]
    public partial class TweenSpriteColor : TweenColor {
        public SpriteRenderer targetRenderer;

        public override Color Current {
            get {
                if (targetRenderer) return targetRenderer.color;
                return new Color (1, 1, 1);
            }
            set {
                if (targetRenderer) targetRenderer.color = value;
            }
        }
    }
}