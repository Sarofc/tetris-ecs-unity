using Leopotam.EcsLite;
using UnityEngine;

namespace Tetris
{
    internal sealed class GameTimeSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            // var world = systems.GetWorld();
            var gameCtx = systems.GetShared<GameContext>();

            if (gameCtx.gamming) gameCtx.gameTime += Time.deltaTime;
        }
    }
}