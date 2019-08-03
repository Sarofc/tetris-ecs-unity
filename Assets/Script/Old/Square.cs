//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//public enum BlockType
//{
//    T,
//    S,
//    Z,
//    O,
//    I,
//    L,
//    J
//}
//public class Square
//{
//    public Block[] currentBlocks;
//    public int currentBlocksIdx;
//    public Block CurrentBlock { get => currentBlocks[currentBlocksIdx]; }

//    public Vector2Int currentPos = new Vector2Int();

//    public bool getNext;

//    private Dictionary<BlockType, Block[]> dic = new Dictionary<BlockType, Block[]>();

//    private T[] t_block = new T[4]
//    {
//        new T(
//            new int[,]
//            {
//                {0,1,0 },
//                {1,1,1 },
//                {0,0,0 },
//            },
//            new Vector2Int(0,0),
//            new Vector2Int(1,2)
//        ),
//        new T(
//            new int[,]
//            {
//                {0,1,0 },
//                {0,1,1 },
//                {0,1,0 },
//            },
//            new Vector2Int(0,1),
//            new Vector2Int(2,2)
//        ),
//        new T(
//            new int[,]
//            {
//                {0,0,0 },
//                {1,1,1 },
//                {0,1,0 },
//            },
//            new Vector2Int(1,0),
//            new Vector2Int(2,2)
//        ),
//        new T(
//            new int[,]
//            {
//                {0,1,0 },
//                {1,1,0 },
//                {0,1,0 },
//            },
//            new Vector2Int(0,0),
//            new Vector2Int(2,1)
//        )
//    };

//    public Square()
//    {
//        dic.Add(BlockType.T, t_block);
//        getNext = true;
//    }

//    public void ClockwiseRotate()
//    {
//        currentBlocksIdx++;
//        if (currentBlocksIdx >= currentBlocks.Length) currentBlocksIdx = 0;
//    }

//    public void AntiClockwiseRotate()
//    {
//        currentBlocksIdx--;
//        if (currentBlocksIdx < 0) currentBlocksIdx = currentBlocks.Length - 1;
//    }

//    public Vector2Int[] GetCurrentShape()
//    {
//        var res = new Vector2Int[4];
//        int idx = 0;
//        var matrix = currentBlocks[currentBlocksIdx].rot;
//        for (int i = 0; i < matrix.GetLength(0); i++)
//        {
//            for (int j = 0; j < matrix.GetLength(1); j++)
//            {
//                if (matrix[i, j] == 1)
//                {
//                    res[idx].x = currentPos.x + i;
//                    res[idx].y = currentPos.y + j;
//                    idx++;

//                    if (idx == 4) break;
//                }
//            }
//        }

//        return res;
//    }

//    public void MoveDown()
//    {
//        if (getNext = Judge(currentPos.x, currentPos.y - 1))
//            currentPos.y -= 1;
//    }

//    public void MoveLeft()
//    {
//        if (getNext = Judge(currentPos.x - 1, currentPos.y))
//            currentPos.x -= 1;
//    }

//    public void MoveRight()
//    {
//        if (getNext = Judge(currentPos.x + 1, currentPos.y))
//            currentPos.x += 1;
//    }

//    public void GenerateNext(BlockType type)
//    {
//        if (getNext)
//        {
//            foreach (var pos in GetCurrentShape())
//            {
//                Tetris.grid[pos.x, pos.y] = 1;
//            }

//            var removedList = HasLine();

//            for (int i = 0; i < removedList.Count; i++)
//            {
//                for (int j = 0; j < Tetris.Width; j++)
//                {
//                    Tetris.grid[j, removedList[i]] = 0;
//                }
//            }

//            for (int i = 0; i < removedList.Count; i++)
//            {
//                for (int j = 0; j < Tetris.Width; j++)
//                {
//                    Tetris.grid[j, i] = 0;
//                }
//            }

//            GetBlocks(type);
//        }
//        getNext = false;
//    }

//    private List<int> HasLine()
//    {
//        var res = new List<int>(4);
//        for (int i = CurrentBlock.p1.y; i <= CurrentBlock.p2.y; i++)
//        {
//            for (int j = 0; j < Tetris.Width; j++)
//            {
//                if (Tetris.grid[j, i] == 0) break;
//                else res.Add(i);
//            }
//        }
//        return res;
//    }

//    private void GetBlocks(BlockType type)
//    {
//        dic.TryGetValue(type, out currentBlocks);
//        currentBlocksIdx = 0;
//        currentPos.x = Tetris.Width / 2;
//        currentPos.y = Tetris.Height - 3;
//    }

//    private bool Judge(int x, int y)
//    {
//        for (int col = CurrentBlock.p1.y; col <= CurrentBlock.p2.y; col++)
//        {
//            if (y + col < 0)
//            {
//                return false;
//            }

//            for (int row = CurrentBlock.p1.x; row <= CurrentBlock.p2.x; row++)
//            {
//                if (x + row < 0 || x + row > Tetris.Width - 1) return false;

//                if (Tetris.grid[x + row, y + col] != 0) return false;
//            }
//        }

//        return true;
//    }

//    private bool CanRotate()
//    {
//        return true;
//    }

//}
