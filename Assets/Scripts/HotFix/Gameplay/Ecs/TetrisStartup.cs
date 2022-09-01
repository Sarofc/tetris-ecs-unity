#if UNITY_EDITOR
#define ENABLE_DEBUG_ECS
#endif

using Saro.Entities;
using UnityEngine;

namespace Tetris
{
    public sealed class TetrisStartup : MonoBehaviour
    {
        public GameplayAssets gameplayAssets;
        public BatchRenderer batchRenderer;

#if ENABLE_DEBUG_ECS
        private EcsSystems m_EditorSystems;
#endif
        private EcsSystems m_Systems;

        private void Start()
        {
            var world = new EcsWorld("Game");

            var gameCtx = new GameContext(world);
            gameCtx.batchRenderer = batchRenderer;
            gameCtx.gameplayAssets = gameplayAssets;

            m_Systems = new EcsSystems("GameSystems", world, gameCtx);

            m_Systems
                .Add(new DelaySystem())

                // logic
                .Add(new GameTimeSystem())
                .Add(new BoardCreateSystem())
                .Add(new PieceBagInitSystem())
                .Add(new GameInputSystem())
                .Add(new GameStartSystem())
                .Add(new PieceRotateSystem())
                .Add(new PieceMoveSystem())
                .Add(new PieceHoldSystem())
                .Add(new PieceResetDelaySystem())
                .Add(new AddToGridSystem())
                .Add(new LineClearSystem())
                .Add(new PieceNextSystem())
                .Add(new PieceSpawnSystem())
                .Add(new GameEndSystem())

                // 新增，entity的清理函数
                .Add(new EntityDestroyFeature())

                // view
                .Add(new PieceGhostSystem())
                .Add(new TileRendererSystem())
                .Add(new AudioSystem())
                .Add(new EffectSystem());

            m_Systems
                // one frame
                .Del<GameStartRequest>()
                .Del<PieceNextRequest>()
                .Del<PieceSpawnRequest>()
                .Del<LineClearRequest>()
                .Del<PieceRotationRequest>()
                .Del<PieceMoveRequest>()
                .Del<PieceDropRequest>()
                .Del<PieceHoldRequest>()
                .Del<PieceRotationSuccess>()
                .Del<PieceMoveSuccess>()
                .Del<PieceGhostUpdateRequest>()
                .Del<SeAudioEvent>()
                .Del<BGMAudioEvent>()
                .Del<EffectEvent>();

            m_Systems
                .Init();

#if ENABLE_DEBUG_ECS
            m_EditorSystems = new EcsSystems("DebugSystem", world);
            m_EditorSystems
                .Add(new Saro.Entities.UnityEditor.EcsWorldDebugSystem(world))
                .Init();
#endif
        }

        private void Update()
        {
            m_Systems?.Run();
        }

#if ENABLE_DEBUG_ECS
        private void LateUpdate()
        {
            m_EditorSystems?.Run();
        }
#endif

        private void OnDestroy()
        {
            if (m_Systems != null)
            {
                m_Systems.Destroy();
                m_Systems.GetWorld()?.Destroy();
                m_Systems = null;
            }
        }
    }
}