using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using Saro.Pool;
using Saro.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    sealed class LineClearSystem : IEcsRunSystem
    {
        readonly GameContext m_GameCtx;

        EcsFilter<LineClearRequest> m_LineRequest;

        EcsFilter<LineClearDelayRequest>.Exclude<DelayComponent> m_LineDelay;

        List<int> m_LineToClear => m_GameCtx.lineToClear;
        void IEcsRunSystem.Run()
        {
            if (m_LineRequest.GetEntitiesCount() > 0)
            {
                //m_LineToClear.Clear();
                ref var request = ref m_LineRequest.Get1(0);
                for (int j = request.endLine; j >= request.startLine; j--)
                {
                    bool clear = true;
                    for (int k = 0; k < TetrisDef.k_Width; k++)
                    {
                        if (!m_GameCtx.grid[j][k].IsAlive())
                        {
                            clear = false;
                            break;
                        }
                    }

                    if (clear)
                    {
                        ClearLine(j);

                        m_LineToClear.Add(j);
                        m_GameCtx.SendMessage(new EffectEvent { effectAsset = "vfx_clear_line.prefab", effectPosition = new Vector3(0, j) });
                    }
                }

                if (m_LineToClear.Count > 0)
                {
                    m_GameCtx.SendMessage(new LineClearDelayRequest { lineToClear = m_LineToClear }, new DelayComponent { delay = TetrisDef.k_LineClearDelay });
                    m_GameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = EcsEntity.Null });
                }
                else
                {
                    m_GameCtx.SendMessage(new PieceNextRequest { });
                }

                // score
                {
                    var clearLineCount = m_LineToClear.Count;
                    m_GameCtx.clearLineCount += clearLineCount;
                    var (isTSpin, isMini) = TetrisUtil.IsTSpin(request.ePiece);
                    var isSpecial = isTSpin || clearLineCount == 4;



                    if (m_GameCtx.lastClearIsSpecial && isSpecial)
                    {
                        // b2b
                    }
                    m_GameCtx.lastClearIsSpecial = isSpecial;
                }
            }

            if (m_LineDelay.GetEntitiesCount() > 0)
            {
                for (int k = 0; k < m_LineToClear.Count; k++)
                {
                    var line = m_LineToClear[k];
                    DownLines(line);
                }

                m_LineToClear.Clear();

                m_GameCtx.SendMessage(new PieceNextRequest { });

                // 销毁消息实体
                ref var ent = ref m_LineDelay.GetEntity(0);
                ent.Destroy();
            }
        }

        private void ClearLine(int line)
        {
            var lineToClear = m_GameCtx.grid[line];
            for (int j = 0; j < lineToClear.Length; j++)
            {
                ref var eTile = ref lineToClear[j];

                eTile.Destroy();
            }
        }

        private void DownLines(int line)
        {
            var lineToClear = m_GameCtx.grid[line];

            var m_Grid = m_GameCtx.grid;
            int i = line + 1;
            for (; i < TetrisDef.k_Height + TetrisDef.k_ExtraHeight; i++)
            {
                for (int k = 0; k < m_Grid[k].Length; k++)
                {
                    ref var eTile = ref m_Grid[i][k];
                    if (!eTile.IsAlive()) continue;
                    ref var cPos = ref eTile.Get<PositionComponent>();
                    ref var posY = ref cPos.position.y;
                    posY--;
                }

                m_Grid[i - 1] = m_Grid[i];
            }
            m_Grid[i - 1] = lineToClear;
        }
    }
}