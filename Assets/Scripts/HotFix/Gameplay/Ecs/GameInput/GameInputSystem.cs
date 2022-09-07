//#define USE_INPUT_HUD

using Saro.Entities;
using Saro.Entities.Extension;
using Saro.UI;
using Tetris.UI;
using UnityEngine;

namespace Tetris
{
    internal sealed class GameInputSystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        public bool Enable { get; set; } = true;

        private IInputController m_InputController;

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            world.GetSingleton<GameInputComponent>();

#if USE_INPUT_HUD
            m_InputController = new Input_HUD();
#else
            m_InputController = new Input_Keyboard();
#endif

            m_InputController.BindInput(world);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var dt = Time.deltaTime;
            ref var input = ref world.GetSingleton<GameInputComponent>();
            ProcessBlockInput(world, ref input, dt);
        }

        private void ProcessBlockInput(EcsWorld world, ref GameInputComponent input, float deltaTime)
        {
            m_InputController.ProcessInput();

            // hard & soft drop
            if (input.spaceDown)
            {
                world.SendMessage(new PieceDropRequest { dropType = EDropType.Hard });
            }
            else if (!input.downPressed && input.downArrowDown)
            {
                input.downPressed = true;
                world.SendMessage(new PieceDropRequest { dropType = EDropType.Soft });
            }
            else if (input.downPressed && input.downArrowUp)
            {
                input.downPressed = false;
                world.SendMessage(new PieceDropRequest { dropType = EDropType.Normal });
            }

            // move left
            if (input.leftArrowDown)
            {
                world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.left });
                input.leftPressed = true;
            }
            if (!input.rightPressed && input.leftPressed && input.leftArrowPressing)
            {
                if (input.lastStartTime >= TetrisDef.StartTime)
                {
                    if (input.lastInputTime >= TetrisDef.InputDelta)
                    {
                        world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.left });
                        input.lastInputTime = 0;
                    }
                    else
                    {
                        input.lastInputTime += deltaTime;
                    }
                }
                else
                {
                    input.lastStartTime += deltaTime;
                }
            }
            if (input.leftPressed && input.leftArrowUp)
            {
                input.leftPressed = false;
                input.lastStartTime = 0;
                input.lastInputTime = 0;
            }

            // move right
            if (input.rightArrowDown)
            {
                world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.right });
                input.rightPressed = true;
            }
            if (!input.leftPressed && input.rightPressed && input.rightArrowPressing)
            {
                if (input.lastStartTime >= TetrisDef.StartTime)
                {
                    if (input.lastInputTime >= TetrisDef.InputDelta)
                    {
                        world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.right });

                        input.lastInputTime = 0;
                    }
                    else
                    {
                        input.lastInputTime += deltaTime;
                    }
                }
                else
                {
                    input.lastStartTime += deltaTime;
                }
            }
            if (input.rightPressed && input.rightArrowUp)
            {
                input.rightPressed = false;
                input.lastStartTime = 0;
                input.lastInputTime = 0;
            }

            // rotate
            if (input.zDown) world.SendMessage(new PieceRotationRequest { clockwise = false });
            if (input.xDown) world.SendMessage(new PieceRotationRequest { clockwise = true });

            // hold
            if (input.cDown) world.SendMessage(new PieceHoldRequest());
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            m_InputController.OnDestroy();
        }
    }
}