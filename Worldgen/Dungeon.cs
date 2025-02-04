using Microsoft.Xna.Framework;
using Remnants.Tiles.Objects.Furniture;
using Remnants.Tiles.Objects.Hazards;
using Remnants.Walls.Parallax;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Remnants.Worldgen
{
    public class TheDungeon : GenPass
    {
        public TheDungeon(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Building a cursed city";

            Main.tileSolid[TileID.BlueDungeonBrick] = true;
            Main.tileSolid[TileID.GreenDungeonBrick] = true;
            Main.tileSolid[TileID.PinkDungeonBrick] = true;
            Main.tileSolid[TileID.Spikes] = true;
            Main.tileSolid[TileID.ClosedDoor] = true;

            bool devMode = false;

            #region setup
            Structures.Dungeon dungeon = new Structures.Dungeon(0, (int)Main.worldSurface + 30, (int)(Main.maxTilesX / 4200f * 8) + 1, (int)(Main.maxTilesY / 1200f * 6), 35, 30, 5);

            if (GenVars.dungeonSide == 1)
            {
                dungeon.X = (int)(Main.maxTilesX * (ModLoader.TryGetMod("InfernumMode", out Mod infernum) ? 0.875f : 0.9f)) - dungeon.area.Width / 2;
            }
            else
            {
                dungeon.X = (int)(Main.maxTilesX * (ModLoader.TryGetMod("InfernumMode", out Mod infernum) ? 0.125f : 0.1f)) - dungeon.area.Width / 2;
            }
            dungeon.Y = (int)Main.worldSurface + 10;

            Main.dungeonX = GenVars.dungeonX = dungeon.area.Center.X;

            GenVars.dMinX = dungeon.area.Left;
            GenVars.dMaxX = dungeon.area.Right;
            GenVars.dMinY = dungeon.area.Top;
            GenVars.dMaxY = dungeon.area.Bottom;

            //for (int i = dungeon.area.Left - 7; i < dungeon.area.Right + 7; i++)
            //{
            //    WorldGen.TileRunner(i, dungeon.area.Top - 13, WorldGen.genRand.Next(6, 18), 1, TileID.Dirt, true, overRide: false);
            //    WorldGen.TileRunner(i, dungeon.area.Bottom + 13, WorldGen.genRand.Next(6, 18), 1, TileID.Stone, true, overRide: false);
            //}

            Structures.FillEdges(dungeon.area.Left - 8, dungeon.area.Top - 1, dungeon.area.Right + 7, dungeon.area.Bottom + 12, TileID.BlueDungeonBrick, false);

            WGTools.Rectangle(dungeon.area.Left - 8, dungeon.area.Top - 1, dungeon.area.Right + 7, dungeon.area.Bottom + 12, TileID.BlueDungeonBrick, liquid: 0);
            WGTools.Rectangle(dungeon.area.Left - 7, dungeon.area.Top, dungeon.area.Right + 6, dungeon.area.Bottom + 11, wall: WallID.BlueDungeonUnsafe);
            GenVars.structures.AddProtectedStructure(dungeon.area, 25);

            int[] bricks = new int[3];
            bricks[0] = TileID.BlueDungeonBrick;
            bricks[1] = TileID.GreenDungeonBrick;
            bricks[2] = TileID.PinkDungeonBrick;

            int brick = bricks[WorldGen.genRand.Next(bricks.Length)];
            if (devMode) { brick = bricks[0]; }
            #endregion

            #region entrance
            int entranceX = dungeon.area.Center.X - 17;
            int entranceY = dungeon.Y;

            bool valid2 = false;
            while (!valid2)
            {
                entranceY -= 6;
                if (entranceY < Main.worldSurface - 30)
                {
                    int score = 0;
                    for (int i = -35; i <= 70; i++)
                    {
                        if ((Framing.GetTileSafely(entranceX + i, entranceY).HasTile && Main.tileSolid[Framing.GetTileSafely(entranceX + i, entranceY).TileType]) || Framing.GetTileSafely(entranceX + i, entranceY).WallType != 0)
                        {
                            score--;
                        }
                        else score++;
                    }
                    valid2 = true;
                    if (score < 0)
                    {
                        valid2 = false;
                    }
                }
                //if (!valid2)
                //{
                //    Generator.GenerateStructure("Structures/special/dungeon/entrance-tunnel", new Point16(entranceX, entranceY), ModContent.GetInstance<Remnants>());
                //}
            }
            entranceY -= 1;

            int stairs = 80;

            Structures.FillEdges(entranceX, entranceY + 80, entranceX + 34, dungeon.area.Top, TileID.BlueDungeonBrick, false);
            Structures.FillEdges(entranceX - stairs - 24, entranceY + stairs + 1, entranceX + 34 + stairs + 24, entranceY + stairs + 21, TileID.BlueDungeonBrick, false);

            for (int i = 1; i <= stairs; i++)
            {
                WGTools.Rectangle(entranceX - 5 - i, entranceY + 1 + i, entranceX, entranceY + 1 + i, TileID.BlueDungeonBrick);
                WGTools.Rectangle(entranceX + 35, entranceY + 1 + i, entranceX + 39 + i, entranceY + 1 + i, TileID.BlueDungeonBrick);
            }
            WGTools.Rectangle(entranceX - stairs - 24, entranceY + stairs + 1, entranceX, entranceY + stairs + 21, TileID.BlueDungeonBrick);
            WGTools.Rectangle(entranceX + 35, entranceY + stairs + 1, entranceX + 35 + stairs + 24, entranceY + stairs + 21, TileID.BlueDungeonBrick);

            //WGTools.Rectangle(entranceX - scale - 24, entranceY - 27, entranceX + scale + 60, entranceY + scale + 21, liquid: 0);

            WGTools.Terraform(new Vector2(entranceX, entranceY), 23);
            WGTools.Terraform(new Vector2(entranceX + 35, entranceY), 23);

            for (int j = dungeon.Y; j > entranceY + 1; j -= 6)
            {
                Generator.GenerateStructure("Structures/special/dungeon/entrance-tunnel", new Point16(entranceX, j), ModContent.GetInstance<Remnants>());
            }
            Generator.GenerateStructure("Structures/special/dungeon/entrance-top", new Point16(entranceX - 5, entranceY - 33), ModContent.GetInstance<Remnants>());

            for (int i = 1; !WGTools.Tile(entranceX - 5 - i, entranceY + i).HasTile; i++)
            {
                WorldGen.PlaceTile(entranceX - 5 - i, entranceY + i, TileID.Platforms, style: 6);
                WGTools.Tile(entranceX - 5 - i, entranceY + i).Slope = SlopeType.SlopeDownRight;
            }
            for (int i = 1; !WGTools.Tile(entranceX + 39 + i, entranceY + i).HasTile; i++)
            {
                WorldGen.PlaceTile(entranceX + 39 + i, entranceY + i, TileID.Platforms, style: 6);
                WGTools.Tile(entranceX + 39 + i, entranceY + i).Slope = SlopeType.SlopeDownLeft;
            }

            Main.dungeonY = entranceY - 29;

            GenVars.structures.AddProtectedStructure(new Rectangle(entranceX, entranceY - 33, 35, dungeon.Y - entranceY + 33), 25);
            #endregion

            #region rooms
            dungeon.targetCell.X = dungeon.grid.Center.X;
            dungeon.targetCell.Y = 0;

            if (dungeon.AddRoom(1, 1))
            {
                dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2);

                Generator.GenerateStructure("Structures/special/dungeon/entrance-bottom", dungeon.roomPos, ModContent.GetInstance<Remnants>());
            }

            dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
            dungeon.targetCell.Y = dungeon.grid.Height - 1;
            if (dungeon.AddRoom(3, 1))
            {
                dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y, 2);

                Generator.GenerateStructure("Structures/special/dungeon/vault", dungeon.roomPos, ModContent.GetInstance<Remnants>());
            }

            int roomCount;

            #region special
            if (!devMode)
            {
                int chestIndex;

                chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 37, dungeon.roomPos.Y + 21, TileID.Containers2, style: 13);
                BiomeChestLoot(chestIndex, 6);

                chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 51, dungeon.roomPos.Y + 21, style: 23);
                BiomeChestLoot(chestIndex, 1);

                chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 67, dungeon.roomPos.Y + 21, style: 27);
                BiomeChestLoot(chestIndex, 5);
            }
            #region 3x2
            roomCount = dungeon.grid.Height * dungeon.grid.Width / 54;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
                dungeon.targetCell.Y = WorldGen.genRand.Next(0, dungeon.grid.Height - 1);

                dungeon.targetCell.X = (int)((dungeon.targetCell.X - 1) / 3) * 3 + 1;

                bool valid = true;

                if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 2, 1))
                {
                    valid = false;
                }
                else if (dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 2) && !dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 2, 1) && dungeon.targetCell.Y != dungeon.grid.Height - 2)
                {
                    valid = false;
                }

                if (dungeon.AddRoom(3, 2, valid))
                {
                    dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                    dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 2);

                    Generator.GenerateMultistructureRandom("Structures/special/dungeon/stairwell1", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount--;
                }
            }

            roomCount = dungeon.grid.Height * dungeon.grid.Width / 54;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
                dungeon.targetCell.Y = WorldGen.genRand.Next(0, dungeon.grid.Height - 1);

                dungeon.targetCell.X = (int)((dungeon.targetCell.X - 1) / 3) * 3 + 1;

                bool valid = true;

                if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 2, 1))
                {
                    valid = false;
                }
                else if (dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 2) && !dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 2, 1) && dungeon.targetCell.Y != dungeon.grid.Height - 2)
                {
                    valid = false;
                }

                if (dungeon.AddRoom(3, 2, valid))
                {
                    dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                    dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 2); dungeon.AddMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 1, 2);



                    Generator.GenerateMultistructureRandom("Structures/special/dungeon/stairwell2", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount--;
                }
            }
            #region unused
            //roomCount = dungeon.grid.Height * dungeon.grid.Width / 54;
            //while (roomCount > 0)
            //{
            //    dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right);
            //    dungeon.targetCell.Y = WorldGen.genRand.Next(0, dungeon.grid.Height - 1);
            //    int index = WorldGen.genRand.Next(2);

            //    bool valid = true;

            //    if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y) || dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1))
            //    {
            //        valid = false;
            //    }
            //    else if (!OpenLeft(dungeon.targetCell.X, dungeon.targetCell.Y) || !OpenRight(dungeon.targetCell.X + 1, dungeon.targetCell.Y) || !OpenLeft(dungeon.targetCell.X, dungeon.targetCell.Y + 1) || !OpenRight(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1) || !OpenTop(dungeon.targetCell.X + (index == 1 ? 1 : 0), dungeon.targetCell.Y))
            //    {
            //        valid = false;
            //    }
            //    else if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 2) && OpenBottom(dungeon.targetCell.X, dungeon.targetCell.Y + 1) || dungeon.FindMarker(dungeon.targetCell.X + (index == 1 ? 0 : 1), dungeon.targetCell.Y - 1) && OpenTop(dungeon.targetCell.X + (index == 1 ? 0 : 1), dungeon.targetCell.Y) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 2) && OpenBottom(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1))
            //    {
            //        valid = false;
            //    }

            //    if (valid)
            //    {
            //        dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y); dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1);
            //        dungeon.AddMarker(dungeon.targetCell.X + (index == 1 ? 0 : 1), dungeon.targetCell.Y, 1); dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 2);

            //        
            //        
            //        Generator.GenerateMultistructureSpecific("Structures/special/dungeon/prison", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index);

            //        if (WorldGen.genRand.NextBool(2))
            //        {
            //            if (index == 1)
            //            {
            //                PlaceLargePainting(25, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //                PlaceLargePainting(38, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //            else
            //            {
            //                PlaceLargePainting(32, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //                PlaceLargePainting(45, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //        }
            //        else
            //        {
            //            if (index == 1)
            //            {
            //                PlaceSmallPainting(32, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //            else
            //            {
            //                PlaceSmallPainting(39, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //        }

            //        roomCount--;
            //    }
            //}

            //roomCount = dungeon.grid.Height * dungeon.grid.Width / 54;
            //while (roomCount > 0)
            //{
            //    dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right);
            //    dungeon.targetCell.Y = WorldGen.genRand.Next(0, dungeon.grid.Height - 1);
            //    int index = WorldGen.genRand.Next(2);

            //    bool valid = true;

            //    if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y) || dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1))
            //    {
            //        valid = false;
            //    }
            //    else if (!OpenLeft(dungeon.targetCell.X, dungeon.targetCell.Y) || !OpenRight(dungeon.targetCell.X + 1, dungeon.targetCell.Y) || !OpenLeft(dungeon.targetCell.X, dungeon.targetCell.Y + 1) || !OpenRight(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1) || !OpenTop(dungeon.targetCell.X + (index == 1 ? 1 : 0), dungeon.targetCell.Y))
            //    {
            //        valid = false;
            //    }
            //    else if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 2) && OpenBottom(dungeon.targetCell.X, dungeon.targetCell.Y + 1) || dungeon.FindMarker(dungeon.targetCell.X + (index == 1 ? 0 : 1), dungeon.targetCell.Y - 1) && OpenTop(dungeon.targetCell.X + (index == 1 ? 0 : 1), dungeon.targetCell.Y) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 2) && OpenBottom(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1))
            //    {
            //        valid = false;
            //    }

            //    if (valid)
            //    {
            //        dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y); dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1);
            //        dungeon.AddMarker(dungeon.targetCell.X + (index == 1 ? 0 : 1), dungeon.targetCell.Y, 1); dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 2);

            //        
            //        
            //        Generator.GenerateMultistructureSpecific("Structures/special/dungeon/livingquarters-alt", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index);

            //        int style = 5;
            //        if (brick == TileID.GreenDungeonBrick)
            //        {
            //            style = 6;
            //        }
            //        else if (brick == TileID.PinkDungeonBrick)
            //        {
            //            style = 7;
            //        }

            //        if (!devMode)
            //        {
            //            int chestIndex;
            //            for (int i = 0; i < 3; i++)
            //            {
            //                chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + (index == 1 ? 48 : 22), dungeon.roomPos.Y + 33 + i * 8, TileID.Dressers, style: style);
            //                DresserLoot(chestIndex);
            //            }
            //        }

            //        if (WorldGen.genRand.NextBool(2))
            //        {
            //            if (index == 1)
            //            {
            //                PlaceLargePainting(25, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //                PlaceLargePainting(38, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //            else
            //            {
            //                PlaceLargePainting(32, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //                PlaceLargePainting(45, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //        }
            //        else
            //        {
            //            if (index == 1)
            //            {
            //                PlaceSmallPainting(32, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //            else
            //            {
            //                PlaceSmallPainting(39, 15, dungeon.targetCell.X, dungeon.targetCell.Y);
            //            }
            //        }

            //        roomCount--;
            //    }
            //}
            #endregion
            #endregion
            #region 2x2
            roomCount = dungeon.grid.Height * dungeon.grid.Width / 27;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 1);
                dungeon.targetCell.Y = WorldGen.genRand.Next(1, dungeon.grid.Height - 1);

                bool valid = true;

                if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1, 2))
                {
                    valid = false;
                }
                else if (dungeon.targetCell.Y != dungeon.grid.Height - 2 && (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 2) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 2, 1) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 2) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 2, 1)))
                {
                    valid = false;
                }
                else if (!WorldGen.genRand.NextBool(100) && (dungeon.FindMarker(dungeon.targetCell.X - 1, dungeon.targetCell.Y + 1, 3) && dungeon.FindMarker(dungeon.targetCell.X - 2, dungeon.targetCell.Y + 1, 3) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 1, 3) && dungeon.FindMarker(dungeon.targetCell.X + 3, dungeon.targetCell.Y + 1, 3)))
                {
                    valid = false;
                }

                if (dungeon.AddRoom(2, 2, valid))
                {
                    dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 2);
                    dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 3); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 3);

                    Generator.GenerateStructure("Structures/special/dungeon/deathpit", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                    WorldGen.PlaceObject(dungeon.roomPos.X + 32, dungeon.roomPos.Y + 48, ModContent.TileType<PiranhaSpawner>());
                    TileEntity.PlaceEntityNet(dungeon.roomPos.X + 31, dungeon.roomPos.Y + 47, ModContent.GetInstance<TEpiranhaspawner>().Type);

                    WorldGen.PlaceObject(dungeon.roomPos.X + 37, dungeon.roomPos.Y + 48, ModContent.TileType<PiranhaSpawner>());
                    TileEntity.PlaceEntityNet(dungeon.roomPos.X + 36, dungeon.roomPos.Y + 47, ModContent.GetInstance<TEpiranhaspawner>().Type);

                    for (int i = dungeon.room.Left + 13; i <= dungeon.room.Right + dungeon.room.Width - 14; i += 2)
                    {
                        WorldGen.TileRunner(i, dungeon.room.Bottom + dungeon.room.Height - WorldGen.genRand.Next(2), WorldGen.genRand.Next(3, 10), 1, TileID.BoneBlock, true, overRide: false);
                    }

                    int objects;

                    objects = 1;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(dungeon.room.Left + 14, dungeon.room.Right + dungeon.room.Width - 15);
                        int y = WorldGen.genRand.Next(dungeon.room.Top + 55, dungeon.room.Bottom + dungeon.room.Height);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.Containers)
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, style: 41, notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                            {
                                #region chestloot
                                var itemsToAdd = new List<(int type, int stack)>();

                                itemsToAdd.Add((ItemID.BoneWelder, 1));
                                itemsToAdd.Add((ItemID.BoneWand, 1));

                                Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                                Structures.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }

                    roomCount--;
                }
            }
            #endregion
            #region 3x1
            roomCount = dungeon.grid.Height * dungeon.grid.Width / 27;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
                dungeon.targetCell.Y = WorldGen.genRand.Next(0, dungeon.grid.Height);
                int index = WorldGen.genRand.Next(2);

                bool valid = true;

                if (dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 1) && dungeon.targetCell.Y != dungeon.grid.Height - 1)
                {
                    valid = false;
                }
                else if (index == 1)
                {
                    if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 1, 1))
                    {
                        valid = false;
                    }
                    else if (dungeon.targetCell.Y != dungeon.grid.Height - 1 && dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 1) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y - 1, 2))
                    {
                        valid = false;
                    }
                }
                else
                {
                    if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 1) || dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y - 1, 2))
                    {
                        valid = false;
                    }
                    else if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.targetCell.Y != dungeon.grid.Height - 1 && dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 1) && !dungeon.FindMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y + 1, 1))
                    {
                        valid = false;
                    }
                }

                if (dungeon.AddRoom(3, 1, valid))
                {
                    dungeon.AddMarker(dungeon.targetCell.X + (index == 1 ? 2 : 0), dungeon.targetCell.Y, 1); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                    dungeon.AddMarker(dungeon.targetCell.X + (index != 1 ? 2 : 0), dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 2);

                    Generator.GenerateMultistructureSpecific("Structures/special/dungeon/library", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index + (roomCount <= dungeon.grid.Height * dungeon.grid.Width / 54 ? 2 : 0));

                    roomCount--;
                }
            }
            #endregion
            #region 2x1
            roomCount = dungeon.grid.Height * dungeon.grid.Width / 27;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 1);
                dungeon.targetCell.Y = WorldGen.genRand.Next(1, dungeon.grid.Height);

                bool valid = true;
                int index = WorldGen.genRand.Next(2);

                if (dungeon.FindMarker(dungeon.targetCell.X + 1 - index, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 1 - index, dungeon.targetCell.Y + 1, 1))
                {
                    valid = false;
                }
                else if (dungeon.FindMarker(dungeon.targetCell.X + index, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X + index, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + index, dungeon.targetCell.Y + 1) && !dungeon.FindMarker(dungeon.targetCell.X + index, dungeon.targetCell.Y + 1, 1))
                {
                    valid = false;
                }
                else if (dungeon.FindMarker(dungeon.targetCell.X + (index == 1 ? -1 : 2), dungeon.targetCell.Y, 3) || dungeon.FindMarker(dungeon.targetCell.X + (index != 1 ? -1 : 2), dungeon.targetCell.Y, 4))
                {
                    valid = false;
                }

                if (dungeon.AddRoom(2, 1, valid))
                {
                    dungeon.AddMarker(dungeon.targetCell.X + (index == 1 ? 1 : 0), dungeon.targetCell.Y, 1); dungeon.AddMarker(dungeon.targetCell.X + (index == 1 ? 1 : 0), dungeon.targetCell.Y, 2);
                    dungeon.AddMarker(dungeon.targetCell.X + (index == 1 ? 1 : 0), dungeon.targetCell.Y, 3); dungeon.AddMarker(dungeon.targetCell.X + (index != 1 ? 1 : 0), dungeon.targetCell.Y, 4);

                    Generator.GenerateMultistructureSpecific("Structures/special/dungeon/prison", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index);

                    int x = dungeon.roomPos.X + (index == 1 ? 64 : 4);
                    WGTools.Rectangle(x, dungeon.roomPos.Y + 28, x + 1, dungeon.roomPos.Y + 29, TileID.CrackedBlueDungeonBrick);

                    x = dungeon.roomPos.X + (index == 1 ? 66 : 2);
                    WGTools.Rectangle(x, dungeon.roomPos.Y + 28, x + 1, dungeon.roomPos.Y + 29, -1);

                    #region chest
                    int chestIndex = WorldGen.PlaceChest(x, dungeon.roomPos.Y + 29, style: 40);

                    var itemsToAdd = new List<(int type, int stack)>();

                    itemsToAdd.Add((ItemID.Handgun, 1));
                    itemsToAdd.Add((ItemID.MusketBall, Main.rand.Next(50, 150)));

                    Structures.GenericLoot(chestIndex, itemsToAdd);

                    Structures.FillChest(chestIndex, itemsToAdd);
                    #endregion

                    roomCount--;
                }
            }
            #endregion
            #endregion

            #region standard

            int roomID = 1;
            int attempts = 1000;
            while (attempts > 0)
            {
                attempts--;

                #region 2x1
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 1);
                dungeon.targetCell.Y = WorldGen.genRand.Next(0, dungeon.grid.Height);



                if (roomID == 1)
                {
                    bool valid = true;

                    if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 1))
                    {
                        valid = false;
                    }
                    else if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 1) && dungeon.targetCell.Y != dungeon.grid.Bottom - 1 || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1, 2))
                    {
                        valid = false;
                    }

                    if (dungeon.AddRoom(2, 1, valid))
                    {
                        dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);

                        Generator.GenerateMultistructureRandom("Structures/special/dungeon/b1", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                        attempts = 1000;
                    }
                }
                if (roomID == 2)
                {
                    bool valid = true;

                    if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 1) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y - 1, 2))
                    {
                        valid = false;
                    }
                    else if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1) && !dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y + 1, 1) && dungeon.targetCell.Y != dungeon.grid.Bottom - 1)
                    {
                        valid = false;
                    }

                    if (dungeon.AddRoom(2, 1, valid))
                    {
                        dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 1); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 2);

                        Generator.GenerateMultistructureRandom("Structures/special/dungeon/b2", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                        attempts = 1000;
                    }
                }
                #endregion

                roomID++;
                if (roomID > 2)
                {
                    roomID = 1;
                }
            }

            #endregion

            #region filler
            for (dungeon.targetCell.Y = 0; dungeon.targetCell.Y < dungeon.grid.Height; dungeon.targetCell.Y++)
            {
                for (dungeon.targetCell.X = dungeon.grid.Left; dungeon.targetCell.X < dungeon.grid.Right; dungeon.targetCell.X++)
                {
                    bool altWall = false;

                    if (!dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y))
                    {
                        if (!devMode)
                        {
                            bool top = !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) && dungeon.targetCell.Y > 0;
                            bool bottom = !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 1) && dungeon.targetCell.Y < dungeon.grid.Height - 1;

                            if (bottom && !top)
                            {
                                Generator.GenerateMultistructureRandom("Structures/special/dungeon/a1", dungeon.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (top && !bottom)
                            {
                                Generator.GenerateMultistructureRandom("Structures/special/dungeon/a2", dungeon.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (top && bottom)
                            {
                                int index = WorldGen.genRand.Next(2);
                                Generator.GenerateMultistructureSpecific("Structures/special/dungeon/a3", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index);

                                altWall = true;

                                if (dungeon.targetCell.X == dungeon.grid.Left || dungeon.FindMarker(dungeon.targetCell.X - 1, dungeon.targetCell.Y, 3))
                                {
                                    WGTools.Rectangle(dungeon.roomPos.X, dungeon.roomPos.Y + 14, dungeon.roomPos.X + (index == 0 ? 11 : 4), dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                                }
                                if (dungeon.targetCell.X == dungeon.grid.Right - 1 || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 3))
                                {
                                    WGTools.Rectangle(dungeon.roomPos.X + (index == 0 ? 30 : 23), dungeon.roomPos.Y + 14, dungeon.room.Right, dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                                }

                                if (index == 0)
                                {
                                    WorldGen.PlaceTile(dungeon.roomPos.X + dungeon.room.Width, dungeon.roomPos.Y + 24, TileID.Platforms, style: 9);
                                }
                                else WorldGen.PlaceTile(dungeon.roomPos.X - 1, dungeon.roomPos.Y + 24, TileID.Platforms, style: 9);
                            }
                            else Generator.GenerateStructure("Structures/special/dungeon/hallway", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                            //if (OpenTop(dungeon.targetCell.X, dungeon.targetCell.Y) && (dungeon.targetCell.X == 0 || dungeon.targetCell.Y > 0))
                            //{
                            //    DungeonRoom(17, 1, 19, 9, dungeon.targetCell.X, dungeon.targetCell.Y, -1);
                            //    DungeonRoom(16, 10, 16, 20, dungeon.targetCell.X, dungeon.targetCell.Y, TileID.WoodenBeam);
                            //    DungeonRoom(20, 10, 20, 20, dungeon.targetCell.X, dungeon.targetCell.Y, TileID.WoodenBeam);

                            //    for (int i = 0; i < 11; i++)
                            //    {
                            //        DungeonRoom(17, 19 - i * 2, 19, 19 - i * 2, dungeon.targetCell.X, dungeon.targetCell.Y, TileID.Platforms, style: 6);
                            //    }
                            //}
                            //if (OpenBottom(dungeon.targetCell.X, dungeon.targetCell.Y) && dungeon.targetCell.Y < dungeon.grid.Height - 1)
                            //{
                            //    DungeonRoom(17, 21, 19, dungeon.room.Height, dungeon.targetCell.X, dungeon.targetCell.Y, -1);

                            //    for (int i = 0; i < 5; i++)
                            //    {
                            //        DungeonRoom(17, 21 + i * 2, 19, 21 + i * 2, dungeon.targetCell.X, dungeon.targetCell.Y, TileID.Platforms, style: 6);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        if (dungeon.targetCell.Y == dungeon.grid.Height - 1 && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2))
                        {
                            WGTools.Rectangle(dungeon.roomPos.X + 16, dungeon.room.Bottom, dungeon.roomPos.X + 18, dungeon.room.Bottom, TileID.BlueDungeonBrick);
                            WorldGen.KillTile(dungeon.roomPos.X + 15, dungeon.room.Bottom - 1); WorldGen.KillTile(dungeon.roomPos.X + 19, dungeon.room.Bottom - 1);
                        }
                    }

                    if (!altWall && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 3) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 4))
                    {
                        if (dungeon.targetCell.X == dungeon.grid.Left && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) && (dungeon.targetCell.Y != dungeon.grid.Height - 1 || dungeon.X < Main.maxTilesX * 0.5f) || dungeon.FindMarker(dungeon.targetCell.X - 1, dungeon.targetCell.Y, 3))
                        {
                            WGTools.Rectangle(dungeon.roomPos.X, dungeon.roomPos.Y + (!WGTools.Solid(dungeon.roomPos.X + 13, dungeon.roomPos.Y + 17) ? 19 : 18), dungeon.roomPos.X + 11, dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                            WGTools.Rectangle(dungeon.roomPos.X, dungeon.roomPos.Y + 18, dungeon.roomPos.X + 8, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                            if (WGTools.Solid(dungeon.roomPos.X + 13, dungeon.roomPos.Y + 17))
                            {
                                WGTools.Rectangle(dungeon.roomPos.X + 12, dungeon.roomPos.Y + 18, dungeon.roomPos.X + 14, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                                if (!WGTools.Solid(dungeon.roomPos.X + 13, dungeon.roomPos.Y + 19))
                                {
                                    Generator.GenerateStructure("Structures/special/dungeon/hallway-end", new Point16(dungeon.roomPos.X + 12, dungeon.roomPos.Y + 19), ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                        if (dungeon.targetCell.X == dungeon.grid.Right - 1 && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) && (dungeon.targetCell.Y != dungeon.grid.Height - 1 || dungeon.X > Main.maxTilesX * 0.5f) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 3))
                        {
                            WGTools.Rectangle(dungeon.roomPos.X + 23, dungeon.roomPos.Y + (!WGTools.Solid(dungeon.roomPos.X + 21, dungeon.roomPos.Y + 17) ? 19 : 18), dungeon.room.Right - 1, dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                            WGTools.Rectangle(dungeon.roomPos.X + 26, dungeon.roomPos.Y + 18, dungeon.room.Right - 1, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                            if (WGTools.Solid(dungeon.roomPos.X + 21, dungeon.roomPos.Y + 17))
                            {
                                WGTools.Rectangle(dungeon.roomPos.X + 20, dungeon.roomPos.Y + 18, dungeon.roomPos.X + 22, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                                if (!WGTools.Solid(dungeon.roomPos.X + 21, dungeon.roomPos.Y + 19))
                                {
                                    Generator.GenerateStructure("Structures/special/dungeon/hallway-end", new Point16(dungeon.roomPos.X + 20, dungeon.roomPos.Y + 19), ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                    }

                    if (!dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 3))
                    {
                        if (dungeon.targetCell.X == dungeon.grid.Left && dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.targetCell.X == dungeon.grid.Left && dungeon.targetCell.Y == dungeon.grid.Height - 1 && dungeon.X > Main.maxTilesX * 0.5f)
                        {
                            WGTools.Rectangle(dungeon.area.Left - 20, dungeon.room.Bottom - 5, dungeon.roomPos.X, dungeon.room.Bottom - 1, -1);

                            Generator.GenerateMultistructureSpecific("Structures/special/dungeon/sideexit", new Point16(dungeon.roomPos.X - 12, dungeon.roomPos.Y + 9), ModContent.GetInstance<Remnants>(), 1);
                            Vector2 pos = dungeon.roomPos.ToVector2();
                            WGTools.Terraform(new Vector2(dungeon.area.Left - 20, dungeon.room.Bottom - 3), 3.5f);

                            //if (dungeon.targetCell.Y == dungeon.grid.Height - 1 && X > Main.maxTilesX * 0.5f)
                            //{
                            //    Generator.GenerateMultistructureSpecific("Structures/railnetwork/dungeonstation", new Point16(dungeon.roomPos.X - 35, dungeon.roomPos.Y + 11), ModContent.GetInstance<Remnants>(), 1);
                            //    Infrastructure.AddTrack(dungeon.roomPos.X - 35, dungeon.roomPos.Y + 21);
                            //}
                        }
                        if (dungeon.targetCell.X == dungeon.grid.Right - 1 && dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.targetCell.X == dungeon.grid.Right - 1 && dungeon.targetCell.Y == dungeon.grid.Height - 1 && dungeon.X < Main.maxTilesX * 0.5f)
                        {
                            WGTools.Rectangle(dungeon.room.Right - 1, dungeon.room.Bottom - 5, dungeon.area.Right + 19, dungeon.room.Bottom - 1, -1);

                            Generator.GenerateMultistructureSpecific("Structures/special/dungeon/sideexit", new Point16(dungeon.roomPos.X + dungeon.room.Width - 1, dungeon.roomPos.Y + 9), ModContent.GetInstance<Remnants>(), 0);
                            Vector2 pos = dungeon.roomPos.ToVector2();
                            WGTools.Terraform(new Vector2(dungeon.area.Right + 19, dungeon.room.Bottom - 3), 3.5f);

                            //if (dungeon.targetCell.Y == dungeon.grid.Height - 1 && X < Main.maxTilesX * 0.5f)
                            //{
                            //    Generator.GenerateMultistructureSpecific("Structures/railnetwork/dungeonstation", new Point16(dungeon.roomPos.X + dungeon.room.Width + 7, dungeon.roomPos.Y + 11), ModContent.GetInstance<Remnants>(), 0);
                            //    Infrastructure.AddTrack(dungeon.roomPos.X + dungeon.room.Width + 34, dungeon.roomPos.Y + 21);
                            //}
                        }
                    }
                }
            }
            #endregion

            #endregion

            if (!devMode)
            {
                Spikes(dungeon);

                #region statuetraps
                #endregion
            }

            #region objects
            if (!devMode)
            {
                int objects;

                objects = dungeon.grid.Height * dungeon.grid.Width / 7;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(dungeon.area.Left, dungeon.area.Right);
                    int y = WorldGen.genRand.Next(dungeon.area.Top, dungeon.area.Bottom);

                    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType == TileID.BlueDungeonBrick && WGTools.Tile(x + 1, y + 1).TileType == TileID.BlueDungeonBrick && WGTools.Tile(x - 1, y).TileType != TileID.ClosedDoor && WGTools.Tile(x + 2, y).TileType != TileID.ClosedDoor)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, style: 2, notNearOtherChests: true);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            ChestLoot(chestIndex, objects);

                            WGTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Candelabras, 22);
                            if (devMode || brick == TileID.BlueDungeonBrick)
                            {
                                WGTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Statues, 46);
                            }
                            else if (brick == TileID.GreenDungeonBrick)
                            {
                                WGTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Statues, 47);
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                WGTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Statues, 48);
                            }

                            objects--;
                        }
                    }
                }

                objects = dungeon.grid.Height * dungeon.grid.Width / 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(dungeon.area.Left, dungeon.area.Right);
                    int y = WorldGen.genRand.Next(dungeon.area.Top, dungeon.area.Bottom);

                    for (; !WGTools.Tile(x, y - 1).HasTile || WGTools.Tile(x, y).HasTile; y += WGTools.Tile(x, y).HasTile ? 1 : -1)
                    {

                    }

                    int length = WorldGen.genRand.Next(10, 20);

                    bool valid = true;
                    if (!WGTools.Solid(x, y - 1) || WGTools.Tile(x, y).WallType != ModContent.WallType<dungeonblue>() || WGTools.Tile(x, y - 1).TileType == TileID.TrapdoorClosed)
                    {
                        valid = false;
                    }
                    else for (int j = y + 1; j <= y + length + 3; j++)
                        {
                            for (int i = x - 1; i <= x + 1; i++)
                            {
                                if (i == x && WGTools.Tile(i, j).HasTile || WGTools.Solid(i, j) || WGTools.Tile(i, j).TileType == TileID.Chain)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) { break; }
                        }

                    if (valid)
                    {
                        //WGTools.GetTile(x, y - 1).TileType = TileID.IronBrick;
                        for (int j = y; j < y + length; j++)
                        {
                            WorldGen.PlaceTile(x, j, TileID.Chain);
                        }
                        WorldGen.PlaceTile(x, y + length + 1, TileID.Platforms, style: 9);
                        WorldGen.PlaceTile(x, y + length, TileID.WaterCandle);
                        objects--;
                    }
                }

                objects = 0;
                while (objects < dungeon.grid.Height * dungeon.grid.Width / 4)
                {
                    int x = WorldGen.genRand.Next(dungeon.area.Left, dungeon.area.Right);
                    int y = WorldGen.genRand.Next(dungeon.area.Top, dungeon.area.Bottom);

                    bool valid = true;

                    if (Framing.GetTileSafely(x, y).TileType == TileID.Painting6X4 || Framing.GetTileSafely(x, y + 3).LiquidAmount > 0)
                    {
                        valid = false;
                    }
                    else for (int j = y - 5; j <= y + 4; j++)
                        {
                            for (int i = x - 5; i <= x + 6; i++)
                            {
                                if (Main.tile[i, j].HasTile || Main.tile[i, j].WallType != ModContent.WallType<dungeonblue>())
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }

                    if (valid)
                    {
                        PlaceLargePainting(x, y, objects < 17 ? objects : -1);

                        if (Framing.GetTileSafely(x, y).TileType == TileID.Painting6X4)
                        {
                            objects++;
                        }
                    }
                }

                objects = 0;
                while (objects < dungeon.grid.Height * dungeon.grid.Width / 4)
                {
                    int x = WorldGen.genRand.Next(dungeon.area.Left, dungeon.area.Right);
                    int y = WorldGen.genRand.Next(dungeon.area.Top, dungeon.area.Bottom);

                    bool valid = true;

                    if (Framing.GetTileSafely(x, y).TileType == TileID.Painting3X3 || Framing.GetTileSafely(x, y + 2).LiquidAmount > 0)
                    {
                        valid = false;
                    }
                    else for (int j = y - 3; j <= y + 3; j++)
                        {
                            for (int i = x - 3; i <= x + 3; i++)
                            {
                                if (Main.tile[i, j].HasTile || Main.tile[i, j].WallType != ModContent.WallType<dungeonblue>())
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }

                    if (valid)
                    {
                        PlaceSmallPainting(x, y, objects < 7 ? objects : -1);

                        if (Framing.GetTileSafely(x, y).TileType == TileID.Painting3X3)
                        {
                            objects++;
                        }
                    }
                }
            }
            #endregion

            #region cleanup
            if (!devMode)
            {
                int floodedLevel = dungeon.area.Center.Y; // + dungeon.area.Height / 4;

                for (int y = dungeon.area.Top; y <= dungeon.area.Bottom; y++)
                {
                    for (int x = dungeon.area.Left; x <= dungeon.area.Right; x++)
                    {
                        if (WGTools.Tile(x, y).TileType == TileID.Candles && WorldGen.genRand.NextBool(2) || WGTools.Tile(x, y).TileType == TileID.HangingLanterns)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }
                }

                Structures.AddDecorations(new Rectangle(dungeon.area.Left, entranceY - 18, dungeon.area.Width, dungeon.area.Height + (dungeon.area.Top - (entranceY - 18))));
                Structures.AddVariation(dungeon.area);
                Structures.AddTraps(dungeon.area);

                FastNoiseLite walls = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                walls.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                walls.SetFrequency(0.002f);
                walls.SetFractalType(FastNoiseLite.FractalType.FBm);
                walls.SetFractalOctaves(3);
                walls.SetFractalLacunarity(3);
                walls.SetFractalGain(0.7f);
                walls.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
                walls.SetCellularJitter(0);

                for (int y = entranceY - 33; y <= dungeon.area.Bottom + 35; y++)
                {
                    for (int x = dungeon.area.Left - 34; x <= dungeon.area.Right + 34; x++)
                    {
                        Tile tile = WGTools.Tile(x, y);

                        if (WGTools.Solid(x, y) && tile.TileType != TileID.Spikes && (x >= dungeon.area.Left - 3 && x <= dungeon.area.Right + 3 && y >= dungeon.area.Top - 7 && y <= dungeon.area.Bottom + 8))
                        {
                            tile.WallType = WallID.BlueDungeonUnsafe;
                        }

                        bool dungeonWall = tile.WallType == WallID.BlueDungeonUnsafe || tile.WallType == WallID.BlueDungeonSlabUnsafe || tile.WallType == WallID.BlueDungeonTileUnsafe;

                        float _walls = walls.GetNoise(x + WorldGen.genRand.Next(-5, 6), y + WorldGen.genRand.Next(-5, 6));
                        int lanternStyle;
                        int bannerStyle;
                        if (_walls > 0.25f)
                        {
                            if (dungeonWall)
                            {
                                tile.WallType = WallID.BlueDungeonSlabUnsafe;
                            }
                            lanternStyle = WorldGen.genRand.NextBool(2) ? 1 : 3;
                            bannerStyle = WorldGen.genRand.NextBool(2) ? 2 : 3;
                            if (tile.TileType == TileID.Platforms && tile.TileFrameY == 18 * 9)
                            {
                                tile.TileFrameY += 18 * 1;
                            }
                        }
                        else if (_walls < -0.25f)
                        {
                            if (dungeonWall)
                            {
                                tile.WallType = WallID.BlueDungeonTileUnsafe;
                            }
                            lanternStyle = WorldGen.genRand.NextBool(2) ? 4 : 5;
                            bannerStyle = WorldGen.genRand.NextBool(2) ? 4 : 5;
                            if (tile.TileType == TileID.Platforms && tile.TileFrameY == 18 * 9)
                            {
                                tile.TileFrameY += 18 * 3;
                            }
                        }
                        else
                        {
                            if (dungeonWall)
                            {
                                tile.WallType = WallID.BlueDungeonUnsafe;
                            }
                            lanternStyle = WorldGen.genRand.NextBool(2) ? 0 : 2;
                            bannerStyle = WorldGen.genRand.NextBool(2) ? 0 : 1;
                        }

                        if (WGTools.Tile(x, y).HasTile)
                        {
                            if (WGTools.Tile(x, y).TileType == TileID.HangingLanterns && WGTools.Tile(x, y - 1).TileType != TileID.HangingLanterns)
                            {
                                WGTools.Tile(x, y).TileFrameY += (short)(lanternStyle * 36);
                                WGTools.Tile(x, y + 1).TileFrameY += (short)(lanternStyle * 36);
                            }
                            if (WGTools.Tile(x, y).TileType == TileID.Banners && WGTools.Tile(x, y - 1).TileType != TileID.Banners)
                            {
                                WGTools.Tile(x, y).TileFrameX += (short)(bannerStyle * 18);
                                WGTools.Tile(x, y + 1).TileFrameX += (short)(bannerStyle * 18);
                                WGTools.Tile(x, y + 2).TileFrameX += (short)(bannerStyle * 18);
                            }
                        }

                        //if (y > floodedLevel)
                        //{
                        //    if (!tile.HasTile && WorldGen.genRand.NextBool(100))
                        //    {
                        //        WGTools.DrawRectangle(x - 2, y - 2, x + 2, y + 2, liquid: 255);
                        //    }
                        //    //if (tile.TileType == TileID.BlueDungeonBrick && weathering.GetNoise(x, y) > 0.2f)
                        //    //{
                        //    //    tile.TileType = TileID.CrackedBlueDungeonBrick;
                        //    //}
                        //}

                        #region reframing
                        if (tile.TileType == TileID.Platforms)
                        {
                            if (tile.TileFrameY == 18 * 6)
                            {
                                if (brick == TileID.PinkDungeonBrick)
                                {
                                    tile.TileFrameY += 18;
                                }
                                else if (brick == TileID.GreenDungeonBrick)
                                {
                                    tile.TileFrameY += 18 * 2;
                                }
                            }
                        }
                        if (tile.TileType == TileID.Tables && (tile.TileFrameX < 18 * 17 * 3 || tile.TileFrameX > 18 * 19 * 3) || tile.TileType == TileID.Bookcases || tile.TileType == TileID.Benches || tile.TileType == ModContent.TileType<PiranhaSpawner>())
                        {
                            //if (tile.wall == WallID.BlueDungeonTileUnsafe && (tile.type == TileID.Tables || tile.type == TileID.Bookcases))
                            //{
                            //    tile.frameX += 18 * 12;
                            //}
                            //else
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameX += 18 * 3;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameX += 18 * 6;
                            }
                        }
                        if (tile.TileType == TileID.Chairs && tile.TileFrameY > 18 * 4)
                        {
                            //if (tile.wall == WallID.BlueDungeonTileUnsafe)
                            //{
                            //    tile.frameY += 20 * 8;
                            //}
                            //else
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameY += 20 * 2;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameY += 20 * 4;
                            }
                        }
                        if (tile.TileType == TileID.WorkBenches)
                        {
                            //if (tile.wall == WallID.BlueDungeonTileUnsafe)
                            //{
                            //    tile.frameX += 18 * 8;
                            //}
                            //else
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameX += 36 * 1;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameX += 36 * 2;
                            }
                        }
                        if (tile.TileType == TileID.Containers && tile.TileFrameX >= 36 * 39 && tile.TileFrameX < 36 * 41)
                        {
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameX -= 72 * 2;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameX -= 72 * 1;
                            }
                        }
                        if (tile.TileType == TileID.Beds || tile.TileType == TileID.Candelabras)
                        {
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameY += 18 * 2;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameY += 18 * 4;
                            }
                        }
                        if (tile.TileType == TileID.Candles)
                        {
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameY += 22 * 1;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameY += 22 * 2;
                            }
                        }
                        if (tile.TileType == TileID.Chandeliers || tile.TileType == TileID.Lamps && tile.TileFrameY >= 18 * 72 && tile.TileFrameY <= 18 * 74)
                        {
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameY += 18 * 3;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameY += 18 * 6;
                            }
                        }
                        if (tile.TileType == TileID.GrandfatherClocks)
                        {
                            if (brick == TileID.GreenDungeonBrick)
                            {
                                tile.TileFrameX += 18 * 2;
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                tile.TileFrameX += 18 * 4;
                            }
                        }

                        //if (y > dungeon.area.Center.Y)
                        //{
                        //    if (tile.TileType == TileID.ClosedDoor && tile.TileFrameY >= 18 * 39 && tile.TileFrameY <= 18 * 41)
                        //    {
                        //        tile.TileFrameY += 18 * 6;
                        //    }
                        //}
                        //if (y > floodedLevel)
                        //{
                        //    if (tile.TileType == TileID.Lamps || tile.TileType == TileID.HangingLanterns)
                        //    {
                        //        tile.TileFrameX += 18 * 1;
                        //    }
                        //    if (tile.TileType == TileID.Candelabras)
                        //    {
                        //        tile.TileFrameX += 18 * 2;
                        //    }
                        //    if (tile.TileType == TileID.Chandeliers)
                        //    {
                        //        tile.TileFrameX += 18 * 3;
                        //    }
                        //}
                        #endregion

                        #region bricks
                        if (brick == TileID.GreenDungeonBrick)
                        {
                            if (WGTools.Tile(x, y).TileType == TileID.BlueDungeonBrick)
                            {
                                WGTools.Tile(x, y).TileType = TileID.GreenDungeonBrick;
                            }
                            else if (WGTools.Tile(x, y).TileType == TileID.CrackedBlueDungeonBrick)
                            {
                                WGTools.Tile(x, y).TileType = TileID.CrackedGreenDungeonBrick;
                            }
                            if (WGTools.Tile(x, y).WallType == ModContent.WallType<dungeonblue>())
                            {
                                WGTools.Tile(x, y).WallType = (ushort)ModContent.WallType<dungeongreen>();
                            }
                            else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeonUnsafe || WGTools.Tile(x, y).WallType == WallID.BlueDungeon)
                            {
                                WGTools.Tile(x, y).WallType = WallID.GreenDungeonUnsafe;
                            }
                            else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeonSlabUnsafe)
                            {
                                WGTools.Tile(x, y).WallType = WallID.GreenDungeonSlabUnsafe;
                            }
                            else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeonTileUnsafe)
                            {
                                WGTools.Tile(x, y).WallType = WallID.GreenDungeonTileUnsafe;
                            }
                        }
                        else if (brick == TileID.PinkDungeonBrick)
                        {
                            if (WGTools.Tile(x, y).TileType == TileID.BlueDungeonBrick)
                            {
                                WGTools.Tile(x, y).TileType = TileID.PinkDungeonBrick;
                            }
                            else if (WGTools.Tile(x, y).TileType == TileID.CrackedBlueDungeonBrick)
                            {
                                WGTools.Tile(x, y).TileType = TileID.CrackedPinkDungeonBrick;
                            }
                            if (WGTools.Tile(x, y).WallType == ModContent.WallType<dungeonblue>())
                            {
                                WGTools.Tile(x, y).WallType = (ushort)ModContent.WallType<dungeonpink>();
                            }
                            else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeonUnsafe || WGTools.Tile(x, y).WallType == WallID.BlueDungeon)
                            {
                                WGTools.Tile(x, y).WallType = WallID.PinkDungeonUnsafe;
                            }
                            else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeonSlabUnsafe)
                            {
                                WGTools.Tile(x, y).WallType = WallID.PinkDungeonSlabUnsafe;
                            }
                            else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeonTileUnsafe)
                            {
                                WGTools.Tile(x, y).WallType = WallID.PinkDungeonTileUnsafe;
                            }
                        }
                        else if (WGTools.Tile(x, y).WallType == WallID.BlueDungeon)
                        {
                            WGTools.Tile(x, y).WallType = WallID.BlueDungeonUnsafe;
                        }
                        #endregion
                    }
                }
            }

            int books = dungeon.grid.Height * dungeon.grid.Width / 7;
            while (books > 0)
            {
                int x = WorldGen.genRand.Next(dungeon.area.Left, dungeon.area.Right);
                int y = WorldGen.genRand.Next(entranceY - 18, dungeon.area.Bottom);

                Tile tile = Main.tile[x, y];
                if (tile.HasTile && tile.TileType == TileID.Books && tile.TileFrameX != 18 * 5)
                {
                    tile.TileFrameX = 18 * 5;
                    books--;
                }
            }
            #endregion
        }
        #region functions

        private void ChestLoot(int chestIndex, int objects)
        {
            // itemsToAdd will hold type and stack data for each item we want to add to the chest
            var itemsToAdd = new List<(int type, int stack)>();

            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.

            int[] specialItems = new int[6];
            specialItems[0] = ItemID.CobaltShield;
            specialItems[1] = ItemID.ShadowKey;
            specialItems[2] = ItemID.Muramasa;
            specialItems[3] = ItemID.MagicMissile;
            specialItems[4] = ItemID.BlueMoon;
            specialItems[5] = ItemID.AquaScepter;

            int specialItem = specialItems[(objects - 1) % specialItems.Length];

            itemsToAdd.Add((specialItem, 1));
            if (specialItem == ItemID.Handgun)
            {
                itemsToAdd.Add((ItemID.MusketBall, Main.rand.Next(50, 150)));
            }

            //if (objects <= 4)
            //{
            //    itemsToAdd.Add((ModContent.ItemType<ironkey>(), 1));
            //}

            Structures.GenericLoot(chestIndex, itemsToAdd, 2);

            Structures.FillChest(chestIndex, itemsToAdd);
        }

        private void BiomeChestLoot(int chestIndex, int type)
        {
            Chest chest = Main.chest[chestIndex];
            // itemsToAdd will hold type and stack data for each item we want to add to the chest
            var itemsToAdd = new List<(int type, int stack)>();

            if (type == 1)
            {
                itemsToAdd.Add((ItemID.PiranhaGun, 1));
            }
            else if (type == 2)
            {
                itemsToAdd.Add((ItemID.VampireKnives, 1));
            }
            else if (type == 3)
            {
                itemsToAdd.Add((ItemID.ScourgeoftheCorruptor, 1));
            }
            else if (type == 4)
            {
                itemsToAdd.Add((ItemID.RainbowGun, 1));
            }
            else if (type == 5)
            {
                itemsToAdd.Add((ItemID.StaffoftheFrostHydra, 1));
            }
            else if (type == 6)
            {
                itemsToAdd.Add((ItemID.StormTigerStaff, 1));
            }

            Structures.GenericLoot(chestIndex, itemsToAdd, 3, new int[] { ItemID.BiomeSightPotion });

            int chestItemIndex = 0;
            foreach (var itemToAdd in itemsToAdd)
            {
                Item item = new Item();
                item.SetDefaults(itemToAdd.type);
                item.stack = itemToAdd.stack;
                chest.item[chestItemIndex] = item;
                chestItemIndex++;
                if (chestItemIndex >= 40)
                    break;
            }
        }

        private void BarrelLoot(int chestIndex)
        {
            Chest chest = Main.chest[chestIndex];
            // itemsToAdd will hold type and stack data for each item we want to add to the chest
            var itemsToAdd = new List<(int type, int stack)>();

            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
            int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                Tuple.Create((int)ItemID.CobaltShield, 1.0),
                Tuple.Create((int)ItemID.ShadowKey, 1.0),
                Tuple.Create((int)ItemID.Muramasa, 1.0),
                Tuple.Create((int)ItemID.Handgun, 1.0),
                Tuple.Create((int)ItemID.AquaScepter, 1.0),
                Tuple.Create((int)ItemID.MagicMissile, 1.0),
                Tuple.Create((int)ItemID.BlueMoon, 1.0)
            );

            itemsToAdd.Add((ItemID.HerbBag, Main.rand.Next(1, 2)));

            itemsToAdd.Add((Structures.CommonPotion(), Main.rand.Next(1, 2)));

            int chestItemIndex = 0;
            foreach (var itemToAdd in itemsToAdd)
            {
                Item item = new Item();
                item.SetDefaults(itemToAdd.type);
                item.stack = itemToAdd.stack;
                chest.item[chestItemIndex] = item;
                chestItemIndex++;
                if (chestItemIndex >= 40)
                    break;
            }
        }

        private void PlaceLargePainting(int x, int y, int style = -1)
        {
            if (style == -1)
            {
                style = WorldGen.genRand.Next(17);
            }
            if (style == 14)
            {
                WorldGen.Place6x4Wall(x, y, TileID.Painting6X4, style: 15);
            }
            else if (style == 15)
            {
                WorldGen.Place6x4Wall(x, y, TileID.Painting6X4, style: 16);
            }
            else if (style == 16)
            {
                WorldGen.Place6x4Wall(x, y, TileID.Painting6X4, style: 30);
            }
            else WorldGen.Place6x4Wall(x, y, TileID.Painting6X4, style: style);
        }

        private void PlaceSmallPainting(int x, int y, int style = -1)
        {
            if (style == -1)
            {
                style = WorldGen.genRand.Next(7);
            }

            if (style == 0)
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 12);
            }
            else if (style == 1)
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 13);
            }
            else if (style == 2)
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 14);
            }
            else if (style == 3)
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 15);
            }
            else if (style == 4)
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 18);
            }
            else if (style == 5)
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 19);
            }
            else
            {
                WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: 23);
            }
        }
        #endregion

        private void Spikes(Structures.Dungeon dungeon, float multiplier = 1)
        {
            int num4 = 0;
            int num5 = 1000;
            int num6 = 0;
            while (num6 < dungeon.grid.Height * dungeon.grid.Width * multiplier)
            {
                num4++;
                int num7 = WorldGen.genRand.Next(dungeon.area.Left, dungeon.area.Right);
                int num8 = WorldGen.genRand.Next(dungeon.area.Top, dungeon.area.Bottom);
                int num9 = num7;
                if (Main.wallDungeon[Main.tile[num7, num8].WallType] && !Main.tile[num7, num8].HasTile)
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
                            Main.tile[num7, num8].TileType = 48;
                            if (!Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                            {
                                WGTools.Tile(num7, num8 - num10).TileType = 48;
                                WGTools.Tile(num7, num8 - num10).HasTile = true;
                            }
                            num7--;
                            num12--;
                        }
                        if (WGTools.Tile(num7, num8).TileType == TileID.WoodBlock)
                        {
                            WGTools.Tile(num7, num8).TileType = TileID.BlueDungeonBrick;
                        }
                        num12 = WorldGen.genRand.Next(5, 13);
                        num7 = num9 + 1;
                        while (SpikeValidation(num7, num8) && Main.tile[num7 + 1, num8].HasTile && Main.tile[num7, num8 + num10].HasTile && Main.tile[num7, num8].HasTile && !Main.tile[num7, num8 - num10].HasTile && num12 > 0)
                        {
                            Main.tile[num7, num8].TileType = 48;
                            if (!Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                            {
                                WGTools.Tile(num7, num8 - num10).TileType = 48;
                                WGTools.Tile(num7, num8 - num10).HasTile = true;
                            }
                            num7++;
                            num12--;
                        }
                        if (WGTools.Tile(num7, num8).TileType == TileID.WoodBlock)
                        {
                            WGTools.Tile(num7, num8).TileType = TileID.BlueDungeonBrick;
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
            for (int i = x - 4; i <= x + 4; i++)
            {
                if (i >= x - 1 && i <= x + 1)
                {
                    if (WGTools.Tile(i, y).HasTile && (WGTools.Tile(i, y).TileType == TileID.Platforms || WGTools.Tile(i, y).TileType == TileID.TrapdoorClosed || WGTools.Tile(i, y).TileType == TileID.PlanterBox))
                    {
                        return false;
                    }
                }
                if (WGTools.Tile(i, y - 1).HasTile && (WGTools.Tile(i, y - 1).TileType == TileID.ClosedDoor || WGTools.Tile(i, y - 1).TileType == ModContent.TileType<LockedIronDoor>()))
                {
                    return false;
                }
            }
            return WGTools.Solid(x, y) && WGTools.Tile(x, y).TileType != TileID.CrackedBlueDungeonBrick;
        }
    }
}