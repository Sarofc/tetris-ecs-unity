using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Saro.UI;


namespace Tetris.UI
{
    public sealed partial class UICountdown : SingletonUI<UICountdown, UIBinder>
    {
        public UICountdown()
        {
            m_AssetName = "UICountdown";
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
            //Listen(Binder.Get<Button>("btn_close").onClick, Close);
        }

        #endregion
    }

    // =============================================
    // code generate between >>begin and <<end
    // don't modify this scope

    //>>begin

    public partial class UICountdown
    {
		private UnityEngine.Animator anim_root;

		void GetComps()
		{
			anim_root = Binder.Get<UnityEngine.Animator>("anim_root");
		}
	}

	//<<end

    // =============================================
}
