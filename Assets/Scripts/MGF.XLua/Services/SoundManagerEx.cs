using Cysharp.Threading.Tasks;
using Saro.Audio;

namespace Saro.Lua
{
    [XLua.LuaCallCSharp]
    public static partial class SoundManagerEx
    {
        public static void SetVolumeBGM(this SoundManager self, float val)
        {
            self.VolumeBGM = val;
        }

        public static float GetVolumeBGM(this SoundManager self)
        {
            return self.VolumeBGM;
        }

        public static void SetVolumeSE(this SoundManager self, float val)
        {
            self.VolumeSE = val;
        }

        public static float GetVolumeSE(this SoundManager self)
        {
            return self.VolumeSE;
        }

        public static async UniTask PlayBGMAsync(this SoundManager self, string assetName)
        {
            await self.PlayBGMAsync(assetName);
        }

        public static async UniTask PlaySEAsync(this SoundManager self, string assetName)
        {
            await self.PlaySEAsync(assetName);
        }
    }
}
