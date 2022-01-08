using UnityEngine;
using System.Collections;

namespace Tetris
{


    public class BlockO : Block
    {
        public override bool AntiClockwiseRotation()
        {
            return false;
        }

        public override bool ClockwiseRotation()
        {
            return false;
        }
    }

}