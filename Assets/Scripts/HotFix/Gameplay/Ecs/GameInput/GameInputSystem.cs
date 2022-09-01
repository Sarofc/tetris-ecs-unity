using Saro.Entities;
using Saro.Entities.Extension;
using UnityEngine;

namespace Tetris
{
    internal sealed class GameInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        public bool Enable { get; set; } = true;
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            world.GetSingleton<GameInputComponent>();
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
            // hard & soft drop
            if (Input.GetKeyDown(KeyCode.Space))
            {
                world.SendMessage(new PieceDropRequest { dropType = EDropType.Hard });
            }
            else if (!input.downPressed && Input.GetKeyDown(KeyCode.DownArrow))
            {
                input.downPressed = true;
                world.SendMessage(new PieceDropRequest { dropType = EDropType.Soft });
            }
            else if (input.downPressed && Input.GetKeyUp(KeyCode.DownArrow))
            {
                input.downPressed = false;
                world.SendMessage(new PieceDropRequest { dropType = EDropType.Normal });
            }

            // move left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.left });
                input.leftPressed = true;
            }
            if (!input.rightPressed && input.leftPressed && Input.GetKey(KeyCode.LeftArrow))
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
            if (input.leftPressed && Input.GetKeyUp(KeyCode.LeftArrow))
            {
                input.leftPressed = false;
                input.lastStartTime = 0;
                input.lastInputTime = 0;
            }

            // move right
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.right });
                input.rightPressed = true;
            }
            if (!input.leftPressed && input.rightPressed && Input.GetKey(KeyCode.RightArrow))
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
            if (input.rightPressed && Input.GetKeyUp(KeyCode.RightArrow))
            {
                input.rightPressed = false;
                input.lastStartTime = 0;
                input.lastInputTime = 0;
            }

            // rotate
            if (Input.GetKeyDown(KeyCode.Z)) world.SendMessage(new PieceRotationRequest { clockwise = false });
            if (Input.GetKeyDown(KeyCode.X)) world.SendMessage(new PieceRotationRequest { clockwise = true });

            // hold
            if (Input.GetKeyDown(KeyCode.C)) world.SendMessage(new PieceHoldRequest());
        }
    }
}