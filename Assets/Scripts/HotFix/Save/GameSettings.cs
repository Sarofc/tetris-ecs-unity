using Saro.SaveSystem;

namespace Tetris.Save
{
    public sealed class GameSettings : ISaveData
    {
        public int language = 0;

        public float volumeBGM = 1f;
        public float volumeSE = 1f;
    }
}
