using System;
using System.Collections.Generic;
using DG.Tweening;
using Saro;
using UnityEngine;

namespace Tetris
{
    public class Tetris
    {
        public const int k_Height = 20;
        public const int k_Width = 10;
        public const int k_ExtraHeight = 2;
        public const float k_DeltaNormal = 1f;
        public const float k_DeltaSoft = 0.03f;

        public static Transform[,] Grid { get; private set; }

        public int Level { get; private set; } = 1;
        public int Score { get; private set; } = 0;
        public int Line { get; private set; } = 0;
        public float Time { get; private set; }

        public event Action OnGameStart;
        public event Action OnGameOver;

        public event Action<int> OnLineClear;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnLevelChanged;
        public event Action<int> OnLineChanged;
        public event Action<float> OnTimeChanged;
        public event Action<int> OnRenText;

        public event Action OnBlockLanding;
        public event Action OnBlockRotate;
        public event Action OnBlockMove;
        public event Action<Block> OnHoldBlock;
        public event Action<Vector2> OnBlockSoftDrop;
        public event Action<Vector2> OnBlockHardDrop;

        // auto fall delta time
        public float FallDeltaTime
        {
            get
            {
                switch (m_MoveDelta)
                {
                    case EMoveDelta.Normal:
                        return k_DeltaNormal / Level;
                    case EMoveDelta.SoftDrop:
                        return k_DeltaSoft / Level;
                }

                return 0f;
            }
        }

        private float m_LastFallTime;

        // check landed
        private readonly float m_WaitLandTime = .4f;
        private float m_LastWaitTime;
        private bool m_IsLanding = false;
        private bool m_Landed = false;

        // special clear check
        private bool m_LandedWithRotate = false;
        private bool m_IsTSpin = false;
        private bool m_IsMini = false;
        private bool m_LastClearIsSpecial = false;
        private int m_Ren = 0;

        private EMoveDelta m_MoveDelta = EMoveDelta.Normal;

        private Block m_CurrentBlock;
        //private bool gameOver = false;

        private Block m_Holded;
        private bool m_HoldedThisTurn;

        private BlockSpawner m_BlockSpawner;
        //private VFX vfx;

        // block land point preview
        private Block m_Preview;

        private enum EMoveDelta
        {
            Normal,
            SoftDrop,
            HardDrop
        }

        public Tetris(BlockSpawner spawner)
        {
            Grid = new Transform[k_Width, k_Height + k_ExtraHeight];
            m_BlockSpawner = spawner;

            m_BlockSpawner.UpdateNextChainSlot();
        }

        public void StartGame()
        {
            GameState = EGameState.Gamming;

            OnGameStart?.Invoke();

            NextBlock();
        }

        public void PauseGame()
        {
            GameState = EGameState.Pause;
        }

        private void NextBlock()
        {
            if (GameState == EGameState.GameOver || GameState == EGameState.Pause) return;

            m_CurrentBlock = m_BlockSpawner.NextBlock();

            // check game over
            if (!m_CurrentBlock.MoveDown())
            {
                foreach (Transform child in m_CurrentBlock.transform)
                {
                    var x = Mathf.RoundToInt(child.transform.position.x);
                    var y = Mathf.RoundToInt(child.transform.position.y);

                    if (Grid[x, y] != null)
                    {
                        GameState = EGameState.GameOver;
                        OnGameOver?.Invoke();
                        m_CurrentBlock = null;
                        return;
                    }
                }
            }

            InitPreview();

            UpdatePreview();

            m_BlockSpawner.UpdateNextChainSlot();

            m_HoldedThisTurn = false;

            m_LandedWithRotate = false;

            m_IsTSpin = false;
        }


        #region Block Control

        public EGameState GameState { get; private set; }

        private bool m_LeftPressed;
        private bool m_RightPressed;
        private bool m_UpPressed;

        private float m_StartTime = 0.15f;
        private float m_LastStartTime;

        private float m_InputDelta = 0.03f;
        private float m_LastInputTime;

        public enum EGameState
        {
            Pause,
            Gamming,
            GameOver,
        }

        private float holdedViewPosX = -2.4f;
        private float holdedViewPosY = 16;
        private float holdedViewPosOffset = 0.3f;
        private float sizeScale = 0.6f;

        private Vector3 originSize;

        public void Update(float deltaTime)
        {
            ProcessBlockInput(deltaTime);
            Fall(deltaTime);
        }

        private void HoldBlock()
        {
            if (m_CurrentBlock == null) return;

            if (m_HoldedThisTurn) return;

            originSize = m_CurrentBlock.transform.localScale;

            if (m_Holded != null)
            {
                var tmp = m_CurrentBlock;
                m_CurrentBlock = m_Holded;
                m_Holded = tmp;

                m_CurrentBlock.transform.position = new Vector3(Tetris.k_Width / 2, Tetris.k_Height);
                m_CurrentBlock.transform.localScale = originSize;

                DestroyPreview();
                InitPreview();
                UpdatePreview();
            }
            else
            {
                DestroyPreview();
                m_Holded = m_CurrentBlock;
                NextBlock();
            }

            OnHoldBlock?.Invoke(m_Holded);
            //vfx?.PlayClip(SV.ClipHold);

            // reset block state
            m_MoveDelta = EMoveDelta.Normal;

            m_HoldedThisTurn = true;
            if (m_Holded is BlockO || m_Holded is BlockI)
            {
                m_Holded.transform.position = new Vector3(holdedViewPosX - holdedViewPosOffset, holdedViewPosY, 0);
            }
            else
            {
                m_Holded.transform.position = new Vector3(holdedViewPosX, holdedViewPosY, 0);
            }
            m_Holded.transform.localScale = new Vector3(sizeScale, sizeScale);
            m_Holded.transform.rotation = Quaternion.Euler(0, 0, 0);
            m_Holded.ResetState();
        }

        private void MoveRight()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            if (m_CurrentBlock.MoveRight())
            {
                m_LandedWithRotate = false;
                m_LastWaitTime = 0;
                //vfx?.PlayClip(SV.ClipMove);
                OnBlockMove?.Invoke();
                UpdatePreview();
            }
        }

        private void MoveLeft()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            if (m_CurrentBlock.MoveLeft())
            {
                m_LandedWithRotate = false;
                m_LastWaitTime = 0;
                OnBlockMove?.Invoke();
                //vfx?.PlayClip(SV.ClipMove);
                UpdatePreview();
            }
        }

        private void ClockwiseRotation()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            if (m_CurrentBlock.ClockwiseRotation())
            {
                m_LandedWithRotate = true;
                m_LastWaitTime = 0;
                OnBlockRotate?.Invoke();
                //vfx?.PlayClip(SV.ClipRotate);
                UpdatePreview();
            }
        }

        private void AntiClockwiseRotation()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            if (m_CurrentBlock.AntiClockwiseRotation())
            {
                m_LandedWithRotate = true;
                m_LastWaitTime = 0;
                OnBlockRotate?.Invoke();
                //vfx?.PlayClip(SV.ClipRotate);
                UpdatePreview();
            }
        }

        private void SoftDrop()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            m_MoveDelta = EMoveDelta.SoftDrop;
        }

        private void NormalDrop()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            m_MoveDelta = EMoveDelta.Normal;
        }

        private void HardDrop()
        {
            if (m_CurrentBlock == null || m_Landed) return;
            m_LandedWithRotate = false;

            while (m_CurrentBlock.MoveDown()) ;
            m_Landed = true;

            OnBlockHardDrop?.Invoke(m_CurrentBlock.transform.position);

            //if (vfx)
            //{
            //    vfx.VFX_HardDrop(currentBlock.transform.position);
            //    vfx.PlayClip(SV.ClipHardDrop);
            //}
        }

        private void ProcessBlockInput(float deltaTime)
        {
            // hard & soft drop
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop(); // execute in one frame
            }
            else if (!m_UpPressed && Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_UpPressed = true;
                SoftDrop();
            }
            else if (m_UpPressed && Input.GetKeyUp(KeyCode.DownArrow))
            {
                m_UpPressed = false;
                NormalDrop();
            }

            // move left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
                m_LeftPressed = true;
            }
            if (!m_RightPressed && m_LeftPressed && Input.GetKey(KeyCode.LeftArrow))
            {
                if (m_LastStartTime >= m_StartTime)
                {
                    if (m_LastInputTime >= m_InputDelta)
                    {
                        MoveLeft();
                        m_LastInputTime = 0;
                    }
                    else
                    {
                        m_LastInputTime += deltaTime;
                    }
                }
                else
                {
                    m_LastStartTime += deltaTime;
                }
            }
            if (m_LeftPressed && Input.GetKeyUp(KeyCode.LeftArrow))
            {
                m_LeftPressed = false;
                m_LastStartTime = 0;
                m_LastInputTime = 0;
            }

            // move right
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
                m_RightPressed = true;
            }
            if (!m_LeftPressed && m_RightPressed && Input.GetKey(KeyCode.RightArrow))
            {
                if (m_LastStartTime >= m_StartTime)
                {
                    if (m_LastInputTime >= m_InputDelta)
                    {
                        MoveRight();

                        m_LastInputTime = 0;
                    }
                    else
                    {
                        m_LastInputTime += deltaTime;
                    }
                }
                else
                {
                    m_LastStartTime += deltaTime;
                }
            }
            if (m_RightPressed && Input.GetKeyUp(KeyCode.RightArrow))
            {
                m_RightPressed = false;
                m_LastStartTime = 0;
                m_LastInputTime = 0;
            }

            // rotate
            if (Input.GetKeyDown(KeyCode.Z))
            {
                AntiClockwiseRotation();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                ClockwiseRotation();
            }

            // hold
            if (Input.GetKeyDown(KeyCode.C))
            {
                HoldBlock();
            }
        }

        private void Fall(float deltaTime)
        {
            if (GameState == EGameState.Pause || GameState == EGameState.GameOver) return;

            Time += deltaTime;
            OnTimeChanged?.Invoke(Time);

            if (m_CurrentBlock == null) return;

            if (!m_Landed)
            {
                if (!m_IsLanding)
                {
                    if (m_LastFallTime >= FallDeltaTime)
                    {
                        m_IsLanding = !m_CurrentBlock.MoveDown();
                        m_LastFallTime = 0;
                    }
                    else
                    {
                        m_LastFallTime += deltaTime;
                    }
                }
                else
                {
                    if (m_LastWaitTime >= m_WaitLandTime)
                    {
                        m_Landed = true;
                        m_LastWaitTime = 0;
                        OnBlockSoftDrop?.Invoke(m_CurrentBlock.transform.position);
                        //vfx?.PlayClip(SV.ClipSoftDrop);
                    }
                    else
                    {
                        m_IsLanding = !m_CurrentBlock.MoveDown();
                        m_LastWaitTime += deltaTime;
                    }
                }

            }
            else
            {
                // trigger vfx before check(), because the currentBlock reference has been cleared after check() method.
                //vfx?.PlayClip(SV.ClipLanding);
                OnBlockLanding?.Invoke();

                DestroyPreview();

                AddToGrid();

                if (GameState == EGameState.GameOver) return;

                Check();

                m_MoveDelta = EMoveDelta.Normal;
                m_IsLanding = false;
                m_Landed = false;
            }
        }

        #endregion


        #region Check

        private void AddToGrid()
        {
            bool res = true;
            foreach (Transform child in m_CurrentBlock.transform)
            {
                var x = Mathf.RoundToInt(child.transform.position.x);
                var y = Mathf.RoundToInt(child.transform.position.y);

                if (y < k_Height) res = false;

                Grid[x, y] = child;
            }

            // check game over
            if (res)
            {
                GameState = EGameState.GameOver;
                OnGameOver?.Invoke();
                m_CurrentBlock = null;
            }
        }

        // which line should clear
        private List<int> m_Rows = new List<int>(4);

        private void Check()
        {
            // check T-Spin
            if (m_CurrentBlock is BlockT blockT)
            {
                m_IsTSpin = blockT.IsTSpin(out m_IsMini) && m_LandedWithRotate;
                Log.INFO($"roateted {m_LandedWithRotate}, special {m_IsTSpin}");
            }

            foreach (Transform child in m_CurrentBlock.transform)
            {
                var y = Mathf.RoundToInt(child.transform.position.y);

                if (!m_Rows.Contains(y)) m_Rows.Add(y);
            }

            // lock block move
            m_CurrentBlock = null;

            for (int i = m_Rows.Count - 1; i >= 0; i--)
            {
                if (!HasLine(m_Rows[i]))
                {
                    m_Rows.RemoveAt(i);
                }
                else
                {
                    ClearLine(m_Rows[i]);

                    OnLineClear?.Invoke(m_Rows[i]);
                    //vfx.VFX_LineClear(m_rows[i]);
                }
            }

            if (m_Rows.Count > 0)
            {
                m_Rows.Sort();

                // update score, etc.
                UpdateData(m_Rows.Count);
                m_Ren++;

                if (m_Ren > 0)
                {
                    OnRenText?.Invoke(m_Ren);
                    //vfx.UpdateTextRen(ren);
                }

                DOVirtual.DelayedCall(0.3f, () =>
                {
                    for (int i = m_Rows.Count - 1; i >= 0; i--)
                    {
                        DownLine(m_Rows[i]);
                    }

                    NextBlock();

                    m_Rows.Clear();
                });
            }
            else
            {
                NextBlock();
                m_Rows.Clear();

                // clear combo data
                m_Ren = -1;
                //vfx.HideTextRen();
            }
        }

        private bool HasLine(int row)
        {
            for (int i = 0; i < k_Width; i++)
            {
                if (Grid[i, row] == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearLine(int row)
        {
            for (int i = 0; i < k_Width; i++)
            {
                if (Grid[i, row] != null)
                {
                    GameObject.Destroy(Grid[i, row].gameObject);
                    Grid[i, row] = null;

#if UNITY_EDITOR
                    Debug.DrawLine(new Vector2(0, row), new Vector2(10, row), Color.red, 2);
#endif
                }
            }
        }

        private void DownLine(int row)
        {
            for (int i = row; i < k_Height + k_ExtraHeight; i++)
            {
                for (int j = 0; j < k_Width; j++)
                {
                    if (Grid[j, i] == null) continue;

                    Grid[j, i - 1] = Grid[j, i];
                    Grid[j, i] = null;
                    Grid[j, i - 1].position += Vector3.down * 1f;
                }
            }
        }

        #endregion


        #region Block Preview 
        private void InitPreview()
        {
            m_Preview = GameObject.Instantiate(m_CurrentBlock);
            foreach (Transform child in m_Preview.transform)
            {
                var sr = child.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    sr.color = new Color(1, 1, 1, .3f);
                    sr.sortingOrder = 15;
                }
            }
        }

        private void UpdatePreview()
        {
            if (!m_Preview) return;
            m_Preview.transform.position = m_CurrentBlock.transform.position;
            m_Preview.transform.rotation = m_CurrentBlock.transform.rotation;
            LandingPoint(m_Preview);
        }

        private void DestroyPreview()
        {
            if (m_Preview != null)
            {
                GameObject.Destroy(m_Preview.gameObject);
            }
        }

        private void LandingPoint(Block block)
        {
            while (block.MoveDown()) ;
        }
        #endregion

        private void UpdateData(int count)
        {
            var special = m_IsTSpin || count == 4;

            if (m_LastClearIsSpecial && special)
            {
                //vfx.TextVFX_B2B();
            }

            m_LastClearIsSpecial = special;

            int points = 0;
            if (m_IsTSpin)
            {
                if (m_IsMini)
                {
                    switch (count)
                    {
                        case 1:
                            points = 200 * Level;
                            //vfx.TextVFX_TSpinMiniSingle();
                            break;
                        case 2:
                            points = 1200 * Level;
                            //vfx.TextVFX_TSpinMiniDouble();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (count)
                    {
                        case 1:
                            points = 800 * Level;
                            //vfx.TextVFX_TSpinSingle();
                            break;
                        case 2:
                            points = 1200 * Level;
                            //vfx.TextVFX_TSpinDouble();
                            break;
                        case 3:
                            points = 1600 * Level;
                            //vfx.TextVFX_TSpinTriple();
                            break;
                        default:
                            break;
                    }
                }


                //vfx.PlayClip(SV.ClipSpecial);
            }
            else
            {
                switch (count)
                {
                    case 1:
                        points = 100 * Level;
                        //vfx.PlayClip(SV.ClipSingle);
                        break;
                    case 2:
                        points = 300 * Level;
                        //vfx.PlayClip(SV.ClipDouble);
                        break;
                    case 3:
                        points = 500 * Level;
                        //vfx.PlayClip(SV.ClipTriple);
                        break;
                    case 4:
                        points = 800 * Level;
                        //vfx.PlayClip(SV.ClipTetris);
                        //vfx.TextVFX_Tetris();
                        break;
                    default:
                        break;
                }
            }


            if (special) points = (int)(points * 1.5f);
            Score += points;

            OnScoreChanged?.Invoke(Score);

            if (Score / ((Level + 1) * (Level + 1) * (Level + 1)) > 1000)
            {
                Level++;
                OnLevelChanged?.Invoke(Level);
            }

            Line += count;
            OnLineChanged?.Invoke(Line);
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            Gizmos.color = Color.black;

            if (Grid != null)
            {
                for (int i = 0; i < Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < Grid.GetLength(1); j++)
                    {
                        if (Grid[i, j] != null) Gizmos.DrawCube(Grid[i, j].transform.position, new Vector2(.9f, .9f));
                    }
                }
            }

            if (m_CurrentBlock != null)
            {
                m_CurrentBlock.DrawGizmos();
            }
        }
#endif
    }

}