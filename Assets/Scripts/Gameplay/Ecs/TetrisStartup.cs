using Leopotam.Ecs;
using UnityEngine;

namespace Tetris
{
    /*
     * TODO 
     * 1. 本地双人，是两个world，还是一个world？各有什么好处？
     * 2. 本地双人是一个world，怎么处理两个board之间的输入，位置，等等差异
     * 3. 针对第1、2两点，推广到俄罗斯方块99？
     * 
     * 
     */
    internal sealed class TetrisStartup : MonoBehaviour
    {
        public GameplayAssets gameplayAssets;
        public BatchRenderer batchRenderer;
        private EcsWorld m_World;
        private EcsSystems m_Systems;

        private void Start()
        {
            m_World = new EcsWorld();

            var gameCtx = new GameContext(m_World);
            gameCtx.batchRenderer = batchRenderer;

            m_Systems = new EcsSystems(m_World);

#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(m_World);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(m_Systems);
#endif
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

                  .OneFrame<GameStartRequest>()
                  //.OneFrame<GameEndComponent>()
                  //.OneFrame<AddToGridComponent>()
                  .OneFrame<PieceNextRequest>()
                  .OneFrame<PieceSpawnRequest>()
                  .OneFrame<LineClearRequest>()
                  .OneFrame<PieceRotationRequest>()
                  .OneFrame<PieceMoveRequest>()
                  .OneFrame<PieceDropRequest>()
                  .OneFrame<PieceHoldRequest>()
                  .OneFrame<PieceRotationSuccess>()
                  .OneFrame<PieceMoveSuccess>()
                  .OneFrame<PieceGhostUpdateRequest>()
                  .OneFrame<SEAudioEvent>()
                  .OneFrame<BGMAudioEvent>()
                  .OneFrame<EffectEvent>()

                 .Inject(gameplayAssets)
                 .Inject(gameCtx)
                 .ProcessInjects()
                .Init();
        }

        private void Update()
        {
            m_Systems?.Run();
        }

        private void OnDestroy()
        {
            if (m_Systems != null)
            {
                m_Systems.Destroy();
                m_Systems = null;
                m_World.Destroy();
                m_World = null;
            }
        }
    }
}