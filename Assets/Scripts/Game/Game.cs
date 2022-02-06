using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using Saro;
using Saro.XAsset;
using Saro.Audio;
using Saro.UI;
using Tetris.UI;
using Cysharp.Threading.Tasks;
using Saro.Lua.UI;

namespace Tetris
{
    public class Game : MonoBehaviour
    {
        //public float DAS { get { return startTime; } set { startTime = value; } }

        [SerializeField] private Block[] blocks;
        [SerializeField] LineRenderer linePrefab;
        [SerializeField] PostProcessVolume m_Volume;

        private Tetris m_tetris;

        private const float offset = .5f;

        void ListenEvents()
        {
            m_tetris.OnGameStart += OnGameStart;
            m_tetris.OnGameOver += OnGameOver;

            m_tetris.OnBlockHardDrop += OnBlockHardDrop;
            m_tetris.OnBlockLanding += OnBlockLanding;
            m_tetris.OnBlockMove += OnBlockMove;
            m_tetris.OnBlockRotate += OnBlockRotate;
            m_tetris.OnBlockSoftDrop += OnBlockSoftDrop;
            m_tetris.OnHoldBlock += OnHoldBlock;

            m_tetris.OnLevelChanged += OnLevelChanged;
            m_tetris.OnLineChanged += OnLineChanged;
            m_tetris.OnRenText += OnRenText;
            m_tetris.OnScoreChanged += OnScoreChanged;
            m_tetris.OnTimeChanged += OnTimeChanged;
        }

        private void OnGameStart()
        {
            SoundComponent.Current.PlayBGMAsync("BGM/bgm_t02_swap_t.wav", time: 0.1f).Forget();

            // TODO open ui
            //await UIComponent.Instance.OpenUIAsync<UIStartPanel>();
        }

        private void OnGameOver()
        {
            //UIComponent.Current.OpenUIAsync<UIGameOverPanel>().Forget();
            UIComponent.Current.OpenUIAsync("UIGameOverPanel").Forget();
        }


        private void OnTimeChanged(float obj)
        {
        }

        private void OnScoreChanged(int obj)
        {
        }

        private void OnRenText(int obj)
        {
        }

        private void OnLineChanged(int obj)
        {
        }

        private void OnLevelChanged(int obj)
        {
        }

        private void OnHoldBlock(Block obj)
        {
            SoundComponent.Current.PlaySEAsync("SE/se_game_hold.wav").Forget();
        }

        private void OnBlockSoftDrop(Vector2 obj)
        {
            SoundComponent.Current.PlaySEAsync("SE/se_game_softdrop.wav").Forget();
        }

        private void OnBlockRotate()
        {
            SoundComponent.Current.PlaySEAsync("SE/se_game_rotate.wav").Forget();
        }

        private void OnBlockMove()
        {
            SoundComponent.Current.PlaySEAsync("SE/se_game_move.wav").Forget();
        }

        private void OnBlockLanding()
        {
            SoundComponent.Current.PlaySEAsync("SE/se_game_landing.wav").Forget();
        }

        private void OnBlockHardDrop(Vector2 obj)
        {
            SoundComponent.Current.PlaySEAsync("SE/se_game_harddrop.wav").Forget();
        }

        async void Start()
        {
            var handle = FGame.Resolve<XAssetComponent>().LoadAssetAsync("Assets/Res/Volumes/GameScene.asset", typeof(PostProcessProfile));
            await handle;
            m_Volume.profile = handle.GetAsset<PostProcessProfile>();

            m_tetris = new Tetris(new BlockSpawner(blocks));

            ListenEvents();

            // draw row line
            for (int i = 0; i <= Tetris.k_Height + Tetris.k_ExtraHeight - 1; i++)
            {
                var row = Instantiate(linePrefab, this.transform);
                row.positionCount = 2;
                row.SetPosition(0, new Vector3(-offset, i - offset));
                row.SetPosition(1, new Vector3(Tetris.k_Width - offset, i - offset));
            }

            // draw col line
            for (int i = 0; i <= Tetris.k_Width; i++)
            {
                var col = Instantiate(linePrefab, this.transform);
                col.positionCount = 2;
                col.SetPosition(0, new Vector3(i - offset, -offset));
                col.SetPosition(1, new Vector3(i - offset, Tetris.k_Height + Tetris.k_ExtraHeight - 1 - offset));
            }

            StartGame();
        }

        void Update()
        {
            if (m_tetris != null)
            {
                m_tetris.Update(Time.deltaTime);
            }
        }

        public void PauseGame()
        {
            m_tetris.PauseGame();
        }

        public void StartGame()
        {
            StartCoroutine(Gamming());
        }

        private IEnumerator Gamming()
        {
            yield return new WaitForSeconds(1f);

            //vfx.TextVFX_Start();

            yield return new WaitForSeconds(5f);

            m_tetris.StartGame();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_tetris != null) m_tetris.DrawGizmos();
        }
#endif
    }
}