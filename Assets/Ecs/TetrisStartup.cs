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
    sealed class TetrisStartup : MonoBehaviour
    {
        public GameplayAssets gameplayAssets;
        public BatchRenderer batchRenderer;

        EcsWorld _world;
        EcsSystems _systems;

        void Start()
        {
            _world = new EcsWorld();

            var gameCtx = new GameContext(_world);
            gameCtx.batchRenderer = batchRenderer;

            _systems = new EcsSystems(_world);
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
#endif
            _systems
                .Add(new DelaySystem())
                .Add(new BoardCreateSystem())
                .Add(new PieceBagInitSystem())
                .Add(new GameInputSystem())
                .Add(new GameCtrlSystem())
                .Add(new PieceRotateSystem())
                .Add(new PieceMoveSystem())
                .Add(new PieceHoldSystem())
                .Add(new PieceResetDelaySystem())
                .Add(new AddToGridSystem())
                .Add(new LineClearSystem())
                .Add(new PieceNextSystem())
                .Add(new PieceSpawnSystem())
                .Add(new PieceGhostSystem())
                .Add(new TileRendererSystem())
                .Add(new AudioSystem())
                .Add(new EffectSystem())

                  .OneFrame<GameStartRequest>()
                  .OneFrame<GameEndRequest>()
                  //.OneFrame<AddToGridComponent>()
                  .OneFrame<PieceNextRequest>()
                  .OneFrame<PieceSpawnRequest>()
                  .OneFrame<LineClearRequest>()
                  .OneFrame<PieceRotationRequest>()
                  .OneFrame<PieceMoveRequest>()
                  .OneFrame<PieceDropRequest>()
                  .OneFrame<PieceHoldRequest>()
                  .OneFrame<PieceRotationSuccess>()
                  .OneFrame<SEAudioEvent>()
                  .OneFrame<BGMAudioEvent>()
                  .OneFrame<PieceGhostUpdateRequest>()
                  .OneFrame<EffectEvent>()

                 .Inject(gameplayAssets)
                 .Inject(gameCtx)
                 .ProcessInjects()
                .Init();
        }

        void Update()
        {
            _systems?.Run();
        }

        void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
                _world.Destroy();
                _world = null;
            }
        }
    }
}