using Cysharp.Threading.Tasks;
using Saro.UI;
using System;
using UnityEngine.Events;
using XLua;

namespace Saro.Lua.UI
{
    [LuaCallCSharp]
    public static class UIComponentLuaExt
    {
        /// <summary>
        /// 不给uiType，就默认XLuaUI
        /// </summary>
        /// <param name="self"></param>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public static async UniTask<UIBase> OpenUIAsync(this UIComponent self, string uiName)
        {
            return await self.OpenUIAsync<XLuaUI>(uiName);
        }

        public static async UniTask<UIBase> OpenUIAsync(this UIComponent self, Type uiType, string uiName)
        {
            return await self.OpenUIAsync(uiType, uiName);
        }

        public static void CloseUI(this UIComponent self, string uiName)
        {
            self.CloseUI(uiName);
        }

        public static void Listen(this UIBase self, UnityEvent src, UnityAction dst)
        {
            self.Listen(src, dst);
        }

        public static void Listen(this UIBase self, UnityEvent<float> src, UnityAction<float> dst)
        {
            self.Listen(src, dst);
        }

        public static void Close(this UIBase self)
        {
            self.Close();
        }
    }
}
