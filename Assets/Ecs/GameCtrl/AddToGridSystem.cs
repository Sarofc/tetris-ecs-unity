using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using UnityEngine;

namespace Tetris
{
    sealed class AddToGridSystem : IEcsRunSystem
    {
        readonly GameContext m_GameCtx;
        [EcsIgnoreInject]
        EcsEntity[][] m_Grid => m_GameCtx.grid;

        //EcsFilter<AddToGridReuqest> m_Requests;
        EcsFilter<PieceMoveComponent, ComponentList<EcsEntity>, AddToGridComponent>.Exclude<DelayComponent> m_Pieces;

        void IEcsRunSystem.Run()
        {
            //foreach (var i1 in m_Requests)
            {
                foreach (var i2 in m_Pieces)
                {
                    ref var ePiece = ref m_Pieces.GetEntity(i2);
                    ref var cPiecePosition = ref ePiece.Get<PositionComponent>();
                    var tileList = m_Pieces.Get2(i2).Value;

                    bool isGameOver = true;

                    var yMin = int.MaxValue;
                    var yMax = 0;

                    for (int i = 0; i < tileList.Count; i++)
                    {
                        var eTile = tileList[i];
                        ref var cTilePos = ref eTile.Get<PositionComponent>();
                        var pos = cTilePos.position + cPiecePosition.position;
                        var x = Mathf.RoundToInt(pos.x);
                        var y = Mathf.RoundToInt(pos.y);

                        if (y < TetrisDef.k_Height) isGameOver = false;

                        m_Grid[y][x] = eTile;

                        if (y > yMax) yMax = y;
                        if (y < yMin) yMin = y;
                    }

                    ref var cMove = ref ePiece.Get<PieceMoveComponent>();
                    FireSound(cMove.dropType);
                    FireEffect(cMove.dropType, ePiece.Get<PositionComponent>().position);

                    {
                        //ePiece.Del<PieceComponent>();
                        ePiece.Del<PieceMoveComponent>();
                        ePiece.Del<PieceRotateFlag>();
                        ePiece.Del<AddToGridComponent>();
                    }

                    // check game over
                    if (isGameOver)
                    {
                        m_GameCtx.SendMessage(new GameEndRequest { });
                    }
                    else
                    {
                        m_GameCtx.SendMessage(new LineClearRequest { ePiece = ePiece, startLine = yMin, endLine = yMax });
                    }
                }
            }
        }

        void FireEffect(EDropType dropType, Vector3 position)
        {
            if (dropType == EDropType.Hard)
            {
                m_GameCtx.SendMessage(new EffectEvent { effectAsset = "vfx_hard_drop.prefab", effectPosition = position });
            }
        }

        void FireSound(EDropType dropType)
        {
            switch (dropType)
            {
                case EDropType.Normal:
                    m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_landing.wav" });
                    break;
                case EDropType.Soft:
                    m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_softdrop.wav" });
                    break;
                case EDropType.Hard:
                    m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_harddrop.wav" });
                    break;
                default:
                    break;
            }
        }
    }
}