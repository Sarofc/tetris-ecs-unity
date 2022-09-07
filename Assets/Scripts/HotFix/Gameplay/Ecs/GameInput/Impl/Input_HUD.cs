
using Saro.Entities;
using Saro.UI;
using Tetris.UI;
using UnityEngine;

namespace Tetris
{
    public class Input_HUD : IInputController
    {
        public UIInputHUD InputHUD { get; private set; }
        public EcsWorld World { get; private set; }

        void IInputController.BindInput(EcsWorld world)
        {
            // 在 GameplayAssets 里已经提前预载了
            InputHUD = UIManager.Current.ShowWindow<UIInputHUD>(EGameUI.UIInputHUD);

            World = world;
        }

        void IInputController.ProcessInput()
        {
            ref var input = ref World.GetSingleton<GameInputComponent>();

            input.spaceDown = InputHUD.btn_drop.PressDown;

            input.downArrowDown = InputHUD.btn_down.PressDown;
            input.downArrowUp = InputHUD.btn_down.PressUp;

            input.leftArrowDown = InputHUD.btn_left.PressDown;
            input.leftArrowUp = InputHUD.btn_left.PressUp;
            input.leftArrowPressing = InputHUD.btn_left.Pressed;

            input.rightArrowDown = InputHUD.btn_right.PressDown;
            input.rightArrowUp = InputHUD.btn_right.PressUp;
            input.rightArrowPressing = InputHUD.btn_right.Pressed;

            input.zDown = InputHUD.btn_rotateL.PressDown;
            input.xDown = InputHUD.btn_rotateR.PressDown;
            input.cDown = InputHUD.btn_hold.PressDown;
        }
    }
}
