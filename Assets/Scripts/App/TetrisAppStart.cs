using Cysharp.Threading.Tasks;
using Saro;
using Saro.Audio;
using Saro.Core;
using Saro.Events;
using Saro.Gameplay.Effect;
using Saro.Localization;
using Saro.Lua;
using Saro.Net;
using Saro.Table;
using Saro.UI;
using Saro.XAsset;
using Saro.XAsset.Update;
using System;
using System.Collections.Generic;
using System.IO;
using Tetris.Save;
using UnityEngine;

namespace Tetris
{
    public sealed class TetrisAppStart : IStartup
    {
        public override async UniTask StartAsync()
        {
            Main.Register<EventManager>();

            SetupDownloader();

            SetupAssetManager();

            //LoadIFixPatch();

            SetupDataTable();

            SetupLocalization().Forget();

            // TODO 探究易用、易扩展的资源加载方式
            //await Main.Register<UIManager>().InitializeAsync("Assets/Res/Prefab/UI/");

            BDFramework.UFlux.UIManager.Instance.Init();

            await Main.Register<SoundManager>().InitializeAsync("Assets/Res/Audios/");
            Main.Register<EffectManager>().SetAssetInterface(Main.Resolve<IAssetInterface>(), "Assets/Res/Prefab/Vfx/");

            //await UpdateAssetsAsync();

            LoadGameSave();

            //SetupLuaEnv();

            Main.Instance.gameObject.AddComponent<Saro.Profiler.ProfilerDisplay>();

            //await UIManager.Current.OpenUIAsync<UIStartPanel>();

            BDFramework.UFlux.UIManager.Instance.LoadWindow(BDFramework.UI.WinEnum.Win_UFlux);
            BDFramework.UFlux.UIManager.Instance.ShowWindow(BDFramework.UI.WinEnum.Win_UFlux);
        }

        private void LoadGameSave()
        {
            Main.Register<SaveManager>();

            //SaveManager.Current.Load();

            //SoundManager.Current.ApplySettings();
            //LocalizationManager.Current.ApplySettings();
        }

        private void SetupAssetManager()
        {
            var xassetManager = new XAssetManager();
            xassetManager.SetDefaultLocator();
            xassetManager.Policy.AutoUnloadAsset = true;

            Main.Register<IAssetInterface>(xassetManager);
        }

        private void SetupDownloader()
        {
            Main.Register<DownloaderManager>();

            Downloader.s_SpeedLimit = 128;

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

        // TODO 需要改为vfs加载
        private void TestLoadIFixPatch()
        {
#if UNITY_EDITOR
            IFix.IFixManager.Patch("./ExtraAssets/Patch/patch.json");
#else
            IFix.IFixManager.Patch($"{XAssetConfig.k_DlcPath}/Patch/patch.json");
#endif

            // Test
            Main.Instance.gameObject.AddComponent<Tests.TestIFix>();
        }

        private void SetupDataTable()
        {
            TableLoader.s_BytesLoader = tableName =>
            {
#if UNITY_EDITOR
                var mode = XAssetManager.s_Mode;
                if (mode == XAssetManager.EMode.Editor)
                {
                    using (var fs = new FileStream($"GameTools/tables/data/config/{tableName}", FileMode.Open, FileAccess.Read))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
#endif

                return Main.Resolve<IAssetInterface>().LoadCustomAsset("tables/" + tableName);
            };

            TableLoader.s_BytesLoaderAsync = async tableName =>
            {
#if UNITY_EDITOR
                var mode = XAssetManager.s_Mode;
                if (mode == XAssetManager.EMode.Editor)
                {
                    using (var fs = new FileStream($"GameTools/tables/data/config/{tableName}", FileMode.Open, FileAccess.Read))
                    {
                        //var buffer = new byte[fs.Length];
                        //fs.Read(buffer, 0, buffer.Length);
                        //return buffer;

                        Memory<byte> buffer = new Memory<byte>(new byte[fs.Length]);
                        var count = await fs.ReadAsync(buffer);
                        return buffer.ToArray();
                    }
                }
#endif

                return await Main.Resolve<IAssetInterface>().LoadCustomAssetAsync("tables/" + tableName);
            };
        }

        private async UniTask SetupLocalization()
        {
            await Main.Register<LocalizationManager>()
                  .SetProvider(new LocalizationDataProvider_Excel())
                  .SetLanguageAsync(ELanguage.ZH);
        }

        private void SetupLuaEnv()
        {
            Main.Register<LuaManager>();

            var luaEnv = LuaManager.Current.LuaEnv;

#if UNITY_EDITOR && true
            if (XAssetManager.s_Mode == XAssetManager.EMode.Editor)
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
                luaEnv.AddLoader(new FileLuaLoader(XAssetConfig.k_Editor_DlcOutputPath + "/" + XAssetConfig.k_CustomFolder + "/luascripts", ".lua"));
                luaEnv.AddLoader(new CustomBundleLuaLoader("luascripts", ".lua"));
            }
#else
            // 先加载dlc目录，在加载streamming目录 
            luaEnv.AddLoader(new CustomBundleLuaLoader("luascripts", ".lua"));
#endif

            try
            {
                luaEnv.DoString("require (\"Main\")");
                //await UIManager.Current.OpenUIAsync("UIStartPanel");
                //await UIManager.Current.OpenLuaUIAsync("UISetting");
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
            // TODO 这种ui，应该放到母包里
            var uiwait = await UIManager.Current.ShowNetworkWaitingUIAsync();
            uiwait.Close();

            var uiCounter = 0;
            var xassetManager = Main.Resolve<IAssetInterface>() as XAssetManager;
            xassetManager.OnLoadRemoteAsset = async (assetName, state) =>
            {
                if (!state)
                {
                    uiCounter++;

                    //Log.ERROR($"open uiwait start: {Time.frameCount}");
                    // 临时解决方，外部先异步加载好bundle，委托内部就同步加载好了
                    var uiwait = UIManager.Current.ShowNetworkWaitingUI();
                    //Log.ERROR($"open uiwait end: {Time.frameCount}");
                    uiwait.SetEntry(assetName);
                }
                else
                {
                    uiCounter--;

                    if (uiCounter == 0)
                    {
                        UIManager.Current.CloseNetworkWaitingUI();
                    }
                }
            };

            var assetUpdaterComponent = Main.Register<AssetUpdaterManager>();
            assetUpdaterComponent.RequestDownloadOperationFunc = RequestDownloadOperation;

            //var uiloading = await UIManager.Current.OpenUIAsync("UILoading");

            assetUpdaterComponent.StartUpdate().Forget();

            while (!assetUpdaterComponent.IsComplete)
            {
                await UniTask.Yield();
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

                UIManager.Current.AddMessageBox(info);
            }

            return cts.Task;
        }
    }
}
