using Saro.Audio;
using Tetris.Save;

namespace Tetris
{
    [XLua.LuaCallCSharp]
    public static partial class SoundComponentEx
    {
        public static void ApplySettings(this SoundComponent self)
        {
            var gameSettings = SaveComponent.Current.GetSaveData<GameSettings>();
            self.VolumeBGM = gameSettings.volumeBGM;
            self.VolumeSE = gameSettings.volumeSE;
        }

        public static void StoreSettings(this SoundComponent self)
        {
            var gameSettings = SaveComponent.Current.GetSaveData<GameSettings>();
            gameSettings.volumeBGM = self.VolumeBGM;
            gameSettings.volumeSE = self.VolumeSE;
        }
    }
}
