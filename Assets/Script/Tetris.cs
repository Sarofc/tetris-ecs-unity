using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris
{
    public const int Height = 22;
    public const int Width = 10;
    public const float DeltaNormal = 1f;
    public const float DeltaSoft = .1f;

    public static Transform[,] Grid;

    public int level = 1;
    public int score = 0;
    public int goal = 40;
    public float time;

    public static Action<int> OnScoreChanged;
    public static Action<int> OnLevelChanged;
    public static Action<int> OnGoalChanged;
    public static Action<float> OnTimeChanged;
    public static Action OnGameOver;

    private int clearLintCount = 0;
    //private int combo = 0;

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

    private float waitLandTime = .5f;
    private float lastWaitTime;
    private bool isLanding = false;

    private MoveDelda moveDelta = MoveDelda.Normal;

    private Block currentBlock;
    private Block holded;


    private BlockSpawner m_spawner;

    private Block m_preview;
    private bool showPreview;

    enum MoveDelda
    {
        Normal,
        SoftDrop,
        HardDrop
    }

    public Tetris(BlockSpawner spawner)
    {
        Grid = new Transform[Width, Height];
        m_spawner = spawner;

        m_spawner.InitNextChainSlot();
    }


    public void NextBlock()
    {
        currentBlock = m_spawner.NextBlock();

        foreach (Transform child in currentBlock.transform)
        {
            var x = EX.Float2Int(child.transform.position.x);
            var y = EX.Float2Int(child.transform.position.y);
            if (y >= Height) continue;
            if (Grid[x, y] != null)
            {
                OnGameOver?.Invoke();
                currentBlock = null;
                return;
            }
        }

        InitPreview();

        UpdatePreview();

        m_spawner.UpdateNextChainSlot();
    }


    #region Block Control
    public void HoldBlock()
    {
        if (holded != null)
        {
            var tmp = currentBlock;
            currentBlock = holded;
            holded = tmp;

            currentBlock.transform.position = new Vector3(5, 21);

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

        moveDelta = MoveDelda.Normal;
        holded.transform.position = new Vector3(-4, 18, 0);
    }

    public void MoveRight()
    {
        if (currentBlock.MoveRight())
            UpdatePreview();
    }

    public void MoveLeft()
    {
        if (currentBlock.MoveLeft())
            UpdatePreview();
    }

    public void ClockwiseRotation()
    {
        if (currentBlock.ClockwiseRotation())
        {
            lastWaitTime = 0;
            UpdatePreview();
        }
    }

    public void AntiClockwiseRotation()
    {
        if (currentBlock.AntiClockwiseRotation())
        {
            lastWaitTime = 0;
            UpdatePreview();
        }
    }

    public void Fall(float deltaTime)
    {
        time += deltaTime;
        OnTimeChanged?.Invoke(time);

        if (currentBlock == null) return;

        if (!isLanding)
        {
            if (moveDelta == MoveDelda.HardDrop)
            {
                LandingPoint(currentBlock);
                isLanding = true;
            }
            else
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
        }
        else
        {
            if (moveDelta == MoveDelda.HardDrop)
            {
                DestroyPreview();

                AddToGrid();

                Check();

            }
            else
            {
                if (lastWaitTime >= waitLandTime)
                {
                    DestroyPreview();

                    AddToGrid();

                    Check();

                    lastWaitTime = 0;
                }
                else
                {
                    isLanding = !currentBlock.MoveDown();
                    lastWaitTime += Time.deltaTime;
                }
            }
        }
    }

    public void SoftDrop()
    {
        moveDelta = MoveDelda.SoftDrop;
    }

    public void HardDrop()
    {
        moveDelta = MoveDelda.HardDrop;
    }

    #endregion


    private void AddToGrid()
    {
        foreach (Transform child in currentBlock.transform)
        {
            Grid[EX.Float2Int(child.transform.position.x), EX.Float2Int(child.transform.position.y)] = child;
        }
    }

    // which line should clear
    private List<int> m_rows = new List<int>(4);
    private bool isTSpin = false;
    private bool hasLineToClear = false;

    private void Check()
    {
        foreach (Transform child in currentBlock.transform)
        {
            var y = EX.Float2Int(child.transform.position.y);

            if (!m_rows.Contains(y)) m_rows.Add(y);
        }

        m_rows.Sort();

        for (int i = m_rows[m_rows.Count - 1]; i >= m_rows[0]; i--)
        {
            if (HasLine(i))
            {
                if (!hasLineToClear)
                {
                    //isTSpin = CheckTSpin();
                    hasLineToClear = true;
                }

                clearLintCount++;

                ClearLine(i);
                DownLine(i);
            }
        }


        NextBlock();

        moveDelta = MoveDelda.Normal;
        isLanding = false;

        UpdateData(clearLintCount);
        clearLintCount = 0;

        m_rows.Clear();
        hasLineToClear = false;
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

                Debug.DrawLine(new Vector2(0, row), new Vector2(10, row), Color.blue, 2);
            }
        }
    }

    private void DownLine(int row)
    {
        for (int i = row; i < Height; i++)
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

    #region Block Preview 
    private void InitPreview()
    {
        m_preview = GameObject.Instantiate(currentBlock);
        foreach (Transform child in m_preview.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr) sr.color = new Color(1, 1, 1, .2f);
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


    private bool CheckTSpin()
    {
        //if (currentBlock is BlockT t)
        //{
        //    var points = t.GetPoints();
        //    int x, y;
        //    int count = 0;
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        x = EX.Float2Int(points[i].x);
        //        y = EX.Float2Int(points[i].y);

        //        if (Valid(x, y))
        //        {
        //            if (Grid[x, y] != null) count++;
        //        }

        //        if (count == 3)
        //        {
        //            return true;
        //        }
        //    }
        //}

        return false;
    }

    private void UpdateData(int count)
    {
        switch (count)
        {
            case 0:
                break;
            case 1:
                score += 100 * level;
                break;
            case 2:
                score += 300 * level;
                break;
            case 3:
                score += 500 * level;
                break;
            case 4:
                score += 800 * level;
                break;
            default:
                break;
        }

        OnScoreChanged?.Invoke(score);

        if (score / ((level + 1) * (level + 1) * (level + 1)) > 1000)
        {
            level++;
            OnLevelChanged?.Invoke(level);
        }

        goal -= count;
        OnGoalChanged?.Invoke(goal);
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
