using Saro.Audio;
using Tetris.Save;

namespace Tetris
{
    public static class SoundManagerEx
    {
        public static void ApplySettings(this AudioManager self)
        {
            var gameSettings = SaveManager.Current.GetSaveData<GameSettings>();
            self.VolumeBGM = gameSettings.volumeBGM;
            self.VolumeSE = gameSettings.volumeSE;
        }

        public static void StoreSettings(this AudioManager self)
        {
            var gameSettings = SaveManager.Current.GetSaveData<GameSettings>();
            gameSettings.volumeBGM = self.VolumeBGM;
            gameSettings.volumeSE = self.VolumeSE;
        }
    }
}