using Saro.Entities;
using Saro;

namespace Tetris
{
    internal sealed class PieceSpawnSystem : IEcsRunSystem
    {
        public bool Enable { get; set; } = true;
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var gameCtx = systems.GetShared<GameContext>();
            var world = systems.GetWorld();
            var spawnPieces = world.Filter().Inc<PieceSpawnRequest>().End();

            foreach (var i in spawnPieces)
            {
                ref var spawnRequest = ref i.Get<PieceSpawnRequest>(world);

                var ePiece = TetrisUtil.CreatePiece(gameCtx.world, spawnRequest.pieceID, spawnRequest.spawnPosition);

                gameCtx.pieceCount++;

                if (!TetrisUtil.IsValidBlock(world, gameCtx.grid, ePiece))
                {
                    ePiece.Del<PieceMoveComponent>();
                    ePiece.Del<PieceRotateFlag>();
                    gameCtx.SendMessage(new GameEndComponent(), new DelayComponent { delay = 1f });

                    Log.ERROR("GameOver");
                }
                else
                {
                    gameCtx.lastOpIsRotate = false;

                    gameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });

                    if (gameCtx.firstHold)
                    {
                        gameCtx.firstHold = false;
                    }
                    else
                    {
                        if (!gameCtx.canHold)
                        {
                            gameCtx.canHold = true;

                            ref var heldPiece = ref gameCtx.heldPiece;
                            if (!heldPiece.IsNull())
                                TetrisUtil.ChangePieceColor(world, ref heldPiece,
                                    TetrisUtil.GetTileColor(heldPiece.Add<PieceComponent>().pieceID));
                        }
                    }
                }
            }
        }
    }
}