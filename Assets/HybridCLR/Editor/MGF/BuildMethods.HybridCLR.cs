using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Saro.MoonAsset.Build;

namespace HybridCLR.Editor
{
    internal class BuildMethods : IBuildProcessor
    {
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
   }
}
