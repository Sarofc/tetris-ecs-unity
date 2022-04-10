using Cysharp.Threading.Tasks;
using Saro.Localization;

namespace Saro.Lua
{
    [XLua.LuaCallCSharp]
    public static partial class LocalizationManagerEx
    {
        public static int GetLanguage(this LocalizationManager self)
        {
            return (int)self.CurrentLanguage;
        }

        public static void SetLanguage(this LocalizationManager self, int language)
        {
            self.SetLanguage((ELanguage)language);
        }

        public static async UniTask SetLanguageAsync(this LocalizationManager self, int language)
        {
            await self.SetLanguageAsync((ELanguage)language);
        }
    }
}
