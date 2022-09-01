using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    public struct PieceMoveRequest : IEcsComponent
    {
        public Vector2 moveDelta;
    }
}