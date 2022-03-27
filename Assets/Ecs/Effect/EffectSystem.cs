using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Saro.Gameplay.Effect;

namespace Tetris
{
    sealed class EffectSystem : IEcsRunSystem
    {
        EcsFilter<EffectEvent> m_EffectEvent;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_EffectEvent)
            {
                ref var evt = ref m_EffectEvent.Get1(i);

                var handle = EffectManager.Current.CreateEffect(evt.effectAsset);
                var effectTransform = handle.EffectScript.transform;
                effectTransform.SetParent(null);
                effectTransform.position = evt.effectPosition;
            }
        }
    }
}