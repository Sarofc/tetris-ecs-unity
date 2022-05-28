using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Extension;
using UnityEngine;

namespace Tetris
{
    public static class TetrisUtil
    {
        private static readonly float s_StartPosX = 11.3f;
        private static readonly float s_StartPosY = 16.5f;
        private static readonly float s_LeftOffset = -.25f;
        private static readonly int s_Distance = 2;
        private static float s_SizeScale = .5f;

        public static void UpdateNextChainSlot(EcsWorld world, List<EcsPackedEntity> queue, int count = 5)
        {
            if (count > queue.Count) throw new ArgumentOutOfRangeException();

            var i = 0;
            for (; i < count; i++)
            {
                var ePiece = queue[i];
                ref var cPiece = ref ePiece.Get<PieceComponent>(world);
                ref var cPos = ref ePiece.Get<PositionComponent>(world);
                var pieceID = cPiece.pieceID;

                if (pieceID == EPieceID.O || pieceID == EPieceID.I)
                    cPos.position = new Vector3(s_StartPosX + s_LeftOffset, s_StartPosY - s_Distance * i);
                else
                    cPos.position = new Vector3(s_StartPosX, s_StartPosY - s_Distance * i);

                ChangePieceColor(world, ref ePiece, GetTileColor(pieceID));
            }

            for (; i < queue.Count; i++)
            {
                var ePiece = queue[i];

                var tileList = ePiece.Get<ComponentList<EcsPackedEntity>>(world).Value;
                for (var k = 0; k < tileList.Count; k++)
                {
                    var tile = tileList[k];
                    tile.Del<TileRendererComponent>(world);
                }
            }
        }

        public static void ChangePieceColor(EcsWorld world, ref EcsPackedEntity ePiece, Color color)
        {
            var tileList = ePiece.Add<ComponentList<EcsPackedEntity>>(world).Value;
            for (var k = 0; k < tileList.Count; k++)
            {
                var tile = tileList[k];
                tile.Add<TileRendererComponent>(world).color = color;
                ;
            }
        }

        public static (bool isTSpin, bool isMini) IsTSpin(EcsWorld world, EcsPackedEntity[][] grid,
            in EcsPackedEntity ePiece)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>(world);
            if (cPiece.pieceID != EPieceID.T) return (false, false);

            var rootPos = ePiece.Get<PositionComponent>(world).position;

            Span<Vector2Int> points = stackalloc Vector2Int[4];
            points[0] = Vector2Int.RoundToInt(rootPos + new Vector3(1, 1));
            points[1] = Vector2Int.RoundToInt(rootPos + new Vector3(1, -1));
            points[2] = Vector2Int.RoundToInt(rootPos + new Vector3(-1, 1));
            points[3] = Vector2Int.RoundToInt(rootPos + new Vector3(-1, -1));

            var c1 = 0; // tspin
            var c2 = 0; // mini

            for (var i = 0; i < points.Length; i++)
                // out of range check
                if (points[i].x >= TetrisDef.Width || points[i].x < 0 ||
                    points[i].y >= TetrisDef.Height + TetrisDef.ExtraHeight || points[i].y < 0)
                {
                    c1++;
                    c2++;
                }
                else if (!grid[points[i].y][points[i].x].IsNull())
                {
                    c1++;
                }

            var isSpin = c1 >= 3;
            return (isSpin, isSpin && c2 >= 2);
        }

        public static bool MovePiece(EcsWorld world, EcsPackedEntity[][] grid, in EcsPackedEntity ePiece,
            in Vector2 moveDelta)
        {
            ref var cPiecePos = ref ePiece.Get<PositionComponent>(world);

            ref var x = ref cPiecePos.position.x;
            ref var y = ref cPiecePos.position.y;
            x += moveDelta.x;
            y += moveDelta.y;

            if (!IsValidBlock(world, grid, ePiece))
            {
                x -= moveDelta.x;
                y -= moveDelta.y;

                return false;
            }

            return true;
        }

        public static void RotateBlockWithoutCheck(EcsWorld world, in EcsPackedEntity ePiece, bool clockwise)
        {
            var tileList = ePiece.Get<ComponentList<EcsPackedEntity>>(world).Value;
            ref var cPiece = ref ePiece.Get<PieceComponent>(world);
            ref var cPiecePos = ref ePiece.Get<PositionComponent>(world);

            var angle = clockwise ? -90 : 90;

            var root = cPiecePos.position;
            var pivot = cPiece.pivot + root;
            for (var k = 0; k < tileList.Count; k++)
            {
                var eTile = tileList[k];
                ref var cPos = ref eTile.Get<PositionComponent>(world);

                var tilePos = cPos.position + root;
                var pos = RotateAt(tilePos, pivot, angle);
                cPos.position = pos - root;
            }
        }

        public static Vector3 RotateAt(in Vector3 point, in Vector3 pivot, int angle)
        {
            var rot = Quaternion.Euler(0f, 0f, angle);
            var dir = point - pivot;
            var newDir = rot * dir;
            return newDir + pivot;
        }

        public static void ResetPieceRotation(EcsWorld world, ref EcsPackedEntity ePiece)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>(world);
            if (cPiece.state == 2)
            {
                RotateBlockWithoutCheck(world, ePiece, true);
                RotateBlockWithoutCheck(world, ePiece, true);
                cPiece.state = 0;
            }
            else if (cPiece.state == 3)
            {
                RotateBlockWithoutCheck(world, ePiece, true);
                cPiece.state = 0;
            }
            else if (cPiece.state == 1)
            {
                RotateBlockWithoutCheck(world, ePiece, false);
                cPiece.state = 0;
            }
        }

        public static EcsPackedEntity CreatePiece(EcsWorld world, EPieceID pieceID, Vector3 spawnPosition)
        {
            var ePiece = world.PackEntity(world.NewEntity());
            ref var cPiece = ref ePiece.Add<PieceComponent>(world);
            cPiece.pieceID = pieceID;
            cPiece.scale = 1f;
            ePiece.Add<PieceMoveComponent>(world);
            ePiece.Add<PositionComponent>(world).position = spawnPosition;

            var tileList = ePiece.Add<ComponentList<EcsPackedEntity>>(world).Value;

            CreateTileList(world, ePiece, tileList);
            if (pieceID != EPieceID.O)
                ePiece.Add<PieceRotateFlag>(world);

            return ePiece;
        }

        public static EcsPackedEntity CreatePieceForBagView(EcsWorld world, EPieceID pieceID, Vector3 spawnPosition)
        {
            var ePiece = world.PackEntity(world.NewEntity());
            ref var cPiece = ref ePiece.Add<PieceComponent>(world);
            cPiece.pieceID = pieceID;
            cPiece.scale = 0.6f;
            ePiece.Add<PositionComponent>(world).position = spawnPosition;

            var tileList = ePiece.Add<ComponentList<EcsPackedEntity>>(world).Value;

            CreateTileList(world, ePiece, tileList);

            return ePiece;
        }

        public static EcsPackedEntity CreatePieceForGhost(EcsWorld world, EPieceID pieceID, Vector3 spawnPosition)
        {
            var ePiece = world.PackEntity(world.NewEntity());
            ref var cPiece = ref ePiece.Add<PieceComponent>(world);
            cPiece.pieceID = pieceID;
            cPiece.scale = 1f;
            ePiece.Add<PositionComponent>(world).position = spawnPosition;
            ePiece.Add<PieceGhostComponent>(world);

            var tileList = ePiece.Add<ComponentList<EcsPackedEntity>>(world).Value;
            CreateTileList(world, ePiece, tileList);

            for (var i = 0; i < tileList.Count; i++)
            {
                var tile = tileList[i];
                ref var tileRenderer = ref tile.Add<TileRendererComponent>(world);
                tileRenderer.color = new Color(1, 1, 1, 0.5f);
            }

            return ePiece;
        }

        public static Color GetTileColor(EPieceID pieceID)
        {
            switch (pieceID)
            {
                case EPieceID.J:
                    return Color.blue;
                case EPieceID.I:
                    return Color.cyan;
                case EPieceID.L:
                    return new Color(1f, 0.65f, 0.4f);
                case EPieceID.O:
                    return Color.yellow;
                case EPieceID.S:
                    return Color.green;
                case EPieceID.T:
                    return Color.magenta;
                case EPieceID.Z:
                    return Color.red;
            }

            return Color.white;
        }

        private static void CreateTileList(EcsWorld world, in EcsPackedEntity ePiece, List<EcsPackedEntity> tileList)
        {
            ref var cPiece = ref ePiece.Add<PieceComponent>(world);
            var pieceID = cPiece.pieceID;

            var color = GetTileColor(pieceID);
            switch (pieceID)
            {
                case EPieceID.J:
                {
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 0, 0), color));
                }
                    break;
                case EPieceID.I:
                {
                    cPiece.pivot = new Vector3(0.5f, -0.5f, 0);

                    tileList.Add(CreateTile(world, ePiece, new Vector3(2, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                }
                    break;
                case EPieceID.L:
                {
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 0, 0), color));
                }
                    break;
                case EPieceID.O:
                {
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 1, 0), color));
                }
                    break;
                case EPieceID.S:
                {
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 1, 0), color));
                }
                    break;
                case EPieceID.T:
                {
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                }
                    break;
                case EPieceID.Z:
                {
                    tileList.Add(CreateTile(world, ePiece, new Vector3(-1, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 1, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(0, 0, 0), color));
                    tileList.Add(CreateTile(world, ePiece, new Vector3(1, 0, 0), color));
                }
                    break;
            }
        }

        public static EcsPackedEntity CreateTile(EcsWorld world, in EcsPackedEntity parent, Vector3 pos, Color color)
        {
            var eTile = world.PackEntity(world.NewEntity());
            eTile.Add<PositionComponent>(world).position = pos;
            eTile.Add<TileRendererComponent>(world).color = color;
            eTile.Add<ParentComponent>(world).parent = parent;
            return eTile;
        }

        private static Vector2Int[] GetWallKickData(EPieceID blockID, int state, int next)
        {
            var datas = NewWallKickData.GetWallKickData(blockID);

            switch (state)
            {
                case 0:
                    if (next == 1) return datas[0];
                    if (next == 3) return datas[7];
                    break;
                case 1:
                    if (next == 0) return datas[1];
                    if (next == 2) return datas[2];
                    break;
                case 2:
                    if (next == 1) return datas[3];
                    if (next == 3) return datas[4];
                    break;
                case 3:
                    if (next == 2) return datas[5];
                    if (next == 0) return datas[6];
                    break;
            }

            return null;
        }

        public static bool WallKickTest(EcsWorld world, EcsPackedEntity[][] grid, in EcsPackedEntity ePiece, int next,
            out Vector2Int result)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>(world);
            var data = GetWallKickData(cPiece.pieceID, cPiece.state, next);

            for (var i = 0; i < data.Length; i++)
                if (!IsValidBlock(world, grid, ePiece, data[i]))
                {
                }
                else
                {
                    result = data[i];
                    return true;
                }

            result = Vector2Int.zero;
            return false;
        }

        public static bool IsValidBlock(EcsWorld world, EcsPackedEntity[][] grid, in EcsPackedEntity ePiece)
        {
            var cPiecePos = ePiece.Get<PositionComponent>(world);
            var tileList = ePiece.Get<ComponentList<EcsPackedEntity>>(world).Value;

            var rootPos = cPiecePos.position;
            for (var i = 0; i < tileList.Count; i++)
            {
                ref var tilePos = ref tileList[i].Get<PositionComponent>(world);
                var childPos = tilePos.position + rootPos;
                var x = Mathf.RoundToInt(childPos.x);
                var y = Mathf.RoundToInt(childPos.y);
                if (!IsValid(world, grid, x, y)) return false;
            }
            return true;
        }

        public static bool IsValidBlock(EcsWorld world, EcsPackedEntity[][] grid, in EcsPackedEntity ePiece,
            Vector2 move)
        {
            var cPiecePos = ePiece.Get<PositionComponent>(world);
            var tileList = ePiece.Get<ComponentList<EcsPackedEntity>>(world).Value;

            var rootPos = cPiecePos.position;
            for (var i = 0; i < tileList.Count; i++)
            {
                ref var tilePos = ref tileList[i].Get<PositionComponent>(world);
                var childPos = tilePos.position + rootPos;
                var x = Mathf.RoundToInt(childPos.x + move.x);
                var y = Mathf.RoundToInt(childPos.y + move.y);
                if (!IsValid(world, grid, x, y)) return false;
            }
            return true;
        }

        public static bool IsValid(EcsWorld world, EcsPackedEntity[][] grid, int x, int y)
        {
            if (x >= TetrisDef.Width || x < 0 || y >= TetrisDef.Height + TetrisDef.ExtraHeight || y < 0)
                return false;
            if (grid[y][x].IsAlive(world)) return false;

            return true;
        }
    }
}