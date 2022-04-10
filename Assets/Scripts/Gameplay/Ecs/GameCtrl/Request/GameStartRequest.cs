namespace Tetris
{
    public struct GameStartRequest
    {
        public int gameMode;

        public override string ToString()
        {
            return $"{nameof(GameStartRequest)}=[{gameMode}]";
        }
    }
}
