using Leopotam.Ecs;
using UnityEngine;

namespace Tetris
{
    public struct PieceSpawnRequest
    {
        public EPieceID pieceID;
        public Vector3 spawnPosition;

        public override string ToString()
        {
            return $"{nameof(PieceSpawnRequest)} {pieceID} {spawnPosition}";
        }
    }
}
