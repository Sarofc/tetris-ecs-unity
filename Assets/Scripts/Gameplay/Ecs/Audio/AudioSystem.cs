using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Saro.Audio;

namespace Tetris
{
    internal sealed class AudioSystem : IEcsRunSystem
    {
        private EcsFilter<SEAudioEvent> m_SEAudioEvent;
        private EcsFilter<BGMAudioEvent> m_BGMAudioEvent;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_SEAudioEvent)
            {
                ref var evt = ref m_SEAudioEvent.Get1(i);

                SoundManager.Current.PlaySEAsync(evt.audioAsset).Forget();
            }

            foreach (var i in m_BGMAudioEvent)
            {
                ref var evt = ref m_BGMAudioEvent.Get1(i);

                SoundManager.Current.PlayBGMAsync(evt.audioAsset).Forget();
            }
        }
    }
}