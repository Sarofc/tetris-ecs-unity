using System.Collections.Generic;
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
    public class HotFixApp
    {
        public static void Start()
        {
            if (HybridCLR.HybridCLRUtil.IsHotFix)
            {
#if !UNITY_EDITOR
                HybridCLR.HybridCLRUtil.LoadMetadataForAOTAssembly();
#endif

                // 反射需要重新加载一下
                ReCacheAssemblies();
            }

            HotFixAppInternal.Start().Forget();
        }

        private static void ReCacheAssemblies()
        {
            ReflectionUtility.ClearCacheAssemblies();
            ReflectionUtility.CacheAssemblies();

#if ENABLE_LOG
            Debug.LogError("ReCacheAssemblies:\n" + string.Join(", ", ReflectionUtility.AssemblyMap.Values.Select(asm => asm.GetName().Name)));
#endif
        }
    }

    internal class HotFixAppInternal
    {
        public static async UniTask Start()
        {
            // 测试补充元数据后使用 AOT泛型
            TestAOTGeneric();

            Debug.LogError("hello, HybridCLR 1");
            //var go = new GameObject("HotFixApp");
            //go.AddComponent<CreateByCode>();

            await SetupLocalization();
            Debug.LogError("SetupLocalization");

            LoadGameSave();
            Debug.LogError("LoadGameSave");

#if DEBUG
            Main.Instance.gameObject.AddComponent<ProfilerDisplay>();
            Debug.Log("Add ProfilerDisplay");
#endif

            UIManager.Current.CacheUIAttributes();

            Debug.LogError("Start Load StartWindow");
            await UIManager.Current.LoadAndShowWindowAsync(ETetrisUI.StartWindow);
            Debug.LogError("Load StartWindow");

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

            Debug.Log("=======看到此条日志代表你成功运行了示例项目的热更新代码=======");
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

        /// <summary>
        /// 测试 aot泛型
        /// </summary>
        public static void TestAOTGeneric()
        {
            var arr = new List<MyValue>();
            arr.Add(new MyValue() { x = 1, y = 10, s = "abc" });
            Debug.Log("AOT泛型补充元数据机制测试正常");
        }
    }

    public struct MyValue
    {
        public int x, y;
        public string s;
    }
}