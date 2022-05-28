using Leopotam.EcsLite;
using Leopotam.EcsLite.Extension;
using Saro;

namespace Tetris
{
    internal sealed class PieceRotateSystem : IEcsRunSystem
    {
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var gameCtx = systems.GetShared<GameContext>();
            var world = systems.GetWorld();
            var requests = world.Filter().Inc<PieceRotationRequest>().End();
            var pieces = world.Filter().Inc<PieceRotateFlag, ComponentList<EcsPackedEntity>>().End();

            foreach (var i1 in requests)
            {
                ref var request = ref i1.Get<PieceRotationRequest>(world);

                foreach (var i2 in pieces)
                {
                    var ePiece = world.PackEntity(i2);

                    RotateBlock(world, gameCtx, ePiece, request.clockwise);
                }
            }
        }

        private void RotateBlock(EcsWorld world, GameContext ctx, in EcsPackedEntity ePiece, bool clockwise)
        {
            TetrisUtil.RotateBlockWithoutCheck(world, ePiece, clockwise);

            ref var cPiece = ref ePiece.Get<PieceComponent>(world);
            ref var state = ref cPiece.state;
            var next = clockwise ? state + 1 : state - 1;
            if (next < 0) next = 3;
            else if (next > 3) next = 0;

            var rotateSuccess = false;
            if (!TetrisUtil.IsValidBlock(world, ctx.grid, ePiece))
            {
                if (TetrisUtil.WallKickTest(world, ctx.grid, ePiece, next, out var result)) // ���Գɹ�
                {
                    Log.INFO($"wallkick: {result}");
                    state = next;

                    TetrisUtil.MovePiece(world, ctx.grid, ePiece, result);

                    rotateSuccess = true;
                }
                else // ����ʧ�ܣ���ԭ��ת
                {
                    TetrisUtil.RotateBlockWithoutCheck(world, ePiece, !clockwise);
                    Log.INFO("wallkick failed");
                }
            }
            else
            {
                state = next;
                rotateSuccess = true;
            }

            if (rotateSuccess)
            {
                ctx.lastOpIsRotate = true;

                ctx.SendMessage(new PieceRotationSuccess());

                ctx.SendMessage(new PieceGhostUpdateRequest { ePiece = ePiece });
            }

            ctx.SendMessage(new SeAudioEvent { audioAsset = "SE/se_game_rotate.wav" });
        }
    }
}