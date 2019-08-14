using UnityEngine;
using System.Collections;
using TMPro;

public class RenText : MonoBehaviour
{
    private Saro.Tween tween;
    private TMP_Text text;

    public void RestartTween(int count)
    {
        if (!tween) tween = GetComponent<Saro.Tween>();
        if (!text) text = GetComponent<TMP_Text>();

        text.text = string.Format("Ren\n{0}", count.ToString());
        tween.ResetNormalizedTime();
        tween.enabled = true;
    }
}
