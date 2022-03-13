using System;
using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Saro;
using Saro.Audio;
using Saro.EventDef;
using Saro.Events;
using Saro.Net;
using Saro.UI;
using Saro.Lua;
using Saro.Lua.UI;
using Saro.Utility;
using Saro.XAsset;
using Saro.XAsset.Update;
using UnityEngine;
using Tetris.UI;
using Tetris.Save;
using Saro.Localization;
using Saro.Table;

namespace Tetris
{
    public sealed class TetrisAppStart : FEvent<AppStart>
    {
        protected override async UniTask Run(AppStart args)
        {
            FGame.Register<EventComponent>();

            SetupDownloader();

            SetupAssetManager();

            //LoadIFixPatch();

            SetupDataTable();

            SetupLocalization();

            await FGame.Register<SoundComponent>().InitializeAsync(FGame.Resolve<XAssetComponent>(), "Assets/Res/Audios/");
            await FGame.Register<UIComponent>().InitializeAsync(FGame.Resolve<XAssetComponent>(), "Assets/Res/Prefab/UI/");

            await UpdateAssetsAsync();

            // incase
            if (FGame.Scene.IsDisposed) return;

            LoadGameSave();

            SetupLuaEnv();

            Main.Instance.gameObject.AddComponent<Saro.Profiler.ProfilerDisplay>();

            //await UIComponent.Current.OpenUIAsync<UIStartPanel>();
        }

        private void LoadGameSave()
        {
            FGame.Register<SaveComponent>();

            SaveComponent.Current.Load();

            SoundComponent.Current.ApplySettings();
        }

        private void SetupAssetManager()
        {
            var xassetComponent = FGame.Register<XAssetComponent>();

            xassetComponent.SetDefaultLocator();

            xassetComponent.Initialize();
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

        private void SetupDataTable()
        {
            TableCfg.s_BytesLoader = tableName =>
            {
#if UNITY_EDITOR
                var mode = XAssetComponent.s_Mode;
                if (mode == XAssetComponent.EMode.Editor)
                {
                    using (var fs = new FileStream($"GameTools/tables/data/config/{tableName}", FileMode.Open, FileAccess.Read))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
#endif

                return XAssetComponent.Current.LoadCustomAsset("tables/" + tableName);
            };
        }

        private void SetupLocalization()
        {
            FGame.Register<LocalizationComponent>()
                .SetProvider(new LocalizationDataProvider_Excel())
                .SetLanguage(ELanguage.ZH);
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
                luaEnv.AddLoader(new CustomBundleLuaLoader("luascripts", ".lua"));
            }
#else
            // 先加载dlc目录，在加载streamming目录 
            luaEnv.AddLoader(new CustomBundleLuaLoader("luascripts", ".lua"));
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

            bool result = await XAssetComponent.Current.PreloadRemoteAssets();
            if (!result)
            {
                Log.ERROR("PreloadRemoteAssets failed");
            }

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
    }
}
