using Leopotam.EcsLite;
using Leopotam.EcsLite.Extension;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceGhostSystem : IEcsRunSystem, IEcsInitSystem
    {
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            TetrisUtil.CreatePieceForGhost(world, EPieceID.O, new Vector2(-99, -99));
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var gameCtx = systems.GetShared<GameContext>();
            var grid = gameCtx.grid;
            var world = systems.GetWorld();
            var ghostUpdateRequest = world.Filter().Inc<PieceGhostUpdateRequest>().End();
            var ghostPiece = world.Filter()
                .Inc<PieceGhostComponent, ComponentList<EcsPackedEntity>, PositionComponent>().End();


            foreach (var i in ghostUpdateRequest)
            {
                ref var request = ref i.Get<PieceGhostUpdateRequest>(world);

                var eGhostPiece = world.PackEntity(ghostPiece[0]);

                CopyState(world, ref eGhostPiece, in request.ePiece);

                while (TetrisUtil.MovePiece(world, grid, eGhostPiece, Vector2.down))
                {
                }
            }
        }

        private void CopyState(EcsWorld world, ref EcsPackedEntity eGhostPiece, in EcsPackedEntity ePiece)
        {
            ref var cGhostPos = ref eGhostPiece.Get<PositionComponent>(world);

            if (!ePiece.IsAlive(world))
            {
                cGhostPos.position = new Vector2(-99, -99);
            }
            else
            {
                // λ�ñ���ͬ��
                cGhostPos.position = ePiece.Get<PositionComponent>(world).position;

                ref var cPiece = ref ePiece.Get<PieceComponent>(world);
                ref var cGhostPiece = ref eGhostPiece.Get<PieceComponent>(world);

                // ��� piece id �� state����������Ҫ���Ƶ�����
                if (cPiece.pieceID == cGhostPiece.pieceID && cPiece.state == cGhostPiece.state) return;

                cGhostPiece.state = cPiece.state;
                cGhostPiece.pieceID = cPiece.pieceID;

                var tileList = ePiece.Get<ComponentList<EcsPackedEntity>>(world).Value;
                var ghostTileList = eGhostPiece.Get<ComponentList<EcsPackedEntity>>(world).Value;

                for (var i = 0; i < tileList.Count; i++)
                {
                    var tile = tileList[i];
                    var ghostTile = ghostTileList[i];

                    ref var cTilePos = ref tile.Get<PositionComponent>(world);
                    ref var cGhostTilePos = ref ghostTile.Get<PositionComponent>(world);
                    cGhostTilePos.position = cTilePos.position;
                }
            }
        }
    }
}