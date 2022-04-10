using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceGhostSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly GameContext gameCtx;

        private EcsEntity[][] m_Grid => gameCtx.grid;

        private EcsFilter<PieceGhostUpdateRequest> m_GhostUpdateRequest;
        private EcsFilter<PieceGhostComponent, ComponentList<EcsEntity>, PositionComponent> m_GhostPiece;

        void IEcsInitSystem.Init()
        {
            TetrisUtil.CreatePieceForGhost(gameCtx.world, EPieceID.O, new Vector2(-99, -99));
        }

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_GhostUpdateRequest)
            {
                ref var request = ref m_GhostUpdateRequest.Get1(i);

                ref var eGhostPiece = ref m_GhostPiece.GetEntity(0);

                CopyState(ref eGhostPiece, in request.ePiece);

                while (TetrisUtil.MovePiece(m_Grid, eGhostPiece, Vector2.down))
                { }
            }
        }

        private void CopyState(ref EcsEntity eGhostPiece, in EcsEntity ePiece)
        {
            ref var cGhostPos = ref eGhostPiece.Get<PositionComponent>();

            if (!ePiece.IsAlive())
            {
                cGhostPos.position = new Vector2(-99, -99);
            }
            else
            {
                // 位置必须同步
                cGhostPos.position = ePiece.Get<PositionComponent>().position;

                ref var cPiece = ref ePiece.Get<PieceComponent>();
                ref var cGhostPiece = ref eGhostPiece.Get<PieceComponent>();

                // 检查 piece id 和 state，跳过不需要复制的数据
                if (cPiece.pieceID == cGhostPiece.pieceID && cPiece.state == cGhostPiece.state) return;

                cGhostPiece.state = cPiece.state;
                cGhostPiece.pieceID = cPiece.pieceID;

                var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;
                var ghostTileList = eGhostPiece.Get<ComponentList<EcsEntity>>().Value;

                for (int i = 0; i < tileList.Count; i++)
                {
                    var tile = tileList[i];
                    var ghostTile = ghostTileList[i];

                    ref var cTilePos = ref tile.Get<PositionComponent>();
                    ref var cGhostTilePos = ref ghostTile.Get<PositionComponent>();
                    cGhostTilePos.position = cTilePos.position;
                }
            }
        }
    }
}