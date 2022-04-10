using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceMoveSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;
        [EcsIgnoreInject]
        private EcsEntity[][] m_Grid => m_GameCtx.grid;
        public const float k_DeltaNormal = 1f;
        private EcsFilter<PieceDropRequest> m_DropRequests;
        private EcsFilter<PieceMoveRequest> m_MoveRequests;
        private EcsFilter<PieceMoveComponent, ComponentList<EcsEntity>, PositionComponent> m_Moves;

        void IEcsRunSystem.Run()
        {
            var deltaTime = Time.deltaTime;

            foreach (var i in m_Moves)
            {
                ref var ePiece = ref m_Moves.GetEntity(i);
                ref var cMove = ref m_Moves.Get1(i);

                foreach (var i2 in m_DropRequests)
                {
                    ref var request = ref m_DropRequests.Get1(i2);

                    cMove.dropType = request.dropType;
                    if (cMove.dropType == EDropType.Soft)
                    {
                        cMove.lastFallTime = 0f;
                    }
                    else if (cMove.dropType == EDropType.Hard)
                    {
                        m_GameCtx.lastOpIsRotate = false;
                    }
                }

                foreach (var i1 in m_MoveRequests)
                {
                    ref var request = ref m_MoveRequests.Get1(i1);

                    bool moved = TetrisUtil.MovePiece(m_Grid, ePiece, request.moveDelta);

                    if (moved)
                    {
                        m_GameCtx.lastOpIsRotate = false;

                        m_GameCtx.SendMessage(new PieceMoveSuccess { });

                        m_GameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });

                        m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_move.wav" });
                    }
                }

                AutoDrop(ePiece, deltaTime, Vector2.down);
            }
        }

        private void AutoDrop(in EcsEntity ePiece, float deltaTime, in Vector2 moveDelta)
        {
            ref var cMove = ref ePiece.Get<PieceMoveComponent>();

            ref var lastFallTime = ref cMove.lastFallTime;

            float dropDeltaTime;
            switch (cMove.dropType)
            {
                case EDropType.Normal:
                default:
                    dropDeltaTime = k_DeltaNormal;
                    break;
                case EDropType.Soft:
                    dropDeltaTime = k_DeltaNormal * 0.1f;
                    break;
                case EDropType.Hard:
                    dropDeltaTime = 0f;
                    break;
            }

            lastFallTime += deltaTime;

            while (lastFallTime >= dropDeltaTime)
            {
                lastFallTime -= dropDeltaTime;
                if (!TetrisUtil.MovePiece(m_Grid, ePiece, moveDelta))
                {
                    // TODO 无法继续往下移动了，过一定时间，此方块就不能再操作了
                    // 如果是hard drop,则直接加入grid
                    // 如果是 其他类型,添加一个 delay,再add to grid

                    if (ePiece.Has<AddToGridComponent>() == false)
                    {
                        ePiece.Get<AddToGridComponent>();
                    }

                    if (cMove.dropType != EDropType.Hard)
                    {
                        if (ePiece.Has<DelayComponent>() == false)
                        {
                            ref var delay = ref ePiece.Get<DelayComponent>();
                            delay.delay = TetrisDef.k_AddToGridDelay;
                        }
                    }

                    lastFallTime = 0f;
                    break;
                }
            }
        }
    }
}