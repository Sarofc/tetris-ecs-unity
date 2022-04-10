using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Saro.Gameplay.Effect;

namespace Tetris
{
    internal sealed class EffectSystem : IEcsRunSystem
    {
        private EcsFilter<EffectEvent> m_EffectEvent;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_EffectEvent)
            {
                ref var evt = ref m_EffectEvent.Get1(i);

                /*var handle = */
                EffectManager.Current.CreateEffectAsync(evt.effectAsset, evt.effectPosition).Forget();
                //if (handle)
                //{
                //    var effectTransform = handle.EffectScript.transform;
                //    effectTransform.SetParent(null);
                //    effectTransform.position = evt.effectPosition;
                //}
            }
        }
    }
}