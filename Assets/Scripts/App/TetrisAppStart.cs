using System;
using System.Collections.Generic;
using System.IO;
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
using Tetris.Save;
using Tetris.UI;
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

            SetupLocalization().Forget();

            Main.Register<UIManager>();

            // TODO 探究易用、易扩展的资源加载方式
            await Main.Register<AudioManager>().InitializeAsync("Assets/Res/Audios/");
            Main.Register<EffectManager>().SetAssetInterface(Main.Resolve<IAssetManager>(), "Assets/Res/Prefab/Vfx/");

            await UpdateAssetsAsync();

            // TODO 换成 huatuo
            //SetupLuaEnv();

            LoadGameSave();

#if DEBUG
            Main.Instance.gameObject.AddComponent<ProfilerDisplay>();
#endif

            await UIManager.Current.LoadAndShowWindowAsync(ETetrisUI.StartWindow);

            {
                //Saro.UI.UIManager.Current.QueueAsync(Tetris.UI.ETetrisUI.AboutPanel, 1);
                //Saro.UI.UIManager.Current.QueueAsync(Tetris.UI.ETetrisUI.SettingPanel, -1);

                //Saro.UI.UIManager.Current.QueueAsync(Saro.UI.EDefaultUI.UIAlertDialog, 0, new Saro.UI.AlertDialogInfo
                //{
                //    title = "test title",
                //    content = "content",
                //    rightText = "yes",
                //    leftText = "no",
                //    clickHandler = (click) =>
                //    {
                //        Log.ERROR("click: " + click);
                //    }
                //});
            }
        }

        private void LoadGameSave()
        {
            Main.Register<SaveManager>();

            SaveManager.Current.Load();

            AudioManager.Current.ApplySettings();
            LocalizationManager.Current.ApplySettings();
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
                if (mode == MoonAsset.EMode.Editor)
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
                if (mode == MoonAsset.EMode.Editor)
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

        private async UniTask SetupLocalization()
        {
            await Main.Register<LocalizationManager>()
                .SetProvider(new LocalizationDataProviderExcel())
                .SetLanguageAsync(ELanguage.ZH);
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

            if (totalDownloadSize <= 0 ) // 任意大小都提示
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