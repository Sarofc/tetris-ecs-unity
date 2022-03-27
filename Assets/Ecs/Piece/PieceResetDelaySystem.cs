using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    sealed class PieceResetDelaySystem : IEcsRunSystem
    {
        //readonly GameContext m_GameCtx;

        EcsFilter<PieceRotationSuccess> m_RotationSuccess;
        EcsFilter<PieceHoldRequest> m_HoldRequest;

        EcsFilter<DelayComponent, PieceMoveComponent> m_Delay;

        //EcsFilter<PieceMoveComponent>.Exclude<DelayComponent> m_MovePieces;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_Delay)
            {
                //ref var cDelay = ref m_Delay.Get1(i);
                ref var ePiece = ref m_Delay.GetEntity(i);

                foreach (var i3 in m_HoldRequest)
                {
                    //cDelay.delay = TetrisDef.k_AddToGridDelay;
                    ePiece.Del<AddToGridComponent>();
                    ePiece.Del<DelayComponent>();
                }

                foreach (var i2 in m_RotationSuccess)
                {
                    //cDelay.delay = TetrisDef.k_AddToGridDelay;
                    ePiece.Del<AddToGridComponent>();
                    ePiece.Del<DelayComponent>();
                }
            }

            //foreach (var i in m_MovePieces)
            //{
            //    ref var ePiece = ref m_MovePieces.GetEntity(i);
            //    ref var cMove = ref ePiece.Get<PieceMoveComponent>();

            //    if (cMove.hasLanded)
            //    {
            //        cMove.hasLanded = false;
            //        m_GameCtx.SendMessage(new AddToGridReuqest { });

            //        FireSound(cMove.dropType);
            //        FireEffect(cMove.dropType, ePiece.Get<PositionComponent>().position);
            //    }
            //}
        }
    }
}