
using UnityEngine;

namespace Tetris
{
    public enum EPieceID : short
    {
        J = 0,
        I,
        L,
        O,
        S,
        T,
        Z
    }

    public struct PieceComponent
    {
        public EPieceID pieceID;

        public Vector3 pivot;

        public float scale;

        /// <summary>
        ///      0
        ///   3     1
        ///      2
        /// 0 - spawn state
        /// 1 - rotate right
        /// 2 - 2 successive rotations in either direction form spawn
        /// 3 - rotate left
        /// </summary>
        public int state;
    }
}
