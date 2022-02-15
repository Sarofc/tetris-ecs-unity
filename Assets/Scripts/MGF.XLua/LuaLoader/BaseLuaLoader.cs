
using XLua;

namespace Saro.Lua
{
    public abstract class BaseLuaLoader
    {
        protected string m_Directory;
        protected string m_Suffix;

        public BaseLuaLoader(string directory, string suffix)
        {
            m_Directory = directory;
            m_Suffix = suffix;
        }

        protected abstract byte[] Load(ref string fileName);

        public static implicit operator LuaEnv.CustomLoader(BaseLuaLoader luaLoader)
        {
            return luaLoader.Load;
        }

        public static bool HasBOMFlag(byte[] data)
        {
            if (data == null || data.Length < 3)
                return false;

            if (data[0] == 239 && data[1] == 187 && data[2] == 191)
                return true;

            return false;
        }
    }
}
