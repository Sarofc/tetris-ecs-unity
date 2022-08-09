using System;
using Cysharp.Threading.Tasks;
using Saro;
using Saro.Audio;
using Saro.Localization;
using Saro.UI;
using Tetris.Save;
using TMPro;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)ETetrisUI.SettingPanel, "Assets/Res/Prefab/UI/UISetting.prefab")]
    public sealed partial class UISetting : UIWindow
    {
        public UISetting(string path) : base(path)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            SliderBGM.value = AudioManager.Current.VolumeBGM;
            SliderSe.value = AudioManager.Current.VolumeSE;
            TmpdropLanguage.value = (int)LocalizationManager.Current.CurrentLanguage;

            Listen(BtnMask.onClick, OnClick_Close);
            Listen(SliderBGM.onValueChanged, OnBGMChanged);
            Listen(SliderSe.onValueChanged, OnSEChanged);
            Listen(TmpdropLanguage.onValueChanged, OnLanguageChanged);
        }

        protected override void OnHide()
        {
            base.OnHide();

            AudioManager.Current.StoreSettings();
            LocalizationManager.Current.StoreSettings();

            try
            {
                SaveManager.Current.Save();
            }
            catch (Exception e)
            {
                Log.ERROR(e);
            }
        }

        private void OnLanguageChanged(int val)
        {
            LocalizationManager.Current.SetLanguageAsync((ELanguage)val).Forget();
        }

        private void OnBGMChanged(float val)
        {
            AudioManager.Current.VolumeBGM = val;
        }

        private void OnSEChanged(float val)
        {
            AudioManager.Current.VolumeSE = val;
        }

        private void OnClick_Close()
        {
            UIManager.Instance.UnLoadWindow(ETetrisUI.SettingPanel);
        }
    }


    public partial class UISetting
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope
        //>>begin
        public Button BtnMask => Binder.GetRef<Button>("btn_Mask");
        public Slider SliderBGM => Binder.GetRef<Slider>("slider_bgm");
        public Slider SliderSe => Binder.GetRef<Slider>("slider_se");
        public TMP_Dropdown TmpdropLanguage => Binder.GetRef<TMP_Dropdown>("tmpdrop_language");

        //<<end
        // =============================================
    }
}