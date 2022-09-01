using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    // 0 - spawn state
    // 1(R) - rotate right
    // 2 - 2 successive rotations in either direction form spawn(rotate left/right twice)
    // 3(L) - rotate left
    public class WallKickData
    {
        private static readonly List<Vector2Int[]> s_ = new(8)
        {
            // 0 >> 1
            new Vector2Int[]
            {
                // (-2,1) : move left 2 grid, move up 1 grid
                new(0, 0),
                new(-2, 0),
                new(1, 0),
                new(-2, -1),
                new(1, 2)
            },
            // 1 >> 0
            new Vector2Int[]
            {
                new(0, 0),
                new(2, 0),
                new(-1, 0),
                new(2, 1),
                new(-1, -2)
            },
            // 1 >> 2
            new Vector2Int[]
            {
                new(0, 0),
                new(-1, 0),
                new(2, 0),
                new(-1, 2),
                new(2, -1)
            },
            // 2 >> 1
            new Vector2Int[]
            {
                new(0, 0),
                new(1, 0),
                new(-2, 0),
                new(1, -2),
                new(-2, 1)
            },
            // 2 >> 3
            new Vector2Int[]
            {
                new(0, 0),
                new(2, 0),
                new(-1, 0),
                new(2, 1),
                new(-1, -2)
            },
            // 3 >> 2
            new Vector2Int[]
            {
                new(0, 0),
                new(-2, 0),
                new(1, 0),
                new(-2, -1),
                new(1, 2)
            },
            // 3 >> 0
            new Vector2Int[]
            {
                new(0, 0),
                new(1, 0),
                new(-2, 0),
                new(1, -2),
                new(-2, 1)
            },
            // 0 >> 3
            new Vector2Int[]
            {
                new(0, 0),
                new(-1, 0),
                new(2, 0),
                new(-1, 2),
                new(2, -1)
            }
        };

        private static readonly List<Vector2Int[]> s_Generic = new(8)
        {
            // 0 >> 1
            new Vector2Int[]
            {
                new(0, 0),
                new(-1, 0),
                new(-1, 1),
                new(0, -2),
                new(-1, -2)
            },
            // 1 >> 0
            new Vector2Int[]
            {
                new(0, 0),
                new(1, 0),
                new(1, -1),
                new(0, 2),
                new(1, 2)
            },
            // 1 >> 2
            new Vector2Int[]
            {
                new(0, 0),
                new(1, 0),
                new(1, -1),
                new(0, 2),
                new(1, 2)
            },
            // 2 >> 1
            new Vector2Int[]
            {
                new(0, 0),
                new(-1, 0),
                new(-1, 1),
                new(0, -2),
                new(-1, -2)
            },
            // 2 >> 3
            new Vector2Int[]
            {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, -2),
                new(1, -2)
            },
            // 3 >> 2
            new Vector2Int[]
            {
                new(0, 0),
                new(-1, 0),
                new(-1, -1),
                new(0, 2),
                new(-1, 2)
            },
            // 3 >> 0
            new Vector2Int[]
            {
                new(0, 0),
                new(-1, 0),
                new(-1, -1),
                new(0, 2),
                new(-1, 2)
            },
            // 0 >> 3
            new Vector2Int[]
            {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, -2),
                new(1, -2)
            }
        };

        public static List<Vector2Int[]> GetWallKickData(EPieceID blockID)
        {
            if (blockID == EPieceID.I)
                return s_;
            return s_Generic;
        }
    }
}