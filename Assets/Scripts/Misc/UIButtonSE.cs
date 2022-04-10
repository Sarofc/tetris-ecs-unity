using Saro.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.Misc
{
    [RequireComponent(typeof(Button))]
    public sealed class UIButtonSE : MonoBehaviour
    {
        [SerializeField]
        private AudioClip m_PressClip;

        private Button m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();

            if (m_Button)
            {
                m_Button.onClick.AddListener(OnPress);
            }
        }

        private void OnDestroy()
        {
            if (m_Button)
            {
                m_Button.onClick.RemoveListener(OnPress);
            }
        }

        private void OnPress()
        {
            SoundManager.Current.PlaySE(m_PressClip);
        }
    }
}