using TMPro;
using UnityEngine;

namespace Saro
{
    [TweenAnimation("2D/TMPText Color", "TMPText Color")]
    public partial class TweenTMPTextColor : TweenColor
    {
        public TMP_Text targetText;

        public override Color Current
        {
            get
            {
                if (targetText) return targetText.color;
                return new Color(1, 1, 1, 1);
            }
            set
            {
                if (targetText) targetText.color = value;
            }
        }
    }
}