using UnityEngine;
using System.Collections;
using TMPro;

public class RenText : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void RestartTween(int count)
    {
        text.text = string.Format("Ren\n{0}", count.ToString());
    }
}
