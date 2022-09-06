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
    [UIWindow((int)EGameUI.UISettingPanel, "Assets/Res/Prefabs/UI/UISetting.prefab")]
    public sealed partial class UISetting : UIWindow
    {
        public UISetting(string path) : base(path)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            slider_bgm.value = AudioManager.Current.VolumeBGM;
            slider_se.value = AudioManager.Current.VolumeSE;
            tmpdrop_language.value = (int)LocalizationManager.Current.CurrentLanguage;

            Listen(btn_Mask.onClick, OnClick_Close);
            Listen(slider_bgm.onValueChanged, OnBGMChanged);
            Listen(slider_se.onValueChanged, OnSEChanged);
            Listen(tmpdrop_language.onValueChanged, OnLanguageChanged);
        }

        protected override void OnHide()
        {
            base.OnHide();

            Save();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Save();
        }

        private void Save()
        {
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
            UIManager.Current.UnLoadWindow(EGameUI.UISettingPanel);
        }
    }


    public partial class UISetting
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope
        //>>begin
        public Button btn_Mask => Binder.GetRef<Button>("btn_Mask");
        public Slider slider_bgm => Binder.GetRef<Slider>("slider_bgm");
        public Slider slider_se => Binder.GetRef<Slider>("slider_se");
        public TMP_Dropdown tmpdrop_language => Binder.GetRef<TMP_Dropdown>("tmpdrop_language");

        //<<end
        // =============================================
    }
}