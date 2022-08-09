using Cysharp.Threading.Tasks;
using Saro.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)ETetrisUI.AboutPanel, "Assets/Res/Prefab/UI/UIAboutPanel.prefab")]
    public sealed partial class UIAboutPanel : UIWindow
    {
        public UIAboutPanel(string path) : base(path)
        {
        }

        protected override async void Awake()
        {
            base.Awake();

            Listen(BtnMask.onClick, OnClick_Close);
            Listen(BtnIcon.onClick, OnClick_OpenURL);

            BtnIcon.GetComponent<Image>().sprite =
                 await AssetLoader.LoadAssetRefAsync<Sprite>("Assets/Arts/Textures/UI/icon_github.png");

            //AssetLoader.LoadAssetRefAsync<Sprite>("Assets/Arts/Textures/UI/icon_github.png").ContinueWith((sprite) =>
            //{
            //    BtnIcon.GetComponent<Image>().sprite = sprite;
            //}).Forget();
        }

        private void OnClick_OpenURL()
        {
            Application.OpenURL("https://github.com/Sarofc/com.saro.mgf");
        }

        private void OnClick_Close()
        {
            UIManager.Current.HideWindow(ETetrisUI.AboutPanel);
        }
    }


    public partial class UIAboutPanel
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope
        //>>begin
        public Button BtnMask => Binder.GetRef<Button>("btn_Mask");
        public Button BtnIcon => Binder.GetRef<Button>("btn_icon");

        //<<end
        // =============================================
    }
}