using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    sealed class DelaySystem : IEcsRunSystem
    {
        EcsFilter<DelayComponent> m_Delay;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_Delay)
            {
                ref var ent = ref m_Delay.GetEntity(i);
                ref var request = ref m_Delay.Get1(i);

                if (request.delay <= 0)
                {
                    request.delay = 0f;
                    ent.Del<DelayComponent>();
                }
                else
                {
                    request.delay -= Time.deltaTime;
                }
            }
        }
    }
}