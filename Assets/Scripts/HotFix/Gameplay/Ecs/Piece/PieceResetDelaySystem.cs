using Leopotam.EcsLite;

namespace Tetris
{
    internal sealed class PieceResetDelaySystem : IEcsRunSystem
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var rotationSuccess = world.Filter().Inc<PieceRotationSuccess>().End();
            var moveSuccess = world.Filter().Inc<PieceMoveSuccess>().End();
            var holdRequest = world.Filter().Inc<PieceHoldRequest>().End();
            var delay = world.Filter().Inc<DelayComponent, PieceMoveComponent>().End();

            foreach (var i in delay)
            {
                var ePiece = world.PackEntity(i);

                foreach (var i3 in holdRequest)
                {
                    //cDelay.delay = TetrisDef.k_AddToGridDelay;
                    ePiece.Del<AddToGridComponent>(world);
                    ePiece.Del<DelayComponent>(world);
                }

                foreach (var item in moveSuccess)
                {
                    //cDelay.delay = TetrisDef.k_AddToGridDelay;
                    ePiece.Del<AddToGridComponent>(world);
                    ePiece.Del<DelayComponent>(world);
                }

                foreach (var i2 in rotationSuccess)
                {
                    //cDelay.delay = TetrisDef.k_AddToGridDelay;
                    ePiece.Del<AddToGridComponent>(world);
                    ePiece.Del<DelayComponent>(world);
                }
            }
        }
    }
}