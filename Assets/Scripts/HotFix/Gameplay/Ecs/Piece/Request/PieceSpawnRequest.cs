using Leopotam.EcsLite;
using UnityEngine;

namespace Tetris
{
    public struct PieceSpawnRequest : IEcsComponent
    {
        public EPieceID pieceID;
        public Vector3 spawnPosition;

        public override string ToString()
        {
            return $"{nameof(PieceSpawnRequest)} {pieceID} {spawnPosition}";
        }
    }
}