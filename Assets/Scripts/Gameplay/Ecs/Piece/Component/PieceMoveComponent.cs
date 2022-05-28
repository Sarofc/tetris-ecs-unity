using Leopotam.EcsLite;

namespace Tetris
{
    public enum EDropType
    {
        Normal,
        Soft,
        Hard
    }

    public struct PieceMoveComponent : IEcsComponent
    {
        public EDropType dropType;
        public float lastFallTime;
    }
}