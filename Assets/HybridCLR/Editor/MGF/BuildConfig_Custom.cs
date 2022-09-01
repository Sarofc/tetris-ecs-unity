﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HybridCLR.Editor
{
    public static partial class BuildConfig
    {
        /// <summary>
        /// TODO. 现在打包是写死的 mgf_hotfix 路径，后面考虑，框架根据次属性，自动生成打包路径
        /// </summary>
        public static string MGFHotFixDLLPath { get; set; } = $"{HybridCLRDataDir}/mgf_hotfix";

        /// <summary>
        /// 所有热更新dll列表。放到此列表中的dll在打包时OnFilterAssemblies回调中被过滤。
        /// </summary>
        public static List<string> HotUpdateAssemblies { get; } = new List<string>
        {
            "HotFix.dll",
            //"HotFix2.dll",
        };

        public static List<string> AOTMetaAssemblies { get => HybridCLRUtil.AOTMetaAssemblies; }

        [System.Obsolete("对此项目是无用的")]
        public static List<string> AssetBundleFiles { get; } = new List<string>
        {
            "common",
        };
    }
}