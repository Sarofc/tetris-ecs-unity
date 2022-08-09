using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Extension;
using Saro.Utility;
using UnityEngine;

namespace Tetris
{
    internal sealed class PieceBagInitSystem : IEcsInitSystem
    {
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var ent = world.NewEntity();
            var bagList = ent.Add<ComponentList<EcsPackedEntity>>(world).Value;
            ent.Add<PieceBagComponent>(world);

            FillBag(world, bagList, new EPieceID[7]
            {
                EPieceID.I,
                EPieceID.J,
                EPieceID.L,
                EPieceID.O,
                EPieceID.S,
                EPieceID.T,
                EPieceID.Z
            });

            Debug.LogError("PieceBagInitSystem init done");
        }

        private void FillBag(EcsWorld world, List<EcsPackedEntity> queue, EPieceID[] blocks)
        {
            for (var i = 0; i < blocks.Length; i++)
                queue.Add(TetrisUtil.CreatePieceForBagView(world, blocks[i], new Vector3()));
            for (var i = 0; i < blocks.Length; i++)
                queue.Add(TetrisUtil.CreatePieceForBagView(world, blocks[i], new Vector3()));

            RandomLeft(queue);
            RandomRight(queue);

            TetrisUtil.UpdateNextChainSlot(world, queue);
        }

        private static void RandomLeft(List<EcsPackedEntity> queue)
        {
            RandomUtility.Shuffle(queue, 0, 7);
        }

        private static void RandomRight(List<EcsPackedEntity> queue)
        {
            RandomUtility.Shuffle(queue, 7, 7);
        }
    }
}