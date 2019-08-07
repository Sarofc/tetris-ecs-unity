using TMPro;
using UnityEngine;

namespace Saro
{
    [TweenAnimation("2D/TMPText FontSize", "TMPText FontSize")]
    public partial class TweenTMPTextFontSize : TweenFloat
    {
        public TMP_Text targetText;

        public override float Current
        {
            get
            {
                if (targetText) return targetText.fontSize;
                return 16;
            }
            set
            {
                if (targetText) targetText.fontSize = value;
            }
        }
    }
}