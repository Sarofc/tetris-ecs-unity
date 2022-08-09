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
    public sealed class TetrisStartup : MonoBehaviour
    {
        public GameplayAssets gameplayAssets;
        public BatchRenderer batchRenderer;

#if ENABLE_DEBUG_SYSTEM
        private EcsSystems m_EditorSystems;
#endif
        private EcsSystems m_Systems;

        private int[] _int = new int[1] { 1 };
        private ref int GetInt()
        {
            return ref _int[0];
        }

        private void Start()
        {
            Debug.LogError("Start 1");

            var world = new EcsWorld();

            {
                Debug.LogError("Start 1.1");

                int a = 10;
                ref var pA = ref a;
                pA = 11;
                Debug.LogError("Start 1.2");

                var ent = world.NewEntity();
                Debug.LogError("Start 1.3 entity: " + ent);

                ref var intVar = ref GetInt();
                Debug.LogError("Start 1.3.1");

                var pool = world.GetPool<DelayComponent>();
                Debug.LogError("Start 1.4 : " + "cehck pool null: " + pool.ToString());

                var array = pool.GetRawDenseItems();
                var result = pool.Has(ent);
                Debug.LogError("Start 1.4.1 : new " + array.Length + $" {result}");

                ref var msg = ref pool.Add(ent); // TODO crash
                Debug.LogError("Start 1.5");

                msg = new DelayComponent { delay = 1f };
                Debug.LogError("Start 1.6");
            }

            Debug.LogError("Start 2");
            var gameCtx = new GameContext(world);
            gameCtx.batchRenderer = batchRenderer;
            gameCtx.gameplayAssets = gameplayAssets;

            Debug.LogError("Start 3");

            m_Systems = new EcsSystems("GameSystems", world, gameCtx);

            Debug.LogError("Start 4");

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
                .Add(new EffectSystem());

            Debug.LogError("Start 4.1");

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

            Debug.LogError("Start 4.2");

            m_Systems
                .Init(); // TODO crash

            Debug.LogError("Start 5");

#if ENABLE_DEBUG_SYSTEM

            Debug.LogError("Start m_EditorSystems");

            m_EditorSystems = new EcsSystems("DebugSystem", world);
            m_EditorSystems
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(world))
                .Init();
#endif

            Debug.LogError("Start 6");
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