using XLua;

namespace Saro
{
    public sealed class LuaManager : IService
    {
        public static LuaManager Current => Main.Resolve<LuaManager>();

        [System.Obsolete("Use 'LuaComponent.Current.LuaEnv.Global' instead")]
        public LuaTable Global => LuaEnv.Global;

        public LuaEnv LuaEnv { get; private set; }

        [System.Obsolete("Use 'LuaComponent.Current.LuaEnv' instead")]
        public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return LuaEnv.DoString(chunk, chunkName, env);
        }

        void IService.Awake()
        {
            LuaEnv = new LuaEnv();
        }

        void IService.Update()
        {
            // TODO 不要每帧调用，间隔一会儿
            LuaEnv.Tick();
        }

        void IService.Dispose()
        {
            LuaEnv.Dispose();
        }
    }
}
