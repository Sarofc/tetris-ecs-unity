using Cysharp.Threading.Tasks;
using Saro.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.UI
{
    [UIWindow((int)EGameUI.UIAboutPanel, "Assets/Res/Prefabs/UI/UIAboutPanel.prefab")]
    public sealed partial class UIAboutPanel : UIWindow
    {
        public UIAboutPanel(string path) : base(path)
        {
        }

        protected override async void Awake()
        {
            base.Awake();

            Listen(btn_Mask.onClick, OnClick_Close);
            Listen(btn_icon.onClick, OnClick_OpenURL);

            btn_icon.GetComponent<Image>().sprite =
                 await AssetLoader.LoadAssetRefAsync<Sprite>("Assets/Arts/Textures/UI/icon_github.png");

            // TODO 大图，可以封装成工具类，加载时转圈，加载完成后再附上去
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
            UIManager.Current.HideWindow(EGameUI.UIAboutPanel);
        }
    }


    public partial class UIAboutPanel
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope
        //>>begin
        public Button btn_Mask => Binder.GetRef<Button>("btn_Mask");
        public Button btn_icon => Binder.GetRef<Button>("btn_icon");

        //<<end
        // =============================================
    }
}