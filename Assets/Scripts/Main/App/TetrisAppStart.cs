using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Saro;
using Saro.Audio;
using Saro.Core;
using Saro.Events;
using Saro.Gameplay.Effect;
using Saro.Localization;
using Saro.MoonAsset;
using Saro.MoonAsset.Update;
using Saro.Net;
using Saro.Profiler;
using Saro.Table;
using Saro.UI;
using UnityEngine;

namespace Tetris
{
    public sealed class TetrisAppStart : IStartup
    {
        public async UniTask StartAsync()
        {
            QualitySettings.vSyncCount = 0;

            Main.Register<EventManager>();
            SetupDownloader();
            SetupAssetManager();
            SetupDataTable();

            Main.Register<UIManager>();
            // TODO 探究易用、易扩展的资源加载方式
            await Main.Register<AudioManager>().InitializeAsync("Assets/Res/Audios/");
            Main.Register<EffectManager>().SetAssetInterface(Main.Resolve<IAssetManager>(), "Assets/Res/Prefab/Vfx/");

            await UpdateAssetsAsync();

            await LoadHybirdCLR();
        }

        private async UniTask LoadHybirdCLR()
        {
            Assembly hotfix = null;

            Log.ERROR("HybridCLR", $"hotfix enable: {HybridCLR.HybridCLRUtil.IsHotFix}");

            if (!HybridCLR.HybridCLRUtil.IsHotFix || MoonAsset.s_Mode == MoonAsset.EMode.AssetDatabase)
            {
                //hotfix = Assembly.GetAssembly(Type.GetType("HotFix.HotFixApp"));
                hotfix = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == Path.GetFileNameWithoutExtension(HybridCLR.HybridCLRUtil.s_HotFixDLL));
            }
            else
            {
                var hotfixBytes = await Main.Resolve<IAssetManager>().LoadRawAssetAsync("hotfix/" + HybridCLR.HybridCLRUtil.s_HotFixDLL);
                hotfix = Assembly.Load(hotfixBytes);
            }

            if (hotfix == null)
            {
                Log.ERROR("HybridCLR", "dll未加载");
                return;
            }

            try
            {
                var appType = hotfix.GetType("HotFix.HotFixApp");
                var mainMethod = appType.GetMethod("Start");
                mainMethod.Invoke(null, null);
            }
            catch (Exception e)
            {
                Log.ERROR("HybridCLR", "HotFixApp::Start Exception: " + e);
            }
        }

        private void SetupAssetManager()
        {
            var moonAsset = new MoonAsset();
            moonAsset.Policy.AutoUnloadAsset = true;

            Main.Register<IAssetManager>(moonAsset);
        }

        private void SetupDownloader()
        {
            Main.Register<DownloaderManager>();

            Downloader.s_SpeedLimit = 128;

            Downloader.s_GlobalCompleted += download =>
            {
                if (download.Status == EDownloadStatus.Success)
                    Log.INFO($"download success. url: {download.Info.DownloadUrl}");
                else if (download.Status == EDownloadStatus.Failed)
                    Log.ERROR($"download failed. url: {download.Info.DownloadUrl} error: {download.Error}");
            };

            Downloader.s_OnDownloadAgentFactory = () => new HttpDownload();
        }

        private void SetupDataTable()
        {
            TableLoader.bytesLoader = tableName =>
            {
#if UNITY_EDITOR
                var mode = MoonAsset.s_Mode;
                if (mode == MoonAsset.EMode.AssetDatabase)
                {
                    using (var fs = new FileStream($"GameTools/tables/data/config/{tableName}", FileMode.Open,
                               FileAccess.Read))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
#endif

                return Main.Resolve<IAssetManager>().LoadRawAsset("tables/" + tableName);
            };

            TableLoader.bytesLoaderAsync = async tableName =>
            {
#if UNITY_EDITOR
                var mode = MoonAsset.s_Mode;
                if (mode == MoonAsset.EMode.AssetDatabase)
                {
                    using (var fs = new FileStream($"GameTools/tables/data/config/{tableName}", FileMode.Open,
                               FileAccess.Read))
                    {
                        //var buffer = new byte[fs.Length];
                        //fs.Read(buffer, 0, buffer.Length);
                        //return buffer;

                        var buffer = new Memory<byte>(new byte[fs.Length]);
                        var count = await fs.ReadAsync(buffer);
                        return buffer.ToArray();
                    }
                }
#endif

                return await Main.Resolve<IAssetManager>().LoadRawAssetAsync("tables/" + tableName);
            };
        }

        /// <summary>
        ///     更新资源
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateAssetsAsync()
        {
            // TODO 这种ui，应该放到母包里
            await UIManager.Current.LoadWindowAsync(EDefaultUI.UIWaiting);

            var uiCounter = 0;
            var moonAsset = Main.Resolve<IAssetManager>() as MoonAsset;
            moonAsset.OnLoadRemoteAsset = (assetName, state) =>
            {
                if (!state)
                {
                    uiCounter++;

                    //Log.ERROR($"open uiwait start: {Time.frameCount}");
                    // 临时解决方，外部先异步加载好bundle，委托内部就同步加载好了
                    var uiwait = UIManager.Current.ShowWindow<UIWaiting>(EDefaultUI.UIWaiting, layer: EUILayer.Top);
                    //Log.ERROR($"open uiwait end: {Time.frameCount}");
                    uiwait.SetEntry(assetName);
                    uiwait.Refresh();
                }
                else
                {
                    uiCounter--;

                    if (uiCounter == 0) UIManager.Current.HideWindow(EDefaultUI.UIWaiting);
                }
            };

            var assetUpdaterComponent = Main.Register<AssetUpdaterManager>();
            assetUpdaterComponent.RequestDownloadOperationFunc = RequestDownloadOperation;

            //var uiloading = await UIManager.Current.OpenUIAsync("UILoading");

            assetUpdaterComponent.StartUpdate().Forget();

            while (!assetUpdaterComponent.IsComplete) await UniTask.Yield();

            //uiloading.Close();
        }

        private UniTask<bool> RequestDownloadOperation(List<DownloadInfo> infos)
        {
            var cts = new UniTaskCompletionSource<bool>();

            var totalDownloadSize = 0L;

            foreach (var info in infos) totalDownloadSize += info.Size;

            if (totalDownloadSize <= 0) // 任意大小都提示
            {
                cts.TrySetResult(true);
            }
            else // 大于5m就 提示
            {
                var info = new AlertDialogInfo
                {
                    title = "下载",
                    content = $"下载文件大小：{Downloader.FormatBytes(totalDownloadSize)}，是否要下载？",
                    leftText = "退出",
                    rightText = "下载",
                    clickHandler = state =>
                    {
                        if (state == 0)
                            cts.TrySetResult(false);
                        else if (state == 1)
                            cts.TrySetResult(true);
                        else
                            cts.TrySetException(new Exception("MessageBox can't handle click: " + state));
                    }
                };

                UIManager.Current.QueueAsync(EDefaultUI.UIAlertDialog, 99, info);
            }

            return cts.Task;
        }
    }
}