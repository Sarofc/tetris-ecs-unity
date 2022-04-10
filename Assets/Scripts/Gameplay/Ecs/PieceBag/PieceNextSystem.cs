using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using Saro.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceNextSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = null;
        private EcsFilter<PieceNextRequest> m_Requests;

        // TODO 改成 reactive 模式
        private EcsFilter<PieceBagComponent, ComponentList<EcsEntity>> m_Bags;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_Requests)
            {
                foreach (var j in m_Bags)
                {
                    ref var bag = ref m_Bags.Get1(j);
                    var bagList = m_Bags.Get2(j).Value;

                    RequestNextBlock(ref bag, bagList);

                    TetrisUtil.UpdateNextChainSlot(bagList);
                }
            }
        }

        private void RequestNextBlock(ref PieceBagComponent bag, List<EcsEntity> queue)
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
            _world.SendMessage(new PieceSpawnRequest { pieceID = cPiece.pieceID, spawnPosition = new Vector3(TetrisDef.k_Width / 2, TetrisDef.k_Height) });
        }

        private static void RandomRight(List<EcsEntity> queue)
        {
            RandomUtility.Shuffle(queue, 7, 7);
        }

        private static void SwapLeftRight(List<EcsEntity> queue)
        {
            var halfLen = queue.Count / 2;
            for (int i = 0; i < halfLen; i++)
            {
                RandomUtility.Swap(queue, i, halfLen + i);
            }
        }
    }
}