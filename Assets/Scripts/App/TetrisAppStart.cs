using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Saro;
using Saro.EventDef;
using Saro.Events;
using Saro.Net;
using Saro.UI;
using Saro.XAsset;
using Saro.XAsset.Update;
using UnityEngine;
using Tetris.UI;
using Saro.Audio;
using Saro.Lua.UI;
using Saro.Lua;
using System.IO;
using System.Threading.Tasks;
using Saro.Utility;

namespace Tetris
{
    public sealed class TetrisAppStart : AEvent<AppStart>
    {
        protected override async UniTask Run(AppStart args)
        {
            Main.Instance.gameObject.AddComponent<Saro.Profiler.ProfilerDisplay>();

            FGame.Register<EventComponent>();

            SetupDownloader();

            FGame.Register<XAssetComponent>().Initialize();

            //LoadIFixPatch();

            await FGame.Register<SoundComponent>().InitializeAsync(FGame.Resolve<XAssetComponent>(), "Assets/Res/Audios/");
            await FGame.Register<UIComponent>().InitializeAsync(FGame.Resolve<XAssetComponent>(), "Assets/Res/Prefab/UI/");

            await UpdateAssetsAsync();

            // incase
            if (FGame.Scene.IsDisposed) return;

            //await UIComponent.Current.OpenUIAsync<UIStartPanel>();

            SetupLuaEnv();
        }

        private void SetupDownloader()
        {
            FGame.Register<DownloaderComponent>();

            Downloader.s_GlobalCompleted += (download) =>
            {
                if (download.Status == EDownloadStatus.Success)
                {
                    Log.INFO($"download success. url: {download.Info.DownloadUrl}");
                }
                else if (download.Status == EDownloadStatus.Failed)
                {
                    Log.ERROR($"download failed. url: {download.Info.DownloadUrl} error: {download.Error}");
                }
            };

            Downloader.s_OnDownloadAgentFactory = () =>
            {
                return new HttpDownload();
            };
        }

        private void LoadIFixPatch()
        {
#if UNITY_EDITOR
            IFix.IFixManager.Patch("./ExtraAssets/Patch/patch.json");
#else
            IFix.IFixManager.Patch($"{XAssetPath.k_DlcPath}/Patch/patch.json");
#endif

            // Test
            Main.Instance.gameObject.AddComponent<Tests.TestIFix>();
        }

        private void SetupLuaEnv()
        {
            FGame.Register<LuaComponent>();

            var luaEnv = LuaComponent.Current.LuaEnv;

#if UNITY_EDITOR && true
            if (XAssetComponent.s_Mode == XAssetComponent.EMode.Editor)
            {
                var dirs = Directory.GetDirectories(Application.dataPath, "LuaScripts", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    luaEnv.AddLoader(new FileLuaLoader(dir, ".lua.txt"));
                    luaEnv.AddLoader(new FileLuaLoader(dir, ".lua"));
                }
            }
            else
            {
                luaEnv.AddLoader(new FileLuaLoader(XAssetPath.k_Editor_DlcOutputPath + "/" + XAssetPath.k_CustomFolder + "/luascripts", ".lua"));
                luaEnv.AddLoader(new VFSLuaLoader(XAssetPath.k_Editor_DlcOutputPath + "/" + XAssetPath.k_CustomFolder + "/luascripts"));
            }
#else
            // 先加载dlc目录，在加载streamming目录 
            luaEnv.AddLoader(new VFSLuaLoader(XAssetPath.k_DlcPath + "/" + XAssetPath.k_CustomFolder + "/luascripts"));
            luaEnv.AddLoader(new VFSLuaLoader(XAssetPath.k_BasePath + "/" + XAssetPath.k_CustomFolder + "/luascripts"));
#endif

            try
            {
                luaEnv.DoString("require (\"Main\")");
                //await UIComponent.Current.OpenUIAsync("UIStartPanel");
                //await UIComponent.Current.OpenLuaUIAsync("UISetting");
            }
            catch (Exception e)
            {
                Log.ERROR(e);
            }
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateAssetsAsync()
        {
            var assetUpdaterComponent = FGame.Register<AssetUpdaterComponent>();
            assetUpdaterComponent.RequestDownloadOperationFunc = RequestDownloadOperation;

            //var uiloading = await UIComponent.Current.OpenUIAsync("UILoading");

            assetUpdaterComponent.StartUpdate().Forget();

            while (!assetUpdaterComponent.IsComplete)
            {
                await UniTask.Yield();
            }

            await PrepareAssets();

            //uiloading.Close();
        }

        private UniTask<bool> RequestDownloadOperation(List<DownloadInfo> infos)
        {
            var cts = new UniTaskCompletionSource<bool>();

            var totalDownloadSize = 0L;

            foreach (var info in infos)
            {
                totalDownloadSize += info.Size;
            }

            if (totalDownloadSize <= 1024 * 1024 * 10) //小于10m也静默下载
            {
                cts.TrySetResult(true);
            }
            else // 大于5m就 提示
            {
                var info = new MessageInfo
                {
                    title = "下载",
                    content = $"下载文件大小：{Downloader.FormatBytes(totalDownloadSize)}，是否要下载？",
                    leftText = "退出",
                    rightText = "下载",
                    clickHandler = (state) =>
                    {
                        if (state == 0)
                            cts.TrySetResult(false);
                        else if (state == 1)
                            cts.TrySetResult(true);
                        else
                            cts.TrySetException(new Exception("MessageBox can't handle click: " + state));
                    }
                };

                UIComponent.Current.AddMessageBox(info);
            }

            return cts.Task;
        }

        /// <summary>
        /// 准备必要的资源，确保本地存在
        /// </summary>
        /// <returns></returns>
        private async UniTask PrepareAssets()
        {
            await XAssetComponent.Current.CheckCustomAssets(XAssetPath.k_CustomFolder + "/luascripts");
        }
    }
}
