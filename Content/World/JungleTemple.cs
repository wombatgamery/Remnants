using Microsoft.Xna.Framework;
using Remnants.Content.Walls;
using StructureHelper;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Remnants.Content.World.PrimaryBiomes;
using Remnants.Content.Walls.Parallax;

namespace Remnants.Content.World
{
    public class JungleTemple : GenPass
    {
        public int X;
        public int Y;

        int[,,] layout;

        int roomWidth => 63;
        int roomHeight => 63;

        int roomsVertical => 1 + Main.maxTilesY / 600;
        int roomsHorizontal => roomsVertical * 2 - 1;

        int roomsLeft => 0 - (roomsHorizontal - 1) / 2;
        int roomsRight => (roomsHorizontal - 1) / 2;

        public Rectangle location => new Rectangle(X - roomWidth * roomsHorizontal / 2, Y - roomHeight / 2, roomWidth * roomsHorizontal + 2, roomHeight * roomsVertical);

        public JungleTemple(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.JungleTemple");

            Main.tileSolid[TileID.LihzahrdBrick] = true;
            Main.tileSolid[TileID.WoodenSpikes] = true;
            Main.tileSolid[TileID.Mud] = true;
            Main.tileSolid[TileID.JungleGrass] = true;
            Main.tileSolid[TileID.ClosedDoor] = true;

            bool devMode = false;

            #region setup
            layout = new int[roomsHorizontal, roomsVertical, 6];

            X = Jungle.Center * biomes.scale;
            Y = Math.Max((int)Main.rockLayer, (int)RemWorld.lavaLevel - roomsVertical * roomHeight);

            GenVars.tLeft = location.Left;
            GenVars.tRight = location.Right;
            GenVars.tTop = location.Top;
            GenVars.tBottom = location.Bottom;

            WGTools.Terraform(new Vector2(X, Y - 42), 42, killWall: true);

            Structures.FillEdges(location.Left - roomWidth + 1, location.Bottom, location.Right + roomWidth - 2, location.Bottom + 21, TileID.LihzahrdBrick, false);

            GenVars.structures.AddProtectedStructure(location, 50);

            int cellX;
            int cellY;

            #endregion

            #region rooms
            int roomCount;
            int roomID;

            #region special
            cellX = -1;
            cellY = roomsVertical - 1;

            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX + 2, cellY);

            //AddMarker(cellX, cellY, 2); AddMarker(cellX + 1, cellY, 2); AddMarker(cellX + 2, cellY, 2);
            AddMarker(cellX, cellY, 4); AddMarker(cellX + 1, cellY, 4); AddMarker(cellX + 2, cellY, 4);

            int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
            int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/endroom", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

            GenVars.lAltarX = posX + 93;
            GenVars.lAltarY = posY + 61;

            #endregion

            #region standard
            for (cellY = 0; cellY < roomsVertical; cellY++)
            {
                for (cellX = roomsLeft; cellX <= roomsRight; cellX++)
                {
                    if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY, 1))
                    {
                        if ((cellX + cellY) % 2 == 0)
                        {
                            roomID = WorldGen.genRand.Next(1, 4);
                        }
                        else
                        {
                            roomID = WorldGen.genRand.Next(4, 7);
                        }

                        AddMarker(cellX, cellY);
                        AddMarker(cellX, cellY, 4);

                        posX = X + cellX * roomWidth - roomWidth / 2 + 1;
                        posY = Y + cellY * roomHeight - roomHeight / 2 + 1;

                        if (roomID == 1)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/a1", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        }
                        if (roomID == 2)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/a2", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        }
                        if (roomID == 3)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/a3", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        }
                        if (roomID == 4)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/b1", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        }
                        if (roomID == 5)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/b2", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        }
                        if (roomID == 6)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/b3", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        }

                        //if (OpenLeft(cellX, cellY) && OpenTop(cellX, cellY) && OpenRight(cellX, cellY) && OpenBottom(cellX, cellY))
                        //{
                        //    posX = X + cellX * roomWidth - roomWidth / 2 + 1;
                        //    posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
                        //    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/4way", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                        //}
                        //else
                        //{
                        //    DungeonRoom(1, 1, roomWidth, roomHeight, WallID.LihzahrdBrickUnsafe, cellX, cellY, wall: true);
                        //    if (WorldGen.genRand.NextBool(2))
                        //    {
                        //        DungeonRoom(1, 1, roomWidth, roomHeight, TileID.LihzahrdBrick, cellX, cellY);
                        //    }
                        //    else DungeonRoom(1, 1, roomWidth, roomHeight, -1, cellX, cellY);
                        //}
                    }
                }
            }
            #endregion

            #region filler
            for (cellY = 0; cellY < roomsVertical; cellY++)
            {
                for (cellX = roomsLeft; cellX <= roomsRight; cellX++)
                {
                    if (!FindMarker(cellX, cellY, 1))
                    {
                        if (FindMarker(cellX - 1, cellY, 1))
                        {
                            posX = X + (cellX - 1) * roomWidth - roomWidth / 2 + 1;
                            posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/corner-left", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                            if (cellY >= roomsVertical - 1)
                            {
                                DungeonRoom(1, 1, roomWidth, 21, cellX - 1, cellY + 1, tile: TileID.LihzahrdBrick);
                                DungeonRoom(2, 1, roomWidth, 20, cellX - 1, cellY + 1, wall: WallID.LihzahrdBrickUnsafe);
                            }
                            //WGTools.Terraform(new Vector2(posX + 49, posY - 1), 24, true);
                            //WGTools.Terraform(new Vector2(posX + 25, posY + 25), 24, true);
                        }
                        if (FindMarker(cellX + 1, cellY, 1))
                        {
                            posX = X + (cellX + 1) * roomWidth - roomWidth / 2 + 1;
                            posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/corner-right", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
                            if (cellY >= roomsVertical - 1)
                            {
                                DungeonRoom(1, 1, roomWidth, 21, cellX + 1, cellY + 1, tile: TileID.LihzahrdBrick);
                                DungeonRoom(1, 1, roomWidth - 1, 20, cellX + 1, cellY + 1, wall: WallID.LihzahrdBrickUnsafe);
                            }
                            //WGTools.Terraform(new Vector2(posX - 2, posY - 1), 24, true, 1.5f);
                            //WGTools.Terraform(new Vector2(posX + 22, posY + 25), 24, true, 1.5f);
                        }

                        if (cellY >= roomsVertical - 1)
                        {
                            DungeonRoom(1, 1, roomWidth, 21, cellX, cellY + 1, tile: TileID.LihzahrdBrick);
                            DungeonRoom(1, 1, roomWidth, 20, cellX, cellY + 1, wall: WallID.LihzahrdBrickUnsafe);
                        }

                        if (cellX == 0 && FindMarker(cellX, cellY - 1, 1))
                        {
                            posX = X + cellX * roomWidth - roomWidth / 2 + 1;
                            posY = Y + (cellY - 1) * roomHeight - roomHeight / 2 + 1 + roomHeight - 42;
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/JungleTemple/top", new Point16(posX, posY + 1), ModContent.GetInstance<Remnants>());

                            //for (int i = -1; i <= 13; i++)
                            //{
                            //    WGTools.DrawRectangle(posX - i, posY + WorldGen.genRand.Next(21, 23), posX - i, posY + 25, -1, WallID.JungleUnsafe, false);
                            //    WGTools.DrawRectangle(posX + 47 + i, posY + WorldGen.genRand.Next(21, 23), posX + 47 + i, posY + 25, -1, WallID.JungleUnsafe, false);
                            //}
                        }
                    }
                }
            }
            #endregion
            #endregion

            if (!devMode) { Spikes(); }

            Structures.AddTraps(location, 1, true);

            #region objects
            if (!devMode)
            {
                int objects;

                objects = roomsVertical * roomsHorizontal;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    int y = WorldGen.genRand.Next(location.Top + 12, location.Bottom);

                    for (; !WGTools.Solid(x, y + 1) || WGTools.Solid(x, y); y += WGTools.Solid(x, y) ? -1 : 1)
                    {
                    }

                    bool valid = true;
                    if (WGTools.Tile(x, y).LiquidAmount > 0 || WGTools.Tile(x, y).WallType != ModContent.WallType<temple>() && WGTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe)
                    {
                        valid = false;
                    }
                    else for (int i = x - 3; i <= x + 3; i++)
                        {
                            if (WGTools.Tile(i, y).HasTile || !WGTools.Solid(i, y + 1))
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        WGTools.CandleBunch(x - 3, y, x + 3, y, WorldGen.genRand.Next(3, 6));
                        objects--;
                    }
                }

                objects = roomsVertical * roomsHorizontal / 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    int y = WorldGen.genRand.Next(location.Top + 12, location.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x - 1, y).HasTile && WGTools.Tile(x - 1, y).TileType == TileID.ClosedDoor || WGTools.Tile(x + 2, y).HasTile && WGTools.Tile(x + 2, y).TileType == TileID.ClosedDoor)
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && WGTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else for (int i = -1; i <= 2; i++)
                        {
                            if (!WGTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 1).TileType] || WGTools.Tile(x + i, y + 1).TileType != TileID.LihzahrdBrick)
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, style: 16, notNearOtherChests: true);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            ChestLoot(chestIndex);

                            objects--;
                        }
                    }
                }

                objects = roomsVertical * roomsHorizontal;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    int y = WorldGen.genRand.Next(location.Top - 12, location.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles)
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && WGTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y + 1).TileType != TileID.LihzahrdBrick && WGTools.Tile(x, y + 1).TileType != TileID.JungleGrass)
                    {
                        valid = false;
                    }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, TileID.LargePiles2, style: Main.rand.Next(18, 23));
                        if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles2)
                        {
                            objects--;
                        }
                    }
                }

                objects = roomsVertical * roomsHorizontal * 8;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    int y = WorldGen.genRand.Next(location.Top - 12, location.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Pots || WGTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && WGTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y + 1).TileType != TileID.LihzahrdBrick && WGTools.Tile(x, y + 1).TileType != TileID.JungleGrass)
                    {
                        valid = false;
                    }

                    if (valid)
                    {
                        WorldGen.PlacePot(x, y, style: Main.rand.Next(28, 31));
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Pots)
                        {
                            objects--;
                        }
                    }
                }

                objects = roomsVertical * roomsHorizontal / 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot || WGTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && WGTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y + 1).TileType != TileID.LihzahrdBrick && WGTools.Tile(x, y + 1).TileType != TileID.JungleGrass)
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y - 1).HasTile)
                    {
                        valid = false;
                    }
                    else for (int i = -1; i <= 1; i++)
                        {
                            if (!WGTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 1).TileType])
                            {
                                valid = false;
                                break;
                            }
                            else if (WGTools.Tile(x + i, y).HasTile && WGTools.Tile(x + i, y).TileType == TileID.ClosedDoor)
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, TileID.ClayPot);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                        {
                            WorldGen.PlaceTile(x, y - 1, TileID.ImmatureHerbs, style: 1);
                            objects--;
                        }
                    }
                }
            }
            #endregion

            #region cleanup
            if (!devMode)
            {
                FastNoiseLite weathering = new FastNoiseLite();
                weathering.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                weathering.SetFrequency(0.05f);
                weathering.SetFractalType(FastNoiseLite.FractalType.FBm);
                weathering.SetFractalOctaves(3);

                Structures.AddVariation(location);

                for (int y = location.Top + 1; y <= location.Bottom + 21; y++)
                {
                    for (int x = location.Left - 21 - 38; x <= location.Right + 21 + 38; x++)
                    {
                        Tile tile = WGTools.Tile(x, y);

                        if (tile.TileType == TileID.LihzahrdBrick && WGTools.Tile(x, y - 1).TileType != TileID.LihzahrdAltar)
                        {
                            float _weathering = weathering.GetNoise(x, y);
                            if (_weathering > -0.05f && _weathering < 0.05f)
                            {
                                tile.TileType = !WGTools.SurroundingTilesActive(x, y, true) ? TileID.JungleGrass : TileID.Mud;
                            }
                        }
                        if (tile.WallType == WallID.LihzahrdBrickUnsafe || tile.WallType == ModContent.WallType<temple>())
                        {
                            tile.LiquidAmount = 0;
                        }
                    }
                }
            }
            #endregion
        }

        #region functions
        private void ChestLoot(int chestIndex)
        {
            var itemsToAdd = new List<(int type, int stack)>();

            itemsToAdd.Add((ItemID.LihzahrdPowerCell, 1));

            if (Main.rand.NextBool(2))
            {
                itemsToAdd.Add((ItemID.LunarTabletFragment, Main.rand.Next(1, 5)));
            }

            Structures.GenericLoot(chestIndex, itemsToAdd, 3);

            Structures.FillChest(chestIndex, itemsToAdd);
        }

        private bool FreeSpace(int cellX, int cellY, int width = 1, int height = 1)
        {
            for (int j = cellY; j < cellY + height; j++)
            {
                for (int i = cellX; i < cellX + width; i++)
                {
                    if (FindMarker(i, j) || FindMarker(i, j, 1))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void DungeonRoom(int left, int top, int right, int bottom, int cellX, int cellY, int tile = -2, int wall = -2, bool add = true, bool replace = true, int style = 0, int liquid = -1, int liquidType = -1)
        {
            WGTools.Rectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top - roomHeight / 2, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom - roomHeight / 2, tile, wall, add, replace, style, liquid, liquidType);
        }
        private void AddMarker(int cellX, int cellY, int layer = 0)
        {
            layout[cellX - roomsLeft, cellY, layer] = -1;
        }
        private bool FindMarker(int cellX, int cellY, int layer = 0)
        {
            if (layer == 1 && (cellX < -cellY || cellX > cellY))
            {
                return true;
            }
            else if (cellX < roomsLeft || cellX > roomsRight || cellY < 0 || cellY >= roomsVertical)
            {
                return false;
            }
            else return layout[cellX - roomsLeft, cellY, layer] == -1;
        }

        private bool OpenLeft(int cellX, int cellY)
        {
            if (FindMarker(cellX - 1, cellY, 1))
            {
                return true;
            }
            else return !FindMarker(cellX - 1, cellY, 3);
        }
        private bool OpenRight(int cellX, int cellY)
        {
            if (FindMarker(cellX + 1, cellY, 1))
            {
                return true;
            }
            else return !FindMarker(cellX + 1, cellY, 5);
        }
        private bool OpenTop(int cellX, int cellY)
        {
            if (FindMarker(cellX, cellY - 1, 1))
            {
                return true;
            }
            else return !FindMarker(cellX, cellY - 1, 4);
        }
        private bool OpenBottom(int cellX, int cellY)
        {
            if (cellY + 1 >= roomsVertical)
            {
                return true;
            }
            else return !FindMarker(cellX, cellY + 1, 2);
        }
        #endregion

        private void Spikes()
        {
            int num4 = 0;
            int num5 = 1000;
            int num6 = 0;
            while (num6 < roomsVertical * roomsHorizontal)
            {
                num4++;
                int num7 = WorldGen.genRand.Next(location.Left, location.Right);
                int num8 = WorldGen.genRand.Next(location.Top, location.Bottom);
                int num9 = num7;
                if ((Main.tile[num7, num8].WallType == WallID.LihzahrdBrickUnsafe || Main.tile[num7, num8].WallType == ModContent.WallType<temple>()) && !Main.tile[num7, num8].HasTile)
                {
                    int num10 = 1;
                    if (WorldGen.genRand.NextBool(2))
                    {
                        num10 = -1;
                    }
                    for (; !Main.tile[num7, num8].HasTile; num8 += num10)
                    {
                    }
                    if (SpikeValidation(num7, num8) && Main.tile[num7 - 1, num8].HasTile && Main.tile[num7 + 1, num8].HasTile && !Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                    {
                        num6++;
                        int num12 = WorldGen.genRand.Next(5, 13);
                        while (SpikeValidation(num7, num8) && Main.tile[num7 - 1, num8].HasTile && Main.tile[num7, num8 + num10].HasTile && Main.tile[num7, num8].HasTile && !Main.tile[num7, num8 - num10].HasTile && num12 > 0)
                        {
                            Main.tile[num7, num8].TileType = TileID.WoodenSpikes;
                            if (!Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                            {
                                WGTools.Tile(num7, num8 - num10).TileType = TileID.WoodenSpikes;
                                WGTools.Tile(num7, num8 - num10).HasTile = true;
                            }
                            num7--;
                            num12--;
                        }
                        num12 = WorldGen.genRand.Next(5, 13);
                        num7 = num9 + 1;
                        while (SpikeValidation(num7, num8) && Main.tile[num7 + 1, num8].HasTile && Main.tile[num7, num8 + num10].HasTile && Main.tile[num7, num8].HasTile && !Main.tile[num7, num8 - num10].HasTile && num12 > 0)
                        {
                            Main.tile[num7, num8].TileType = TileID.WoodenSpikes;
                            if (!Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                            {
                                WGTools.Tile(num7, num8 - num10).TileType = TileID.WoodenSpikes;
                                WGTools.Tile(num7, num8 - num10).HasTile = true;
                            }
                            num7++;
                            num12--;
                        }
                    }
                }
                if (num4 > num5)
                {
                    num4 = 0;
                    num6++;
                }
            }
        }

        private bool SpikeValidation(int x, int y)
        {
            for (int i = x - 2; i <= x + 2; i++)
            {
                if (i >= x - 1 && i <= x + 1)
                {
                    if (WGTools.Tile(i, y).HasTile && (WGTools.Tile(i, y).TileType == TileID.Platforms || WGTools.Tile(i, y).TileType == TileID.TrapdoorClosed))
                    {
                        return false;
                    }
                }
                if (WGTools.Tile(i, y - 1).HasTile && WGTools.Tile(i, y - 1).TileType == TileID.ClosedDoor)
                {
                    return false;
                }
            }
            return WGTools.Solid(x, y);
        }
    }
}
