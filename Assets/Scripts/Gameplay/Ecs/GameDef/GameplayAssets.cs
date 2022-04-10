using UnityEngine;

namespace Tetris
{
    [CreateAssetMenu()]
    public class GameplayAssets : ScriptableObject
    {
        // 这个可以用excel代替
        // 异步模式下，可以提前加载资源，再进游戏

        public GameObject tetrisBoard;
        public LineRenderer linePrefab;
    }
}