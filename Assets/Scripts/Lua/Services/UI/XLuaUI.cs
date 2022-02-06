using Saro.UI;
using System;
using XLua;

namespace Saro.Lua.UI
{
    [LuaCallCSharp]
    public class XLuaUI : UIBase<UIBinder>
    {
        private Action<XLuaUI> m_OnStart, m_OnClose, m_OnAwake;
        private Action<XLuaUI, float> m_OnUpdate;

        private LuaTable m_ScriptEnv;
        private LuaTable m_Metatable;

        protected override void InternalAwake()
        {
            InitLua();

            m_OnAwake?.Invoke(this);
        }

        protected override void InternalStart()
        {
            m_OnStart?.Invoke(this);
        }

        protected override void InternalUpdate(float deltaTime)
        {
            m_OnUpdate?.Invoke(this, deltaTime);
        }

        protected override void InternalClose()
        {
            m_OnClose?.Invoke(this);
        }

        protected override void DoDestroy()
        {
            base.DoDestroy();

            DisposeLua();
        }

        private void DisposeLua()
        {
            m_OnAwake = null;
            m_OnStart = null;
            m_OnClose = null;
            m_OnUpdate = null;

            if (m_Metatable != null)
            {
                m_Metatable.Dispose();
                m_Metatable = null;
            }

            if (m_ScriptEnv != null)
            {
                m_ScriptEnv.Dispose();
                m_ScriptEnv = null;
            }
        }

        private void InitLua()
        {
            var luaEnv = LuaComponent.Current.LuaEnv;
            m_ScriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            m_ScriptEnv.SetMetaTable(meta);
            meta.Dispose();

            m_ScriptEnv.Set("target", this);

            var script = Binder.GetComponent<LuaScript>().script;
            string scriptText = (script.Type == EScriptReferenceType.TextAsset) ? script.Text.text : string.Format("require(\"Core.System\");local cls=require(\"{0}\");return extends(target,cls);", script.Filename);

            object[] result = luaEnv.DoString(scriptText, string.Format("{0}({1})", "XLuaUI", UIName), m_ScriptEnv);

            if (result.Length != 1 || !(result[0] is LuaTable))
                throw new Exception("");

            m_Metatable = (LuaTable)result[0];

            foreach (var item in Binder.Datas)
            {
                m_Metatable.Set(item.key, item.obj);
            }

            m_OnAwake = m_Metatable.Get<Action<XLuaUI>>("OnAwake");
            m_OnStart = m_Metatable.Get<Action<XLuaUI>>("OnStart");
            m_OnClose = m_Metatable.Get<Action<XLuaUI>>("OnClose");
            m_OnUpdate = m_Metatable.Get<Action<XLuaUI, float>>("OnUpdate");
        }
    }
}
