using Cysharp.Threading.Tasks;
using Saro.Core;
using XLua;

namespace Saro
{
    [LuaCallCSharp]
    public static class SceneManagerLuaEx
    {
        public static async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            var scenehandle = Main.Resolve<IAssetInterface>().LoadSceneAsync(sceneName);
            await scenehandle;

            // TODO 卸载了bundle，场景bundle里的资源就都被卸载了，例如material就会missing
            // 目前 非additive加载方式，XAssetManager内部会自动卸载
            // 针对additive加载方式，XAssetManager里，也管理下？
            //scenehandle.DecreaseRefCount();
        }
    }
}
