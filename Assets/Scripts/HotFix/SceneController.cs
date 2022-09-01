using Saro;
using Saro.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tetris
{
    public sealed class SceneController : IService
    {
        public static SceneController Current => Main.Resolve<SceneController>();

        public enum ESceneType
        {
            Init,
            Title,
            Game,
        }

        public ESceneType CurSceneType { get; private set; } = ESceneType.Init;
        private IAssetHandle m_CurrentSceneHandle;

        // TODO 稳妥起见，这里应该使用queue
        // 不过只要这个api，全部是await完毕再调用，就没问题
        public async UniTask ChangeScene(ESceneType sceneType)
        {
            switch (sceneType)
            {
                case ESceneType.Title:
                    if (CurSceneType == ESceneType.Title) return;
                    CurSceneType = ESceneType.Title;

                    ReleaseHandle();

                    m_CurrentSceneHandle = IAssetManager.Current.LoadSceneAsync("Assets/Res/Scenes/Title.unity");
                    await m_CurrentSceneHandle;
                    break;
                case ESceneType.Game:
                    if (CurSceneType == ESceneType.Game) return;
                    CurSceneType = ESceneType.Game;

                    ReleaseHandle();

                    m_CurrentSceneHandle = IAssetManager.Current.LoadSceneAsync("Assets/Res/Scenes/EcsGaming.unity");
                    await m_CurrentSceneHandle;
                    break;
                default:
                    Debug.LogError($"[SceneController] error: {sceneType}");
                    break;
            }
        }

        private void ReleaseHandle()
        {
            if (m_CurrentSceneHandle != null)
            {
                m_CurrentSceneHandle.DecreaseRefCount();
                m_CurrentSceneHandle = null;
            }
        }

        void IService.Awake()
        {

        }

        void IService.Dispose()
        {

        }

        void IService.Update()
        {

        }
    }
}
