using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    sealed class BoardCreateSystem : IEcsInitSystem
    {
        // auto-injected fields.
        readonly EcsWorld m_World = null;
        readonly GameplayAssets m_GameplayAssets;

        public void Init()
        {
            //var grid = new int[TetrisDef.k_Width, TetrisDef.k_Height + TetrisDef.k_ExtraHeight];
            //ent.Replace(new TetrisBoardComponent { grid = grid });

            CreateView();

            m_World.SendMessage(new GameStartRequest { gameMode = 1 });
        }

        private void CreateView()
        {
            var tetrisView = GameObject.Instantiate(m_GameplayAssets.tetrisViewPrefab);

            var linePrefab = m_GameplayAssets.linePrefab;
            var transform = tetrisView.transform;

            const float offset = .5f;

            // draw row line
            for (int i = 0; i <= TetrisDef.k_Height + TetrisDef.k_ExtraHeight - 1; i++)
            {
                var row = GameObject.Instantiate(linePrefab, transform);
                row.positionCount = 2;
                row.SetPosition(0, new Vector3(-offset, i - offset));
                row.SetPosition(1, new Vector3(TetrisDef.k_Width - offset, i - offset));
            }

            // draw col line
            for (int i = 0; i <= TetrisDef.k_Width; i++)
            {
                var col = GameObject.Instantiate(linePrefab, transform);
                col.positionCount = 2;
                col.SetPosition(0, new Vector3(i - offset, -offset));
                col.SetPosition(1, new Vector3(i - offset, TetrisDef.k_Height + TetrisDef.k_ExtraHeight - 1 - offset));
            }
        }
    }
}