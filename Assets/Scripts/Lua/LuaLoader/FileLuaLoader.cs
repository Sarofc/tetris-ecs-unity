using Saro.Utility;
using System.IO;

namespace Saro.Lua
{
    internal sealed class FileLuaLoader : LuaLoaderBase
    {
        public FileLuaLoader(string path, string suffix) : base(path, suffix)
        { }

        protected override byte[] Load(ref string fileName)
        {
            var path = $"{m_Path}/{fileName.Replace(".", "/")}{m_Suffix}";

            if (!FileUtility.Exists(path)) return null;

            Log.INFO("FileLuaLoader load: " + path);

            var data = FileUtility.ReadAllBytes(path);
            if (HasBOMFlag(data))
            {
                Log.ERROR("has bom");
            }
            return data;
        }
    }
}
