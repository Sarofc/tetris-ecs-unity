using Cysharp.Threading.Tasks;
using Saro.Entities;
using Saro.UI;
using Tetris.UI;

namespace Tetris
{
    internal sealed class GameEndSystem : IEcsRunSystem
    {
        public bool Enable { get; set; } = true;
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var gameCtx = systems.GetShared<GameContext>();
            var gameEndRequest = world.Filter().Inc<GameEndComponent>().Exc<DelayComponent>().End();
            foreach (var msgEnt in gameEndRequest)
            {
                world.DelEntity(msgEnt);

                gameCtx.gamming = false;

                gameCtx.SendMessage(new BGMAudioEvent { audioAsset = null });
                gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/me_game_gameover.wav" });

                UIManager.Current.LoadAndShowWindowAsync(EGameUI.GameOverPanel).Forget();
            }
        }
    }
}