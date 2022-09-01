using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Saro.MoonAsset.Build;

namespace HybridCLR.Editor
{
    internal class BuildMethods : IBuildProcessor
    {
        [MoonAssetBuildMethod(-1, "<color=red>[HotFix]</color> Complie DLL")]
        public static void CompileDLL()
        {
            CollectDLL(BuildConfig.MGFHotFixDLLPath, EditorUserBuildSettings.activeBuildTarget);
        }

        [MoonAssetBuildMethod(48, "<color=red>[HotFix]</color> MethodBridge_All_Normal", tooltip = "生成hybridclr的桥接文件")]
        public static void MethodBridge_All_Normal()
        {
            MethodBridgeHelper.MethodBridge_All_Normal();
        }

        [MoonAssetBuildMethod(49, "<color=red>[HotFix]</color> Auto GenLinkXML", tooltip = "扫描hotfix程序集，以及配置，生成link.xml，需要自定义 AotCongif.IsAutoGenLinkXML 委托")]
        public static void AutoGenLinkXML()
        {
            AotConfig.AutoGenLinkXML();
        }

        private static void CollectDLL(string tempDir, BuildTarget target)
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);

            CompileDllHelper.CompileDll(target);

            string hotfixDllSrcDir = BuildConfig.GetHotFixDllsOutputDirByTarget(target);
            foreach (var dll in BuildConfig.HotUpdateAssemblies)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                if (!File.Exists(dllPath))
                {
                    Debug.LogError($"[CollectDLL] path: {dllPath} 不存在！");
                    continue;
                }
                string dllBytesPath = $"{tempDir}/{dll}";
                File.Copy(dllPath, dllBytesPath, true);
            }

            string aotDllDir = BuildConfig.GetAssembliesPostIl2CppStripDir(target);
            foreach (var dll in BuildConfig.AOTMetaAssemblies)
            {
                string dllPath = $"{aotDllDir}/{dll}";
                if (!File.Exists(dllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{tempDir}/{dll}";
                File.Copy(dllPath, dllBytesPath, true);
            }
        }
    }
}
