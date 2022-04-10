using Leopotam.Ecs;
using Saro;
using Saro.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    internal sealed class LineClearSystem : IEcsRunSystem
    {
        private readonly GameContext m_GameCtx;
        private EcsFilter<LineClearRequest> m_LineRequest;
        private EcsFilter<LineClearDelayRequest>.Exclude<DelayComponent> m_LineDelay;

        private List<int> m_LineToClear => m_GameCtx.lineToClear;
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
                    if (clearLineCount <= 0) m_GameCtx.ren = -1;
                    else m_GameCtx.ren += 1;
                    m_GameCtx.line += clearLineCount;
                    var (isTSpin, isMini) = TetrisUtil.IsTSpin(m_GameCtx.grid, request.ePiece);
                    isTSpin &= m_GameCtx.lastOpIsRotate;
                    var isSpecial = isTSpin || clearLineCount == 4;

                    var level = m_GameCtx.level;
                    int score = 0;

                    if (isTSpin)
                    {
                        Log.INFO($"TSpin {clearLineCount} {(isMini ? "Mini" : "")}");

                        if (isMini)
                        {
                            switch (clearLineCount)
                            {
                                case 1:
                                    score = 200 * level;
                                    //vfx.TextVFX_TSpinMiniSingle();
                                    break;
                                case 2:
                                    score = 1200 * level;
                                    //vfx.TextVFX_TSpinMiniDouble();
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (clearLineCount)
                            {
                                case 1:
                                    score = 800 * level;
                                    //vfx.TextVFX_TSpinSingle();
                                    break;
                                case 2:
                                    score = 1200 * level;
                                    //vfx.TextVFX_TSpinDouble();
                                    break;
                                case 3:
                                    score = 1600 * level;
                                    //vfx.TextVFX_TSpinTriple();
                                    break;
                                default:
                                    break;
                            }
                        }

                        m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_special.wav" });
                    }
                    else
                    {
                        switch (clearLineCount)
                        {
                            case 1:
                                score = 100 * level;
                                m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_single.wav" });
                                //vfx.PlayClip(SV.ClipSingle);
                                break;
                            case 2:
                                score = 300 * level;
                                m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_double.wav" });
                                //vfx.PlayClip(SV.ClipDouble);
                                break;
                            case 3:
                                score = 500 * level;
                                m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_triple.wav" });
                                //vfx.PlayClip(SV.ClipTriple);
                                break;
                            case 4:
                                score = 800 * level;
                                m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_tetris.wav" });
                                //vfx.PlayClip(SV.ClipTetris);
                                //vfx.TextVFX_Tetris();
                                break;
                            default:
                                break;
                        }
                    }

                    bool isB2B = false;
                    if (m_GameCtx.lastClearIsSpecial && isSpecial)
                    {
                        isB2B = true;
                    }

                    if (isB2B)
                    {
                        score = (int)(score * 1.5f);
                    }

                    m_GameCtx.score += score;

                    m_GameCtx.lastClearIsSpecial = isSpecial;

                    // fire events
                    {
                        // 目前认为消行了，才会改变以下数据
                        if (clearLineCount > 0)
                        {
                            var args = TetrisScoreEventArgs.Create();
                            args.line = m_GameCtx.line;
                            args.score = m_GameCtx.score;
                            args.level = m_GameCtx.level;
                            EventManager.Global.BroadcastQueued(this, args);
                        }
                    }

                    {
                        var args = TetrisLineClearArgs.Create();
                        args.isTSpin = isTSpin;
                        args.isMini = isMini;
                        args.line = clearLineCount;
                        args.isB2B = isB2B;
                        args.ren = m_GameCtx.ren;
                        EventManager.Global.BroadcastQueued(this, args);
                    }
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