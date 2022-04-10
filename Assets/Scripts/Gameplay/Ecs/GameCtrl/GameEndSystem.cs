using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Saro.UI;
using Saro.Lua.UI;

namespace Tetris
{
    internal sealed class GameEndSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;
        private EcsFilter<GameEndComponent>.Exclude<DelayComponent> m_GameEndRequest;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_GameEndRequest)
            {
                ref var msgEnt = ref m_GameEndRequest.GetEntity(i);
                msgEnt.Destroy();

                m_GameCtx.gamming = false;

                m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/me_game_gameover.wav" });

                UIManager.Current.OpenUIAsync("UIGameOverPanel").Forget();
            }
        }
    }
}