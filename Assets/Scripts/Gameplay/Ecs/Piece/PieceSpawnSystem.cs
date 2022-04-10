using Leopotam.Ecs;

namespace Tetris
{
    internal sealed class PieceSpawnSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;
        private EcsFilter<PieceSpawnRequest> m_SpawnPieces;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_SpawnPieces)
            {
                ref var spawnRequest = ref m_SpawnPieces.Get1(i);

                var ePiece = TetrisUtil.CreatePiece(m_GameCtx.world, spawnRequest.pieceID, spawnRequest.spawnPosition);

                if (!TetrisUtil.IsValidBlock(m_GameCtx.grid, ePiece))
                {
                    ePiece.Del<PieceMoveComponent>();
                    ePiece.Del<PieceRotateFlag>();
                    m_GameCtx.SendMessage(new GameEndComponent { }, new DelayComponent { delay = 1f });
                }
                else
                {
                    m_GameCtx.lastOpIsRotate = false;

                    m_GameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });
                }
            }
        }
    }
}