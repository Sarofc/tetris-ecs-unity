
namespace Saro.Localization
{
    [UnityEngine.RequireComponent(typeof(TMPro.TMP_Text))]
    public class TMP_TextLocalized : ALocalized<TMPro.TMP_Text>
    {
        protected override void OnValueChanged()
        {
            m_Target.text = m_Localization.GetValue(m_Key);
        }
    }
}
