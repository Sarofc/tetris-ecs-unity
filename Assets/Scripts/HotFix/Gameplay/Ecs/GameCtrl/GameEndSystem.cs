using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Saro.UI;
using Tetris.UI;

namespace Tetris
{
    internal sealed class GameEndSystem : IEcsRunSystem
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var gameCtx = systems.GetShared<GameContext>();
            var gameEndRequest = world.Filter().Inc<GameEndComponent>().Exc<DelayComponent>().End();
            foreach (var msgEnt in gameEndRequest)
            {
                world.DelEntity(msgEnt);

                gameCtx.gamming = false;

                gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/me_game_gameover.wav" });

                UIManager.Instance.LoadAndShowWindowAsync(ETetrisUI.GameOverPanel).Forget();
                //UIManager.Current.OpenUIAsync<UIGameOverPanel>().Forget();
            }
        }
    }
}