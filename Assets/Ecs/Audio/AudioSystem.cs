using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using Saro.Audio;
using UnityEngine;

namespace Tetris
{
    sealed class AudioSystem : IEcsRunSystem
    {
        EcsFilter<SEAudioEvent> m_SEAudioEvent;
        EcsFilter<BGMAudioEvent> m_BGMAudioEvent;

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