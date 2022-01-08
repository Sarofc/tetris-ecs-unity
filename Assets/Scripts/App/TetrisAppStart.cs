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

namespace Tetris
{
    public class TetrisAppStart : AEvent<AppStart>
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

            {
                //var assetUpdaterComponent = FGame.Register<AssetUpdaterComponent>();
                //assetUpdaterComponent.RequestDownloadOperationFunc = RequestDownloadOperation;

                //var uiloading = await UIComponent.Instance.OpenUIAsync<UILoading>();

                //await assetUpdaterComponent.StartUpdate();

                //uiloading.Close();
            }

            // incase
            if (FGame.Scene.IsDisposed) return;

            await UIComponent.Current.OpenUIAsync<UIStartPanel>();
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

        private UniTask<bool> RequestDownloadOperation(List<DownloadInfo> infos)
        {
            var cts = new UniTaskCompletionSource<bool>();

            var totalDownloadSize = 0L;

            foreach (var info in infos)
            {
                totalDownloadSize += info.Size;
            }

            if (totalDownloadSize <= 0) // 没有下载就直接跳过下载步骤，直接进游戏
            {
                cts.TrySetResult(false);
            }
            else if (totalDownloadSize <= 1024 * 1024 * 5) //小于5m也静默下载
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
