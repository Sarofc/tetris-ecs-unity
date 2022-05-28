using Leopotam.EcsLite;
using UnityEngine;

namespace Tetris
{
    public struct EffectEvent : IEcsComponent
    {
        public string effectAsset;
        public Vector3 effectPosition;
    }
}