using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Saro.Gameplay.Effect;

namespace Tetris
{
    internal sealed class EffectSystem : IEcsRunSystem
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();

            var effectEvent = world.Filter().Inc<EffectEvent>().End();
            foreach (var i in effectEvent)
            {
                ref var evt = ref i.Get<EffectEvent>(world);

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