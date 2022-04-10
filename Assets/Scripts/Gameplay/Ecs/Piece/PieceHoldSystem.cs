using Leopotam.Ecs;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceHoldSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;
        private EcsFilter<PieceHoldRequest> m_Filter;
        private EcsFilter<PieceMoveComponent> m_Move;

        private float holdedViewPosX = -2.4f;
        private float holdedViewPosY = 16;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_Filter)
            {
                ref var request = ref m_Filter.Get1(i);

                ref var heldPiece = ref m_GameCtx.heldPiece;
                ref var lastHeldPiece = ref m_GameCtx.lastHeldPiece;

                foreach (var i2 in m_Move)
                {
                    ref var ePiece = ref m_Move.GetEntity(i2);

                    if (heldPiece == ePiece || lastHeldPiece == ePiece)
                        return;

                    ePiece.Del<PieceMoveComponent>();
                    ePiece.Del<PieceRotateFlag>();
                    ePiece.Del<AddToGridComponent>();
                    ePiece.Del<DelayComponent>();

                    if (!heldPiece.IsAlive())
                    {
                        heldPiece = ePiece;

                        m_GameCtx.SendMessage(new PieceNextRequest { });
                    }
                    else
                    {
                        var tmpPiece = heldPiece;
                        tmpPiece.Get<PieceMoveComponent>();

                        if (tmpPiece.Get<PieceComponent>().pieceID != EPieceID.O)
                            tmpPiece.Get<PieceRotateFlag>();

                        heldPiece = ePiece;
                        ePiece = tmpPiece;

                        ePiece.Get<PositionComponent>().position = new Vector3(TetrisDef.k_Width / 2, TetrisDef.k_Height);

                        lastHeldPiece = ePiece;

                        ePiece.Get<PieceComponent>().scale = 1f;

                        m_GameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });
                    }

                    TetrisUtil.ResetPieceRotation(ref heldPiece);
                    heldPiece.Get<PieceComponent>().scale = 0.6f;
                    heldPiece.Get<PositionComponent>().position = new Vector3(holdedViewPosX, holdedViewPosY);

                    m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_hold.wav" });
                }
            }
        }
    }
}