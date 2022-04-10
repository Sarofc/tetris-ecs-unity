using Cysharp.Threading.Tasks;
using Saro.Audio;
using Saro.Events;
using Saro.Localization;
using Saro.UI;
using System;

namespace Tetris.UI
{
    public sealed partial class UISetting : SingletonUI<UISetting, UIBinder>
    {
        #region Impl

        protected override void InternalStart()
        {
        }

        protected override void InternalUpdate(float deltaTime)
        {
        }

        protected override void InternalClose()
        {
        }

        protected override void InternalAwake()
        {
            GetComps();

            Listen(slider_bgm.onValueChanged, OnBGMChanged);
            Listen(slider_se.onValueChanged, OnSEChanged);

            tmpdrop_language.value = (int)LocalizationManager.Current.CurrentLanguage;
            Listen(tmpdrop_language.onValueChanged, OnLanguageChanged);
        }

        private void OnLanguageChanged(int val)
        {
            LocalizationManager.Current.SetLanguageAsync((ELanguage)val).Forget();
        }

        private void OnBGMChanged(float val)
        {
            SoundManager.Current.VolumeBGM = val;
        }

        private void OnSEChanged(float val)
        {
            SoundManager.Current.VolumeSE = val;
        }

        #endregion
    }

    public partial class UISetting
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope

        //>>begin
        private UnityEngine.UI.Slider slider_bgm;
        private UnityEngine.UI.Slider slider_se;
        private TMPro.TMP_Dropdown tmpdrop_language;

        private void GetComps()
        {
            slider_bgm = Binder.Get<UnityEngine.UI.Slider>("slider_bgm");
            slider_se = Binder.Get<UnityEngine.UI.Slider>("slider_se");
            tmpdrop_language = Binder.Get<TMPro.TMP_Dropdown>("tmpdrop_language");
        }
        //<<end

        // =============================================
    }
}
