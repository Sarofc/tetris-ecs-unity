using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris
{
    public const int Height = 20;
    public const int Width = 10;
    public const int ExtraHeight = 2;
    public const float DeltaNormal = 1f;
    public const float DeltaSoft = .03f;

    public static Transform[,] Grid;

    //public static Logger logger = new Logger();

    public int level = 1;
    public int score = 0;
    public int line = 0;
    public float time;

    public static Action<int> OnScoreChanged;
    public static Action<int> OnLevelChanged;
    public static Action<int> OnGoalChanged;
    public static Action<float> OnTimeChanged;
    public static Action OnGameOver;

    public static Action<Vector2> OnHardDrop;
    public static Action<int> OnLineClear;

    // auto fall delta time
    public float FallDeltaTime
    {
        get
        {
            switch (moveDelta)
            {
                case MoveDelda.Normal:
                    return DeltaNormal / level;
                case MoveDelda.SoftDrop:
                    return DeltaSoft / level;
            }

            return 0f;
        }
    }
    private float lastFallTime;

    // check landed
    private float waitLandTime = .5f;
    private float lastWaitTime;
    private bool isLanding = false;
    private bool landed = false;

    // special clear check
    private bool landedWithRotate = false;
    private bool isTSpin = false;
    private bool isMini = false;
    private bool lastClearIsSpecial = false;
    private int ren = 0;

    private MoveDelda moveDelta = MoveDelda.Normal;

    private Block currentBlock;
    private bool gameOver = false;

    private Block holded;
    private bool holdedThisTurn;

    private BlockSpawner m_spawner;
    private VFX vfx;

    // block land point preview
    private Block m_preview;

    private enum MoveDelda
    {
        Normal,
        SoftDrop,
        HardDrop
    }


    public Tetris(BlockSpawner spawner)
    {
        Grid = new Transform[Width, Height + ExtraHeight];
        m_spawner = spawner;

        m_spawner.InitNextChainSlot();
    }

    public Tetris(BlockSpawner spawner, VFX vfx) : this(spawner)
    {
        this.vfx = vfx;
    }

    public void NextBlock()
    {
        if (gameOver) return;

        currentBlock = m_spawner.NextBlock();

        // check game over
        if (!currentBlock.MoveDown())
        {
            foreach (Transform child in currentBlock.transform)
            {
                var x = EX.Float2Int(child.transform.position.x);
                var y = EX.Float2Int(child.transform.position.y);

                if (Grid[x, y] != null)
                {
                    OnGameOver?.Invoke();
                    gameOver = true;
                    currentBlock = null;
                    return;
                }
            }
        }

        InitPreview();

        UpdatePreview();

        m_spawner.UpdateNextChainSlot();

        holdedThisTurn = false;

        landedWithRotate = false;

        isTSpin = false;

        //logger.Log("New Block");
    }


    #region Block Control

    private float holdedViewPosX = -2.4f;
    private float holdedViewPosY = 16;
    private float holdedViewPosOffset = .3f;
    private float sizeScale = .6f;

    private Vector3 originSize;

    public void HoldBlock()
    {
        if (currentBlock == null) return;

        if (holdedThisTurn) return;

        originSize = currentBlock.transform.localScale;

        if (holded != null)
        {
            var tmp = currentBlock;
            currentBlock = holded;
            holded = tmp;

            currentBlock.transform.position = new Vector3(Tetris.Width / 2, Tetris.Height);
            currentBlock.transform.localScale = originSize;

            DestroyPreview();
            InitPreview();
            UpdatePreview();
        }
        else
        {
            DestroyPreview();
            holded = currentBlock;
            NextBlock();
        }

        vfx?.PlayClip(SV.ClipHold);

        // reset block state
        moveDelta = MoveDelda.Normal;

        holdedThisTurn = true;
        if (holded is BlockO || holded is BlockI)
        {
            holded.transform.position = new Vector3(holdedViewPosX - holdedViewPosOffset, holdedViewPosY, 0);
        }
        else
        {
            holded.transform.position = new Vector3(holdedViewPosX, holdedViewPosY, 0);
        }
        holded.transform.localScale = new Vector3(sizeScale, sizeScale);
        holded.transform.rotation = Quaternion.Euler(0, 0, 0);
        holded.ResetState();
    }

    public void MoveRight()
    {
        if (currentBlock == null) return;
        if (currentBlock.MoveRight())
        {
            landedWithRotate = false;
            lastWaitTime = 0;
            vfx?.PlayClip(SV.ClipMove);
            UpdatePreview();
        }
    }

    public void MoveLeft()
    {
        if (currentBlock == null) return;
        if (currentBlock.MoveLeft())
        {
            landedWithRotate = false;
            lastWaitTime = 0;
            vfx?.PlayClip(SV.ClipMove);
            UpdatePreview();
        }
    }

    public void ClockwiseRotation()
    {
        if (currentBlock == null) return;
        if (currentBlock.ClockwiseRotation())
        {
            landedWithRotate = true;
            lastWaitTime = 0;
            vfx?.PlayClip(SV.ClipRotate);
            UpdatePreview();
        }
    }

    public void AntiClockwiseRotation()
    {
        if (currentBlock == null) return;
        if (currentBlock.AntiClockwiseRotation())
        {
            landedWithRotate = true;
            lastWaitTime = 0;
            vfx?.PlayClip(SV.ClipRotate);
            UpdatePreview();
        }
    }

    public void SoftDrop()
    {
        if (currentBlock == null) return;
        //landedWithRotate = false;
        moveDelta = MoveDelda.SoftDrop;
    }

    public void NormalDrop()
    {
        if (currentBlock == null) return;
        moveDelta = MoveDelda.Normal;
    }

    public void HardDrop()
    {
        if (currentBlock == null) return;
        landedWithRotate = false;
        moveDelta = MoveDelda.HardDrop;

        //logger.Log("Hard Drop, current height : " + currentBlock.transform.position.y);
    }

    public void Fall(float deltaTime)
    {
        if (gameOver) return;

        time += deltaTime;
        OnTimeChanged?.Invoke(time);

        if (currentBlock == null) return;

        if (!landed)
        {
            if (moveDelta == MoveDelda.HardDrop)
            {
                while (currentBlock.MoveDown()) ;
                landed = true;

                if (vfx)
                {
                    vfx.VFX_HardDrop(currentBlock.transform.position);
                    vfx.PlayClip(SV.ClipHardDrop);
                }
            }
            else
            {
                if (!isLanding)
                {
                    if (lastFallTime >= FallDeltaTime)
                    {
                        isLanding = !currentBlock.MoveDown();
                        lastFallTime = 0;
                    }
                    else
                    {
                        lastFallTime += deltaTime;
                    }
                }
                else
                {
                    if (lastWaitTime >= waitLandTime)
                    {
                        landed = true;
                        lastWaitTime = 0;
                        vfx?.PlayClip(SV.ClipSoftDrop);
                    }
                    else
                    {
                        isLanding = !currentBlock.MoveDown();
                        lastWaitTime += Time.deltaTime;
                    }
                }
            }
        }
        else
        {
            // trigger vfx before check(), because the currentBlock reference has been cleared after check() method.
            vfx?.PlayClip(SV.ClipLanding);

            DestroyPreview();

            AddToGrid();

            if (gameOver) return;

            Check();

            moveDelta = MoveDelda.Normal;
            isLanding = false;
            landed = false;
        }
    }

    #endregion


    #region Check

    private void AddToGrid()
    {
        bool res = true;
        foreach (Transform child in currentBlock.transform)
        {
            var x = EX.Float2Int(child.transform.position.x);
            var y = EX.Float2Int(child.transform.position.y);

            if (y < Height) res = false;

            Grid[x, y] = child;
        }

        // check game over
        if (res)
        {
            OnGameOver?.Invoke();
            gameOver = true;
            currentBlock = null;
        }
    }

    // which line should clear
    private List<int> m_rows = new List<int>(4);

    private void Check()
    {
        // check T-Spin
        if (currentBlock is BlockT blockT)
        {
            isTSpin = blockT.IsTSpin(out isMini) && landedWithRotate;
            Debug.LogFormat("roateted {0}, special {1}", landedWithRotate, isTSpin);
        }

        foreach (Transform child in currentBlock.transform)
        {
            var y = EX.Float2Int(child.transform.position.y);

            if (!m_rows.Contains(y)) m_rows.Add(y);
        }

        // lock block move
        currentBlock = null;

        for (int i = m_rows.Count - 1; i >= 0; i--)
        {
            if (!HasLine(m_rows[i]))
            {
                m_rows.RemoveAt(i);
            }
            else
            {
                ClearLine(m_rows[i]);

                //OnLineClear?.Invoke(m_rows[i]);
                vfx.VFX_LineClear(m_rows[i]);
            }
        }

        if (m_rows.Count > 0)
        {
            m_rows.Sort();

            // update score, etc.
            UpdateData(m_rows.Count);
            ren++;

            if (ren > 0)
            {
                vfx.UpdateTextRen(ren);
            }

            Saro.Timer.Register(.3f, () =>
            {
                for (int i = m_rows.Count - 1; i >= 0; i--)
                {
                    DownLine(m_rows[i]);
                }

                NextBlock();

                m_rows.Clear();
            }).Start();
        }
        else
        {
            NextBlock();
            m_rows.Clear();

            // clear combo data
            ren = -1;
            vfx.HideTextRen();
        }
    }

    private bool HasLine(int row)
    {
        for (int i = 0; i < Width; i++)
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
        for (int i = 0; i < Width; i++)
        {
            if (Grid[i, row] != null)
            {
                GameObject.Destroy(Grid[i, row].gameObject);
                //Grid[i, row].gameObject.SetActive(false);
                Grid[i, row] = null;

                Debug.DrawLine(new Vector2(0, row), new Vector2(10, row), Color.red, 2);
            }
        }
    }

    private void DownLine(int row)
    {
        for (int i = row; i < Height + ExtraHeight; i++)
        {
            for (int j = 0; j < Width; j++)
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
        m_preview = GameObject.Instantiate(currentBlock);
        foreach (Transform child in m_preview.transform)
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
        if (!m_preview) return;
        m_preview.transform.position = currentBlock.transform.position;
        m_preview.transform.rotation = currentBlock.transform.rotation;
        LandingPoint(m_preview);
    }

    private void DestroyPreview()
    {
        if (m_preview != null)
        {
            GameObject.Destroy(m_preview.gameObject);
        }
    }

    private void LandingPoint(Block block)
    {
        while (block.MoveDown()) ;
    }
    #endregion


    private void UpdateData(int count)
    {
        var special = isTSpin || count == 4;

        if (lastClearIsSpecial && special)
        {
            vfx.TextVFX_B2B();
        }

        lastClearIsSpecial = special;

        int points = 0;
        if (isTSpin)
        {
            if (isMini)
            {
                switch (count)
                {
                    case 1:
                        points = 200 * level;
                        vfx.TextVFX_TSpinMiniSingle();
                        break;
                    case 2:
                        points = 1200 * level;
                        vfx.TextVFX_TSpinMiniDouble();
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
                        points = 800 * level;
                        vfx.TextVFX_TSpinSingle();
                        break;
                    case 2:
                        points = 1200 * level;
                        vfx.TextVFX_TSpinDouble();
                        break;
                    case 3:
                        points = 1600 * level;
                        vfx.TextVFX_TSpinTriple();
                        break;
                    default:
                        break;
                }
            }


            vfx.PlayClip(SV.ClipSpecial);
        }
        else
        {
            switch (count)
            {
                case 1:
                    points = 100 * level;
                    vfx.PlayClip(SV.ClipSingle);
                    break;
                case 2:
                    points = 300 * level;
                    vfx.PlayClip(SV.ClipDouble);
                    break;
                case 3:
                    points = 500 * level;
                    vfx.PlayClip(SV.ClipTriple);
                    break;
                case 4:
                    points = 800 * level;
                    vfx.PlayClip(SV.ClipTetris);
                    vfx.TextVFX_Tetris();
                    break;
                default:
                    break;
            }
        }


        if (special) points = (int)(points * 1.5f);
        score += points;

        OnScoreChanged?.Invoke(score);

        if (score / ((level + 1) * (level + 1) * (level + 1)) > 1000)
        {
            level++;
            OnLevelChanged?.Invoke(level);
        }

        line += count;
        OnGoalChanged?.Invoke(line);
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

        if (currentBlock != null)
        {
            currentBlock.DrawGizmos();
        }
    }
#endif
}
