using XLua;

namespace Saro
{
    public sealed class LuaComponent : FEntity
    {
        public static LuaComponent Current => FGame.Resolve<LuaComponent>();
        public LuaTable Global => LuaEnv.Global;

        public LuaEnv LuaEnv { get; private set; }

        [System.Obsolete("Use 'LuaComponent.Current.LuaEnv' instead")]
        public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return LuaEnv.DoString(chunk, chunkName, env);
        }

        internal void Awake()
        {
            LuaEnv = new LuaEnv();
        }

        internal void Update()
        {
            LuaEnv.Tick();
        }

        internal void Destroy()
        {
            LuaEnv.Dispose();
        }
    }

    [FObjectSystem]
    internal sealed class LuaComponentAwakeSystem : AwakeSystem<LuaComponent>
    {
        public override void Awake(LuaComponent self)
        {
            self.Awake();
        }
    }

    [FObjectSystem]
    internal sealed class LuaComponentUpdateSystem : UpdateSystem<LuaComponent>
    {
        public override void Update(LuaComponent self)
        {
            self.Update();
        }
    }

    [FObjectSystem]
    internal sealed class LuaComponentDestroySystem : DestroySystem<LuaComponent>
    {
        public override void Destroy(LuaComponent self)
        {
            self.Destroy();
        }
    }
}
