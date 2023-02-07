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
using Saro.GTable;
using Saro.UI;
using UnityEngine;

namespace Tetris
{
    public sealed class TetrisAppStart : IStartup
    {
        public async UniTask StartAsync()
        {
            Main.Register<EventManager>();
            SetupDownloader();
            await MoonAsset.RegisterAsync();
            SetupDataTable();

            //await TMP_Registers.LoadSettings(); // 加载tmp的配置
            Main.Register<UIManager>();
            // TODO 探究易用、易扩展的资源加载方式
            await Main.Register<AudioManager>().InitializeAsync("Assets/Res/Audios/");
            Main.Register<EffectManager>().SetLoadPath("Assets/Res/Prefabs/Vfx/");
            //Main.Register<EffectManager>().SetAssetInterface(Main.Resolve<IAssetManager>(), "Assets/Res/Prefabs/Vfx/");

            await UpdateAssetsAsync();

            await LoadHybridCLR();
        }

        private async UniTask LoadHybridCLR()
        {
            Assembly hotfix = null;

            // hybridclr只有打包后才能生效
#if UNITY_EDITOR
            if (false)
#else
            if (HybridCLR.HybridCLRUtil.IsHotFix)
#endif
            {
                using var vfile = await IAssetManager.Current.OpenVFileAsync("Assets/ResRaw/hotfix");
                var hotfixBytes = vfile.ReadFile(HybridCLR.HybridCLRUtil.s_HotFixDLL);
                hotfix = Assembly.Load(hotfixBytes);
            }
            else
            {
                hotfix = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == Path.GetFileNameWithoutExtension(HybridCLR.HybridCLRUtil.s_HotFixDLL));
            }

            if (hotfix == null)
            {
                Log.ERROR("HybridCLR", "dll未加载");
                return;
            }

            var appType = hotfix.GetType("HotFix.HotFixApp");
            var mainMethod = appType.GetMethod("Start");
            mainMethod.Invoke(null, null);
        }

        private void SetupDownloader()
        {
            Downloader.Initialize();

            Downloader.s_SpeedLimit = 256; // 限速

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
            TableLoader.s_BytesLoader = tableName =>
            {
#if UNITY_EDITOR
                var mode = MoonAsset.s_Mode;
                if (mode == MoonAsset.EMode.AssetDatabase)
                {
                    using (var fs = new FileStream($"ExtraAssets/tables/data/{tableName}", FileMode.Open,
                               FileAccess.Read))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
#endif
                return IAssetManager.Current.ReadVFile("Assets/ResRaw/tables", tableName);
            };

            TableLoader.s_BytesLoaderAsync = async tableName =>
            {
#if UNITY_EDITOR
                var mode = MoonAsset.s_Mode;
                if (mode == MoonAsset.EMode.AssetDatabase)
                {
                    using (var fs = new FileStream($"ExtraAssets/tables/data/{tableName}", FileMode.Open,
                               FileAccess.Read))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
#endif

                return await IAssetManager.Current.ReadVFileAsync("Assets/ResRaw/tables", tableName);
            };
        }

        /// <summary>
        ///     更新资源
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateAssetsAsync()
        {
            // 提前加载 此ui资源
            await UIManager.Current.LoadWindowAsync(EDefaultUI.UIWaiting);

            var uiCounter = 0;
            var assetManager = IAssetManager.Current;
            assetManager.OnLoadRemoteAsset = (assetName, state) =>
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
            //assetUpdaterComponent.RequestDownloadOperationFunc = RequestDownloadOperation;

            //var uiloading = await UIManager.Current.OpenUIAsync("UILoading");

            assetUpdaterComponent.StartUpdate().Forget();

            while (!assetUpdaterComponent.IsComplete) await UniTask.Yield();

            //uiloading.Close();
        }

    }
}