using System.Collections.Generic;
using Saro.Entities;
using Saro.Diagnostics;
using UnityEngine;

namespace Tetris
{
    internal sealed class TileRendererSystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        public const int MaxBatchCount = 1023;
        private List<Vector4[]> m_ColorBatches;
        private GameContext m_GameContext;
        private List<Vector4[]> m_SpriteOffsetBatches;

        private List<Matrix4x4[]> m_TransfromMatrixBatches;
        public bool Enable { get; set; } = true;
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            m_GameContext.batchRenderer.Destroy();
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            m_GameContext = systems.GetShared<GameContext>();
            m_TransfromMatrixBatches = m_GameContext.TransfromMatrixBatches = new List<Matrix4x4[]>();
            m_ColorBatches = m_GameContext.ColorBatches = new List<Vector4[]>();
            m_SpriteOffsetBatches = m_GameContext.SpriteOffsetBatches = new List<Vector4[]>();

            m_GameContext.batchRenderer.Init();
            m_GameContext.batchRenderer.TransformMatrixBatches = m_GameContext.TransfromMatrixBatches;
            m_GameContext.batchRenderer.ColorBatches = m_GameContext.ColorBatches;
            m_GameContext.batchRenderer.SpriteOffsetBatches = m_GameContext.SpriteOffsetBatches;

            m_GameContext.batchRenderer.SetRenderingCamera(Camera.main);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter().Inc<TileRendererComponent, PositionComponent, ParentComponent>().End();
            var index = 0;
            foreach (var i in filter)
            {
                ref var cTileRenderer = ref i.Get<TileRendererComponent>(world);
                ref var cPos = ref i.Get<PositionComponent>(world);
                ref var cParent = ref i.Get<ParentComponent>(world);

                var scale = cParent.parent.Get<PieceComponent>().scale;
                var pos = cPos.position * scale + cParent.parent.Get<PositionComponent>().position;
                //DrawTile(cPos.position * scale + cParent.parent.Get<PositionComponent>().position, scale);

                var batchIndex = index / MaxBatchCount;
                var elementIndex = index % MaxBatchCount;

                if (m_TransfromMatrixBatches.Count <= batchIndex)
                {
                    m_TransfromMatrixBatches.Add(new Matrix4x4[MaxBatchCount]);
                    m_ColorBatches.Add(new Vector4[MaxBatchCount]);

                    var offsets = new Vector4[MaxBatchCount];
                    m_SpriteOffsetBatches.Add(offsets);
                    for (var k = 0; k < offsets.Length; k++) offsets[k] = new Vector4(1, 1, 0, 0);
                }

                {
                    ref var matrix = ref cTileRenderer.matrix;
                    var newScale = Vector3.one * scale * 0.9f;
                    matrix.SetTRS(pos, Quaternion.identity, newScale);

                    m_TransfromMatrixBatches[batchIndex][elementIndex] = matrix;
                    m_ColorBatches[batchIndex][elementIndex] = cTileRenderer.color;
                    //m_SpriteOffsetBatches[batchIndex][elementIndex] = GetTextureOffset(cTileRenderer.spriteIndex);
                }

                index++;
            }

            for (; index < m_TransfromMatrixBatches.Count * MaxBatchCount; index++)
            {
                var batchIndex = index / MaxBatchCount;
                var elementIndex = index % MaxBatchCount;

                m_TransfromMatrixBatches[batchIndex][elementIndex] = Matrix4x4.zero;
                //m_ColorBatches[batchIndex][elementIndex] = Color.white;
            }

            m_GameContext.batchRenderer.BatchAndRender();
        }

        private void DrawTile(Vector3 pos, float scale)
        {
            GLDebug.DebugBox(pos, Vector3.one * 0.45f * scale, Color.red, Quaternion.identity, Time.deltaTime,
                EGLDebug.Game);
        }

        private void DrawPivot(Vector3 pos)
        {
            GLDebug.DebugCircle(pos, Vector3.forward, Color.red, 0.1f, Time.deltaTime, EGLDebug.Game);
        }
    }
}