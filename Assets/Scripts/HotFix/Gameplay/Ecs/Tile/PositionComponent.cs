using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    public struct PositionComponent : IEcsComponent
    {
        public Vector3 position;

        public override string ToString()
        {
            return $"{nameof(PositionComponent)} {position}";
        }
    }
}