using Saro.Entities;

namespace Tetris
{
    public struct PieceRotationRequest : IEcsComponent
    {
        public bool clockwise;
    }
}