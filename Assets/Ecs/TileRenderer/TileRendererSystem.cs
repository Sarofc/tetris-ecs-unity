using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using Saro.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    sealed class TileRendererSystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        readonly GameContext m_GameContext;

        EcsFilter<TileRendererComponent, PositionComponent, ParentComponent> m_Filter;

        private List<Matrix4x4[]> m_TransfromMatrixBatches;
        private List<Vector4[]> m_ColorBatches;
        private List<Vector4[]> m_SpriteOffsetBatches;

        void IEcsInitSystem.Init()
        {
            m_TransfromMatrixBatches = m_GameContext.TransfromMatrixBatches = new List<Matrix4x4[]>();
            m_ColorBatches = m_GameContext.ColorBatches = new List<Vector4[]>();
            m_SpriteOffsetBatches = m_GameContext.SpriteOffsetBatches = new List<Vector4[]>();

            m_GameContext.batchRenderer.Init();
            m_GameContext.batchRenderer.TransfromMatrixBatches = m_GameContext.TransfromMatrixBatches;
            m_GameContext.batchRenderer.ColorBatches = m_GameContext.ColorBatches;
            m_GameContext.batchRenderer.SpriteOffsetBatches = m_GameContext.SpriteOffsetBatches;

            m_GameContext.batchRenderer.SetRenderingCamera(Camera.main);
        }

        void IEcsDestroySystem.Destroy()
        {
            m_GameContext.batchRenderer.Destroy();
        }

        void IEcsRunSystem.Run()
        {
            var index = 0;
            foreach (var i in m_Filter)
            {
                ref var cTileRenderer = ref m_Filter.Get1(i);
                ref var cPos = ref m_Filter.Get2(i);
                ref var cParent = ref m_Filter.Get3(i);

                var scale = cParent.parent.Get<PieceComponent>().scale;
                var pos = cPos.position * scale + cParent.parent.Get<PositionComponent>().position;
                //DrawTile(cPos.position * scale + cParent.parent.Get<PositionComponent>().position, scale);

                var batchIndex = index / k_MAX_BATCH_COUNT;
                var elementIndex = index % k_MAX_BATCH_COUNT;

                if (m_TransfromMatrixBatches.Count <= batchIndex)
                {
                    m_TransfromMatrixBatches.Add(new Matrix4x4[k_MAX_BATCH_COUNT]);
                    m_ColorBatches.Add(new Vector4[k_MAX_BATCH_COUNT]);

                    var offsets = new Vector4[k_MAX_BATCH_COUNT];
                    m_SpriteOffsetBatches.Add(offsets);
                    for (int k = 0; k < offsets.Length; k++)
                    {
                        offsets[k] = new Vector4(1, 1, 0, 0);
                    }
                }

                {
                    ref var matrix = ref cTileRenderer.matrix;
                    var _scale = Vector3.one * scale * 0.9f;
                    matrix.SetTRS(pos, Quaternion.identity, _scale);

                    m_TransfromMatrixBatches[batchIndex][elementIndex] = matrix;
                    m_ColorBatches[batchIndex][elementIndex] = cTileRenderer.color;
                    //m_SpriteOffsetBatches[batchIndex][elementIndex] = GetTextureOffset(cTileRenderer.spriteIndex);
                }

                index++;
            }

            for (; index < m_TransfromMatrixBatches.Count * k_MAX_BATCH_COUNT; index++)
            {
                var batchIndex = index / k_MAX_BATCH_COUNT;
                var elementIndex = index % k_MAX_BATCH_COUNT;

                m_TransfromMatrixBatches[batchIndex][elementIndex] = Matrix4x4.zero;
                //m_ColorBatches[batchIndex][elementIndex] = Color.white;
            }

            m_GameContext.batchRenderer.BatchAndRender();
        }

        public const int k_MAX_BATCH_COUNT = 1023;

        void DrawTile(Vector3 pos, float scale)
        {
            GLDebugHelper.DebugBox(pos, Vector3.one * 0.45f * scale, Color.red, Quaternion.identity, Time.deltaTime, EGLDebug.Game);
        }

        void DrawPivot(Vector3 pos)
        {
            GLDebugHelper.DebugCircle(pos, Vector3.forward, Color.red, 0.1f, Time.deltaTime, EGLDebug.Game);
        }
    }
}