using Leopotam.Ecs;

namespace Tetris
{
    internal sealed class PieceResetDelaySystem : IEcsRunSystem
    {
        //readonly GameContext m_GameCtx;

        private EcsFilter<PieceRotationSuccess> m_RotationSuccess;
        private EcsFilter<PieceMoveSuccess> m_MoveSuccess;
        private EcsFilter<PieceHoldRequest> m_HoldRequest;
        private EcsFilter<DelayComponent, PieceMoveComponent> m_Delay;

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

                foreach (var item in m_MoveSuccess)
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