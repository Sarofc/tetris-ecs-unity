
using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class GameContext
    {
        // board grid
        public EcsEntity[][] grid;

        // game state
        public bool gamming;
        public float gameTime;

        // hold system
        public EcsEntity heldPiece;
        public EcsEntity lastHeldPiece;

        // line clear
        public List<int> lineToClear = new List<int>(4);

        // score
        public int level = 1; // 当前等级
        public int score; // 当前分数
        public int line; // 清理行数

        public int ren = -1; // 连击数
        public bool lastClearIsSpecial; // tspin 或者 消4
        public bool lastOpIsRotate; // 上一个生效操作为旋转

        // renderer
        public BatchRenderer batchRenderer;
        public List<Matrix4x4[]> TransfromMatrixBatches { get; set; }
        public List<Vector4[]> ColorBatches { get; set; }
        public List<Vector4[]> SpriteOffsetBatches { get; internal set; }

        public EcsWorld world;

        public GameContext(EcsWorld world)
        {
            this.world = world;

            var width = TetrisDef.k_Width;
            const int height = TetrisDef.k_Height + TetrisDef.k_ExtraHeight;
            grid = new EcsEntity[height][];
            for (int i = 0; i < height; i++)
            {
                grid[i] = new EcsEntity[width];
            }
        }

        public void SendMessage<T>(in T @event) where T : struct
        {
            world.SendMessage(@event);
        }

        public void SendMessage<T1, T2>(in T1 evt1, in T2 evt2) where T1 : struct where T2 : struct
        {
            world.SendMessage(evt1, evt2);
        }
    }
}
