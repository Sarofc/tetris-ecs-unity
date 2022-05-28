using Cysharp.Threading.Tasks;
using Saro;
using Saro.UI;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)ETetrisUI.GameOverPanel, "Assets/Res/Prefab/UI/UIGameOverPanel.prefab")]
    public sealed partial class UIGameOverPanel : UIWindow
    {
        public UIGameOverPanel(string resPath) : base(resPath)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            Listen(BtnReplay.onClick, OnClick_Replay);
            Listen(BtnSetting.onClick, OnClick_Setting);
            Listen(BtnAbout.onClick, OnClick_About);
            Listen(BtnQuit.onClick, Main.Quit);
        }

        private void OnClick_Replay()
        {
            Toast.AddToast("TODO 未实现");
        }

        private void OnClick_Setting()
        {
            UIManager.Instance.LoadAndShowWindowAsync(ETetrisUI.SettingPanel).Forget();
        }

        private void OnClick_About()
        {
            UIManager.Instance.LoadAndShowWindowAsync(ETetrisUI.AboutPanel).Forget();
        }
    }

    public partial class UIGameOverPanel
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope

        //>>begin
        public Button BtnReplay => Binder.GetRef<Button>("btn_replay");
        public Button BtnSetting => Binder.GetRef<Button>("btn_setting");
        public Button BtnAbout => Binder.GetRef<Button>("btn_about");
        public Button BtnQuit => Binder.GetRef<Button>("btn_quit");

        //<<end

        // =============================================
    }
}