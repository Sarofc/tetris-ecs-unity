using Saro.UI;
using Saro.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.UI
{

    [UIWindow((int)EGameUI.UIInputHUD, "Assets/Res/Prefabs/UI/UIInputHUD.prefab")]
    public partial class UIInputHUD : UIWindow
    {
        public UIInputHUD(string resPath) : base(resPath)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            //Listen(btn_ok.onClick, Method);
        }

        protected override void OnShow()
        {
            base.OnShow();

        }
    }

    public partial class UIInputHUD
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope

        //>>begin
		public Tetris.XButton btn_left => Binder.GetRef<Tetris.XButton>("btn_left");
		public Tetris.XButton btn_right => Binder.GetRef<Tetris.XButton>("btn_right");
		public Tetris.XButton btn_down => Binder.GetRef<Tetris.XButton>("btn_down");
		public Tetris.XButton btn_rotateL => Binder.GetRef<Tetris.XButton>("btn_rotateL");
		public Tetris.XButton btn_rotateR => Binder.GetRef<Tetris.XButton>("btn_rotateR");
		public Tetris.XButton btn_drop => Binder.GetRef<Tetris.XButton>("btn_drop");
		public Tetris.XButton btn_hold => Binder.GetRef<Tetris.XButton>("btn_hold");

	//<<end

        // =============================================
    }
}
