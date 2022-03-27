
namespace Tetris
{
    public enum EDropType
    {
        Normal,
        Soft,
        Hard,
    }

    public struct PieceMoveComponent
    {
        public EDropType dropType;
        public float lastFallTime;
    }
}
