using Leopotam.EcsLite;

namespace Tetris
{
    public struct PieceRotationRequest : IEcsComponent
    {
        public bool clockwise;
    }
}