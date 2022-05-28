using Leopotam.EcsLite;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceHoldSystem : IEcsRunSystem
    {
        private readonly float m_HoldedViewPosX = -2.4f;
        private readonly float m_HoldedViewPosY = 16;

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world.Filter().Inc<PieceHoldRequest>().End();
            var move = world.Filter().Inc<PieceMoveComponent>().End();
            var gameCtx = systems.GetShared<GameContext>();

            foreach (var i in filter)
            {
                ref var request = ref i.Get<PieceHoldRequest>(world);

                ref var heldPiece = ref gameCtx.heldPiece;
                //ref var lastHeldPiece = ref m_GameCtx.lastHeldPiece;

                foreach (var i2 in move)
                {
                    if (!gameCtx.canHold) return;

                    var ePiece = world.PackEntity(i2);

                    gameCtx.canHold = false;

                    //if (heldPiece == ePiece)
                    //    return;

                    ePiece.Del<PieceMoveComponent>(world);
                    ePiece.Del<PieceRotateFlag>(world);
                    ePiece.Del<AddToGridComponent>(world);
                    ePiece.Del<DelayComponent>(world);

                    if (!heldPiece.IsAlive(world))
                    {
                        heldPiece = ePiece;

                        gameCtx.firstHold = true;

                        TetrisUtil.ChangePieceColor(world, ref heldPiece, Color.gray);

                        gameCtx.SendMessage(new PieceNextRequest());
                    }
                    else
                    {
                        var tmpPiece = heldPiece;
                        tmpPiece.Add<PieceMoveComponent>(world);

                        var pieceID = tmpPiece.Add<PieceComponent>(world).pieceID;
                        if (pieceID != EPieceID.O)
                            tmpPiece.Add<PieceRotateFlag>(world);

                        heldPiece = ePiece;
                        ePiece = tmpPiece;

                        ePiece.Add<PositionComponent>(world).position =
                            new Vector3(TetrisDef.Width / 2, TetrisDef.Height);
                        ePiece.Add<PieceComponent>(world).scale = 1f;

                        TetrisUtil.ChangePieceColor(world, ref heldPiece, Color.gray);
                        TetrisUtil.ChangePieceColor(world, ref ePiece, TetrisUtil.GetTileColor(pieceID));

                        gameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });
                    }

                    TetrisUtil.ResetPieceRotation(world, ref heldPiece);

                    heldPiece.Add<PieceComponent>(world).scale = 0.6f;
                    heldPiece.Add<PositionComponent>(world).position = new Vector3(m_HoldedViewPosX, m_HoldedViewPosY);

                    gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_hold.wav" });
                }
            }
        }
    }
}