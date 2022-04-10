//using Saro.Audio;
//using Tetris.Save;

//namespace Tetris
//{
//    public static partial class SoundManagerEx
//    {
//        public static void ApplySettings(this SoundManager self)
//        {
//            var gameSettings = SaveManager.Current.GetSaveData<GameSettings>();
//            self.VolumeBGM = gameSettings.volumeBGM;
//            self.VolumeSE = gameSettings.volumeSE;
//        }

//        public static void StoreSettings(this SoundManager self)
//        {
//            var gameSettings = SaveManager.Current.GetSaveData<GameSettings>();
//            gameSettings.volumeBGM = self.VolumeBGM;
//            gameSettings.volumeSE = self.VolumeSE;
//        }
//    }
//}
