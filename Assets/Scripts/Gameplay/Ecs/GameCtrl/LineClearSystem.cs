using Leopotam.EcsLite;
using Saro;
using Saro.Events;
using UnityEngine;

namespace Tetris
{
    internal sealed class LineClearSystem : IEcsRunSystem
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var gameCtx = systems.GetShared<GameContext>();

            var lineToClear = gameCtx.lineToClear;

            var lineRequest = world.Filter().Inc<LineClearRequest>().End();
            var lineDelay = world.Filter().Inc<LineClearDelayRequest>().Exc<DelayComponent>().End();

            if (lineRequest.GetEntitiesCount() > 0)
            {
                //m_LineToClear.Clear();
                var request = lineRequest[0].Get<LineClearRequest>(world);
                for (var j = request.endLine; j >= request.startLine; j--)
                {
                    var clear = true;
                    for (var k = 0; k < TetrisDef.Width; k++)
                        if (!gameCtx.grid[j][k].IsAlive(world))
                        {
                            clear = false;
                            break;
                        }

                    if (clear)
                    {
                        ClearLine(world, gameCtx, j);

                        lineToClear.Add(j);
                        gameCtx.SendMessage(new EffectEvent
                            { effectAsset = "vfx_clear_line.prefab", effectPosition = new Vector3(0, j) });
                    }
                }

                if (lineToClear.Count > 0)
                {
                    gameCtx.SendMessage(new LineClearDelayRequest { lineToClear = lineToClear },
                        new DelayComponent { delay = TetrisDef.LineClearDelay });
                    gameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = EcsPackedEntity.k_Null });
                }
                else
                {
                    gameCtx.SendMessage(new PieceNextRequest());
                }

                // score
                {
                    var clearLineCount = lineToClear.Count;
                    if (clearLineCount <= 0) gameCtx.ren = -1;
                    else gameCtx.ren += 1;
                    gameCtx.line += clearLineCount;
                    var (isTSpin, isMini) = TetrisUtil.IsTSpin(world, gameCtx.grid, request.ePiece);
                    isTSpin &= gameCtx.lastOpIsRotate;
                    var isSpecial = isTSpin || clearLineCount == 4;

                    var level = gameCtx.level;
                    var score = 0;

                    if (isTSpin)
                    {
                        Log.INFO($"TSpin {clearLineCount} {(isMini ? "Mini" : "")}");

                        if (isMini)
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
                            }
                        else
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
                            }

                        gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_special.wav" });
                    }
                    else
                    {
                        switch (clearLineCount)
                        {
                            case 1:
                                score = 100 * level;
                                gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_single.wav" });
                                //vfx.PlayClip(SV.ClipSingle);
                                break;
                            case 2:
                                score = 300 * level;
                                gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_double.wav" });
                                //vfx.PlayClip(SV.ClipDouble);
                                break;
                            case 3:
                                score = 500 * level;
                                gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_triple.wav" });
                                //vfx.PlayClip(SV.ClipTriple);
                                break;
                            case 4:
                                score = 800 * level;
                                gameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_tetris.wav" });
                                //vfx.PlayClip(SV.ClipTetris);
                                //vfx.TextVFX_Tetris();
                                break;
                        }
                    }

                    var isB2B = false;
                    if (gameCtx.lastClearIsSpecial && isSpecial) isB2B = true;

                    if (isB2B) score = (int)(score * 1.5f);

                    gameCtx.score += score;

                    gameCtx.lastClearIsSpecial = isSpecial;

                    // fire events
                    {
                        // 目前认为消行了，才会改变以下数据
                        if (clearLineCount > 0)
                        {
                            var args = TetrisScoreEventArgs.Create();
                            args.line = gameCtx.line;
                            args.score = gameCtx.score;
                            args.level = gameCtx.level;
                            EventManager.Global.BroadcastQueued(this, args);
                        }
                    }

                    {
                        var args = TetrisLineClearArgs.Create();
                        args.isTSpin = isTSpin;
                        args.isMini = isMini;
                        args.line = clearLineCount;
                        args.isB2B = isB2B;
                        args.ren = gameCtx.ren;
                        EventManager.Global.BroadcastQueued(this, args);
                    }
                }
            }

            if (lineDelay.GetEntitiesCount() > 0)
            {
                for (var k = 0; k < lineToClear.Count; k++)
                {
                    var line = lineToClear[k];
                    DownLines(world, gameCtx, line);
                }

                lineToClear.Clear();

                gameCtx.SendMessage(new PieceNextRequest());

                // 销毁消息实体
                var ent = lineDelay[0];
                world.DelEntity(ent);
            }
        }

        private void ClearLine(EcsWorld world, GameContext gameCtx, int line)
        {
            var lineToClear = gameCtx.grid[line];
            for (var j = 0; j < lineToClear.Length; j++)
            {
                ref var eTile = ref lineToClear[j];

                world.DelEntity(eTile.id);
            }
        }

        private void DownLines(EcsWorld world, GameContext gameCtx, int line)
        {
            var lineToClear = gameCtx.grid[line];

            var grid = gameCtx.grid;
            var i = line + 1;
            for (; i < TetrisDef.Height + TetrisDef.ExtraHeight; i++)
            {
                for (var k = 0; k < grid[k].Length; k++)
                {
                    ref var eTile = ref grid[i][k];
                    if (!eTile.IsAlive(world)) continue;
                    ref var cPos = ref eTile.Get<PositionComponent>(world);
                    ref var posY = ref cPos.position.y;
                    posY--;
                }

                grid[i - 1] = grid[i];
            }
            grid[i - 1] = lineToClear;
        }
    }
}