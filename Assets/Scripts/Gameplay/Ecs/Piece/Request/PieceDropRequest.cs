namespace Tetris
{
    public struct PieceDropRequest
    {
        public EDropType dropType;

        public override string ToString()
        {
            return $"{nameof(PieceDropRequest)} {dropType}";
        }
    }
}
