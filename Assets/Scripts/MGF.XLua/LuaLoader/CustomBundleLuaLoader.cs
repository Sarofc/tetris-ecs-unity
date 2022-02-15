using Cysharp.Threading.Tasks;
using Saro.IO;
using Saro.Utility;
using Saro.XAsset;
using System;
using System.IO;
using UnityEngine;

namespace Saro.Lua
{
    /// <summary>
    /// XAsset的CustomBundle
    /// </summary>
    public sealed class CustomBundleLuaLoader : BaseLuaLoader
    {
        public CustomBundleLuaLoader(string directory, string suffix) : base(directory, suffix)
        {
        }

        protected override byte[] Load(ref string fileName)
        {
            var path = m_Directory + "/" + fileName + m_Suffix;

            Log.INFO($"CustomBundleLuaLoader load {fileName} at {path}");

            var data = XAssetComponent.Current.LoadCustomAsset(path);

            if (HasBOMFlag(data))
            {
                Log.ERROR("has bom");
            }

            return data;
        }
    }
}