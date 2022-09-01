using Saro.Entities;
using Saro.Entities.Extension;
using Saro;
using UnityEngine;

namespace Tetris
{
    internal sealed class AddToGridSystem : IEcsRunSystem
    {
        private GameContext m_GameCtx;
        private EcsEntity[][] Grid => m_GameCtx.grid;

        public bool Enable { get; set; } = true;
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            m_GameCtx = systems.GetShared<GameContext>();

            var world = systems.GetWorld();

            var pieces = world.Filter().Inc<PieceMoveComponent, ComponentList<EcsEntity>, AddToGridComponent>()
                .Exc<DelayComponent>()
                .End();

            //foreach (var i1 in m_Requests)
            {
                foreach (var ePiece in pieces)
                {
                    ref var cPiecePosition = ref ePiece.Get<PositionComponent>(world);
                    var tileList = ePiece.Get<ComponentList<EcsEntity>>(world).Value;

                    var isGameOver = true;

                    var yMin = int.MaxValue;
                    var yMax = 0;

                    for (var i = 0; i < tileList.Count; i++)
                    {
                        var eTile = tileList[i];
                        ref var cTilePos = ref eTile.Get<PositionComponent>();
                        var pos = cTilePos.position + cPiecePosition.position;
                        var x = Mathf.RoundToInt(pos.x);
                        var y = Mathf.RoundToInt(pos.y);

                        if (y < TetrisDef.Height) isGameOver = false;

                        Grid[y][x] = eTile;

                        if (y > yMax) yMax = y;
                        if (y < yMin) yMin = y;
                    }

                    ref var cMove = ref ePiece.Get<PieceMoveComponent>(world);
                    FireSound(cMove.dropType);
                    FireEffect(cMove.dropType, ePiece.Get<PositionComponent>(world).position);

                    {
                        //ePiece.Del<PieceComponent>(world);
                        ePiece.Del<PieceMoveComponent>(world);
                        ePiece.Del<PieceRotateFlag>(world);
                        ePiece.Del<AddToGridComponent>(world);
                    }

                    // check game over
                    if (isGameOver)
                    {
                        m_GameCtx.SendMessage(new GameEndComponent(), new DelayComponent { delay = 1f });

                        Log.ERROR("GameOver");
                    }
                    else
                    {
                        m_GameCtx.SendMessage(
                            new LineClearRequest
                                { ePiece = world.Pack(ePiece), startLine = yMin, endLine = yMax });
                    }
                }
            }
        }

        private void FireEffect(EDropType dropType, Vector3 position)
        {
            if (dropType == EDropType.Hard)
                m_GameCtx.SendMessage(new EffectEvent
                    { effectAsset = "vfx_hard_drop.prefab", effectPosition = position });
        }

        private void FireSound(EDropType dropType)
        {
            switch (dropType)
            {
                case EDropType.Normal:
                    m_GameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_landing.wav" });
                    break;
                case EDropType.Soft:
                    m_GameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_softdrop.wav" });
                    break;
                case EDropType.Hard:
                    m_GameCtx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_harddrop.wav" });
                    break;
            }
        }
    }
}