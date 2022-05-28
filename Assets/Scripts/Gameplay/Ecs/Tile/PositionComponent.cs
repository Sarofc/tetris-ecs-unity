using Leopotam.EcsLite;
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