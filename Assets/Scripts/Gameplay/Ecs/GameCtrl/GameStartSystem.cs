using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Saro.Lua.UI;
using Saro.UI;

namespace Tetris
{
    internal sealed class GameStartSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;
        private EcsFilter<GameStartRequest> m_GameStartRequests;
        private EcsFilter<PieceBagComponent> m_Bags;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_GameStartRequests)
            {
                var gameStartRequest = m_GameStartRequests.Get1(i);

                foreach (var ii in m_Bags)
                {
                    m_GameCtx.SendMessage(new PieceNextRequest { });
                }

                m_GameCtx.gamming = true;
                m_GameCtx.SendMessage(new BGMAudioEvent { audioAsset = "BGM/bgm_t02_swap_t.wav" });

                OpenUI();
            }
        }

        private async void OpenUI()
        {
            var uihud = await UIManager.Current.OpenUIAsync("UIGameHUD") as XLuaUI;
            uihud.SetUserData("gameCtx", m_GameCtx);
        }
    }
}