using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    // 0 - spawn state
    // 1(R) - rotate right
    // 2 - 2 successive rotations in either direction form spawn(rotate left/right twice)
    // 3(L) - rotate left
    public class NewWallKickData
    {
        private static readonly List<Vector2Int[]> I = new List<Vector2Int[]>(8)
    {
        // 0 >> 1
        new Vector2Int[]
        {
            // (-2,1) : move left 2 grid, move up 1 grid
            new Vector2Int(0,0),
            new Vector2Int(-2,0),
            new Vector2Int(1,0),
            new Vector2Int(-2,-1),
            new Vector2Int(1,2)
        },
        // 1 >> 0
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(2,0),
            new Vector2Int(-1,0),
            new Vector2Int(2,1),
            new Vector2Int(-1,-2)
        },
        // 1 >> 2
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(2,0),
            new Vector2Int(-1,2),
            new Vector2Int(2,-1)
        },
        // 2 >> 1
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(-2,0),
            new Vector2Int(1,-2),
            new Vector2Int(-2,1)
        },
        // 2 >> 3
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(2,0),
            new Vector2Int(-1,0),
            new Vector2Int(2,1),
            new Vector2Int(-1,-2),
        },
        // 3 >> 2
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-2,0),
            new Vector2Int(1,0),
            new Vector2Int(-2,-1),
            new Vector2Int(1,2),
        },
        // 3 >> 0
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(-2,0),
            new Vector2Int(1,-2),
            new Vector2Int(-2,1),
        },
        // 0 >> 3
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(2,0),
            new Vector2Int(-1,2),
            new Vector2Int(2,-1),
        }
    };

        private static readonly List<Vector2Int[]> Generic = new List<Vector2Int[]>(8)
    {
        // 0 >> 1
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1),
            new Vector2Int(0,-2),
            new Vector2Int(-1,-2)
        },
        // 1 >> 0
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,-1),
            new Vector2Int(0,2),
            new Vector2Int(1,2)
        },
        // 1 >> 2
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,-1),
            new Vector2Int(0,2),
            new Vector2Int(1,2)
        },
        // 2 >> 1
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1),
            new Vector2Int(0,-2),
            new Vector2Int(-1,-2)
        },
        // 2 >> 3
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(0,-2),
            new Vector2Int(1,-2)
        },
        // 3 >> 2
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(-1,-1),
            new Vector2Int(0,2),
            new Vector2Int(-1,2)
        },
        // 3 >> 0
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(-1,-1),
            new Vector2Int(0,2),
            new Vector2Int(-1,2)
        },
        // 0 >> 3
        new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(0,-2),
            new Vector2Int(1,-2)
        }
    };

        public static List<Vector2Int[]> GetWallKickData(EPieceID blockID)
        {
            if (blockID == EPieceID.I)
                return I;
            return Generic;
        }
    }
}