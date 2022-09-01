using Cysharp.Threading.Tasks;
using Saro;
using Saro.Audio;
using Saro.Core;
using Saro.Localization;
using Saro.Profiler;
using Saro.UI;
using Saro.Utility;
using Tetris;
using Tetris.Save;
using Tetris.UI;
using UnityEngine;
using System.Linq;

namespace HotFix
{
    public static class HotFixApp
    {
        public static void Start()
        {
            // TODO 这部分没必要 在hotfix里调用，流程上应该不会更改？
            // 主要是性能差距，看看耗时高不高再说
            if (HybridCLR.HybridCLRUtil.IsHotFix)
            {
#if !UNITY_EDITOR
                HybridCLR.HybridCLRUtil.LoadMetadataForAOTAssembly();
#endif

                // 反射需要重新加载一下
                ReCacheAssemblies();
            }

            // 启动游戏
            HotFixAppInternal.Start().Forget();
        }

        private static void ReCacheAssemblies()
        {
            ReflectionUtility.ClearCacheAssemblies();
            ReflectionUtility.CacheAssemblies();

#if ENABLE_LOG
            Debug.Log("ReCacheAssemblies:\n" + string.Join(", ", ReflectionUtility.AssemblyMap.Values.Select(asm => asm.GetName().Name)));
#endif
        }
    }

    internal static class HotFixAppInternal
    {
        public static async UniTask Start()
        {
            await SetupLocalization();

            LoadGameSave();

#if DEBUG
            Main.Instance.gameObject.AddComponent<ProfilerDisplay>();
#endif

            Main.Register<SceneController>();
            UIManager.Current.CacheUIAttributes(); // ui反射需要重新缓存一下
            var uiLoading = UIManager.Current.LoadAndShowWindowAsync(EGameUI.StartWindow);
            var sceneLoading = SceneController.Current.ChangeScene(SceneController.ESceneType.Title);
            await uiLoading; // ui可以先加载
            await sceneLoading;

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

        private static async UniTask SetupLocalization()
        {
            await Main.Register<LocalizationManager>()
                .SetProvider(new LocalizationDataProviderExcel())
                .SetLanguageAsync(ELanguage.ZH);
        }

        private static void LoadGameSave()
        {
            Main.Register<SaveManager>();

            SaveManager.Current.Load();

            AudioManager.Current.ApplySettings();
            LocalizationManager.Current.ApplySettings();
        }
    }
}