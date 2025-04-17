using Microsoft.Xna.Framework;
using static Remnants.Content.World.SecondaryBiomes;
using StructureHelper;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Remnants.Content.Items.Accessories;
using Remnants.Content.Items.Documents;
using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Tools;
using Remnants.Content.Items.Weapons;
using Remnants.Content.Biomes;
using Remnants.Content.Walls;
using Remnants.Content.Tiles;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects;
using Remnants.Content.Tiles.Plants;
using Remnants.Content.Tiles.Objects.Decoration;
using Remnants.Content.Tiles.Objects.Furniture;
using Remnants.Content.Tiles.Objects.Hazards;

namespace Remnants.Content.World
{
    public class TheDungeon : GenPass
    {
        public TheDungeon(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Dungeon");

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
                        if (Framing.GetTileSafely(entranceX + i, entranceY).HasTile && Main.tileSolid[Framing.GetTileSafely(entranceX + i, entranceY).TileType] || Framing.GetTileSafely(entranceX + i, entranceY).WallType != 0)
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
                //    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/entrance-tunnel", new Point16(entranceX, entranceY), ModContent.GetInstance<Remnants>());
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
                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/entrance-tunnel", new Point16(entranceX, j), ModContent.GetInstance<Remnants>());
            }
            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/entrance-top", new Point16(entranceX - 5, entranceY - 33), ModContent.GetInstance<Remnants>());

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

                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/entrance-bottom", dungeon.roomPos, ModContent.GetInstance<Remnants>());
            }

            dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
            dungeon.targetCell.Y = dungeon.grid.Height - 1;
            if (dungeon.AddRoom(3, 1))
            {
                dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y, 2);

                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/vault", dungeon.roomPos, ModContent.GetInstance<Remnants>());
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

                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/TheDungeon/stairwell1", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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



                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/TheDungeon/stairwell2", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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
            //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/prison", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index);

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
            //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/livingquarters-alt", dungeon.roomPos, ModContent.GetInstance<Remnants>(), index);

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

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/deathpit", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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

                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/library", index + (roomCount <= dungeon.grid.Height * dungeon.grid.Width / 54 ? 2 : 0), dungeon.roomPos, ModContent.GetInstance<Remnants>());

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

                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/prison", index, dungeon.roomPos, ModContent.GetInstance<Remnants>());

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
            #region 1x1
            roomCount = dungeon.grid.Height * dungeon.grid.Width / 27;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left + 1, dungeon.grid.Right - 1);
                dungeon.targetCell.Y = WorldGen.genRand.Next(1, dungeon.grid.Height - 1);

                bool valid = true;

                if (dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y - 1, 2) || dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y + 1, 1))
                {
                    valid = false;
                }

                if (dungeon.AddRoom(1, 1, valid))
                {
                    dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y); dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 3);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/shaft", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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

                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/TheDungeon/b1", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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

                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/TheDungeon/b2", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/TheDungeon/a1", dungeon.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (top && !bottom)
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/TheDungeon/a2", dungeon.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (top && bottom)
                            {
                                int index = WorldGen.genRand.Next(2);
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/a3", index, dungeon.roomPos, ModContent.GetInstance<Remnants>());

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
                            else StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/hallway", dungeon.roomPos, ModContent.GetInstance<Remnants>());

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
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/hallway-end", new Point16(dungeon.roomPos.X + 12, dungeon.roomPos.Y + 19), ModContent.GetInstance<Remnants>());
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
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/hallway-end", new Point16(dungeon.roomPos.X + 20, dungeon.roomPos.Y + 19), ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                    }

                    if (!dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 3))
                    {
                        if (dungeon.targetCell.X == dungeon.grid.Left && dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.targetCell.X == dungeon.grid.Left && dungeon.targetCell.Y == dungeon.grid.Height - 1 && dungeon.X > Main.maxTilesX * 0.5f)
                        {
                            WGTools.Rectangle(dungeon.area.Left - 20, dungeon.room.Bottom - 5, dungeon.roomPos.X, dungeon.room.Bottom - 1, -1);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/sideexit", 1, new Point16(dungeon.roomPos.X - 12, dungeon.roomPos.Y + 9), ModContent.GetInstance<Remnants>());
                            Vector2 pos = dungeon.roomPos.ToVector2();
                            WGTools.Terraform(new Vector2(dungeon.area.Left - 20, dungeon.room.Bottom - 3), 3.5f);

                            //if (dungeon.targetCell.Y == dungeon.grid.Height - 1 && X > Main.maxTilesX * 0.5f)
                            //{
                            //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/railnetwork/TheDungeonstation", new Point16(dungeon.roomPos.X - 35, dungeon.roomPos.Y + 11), ModContent.GetInstance<Remnants>(), 1);
                            //    Infrastructure.AddTrack(dungeon.roomPos.X - 35, dungeon.roomPos.Y + 21);
                            //}
                        }
                        if (dungeon.targetCell.X == dungeon.grid.Right - 1 && dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.targetCell.X == dungeon.grid.Right - 1 && dungeon.targetCell.Y == dungeon.grid.Height - 1 && dungeon.X < Main.maxTilesX * 0.5f)
                        {
                            WGTools.Rectangle(dungeon.room.Right - 1, dungeon.room.Bottom - 5, dungeon.area.Right + 19, dungeon.room.Bottom - 1, -1);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/sideexit", 0, new Point16(dungeon.roomPos.X + dungeon.room.Width - 1, dungeon.roomPos.Y + 9), ModContent.GetInstance<Remnants>());
                            Vector2 pos = dungeon.roomPos.ToVector2();
                            WGTools.Terraform(new Vector2(dungeon.area.Right + 19, dungeon.room.Bottom - 3), 3.5f);

                            //if (dungeon.targetCell.Y == dungeon.grid.Height - 1 && X < Main.maxTilesX * 0.5f)
                            //{
                            //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/railnetwork/TheDungeonstation", new Point16(dungeon.roomPos.X + dungeon.room.Width + 7, dungeon.roomPos.Y + 11), ModContent.GetInstance<Remnants>(), 0);
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
                Spikes(dungeon, 0.8f);

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

                        if (WGTools.Solid(x, y) && tile.TileType != TileID.Spikes && x >= dungeon.area.Left - 3 && x <= dungeon.area.Right + 3 && y >= dungeon.area.Top - 7 && y <= dungeon.area.Bottom + 8)
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


    public class Undergrowth : GenPass
    {
        public Undergrowth(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Undergrowth");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            bool devMode = false;

            #region setup
            Structures.Dungeon tree = new Structures.Dungeon(0, 0, 1 + (int)(Main.maxTilesX / 4200f * 4), (int)(Main.maxTilesY / 1200f * 2), 42, 42, 5);
            tree.X = Main.maxTilesX / 2 - tree.area.Width / 2;
            tree.Y = (int)Main.worldSurface - tree.area.Height;

            GenVars.structures.AddProtectedStructure(tree.area, 25);

            Structures.FillEdges(tree.area.Left, tree.area.Top, tree.area.Right - 1, tree.area.Bottom - 1, TileID.LivingWood, false);
            #endregion

            #region entrance
            int entranceX = tree.area.Center.X - 6;
            int entranceY = tree.Y;
            int entranceLength = 0;

            bool valid2 = false;
            while (!valid2)
            {
                entranceY -= 6;
                if (entranceY < Main.worldSurface - 30)
                {
                    int score = 0;
                    for (int i = 0; i <= 10; i++)
                    {
                        if (Framing.GetTileSafely(entranceX + i, entranceY).HasTile && Main.tileSolid[Framing.GetTileSafely(entranceX + i, entranceY).TileType] || Framing.GetTileSafely(entranceX + i, entranceY).WallType != 0)
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
                entranceLength++;
            }

            for (int i = 0; i < 3; i++)
            {
                WorldGen.TileRunner(tree.area.Center.X, entranceY - i * 12 + 3, 32, 1, TileID.Dirt, true);
                WorldGen.TileRunner(tree.area.Center.X, entranceY - i * 12 + 3, 32, 1, TileID.Dirt, true);
            }
            for (int y = entranceY - 50; y < entranceY + 20; y++)
            {
                for (int x = tree.area.Center.X - 32; x < tree.area.Center.X + 32; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == TileID.Dirt && WGTools.SurroundingTilesActive(x, y))
                    {
                        tile.WallType = WallID.DirtUnsafe;
                    }
                }
            }

            GenVars.structures.AddProtectedStructure(new Rectangle(entranceX, entranceY - 33, 10, tree.Y - entranceY + 33), 10);

            Vector2 position = new Vector2(tree.area.Center.X, entranceY - 22);
            Vector2 velocity = Vector2.Zero;
            Branch(position, velocity, 6);

            for (int j = 40; j <= entranceY; j++)
            {
                for (int i = tree.area.Center.X - 100; i <= tree.area.Center.X + 100; i++)
                {
                    Tile tile = Main.tile[i, j];

                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LivingWood)
                    {
                        bool left = WorldGen.SolidOrSlopedTile(i - 1, j) && !WorldGen.SolidOrSlopedTile(i + 1, j);
                        bool right = WorldGen.SolidOrSlopedTile(i + 1, j) && !WorldGen.SolidOrSlopedTile(i - 1, j);
                        bool top = WorldGen.SolidOrSlopedTile(i, j - 1) && !WorldGen.SolidOrSlopedTile(i, j + 1);
                        bool bottom = WorldGen.SolidOrSlopedTile(i, j + 1) && !WorldGen.SolidOrSlopedTile(i, j - 1);

                        if (bottom)
                        {
                            if (left)
                            {
                                tile.Slope = SlopeType.SlopeDownLeft;
                            }
                            else if (right)
                            {
                                tile.Slope = SlopeType.SlopeDownRight;
                            }
                        }
                        else if (top)
                        {
                            if (left)
                            {
                                tile.Slope = SlopeType.SlopeUpLeft;
                            }
                            else if (right)
                            {
                                tile.Slope = SlopeType.SlopeUpRight;
                            }
                        }
                    }
                }
            }

            int index = WorldGen.genRand.Next(2);

            WGTools.Terraform(new Vector2(entranceX - 5, entranceY - (index != 1 ? 15 : 3)), 2.5f, scaleX: 4);
            WGTools.Terraform(new Vector2(entranceX + 15, entranceY - (index == 1 ? 15 : 3)), 2.5f, scaleX: 4);

            for (int i = 1; i <= entranceLength; i++)
            {
                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/Undergrowth/entrance-tunnel", new Point16(entranceX, tree.Y - i * 6), ModContent.GetInstance<Remnants>());
            }
            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/Undergrowth/entrance-top", index, new Point16(entranceX, entranceY - 21), ModContent.GetInstance<Remnants>());
            #endregion

            #region rooms

            int roomCount;

            #region special

            #endregion

            #region standard
            int roomID = 4;
            int attempts = 0;
            while (attempts < 10000)
            {
                tree.targetCell.X = WorldGen.genRand.Next(tree.grid.Left, tree.grid.Right);
                tree.targetCell.Y = WorldGen.genRand.Next(0, tree.grid.Bottom);
                if (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y))
                {
                    bool openLeft = tree.targetCell.X > tree.grid.Left && (!tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y) || !tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y, 2));
                    bool openRight = tree.targetCell.X < tree.grid.Right - 1 && (!tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y) || !tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y, 4));
                    bool openTop = (tree.targetCell.Y > 0 || tree.targetCell.X == tree.grid.Center.X) && (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1) || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1, 3));
                    bool openBottom = !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1) || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1, 1);

                    bool closedLeft = tree.targetCell.X == tree.grid.Left || !tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y) || tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y, 2);
                    bool closedRight = tree.targetCell.X == tree.grid.Right - 1 || !tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y) || tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y, 4);
                    bool closedTop = tree.targetCell.Y == 0 && tree.targetCell.X != tree.grid.Center.X || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1) || tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1, 3);
                    bool closedBottom = !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1) || tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1, 1);

                    if (roomID == 0)
                    {
                        if (closedLeft && openRight && openTop && closedBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/ne", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 1)
                    {
                        if (closedLeft && openRight && closedTop && openBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/es", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 2)
                    {
                        if (openLeft && closedRight && closedTop && openBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/sw", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 3)
                    {
                        if (openLeft && closedRight && openTop && closedBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/nw", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 4)
                    {
                        if (openLeft && openRight && openTop && closedBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/new", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 5)
                    {
                        if (closedLeft && openRight && openTop && openBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/nes", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 6)
                    {
                        if (openLeft && openRight && closedTop && openBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); ;

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/esw", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 7)
                    {
                        if (openLeft && closedRight && openTop && openBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/nsw", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 8)
                    {
                        if (openLeft && openRight && openTop && openBottom)
                        {
                            tree.AddMarker(tree.targetCell.X, tree.targetCell.Y);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/nesw", tree.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }

                    if (tree.targetCell.Y == tree.grid.Bottom - 1)
                    {
                        WGTools.Terraform(new Vector2(tree.room.X + tree.room.Width / 2, tree.room.Bottom + 12), 3.5f, scaleX: 1, scaleY: 2);
                        for (int i = -4; i <= 4; i++)
                        {
                            WGTools.Rectangle(tree.room.X + tree.room.Width / 2 + i, tree.room.Bottom, tree.room.X + tree.room.Width / 2 + i, tree.room.Bottom + WorldGen.genRand.Next(6, 12), i < -2 || i > 2 ? TileID.LivingWood : -1, i > -4 && i < 4 ? WallID.LivingWoodUnsafe : -2);
                        }
                    }

                    if (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 9)
                {
                    roomID = 0;
                }
            }

            roomID = 0;
            attempts = 0;
            while (attempts < 10000)
            {
                tree.targetCell.X = WorldGen.genRand.Next(tree.grid.Left, tree.grid.Right);
                tree.targetCell.Y = WorldGen.genRand.Next(0, tree.grid.Bottom);
                if (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y))
                {
                    bool openLeft = tree.targetCell.X > tree.grid.Left && (!tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y) || !tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y, 2));
                    bool openRight = tree.targetCell.X < tree.grid.Right - 1 && (!tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y) || !tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y, 4));
                    bool openTop = (tree.targetCell.Y > 0 || tree.targetCell.X == tree.grid.Center.X) && (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1) || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1, 3));
                    bool openBottom = tree.targetCell.Y < tree.grid.Bottom - 1 && (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1) || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1, 1));

                    bool closedLeft = tree.targetCell.X == tree.grid.Left || !tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y) || tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y, 2);
                    bool closedRight = tree.targetCell.X == tree.grid.Right - 1 || !tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y) || tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y, 4);
                    bool closedTop = tree.targetCell.Y == 0 && tree.targetCell.X != tree.grid.Center.X || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1) || tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1, 3);
                    bool closedBottom = tree.targetCell.Y == tree.grid.Bottom - 1 || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1) || tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1, 1);

                    if (closedTop && closedBottom)
                    {
                        if (roomID == 0)
                        {
                            if (openLeft && openRight)
                            {
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/ew", tree.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 1)
                        {
                            if (closedLeft && openRight)
                            {
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/e", tree.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 2)
                        {
                            if (openLeft && closedRight)
                            {
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2);

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/w", tree.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                    }
                    if (closedLeft && closedRight)
                    {
                        if (roomID == 3)
                        {
                            if (openTop && openBottom)
                            {
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/ns", tree.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 4)
                        {
                            if (openTop && closedBottom)
                            {
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/n", tree.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 5)
                        {
                            if (closedTop && openBottom)
                            {
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);
                                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1);

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/s", tree.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    if (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 6)
                {
                    roomID = 0;
                }
            }
            #endregion

            #region filler
            //for (tree.targetCell.Y = tree.grid.Top; tree.targetCell.Y < tree.grid.Bottom; tree.targetCell.Y++)
            //{
            //    for (tree.targetCell.X = tree.grid.Left; tree.targetCell.X < tree.grid.Right; tree.targetCell.X++)
            //    {
            //        bool openLeft = tree.targetCell.X > tree.grid.Left && (!tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y) || !tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y, 2));
            //        bool openRight = tree.targetCell.X < tree.grid.Right - 1 && (!tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y) || !tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y, 4));
            //        bool openTop = (tree.targetCell.Y > 0 || tree.targetCell.X == tree.grid.Center.X) && (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1) || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1, 3));
            //        bool openBottom = tree.targetCell.Y < tree.grid.Bottom - 1 && (!tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1) || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1, 1));

            //        bool closedLeft = tree.targetCell.X == tree.grid.Left || !tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y) || tree.FindMarker(tree.targetCell.X - 1, tree.targetCell.Y, 2);
            //        bool closedRight = tree.targetCell.X == tree.grid.Right - 1 || !tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y) || tree.FindMarker(tree.targetCell.X + 1, tree.targetCell.Y, 4);
            //        bool closedTop = tree.targetCell.Y == 0 && tree.targetCell.X != tree.grid.Center.X || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1) || tree.FindMarker(tree.targetCell.X, tree.targetCell.Y - 1, 3);
            //        bool closedBottom = tree.targetCell.Y == tree.grid.Bottom - 1 || !tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1) || tree.FindMarker(tree.targetCell.X, tree.targetCell.Y + 1, 1);

            //        if (closedTop && closedBottom)
            //        {
            //            if (openLeft && openRight)
            //            {
            //                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);

            //                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/ew", tree.roomPos, ModContent.GetInstance<Remnants>());
            //            }
            //            else if (closedLeft && openRight)
            //            {
            //                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);
            //                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 4);

            //                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/e", tree.roomPos, ModContent.GetInstance<Remnants>());
            //            }
            //            else if (openLeft && closedRight)
            //            {
            //                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 1); tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 3);
            //                tree.AddMarker(tree.targetCell.X, tree.targetCell.Y, 2);

            //                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/Undergrowth/w", tree.roomPos, ModContent.GetInstance<Remnants>());
            //            }
            //        }
            //    }
            //}
            #endregion
            #endregion

            #region cleanup
            //for (int y = entranceY - tree.room.Height; y <= tree.area.Bottom; y++)
            //{
            //    for (int x = tree.area.Left; x <= tree.area.Right; x++)
            //    {
            //        if (WGTools.Tile(x, y).HasTile && (WGTools.Tile(x, y).TileType == TileID.Rope || WGTools.Tile(x, y).TileType == TileID.Platforms && WGTools.Tile(x, y + 1).TileType == TileID.Rope) || WGTools.Tile(x, y - 2).HasTile && WGTools.Tile(x, y - 2).TileType == TileID.Rope)
            //        {
            //            if (WGTools.Tile(x - 3, y).HasTile && WGTools.Tile(x - 3, y).TileType == TileID.LivingWood)
            //            {
            //                WGTools.Tile(x - 2, y).HasTile = true;
            //                WGTools.Tile(x - 2, y).TileType = TileID.LivingWood;
            //            }
            //            if (WGTools.Tile(x + 3, y).HasTile && WGTools.Tile(x + 3, y).TileType == TileID.LivingWood)
            //            {
            //                WGTools.Tile(x + 2, y).HasTile = true;
            //                WGTools.Tile(x + 2, y).TileType = TileID.LivingWood;
            //            }
            //        }
            //    }
            //}

            //for (int y = entranceY - tree.room.Height; y <= tree.area.Bottom; y++)
            //{
            //    for (int x = tree.area.Left; x <= tree.area.Right; x++)
            //    {
            //        Tile tile = Main.tile[x, y];
            //        if (tile.HasTile && tile.TileType == TileID.LivingWood && tile.WallType == WallID.LivingWoodUnsafe && WorldGen.genRand.NextBool(2) && WorldGen.SolidTile3(x, y - 1) && WorldGen.SolidTile3(x, y + 1))
            //        {
            //            if ((WorldGen.SolidTile3(x - 1, y) || WorldGen.SolidTile3(x + 1, y)) && (!WorldGen.SolidTile3(x - 1, y) || !WorldGen.SolidTile3(x + 1, y)))
            //            {
            //                tile.TileType = (ushort)ModContent.TileType<Tiles.nothing>();

            //                WGTools.Tile(x, y + 1).Slope = !WorldGen.SolidTile3(x + 1, y) ? SlopeType.SlopeDownLeft : SlopeType.SlopeDownRight;
            //                if (WorldGen.genRand.NextBool(2))
            //                {
            //                    WGTools.Tile(x, y - 1).Slope = !WorldGen.SolidTile3(x + 1, y) ? SlopeType.SlopeUpLeft : SlopeType.SlopeUpRight;
            //                }
            //            }
            //        }

            //        if (WGTools.Tile(x, y - 1).TileType == ModContent.TileType<Tiles.nothing>())
            //        {
            //            WGTools.Tile(x, y - 1).TileType = TileID.LivingWood;
            //            WGTools.Tile(x, y - 1).HasTile = false;
            //        }
            //    }
            //}

            for (int y = tree.area.Top - tree.room.Height; y <= tree.area.Bottom; y++)
            {
                for (int x = tree.area.Left; x <= tree.area.Right; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                    {
                        //if (WorldGen.genRand.NextBool(2) && WGTools.Tile(x, y + 1).TileType == TileID.Tables && WGTools.Tile(x, y + 1).TileFrameX % 54 == 18)
                        //{
                        //    WorldGen.PlaceTile(x, y, TileID.Candles, style: 14);
                        //}
                    }
                    else if (tile.TileType == TileID.LivingWood)
                    {
                        if (WGTools.Tile(x, y + 1).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.Chandeliers && WGTools.Tile(x, y + 1).TileFrameX == 18)
                        {
                            FastNoiseLite noise = new FastNoiseLite();
                            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
                            noise.SetFrequency(0.1f);
                            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
                            noise.SetFractalOctaves(3);

                            int j = y + 2;
                            while (WGTools.Tile(x, j + 1).WallType == ModContent.WallType<undergrowth>())
                            {
                                int i = x - 6;
                                if (noise.GetNoise(i, j) > 0)
                                {
                                    if (WGTools.Tile(i, j).TileType == TileID.LivingWood)
                                    {
                                        WGTools.Tile(i, j).HasTile = false;
                                    }
                                    WGTools.Tile(i, j).WallType = (ushort)ModContent.WallType<undergrowth>();
                                }
                                i = x + 6;
                                if (noise.GetNoise(i, j) > 0)
                                {
                                    if (WGTools.Tile(i, j).TileType == TileID.LivingWood)
                                    {
                                        WGTools.Tile(i, j).HasTile = false;
                                    }
                                    WGTools.Tile(i, j).WallType = (ushort)ModContent.WallType<undergrowth>();
                                }

                                j++;
                            }
                        }
                        if (!WGTools.Solid(x, y + 1) && WGTools.Tile(x, y + 1).WallType == ModContent.WallType<undergrowth>())
                        {
                            //WGTools.Tile(x, y).TileType = TileID.LeafBlock;
                            //WGTools.Tile(x, y - 1).TileType = TileID.LeafBlock;
                            //WGTools.Tile(x, y - 2).TileType = TileID.LeafBlock;
                            //if (tile.Slope == SlopeType.Solid)
                            //{
                            //    WorldGen.TileRunner(x, y, Main.rand.Next(2, 4) * 2, 1, TileID.LeafBlock, true, overRide: false);
                            //}
                            for (int k = 0; k < 3; k++)
                            {
                                WGTools.Tile(x, y - k).TileType = TileID.LeafBlock;
                                WGTools.Tile(x, y - k).WallType = (ushort)ModContent.WallType<undergrowth>();
                            }
                            if (!WGTools.Tile(x, y + 1).HasTile && WorldGen.genRand.NextBool(2))
                            {
                                WGTools.Tile(x, y).HasTile = false;
                            }
                        }
                        else if (!WGTools.Solid(x, y - 1) && WGTools.Tile(x, y - 1).WallType == ModContent.WallType<undergrowth>())
                        {
                            if (WGTools.Solid(x - 1, y + 1) && WGTools.Solid(x + 1, y + 1))
                            {
                                WGTools.Tile(x, y).TileType = TileID.Grass;
                                WGTools.Tile(x, y + 1).TileType = TileID.Dirt;
                                WGTools.Tile(x, y + 2).TileType = TileID.Dirt;
                                if (!WGTools.Tile(x, y - 1).HasTile && WorldGen.genRand.NextBool(2) && WGTools.NoDoors(x - 1, y, 3))
                                {
                                    WGTools.Tile(x, y - 1).HasTile = true;
                                    WGTools.Tile(x, y - 1).TileType = TileID.Grass;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            Structures.AddDecorations(tree.area);
            Structures.AddVariation(tree.area);

            #region objects
            if (!devMode)
            {
                int objects;

                objects = tree.grid.Width * tree.grid.Height / 2;
                int woodWand = WorldGen.genRand.Next(objects);
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(tree.area.Left, tree.area.Right);
                    int y = WorldGen.genRand.Next(tree.area.Top, tree.area.Bottom);

                    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y).WallType == ModContent.WallType<undergrowth>() && WGTools.Tile(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y, 2))
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, style: 12, notNearOtherChests: true);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            #region chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[5];
                            specialItems[0] = ItemID.ClimbingClaws;
                            specialItems[1] = ItemID.WandofSparking;
                            specialItems[2] = ItemID.BabyBirdStaff;
                            specialItems[3] = ItemID.Blowpipe;
                            specialItems[4] = ItemID.Spear;

                            int specialItem = specialItems[(objects - 1) % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));
                            if (specialItem == ItemID.Blowpipe)
                            {
                                itemsToAdd.Add((ItemID.Seed, Main.rand.Next(15, 30)));
                            }
                            if (objects - 1 == woodWand)
                            {
                                itemsToAdd.Add((ItemID.LivingWoodWand, 1));
                                itemsToAdd.Add((ItemID.LeafWand, 1));
                            }

                            itemsToAdd.Add((ItemID.CanOfWorms, Main.rand.Next(1, 3)));

                            Structures.GenericLoot(chestIndex, itemsToAdd, 1, new int[] { ItemID.BuilderPotion, ItemID.NightOwlPotion });

                            itemsToAdd.Add((ItemID.Wood, Main.rand.Next(50, 100)));

                            Structures.FillChest(chestIndex, itemsToAdd);
                            #endregion
                            objects--;
                        }
                    }
                }
            }
            #endregion
        }

        private void Branch(Vector2 position, Vector2 velocity, float radius)
        {
            float origRadius = radius;
            float splitRadius = origRadius * 0.8f;

            while (radius > splitRadius)
            {
                WGTools.Circle(position.X, position.Y, radius, TileID.LivingWood);
                WGTools.Circle(position.X, position.Y, radius - 2, -2, WallID.LivingWoodUnsafe);
                position += new Vector2(velocity.X * 4, velocity.Y) * radius / 4;
                position.Y -= radius / 2;

                WGTools.Circle(position.X - velocity.X * 2, position.Y - velocity.Y / 2 + radius / 4, radius, TileID.LivingWood);
                WGTools.Circle(position.X - velocity.X * 2, position.Y - velocity.Y / 2 + radius / 4, radius - 2, -2, WallID.LivingWoodUnsafe);

                velocity += WorldGen.genRand.NextVector2Circular(1f, 1f);
                velocity *= 0.85f;

                radius *= 0.98f;
            }
            if (radius > 1)
            {
                Branch(position, velocity.RotatedBy(MathHelper.PiOver2), radius * 0.75f);
                Branch(position, velocity.RotatedBy(-MathHelper.PiOver2), radius * 0.75f);

                Branch(position, velocity, radius * 0.5f);
            }
            else LeafBlob(position, 10);
        }

        private void LeafBlob(Vector2 position, float radius)
        {
            FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.05f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            for (int y = (int)(position.Y - radius * 2); y <= position.Y + radius * 2; y++)
            {
                for (int x = (int)(position.X - radius * 4); x <= position.X + radius * 4; x++)
                {
                    if (noise.GetNoise(x, y * 2) <= 1 - Vector2.Distance(position, new Vector2((x - position.X) / 2 + position.X, y)) / radius && WorldGen.InWorld(x, y))
                    {
                        Tile tile = WGTools.Tile(x, y);
                        if (!tile.HasTile && WGTools.Tile(x, y).WallType == 0)
                        {
                            WorldGen.PlaceTile(x, y, TileID.LeafBlock);
                        }
                    }
                }
            }
        }
    }


    public class AerialGarden : GenPass
    {
        public AerialGarden(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.AerialGarden");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            bool devMode = false;

            #region setup
            Structures.Dungeon garden = new Structures.Dungeon(0, 0, 1 + (int)(Main.maxTilesX / 4200f * 6), (int)(Main.maxTilesY / 1200f * 2), 50, 36, 2);
            garden.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.4f), (int)(Main.maxTilesX * 0.6f) - garden.area.Width); //Main.maxTilesX / 2 - garden.area.Width / 2;
            garden.Y = (int)(Main.worldSurface * 0.5f) - garden.area.Height;

            GenVars.structures.AddProtectedStructure(garden.area);

            #endregion

            #region rooms
            int roomCount;

            #region special
            roomCount = garden.grid.Width * garden.grid.Height / 8;
            while (roomCount > 0)
            {
                garden.targetCell.X = WorldGen.genRand.Next(garden.grid.Left + 1, garden.grid.Right - 1);
                garden.targetCell.Y = WorldGen.genRand.Next(0, garden.grid.Bottom - 1);

                if (garden.AddRoom(1, 2, !garden.FindMarker(garden.targetCell.X - 1, garden.targetCell.Y) && !garden.FindMarker(garden.targetCell.X + 1, garden.targetCell.Y)))
                {
                    garden.AddMarker(garden.targetCell.X, garden.targetCell.Y, 1);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/aerialgarden/1x2", garden.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount--;
                }
            }
            roomCount = garden.grid.Width * garden.grid.Height / 8;
            while (roomCount > 0)
            {
                garden.targetCell.X = WorldGen.genRand.Next(garden.grid.Left, garden.grid.Right - 1);
                garden.targetCell.Y = WorldGen.genRand.Next(0, garden.grid.Bottom);

                if (garden.AddRoom(2, 1))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/aerialgarden/common", garden.roomPos, ModContent.GetInstance<Remnants>());
                    garden.targetCell.X++;
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/aerialgarden/common", garden.roomPos, ModContent.GetInstance<Remnants>());
                    garden.targetCell.X--;

                    WGTools.Rectangle(garden.room.Right - 15, garden.roomPos.Y, garden.room.Left + 15 + garden.room.Width, garden.roomPos.Y + garden.room.Height - 1, -1, -1);

                    roomCount--;
                }
            }
            roomCount = garden.grid.Width * garden.grid.Height / 8;
            while (roomCount > 0)
            {
                garden.targetCell.X = WorldGen.genRand.Next(garden.grid.Left + 1, garden.grid.Right - 1);
                garden.targetCell.Y = WorldGen.genRand.Next(0, garden.grid.Bottom);

                if (garden.AddRoom(1, 1, !garden.FindMarker(garden.targetCell.X - 1, garden.targetCell.Y, 1) || !garden.FindMarker(garden.targetCell.X + 1, garden.targetCell.Y, 1) || WorldGen.genRand.NextBool(2)))
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/aerialgarden/1x1", 0, garden.roomPos, ModContent.GetInstance<Remnants>());

                    WorldGen.PlaceTile(garden.roomPos.X + 25, garden.roomPos.Y + 19, ModContent.TileType<Spitflower>());
                    ModContent.GetInstance<TESpitflower>().Place(garden.roomPos.X + 25, garden.roomPos.Y + 19);

                    roomCount--;
                }
            }
            roomCount = garden.grid.Width * garden.grid.Height / 8;
            while (roomCount > 0)
            {
                garden.targetCell.X = WorldGen.genRand.Next(garden.grid.Left + 1, garden.grid.Right - 1);
                garden.targetCell.Y = WorldGen.genRand.Next(0, garden.grid.Bottom);

                if (garden.AddRoom(1, 1, !garden.FindMarker(garden.targetCell.X - 1, garden.targetCell.Y, 1) || !garden.FindMarker(garden.targetCell.X + 1, garden.targetCell.Y, 1) || WorldGen.genRand.NextBool(2)))
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/aerialgarden/1x1", 1, garden.roomPos, ModContent.GetInstance<Remnants>());

                    WorldGen.PlaceTile(garden.roomPos.X + 25, garden.roomPos.Y + 19, ModContent.TileType<Spitflower>());
                    ModContent.GetInstance<TESpitflower>().Place(garden.roomPos.X + 25, garden.roomPos.Y + 19);

                    roomCount--;
                }
            }
            #endregion

            #region filler
            for (garden.targetCell.Y = garden.grid.Top; garden.targetCell.Y < garden.grid.Bottom; garden.targetCell.Y++)
            {
                for (garden.targetCell.X = garden.grid.Left; garden.targetCell.X < garden.grid.Right; garden.targetCell.X++)
                {
                    if (!garden.FindMarker((int)garden.targetCell.X, (int)garden.targetCell.Y))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/aerialgarden/common", garden.roomPos, ModContent.GetInstance<Remnants>());
                    }
                    if (garden.targetCell.X == garden.grid.Left)
                    {
                        WGTools.Rectangle(garden.room.Left, garden.roomPos.Y, garden.room.Left + 15, garden.roomPos.Y + garden.room.Height, -1, -1);
                    }
                    else if (garden.targetCell.X == garden.grid.Right - 1)
                    {
                        WGTools.Rectangle(garden.room.Right - 15, garden.roomPos.Y, garden.room.Right, garden.roomPos.Y + garden.room.Height, -1, -1);
                    }
                }
            }
            #endregion
            #endregion

            #region cleanup
            for (int y = garden.area.Top; y <= garden.area.Bottom; y++)
            {
                for (int x = garden.area.Left; x <= garden.area.Right; x++)
                {
                    if (WGTools.Tile(x, y).HasTile)
                    {
                        if (WGTools.Tile(x, y).TileType == TileID.Plants || WGTools.Tile(x, y).TileType == TileID.Plants2 || WGTools.Tile(x, y).TileType == TileID.Vines)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }
                }
            }
            #endregion

            Structures.AddErosion(garden.area, new ushort[] { TileID.Grass, TileID.Dirt }, 1, 2);
            Structures.AddVariation(garden.area);

            #region objects
            if (!devMode)
            {
                int objects;

                objects = garden.grid.Width * garden.grid.Height / 4;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(garden.area.Left, garden.area.Right);
                    int y = WorldGen.genRand.Next(garden.area.Top, garden.area.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Containers || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<RustedChest>() || Framing.GetTileSafely(x, y).LiquidAmount > 0)
                    {
                        valid = false;
                    }
                    for (int i = -1; i <= 2; i++)
                    {
                        if (!WGTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 1).TileType] || WGTools.Tile(x + i, y + 1).TileType == TileID.PlanterBox)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, 21, true, 0);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<RustedChest>())
                        {
                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[5];
                            specialItems[0] = ModContent.ItemType<SummonCrown>();
                            specialItems[1] = ModContent.ItemType<LuminousHook>();
                            specialItems[2] = ItemID.FlowerBoots;
                            specialItems[3] = ItemID.CordageGuide;
                            specialItems[4] = ItemID.Umbrella;

                            int specialItem = specialItems[(objects - 1) % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));

                            itemsToAdd.Add((ItemID.HerbBag, Main.rand.Next(1, 3)));

                            Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.ThornsPotion, ItemID.SummoningPotion });

                            Structures.FillChest(chestIndex, itemsToAdd);

                            objects--;
                        }
                    }
                }

                //objects = garden.grid.Width * garden.grid.Height;
                //while (objects > 0)
                //{
                //    int x = WorldGen.genRand.Next(garden.area.Left, garden.area.Right);
                //    int y = WorldGen.genRand.Next(garden.area.Top, garden.area.Bottom);

                //    for (; !WGTools.Tile(x, y - 1).HasTile || WGTools.Tile(x, y).HasTile; y += WGTools.Tile(x, y).HasTile ? 1 : -1)
                //    {

                //    }

                //    int length = WorldGen.genRand.Next(5, 15);

                //    bool valid = true;
                //    if (!WGTools.Solid(x, y - 1) || y <= garden.area.Top || WGTools.Tile(x, y - 1).TileType == TileID.Cloud || WGTools.Tile(x, y - 1).TileType == TileID.RichMahogany)
                //    {
                //        valid = false;
                //    }
                //    else for (int j = y + 1; j <= y + length + 4; j++)
                //        {
                //            for (int i = x - 1; i <= x + 1; i++)
                //            {
                //                if (i == x && WGTools.Tile(i, j).HasTile || WGTools.Solid(i, j) || WGTools.Tile(i, j).TileType == TileID.Rope)
                //                {
                //                    valid = false;
                //                    break;
                //                }
                //            }
                //            if (!valid) { break; }
                //        }

                //    if (valid)
                //    {
                //        //WGTools.GetTile(x, y - 1).TileType = TileID.IronBrick;
                //        for (int j = y; j < y + length; j++)
                //        {
                //            WorldGen.PlaceTile(x, j, TileID.Rope);
                //        }
                //        WorldGen.PlaceWall(x, y + length, WallID.GrassUnsafe);
                //        WorldGen.PlaceTile(x, y + length + 2, TileID.Platforms, style: 11);
                //        WorldGen.PlaceTile(x, y + length + 1, TileID.ClayPot);
                //        WorldGen.PlaceTile(x, y + length, TileID.ImmatureHerbs, style: 0);
                //        objects--;
                //    }
                //}

                //objects = roomsVertical * roomsHorizontal / 4;
                //while (objects > 0)
                //{
                //    int x = WorldGen.genRand.Next(garden.area.Left, garden.area.Right);
                //    int y = WorldGen.genRand.Next(garden.area.Top, garden.area.Bottom);

                //    bool valid = true;
                //    if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                //    {
                //        valid = false;
                //    }
                //    else if (WGTools.GetTile(x, y + 1).TileType != TileID.GrayBrick && WGTools.GetTile(x, y + 1).TileType != TileID.StoneSlab && WGTools.GetTile(x, y + 1).TileType != TileID.JungleGrass)
                //    {
                //        valid = false;
                //    }
                //    else if (WGTools.GetTile(x, y - 1).HasTile)
                //    {
                //        valid = false;
                //    }
                //    else for (int i = -1; i <= 1; i++)
                //    {
                //        if (!WGTools.GetTile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.GetTile(x + i, y + 1).TileType])
                //        {
                //            valid = false;
                //            break;
                //        }
                //    }

                //    if (valid)
                //    {
                //        WorldGen.PlaceObject(x, y, TileID.ClayPot);
                //        if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                //        {
                //            WorldGen.PlaceTile(x, y - 1, TileID.ImmatureHerbs, style: 1);
                //            objects--;
                //        }
                //    }
                //}
            }
            #endregion
        }
    }

    public class Pyramid : GenPass
    {
        public Pyramid(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Pyramid");

            #region setup
            int height = Main.maxTilesY / 300 + 1;
            Structures.Dungeon pyramid = new Structures.Dungeon(0, 0, height * 2 - 1, height, 24, 24, 2, true);
            pyramid.X = GenVars.UndergroundDesertLocation.Center.X - pyramid.area.Width / 2;
            pyramid.Y = (int)(Main.worldSurface - 10 - pyramid.area.Height);

            //for (; WGTools.Solid(pyramid.X, pyramid.Y); pyramid.Y--)
            //{

            //}
            //pyramid.Y -= 20;

            GenVars.structures.AddProtectedStructure(pyramid.area, 20);

            WGTools.Terraform(new Vector2(pyramid.area.Center.X, pyramid.area.Top - 10), 20);
            //for (int j = pyramid.area.Bottom; j < Main.worldSurface; j += 2)
            //{
            //    WGTools.Terraform(new Vector2(pyramid.area.Center.X + WorldGen.genRand.Next(-5, 6), j), 3);
            //}
            WGTools.Terraform(new Vector2(pyramid.area.Center.X, pyramid.area.Bottom + 5), 10);
            #endregion

            //FastNoiseLite noise = new FastNoiseLite();
            //noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //noise.SetFrequency(0.1f);
            //noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            //WGTools.CustomTileRunner(pyramid.area.Center.X, pyramid.area.Bottom + 4, 15, noise, -1);

            #region rooms
            for (pyramid.targetCell.Y = 0; pyramid.targetCell.Y < pyramid.grid.Height; pyramid.targetCell.Y++)
            {
                pyramid.targetCell.X = pyramid.grid.Width / 2 - pyramid.targetCell.Y - 1;
                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/pyramid/corner-left", pyramid.targetCell.Y == 0 ? 0 : 1, pyramid.roomPos, ModContent.GetInstance<Remnants>());

                pyramid.targetCell.X = pyramid.grid.Width / 2 + pyramid.targetCell.Y + 1;
                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/pyramid/corner-right", pyramid.targetCell.Y == 0 ? 0 : 1, pyramid.roomPos, ModContent.GetInstance<Remnants>());
            }

            pyramid.targetCell.X = pyramid.grid.Center.X;
            pyramid.targetCell.Y = 0;
            if (pyramid.AddRoom(1, 1))
            {
                pyramid.AddMarker(pyramid.targetCell.X, pyramid.targetCell.Y, 1);
                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/top", pyramid.roomPos, ModContent.GetInstance<Remnants>());
            }

            int roomCount;

            #region special
            pyramid.AddMarker(pyramid.grid.Center.X, pyramid.grid.Bottom - 1);

            //StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/pyramid/top", new Point16(pyramid.roomPos.X, pyramid.roomPos.Y + 6), ModContent.GetInstance<Remnants>());

            //pyramid.targetCell.X = -1;
            //pyramid.targetCell.Y = pyramid.grid.Height - 2;

            //pyramid.AddMarker(pyramid.targetCell.X, pyramid.targetCell.Y); pyramid.AddMarker(pyramid.targetCell.X + 1, pyramid.targetCell.Y); pyramid.AddMarker(pyramid.targetCell.X + 2, pyramid.targetCell.Y);
            //pyramid.AddMarker(pyramid.targetCell.X, pyramid.targetCell.Y + 1); pyramid.AddMarker(pyramid.targetCell.X + 1, pyramid.targetCell.Y + 1); pyramid.AddMarker(pyramid.targetCell.X + 2, pyramid.targetCell.Y + 1);

            //pyramid.AddMarker(pyramid.targetCell.X, pyramid.targetCell.Y, 1); pyramid.AddMarker(pyramid.targetCell.X + 1, pyramid.targetCell.Y, 1); pyramid.AddMarker(pyramid.targetCell.X + 2, pyramid.targetCell.Y, 1);

            //StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/endroom", pyramid.roomPos, ModContent.GetInstance<Remnants>());

            //roomCount = pyramid.grid.Height * pyramid.grid.Width / 64;
            //while (roomCount > 0)
            //{
            //    pyramid.targetCell.Y = WorldGen.genRand.Next(2, pyramid.grid.Height - 2);
            //    pyramid.targetCell.X = WorldGen.genRand.Next(0, pyramid.grid.Width);

            //    if (pyramid.AddRoom(1, 3))
            //    {
            //        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/trap", pyramid.roomPos, ModContent.GetInstance<Remnants>());

            //        roomCount--;
            //    }
            //}

            roomCount = pyramid.grid.Height * pyramid.grid.Width / 64;
            while (roomCount > 0)
            {
                pyramid.targetCell.Y = WorldGen.genRand.Next(4, pyramid.grid.Height - 1);
                pyramid.targetCell.X = WorldGen.genRand.Next(1, pyramid.grid.Width - 3);

                if (pyramid.AddRoom(3, 2))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/3x2", pyramid.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount--;
                }
            }

            roomCount = pyramid.grid.Height * pyramid.grid.Width / 64;
            while (roomCount > 0)
            {
                pyramid.targetCell.Y = WorldGen.genRand.Next(3, pyramid.grid.Height - 1);
                pyramid.targetCell.X = WorldGen.genRand.Next(0, pyramid.grid.Width - 1);

                int index = WorldGen.genRand.Next(2);
                bool condition = true;
                if (pyramid.FindMarker(pyramid.targetCell.X + index, pyramid.targetCell.Y + 2) && !pyramid.FindMarker(pyramid.targetCell.X + index, pyramid.targetCell.Y + 2, 1) && pyramid.targetCell.Y < pyramid.grid.Height - 1)
                {
                    condition = false;
                }
                else if (pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y - 1, 1) || pyramid.FindMarker(pyramid.targetCell.X + 1, pyramid.targetCell.Y - 1, 1))
                {
                    condition = false;
                }
                if (pyramid.AddRoom(2, 2, condition))
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/pyramid/2x2", index, pyramid.roomPos, ModContent.GetInstance<Remnants>());

                    pyramid.AddMarker(pyramid.targetCell.X + index, pyramid.targetCell.Y + 1, 1);

                    AddSmallPainting(pyramid.room.Center.X + (index == 0 ? -3 : 3) + index * pyramid.room.Width, pyramid.room.Bottom - 9);

                    roomCount--;
                }
            }

            roomCount = 0;
            while (roomCount < pyramid.grid.Height * pyramid.grid.Width / 16)
            {
                pyramid.targetCell.X = WorldGen.genRand.Next(1, pyramid.grid.Width - 1);
                if (roomCount < 3)//pyramid.grid.Height - 2)
                {
                    pyramid.targetCell.Y = WorldGen.genRand.Next(1, 3) + roomCount;
                }
                else if (roomCount == 3)
                {
                    pyramid.targetCell.Y = pyramid.grid.Height - 2;
                }
                else
                {
                    pyramid.targetCell.Y = WorldGen.genRand.Next(1, pyramid.grid.Height - 1);
                }
                bool condition = true;
                if (pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y - 1) && !pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y - 1, 1) || pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y + 1) && !pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y + 1, 1))
                {
                    condition = false;
                }
                else if ((pyramid.FindMarker(pyramid.targetCell.X - 1, pyramid.targetCell.Y, 1) || pyramid.FindMarker(pyramid.targetCell.X + 1, pyramid.targetCell.Y, 1)) && WorldGen.genRand.NextBool(5))
                {
                    condition = false;
                }
                if (pyramid.AddRoom(1, 1, condition))
                {
                    pyramid.AddMarker(pyramid.targetCell.X, pyramid.targetCell.Y, 1);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/ladder-mid", pyramid.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount++;
                }
            }

            for (pyramid.targetCell.Y = pyramid.grid.Top; pyramid.targetCell.Y < pyramid.grid.Bottom; pyramid.targetCell.Y++)
            {
                for (pyramid.targetCell.X = pyramid.grid.Left; pyramid.targetCell.X < pyramid.grid.Right; pyramid.targetCell.X++)
                {
                    if (!pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y) || pyramid.targetCell.X == pyramid.grid.Center.X && pyramid.targetCell.Y == pyramid.grid.Bottom - 1)
                    {
                        bool top = pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y - 1, 1);
                        bool bottom = pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y + 1, 1) || pyramid.targetCell.X == pyramid.grid.Center.X && pyramid.targetCell.Y == pyramid.grid.Bottom - 1;
                        if (top || bottom)
                        {
                            pyramid.AddMarker(pyramid.targetCell.X, pyramid.targetCell.Y);

                            if (top && bottom)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/ladder-mid", pyramid.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (top)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/ladder-bottom", pyramid.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (bottom)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/ladder-top", pyramid.roomPos, ModContent.GetInstance<Remnants>());

                                AddSmallPainting(pyramid.room.Center.X - 3, pyramid.room.Bottom - 9);
                                AddSmallPainting(pyramid.room.Center.X + 3, pyramid.room.Bottom - 9);
                            }
                        }
                    }
                }
            }

            //roomCount = pyramid.grid.Height * pyramid.grid.Width / 48;
            //while (roomCount > 0)
            //{
            //    pyramid.targetCell.Y = WorldGen.genRand.Next(2, pyramid.grid.Height);
            //    pyramid.targetCell.X = WorldGen.genRand.Next(0, pyramid.grid.Width - 2);

            //    if (pyramid.AddRoom(3, 1))
            //    {
            //        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/3x1", pyramid.roomPos, ModContent.GetInstance<Remnants>());

            //        for (int i = -1; i <= 1; i++)
            //        {
            //            int pos = pyramid.room.Center.X + pyramid.room.Width + WorldGen.genRand.Next(-6, 7) + i * 15;

            //            WGTools.Rectangle(pos, pyramid.room.Bottom - 6, pos, pyramid.room.Bottom - 1, ModContent.TileType<PyramidBrick>());
            //            WorldGen.PlaceTile(pos - 1, pyramid.room.Bottom - 6, TileID.Platforms, style: 42);
            //            WorldGen.PlaceTile(pos + 1, pyramid.room.Bottom - 6, TileID.Platforms, style: 42);
            //        }

            //        roomCount--;
            //    }
            //}

            roomCount = pyramid.grid.Height * pyramid.grid.Width / 24;
            while (roomCount > 0)
            {
                pyramid.targetCell.Y = WorldGen.genRand.Next(1, pyramid.grid.Height);
                pyramid.targetCell.X = WorldGen.genRand.Next(0, pyramid.grid.Width - 1);

                if (pyramid.AddRoom(2, 1))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/2x1", pyramid.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount--;
                }
            }

            //for (pyramid.targetCell.Y = 1; pyramid.targetCell.Y < pyramid.grid.Height; pyramid.targetCell.Y++)
            //{
            //    pyramid.targetCell.X = pyramid.grid.Width / 2 - pyramid.targetCell.Y;
            //    if (pyramid.AddRoom(1, 1))
            //    {
            //        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/left", pyramid.roomPos, ModContent.GetInstance<Remnants>());
            //    }

            //    pyramid.targetCell.X = pyramid.grid.Width / 2 + pyramid.targetCell.Y;
            //    if (pyramid.AddRoom(1, 1))
            //    {
            //        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/right", pyramid.roomPos, ModContent.GetInstance<Remnants>());
            //    }
            //}
            #endregion

            #region filler
            for (pyramid.targetCell.Y = pyramid.grid.Top + 1; pyramid.targetCell.Y < pyramid.grid.Bottom; pyramid.targetCell.Y++)
            {
                for (pyramid.targetCell.X = pyramid.grid.Left; pyramid.targetCell.X < pyramid.grid.Right; pyramid.targetCell.X++)
                {
                    bool marker = pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y);
                    bool marker2 = pyramid.FindMarker(pyramid.targetCell.X, pyramid.targetCell.Y + 1, 1);
                    if (!marker)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/1x1", pyramid.roomPos, ModContent.GetInstance<Remnants>());
                    }
                    //if (pyramid.targetCell.X - pyramid.targetCell.Y == pyramid.grid.Center.X || pyramid.targetCell.X + pyramid.targetCell.Y == pyramid.grid.Center.X)
                    //{
                    //    if (pyramid.targetCell.X <= pyramid.grid.Center.X)
                    //    {
                    //        WGTools.Rectangle(pyramid.room.Left, pyramid.room.Top, pyramid.room.Left + (marker2 ? 4 : 5), pyramid.room.Bottom - 1, ModContent.TileType<PyramidBrick>(), ModContent.WallType<PyramidBrickWallUnsafe>());
                    //    }
                    //    if (pyramid.targetCell.X >= pyramid.grid.Center.X)
                    //    {
                    //        WGTools.Rectangle(pyramid.room.Right - (marker2 ? 4 : 5), pyramid.room.Top, pyramid.room.Right, pyramid.room.Bottom - 1, ModContent.TileType<PyramidBrick>(), ModContent.WallType<PyramidBrickWallUnsafe>());
                    //    }
                    //}
                }
            }
            //pyramid.targetCell.X = 0;
            //pyramid.targetCell.Y = -1;
            //pyramid.roomPos.X = X + pyramid.targetCell.X * pyramid.room.Width - pyramid.room.Width / 2 + 1;
            //pyramid.roomPos.Y = Y + pyramid.targetCell.Y * pyramid.room.Height - pyramid.room.Height / 2 + 1;
            //StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/pyramid/top", new Point16(pyramid.roomPos.X, pyramid.roomPos.Y + 6), ModContent.GetInstance<Remnants>());

            //for (pyramid.targetCell.X = pyramid.grid.Left; pyramid.targetCell.X <= pyramid.grid.Right; pyramid.targetCell.X++)
            //{
            //    pyramid.targetCell.Y = pyramid.grid.Height - 1;
            //    DungeonRoom(1, 1, pyramid.room.Width, 3, pyramid.targetCell.X, pyramid.targetCell.Y + 1, tile: TileID.SmoothSandstone);
            //    DungeonRoom(1, 1, pyramid.room.Width, 2, pyramid.targetCell.X, pyramid.targetCell.Y + 1, wall: ModContent.WallType<PyramidBrickWallUnsafe>());
            //}
            #endregion

            //WGTools.Rectangle(X - 2, pyramid.area.Bottom, X + 2, pyramid.area.Bottom + 3, -1);

            //for (int k = pyramid.area.Bottom; !WGTools.Tile(pyramid.area.Center.X, k + 1).HasTile; k++)
            //{
            //    WorldGen.PlaceTile(pyramid.area.Center.X, k, TileID.Rope);
            //}
            #endregion

            //if (!devMode) { Spikes(); }

            #region objects

            int objects = pyramid.grid.Height * pyramid.grid.Width / 8;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next(pyramid.area.Left, pyramid.area.Right);
                int y = WorldGen.genRand.Next(pyramid.area.Top + 12, pyramid.area.Bottom);

                if ((WGTools.Tile(x, y).WallType == ModContent.WallType<PyramidBrickWallUnsafe>() || WGTools.Tile(x, y).WallType == ModContent.WallType<pyramid>()) && Framing.GetTileSafely(x, y).TileType != ModContent.TileType<PyramidChest>() && WGTools.Tile(x, y + 1).TileType == ModContent.TileType<PyramidBrick>() && WGTools.Tile(x + 1, y + 1).TileType == ModContent.TileType<PyramidBrick>() && WGTools.NoDoors(x, y, 2) && WGTools.Tile(x, y).LiquidAmount == 0)
                {
                    int chestIndex = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<PyramidChest>(), style: 1);
                    if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<PyramidChest>())
                    {
                        var itemsToAdd = new List<(int type, int stack)>();

                        int[] specialItems = new int[8];
                        specialItems[0] = ItemID.MagicConch;
                        specialItems[1] = ItemID.SandstorminaBottle;
                        specialItems[2] = ItemID.FlyingCarpet;
                        specialItems[3] = ItemID.MysticCoilSnake;
                        specialItems[4] = ItemID.EncumberingStone;
                        specialItems[5] = ItemID.SandBoots;
                        specialItems[6] = ItemID.ThunderStaff;
                        specialItems[7] = ItemID.ThunderSpear;

                        int specialItem = specialItems[(objects - 1) % specialItems.Length];

                        itemsToAdd.Add((specialItem, 1));

                        Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                        Structures.FillChest(chestIndex, itemsToAdd);

                        objects--;
                    }
                }
            }

            objects = pyramid.grid.Height * pyramid.grid.Width / 4;
            while (objects > 0)
            {
                Rectangle desert = GenVars.UndergroundDesertLocation;
                int x = WorldGen.genRand.Next(desert.Left, desert.Right);
                int y = WorldGen.genRand.Next(pyramid.area.Bottom + 20, desert.Bottom);

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true);

                if (biomes.FindBiome(x, y) == BiomeID.Desert && GenVars.structures.CanPlace(new Rectangle(x - 1, y - 3, 4, 5), validTiles, 5) && !WGTools.Solid(x, y - 3) && !WGTools.Solid(x + 1, y - 3))
                {
                    WorldGen.PlaceObject(x, y, (ushort)ModContent.TileType<PyramidPot>());
                    if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<PyramidPot>())
                    {
                        WGTools.Rectangle(x - 1, y + 1, x + 2, y + 1, ModContent.TileType<PyramidBrick>());

                        GenVars.structures.AddProtectedStructure(new Rectangle(x - 1, y - 3, 4, 5), 5);

                        objects--;
                    }
                }
            }

            //objects = pyramid.grid.Height * pyramid.grid.Width / 4;
            //while (objects > 0)
            //{
            //    int x = WorldGen.genRand.Next(pyramid.area.Left, pyramid.area.Right);
            //    int y = WorldGen.genRand.Next(pyramid.area.Top, pyramid.area.Bottom);

            //    bool valid = true;
            //    if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<Sarcophagus>() || WGTools.Tile(x, y).LiquidAmount == 255 || WGTools.Tile(x, y).WallType != ModContent.WallType<PyramidBrickWallUnsafe>() && WGTools.Tile(x, y).WallType != WallID.SmoothSandstone)
            //    {
            //        valid = false;
            //    }
            //    else if (WGTools.Tile(x, y + 1).TileType != TileID.SmoothSandstone && WGTools.Tile(x, y + 1).TileType != TileID.SandstoneBrick)
            //    {
            //        valid = false;
            //    }
            //    //else for (int i = -1; i <= 2; i++)
            //    //    {
            //    //        if (WGTools.GetTile(x + i, y).HasTile && WGTools.GetTile(x + i, y).TileType != ModContent.TileType<sarcophagus>())
            //    //        {
            //    //            valid = false;
            //    //            break;
            //    //        }
            //    //    }

            //    if (valid)
            //    {
            //        bool dangerous = objects <= pyramid.grid.Height * pyramid.grid.Width / 8;
            //        WorldGen.PlaceObject(x, y, ModContent.TileType<Sarcophagus>(), style: dangerous ? 4 : Main.rand.Next(3));
            //        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<Sarcophagus>())
            //        {
            //            //if (dangerous)
            //            //{
            //            //    ModContent.GetInstance<TEsarcophagus>().Place(x, y - 3);
            //            //}
            //            objects--;
            //        }
            //    }
            //}

            objects = pyramid.grid.Height * pyramid.grid.Width;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next(pyramid.area.Left, pyramid.area.Right);
                int y = WorldGen.genRand.Next(pyramid.area.Top - 12, pyramid.area.Bottom);

                bool valid = true;
                if (Framing.GetTileSafely(x, y).TileType == TileID.Pots || WGTools.Tile(x, y).WallType != ModContent.WallType<pyramid>() && WGTools.Tile(x, y).WallType != ModContent.WallType<PyramidBrickWallUnsafe>())
                {
                    valid = false;
                }
                else if (WGTools.Tile(x, y + 1).TileType == TileID.Platforms || WGTools.Tile(x, y + 1).TileType == TileID.Platforms)
                {
                    valid = false;
                }

                if (valid)
                {
                    WorldGen.PlacePot(x, y, style: Main.rand.Next(25, 28));
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Pots)
                    {
                        objects--;
                    }
                }
            }
            #endregion

            Structures.AddVariation(pyramid.area);
        }

        #region functions
        private void AddSmallPainting(int x, int y)
        {
            WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: Main.rand.Next(63, 69));
        }

        private void AddLargePainting(int x, int y)
        {
            WorldGen.Place6x4Wall(x, y, TileID.Painting6X4, style: Main.rand.Next(37, 43));
        }
        #endregion
    }

    public class ForgottenTomb : GenPass
    {
        public ForgottenTomb(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Tomb");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            bool devMode = false;

            #region setup
            Structures.Dungeon tomb = new Structures.Dungeon(0, 0, 1 + (int)(Main.maxTilesX / 4200f * 4), (int)(Main.maxTilesY / 1200f * 4), 42, 18, 3);
            tomb.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.41f), (int)(Main.maxTilesX * 0.59f) - tomb.area.Width);
            tomb.Y = Main.maxTilesY - 320 - tomb.area.Height;

            GenVars.structures.AddProtectedStructure(tomb.area, 25);

            WGTools.Terraform(new Vector2(tomb.area.Center.X, tomb.Y - 20), 30, killWall: true);

            Structures.FillEdges(tomb.area.Left - 11, tomb.area.Top, tomb.area.Right + 10, tomb.area.Bottom + 5, ModContent.TileType<TombBrick>());

            for (int j = GenVars.lavaLine; j <= tomb.Y + 1; j++)
            {
                for (int i = tomb.area.Left - 100; i < tomb.area.Right + 100; i++)
                {
                    if (biomes.FindBiome(i, j + 1) != BiomeID.Marble)
                    {
                        WGTools.Tile(i, j).LiquidAmount = 0;
                    }
                }
            }
            #endregion

            #region rooms

            tomb.targetCell.Y = tomb.grid.Bottom - 1;
            for (tomb.targetCell.X = tomb.grid.Left; tomb.targetCell.X < tomb.grid.Right; tomb.targetCell.X++)
            {
                WGTools.Rectangle(tomb.room.Left - (tomb.targetCell.X == tomb.grid.Left ? 11 : 0), tomb.room.Y + tomb.room.Height + 1, tomb.room.Right - 1 + (tomb.targetCell.X == tomb.grid.Right - 1 ? 11 : 0), tomb.room.Y + tomb.room.Height + 5, ModContent.TileType<TombBrick>());
                WGTools.Rectangle(tomb.room.Left + 1 - (tomb.targetCell.X == tomb.grid.Left ? 11 : 0), tomb.room.Y + tomb.room.Height + 1, tomb.room.Right - 2 + (tomb.targetCell.X == tomb.grid.Right - 1 ? 11 : 0), tomb.room.Y + tomb.room.Height + 4, -2, ModContent.WallType<TombBrickWallUnsafe>());
            }

            tomb.targetCell.X = tomb.grid.Center.X;
            tomb.targetCell.Y = 0;
            if (tomb.AddRoom(1, 1))
            {
                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/top", new Point16(tomb.roomPos.X, tomb.roomPos.Y - tomb.room.Height - 6), ModContent.GetInstance<Remnants>());
            }

            int roomCount;

            #region special
            roomCount = 0;
            while (roomCount < 1)
            {
                tomb.targetCell.X = WorldGen.genRand.Next(tomb.grid.Left, tomb.grid.Right - 1);
                tomb.targetCell.Y = tomb.grid.Bottom - 1;

                if (tomb.AddRoom(2, 1))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/vault", tomb.roomPos, ModContent.GetInstance<Remnants>());

                    int chestIndex;
                    var itemsToAdd = new List<(int type, int stack)>();

                    chestIndex = WorldGen.PlaceChest(tomb.roomPos.X + 22, tomb.roomPos.Y + 15, style: 24);
                    itemsToAdd = new List<(int type, int stack)>();
                    itemsToAdd.Add((ItemID.ScourgeoftheCorruptor, 1));
                    Structures.GenericLoot(chestIndex, itemsToAdd, 3, new int[] { ItemID.BiomeSightPotion });
                    Structures.FillChest(chestIndex, itemsToAdd);

                    chestIndex = WorldGen.PlaceChest(tomb.roomPos.X + 60, tomb.roomPos.Y + 15, style: 25);
                    itemsToAdd = new List<(int type, int stack)>();
                    itemsToAdd.Add((ItemID.VampireKnives, 1));
                    Structures.GenericLoot(chestIndex, itemsToAdd, 3, new int[] { ItemID.BiomeSightPotion });
                    Structures.FillChest(chestIndex, itemsToAdd);

                    roomCount++;
                }
            }

            roomCount = 0;
            while (roomCount < tomb.grid.Width * tomb.grid.Height / 6)
            {
                tomb.targetCell.X = WorldGen.genRand.Next(tomb.grid.Left, tomb.grid.Right);
                if (roomCount < tomb.grid.Height - 1)
                {
                    tomb.targetCell.Y = roomCount + 1;
                }
                else tomb.targetCell.Y = WorldGen.genRand.Next(1, tomb.grid.Bottom);

                if (!tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y) && !tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y - 1) && (tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y - 2, 1) || tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y + 2, 1) || WorldGen.genRand.NextBool(2)))
                {
                    tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y);
                    tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y - 1);
                    tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y, 1);
                    tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y - 1, 1);

                    //tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y - 1);

                    //StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/shaft-bottom", tomb.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount++;
                }
            }

            for (tomb.targetCell.Y = tomb.grid.Top; tomb.targetCell.Y < tomb.grid.Bottom; tomb.targetCell.Y++)
            {
                for (tomb.targetCell.X = tomb.grid.Left; tomb.targetCell.X < tomb.grid.Right; tomb.targetCell.X++)
                {
                    if (tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y, 1))
                    {
                        bool top = tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y - 1, 1);
                        bool bottom = tomb.FindMarker(tomb.targetCell.X, tomb.targetCell.Y + 1, 1);
                        if (top || bottom)
                        {
                            tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y);

                            if (top && bottom)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/shaft-mid", tomb.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (top)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/shaft-bottom", tomb.roomPos, ModContent.GetInstance<Remnants>());
                            }
                            else if (bottom)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/shaft-top", tomb.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                    }
                }
            }

            roomCount = 0;
            while (roomCount < tomb.grid.Width * tomb.grid.Height / 12)
            {
                tomb.targetCell.X = WorldGen.genRand.Next(tomb.grid.Left, tomb.grid.Right - 1);
                tomb.targetCell.Y = WorldGen.genRand.Next(0, tomb.grid.Bottom);

                if (tomb.AddRoom(2, 1))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/2x1", tomb.roomPos, ModContent.GetInstance<Remnants>());

                    int frame = roomCount % 3;// (roomCount < 3) ? roomCount : WorldGen.genRand.Next(3);
                    for (int j = 0; j <= 2; j++)
                    {
                        for (int i = 0; i <= 1; i++)
                        {
                            Main.tile[i + tomb.roomPos.X + 41, j + tomb.roomPos.Y + 11].TileFrameX = (short)((frame == 2 ? 1 : frame == 1 ? 2 : 4) * 36 + i * 18);
                        }
                    }

                    roomCount++;
                }
            }

            roomCount = 0;
            while (roomCount < tomb.grid.Width * tomb.grid.Height / 12)
            {
                tomb.targetCell.X = WorldGen.genRand.Next(tomb.grid.Left, tomb.grid.Right);
                tomb.targetCell.Y = roomCount == 0 && !Main.rand.NextBool(100) ? 0 : WorldGen.genRand.Next(1, tomb.grid.Bottom - 1);

                if (tomb.AddRoom(1, 2, !tomb.FindMarker(tomb.targetCell.X - 1, tomb.targetCell.Y, 2) && !tomb.FindMarker(tomb.targetCell.X + 1, tomb.targetCell.Y, 2)))
                {
                    tomb.AddMarker(tomb.targetCell.X, tomb.targetCell.Y, 2);

                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/tomb/1x2", tomb.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount++;
                }
            }

            roomCount = 0;
            while (roomCount < tomb.grid.Width * tomb.grid.Height / 12)
            {
                tomb.targetCell.X = WorldGen.genRand.Next(tomb.grid.Left, tomb.grid.Right);
                tomb.targetCell.Y = WorldGen.genRand.Next(0, tomb.grid.Bottom);

                if (tomb.AddRoom(1, 1))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/1x1", tomb.roomPos, ModContent.GetInstance<Remnants>());

                    roomCount++;
                }
            }
            #endregion

            #region filler
            for (tomb.targetCell.Y = tomb.grid.Top; tomb.targetCell.Y < tomb.grid.Bottom; tomb.targetCell.Y++)
            {
                for (tomb.targetCell.X = tomb.grid.Left; tomb.targetCell.X < tomb.grid.Right; tomb.targetCell.X++)
                {
                    if (!tomb.FindMarker((int)tomb.targetCell.X, (int)tomb.targetCell.Y))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/hallway", tomb.roomPos, ModContent.GetInstance<Remnants>());
                    }

                    if (tomb.targetCell.X == tomb.grid.Left)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/left", new Point16(tomb.roomPos.X - 11, tomb.roomPos.Y), ModContent.GetInstance<Remnants>());
                        //WGTools.Terraform(new Vector2(tomb.room.Left - 11, tomb.room.Bottom - 6), 11);
                    }
                    else if (tomb.targetCell.X == tomb.grid.Right - 1)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/tomb/right", new Point16(tomb.roomPos.X + tomb.room.Width, tomb.roomPos.Y), ModContent.GetInstance<Remnants>());
                        //WGTools.Terraform(new Vector2(tomb.room.Right + 10, tomb.room.Bottom - 6), 11);
                    }

                    if (tomb.targetCell.Y == 0 && tomb.targetCell.X != tomb.grid.Center.X)
                    {
                        WGTools.Rectangle(tomb.room.Left - (tomb.targetCell.X == tomb.grid.Left ? 11 : 0), tomb.room.Y, tomb.room.Right - 1 + (tomb.targetCell.X == tomb.grid.Right - 1 ? 11 : 0), tomb.room.Y, -2, -1);
                    }
                }
            }
            #endregion
            #endregion

            #region cleanup
            for (int y = tomb.area.Top - tomb.room.Height; y <= tomb.area.Bottom; y++)
            {
                for (int x = tomb.area.Left; x <= tomb.area.Right; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.TileType == TileID.ClosedDoor || tile.TileType == TileID.OpenDoor)
                    {
                        tile.TileFrameY += 18 * 3 * 2;
                    }
                }
            }
            #endregion

            Structures.AddDecorations(tomb.area);
            Structures.AddVariation(tomb.area);
            Structures.AddTraps(tomb.area);

            #region objects
            if (!devMode)
            {
                int objects;

                objects = tomb.grid.Width * tomb.grid.Height / 6;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(tomb.area.Left, tomb.area.Right);
                    int y = WorldGen.genRand.Next(tomb.area.Top, tomb.area.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    {
                        valid = false;
                    }
                    for (int i = -1; i <= 2; i++)
                    {
                        if (!WGTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 1).TileType])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, 21, true, 15);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            var itemsToAdd = new List<(int type, int stack)>();

                            //int[] specialItems = new int[5];
                            //specialItems[0] = ItemID.WebSlinger;
                            //specialItems[1] = ModContent.ItemType<LuminousHook>();
                            //specialItems[2] = ItemID.FlowerBoots;
                            //specialItems[3] = ItemID.CordageGuide;
                            //specialItems[4] = ItemID.Umbrella;

                            //int specialItem = specialItems[(objects - 1) % specialItems.Length];
                            itemsToAdd.Add((ItemID.WebSlinger, 1));

                            if (objects <= tomb.grid.Width * tomb.grid.Height / 12)
                            {
                                itemsToAdd.Add((ItemID.SuspiciousLookingEye, 1));
                            }

                            Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.HunterPotion, ItemID.TrapsightPotion });

                            Structures.FillChest(chestIndex, itemsToAdd);

                            objects--;
                        }
                    }
                }

                objects = tomb.grid.Width * tomb.grid.Height / 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(tomb.area.Left, tomb.area.Right);
                    int y = WorldGen.genRand.Next(tomb.area.Top, tomb.area.Bottom);

                    for (; !WGTools.Tile(x, y - 1).HasTile || WGTools.Tile(x, y).HasTile; y += WGTools.Tile(x, y).HasTile ? 1 : -1)
                    {

                    }

                    int length = WorldGen.genRand.Next(10, 30);

                    bool valid = true;
                    if (!WGTools.Solid(x, y - 1) || WGTools.Tile(x, y).WallType != ModContent.WallType<forgottentomb>())
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
                        WorldGen.PlaceTile(x, y + length, TileID.BoneBlock);
                        objects--;
                    }
                }
            }
            #endregion
        }

        private void Stalactite(int x, int y, int style)
        {
            if (Framing.GetTileSafely(x, y - 1).HasTile && Main.tileSolid[Framing.GetTileSafely(x, y - 1).TileType])
            {
                if (!Framing.GetTileSafely(x, y).HasTile && !Framing.GetTileSafely(x, y + 1).HasTile)
                {
                    Framing.GetTileSafely(x, y - 1).TileType = TileID.Stone;

                    Framing.GetTileSafely(x, y).HasTile = true;
                    Framing.GetTileSafely(x, y + 1).HasTile = true;
                    Framing.GetTileSafely(x, y).TileType = TileID.Stalactite;
                    Framing.GetTileSafely(x, y + 1).TileType = TileID.Stalactite;

                    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                    Framing.GetTileSafely(x, y + 1).TileFrameX = (short)(style * 18);
                    Framing.GetTileSafely(x, y).TileFrameY = 0;
                    Framing.GetTileSafely(x, y + 1).TileFrameY = 18;
                }
            }
        }
    }

    public class HoneySanctum : GenPass
    {
        public HoneySanctum(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            Structures.Dungeon hive = new Structures.Dungeon(0, 0, (int)(Main.maxTilesX / 4200f * 8) + 1, (int)(Main.maxTilesY / 1200f * 8) + 1, 12, 15, 2);

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.HoneySanctum");

            #region setup
            hive.X = Hive.X * biomes.scale + biomes.scale / 2 - hive.area.Width / 2;
            hive.Y = Hive.Y * biomes.scale + biomes.scale / 2 - hive.area.Height / 2;

            GenVars.structures.AddProtectedStructure(hive.area, 20);

            //for (int x = lab.area.Left - 4; x <= lab.area.Right + 4; x++)
            //{
            //    WorldGen.TileRunner(x, lab.area.Top - 4, Main.rand.Next(8, 13), 1, TileID.Hive, true);
            //    WorldGen.TileRunner(x, lab.area.Bottom + 4, Main.rand.Next(8, 13), 1, TileID.Hive, true);
            //}
            //for (int y = lab.area.Top - 4; y <= lab.area.Bottom + 4; y++)
            //{
            //    WorldGen.TileRunner(lab.area.Left - 4, y, Main.rand.Next(8, 13), 1, TileID.Hive, true);
            //    WorldGen.TileRunner(lab.area.Right + 4, y, Main.rand.Next(8, 13), 1, TileID.Hive, true);
            //}
            #endregion

            if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("honeybrick", out ModTile honeybrick) && wombatqol.TryFind("honeybrickwall", out ModWall honeybrickwall))
            {
                //WGTools.Rectangle(lab.area.Left - 4, lab.area.Top - 4, lab.area.Right + 4, lab.area.Bottom + 4, honeybrick.Type, honeybrickwall.Type, liquid: 0, liquidType: LiquidID.Honey);

                float left; float right;
                left = right = hive.area.Center.X;
                for (int y = hive.area.Top - 10; y <= hive.area.Center.Y + 6; y++)
                {
                    WGTools.Rectangle((int)left, y, (int)right, y, honeybrick.Type, honeybrickwall.Type, liquid: 0, liquidType: LiquidID.Honey);
                    WGTools.Rectangle((int)left, hive.area.Bottom + 12 - (y - hive.Y), (int)right, hive.area.Bottom + 12 - (y - hive.Y), honeybrick.Type, honeybrickwall.Type, liquid: 0, liquidType: LiquidID.Honey);

                    WorldGen.TileRunner((int)left, y, Main.rand.Next(8, 13), 1, TileID.Hive, true);
                    WorldGen.TileRunner((int)right, y, Main.rand.Next(8, 13), 1, TileID.Hive, true);
                    WorldGen.TileRunner((int)left, hive.area.Bottom + 12 - (y - hive.Y), Main.rand.Next(8, 13), 1, TileID.Hive, true);
                    WorldGen.TileRunner((int)right, hive.area.Bottom + 12 - (y - hive.Y), Main.rand.Next(8, 13), 1, TileID.Hive, true);

                    left -= hive.room.Width / (float)hive.room.Height;
                    right += hive.room.Width / (float)hive.room.Height;
                }

                #region rooms
                int roomCount;

                #region diamond
                for (hive.targetCell.Y = hive.grid.Top; hive.targetCell.Y < hive.grid.Bottom; hive.targetCell.Y++)
                {
                    for (hive.targetCell.X = hive.grid.Left; hive.targetCell.X < hive.grid.Right; hive.targetCell.X++)
                    {
                        if (hive.targetCell.X - hive.targetCell.Y > hive.grid.Center.X || hive.targetCell.X + hive.targetCell.Y < hive.grid.Center.X)
                        {
                            hive.AddMarker(hive.targetCell.X, hive.targetCell.Y);
                        }
                        else if (hive.targetCell.X - (hive.grid.Bottom - 1 - hive.targetCell.Y) > hive.grid.Center.X || hive.targetCell.X + (hive.grid.Bottom - 1 - hive.targetCell.Y) < hive.grid.Center.X)
                        {
                            hive.AddMarker(hive.targetCell.X, hive.targetCell.Y);
                        }
                    }
                }
                #endregion

                #region special
                hive.targetCell.X = hive.grid.Center.X - 2;
                hive.targetCell.Y = hive.grid.Center.Y - 1;

                if (hive.AddRoom(5, 2))
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/honeysanctum/bossroom", hive.roomPos, ModContent.GetInstance<Remnants>());

                    for (int i = hive.room.Center.X - 11; i <= hive.room.Center.X + 11; i++)
                    {
                        WorldGen.TileRunner(i + hive.room.Width * 2, hive.room.Bottom + hive.room.Height + 1, Main.rand.Next(4, 7), 1, TileID.Hive, true);
                    }
                    WGTools.PlaceObjectsInArea(hive.room.Center.X + hive.room.Width * 2 - 11, hive.room.Bottom, hive.room.Center.X + hive.room.Width * 2 + 11, hive.room.Bottom + hive.room.Height, TileID.Larva);
                }

                roomCount = hive.grid.Width * hive.grid.Height / 128;
                while (roomCount > 0)
                {
                    hive.targetCell.X = WorldGen.genRand.Next(hive.grid.Left, hive.grid.Right - 2);
                    hive.targetCell.Y = WorldGen.genRand.Next(1, hive.grid.Bottom - 3);

                    if (hive.AddRoom(3, 3, !hive.FindMarker((int)hive.targetCell.X + 1, (int)hive.targetCell.Y - 1)))
                    {
                        hive.AddMarker(hive.targetCell.X + 1, hive.targetCell.Y, 1);

                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/honeysanctum/3x3", hive.roomPos, ModContent.GetInstance<Remnants>());

                        roomCount--;
                    }
                }
                roomCount = hive.grid.Width * hive.grid.Height / 64;
                while (roomCount > 0)
                {
                    hive.targetCell.X = WorldGen.genRand.Next(hive.grid.Left, hive.grid.Right - 2);
                    hive.targetCell.Y = WorldGen.genRand.Next(0, hive.grid.Bottom - 2);

                    if (hive.AddRoom(3, 2))
                    {
                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/honeysanctum/3x2", 0, hive.roomPos, ModContent.GetInstance<Remnants>());

                        roomCount--;
                    }
                }
                roomCount = hive.grid.Width * hive.grid.Height / 64;
                while (roomCount > 0)
                {
                    hive.targetCell.X = WorldGen.genRand.Next(hive.grid.Left, hive.grid.Right - 2);
                    hive.targetCell.Y = WorldGen.genRand.Next(0, hive.grid.Bottom - 2);

                    if (hive.AddRoom(3, 2))
                    {
                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/honeysanctum/3x2", 1, hive.roomPos, ModContent.GetInstance<Remnants>());

                        roomCount--;
                    }
                }
                roomCount = hive.grid.Width * hive.grid.Height / 32;
                while (roomCount > 0)
                {
                    hive.targetCell.X = WorldGen.genRand.Next(hive.grid.Left, hive.grid.Right - 2);
                    hive.targetCell.Y = WorldGen.genRand.Next(0, hive.grid.Bottom);

                    if (hive.AddRoom(3, 1))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/honeysanctum/3x1", hive.roomPos, ModContent.GetInstance<Remnants>());

                        roomCount--;
                    }
                }

                roomCount = hive.grid.Width * hive.grid.Height / 16;
                int attempts = 0;
                while (roomCount > 0)
                {
                    hive.targetCell.X = WorldGen.genRand.Next(hive.grid.Left + 2, hive.grid.Right - 2);
                    hive.targetCell.Y = WorldGen.genRand.Next(1, hive.grid.Bottom);

                    if (attempts > 100)
                    {
                        attempts = 0;
                        roomCount--;
                    }

                    if (hive.AddRoom(1, 1, !hive.FindMarker((int)hive.targetCell.X, (int)hive.targetCell.Y - 1) || hive.FindMarker((int)hive.targetCell.X, (int)hive.targetCell.Y - 1, 1)))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/honeysanctum/ladder", hive.roomPos, ModContent.GetInstance<Remnants>());
                        hive.AddMarker(hive.targetCell.X, hive.targetCell.Y, 1);

                        roomCount--;
                    }
                    else attempts++;
                }

                #endregion

                #region filler
                for (hive.targetCell.Y = hive.grid.Top; hive.targetCell.Y < hive.grid.Bottom; hive.targetCell.Y++)
                {
                    for (hive.targetCell.X = hive.grid.Left; hive.targetCell.X < hive.grid.Right; hive.targetCell.X++)
                    {
                        if (!hive.FindMarker((int)hive.targetCell.X, (int)hive.targetCell.Y))
                        {
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/honeysanctum/hallway", hive.roomPos, ModContent.GetInstance<Remnants>());
                            //if ((lab.FindMarker((int)lab.targetCell.X, (int)lab.targetCell.Y - 1) || lab.targetCell.Y == lab.grid.Top) && (lab.FindMarker((int)lab.targetCell.X, (int)lab.targetCell.Y + 1) || lab.targetCell.Y == lab.grid.Bottom - 1))
                            //{
                            //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/honeysanctum/hallway", lab.roomPos, ModContent.GetInstance<Remnants>());
                            //}
                            //else
                            //{
                            //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/honeysanctum/shaft", lab.roomPos, ModContent.GetInstance<Remnants>(), lab.FindMarker((int)lab.targetCell.X, (int)lab.targetCell.Y - 1) ? 0 : 1);
                            //    if (lab.FindMarker((int)lab.targetCell.X, (int)lab.targetCell.Y + 1))
                            //    {
                            //        WGTools.Rectangle(lab.room.Left, lab.room.Bottom, lab.room.Right, lab.room.Bottom, honeybrick.Type, honeybrickwall.Type);
                            //    }
                            //    else
                            //    {
                            //        if (!lab.FindMarker((int)lab.targetCell.X - 1, (int)lab.targetCell.Y) || lab.targetCell.X == lab.grid.Left)
                            //        {
                            //            WGTools.Rectangle(lab.room.Left, lab.room.Top, lab.room.Left + 2, lab.room.Bottom, honeybrick.Type, honeybrickwall.Type);
                            //        }
                            //        if (!lab.FindMarker((int)lab.targetCell.X + 1, (int)lab.targetCell.Y) || lab.targetCell.X == lab.grid.Right - 1)
                            //        {
                            //            WGTools.Rectangle(lab.room.Right - 2, lab.room.Top, lab.room.Right, lab.room.Bottom, honeybrick.Type, honeybrickwall.Type);
                            //        }
                            //    }
                            //}
                        }
                        //if (lab.targetCell.X == lab.grid.Left)
                        //{
                        //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/honeysanctum/edge", new Point16(lab.roomPos.X - 3, lab.roomPos.Y), ModContent.GetInstance<Remnants>(), 1);
                        //}
                        //if (lab.targetCell.X == lab.grid.Right - 1)
                        //{
                        //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/honeysanctum/edge", new Point16(lab.roomPos.X + lab.room.Width, lab.roomPos.Y), ModContent.GetInstance<Remnants>(), 0);
                        //}

                        if (hive.FindMarker((int)hive.targetCell.X, (int)hive.targetCell.Y + 1, 1))
                        {
                            WGTools.Rectangle(hive.roomPos.X + 5, hive.room.Bottom, hive.roomPos.X + 7, hive.room.Bottom, TileID.Platforms, style: 24);
                        }
                    }
                }
                #endregion

                Structures.AddVariation(hive.area);

                #endregion

                int objects = hive.grid.Width * hive.grid.Height / 32;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(hive.area.Left, hive.area.Right);
                    int y = WorldGen.genRand.Next(hive.area.Top, hive.area.Bottom);

                    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y).WallType == ModContent.WallType<hive>() && WGTools.Tile(x + 1, y).WallType == ModContent.WallType<hive>() && WGTools.Tile(x - 1, y).TileType != TileID.ClosedDoor && WGTools.Tile(x + 2, y).TileType != TileID.ClosedDoor)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, style: 29, notNearOtherChests: true);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            var itemsToAdd = new List<(int type, int stack)>();

                            itemsToAdd.Add((ItemID.Abeemination, 1));

                            Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                            Structures.FillChest(chestIndex, itemsToAdd);

                            objects--;
                        }
                    }
                }
            }
        }
    }

    public class WaterTemple : GenPass
    {
        public WaterTemple(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.WaterTemple");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            bool devMode = false;

            #region setup
            Structures.Dungeon temple = new Structures.Dungeon(0, (int)GenVars.rockLayer, 5, (int)(Main.maxTilesY / 1200f * 4), 48, 48, 5);

            if (GenVars.dungeonSide == 1)
            {
                temple.X = Main.maxTilesX - 60 - temple.area.Width;
            }
            else
            {
                temple.X = 60;
            }

            temple.Y = (int)Main.rockLayer;

            GenVars.structures.AddProtectedStructure(temple.area, 25);

            Structures.FillEdges(temple.area.Left, temple.area.Top, temple.area.Right - 1, temple.area.Bottom - 1, ModContent.TileType<MarineSlab>(), false);
            #endregion

            #region rooms

            int roomCount;

            #region special

            #endregion

            #region standard
            int roomID = 4;
            int attempts = 0;
            while (attempts < 10000)
            {
                temple.targetCell.X = WorldGen.genRand.Next(temple.grid.Left, temple.grid.Right);
                temple.targetCell.Y = WorldGen.genRand.Next(0, temple.grid.Bottom);
                if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y))
                {
                    bool openLeft = temple.targetCell.X > temple.grid.Left && (!temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 2));
                    bool openRight = temple.targetCell.X < temple.grid.Right - 1 && (!temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y) || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y, 4));
                    bool openTop = (temple.targetCell.Y > 0 || temple.targetCell.X == temple.grid.Center.X) && (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1) || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1, 3));
                    bool openBottom = !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1) || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1);

                    bool closedLeft = temple.targetCell.X == temple.grid.Left || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) || temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 2);
                    bool closedRight = temple.targetCell.X == temple.grid.Right - 1 || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y) || temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y, 4);
                    bool closedTop = temple.targetCell.Y == 0 && temple.targetCell.X != temple.grid.Center.X || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1) || temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1, 3);
                    bool closedBottom = !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1) || temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1);

                    //if (roomID == 0)
                    //{
                    //    if (closedLeft && openRight && openTop && closedBottom)
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/ne", temple.roomPos, ModContent.GetInstance<Remnants>());
                    //    }
                    //}
                    //else if (roomID == 1)
                    //{
                    //    if (closedLeft && openRight && closedTop && openBottom)
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/es", temple.roomPos, ModContent.GetInstance<Remnants>());
                    //    }
                    //}
                    //else if (roomID == 2)
                    //{
                    //    if (openLeft && closedRight && closedTop && openBottom)
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/sw", temple.roomPos, ModContent.GetInstance<Remnants>());
                    //    }
                    //}
                    //else if (roomID == 3)
                    //{
                    //    if (openLeft && closedRight && openTop && closedBottom)
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/nw", temple.roomPos, ModContent.GetInstance<Remnants>());
                    //    }
                    //}
                    if (roomID == 4)
                    {
                        if (openLeft && openRight && openTop && closedBottom)
                        {
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/new", temple.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 5)
                    {
                        if (closedLeft && openRight && openTop && openBottom)
                        {
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/nes", temple.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 6)
                    {
                        if (openLeft && openRight && closedTop && openBottom)
                        {
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); ;

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/esw", temple.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 7)
                    {
                        if (openLeft && closedRight && openTop && openBottom)
                        {
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/nsw", temple.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    //else if (roomID == 8)
                    //{
                    //    if (openLeft && openRight && openTop && openBottom)
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y);

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/nesw", temple.roomPos, ModContent.GetInstance<Remnants>());
                    //    }
                    //}

                    if (temple.targetCell.Y == temple.grid.Bottom - 1)
                    {
                        WGTools.Terraform(new Vector2(temple.room.X + temple.room.Width / 2, temple.room.Bottom + 12), 3.5f, scaleX: 1, scaleY: 2);
                        for (int i = -4; i <= 4; i++)
                        {
                            WGTools.Rectangle(temple.room.X + temple.room.Width / 2 + i, temple.room.Bottom, temple.room.X + temple.room.Width / 2 + i, temple.room.Bottom + WorldGen.genRand.Next(6, 12), i < -2 || i > 2 ? TileID.LivingWood : -1, i > -4 && i < 4 ? WallID.LivingWoodUnsafe : -2);
                        }
                    }

                    if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 8)
                {
                    roomID = 4;
                }
            }

            //roomID = 0;
            //attempts = 0;
            //while (attempts < 10000)
            //{
            //    temple.targetCell.X = WorldGen.genRand.Next(temple.grid.Left, temple.grid.Right);
            //    temple.targetCell.Y = WorldGen.genRand.Next(0, temple.grid.Bottom);
            //    if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y))
            //    {
            //        bool openLeft = temple.targetCell.X > temple.grid.Left && (!temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 2));
            //        bool openRight = temple.targetCell.X < temple.grid.Right - 1 && (!temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y) || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y, 4));
            //        bool openTop = (temple.targetCell.Y > 0 || temple.targetCell.X == temple.grid.Center.X) && (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1) || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1, 3));
            //        bool openBottom = temple.targetCell.Y < temple.grid.Bottom - 1 && (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1) || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1));

            //        bool closedLeft = temple.targetCell.X == temple.grid.Left || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) || temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 2);
            //        bool closedRight = temple.targetCell.X == temple.grid.Right - 1 || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y) || temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y, 4);
            //        bool closedTop = temple.targetCell.Y == 0 && temple.targetCell.X != temple.grid.Center.X || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1) || temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1, 3);
            //        bool closedBottom = temple.targetCell.Y == temple.grid.Bottom - 1 || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1) || temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1);

            //        if (closedTop && closedBottom)
            //        {
            //            if (roomID == 0)
            //            {
            //                if (openLeft && openRight)
            //                {
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);

            //                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/ew", temple.roomPos, ModContent.GetInstance<Remnants>());
            //                }
            //            }
            //            else if (roomID == 1)
            //            {
            //                if (closedLeft && openRight)
            //                {
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);

            //                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/e", temple.roomPos, ModContent.GetInstance<Remnants>());
            //                }
            //            }
            //            else if (roomID == 2)
            //            {
            //                if (openLeft && closedRight)
            //                {
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

            //                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/w", temple.roomPos, ModContent.GetInstance<Remnants>());
            //                }
            //            }
            //        }
            //        if (closedLeft && closedRight)
            //        {
            //            if (roomID == 3)
            //            {
            //                if (openTop && openBottom)
            //                {
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);

            //                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/ns", temple.roomPos, ModContent.GetInstance<Remnants>());
            //                }
            //            }
            //            else if (roomID == 4)
            //            {
            //                if (openTop && closedBottom)
            //                {
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);

            //                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/n", temple.roomPos, ModContent.GetInstance<Remnants>());
            //                }
            //            }
            //            else if (roomID == 5)
            //            {
            //                if (closedTop && openBottom)
            //                {
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);
            //                    temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1);

            //                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/s", temple.roomPos, ModContent.GetInstance<Remnants>());
            //                }
            //            }
            //        }

            //        if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y))
            //        {
            //            attempts++;
            //        }
            //        else
            //        {
            //            attempts = 0;
            //        }
            //    }
            //    else attempts++;

            //    if (attempts % 100 == 0)
            //    {
            //        roomID++;
            //    }
            //    if (roomID >= 6)
            //    {
            //        roomID = 0;
            //    }
            //}
            #endregion

            #region filler
            //for (temple.targetCell.Y = temple.grid.Top; temple.targetCell.Y < temple.grid.Bottom; temple.targetCell.Y++)
            //{
            //    for (temple.targetCell.X = temple.grid.Left; temple.targetCell.X < temple.grid.Right; temple.targetCell.X++)
            //    {
            //        bool openLeft = temple.targetCell.X > temple.grid.Left && (!temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 2));
            //        bool openRight = temple.targetCell.X < temple.grid.Right - 1 && (!temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y) || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y, 4));
            //        bool openTop = (temple.targetCell.Y > 0 || temple.targetCell.X == temple.grid.Center.X) && (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1) || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1, 3));
            //        bool openBottom = temple.targetCell.Y < temple.grid.Bottom - 1 && (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1) || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1));

            //        bool closedLeft = temple.targetCell.X == temple.grid.Left || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) || temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 2);
            //        bool closedRight = temple.targetCell.X == temple.grid.Right - 1 || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y) || temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y, 4);
            //        bool closedTop = temple.targetCell.Y == 0 && temple.targetCell.X != temple.grid.Center.X || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1) || temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1, 3);
            //        bool closedBottom = temple.targetCell.Y == temple.grid.Bottom - 1 || !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1) || temple.FindMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1);

            //        if (closedTop && closedBottom)
            //        {
            //            if (openLeft && openRight)
            //            {
            //                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);

            //                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/ew", temple.roomPos, ModContent.GetInstance<Remnants>());
            //            }
            //            else if (closedLeft && openRight)
            //            {
            //                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);
            //                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 4);

            //                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/e", temple.roomPos, ModContent.GetInstance<Remnants>());
            //            }
            //            else if (openLeft && closedRight)
            //            {
            //                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 3);
            //                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

            //                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/watertemple/w", temple.roomPos, ModContent.GetInstance<Remnants>());
            //            }
            //        }
            //    }
            //}
            #endregion
            #endregion

            #region cleanup

            for (int y = temple.area.Top - temple.room.Height; y <= temple.area.Bottom; y++)
            {
                for (int x = temple.area.Left; x <= temple.area.Right; x++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.LiquidType = LiquidID.Water;
                    tile.LiquidAmount = 255;
                }
            }
            #endregion

            //Structures.AddDecorations(tree.area);
            //Structures.AddVariation(tree.area);

            #region objects
            if (!devMode)
            {
                //int objects;

                //objects = tree.grid.Width * tree.grid.Height / 2;
                //while (objects > 0)
                //{
                //    int x = WorldGen.genRand.Next(tree.area.Left, tree.area.Right);
                //    int y = WorldGen.genRand.Next(tree.area.Top, tree.area.Bottom);

                //    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y).WallType == ModContent.WallType<undergrowth>() && WGTools.Tile(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y, 2))
                //    {
                //        int chestIndex = WorldGen.PlaceChest(x, y, style: 12, notNearOtherChests: true);
                //        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                //        {
                //            #region chest
                //            var itemsToAdd = new List<(int type, int stack)>();

                //            int[] specialItems = new int[5];
                //            specialItems[0] = ItemID.ClimbingClaws;
                //            specialItems[1] = ItemID.WandofSparking;
                //            specialItems[2] = ItemID.BabyBirdStaff;
                //            specialItems[3] = ItemID.Blowpipe;
                //            specialItems[4] = ItemID.Spear;

                //            int specialItem = specialItems[(objects - 1) % specialItems.Length];
                //            itemsToAdd.Add((specialItem, 1));
                //            if (specialItem == ItemID.Blowpipe)
                //            {
                //                itemsToAdd.Add((ItemID.Seed, Main.rand.Next(15, 30)));
                //            }
                //            if (objects - 1 == woodWand)
                //            {
                //                itemsToAdd.Add((ItemID.LivingWoodWand, 1));
                //                itemsToAdd.Add((ItemID.LeafWand, 1));
                //            }

                //            itemsToAdd.Add((ItemID.CanOfWorms, Main.rand.Next(1, 3)));

                //            Structures.GenericLoot(chestIndex, itemsToAdd, 1, new int[] { ItemID.BuilderPotion, ItemID.NightOwlPotion });

                //            itemsToAdd.Add((ItemID.Wood, Main.rand.Next(50, 100)));

                //            Structures.FillChest(chestIndex, itemsToAdd);
                //            #endregion
                //            objects--;
                //        }
                //    }
                //}
            }
            #endregion
        }
    }

    public class InfernalStronghold : GenPass
    {
        public InfernalStronghold(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Stronghold");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            bool devMode = false;

            #region setup
            Structures.Dungeon stronghold = new Structures.Dungeon(0, 0, Main.maxTilesX / 420, 2, 96, 66, 3);
            stronghold.X = Main.maxTilesX / 2 - stronghold.area.Width / 2;
            stronghold.Y = Main.maxTilesY - 190;

            GenVars.structures.AddProtectedStructure(stronghold.area, 25);

            Structures.FillEdges(stronghold.area.Left, stronghold.area.Top + stronghold.room.Height, stronghold.area.Right - 1, stronghold.area.Bottom - 1, ModContent.TileType<HellishBrick>(), false);
            #endregion

            #region rooms
            for (stronghold.targetCell.X = stronghold.grid.Width / 2 - Main.maxTilesX / 2100; stronghold.targetCell.X < stronghold.grid.Width / 2 + Main.maxTilesX / 2100; stronghold.targetCell.X++)
            {
                stronghold.AddMarker(stronghold.targetCell.X, 0);
            }

            int roomID = 0;
            int attempts = 0;
            List<int> rooms = new List<int>();
            while (attempts < 10000)
            {
                if (rooms.Count == 0)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        rooms.Add(i);
                    }
                }

                roomID = rooms[WorldGen.genRand.Next(rooms.Count)];

                stronghold.targetCell.X = WorldGen.genRand.Next(stronghold.grid.Left, stronghold.grid.Right - (roomID == 5 || roomID == 6 || roomID == 7 || roomID == 8 ? 1 : 0));
                stronghold.targetCell.Y = WorldGen.genRand.Next(0, stronghold.grid.Bottom - (roomID == 1 || roomID == 2 || roomID == 3 || roomID == 4 ? 1 : 0));

                if (!stronghold.FindMarker(stronghold.targetCell.X, stronghold.targetCell.Y))
                {
                    if (roomID == 1 || roomID == 2 || roomID == 3 || roomID == 4)
                    {
                        if (!stronghold.FindMarker(stronghold.targetCell.X, stronghold.targetCell.Y + 1))
                        {
                            stronghold.AddMarker(stronghold.targetCell.X, stronghold.targetCell.Y);
                            stronghold.AddMarker(stronghold.targetCell.X, stronghold.targetCell.Y + 1);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/InfernalStronghold/1x2/" + roomID, stronghold.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 5 || roomID == 6 || roomID == 7 || roomID == 8)
                    {
                        if (!stronghold.FindMarker(stronghold.targetCell.X + 1, stronghold.targetCell.Y))
                        {
                            stronghold.AddMarker(stronghold.targetCell.X, stronghold.targetCell.Y);
                            stronghold.AddMarker(stronghold.targetCell.X + 1, stronghold.targetCell.Y);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/InfernalStronghold/2x1/" + (roomID - 4), stronghold.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }

                    if (!stronghold.FindMarker(stronghold.targetCell.X, stronghold.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        rooms.Remove(roomID);
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts > 1000)
                {
                    rooms.Remove(roomID);
                }
            }

            rooms.Clear();

            for (stronghold.targetCell.Y = stronghold.grid.Top; stronghold.targetCell.Y < stronghold.grid.Bottom; stronghold.targetCell.Y++)
            {
                for (stronghold.targetCell.X = stronghold.grid.Left; stronghold.targetCell.X < stronghold.grid.Right; stronghold.targetCell.X++)
                {
                    if (rooms.Count == 0)
                    {
                        for (int i = 1; i <= 2; i++)
                        {
                            rooms.Add(i);
                        }
                    }

                    if (!stronghold.FindMarker(stronghold.targetCell.X, stronghold.targetCell.Y))
                    {
                        roomID = rooms[WorldGen.genRand.Next(rooms.Count)];
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/InfernalStronghold/1x1/" + roomID, stronghold.roomPos, ModContent.GetInstance<Remnants>());
                        rooms.Remove(roomID);
                    }
                }
            }
            #endregion

            for (int y = stronghold.area.Top; y <= stronghold.area.Bottom; y++)
            {
                for (int x = stronghold.area.Left; x <= stronghold.area.Right; x++)
                {
                    if (WGTools.Tile(x, y).WallType == ModContent.WallType<stronghold>() || WGTools.Tile(x, y).WallType == ModContent.WallType<HellishBrickWallUnsafe>())
                    {
                        //if (WGTools.Tile(x, y).HasTile)
                        //{
                        //    if (!WGTools.Tile(x - 1, y).HasTile || !WGTools.Tile(x + 1, y).HasTile)
                        //    {
                        //        if (!WGTools.Tile(x, y - 3).HasTile && WGTools.Tile(x, y - 2).HasTile || !WGTools.Tile(x, y - 5).HasTile && WGTools.Tile(x, y - 4).HasTile)
                        //        {
                        //            WGTools.Tile(x, y).IsHalfBlock = true;
                        //        }
                        //    }
                        //}
                        //else
                        if (WGTools.Solid(x, y - 1) && (!WGTools.Solid(x - 1, y - 1) || !WGTools.Solid(x + 1, y - 1)))
                        {
                            WorldGen.PlaceObject(x, y, TileID.Banners, style: 16);
                        }
                    }
                }
            }

            //Structures.AddDecorations(stronghold.area);
            //Structures.AddVariation(stronghold.area);
            //Structures.AddTraps(stronghold.area);
            Spikes(stronghold, 3);

            #region objects
            if (!devMode)
            {
                int objects;


                objects = stronghold.grid.Width * stronghold.grid.Height * 4;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(stronghold.area.Left, stronghold.area.Right);
                    int y = WorldGen.genRand.Next(stronghold.area.Top, stronghold.area.Bottom);

                    bool valid = true;

                    for (int j = y - 2; j <= y + 2; j++)
                    {
                        for (int i = x - 2; i <= x + 2; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.HasTile || tile.LiquidAmount > 0 || tile.WallType != ModContent.WallType<HellishBrickWallUnsafe>() && i >= x - 1 && i <= x + 1 && j == y - 1)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }

                    if (valid)
                    {
                        WorldGen.Place3x3Wall(x, y, TileID.Painting3X3, style: WorldGen.genRand.Next(16, 18));

                        objects--;
                    }
                }

                objects = stronghold.grid.Width * stronghold.grid.Height;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(stronghold.area.Left, stronghold.area.Right);
                    int y = WorldGen.genRand.Next(stronghold.area.Top, stronghold.area.Bottom);

                    for (; !WGTools.Solid(x, y + 1) || WGTools.Solid(x, y); y += WGTools.Solid(x, y) ? -1 : 1)
                    {
                    }

                    bool valid = true;
                    if (WGTools.Tile(x, y).LiquidAmount > 0 || WGTools.Tile(x, y).WallType != ModContent.WallType<stronghold>() && WGTools.Tile(x, y).WallType != ModContent.WallType<HellishBrickWallUnsafe>())
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

                objects = stronghold.grid.Width * stronghold.grid.Height / 6;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(stronghold.area.Left, stronghold.area.Right);
                    int y = WorldGen.genRand.Next(stronghold.area.Top, stronghold.area.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).HasTile || Framing.GetTileSafely(x, y).TileType == TileID.Hellforge)
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y).LiquidAmount > 0 || WGTools.Tile(x, y).WallType != ModContent.WallType<stronghold>() && WGTools.Tile(x, y).WallType != ModContent.WallType<HellishBrickWallUnsafe>())
                    {
                        valid = false;
                    }
                    else for (int i = -2; i <= 2; i++)
                        {
                            if (!WGTools.Tile(x + i, y + 2).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 2).TileType])
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, TileID.Hellforge);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Hellforge)
                        {
                            objects--;
                        }
                    }
                }

                objects = stronghold.grid.Width * stronghold.grid.Height / 6;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(stronghold.area.Left, stronghold.area.Right);
                    int y = WorldGen.genRand.Next(stronghold.area.Top, stronghold.area.Bottom);

                    bool valid = true;
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    {
                        valid = false;
                    }
                    else if (WGTools.Tile(x, y).LiquidAmount > 0 || WGTools.Tile(x, y).WallType != ModContent.WallType<stronghold>() && WGTools.Tile(x, y).WallType != ModContent.WallType<HellishBrickWallUnsafe>())
                    {
                        valid = false;
                    }
                    else for (int i = -1; i <= 2; i++)
                        {
                            if (!WGTools.Tile(x + i, y + 2).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 2).TileType])
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, 21, true, 4);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[6];
                            specialItems[0] = ItemID.HellwingBow;
                            specialItems[1] = ItemID.LavaCharm;
                            specialItems[2] = ItemID.Sunfury;
                            specialItems[3] = ItemID.Flamelash;
                            specialItems[4] = ItemID.DarkLance;
                            specialItems[5] = ItemID.FlowerofFire;

                            int specialItem = specialItems[objects % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));

                            Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.ObsidianSkinPotion, ItemID.InfernoPotion });

                            Structures.FillChest(chestIndex, itemsToAdd);

                            objects--;
                        }
                    }
                }

                //objects = stronghold.grid.Width * stronghold.grid.Height / 2;
                //while (objects > 0)
                //{
                //    int x = WorldGen.genRand.Next(stronghold.area.Left, stronghold.area.Right);
                //    int y = WorldGen.genRand.Next(stronghold.area.Top, stronghold.area.Bottom);

                //    for (; !WGTools.Tile(x, y - 1).HasTile || WGTools.Tile(x, y).HasTile; y += WGTools.Tile(x, y).HasTile ? 1 : -1)
                //    {

                //    }

                //    int length = WorldGen.genRand.Next(10, 30);

                //    bool valid = true;
                //    if (!WGTools.Solid(x, y - 1) || WGTools.Tile(x, y).WallType != ModContent.WallType<Walls.Parallax.stronghold>())
                //    {
                //        valid = false;
                //    }
                //    else for (int j = y + 1; j <= y + length + 3; j++)
                //        {
                //            for (int i = x - 1; i <= x + 1; i++)
                //            {
                //                if (i == x && WGTools.Tile(i, j).HasTile || WGTools.Solid(i, j) || WGTools.Tile(i, j).TileType == TileID.Chain)
                //                {
                //                    valid = false;
                //                    break;
                //                }
                //            }
                //            if (!valid) { break; }
                //        }

                //    if (valid)
                //    {
                //        //WGTools.GetTile(x, y - 1).TileType = TileID.IronBrick;
                //        for (int j = y; j < y + length; j++)
                //        {
                //            WorldGen.PlaceTile(x, j, TileID.Chain);
                //        }
                //        WorldGen.PlaceTile(x, y + length, TileID.BoneBlock);
                //        objects--;
                //    }
                //}
            }
            #endregion
        }

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
                if (!Main.tile[num7, num8].HasTile && (Main.tile[num7, num8].WallType == ModContent.WallType<stronghold>() || Main.tile[num7, num8].WallType == ModContent.WallType<HellishBrickWallUnsafe>()))
                {
                    int num10 = 1;
                    if (num6 % 2 == 0)
                    {
                        num10 = -1;
                    }
                    for (; !Main.tile[num7, num8].HasTile; num8 += num10)
                    {
                    }
                    if (SpikeValidation(num7, num8) && Main.tile[num7, num8 - 1].LiquidAmount == 0 && Main.tile[num7 - 1, num8].HasTile && Main.tile[num7 + 1, num8].HasTile && !Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                    {
                        num6++;
                        int num12 = WorldGen.genRand.Next(4, 11);
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
                        num12 = WorldGen.genRand.Next(4, 11);
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
                    if (WGTools.Tile(i, y).HasTile && WGTools.Tile(i, y).TileType == ModContent.TileType<HellishPlatform>())
                    {
                        return false;
                    }
                }
            }
            return WGTools.Solid(x, y);
        }
    }

    public class MagicalLab : GenPass
    {
        public MagicalLab(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            Structures.Dungeon lab = new Structures.Dungeon(0, 0, 5, (int)(Main.maxTilesY / 1200f * 4), 54, 54, 5);

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.MagicalLab");

            #region setup
            if (GenVars.dungeonSide != 1)
            {
                lab.X = Main.maxTilesX - 50 - lab.area.Width;
            }
            else
            {
                lab.X = 50;
            }
            lab.Y = Main.maxTilesY - 324 - lab.area.Height;//(int)Main.rockLayer + 300 + (ModLoader.TryGetMod("ThoriumMod", out Mod mod) ? 50 : 0);

            WGTools.Rectangle(lab.area.Left - 5, lab.area.Top - 10, lab.area.Right + 5, lab.area.Bottom + 10, ModContent.TileType<EnchantedBrick>(), ModContent.WallType<EnchantedBrickWallUnsafe>(), liquid: 0, liquidType: LiquidID.Shimmer);

            Structures.FillEdges(lab.area.Left - 5, lab.area.Top - 10, lab.area.Right + 4, lab.area.Bottom + 9, ModContent.TileType<EnchantedBrick>(), false);

            for (int y = lab.area.Top - 35; y <= lab.area.Top - 10; y++)
            {
                for (int x = lab.area.Left; x <= lab.area.Right; x++)
                {
                    if (WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).TileType == TileID.Stone && !WGTools.SurroundingTilesActive(x, y))
                    {
                        WGTools.Tile(x, y).TileType = TileID.VioletMoss;
                    }
                }
            }

            GenVars.structures.AddProtectedStructure(lab.area, 20);
            #endregion

            #region rooms
            int roomCount;

            #region special
            roomCount = 0;
            while (roomCount < lab.grid.Height / 2)
            {
                lab.targetCell.X = WorldGen.genRand.Next(lab.grid.Left, lab.grid.Right);
                lab.targetCell.Y = WorldGen.genRand.Next(1, lab.grid.Bottom - 1);

                if (lab.AddRoom(condition: (!lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y) || lab.targetCell.X == 0) && (!lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y) || lab.targetCell.X == lab.grid.Right - 1) && !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1) && !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1)))
                {
                    lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/treasureroom", lab.roomPos, ModContent.GetInstance<Remnants>());

                    int chestIndex = WorldGen.PlaceChest(lab.roomPos.X + 8, lab.roomPos.Y + 41, (ushort)ModContent.TileType<ArcaneChest2>(), style: 1);
                    ChestLoot(chestIndex, roomCount * 2 + 1, Main.maxTilesY / 600 * 2);

                    chestIndex = WorldGen.PlaceChest(lab.roomPos.X + 44, lab.roomPos.Y + 41, (ushort)ModContent.TileType<ArcaneChest2>(), style: 1);
                    ChestLoot(chestIndex, roomCount * 2, Main.maxTilesY / 600 * 2);

                    roomCount++;
                }
            }

            lab.targetCell.X = lab.grid.Center.X;
            lab.targetCell.Y = lab.grid.Bottom - 1;
            if (lab.AddRoom())
            {
                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3);

                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/vault", lab.roomPos, ModContent.GetInstance<Remnants>());

                int chestIndex = WorldGen.PlaceChest(lab.roomPos.X + 26, lab.roomPos.Y + 39, style: 26);

                var itemsToAdd = new List<(int type, int stack)>();

                itemsToAdd.Add((ItemID.RainbowGun, 1));

                itemsToAdd.Add((ModContent.ItemType<ScientistNotes5>(), 1));

                Structures.GenericLoot(chestIndex, itemsToAdd, 3, new int[] { ItemID.BiomeSightPotion });

                Structures.FillChest(chestIndex, itemsToAdd);
            }
            #endregion

            #region standard
            int roomID = 4;
            int attempts = 0;
            while (attempts < 10000)
            {
                lab.targetCell.X = WorldGen.genRand.Next(lab.grid.Left, lab.grid.Right);
                lab.targetCell.Y = WorldGen.genRand.Next(0, lab.grid.Bottom);
                if (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y))
                {
                    bool openLeft = lab.targetCell.X > lab.grid.Left && (!lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y) || !lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y, 2));
                    bool openRight = lab.targetCell.X < lab.grid.Right - 1 && (!lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y) || !lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y, 4));
                    bool openTop = lab.targetCell.Y > 0 && (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1) || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1, 3));
                    bool openBottom = lab.targetCell.Y < lab.grid.Height - 1 && (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1) || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1, 1));

                    bool closedLeft = lab.targetCell.X == lab.grid.Left || !lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y) || lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y, 2);
                    bool closedRight = lab.targetCell.X == lab.grid.Right - 1 || !lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y) || lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y, 4);
                    bool closedTop = lab.targetCell.Y == 0 || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1) || lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1, 3);
                    bool closedBottom = lab.targetCell.Y == lab.grid.Height - 1 || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1) || lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1, 1);

                    if (roomID == 0)
                    {
                        if (closedLeft && openRight && openTop && closedBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/ne", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 1)
                    {
                        if (closedLeft && openRight && closedTop && openBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/es", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 2)
                    {
                        if (openLeft && closedRight && closedTop && openBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/sw", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 3)
                    {
                        if (openLeft && closedRight && openTop && closedBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/nw", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 4)
                    {
                        if (closedLeft && openRight && closedTop && closedBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/e", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 5)
                    {
                        if (openLeft && closedRight && closedTop && closedBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/w", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 6)
                    {
                        if (closedLeft && closedRight && openTop && closedBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/n", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 7)
                    {
                        if (closedLeft && closedRight && closedTop && openBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/s", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }

                    if (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 8)
                {
                    roomID = 0;
                }
            }

            roomID = 0;
            attempts = 0;
            while (attempts < 10000)
            {
                lab.targetCell.X = WorldGen.genRand.Next(lab.grid.Left, lab.grid.Right);
                lab.targetCell.Y = WorldGen.genRand.Next(0, lab.grid.Bottom);
                if (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y))
                {
                    bool openLeft = lab.targetCell.X > lab.grid.Left && (!lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y) || !lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y, 2));
                    bool openRight = lab.targetCell.X < lab.grid.Right - 1 && (!lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y) || !lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y, 4));
                    bool openTop = lab.targetCell.Y > 0 && (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1) || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1, 3));
                    bool openBottom = lab.targetCell.Y < lab.grid.Bottom - 1 && (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1) || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1, 1));

                    bool closedLeft = lab.targetCell.X == lab.grid.Left || !lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y) || lab.FindMarker(lab.targetCell.X - 1, lab.targetCell.Y, 2);
                    bool closedRight = lab.targetCell.X == lab.grid.Right - 1 || !lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y) || lab.FindMarker(lab.targetCell.X + 1, lab.targetCell.Y, 4);
                    bool closedTop = lab.targetCell.Y == 0 || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1) || lab.FindMarker(lab.targetCell.X, lab.targetCell.Y - 1, 3);
                    bool closedBottom = lab.targetCell.Y == lab.grid.Bottom - 1 || !lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1) || lab.FindMarker(lab.targetCell.X, lab.targetCell.Y + 1, 1);

                    if (openLeft & openRight)
                    {
                        if (roomID == 0)
                        {
                            if (closedTop && closedBottom)
                            {
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3);

                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/ew", lab.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 1)
                        {
                            if (openTop && closedBottom)
                            {
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 3);

                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/new", lab.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 2)
                        {
                            if (closedTop && openBottom)
                            {
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 1);

                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/esw", lab.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                    }
                    if (openTop && openBottom)
                    {
                        if (roomID == 3)
                        {
                            if (closedLeft && closedRight)
                            {
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2); lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);

                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/ns", lab.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 4)
                        {
                            if (closedLeft && openRight)
                            {
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 4);

                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/nes", lab.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                        else if (roomID == 5)
                        {
                            if (openLeft && closedRight)
                            {
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);
                                lab.AddMarker(lab.targetCell.X, lab.targetCell.Y, 2);

                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/nsw", lab.roomPos, ModContent.GetInstance<Remnants>());
                            }
                        }
                    }
                    if (roomID == 6)
                    {
                        if (openLeft && openRight && openTop && openBottom)
                        {
                            lab.AddMarker(lab.targetCell.X, lab.targetCell.Y);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/MagicalLab/nesw", lab.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }

                    if (!lab.FindMarker(lab.targetCell.X, lab.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 7)
                {
                    roomID = 0;
                }
            }
            #endregion

            lab.targetCell.X = lab.grid.Center.X;
            lab.targetCell.Y = lab.grid.Bottom - 1;
            WGTools.Rectangle(lab.roomPos.X - 2, lab.roomPos.Y + 30, lab.roomPos.X - 1, lab.roomPos.Y + 30, TileID.Platforms, style: 10);
            WGTools.Rectangle(lab.roomPos.X - 2, lab.roomPos.Y + 36, lab.roomPos.X - 1, lab.roomPos.Y + 36, TileID.Platforms, style: 10);
            WGTools.Rectangle(lab.roomPos.X + lab.room.Width, lab.roomPos.Y + 30, lab.roomPos.X + lab.room.Width + 1, lab.roomPos.Y + 30, TileID.Platforms, style: 10);
            WGTools.Rectangle(lab.roomPos.X + lab.room.Width, lab.roomPos.Y + 36, lab.roomPos.X + lab.room.Width + 1, lab.roomPos.Y + 36, TileID.Platforms, style: 10);

            #endregion

            #region cleanup
            Structures.AddDecorations(lab.area);

            for (int y = lab.area.Top; y <= lab.area.Bottom; y++)
            {
                for (int x = lab.area.Left; x <= lab.area.Right; x++)
                {
                    if (WGTools.Tile(x, y + 1).HasTile)
                    {
                        //if (WGTools.Tile(x, y + 1).TileType == TileID.Platforms)
                        //{
                        //    WorldGen.PlaceTile(x, y, WorldGen.genRand.NextBool(2) ? TileID.Bottles : TileID.Books);
                        //}
                        //else
                        if (WGTools.Tile(x, y + 1).TileType == ModContent.TileType<EnchantedBrick>() && WGTools.Tile(x + 1, y + 1).HasTile && WGTools.Tile(x + 1, y + 1).TileType == ModContent.TileType<EnchantedBrick>())
                        {
                            if (WGTools.Tile(x - 2, y).HasTile && WGTools.Tile(x - 2, y).TileType == TileID.ClosedDoor || WGTools.Tile(x + 3, y).HasTile && WGTools.Tile(x + 3, y).TileType == TileID.ClosedDoor)
                            {
                                WorldGen.PlaceObject(x + 1, y, TileID.Statues, style: 20);
                            }
                        }
                    }
                }
            }
            #endregion
        }

        private void ChestLoot(int chestIndex, int count, int countInitial)
        {
            var itemsToAdd = new List<(int type, int stack)>();

            int[] specialItems = new int[3];
            specialItems[0] = ModContent.ItemType<SpiritLance>();
            specialItems[1] = ModContent.ItemType<RingofElements>();
            specialItems[2] = ModContent.ItemType<ManaShield>();

            int specialItem = specialItems[count % specialItems.Length];
            itemsToAdd.Add((specialItem, 1));

            if (count > countInitial / 2)
            {
                itemsToAdd.Add((ModContent.ItemType<WandofRefinement>(), 1));
            }

            if (count < 6)
            {
                itemsToAdd.Add((count == 5 ? ModContent.ItemType<ScientistNotes7>() : count == 4 ? ModContent.ItemType<ScientistNotes6>() : count == 3 ? ModContent.ItemType<ScientistNotes4>() : count == 2 ? ModContent.ItemType<ScientistNotes3>() : count == 1 ? ModContent.ItemType<ScientistNotes2>() : ModContent.ItemType<ScientistNotes1>(), 1));
            }

            itemsToAdd.Add((ItemID.HerbBag, Main.rand.Next(1, 3)));

            Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.MagicPowerPotion, ItemID.ManaRegenerationPotion }, true);

            if (WorldGen.genRand.NextBool(2))
            {
                itemsToAdd.Add((ModContent.ItemType<DreamJelly>(), Main.rand.Next(5, 11)));
            }

            Structures.FillChest(chestIndex, itemsToAdd);
        }
    }

    public class Labyrinth : GenPass
    {
        public Labyrinth(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Labyrinth");

            BuildMaze();
        }

        public void BuildMaze()
        {
            Structures.Dungeon maze = new Structures.Dungeon(0, 0, (int)(1 + (float)(Main.maxTilesX / 4200f) * 12), (int)(1 + (float)(Main.maxTilesY / 1200f) * 12), 18, 18, 7);

            if (WorldGen.gen)
            {
                maze.X = GenVars.dungeonX - maze.area.Width / 2;
                maze.Y = Main.maxTilesY - 354 - maze.area.Height;

                WGTools.Terraform(new Vector2(maze.area.Center.X, maze.Y - 50), 50, killWall: true);

                //for (int i = maze.area.Left - 67; i < maze.area.Right + 67; i++)
                //{
                //    if (MathHelper.Distance(i, maze.area.Center.X) > 60)
                //    {
                //        WorldGen.TileRunner(i, maze.area.Top - 18, WorldGen.genRand.Next(6, 12), 1, TileID.Stone, true, overRide: false);
                //    }
                //    WorldGen.TileRunner(i, maze.area.Bottom + 40, WorldGen.genRand.Next(6, 18), 1, TileID.Stone, true, overRide: false);
                //}
                //for (int j = maze.area.Top - 18; j < maze.area.Bottom + 40; j += 2)
                //{
                //    WorldGen.TileRunner(maze.area.Left - 67, j, WorldGen.genRand.Next(6, 18), 1, TileID.Stone, true, overRide: false);
                //    WorldGen.TileRunner(maze.area.Right + 66, j, WorldGen.genRand.Next(6, 18), 1, TileID.Stone, true, overRide: false);
                //}

                GenVars.structures.AddProtectedStructure(maze.area, 100);
                Structures.FillEdges(maze.area.Left - 57, maze.area.Top - 18, maze.area.Right + 56, maze.area.Bottom + 40, ModContent.TileType<LabyrinthBrick>());
                WGTools.Rectangle(maze.area.Left - 57, maze.area.Top - 18, maze.area.Right + 56, maze.area.Bottom + 40, ModContent.TileType<LabyrinthBrick>(), liquid: 0);
                WGTools.Rectangle(maze.area.Left - 66, maze.area.Top - 17, maze.area.Right + 65, maze.area.Bottom + 39, wall: ModContent.WallType<LabyrinthTileWall>(), liquidType: 0);

                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/EchoingHalls/entrance", new Point16(maze.area.Center.X - 39, maze.Y - 54), ModContent.GetInstance<Remnants>());
                RemWorld.whisperingMazeX = maze.area.Center.X;
                RemWorld.whisperingMazeY = maze.Y - 36;


                //for (maze.targetCell.Y = maze.grid.Bottom - 1; maze.targetCell.Y >= -1; maze.targetCell.Y--)
                //{
                //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/side", new Point16(maze.area.Left - 67, maze.roomPos.Y), ModContent.GetInstance<Remnants>(), 0);
                //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/side", new Point16(maze.area.Right + 67 - 9, maze.roomPos.Y), ModContent.GetInstance<Remnants>(), 1);
                //    WGTools.Terraform(new Vector2(maze.area.Left - 59, maze.roomPos.Y), 15);
                //    WGTools.Terraform(new Vector2(maze.area.Right + 59, maze.roomPos.Y), 15);
                //}

                maze.targetCell.X = maze.grid.Center.X - 1;
                maze.targetCell.Y = maze.grid.Height - 4;
                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/EchoingHalls/centre", maze.roomPos, ModContent.GetInstance<Remnants>());
                for (int i = 0; i < 3; i++)
                {
                    WorldGen.PlaceObject(maze.roomPos.X + 15 + i * 12, maze.roomPos.Y + maze.room.Height * 2 - 1, ModContent.TileType<LabyrinthAltar>(), style: i);
                }

                //for (int i = 0; i < 25; i++)
                //{
                //    int x = WorldGen.genRand.Next(maze.roomPos.X + 8, maze.roomPos.X + 47);
                //    int y = WorldGen.genRand.Next(maze.roomPos.Y + maze.room.Height * 2 - 3, maze.roomPos.Y + maze.room.Height * 2);

                //    Tile tile = Main.tile[x, y];

                //    if (!tile.HasTile && (WGTools.Solid(x, y + 1) || Main.tile[x, y + 1].TileType == TileID.GoldCoinPile))
                //    {
                //        WorldGen.PlaceTile(x, y, TileID.GoldCoinPile);
                //    }
                //    else i--;
                //}
            }
            else
            {
                //maze.X = RemWorld.whisperingMazeX;
                //maze.Y = RemWorld.whisperingMazeY;

                Main.NewText("The walls are shifting...", 120, 242, 255);

                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    var player = Main.player[k];

                    if (!player.active)
                    {
                        break;
                    }
                    else if (!player.DeadOrGhost)
                    {
                        if (player.InModBiome<Biomes.EchoingHalls>())
                        {
                            player.AddBuff(BuffID.Obstructed, 60);
                            SoundEngine.PlaySound(new SoundStyle("Remnants/Content/Sounds/starspawnspellcharge"), player.Center);
                        }
                    }
                }

                for (int j = maze.area.Top; j < maze.area.Bottom; j++)
                {
                    for (int i = maze.area.Left; i < maze.area.Right; i++)
                    {
                        Tile tile = Main.tile[i, j];
                        if (tile.TileType == ModContent.TileType<LabyrinthVine>())
                        {
                            tile.HasTile = false;
                        }
                        else if (tile.TileType == ModContent.TileType<LabyrinthSpawner>() && tile.TileFrameX == 0 && tile.TileFrameY == 0)
                        {
                            ModContent.GetInstance<TEmazeguardianspawner>().Kill(i, j);
                        }
                    }
                }
            }

            maze.AddMarker(maze.grid.Center.X, 0, 1);

            maze.targetCell.X = maze.grid.Center.X - 1;
            maze.targetCell.Y = maze.grid.Height - 4;
            for (int j = (int)maze.targetCell.Y; j <= maze.targetCell.Y + 2; j++)
            {
                for (int i = (int)maze.targetCell.X; i <= maze.targetCell.X + 2; i++)
                {
                    maze.AddMarker(i, j);
                }
            }

            maze.AddMarker(maze.grid.Center.X, maze.grid.Height - 1, 1);

            maze.targetCell = new Vector2(maze.grid.Center.X, 0);
            List<Vector2> cellStack = new List<Vector2>();

            bool stop = false;
            while (!stop)
            {
                List<int> adjacencies = new List<int>();
                AdjacentCells(maze, adjacencies);

                if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y))
                {
                    maze.AddMarker(maze.targetCell.X, maze.targetCell.Y); maze.AddMarker(maze.targetCell.X, maze.targetCell.Y, 5);
                    cellStack.Add(maze.targetCell);

                    if (adjacencies.Count == 0)
                    {
                        maze.AddMarker(maze.targetCell.X, maze.targetCell.Y, 6);
                    }
                }

                if (adjacencies.Count > 0 && maze.targetCell != new Vector2(maze.grid.Center.X, maze.grid.Height - 1))
                {
                    int direction = adjacencies[Main.rand.Next(adjacencies.Count)];

                    maze.AddMarker(maze.targetCell.X, maze.targetCell.Y, direction);

                    if (direction == 1)
                    {
                        maze.targetCell.Y--;
                    }
                    else if (direction == 2)
                    {
                        maze.targetCell.X++;
                    }
                    else if (direction == 3)
                    {
                        maze.targetCell.Y++;
                    }
                    else if (direction == 4)
                    {
                        maze.targetCell.X--;
                    }

                    maze.AddMarker(maze.targetCell.X, maze.targetCell.Y, (direction + 1) % 4 + 1);
                }
                else if (cellStack.Count > 0)
                {
                    //AddMarker(maze.targetCell.X, maze.targetCell.Y); AddMarker(maze.targetCell.X, maze.targetCell.Y, 5);
                    //cellStack.RemoveAt(cellStack.IndexOf(maze.targetCell));
                    cellStack.RemoveAt(cellStack.Count - 1);

                    if (cellStack.Count > 0)
                    {
                        //stackmaze.targetCell = WorldGen.genRand.Next(cellStack.Count);
                        maze.targetCell = cellStack[cellStack.Count - 1];
                    }
                    else stop = true;
                }
            }

            for (maze.targetCell.Y = maze.grid.Top; maze.targetCell.Y < maze.grid.Bottom; maze.targetCell.Y++)
            {
                for (maze.targetCell.X = maze.grid.Left; maze.targetCell.X <= maze.grid.Right; maze.targetCell.X++)
                {
                    if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y) && maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 5))
                    {
                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/room", maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6) ? 1 : 0, maze.roomPos, ModContent.GetInstance<Remnants>());

                        if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 1))
                        {
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/ladder", maze.targetCell == new Vector2(maze.grid.Center.X, maze.grid.Top) || maze.targetCell == new Vector2(maze.grid.Center.X, maze.grid.Height - 1) ? 1 : 0, new Point16(maze.room.X + 4, maze.room.Y), ModContent.GetInstance<Remnants>());
                        }

                        if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 2))
                        {
                            WGTools.Rectangle(maze.room.Right - 3, maze.room.Top + 4, maze.room.Right, maze.room.Bottom, ModContent.TileType<LabyrinthBrick>());
                        }
                        if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 4))
                        {
                            WGTools.Rectangle(maze.room.Left, maze.room.Top + 4, maze.room.Left + 3, maze.room.Bottom, ModContent.TileType<LabyrinthBrick>());
                        }

                        if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6))
                        {
                            WGTools.Rectangle(maze.room.Left + 8, maze.room.Top + 1, maze.room.Left + 10, maze.room.Bottom, wall: ModContent.WallType<LabyrinthTileWall>());
                            WGTools.Rectangle(maze.room.Left + 8, maze.room.Top, maze.room.Left + 10, maze.room.Top, wall: ModContent.WallType<LabyrinthBrickWall>());

                            WGTools.Tile(maze.room.Left + 9, maze.room.Top + 2).WallType = (ushort)ModContent.WallType<LabyrinthBrickWall>();
                            WGTools.Tile(maze.room.Left + 9, maze.room.Top + 4).WallType = (ushort)ModContent.WallType<LabyrinthBrickWall>();
                            WGTools.Tile(maze.room.Left + 9, maze.room.Top + 6).WallType = (ushort)ModContent.WallType<LabyrinthBrickWall>();

                            WorldGen.PlaceObject(maze.room.Center.X - 1, maze.room.Top + 8, ModContent.TileType<LabyrinthSpawner>());
                            ModContent.GetInstance<TEmazeguardianspawner>().Place(maze.room.Center.X - 1, maze.room.Top + 8);
                        }
                    }
                }
            }

            int roomCount = maze.grid.Width * maze.grid.Height / 16;
            while (roomCount > 0)
            {
                maze.targetCell.X = WorldGen.genRand.Next(maze.grid.Left + 1, maze.grid.Right);
                maze.targetCell.Y = WorldGen.genRand.Next(maze.grid.Top, maze.grid.Bottom);

                if (maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y, 5) && maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 5) && !maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y, 6) && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6))
                {
                    if (!maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y, 2) && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 4))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/EchoingHalls/doorway", new Point16(maze.roomPos.X - 3, maze.roomPos.Y + 12), ModContent.GetInstance<Remnants>());
                        roomCount--;
                    }
                }
            }

            for (int y = WorldGen.gen ? maze.area.Top - 55 : maze.area.Top; y < maze.area.Bottom; y++)
            {
                for (int x = maze.area.Left; x < maze.area.Right; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                    {
                        if (!WorldGen.genRand.NextBool(4))
                        {
                            if (RemTile.SolidTop(x, y + 1))
                            {
                                if (Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<LabyrinthBrick>() || Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<LabyrinthPlatform>())
                                {
                                    int style = Main.rand.Next(6);
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<LabyrinthGrass>());
                                    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                                }
                            }
                            else if (Main.tile[x, y - 1].HasTile)
                            {
                                if (Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<LabyrinthBrick>() || Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<LabyrinthVine>())
                                {
                                    int style = Main.rand.Next(3);
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<LabyrinthVine>());
                                    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                                }
                            }
                        }
                        if (!WorldGen.gen)
                        {
                            WorldGen.SquareWallFrame(x, y);
                        }
                    }
                    else if (!WorldGen.gen)
                    {
                        WorldGen.TileFrame(x, y);
                    }
                }
            }
        }

        private void AdjacentCells(Structures.Dungeon maze, List<int> adjacencies)
        {
            if (maze.targetCell.Y > maze.grid.Top && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y - 1))
            {
                adjacencies.Add(1);
            }
            if (maze.targetCell.X < maze.grid.Right - 1 && !maze.FindMarker(maze.targetCell.X + 1, maze.targetCell.Y))
            {
                adjacencies.Add(2);
            }
            if (maze.targetCell.Y < maze.grid.Height - 1 && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y + 1))
            {
                adjacencies.Add(3);
            }
            if (maze.targetCell.X > maze.grid.Left && !maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y))
            {
                adjacencies.Add(4);
            }
        }
    }
}
