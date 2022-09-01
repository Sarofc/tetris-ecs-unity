using System.Collections.Generic;
using Saro.Entities;
using Saro.Entities.Extension;
using Saro.Utility;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceNextSystem : IEcsRunSystem
    {
        public bool Enable { get; set; } = true;
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var requests = world.Filter().Inc<PieceNextRequest>().End();
            var bags = world.Filter().Inc<PieceBagComponent, ComponentList<EcsEntity>>().End();

            foreach (var i in requests)
            foreach (var j in bags)
            {
                ref var bag = ref j.Get<PieceBagComponent>(world);
                var bagList = j.Get<ComponentList<EcsEntity>>(world).Value;

                RequestNextBlock(world, ref bag, bagList);

                TetrisUtil.UpdateNextChainSlot(world, bagList);
            }
        }

        private void RequestNextBlock(EcsWorld world, ref PieceBagComponent bag, List<EcsEntity> queue)
        {
            ref var currentIndex = ref bag.currentIndex;

            if (currentIndex >= 7)
            {
                SwapLeftRight(queue);
                RandomRight(queue);
                currentIndex = 0;
            }

            var ePiece = queue[0];
            queue.RemoveAt(0);
            queue.Add(ePiece);

            ref var cPiece = ref ePiece.Get<PieceComponent>();
            world.SendMessage(new PieceSpawnRequest
                { pieceID = cPiece.pieceID, spawnPosition = new Vector3(TetrisDef.Width / 2, TetrisDef.Height) });
        }

        private static void RandomRight(List<EcsEntity> queue)
        {
            RandomUtility.Shuffle(queue, 7, 7);
        }

        private static void SwapLeftRight(List<EcsEntity> queue)
        {
            var halfLen = queue.Count / 2;
            for (var i = 0; i < halfLen; i++) RandomUtility.Swap(queue, i, halfLen + i);
        }
    }
}