using Saro.Entities;

namespace Tetris
{
    public struct GameStartRequest : IEcsComponent
    {
        public int gameMode;

        public override string ToString()
        {
            return $"{nameof(GameStartRequest)}=[{gameMode}]";
        }
    }
}