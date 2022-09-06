using Cysharp.Threading.Tasks;
using Saro;
using Saro.Core;
using Saro.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)EGameUI.UIStartWindow, "Assets/Res/Prefabs/UI/UIStartPanel.prefab")]
    public sealed partial class UIStartPanel : UIWindow
    {
        public UIStartPanel(string path) : base(path)
        {
        }

        protected override void Awake()
        {
            base.Awake();

            Listen(btn_start.onClick, () => { OnClick_Start().Forget(); });
            Listen(btn_setting.onClick, OnClick_Setting);
            Listen(btn_about.onClick, OnClick_About);
            Listen(btn_quit.onClick, OnClick_Quit);

            var iasset = IAssetManager.Current;
            tmptxt_version.text = "v." + iasset.GetAppVersion() + "." + iasset.GetResVersion();

            Listen(btn_reset.onClick, () =>
            {
                var info = new Saro.UI.AlertDialogInfo
                {
                    title = "重要操作",
                    content = $"确认删除DLC目录？",
                    leftText = "取消",
                    rightText = "确认",
                    clickHandler = (state) =>
                    {
                        if (state == 1)
                        {
                            IAssetManager.Current.DeleteDLC();
                            Application.Quit();
                        }
                    }
                };

                Saro.UI.UIManager.Current.QueueAsync(EDefaultUI.UIAlertDialog, 0, info, EUILayer.Top);
            });

            Listen(btn_check.onClick, () =>
            {
                var info = new Saro.UI.AlertDialogInfo
                {
                    title = "重要操作",
                    content = $"校验文件？",
                    leftText = "取消",
                    rightText = "确认",
                    clickHandler = async (state) =>
                    {
                        if (state == 1)
                        {
                            var uicheckAssets = await UIManager.Current.LoadAndShowWindowAsync<UICheckAssets>(EGameUI.UICheckAssets, layer: EUILayer.Top);
                            uicheckAssets.btn_Mask.interactable = false;

                            var result = (IAssetManager.Current as Saro.MoonAsset.MoonAsset).VerifyAllAssetsUseManifest((data) =>
                             {
                                 uicheckAssets.tmptxt_info.text = $"{string.Format("{0}%", data.percent * 100)}  {data.fileName}";
                             });

                            uicheckAssets.tmptxt_info.text = $"result: {(string.IsNullOrEmpty(result) ? "ok" : result)}";
                            uicheckAssets.btn_Mask.interactable = true;
                        }
                    }
                };

                Saro.UI.UIManager.Current.QueueAsync(EDefaultUI.UIAlertDialog, 0, info, EUILayer.Top);
            });
        }

        private async UniTaskVoid OnClick_Start()
        {
            await SceneController.Current.ChangeScene(SceneController.ESceneType.Game);

            UIManager.Current.UnLoadWindow(EGameUI.UIStartWindow);
        }

        private void OnClick_Setting()
        {
            UIManager.Current.LoadAndShowWindowAsync(EGameUI.UISettingPanel).Forget();
        }

        private void OnClick_About()
        {
            UIManager.Current.LoadAndShowWindowAsync(EGameUI.UIAboutPanel).Forget();
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
		public UnityEngine.UI.Button btn_start => Binder.GetRef<UnityEngine.UI.Button>("btn_start");
		public UnityEngine.UI.Button btn_setting => Binder.GetRef<UnityEngine.UI.Button>("btn_setting");
		public UnityEngine.UI.Button btn_about => Binder.GetRef<UnityEngine.UI.Button>("btn_about");
		public UnityEngine.UI.Button btn_quit => Binder.GetRef<UnityEngine.UI.Button>("btn_quit");
		public TMPro.TextMeshProUGUI tmptxt_version => Binder.GetRef<TMPro.TextMeshProUGUI>("tmptxt_version");
		public UnityEngine.UI.Button btn_reset => Binder.GetRef<UnityEngine.UI.Button>("btn_reset");
		public UnityEngine.UI.Button btn_check => Binder.GetRef<UnityEngine.UI.Button>("btn_check");

	//<<end
        // =============================================
    }
}