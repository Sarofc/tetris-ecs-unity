using Saro.Entities;
using Saro.Entities.Extension;
using UnityEngine;

namespace Tetris
{
    internal sealed class BoardCreateSystem : IEcsInitSystem
    {
        private GameContext m_GameCtx;

        public void Init(EcsSystems systems)
        {
            m_GameCtx = systems.GetShared<GameContext>();

            CreateView();

            systems.GetWorld().SendMessage(new GameStartRequest { gameMode = 1 });
        }

        private void CreateView()
        {
            var tetrisView = Object.Instantiate(m_GameCtx.gameplayAssets.tetrisBoard);

            var linePrefab = m_GameCtx.gameplayAssets.linePrefab;
            var transform = tetrisView.transform;

            const float k_Offset = .5f;

            // draw row line
            for (var i = 0; i <= TetrisDef.Height + TetrisDef.ExtraHeight - 1; i++)
            {
                var row = Object.Instantiate(linePrefab, transform);
                row.positionCount = 2;
                row.SetPosition(0, new Vector3(-k_Offset, i - k_Offset, -0.1f));
                row.SetPosition(1, new Vector3(TetrisDef.Width - k_Offset, i - k_Offset, -0.1f));
            }

            // draw col line
            for (var i = 0; i <= TetrisDef.Width; i++)
            {
                var col = Object.Instantiate(linePrefab, transform);
                col.positionCount = 2;
                col.SetPosition(0, new Vector3(i - k_Offset, -k_Offset, -0.1f));
                col.SetPosition(1,
                    new Vector3(i - k_Offset, TetrisDef.Height + TetrisDef.ExtraHeight - 1 - k_Offset, -0.1f));
            }
        }
    }
}