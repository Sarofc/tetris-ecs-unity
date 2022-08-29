
namespace HybridCLR.Editor
{
    public static partial class BuildConfig
    {
        /// <summary>
        /// TODO. 现在打包是写死的 mgf_hotfix 路径，后面考虑，框架根据次属性，自动生成打包路径
        /// </summary>
        public static string MGFHotFixDLLPath { get; set; } = $"{HybridCLRDataDir}/mgf_hotfix";
    }
}
