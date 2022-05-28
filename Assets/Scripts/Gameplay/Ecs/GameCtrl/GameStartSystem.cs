using Leopotam.EcsLite;
using Saro.UI;
using Tetris.UI;

namespace Tetris
{
    internal sealed class GameStartSystem : IEcsRunSystem
    {
        private GameContext m_GameCtx;

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            m_GameCtx = systems.GetShared<GameContext>();
            var gameStartRequests = world.Filter().Inc<GameStartRequest>().End();
            foreach (var start in gameStartRequests)
            {
                var bags = world.Filter().Inc<PieceBagComponent>().End();

                var gameStartRequest = start.Get<GameStartRequest>(world);

                foreach (var ii in bags) m_GameCtx.SendMessage(new PieceNextRequest());

                m_GameCtx.gamming = true;
                m_GameCtx.SendMessage(new BGMAudioEvent { audioAsset = "BGM/bgm_t02_swap_t.wav" });

                OpenUI();
            }
        }

        private async void OpenUI()
        {
            await UIManager.Instance.LoadAndShowWindowAsync(ETetrisUI.GameHUD, m_GameCtx);
        }
    }
}