using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    public struct TileRendererComponent : IEcsComponent
    {
        public Matrix4x4 matrix;
        public Vector4 color;
    }
}