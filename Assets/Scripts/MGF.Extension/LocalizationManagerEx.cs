//using Saro.Localization;
//using Tetris.Save;

//namespace Tetris
//{
//    public static partial class LocalizationManagerEx
//    {
//        public static void ApplySettings(this LocalizationManager self)
//        {
//            var gameSettings = SaveManager.Current.GetSaveData<GameSettings>();
//            if (gameSettings.language >= 0)
//                self.CurrentLanguage = (ELanguage)gameSettings.language;
//        }

//        public static void StoreSettings(this LocalizationManager self)
//        {
//            var gameSettings = SaveManager.Current.GetSaveData<GameSettings>();
//            if (gameSettings.language >= 0)
//                gameSettings.language = (int)self.CurrentLanguage;
//        }
//    }
//}
