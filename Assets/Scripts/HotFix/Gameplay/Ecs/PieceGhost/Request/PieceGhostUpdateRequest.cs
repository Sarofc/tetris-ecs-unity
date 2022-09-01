using Saro.Entities;

namespace Tetris
{
    public struct PieceGhostUpdateRequest : IEcsComponent
    {
        public EcsEntity ePiece;
    }
}