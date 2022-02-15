using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLua;
using Cysharp.Threading.Tasks;

namespace Saro
{
    [LuaCallCSharp]
    public static class SceneManagerLuaEx
    {
        public static async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            var scenehandle = XAsset.XAssetComponent.Current.LoadSceneAsync(sceneName);
            await scenehandle;
            scenehandle.DecreaseRefCount();
        }
    }
}
