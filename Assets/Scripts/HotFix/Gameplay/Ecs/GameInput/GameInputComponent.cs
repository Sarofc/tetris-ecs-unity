using Saro.Entities;

namespace Tetris
{
    public struct GameInputComponent : IEcsComponentSingleton
    {
        public float lastInputTime;
        public float lastStartTime;
        public bool leftPressed;
        public bool rightPressed;
        public bool downPressed;
    }
}