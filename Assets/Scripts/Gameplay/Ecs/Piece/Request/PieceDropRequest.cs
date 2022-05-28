using Leopotam.EcsLite;

namespace Tetris
{
    public struct PieceDropRequest : IEcsComponent
    {
        public EDropType dropType;

        public override string ToString()
        {
            return $"{nameof(PieceDropRequest)} {dropType}";
        }
    }
}