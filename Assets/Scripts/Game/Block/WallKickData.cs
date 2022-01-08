using System.Collections.Generic;
using UnityEngine;

public class WallKickData
{
    // 0 - spawn state
    // 1 - rotate right
    // 2 - 2 successive rotations in either direction form spawn(rotate left/right twice)
    // 3 - rotate left
    public static readonly List<Vector2Int[]> I = new List<Vector2Int[]>(8)
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

    public static readonly List<Vector2Int[]> Other = new List<Vector2Int[]>(8)
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
}
