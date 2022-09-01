using Saro.Events;
using Saro.UI;
using TMPro;
using UnityEngine;

namespace Tetris.UI
{
    [UIWindow((int)EGameUI.GameHUD, "Assets/Res/Prefab/UI/UIGameHUD.prefab")]
    public sealed partial class UIGameHUD : UIWindow
    {
        public UIGameHUD(string resPath) : base(resPath)
        {
        }

        private GameContext GameCtx => UserData as GameContext;

        protected override void Awake()
        {
            base.Awake();

            Listen(TetrisScoreEventArgs.EventID, UpdateUI);
            Listen(TetrisLineClearArgs.EventID, TextVFX);
        }

        protected override void OnUpdate(float dt)
        {
            if (GameCtx != null)
                tmptxt_time.text = string.Format("{0:0.00}", GameCtx.gameTime);
        }

        private void UpdateUI(object sender, GameEventArgs args)
        {
            if (args is TetrisScoreEventArgs _args)
            {
                tmptxt_level.text = _args.level.ToString();
                tmptxt_score.text = _args.score.ToString();
                tmptxt_line.text = _args.line.ToString();
            }
        }

        private void TextVFX(object sender, GameEventArgs _args)
        {
            var args = _args as TetrisLineClearArgs;
            var isTSpin = args.isTSpin;
            var isMini = args.isMini;
            var line = args.line;
            var isB2B = args.isB2B;
            var ren = args.ren;

            if (line == 4) ProcessAnimation(go_Tetris);

            if (isTSpin)
            {
                if (line > 0)
                    ProcessAnimation(go_TSpin);

                if (isMini)
                    ProcessAnimation(go_Mini);
            }

            if (line == 1) ProcessAnimation(go_Single);

            if (line == 2) ProcessAnimation(go_Double);

            if (line == 3) ProcessAnimation(go_Triple);

            if (isB2B) ProcessAnimation(go_B2B);

            if (ren <= 0)
            {
                if (tmptxt_Ren.gameObject.activeSelf)
                    tmptxt_Ren.gameObject.SetActive(false);
            }
            else
            {
                if (tmptxt_Ren.gameObject.activeSelf == false)
                {
                    tmptxt_Ren.gameObject.SetActive(true);
                }
                else
                {
                    tmptxt_Ren.gameObject.SetActive(false);
                    tmptxt_Ren.gameObject.SetActive(true);
                }
            }

            tmptxt_Ren.text = "Ren " + ren;
        }

        private void ProcessAnimation(GameObject go)
        {
            go.SetActive(false);
            go.SetActive(true);
        }
    }

    public partial class UIGameHUD
    {
        // =============================================
        // code generate between >>begin and <<end
        // don't modify this scope

        //>>begin
        public TextMeshProUGUI tmptxt_Ren => Binder.GetRef<TextMeshProUGUI>("tmptxt_Ren");
        public GameObject go_B2B => Binder.GetRef<GameObject>("go_B2B");
        public GameObject go_Tetris => Binder.GetRef<GameObject>("go_Tetris");
        public GameObject go_TSpin => Binder.GetRef<GameObject>("go_TSpin");
        public GameObject go_Single => Binder.GetRef<GameObject>("go_Single");
        public GameObject go_Double => Binder.GetRef<GameObject>("go_Double");
        public GameObject go_Triple => Binder.GetRef<GameObject>("go_Triple");
        public GameObject go_Mini => Binder.GetRef<GameObject>("go_Mini");
        public TextMeshProUGUI tmptxt_level => Binder.GetRef<TextMeshProUGUI>("tmptxt_level");
        public TextMeshProUGUI tmptxt_line => Binder.GetRef<TextMeshProUGUI>("tmptxt_line");
        public TextMeshProUGUI tmptxt_score => Binder.GetRef<TextMeshProUGUI>("tmptxt_score");
        public TextMeshProUGUI tmptxt_time => Binder.GetRef<TextMeshProUGUI>("tmptxt_time");

        //<<end

        // =============================================
    }
}