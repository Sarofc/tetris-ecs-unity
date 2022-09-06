using TMPro;
using UnityEngine;

namespace Saro.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextLocalized : ALocalized<TMP_Text>
    {
        protected override void OnValueChanged()
        {
            m_Target.text = m_Localization.GetValue(m_Key);
        }
    }
}