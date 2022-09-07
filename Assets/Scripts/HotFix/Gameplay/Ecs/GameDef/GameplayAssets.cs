using Cysharp.Threading.Tasks;
using Saro.UI;
using System.Collections.Generic;
using Tetris.UI;
using UnityEngine;

namespace Tetris
{
    [CreateAssetMenu]
    public class GameplayAssets : ScriptableObject
    {
        // 这个可以用excel代替
        // 异步模式下，可以提前加载资源，再进游戏

        public GameObject tetrisBoard;
        public LineRenderer linePrefab;

        /// <summary>
        /// 预载游戏资源
        /// </summary>
        /// <returns></returns>
        public async UniTask PreloadAssets()
        {
            var tasks = new List<UniTask>();
            tasks.Add(UIManager.Current.LoadWindowAsync(EGameUI.UIGameHUD));
            tasks.Add(UIManager.Current.LoadWindowAsync(EGameUI.UIInputHUD));

            await UniTask.WhenAll(tasks);
        }
    }
}