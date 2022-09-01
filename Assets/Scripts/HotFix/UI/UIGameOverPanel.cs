using Cysharp.Threading.Tasks;
using Saro;
using Saro.Core;
using Saro.UI;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)EGameUI.GameOverPanel, "Assets/Res/Prefab/UI/UIGameOverPanel.prefab")]
    public sealed partial class UIGameOverPanel : UIWindow
    {
        public UIGameOverPanel(string resPath) : base(resPath)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            Listen(btn_back.onClick, () => { OnClick_Back().Forget(); });
            Listen(btn_setting.onClick, OnClick_Setting);
            Listen(btn_about.onClick, OnClick_About);
            Listen(btn_quit.onClick, Main.Quit);
        }

        private async UniTaskVoid OnClick_Back()
        {
            await SceneController.Current.ChangeScene(SceneController.ESceneType.Title);

            UIManager.Current.UnLoadWindow(EGameUI.GameOverPanel);
            UIManager.Current.UnLoadWindow(EGameUI.GameHUD);

            UIManager.Current.LoadAndShowWindowAsync(EGameUI.StartWindow).Forget();
        }

        private void OnClick_Setting()
        {
            UIManager.Current.LoadAndShowWindowAsync(EGameUI.SettingPanel).Forget();
        }

        private void OnClick_About()
        {
            UIManager.Current.LoadAndShowWindowAsync(EGameUI.AboutPanel).Forget();
        }
    }

    public partial class UIGameOverPanel
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope

        //>>begin
        public UnityEngine.UI.Button btn_back => Binder.GetRef<UnityEngine.UI.Button>("btn_back");
        public UnityEngine.UI.Button btn_setting => Binder.GetRef<UnityEngine.UI.Button>("btn_setting");
        public UnityEngine.UI.Button btn_about => Binder.GetRef<UnityEngine.UI.Button>("btn_about");
        public UnityEngine.UI.Button btn_quit => Binder.GetRef<UnityEngine.UI.Button>("btn_quit");

        //<<end

        // =============================================
    }
}