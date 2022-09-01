using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    public struct EffectEvent : IEcsComponent
    {
        public string effectAsset;
        public Vector3 effectPosition;
    }
}