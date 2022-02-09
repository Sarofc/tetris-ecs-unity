using Cysharp.Threading.Tasks;
using Saro.IO;
using Saro.Utility;
using Saro.XAsset;
using System;
using System.IO;
using UnityEngine;

namespace Saro.Lua
{
    public sealed class VFSLuaLoader : LuaLoaderBase
    {
        public VFSLuaLoader(string path) : base(path, string.Empty)
        {
        }

        protected override byte[] Load(ref string fileName)
        {
            var path = m_Path;

            // incase
            if (!FileUtility.Exists(path)) return null;

            Log.INFO($"VFSLuaLoader load {fileName} from {path}");

            using (var vfs = VFileSystem.Open(path, FileMode.Open, FileAccess.Read))
            {
                var data = vfs.ReadFile(fileName);

                //byte[] key = new byte[10] { 110, 2, 3, 4, 255, 6, 44, 8, 94, 10 };
                //Saro.Utility.EncryptionUtility.QuickSelfXorBytes(data, key);

                if (HasBOMFlag(data))
                {
                    Log.ERROR("has bom");
                }
                return data;
            }
        }
    }
}