using Leopotam.Ecs;

namespace Tetris
{
    internal sealed class GameTimeSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;

        void IEcsRunSystem.Run()
        {
            if (m_GameCtx.gamming)
            {
                m_GameCtx.gameTime += UnityEngine.Time.deltaTime;
            }
        }
    }
}