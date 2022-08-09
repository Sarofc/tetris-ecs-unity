using Leopotam.EcsLite;

namespace Tetris
{
    public struct GameInputComponent : IEcsComponent
    {
        public float lastInputTime;
        public float lastStartTime;
        public bool leftPressed;
        public bool rightPressed;
        public bool downPressed;
    }
}