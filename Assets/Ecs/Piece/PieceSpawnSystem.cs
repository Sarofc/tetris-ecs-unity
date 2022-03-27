using Leopotam.Ecs;

namespace Tetris
{
    sealed class PieceSpawnSystem : IEcsRunSystem
    {
        readonly GameContext m_GameCtx;

        EcsFilter<PieceSpawnRequest> m_SpawnPieces;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_SpawnPieces)
            {
                ref var spawnRequest = ref m_SpawnPieces.Get1(i);

                var ePiece = TetrisUtil.CreatePiece(m_GameCtx.world, spawnRequest.pieceID, spawnRequest.spawnPosition);

                m_GameCtx.lastOpIsRotate = false;

                m_GameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });
            }
        }
    }
}