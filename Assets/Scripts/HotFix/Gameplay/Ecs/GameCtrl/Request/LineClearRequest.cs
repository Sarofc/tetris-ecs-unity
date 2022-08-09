using Leopotam.EcsLite;

namespace Tetris
{
    public struct LineClearRequest : IEcsComponent
    {
        public EcsPackedEntity ePiece;
        public int startLine;
        public int endLine;

        public override string ToString()
        {
            return $"{nameof(LineClearRequest)} {startLine}-{endLine}";
        }
    }
}