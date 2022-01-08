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
        public UISetting()
        {
            m_AssetName = "UISetting";
        }

        #region Impl

        protected override void InternalStart()
        {
            Binder = GetUIBinder<UIBinder>();
            if (Binder == null)
            {
                return;
            }

            GetComps();
        }

        protected override void InternalUpdate(float deltaTime)
        {
        }

        protected override void InternalClose()
        {
        }

        protected override void ListenEvents()
        {
            Listen(slider_bgm.onValueChanged, OnBGMChanged);
            Listen(slider_se.onValueChanged, OnSEChanged);

            //Listen(Binder.Get<Button>("btn_close").onClick, Close);
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

    // =============================================
    // code generate between >>begin and <<end
    // don't modify this scope

    //>>begin

    public partial class UISetting
    {
		private UnityEngine.UI.Slider slider_bgm;
		private UnityEngine.UI.Slider slider_se;

		void GetComps()
		{
			slider_bgm = Binder.Get<UnityEngine.UI.Slider>("slider_bgm");
			slider_se = Binder.Get<UnityEngine.UI.Slider>("slider_se");
		}
	}

	//<<end

    // =============================================
}
