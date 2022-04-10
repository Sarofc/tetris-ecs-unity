using Leopotam.Ecs;

namespace Tetris
{
    public struct LineClearRequest
    {
        public EcsEntity ePiece;
        public int startLine;
        public int endLine;

        public override string ToString()
        {
            return $"{nameof(LineClearRequest)} {startLine}-{endLine}";
        }
    }
}
