using System.Collections.Generic;
using Saro.Entities;
using Saro.Entities.Extension;
using UnityEngine;

namespace Tetris
{
    public class GameContext
    {
        // renderer
        public BatchRenderer batchRenderer;

        // hold
        public bool canHold;
        public bool firstHold; // 防止第一次hold，重置hold状态
        public GameplayAssets gameplayAssets;
        public float gameTime;

        // game state
        public bool gamming;

        // board grid
        public EcsEntity[][] grid;
        public EcsEntity heldPiece;
        public bool lastClearIsSpecial; // tspin 或者 消4
        public bool lastOpIsRotate; // 上一个生效操作为旋转

        // score
        public int level = 1; // 当前等级
        public int line; // 清理行数

        // line clear
        public List<int> lineToClear = new(4);
        public int pieceCount; // 生成了多少个方块

        public int ren = -1; // 连击数
        public int score; // 当前分数

        public EcsWorld world;

        public GameContext(EcsWorld world)
        {
            this.world = world;

            var width = TetrisDef.Width;
            const int k_Height = TetrisDef.Height + TetrisDef.ExtraHeight;
            grid = new EcsEntity[k_Height][];
            for (var i = 0; i < k_Height; i++) grid[i] = new EcsEntity[width];
        }

        public List<Matrix4x4[]> TransfromMatrixBatches { get; set; }
        public List<Vector4[]> ColorBatches { get; set; }
        public List<Vector4[]> SpriteOffsetBatches { get; internal set; }

        public void SendMessage<T>(in T @event) where T : struct, IEcsComponent
        {
            world.SendMessage(@event);
        }

        public void SendMessage<T1, T2>(in T1 evt1, in T2 evt2) where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
        {
            world.SendMessage(evt1, evt2);
        }
    }
}