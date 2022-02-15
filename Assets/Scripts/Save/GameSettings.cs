using Saro.SaveSystem;
using System;

namespace Tetris.Save
{
    public sealed class GameSettings : ISaveData
    {
        public float volumeBGM = 1f;
        public float volumeSE = 1f;
    }
}
