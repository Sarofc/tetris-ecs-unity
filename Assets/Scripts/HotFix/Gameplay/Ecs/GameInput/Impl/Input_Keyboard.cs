
using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    public class Input_Keyboard : IInputController
    {
        public EcsWorld World { get; private set; }

        void IInputController.BindInput(EcsWorld world)
        {
            World = world;
        }

        void IInputController.ProcessInput()
        {
            ref var input = ref World.GetSingleton<GameInputComponent>();

            input.spaceDown = Input.GetKeyDown(KeyCode.Space);

            input.downArrowDown = Input.GetKeyDown(KeyCode.DownArrow);
            input.downArrowUp = Input.GetKeyUp(KeyCode.DownArrow);

            input.leftArrowDown = Input.GetKeyDown(KeyCode.LeftArrow);
            input.leftArrowUp = Input.GetKeyUp(KeyCode.LeftArrow);
            input.leftArrowPressing = Input.GetKey(KeyCode.LeftArrow);

            input.rightArrowDown = Input.GetKeyDown(KeyCode.RightArrow);
            input.rightArrowUp = Input.GetKeyUp(KeyCode.RightArrow);
            input.rightArrowPressing = Input.GetKey(KeyCode.RightArrow);

            input.zDown = Input.GetKeyDown(KeyCode.Z);
            input.xDown = Input.GetKeyDown(KeyCode.X);
            input.cDown = Input.GetKeyDown(KeyCode.C);
        }
    }
}
