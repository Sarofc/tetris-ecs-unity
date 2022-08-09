using Saro.Events;
using Saro.UI;
using TMPro;
using UnityEngine;

namespace Tetris.UI
{
    [UIWindow((int)ETetrisUI.GameHUD, "Assets/Res/Prefab/UI/UIGameHUD.prefab")]
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
                TmptxtTime.text = string.Format("{0:0.00}", GameCtx.gameTime);
        }

        private void UpdateUI(object sender, GameEventArgs args)
        {
            if (args is TetrisScoreEventArgs _args)
            {
                TmptxtLevel.text = _args.level.ToString();
                TmptxtScore.text = _args.score.ToString();
                TmptxtLine.text = _args.line.ToString();
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

            if (line == 4) ProcessAnimation(GoTetris);

            if (isTSpin)
            {
                if (line > 0)
                    ProcessAnimation(GoTSpin);

                if (isMini)
                    ProcessAnimation(GoMini);
            }

            if (line == 1) ProcessAnimation(GoSingle);

            if (line == 2) ProcessAnimation(GoDouble);

            if (line == 3) ProcessAnimation(GoTriple);

            if (isB2B) ProcessAnimation(GoB2B);

            if (ren <= 0)
            {
                if (TmptxtRen.gameObject.activeSelf)
                    TmptxtRen.gameObject.SetActive(false);
            }
            else
            {
                if (TmptxtRen.gameObject.activeSelf == false)
                {
                    TmptxtRen.gameObject.SetActive(true);
                }
                else
                {
                    TmptxtRen.gameObject.SetActive(false);
                    TmptxtRen.gameObject.SetActive(true);
                }
            }

            TmptxtRen.text = "Ren " + ren;
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
        public TextMeshProUGUI TmptxtRen => Binder.GetRef<TextMeshProUGUI>("tmptxt_Ren");
        public GameObject GoB2B => Binder.GetRef<GameObject>("go_B2B");
        public GameObject GoTetris => Binder.GetRef<GameObject>("go_Tetris");
        public GameObject GoTSpin => Binder.GetRef<GameObject>("go_TSpin");
        public GameObject GoSingle => Binder.GetRef<GameObject>("go_Single");
        public GameObject GoDouble => Binder.GetRef<GameObject>("go_Double");
        public GameObject GoTriple => Binder.GetRef<GameObject>("go_Triple");
        public GameObject GoMini => Binder.GetRef<GameObject>("go_Mini");
        public TextMeshProUGUI TmptxtLevel => Binder.GetRef<TextMeshProUGUI>("tmptxt_level");
        public TextMeshProUGUI TmptxtLine => Binder.GetRef<TextMeshProUGUI>("tmptxt_line");
        public TextMeshProUGUI TmptxtScore => Binder.GetRef<TextMeshProUGUI>("tmptxt_score");
        public TextMeshProUGUI TmptxtTime => Binder.GetRef<TextMeshProUGUI>("tmptxt_time");

        //<<end

        // =============================================
    }
}