using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    sealed class GameCtrlSystem : IEcsRunSystem
    {
        readonly GameContext m_GameCtx;

        EcsFilter<GameStartRequest> m_GameStartRequests;
        EcsFilter<PieceBagComponent> m_Bags;

        void IEcsRunSystem.Run()
        {
            foreach (var i in m_GameStartRequests)
            {
                var gameStartRequest = m_GameStartRequests.Get1(i);

                foreach (var ii in m_Bags)
                {
                    m_GameCtx.SendMessage(new PieceNextRequest { });
                }

                m_GameCtx.SendMessage(new BGMAudioEvent { audioAsset = "BGM/bgm_t02_swap_t.wav" });
            }
        }
    }
}