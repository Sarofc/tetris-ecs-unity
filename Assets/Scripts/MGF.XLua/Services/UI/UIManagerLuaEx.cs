using Cysharp.Threading.Tasks;
using Saro.Events;
using Saro.UI;
using System;
using UnityEngine.Events;
using XLua;

namespace Saro.Lua.UI
{
    [LuaCallCSharp]
    public static class UIManagerLuaEx
    {
        /// <summary>
        /// 不给uiType，就默认XLuaUI
        /// </summary>
        /// <param name="self"></param>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public static async UniTask<BaseUI> OpenUIAsync(this UIManager self, string uiName)
        {
            return await self.OpenUIAsync<XLuaUI>(uiName);
        }

        public static async UniTask<BaseUI> OpenUIAsync(this UIManager self, Type uiType, string uiName)
        {
            return await self.OpenUIAsync(uiType, uiName);
        }

        public static void CloseUI(this UIManager self, string uiName)
        {
            self.CloseUI(uiName);
        }

        public static void Listen(this BaseUI self, UnityEvent src, UnityAction dst)
        {
            self.Listen(src, dst);
        }

        public static void Listen(this BaseUI self, UnityEvent<float> src, UnityAction<float> dst)
        {
            self.Listen(src, dst);
        }

        public static void Listen(this BaseUI self, UnityEvent<int> src, UnityAction<int> dst)
        {
            self.Listen(src, dst);
        }

        public static void Listen(this BaseUI self, int eventID, EventHandler<GameEventArgs> handler)
        {
            self.Listen(eventID, handler);
        }

        public static void Close(this BaseUI self)
        {
            self.Close();
        }
    }
}
