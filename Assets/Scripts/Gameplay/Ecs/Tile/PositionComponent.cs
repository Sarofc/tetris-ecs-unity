
using UnityEngine;

namespace Tetris
{
    public struct PositionComponent
    {
        public Vector3 position;

        public override string ToString()
        {
            return $"{nameof(PositionComponent)} {position}";
        }
    }
}
