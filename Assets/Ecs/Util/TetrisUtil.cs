using Leopotam.Ecs;
using Leopotam.Ecs.Extension;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public static class TetrisUtil
    {
        private static float startPosX = 11.3f;
        private static float startPosY = 16.5f;
        private static float leftOffset = -.25f;
        private static int distance = 2;
        private static float sizeScale = .5f;

        public static void UpdateNextChainSlot(List<EcsEntity> queue, int count = 5)
        {
            if (count > queue.Count) throw new System.ArgumentOutOfRangeException();

            int i = 0;
            for (; i < count; i++)
            {
                var ePiece = queue[i];
                ref var cPiece = ref ePiece.Get<PieceComponent>();
                ref var cPos = ref ePiece.Get<PositionComponent>();
                var pieceID = cPiece.pieceID;

                if (pieceID == EPieceID.O || pieceID == EPieceID.I)
                {
                    cPos.position = new Vector3(startPosX + leftOffset, startPosY - distance * i);
                }
                else
                {
                    cPos.position = new Vector3(startPosX, startPosY - distance * i);
                }

                var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;
                for (int k = 0; k < tileList.Count; k++)
                {
                    var tile = tileList[k];
                    tile.Get<TileRendererComponent>().color = TetrisUtil.GetTileColor(pieceID);
                }
            }

            for (; i < queue.Count; i++)
            {
                var ePiece = queue[i];

                var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;
                for (int k = 0; k < tileList.Count; k++)
                {
                    var tile = tileList[k];
                    tile.Del<TileRendererComponent>();
                }
            }
        }

        public static (bool isTSpin, bool isMini) IsTSpin(in EcsEntity ePiece)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>();
            if (cPiece.pieceID != EPieceID.T) return (false, false);

            var rootPos = ePiece.Get<PositionComponent>().position;

            Span<Vector2Int> points = stackalloc Vector2Int[4];
            points[0] = Vector2Int.RoundToInt(rootPos + new Vector3(1, 1));
            points[1] = Vector2Int.RoundToInt(rootPos + new Vector3(1, -1));
            points[2] = Vector2Int.RoundToInt(rootPos + new Vector3(-1, 1));
            points[3] = Vector2Int.RoundToInt(rootPos + new Vector3(-1, -1));

            int c1 = 0; // tspin
            int c2 = 0; // mini

            for (int i = 0; i < points.Length; i++)
            {
                // out of range check
                if (points[i].x >= Tetris.k_Width || points[i].x < 0 ||
                    points[i].y >= Tetris.k_Height + Tetris.k_ExtraHeight || points[i].y < 0)
                {
                    c1++;
                    c2++;
                }
                else if (Tetris.Grid[points[i].x, points[i].y] != null)
                {
                    c1++;
                }
            }

            return (c1 < 3, c2 >= 2);
        }

        public static bool MovePiece(EcsEntity[][] grid, in EcsEntity ePiece, in Vector2 moveDelta)
        {
            ref var cPiecePos = ref ePiece.Get<PositionComponent>();

            ref var x = ref cPiecePos.position.x;
            ref var y = ref cPiecePos.position.y;
            x += moveDelta.x;
            y += moveDelta.y;

            if (!TetrisUtil.IsValidBlock(grid, ePiece))
            {
                x -= moveDelta.x;
                y -= moveDelta.y;

                return false;
            }

            return true;
        }


        public static void RotateBlockWithoutCheck(in EcsEntity ePiece, bool clockwise)
        {
            var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;
            ref var cPiece = ref ePiece.Get<PieceComponent>();
            ref var cPiecePos = ref ePiece.Get<PositionComponent>();

            int angle = clockwise ? -90 : 90;

            var root = cPiecePos.position;
            var pivot = cPiece.pivot + root;
            for (int k = 0; k < tileList.Count; k++)
            {
                var eTile = tileList[k];
                ref var cPos = ref eTile.Get<PositionComponent>();

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


        public static void ResetPieceRotation(ref EcsEntity ePiece)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>();
            if (cPiece.state == 2)
            {
                RotateBlockWithoutCheck(ePiece, true);
                RotateBlockWithoutCheck(ePiece, true);
                cPiece.state = 0;
            }
            else if (cPiece.state == 3)
            {
                RotateBlockWithoutCheck(ePiece, true);
                cPiece.state = 0;
            }
            else if (cPiece.state == 1)
            {
                RotateBlockWithoutCheck(ePiece, false);
                cPiece.state = 0;
            }
        }

        public static EcsEntity CreatePiece(EcsWorld world, EPieceID pieceID, Vector3 spawnPosition)
        {
            var ePiece = world.NewEntity();
            ref var cPiece = ref ePiece.Get<PieceComponent>();
            cPiece.pieceID = pieceID;
            cPiece.scale = 1f;
            ePiece.Get<PieceMoveComponent>();
            ePiece.Get<PositionComponent>().position = spawnPosition;

            var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;

            CreateTileList(world, ePiece, tileList);
            if (pieceID != EPieceID.O)
                ePiece.Get<PieceRotateFlag>();

            return ePiece;
        }

        public static EcsEntity CreatePieceForBagView(EcsWorld world, EPieceID pieceID, Vector3 spawnPosition)
        {
            var ePiece = world.NewEntity();
            ref var cPiece = ref ePiece.Get<PieceComponent>();
            cPiece.pieceID = pieceID;
            cPiece.scale = 0.6f;
            ePiece.Get<PositionComponent>().position = spawnPosition;

            var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;

            CreateTileList(world, ePiece, tileList);

            return ePiece;
        }

        public static EcsEntity CreatePieceForGhost(EcsWorld world, EPieceID pieceID, Vector3 spawnPosition)
        {
            var ePiece = world.NewEntity();
            ref var cPiece = ref ePiece.Get<PieceComponent>();
            cPiece.pieceID = pieceID;
            cPiece.scale = 1f;
            ePiece.Get<PositionComponent>().position = spawnPosition;
            ePiece.Get<PieceGhostComponent>();

            var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;
            CreateTileList(world, ePiece, tileList);

            for (int i = 0; i < tileList.Count; i++)
            {
                var tile = tileList[i];
                ref var tileRenderer = ref tile.Get<TileRendererComponent>();
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

        private static void CreateTileList(EcsWorld world, in EcsEntity ePiece, List<EcsEntity> tileList)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>();
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
                default:
                    break;
            }
        }

        public static EcsEntity CreateTile(EcsWorld world, in EcsEntity parent, Vector3 pos, Color color)
        {
            var eTile = world.NewEntity();
            eTile.Get<PositionComponent>().position = pos;
            eTile.Get<TileRendererComponent>().color = color;
            eTile.Get<ParentComponent>().parent = parent;
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
                default:
                    break;
            }

            return null;
        }

        public static bool WallKickTest(EcsEntity[][] grid, in EcsEntity ePiece, int next, out Vector2Int result)
        {
            ref var cPiece = ref ePiece.Get<PieceComponent>();

            var data = GetWallKickData(cPiece.pieceID, cPiece.state, next);

            for (int i = 0; i < data.Length; i++)
            {
                if (!IsValidBlock(grid, ePiece, data[i]))
                {
                    continue;
                }
                else
                {
                    result = data[i];
                    return true;
                }
            }

            result = Vector2Int.zero;
            return false;
        }

        public static bool IsValidBlock(EcsEntity[][] grid, in EcsEntity ePiece)
        {
            var cPiecePos = ePiece.Get<PositionComponent>();
            var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;

            var rootPos = cPiecePos.position;
            for (int i = 0; i < tileList.Count; i++)
            {
                ref var tilePos = ref tileList[i].Get<PositionComponent>();
                var childPos = tilePos.position + rootPos;
                var x = Mathf.RoundToInt(childPos.x);
                var y = Mathf.RoundToInt(childPos.y);
                if (!IsValid(grid, x, y)) return false;
            }
            return true;
        }

        public static bool IsValidBlock(EcsEntity[][] grid, in EcsEntity ePiece, Vector2 move)
        {
            var cPiecePos = ePiece.Get<PositionComponent>();
            var tileList = ePiece.Get<ComponentList<EcsEntity>>().Value;

            var rootPos = cPiecePos.position;
            for (int i = 0; i < tileList.Count; i++)
            {
                ref var tilePos = ref tileList[i].Get<PositionComponent>();
                var childPos = tilePos.position + rootPos;
                var x = Mathf.RoundToInt(childPos.x + move.x);
                var y = Mathf.RoundToInt(childPos.y + move.y);
                if (!IsValid(grid, x, y)) return false;
            }
            return true;
        }

        public static bool IsValid(EcsEntity[][] grid, int x, int y)
        {
            if (x >= TetrisDef.k_Width || x < 0 || y >= TetrisDef.k_Height + TetrisDef.k_ExtraHeight || y < 0) return false;
            if (grid[y][x].IsAlive()) return false;

            return true;
        }
    }
}
