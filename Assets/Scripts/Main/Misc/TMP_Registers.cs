//using System.Reflection;
//using Cysharp.Threading.Tasks;
//using Saro.Core;
//using TMPro;

//namespace Saro
//{
//    public class TMP_Registers
//    {
//        public static async UniTask LoadSettings()
//        {
//#if UNITY_EDITOR
//            await LoadRuntime();
//#endif

//            //TMP_Text.OnFontAssetRequest += TMP_Text_OnFontAssetRequest;
//            //TMP_Text.OnSpriteAssetRequest += TMP_Text_OnSpriteAssetRequest;
//        }

//#if UNITY_EDITOR
//        [UnityEditor.InitializeOnLoadMethod]
//        private static void LoadEditor()
//        {
//            TMP_Settings settings = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/Res/TextMesh Pro/Res/TMP Settings.asset");
//            var settingsType = settings.GetType();
//            var settingsInstanceInfo = settingsType.GetField("s_Instance", BindingFlags.Static | BindingFlags.NonPublic);
//            settingsInstanceInfo.SetValue(null, settings);
//        }
//#endif

//        private static async UniTask LoadRuntime()
//        {
//            TMP_Settings settings = await IAssetManager.Current.LoadAssetAsync("Assets/Res/TextMesh Pro/Res/TMP Settings.asset", typeof(TMP_Settings)) as TMP_Settings;
//            if (settings != null)
//            {
//                var settingsType = settings.GetType();
//                var settingsInstanceInfo = settingsType.GetField("s_Instance", BindingFlags.Static | BindingFlags.NonPublic);
//                settingsInstanceInfo.SetValue(null, settings);
//            }
//            else
//            {
//                UnityEngine.Debug.LogError("load TMP Settings.asset failed");
//            }
//        }
//    }
//}
