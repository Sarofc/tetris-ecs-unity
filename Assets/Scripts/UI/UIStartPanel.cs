using Cysharp.Threading.Tasks;
using Saro;
using Saro.Core;
using Saro.UI;
using TMPro;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)ETetrisUI.StartWindow, "Assets/Res/Prefab/UI/UIStartPanel.prefab")]
    public sealed partial class UIStartPanel : UIWindow
    {
        public UIStartPanel(string path) : base(path)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            Listen(BtnStart.onClick, () => { OnClick_Start().Forget(); });
            Listen(BtnSetting.onClick, OnClick_Setting);
            Listen(BtnAbout.onClick, OnClick_About);
            Listen(BtnQuit.onClick, OnClick_Quit);

            var iasset = Main.Resolve<IAssetManager>();
            TmptxtVersion.text = "v." + iasset.GetAppVersion() + "." + iasset.GetResVersion();
        }

        private async UniTaskVoid OnClick_Start()
        {
            var sceneHandle = Main.Resolve<IAssetManager>().LoadSceneAsync("Assets/Res/Scenes/EcsGaming.unity");
            await sceneHandle;
            //sceenHandle.DecreaseRefCount();

            UIManager.Current.UnLoadWindow(ETetrisUI.StartWindow);
        }

        private void OnClick_Setting()
        {
            UIManager.Current.LoadAndShowWindowAsync(ETetrisUI.SettingPanel).Forget();
        }

        private void OnClick_About()
        {
            UIManager.Current.LoadAndShowWindowAsync(ETetrisUI.AboutPanel).Forget();
        }

        private void OnClick_Quit()
        {
            Main.Quit();
        }
    }

    public partial class UIStartPanel
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope
        //>>begin
        public Button BtnStart => Binder.GetRef<Button>("btn_start");
        public Button BtnSetting => Binder.GetRef<Button>("btn_setting");
        public Button BtnAbout => Binder.GetRef<Button>("btn_about");
        public Button BtnQuit => Binder.GetRef<Button>("btn_quit");
        public TextMeshProUGUI TmptxtVersion => Binder.GetRef<TextMeshProUGUI>("tmptxt_version");

        //<<end
        // =============================================
    }
}