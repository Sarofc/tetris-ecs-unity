using Cysharp.Threading.Tasks;
using Saro.Audio;
using System;

namespace Saro.Lua
{
    [XLua.LuaCallCSharp]
    public static partial class SoundComponentEx
    {
        public static void SetVolumeBGM(this SoundComponent self, float val)
        {
            self.VolumeBGM = val;
        }

        public static float GetVolumeBGM(this SoundComponent self)
        {
            return self.VolumeBGM;
        }

        public static void SetVolumeSE(this SoundComponent self, float val)
        {
            self.VolumeSE = val;
        }

        public static float GetVolumeSE(this SoundComponent self)
        {
            return self.VolumeSE;
        }

        public static async UniTask PlayBGMAsync(this SoundComponent self, string assetName)
        {
            await self.PlayBGMAsync(assetName);
        }

        public static async UniTask PlaySEAsync(this SoundComponent self, string assetName)
        {
            await self.PlaySEAsync(assetName);
        }
    }
}
