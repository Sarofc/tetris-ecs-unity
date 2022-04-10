using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using Saro;

namespace Tetris
{
    internal sealed class PieceRotateSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world;
        private readonly GameContext m_GameCtx;
        [EcsIgnoreInject]
        private EcsEntity[][] m_Grid => m_GameCtx.grid;

        private EcsFilter<PieceRotationRequest> m_Requests;
        private EcsFilter<PieceRotateFlag, ComponentList<EcsEntity>> m_Pieces;

        void IEcsRunSystem.Run()
        {
            foreach (var i1 in m_Requests)
            {
                ref var request = ref m_Requests.Get1(i1);

                foreach (var i2 in m_Pieces)
                {
                    ref var ePiece = ref m_Pieces.GetEntity(i2);

                    RotateBlock(ePiece, request.clockwise);
                }
            }
        }

        private void RotateBlock(in EcsEntity ePiece, bool clockwise)
        {
            TetrisUtil.RotateBlockWithoutCheck(ePiece, clockwise);

            ref var cPiece = ref ePiece.Get<PieceComponent>();
            ref var state = ref cPiece.state;
            var next = clockwise ? state + 1 : state - 1;
            if (next < 0) next = 3;
            else if (next > 3) next = 0;

            bool rotateSuccess = false;
            if (!TetrisUtil.IsValidBlock(m_Grid, ePiece))
            {
                if (TetrisUtil.WallKickTest(m_Grid, ePiece, next, out var result)) // 测试成功
                {
                    Log.INFO($"wallkick: {result}");
                    state = next;

                    TetrisUtil.MovePiece(m_Grid, ePiece, result);

                    rotateSuccess = true;
                }
                else // 测试失败，还原旋转
                {
                    TetrisUtil.RotateBlockWithoutCheck(ePiece, !clockwise);
                    Log.INFO($"wallkick failed");
                }
            }
            else
            {
                state = next;
                rotateSuccess = true;
            }

            if (rotateSuccess)
            {
                m_GameCtx.lastOpIsRotate = true;

                m_GameCtx.SendMessage(new PieceRotationSuccess { });

                m_GameCtx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });
            }

            m_GameCtx.SendMessage(new SEAudioEvent { audioAsset = "SE/se_game_rotate.wav" });
        }
    }
}