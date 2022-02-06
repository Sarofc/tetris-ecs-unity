using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Saro.UI;


namespace Tetris.UI
{
    public sealed partial class UIAboutPanel : SingletonUI<UIAboutPanel, UIBinder>
    {
        #region Impl

        protected override void InternalStart()
        {
        }

        protected override void InternalAwake()
        {
            GetComps();
            Listen(btn_close.onClick, Close);
        }

        #endregion
    }

    // =============================================
    // code generate between >>begin and <<end
    // don't modify this scope

    //>>begin

    public partial class UIAboutPanel
    {
		private UnityEngine.UI.Button btn_close;

		void GetComps()
		{
			btn_close = Binder.Get<UnityEngine.UI.Button>("btn_close");
		}
	}

	//<<end

    // =============================================
}
