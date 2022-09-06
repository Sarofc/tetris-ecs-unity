using Saro.UI;
using Saro.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.UI
{

    [UIWindow((int)EGameUI.UICheckAssets, "Assets/Res/Prefabs/UI/UICheckAssets.prefab")]
    public partial class UICheckAssets : UIWindow
    {
        public UICheckAssets(string resPath) : base(resPath)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            Listen(btn_Mask.onClick, () =>
            {
                UIManager.Current.UnLoadWindow(EGameUI.UICheckAssets);
            });
        }

        protected override void OnShow()
        {
            base.OnShow();

        }
    }

    public partial class UICheckAssets
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope

        //>>begin
		public UnityEngine.UI.Button btn_Mask => Binder.GetRef<UnityEngine.UI.Button>("btn_Mask");
		public TMPro.TextMeshProUGUI tmptxt_info => Binder.GetRef<TMPro.TextMeshProUGUI>("tmptxt_info");

	//<<end

        // =============================================
    }
}
