using Saro.Utility;
using System.IO;

namespace Saro.Lua
{
    internal sealed class FileLuaLoader : BaseLuaLoader
    {
        public FileLuaLoader(string directory, string suffix) : base(directory, suffix)
        { }

        protected override byte[] Load(ref string fileName)
        {
            var path = $"{m_Directory}/{fileName.Replace(".", "/")}{m_Suffix}";

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
