using Saro.Entities;

namespace Tetris
{
    public struct GameInputComponent : IEcsComponentSingleton
    {
        // 临时状态
        public float lastInputTime;
        public float lastStartTime;
        public bool leftPressed;
        public bool rightPressed;
        public bool downPressed;

        // 按键操作
        public bool spaceDown;
        public bool downArrowDown, downArrowUp;
        public bool leftArrowDown, leftArrowUp, leftArrowPressing;
        public bool rightArrowDown, rightArrowUp, rightArrowPressing;
        public bool zDown;
        public bool xDown;
        public bool cDown;
    }
}