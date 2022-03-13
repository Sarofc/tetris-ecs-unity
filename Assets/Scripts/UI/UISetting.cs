using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Saro.UI;
using Saro.Audio;

namespace Tetris.UI
{
    public sealed partial class UISetting : SingletonUI<UISetting, UIBinder>
    {
        #region Impl

        protected override void InternalStart()
        {
            var lan = Saro.Localization.LocalizationComponent.Current.CurrentLanguage;
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

            Listen(tmpdrop_language.onValueChanged, OnLanguageChanged);

            //Listen(Binder.Get<Button>("btn_close").onClick, Close);
        }

        private void OnLanguageChanged(int val)
        {

        }

        private void OnBGMChanged(float val)
        {
            SoundComponent.Current.VolumeBGM = val;
        }

        private void OnSEChanged(float val)
        {
            SoundComponent.Current.VolumeSE = val;
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

        void GetComps()
        {
            slider_bgm = Binder.Get<UnityEngine.UI.Slider>("slider_bgm");
            slider_se = Binder.Get<UnityEngine.UI.Slider>("slider_se");
            tmpdrop_language = Binder.Get<TMPro.TMP_Dropdown>("tmpdrop_language");
        }
        //<<end

        // =============================================
    }
}
