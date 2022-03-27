using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    sealed class GameInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        readonly EcsWorld _world = null;

        EcsFilter<GameInputComponent> m_Inputs;

        void IEcsInitSystem.Init()
        {
            _world.NewEntity().Get<GameInputComponent>();
        }

        void IEcsRunSystem.Run()
        {
            var dt = Time.deltaTime;
            foreach (var i in m_Inputs)
            {
                ref var input = ref m_Inputs.Get1(i);
                ProcessBlockInput(ref input, dt);
            }
        }

        private void ProcessBlockInput(ref GameInputComponent input, float deltaTime)
        {
            // hard & soft drop
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _world.SendMessage(new PieceDropRequest { dropType = EDropType.Hard });
            }
            else if (!input.downPressed && Input.GetKeyDown(KeyCode.DownArrow))
            {
                input.downPressed = true;
                _world.SendMessage(new PieceDropRequest { dropType = EDropType.Soft });
            }
            else if (input.downPressed && Input.GetKeyUp(KeyCode.DownArrow))
            {
                input.downPressed = false;
                _world.SendMessage(new PieceDropRequest { dropType = EDropType.Normal });
            }

            // move left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.left });
                input.leftPressed = true;
            }
            if (!input.rightPressed && input.leftPressed && Input.GetKey(KeyCode.LeftArrow))
            {
                if (input.lastStartTime >= TetrisDef.k_StartTime)
                {
                    if (input.lastInputTime >= TetrisDef.k_InputDelta)
                    {
                        _world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.left });
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
                _world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.right });
                input.rightPressed = true;
            }
            if (!input.leftPressed && input.rightPressed && Input.GetKey(KeyCode.RightArrow))
            {
                if (input.lastStartTime >= TetrisDef.k_StartTime)
                {
                    if (input.lastInputTime >= TetrisDef.k_InputDelta)
                    {
                        _world.SendMessage(new PieceMoveRequest { moveDelta = Vector2.right });

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
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _world.SendMessage(new PieceRotationRequest { clockwise = false });
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                _world.SendMessage(new PieceRotationRequest { clockwise = true });
            }

            // hold
            if (Input.GetKeyDown(KeyCode.C))
            {
                _world.SendMessage(new PieceHoldRequest());
            }
        }
    }
}