using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using Saro.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceBagInitSystem : IEcsInitSystem
    {
        private readonly EcsWorld _world = null;

        void IEcsInitSystem.Init()
        {
            var ent = _world.NewEntity();
            var bagList = ent.Get<ComponentList<EcsEntity>>().Value;
            ent.Get<PieceBagComponent>();

            FillBag(bagList, new EPieceID[7]
            {
                EPieceID.I,
                EPieceID.J,
                EPieceID.L,
                EPieceID.O,
                EPieceID.S,
                EPieceID.T,
                EPieceID.Z,
            });
        }

        private void FillBag(List<EcsEntity> queue, EPieceID[] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                queue.Add(TetrisUtil.CreatePieceForBagView(_world, blocks[i], new Vector3()));
            }
            for (int i = 0; i < blocks.Length; i++)
            {
                queue.Add(TetrisUtil.CreatePieceForBagView(_world, blocks[i], new Vector3()));
            }

            RandomLeft(queue);
            RandomRight(queue);

            TetrisUtil.UpdateNextChainSlot(queue);
        }

        private static void RandomLeft(List<EcsEntity> m_Queue)
        {
            RandomUtility.Shuffle(m_Queue, 0, 7);
        }

        private static void RandomRight(List<EcsEntity> m_Queue)
        {
            RandomUtility.Shuffle(m_Queue, 7, 7);
        }
    }
}