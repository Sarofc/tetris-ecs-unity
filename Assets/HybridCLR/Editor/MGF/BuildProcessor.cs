using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using HybridCLR.Editor;

namespace Saro.MoonAsset.Build
{
    internal class BuildProcessor : IBuildProcessor
    {
        [MoonAssetBuildMethod(-1, "<color=red>[HotFix]</color> Complie DLL")]
        public static void CompileDLL()
        {
            CollectDLL(BuildConfig.MGFHotFixDLLPath, EditorUserBuildSettings.activeBuildTarget);
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
