using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    internal sealed class DelaySystem : IEcsRunSystem
    {
        public bool Enable { get; set; } = true;
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var delay = world.Filter().Inc<DelayComponent>().End();

            foreach (var ent in delay)
            {
                ref var request = ref ent.Get<DelayComponent>(world);

                if (request.delay <= 0)
                {
                    request.delay = 0f;
                    ent.Del<DelayComponent>(world);
                }
                else
                {
                    request.delay -= Time.deltaTime;
                }
            }
        }
    }
}