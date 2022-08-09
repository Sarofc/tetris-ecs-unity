using Leopotam.EcsLite;

namespace Tetris
{
    public struct PieceGhostUpdateRequest : IEcsComponent
    {
        public EcsPackedEntity ePiece;
    }
}