#if UNITY_EDITOR
#define ENABLE_DEBUG_SYSTEM
#endif

using Leopotam.EcsLite;
using UnityEngine;

namespace Tetris
{
    /*
     * TODO 
     * 1. ����˫�ˣ�������world������һ��world������ʲô�ô���
     * 2. ����˫����һ��world����ô��������board֮������룬λ�ã��ȵȲ���
     * 3. ��Ե�1��2���㣬�ƹ㵽����˹����99��
     * 
     * 
     */
    internal sealed class TetrisStartup : MonoBehaviour
    {
        public GameplayAssets gameplayAssets;
        public BatchRenderer batchRenderer;

#if ENABLE_DEBUG_SYSTEM
        private EcsSystems m_EditorSystems;
#endif
        private EcsSystems m_Systems;

        private void Start()
        {
            var world = new EcsWorld();

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

                // view
                .Add(new PieceGhostSystem())
                .Add(new TileRendererSystem())
                .Add(new AudioSystem())
                .Add(new EffectSystem())

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
                .Del<EffectEvent>()
                .Init();

#if ENABLE_DEBUG_SYSTEM
            m_EditorSystems = new EcsSystems("DebugSystem", world);
            m_EditorSystems
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(world))
                .Init();
#endif
        }

        private void Update()
        {
            m_Systems?.Run();
        }

        private void LateUpdate()
        {
#if ENABLE_DEBUG_SYSTEM
            m_EditorSystems?.Run();
#endif
        }

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