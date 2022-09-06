using System.IO;
using Saro.MoonAsset.Build;
using UnityEditor;
using UnityEngine;
using Saro.IO;

namespace HybridCLR.Editor
{
    internal sealed class VFilePacker_HotfixDLL : IVFilePacker
    {
        static string vfileName = "hotfix";

        bool IVFilePacker.PackVFile(string dstFolder)
        {
            var dstVFilePath = dstFolder + "/" + vfileName;

            BuildVFile(dstVFilePath, EditorUserBuildSettings.activeBuildTarget);

            return true;
        }

        private static void BuildVFile(string vfilePath, BuildTarget target)
        {
            if (File.Exists(vfilePath))
                File.Delete(vfilePath);

            if (!HybridCLRUtil.IsHotFix)
            {
                Debug.Log("[VFilePacker_HotfixDLL] ENABLE_HOTFIX 未开启");
                return;
            }

            var dllNum = BuildConfig.HotUpdateAssemblies.Count + BuildConfig.AOTMetaAssemblies.Count;
            if (dllNum <= 0)
            {
                Debug.Log("[VFilePacker_HotfixDLL] 没有热更相关dll需要打包");
                return;
            }

            CompileDllHelper.CompileDll(target);

            using (var vfile = VFileSystem.Open(vfilePath, FileMode.CreateNew, FileAccess.ReadWrite, dllNum, dllNum))
            {
                string hotfixDllSrcDir = BuildConfig.GetHotFixDllsOutputDirByTarget(target);
                foreach (var dll in BuildConfig.HotUpdateAssemblies)
                {
                    string dllPath = $"{hotfixDllSrcDir}/{dll}";
                    var result = vfile.WriteFile($"{dll}", dllPath);
                    if (!result)
                    {
                        Debug.LogError($"[VFilePacker_HotfixDLL] path: {dllPath} 不存在！");
                        continue;
                    }
                }

                string aotDllDir = BuildConfig.GetAssembliesPostIl2CppStripDir(target);
                foreach (var dll in BuildConfig.AOTMetaAssemblies)
                {
                    string dllPath = $"{aotDllDir}/{dll}";
                    var result = vfile.WriteFile($"{dll}", dllPath);
                    if (!result)
                    {
                        Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                        continue;
                    }
                }

                Debug.LogError("[VFilePacker_HotfixDLL]\n" + string.Join("\n", vfile.GetAllFileInfos()));
            }
        }
    }
}
