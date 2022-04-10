using Cysharp.Threading.Tasks;
using Saro;
using Saro.Core;
using Saro.UI;

namespace Tetris.UI
{
    public sealed partial class UIStartPanel : SingletonUI<UIStartPanel, UIBinder>
    {
        #region Impl

        protected override void InternalStart()
        {
        }

        protected override void InternalAwake()
        {
            GetComps();

            Listen(btn_start.onClick, () =>
            {
                OnClick_Start().Forget();
            });

            Listen(btn_setting.onClick, OnClick_Setting);
            Listen(btn_about.onClick, OnClick_About);
            Listen(btn_quit.onClick, OnClick_Quit);
        }


        private async UniTaskVoid OnClick_Start()
        {
            var sceenHandle = Main.Resolve<IAssetInterface>().LoadSceneAsync("Assets/Res/Scenes/Gaming.unity");
            await sceenHandle;
            sceenHandle.DecreaseRefCount();
        }


        private void OnClick_Setting()
        {
            UIManager.Current.OpenUIAsync<UISetting>().Forget();
        }

        private void OnClick_About()
        {
            UIManager.Current.OpenUIAsync<UIAboutPanel>().Forget();
        }

        private void OnClick_Quit()
        {
            Main.Quit();
        }

        #endregion
    }

    // =============================================
    // code generate between >>begin and <<end
    // don't modify this scope

    //>>begin

    public partial class UIStartPanel
    {
        private UnityEngine.UI.Button btn_start;
        private UnityEngine.UI.Button btn_setting;
        private UnityEngine.UI.Button btn_about;
        private UnityEngine.UI.Button btn_quit;

        private void GetComps()
        {
            btn_start = Binder.Get<UnityEngine.UI.Button>("btn_start");
            btn_setting = Binder.Get<UnityEngine.UI.Button>("btn_setting");
            btn_about = Binder.Get<UnityEngine.UI.Button>("btn_about");
            btn_quit = Binder.Get<UnityEngine.UI.Button>("btn_quit");
        }
    }

    //<<end

    // =============================================
}
