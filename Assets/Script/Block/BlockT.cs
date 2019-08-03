
using UnityEngine;

public class BlockT : Block
{

    // 1 0 1
    // 0 0 0
    // 1 0 1
    public Vector2[] GetPoints()
    {
        return new Vector2[4]
        {
            new Vector2(rotatePoint.x + 1, rotatePoint.y +1),
            new Vector2(rotatePoint.x + 1, rotatePoint.y -1),
            new Vector2(rotatePoint.x - 1, rotatePoint.y +1),
            new Vector2(rotatePoint.x - 1, rotatePoint.y -1),
        };
    }
}