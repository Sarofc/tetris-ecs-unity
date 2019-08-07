
using UnityEngine;

public class BlockT : Block
{

    // 1 0 1
    // 0 0 0
    // 1 0 1
    private Vector2Int[] GetPoints()
    {
        return new Vector2Int[4]
        {
            new Vector2Int(EX.Float2Int(transform.position.x + 1), EX.Float2Int(transform.position.y + 1)),
            new Vector2Int(EX.Float2Int(transform.position.x + 1), EX.Float2Int(transform.position.y - 1)),
            new Vector2Int(EX.Float2Int(transform.position.x - 1), EX.Float2Int(transform.position.y + 1)),
            new Vector2Int(EX.Float2Int(transform.position.x - 1), EX.Float2Int(transform.position.y - 1)),
        };
    }

    public bool IsTSpin(out bool isMini)
    {
        var points = GetPoints();

        int c1 = 0; // tspin
        int c2 = 0; // mini

        for (int i = 0; i < points.Length; i++)
        {
            // out of range check
            if (points[i].x >= Tetris.Width || points[i].x < 0 ||
                points[i].y >= Tetris.Height + Tetris.ExtraHeight || points[i].y < 0)
            {
                c1++;
                c2++;
            }

            else if (Tetris.Grid[points[i].x, points[i].y] != null)
            {
                c1++;
            }
        }

        isMini = c2 >= 2 ? true : false;

        if (c1 < 3) return false;
        return true;
    }

#if UNITY_EDITOR
    public override void DrawGizmos()
    {
        base.DrawGizmos();

        // draw block T corners
        Gizmos.color = Color.green;
        var points = GetPoints();
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawCube((Vector3Int)points[i], new Vector3(.6f, .6f));
        }
    }
#endif
}