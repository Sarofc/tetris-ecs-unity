using UnityEngine;
using System.Collections;

namespace Tetris
{


    public class BlockI : Block
    {

        protected override Vector2Int[] GetWallKickData(int next)
        {
            switch (state)
            {
                case 0:
                    if (next == 1) return WallKickData.I[0];
                    if (next == 3) return WallKickData.I[7];
                    break;
                case 1:
                    if (next == 0) return WallKickData.I[1];
                    if (next == 2) return WallKickData.I[2];
                    break;
                case 2:
                    if (next == 1) return WallKickData.I[3];
                    if (next == 3) return WallKickData.I[4];
                    break;
                case 3:
                    if (next == 2) return WallKickData.I[5];
                    if (next == 0) return WallKickData.I[6];
                    break;
                default:
                    break;
            }
            return null;
        }
    }

}