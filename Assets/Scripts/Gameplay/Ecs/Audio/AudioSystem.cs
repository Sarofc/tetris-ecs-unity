using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Saro.Audio;

namespace Tetris
{
    internal sealed class AudioSystem : IEcsRunSystem
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var seFilter = world.Filter().Inc<SeAudioEvent>().End();
            var bgmFilter = world.Filter().Inc<BGMAudioEvent>().End();

            foreach (var ent in seFilter)
            {
                ref var evt = ref ent.Get<SeAudioEvent>(world);

                AudioManager.Current.PlaySEAsync(evt.audioAsset).Forget();
            }

            foreach (var ent in bgmFilter)
            {
                ref var evt = ref ent.Get<BGMAudioEvent>(world);

                AudioManager.Current.PlayBGMAsync(evt.audioAsset).Forget();
            }
        }
    }
}