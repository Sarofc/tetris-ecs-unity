using Saro.Entities;

namespace Tetris
{
    public struct LineClearRequest : IEcsComponent
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