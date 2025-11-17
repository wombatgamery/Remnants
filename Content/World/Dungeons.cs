using Microsoft.Xna.Framework;
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
using static Remnants.Content.World.RemWorld;
using static Remnants.Content.World.BiomeMap;
using Remnants.Content.Items.Accessories;
using Remnants.Content.Items.Documents;
using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Tools;
using Remnants.Content.Items.Weapons;
using Remnants.Content.Walls;
using Remnants.Content.Tiles;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects;
using Remnants.Content.Tiles.Plants;
using Remnants.Content.Tiles.Objects.Decoration;
using Remnants.Content.Tiles.Objects.Furniture;
using Remnants.Content.Tiles.Objects.Hazards;
using static Remnants.Content.World.BiomeLocations;

namespace Remnants.Content.World
{
    public class DungeonPasses : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            InsertPass(tasks, new TheDungeon("Dungeon", 100), FindIndex(tasks, "Dungeon"), true);

            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (genIndex != -1)
            {
                //InsertPass(tasks, new ForgottenTomb("Forgotten Tomb", 100), genIndex + 1);

                if (ModContent.GetInstance<Worldgen>().ExperimentalWorldgen)
                {
                    InsertPass(tasks, new InfernalStronghold("Infernal Stronghold", 100), genIndex + 1);
                    InsertPass(tasks, new WaterTemple("Water Temple", 100), genIndex + 1);
                }

                InsertPass(tasks, new MagicalLab("Magical Lab", 0), genIndex + 1);
                InsertPass(tasks, new Labyrinth("Echoing Halls", 0), genIndex + 1);
            }

            InsertPass(tasks, new JungleTemple("Jungle Pyramid", 100), FindIndex(tasks, "Jungle Temple"), true);
            RemovePass(tasks, FindIndex(tasks, "Temple"));
            RemovePass(tasks, FindIndex(tasks, "Altars"));

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (genIndex != -1 && ModContent.GetInstance<Worldgen>().ExperimentalWorldgen)
            {
                tasks.Insert(genIndex + 1, new Vault("The Vault", 0));
            }
        }
    }

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
            StructureTools.Dungeon dungeon = new StructureTools.Dungeon(0, (int)Main.worldSurface + 30, (int)(MiscTools.GetSafeWorldScale() * 8) + 1, (int)(MiscTools.GetSafeWorldScale() * 6), 35, 30, 5);

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

            StructureTools.FillEdges(dungeon.area.Left - 8, dungeon.area.Top - 1, dungeon.area.Right + 7, dungeon.area.Bottom + 12, TileID.BlueDungeonBrick, false);

            MiscTools.Rectangle(dungeon.area.Left - 8, dungeon.area.Top - 1, dungeon.area.Right + 7, dungeon.area.Bottom + 12, TileID.BlueDungeonBrick, liquid: 0);
            MiscTools.Rectangle(dungeon.area.Left - 7, dungeon.area.Top, dungeon.area.Right + 6, dungeon.area.Bottom + 11, wall: WallID.BlueDungeonUnsafe);
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

            StructureTools.FillEdges(entranceX, entranceY + 80, entranceX + 34, dungeon.area.Top, TileID.BlueDungeonBrick, false);
            StructureTools.FillEdges(entranceX - stairs - 24, entranceY + stairs + 1, entranceX + 34 + stairs + 24, entranceY + stairs + 21, TileID.BlueDungeonBrick, false);

            for (int i = 1; i <= stairs; i++)
            {
                MiscTools.Rectangle(entranceX - 5 - i, entranceY + 1 + i, entranceX, entranceY + 1 + i, TileID.BlueDungeonBrick);
                MiscTools.Rectangle(entranceX + 35, entranceY + 1 + i, entranceX + 39 + i, entranceY + 1 + i, TileID.BlueDungeonBrick);
            }
            MiscTools.Rectangle(entranceX - stairs - 24, entranceY + stairs + 1, entranceX, entranceY + stairs + 21, TileID.BlueDungeonBrick);
            MiscTools.Rectangle(entranceX + 35, entranceY + stairs + 1, entranceX + 35 + stairs + 24, entranceY + stairs + 21, TileID.BlueDungeonBrick);

            //WGTools.Rectangle(entranceX - scale - 24, entranceY - 27, entranceX + scale + 60, entranceY + scale + 21, liquid: 0);

            MiscTools.Terraform(new Vector2(entranceX, entranceY), 23, killWall: true);
            MiscTools.Terraform(new Vector2(entranceX + 35, entranceY), 23, killWall: true);

            for (int j = dungeon.Y; j > entranceY + 1; j -= 6)
            {
                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/entrance-tunnel", new Point16(entranceX, j), ModContent.GetInstance<Remnants>());
            }
            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/entrance-top", new Point16(entranceX - 5, entranceY - 33), ModContent.GetInstance<Remnants>());

            for (int i = 1; !MiscTools.Tile(entranceX - 5 - i, entranceY + i).HasTile; i++)
            {
                WorldGen.PlaceTile(entranceX - 5 - i, entranceY + i, TileID.Platforms, style: 6);
                MiscTools.Tile(entranceX - 5 - i, entranceY + i).Slope = SlopeType.SlopeDownRight;
            }
            for (int i = 1; !MiscTools.Tile(entranceX + 39 + i, entranceY + i).HasTile; i++)
            {
                WorldGen.PlaceTile(entranceX + 39 + i, entranceY + i, TileID.Platforms, style: 6);
                MiscTools.Tile(entranceX + 39 + i, entranceY + i).Slope = SlopeType.SlopeDownLeft;
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

            int roomCount;

            #region special

            #region vaults
            roomCount = 1;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
                dungeon.targetCell.Y = dungeon.grid.Height - 1;
                if (dungeon.AddRoom(3, 1))
                {
                    dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                    dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y, 2);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/vault1", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                    int chestIndex;

                    chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 36, dungeon.roomPos.Y + 23, TileID.Containers2, style: 13);
                    BiomeChestLoot(chestIndex, 6);

                    chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 50, dungeon.roomPos.Y + 23, style: 23);
                    BiomeChestLoot(chestIndex, 1);

                    chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 66, dungeon.roomPos.Y + 23, style: 27);
                    BiomeChestLoot(chestIndex, 5);

                    roomCount--;
                }
            }

            roomCount = 1;
            while (roomCount > 0)
            {
                dungeon.targetCell.X = WorldGen.genRand.Next(dungeon.grid.Left, dungeon.grid.Right - 2);
                dungeon.targetCell.Y = dungeon.grid.Height - 1;
                if (dungeon.AddRoom(3, 1))
                {
                    dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 1);
                    dungeon.AddMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 2); dungeon.AddMarker(dungeon.targetCell.X + 2, dungeon.targetCell.Y, 2);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/vault2", dungeon.roomPos, ModContent.GetInstance<Remnants>());

                    int chestIndex;

                    chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 37, dungeon.roomPos.Y + 23, style: 24);
                    BiomeChestLoot(chestIndex, 3);

                    chestIndex = WorldGen.PlaceChest(dungeon.roomPos.X + 67, dungeon.roomPos.Y + 23, style: 25);
                    BiomeChestLoot(chestIndex, 2);

                    roomCount--;
                }
            }
            #endregion
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

                                StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                                StructureTools.FillChest(chestIndex, itemsToAdd);
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
                    MiscTools.Rectangle(x, dungeon.roomPos.Y + 28, x + 1, dungeon.roomPos.Y + 29, TileID.CrackedBlueDungeonBrick);

                    x = dungeon.roomPos.X + (index == 1 ? 66 : 2);
                    MiscTools.Rectangle(x, dungeon.roomPos.Y + 28, x + 1, dungeon.roomPos.Y + 29, -1);

                    #region chest
                    int chestIndex = WorldGen.PlaceChest(x, dungeon.roomPos.Y + 29, style: 40);

                    var itemsToAdd = new List<(int type, int stack)>();

                    itemsToAdd.Add((ItemID.Handgun, 1));
                    itemsToAdd.Add((ItemID.MusketBall, Main.rand.Next(50, 150)));

                    StructureTools.GenericLoot(chestIndex, itemsToAdd);

                    StructureTools.FillChest(chestIndex, itemsToAdd);
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
                                    MiscTools.Rectangle(dungeon.roomPos.X, dungeon.roomPos.Y + 14, dungeon.roomPos.X + (index == 0 ? 11 : 4), dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                                }
                                if (dungeon.targetCell.X == dungeon.grid.Right - 1 || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 3))
                                {
                                    MiscTools.Rectangle(dungeon.roomPos.X + (index == 0 ? 30 : 23), dungeon.roomPos.Y + 14, dungeon.room.Right, dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
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
                            MiscTools.Rectangle(dungeon.roomPos.X + 16, dungeon.room.Bottom, dungeon.roomPos.X + 18, dungeon.room.Bottom, TileID.BlueDungeonBrick);
                            WorldGen.KillTile(dungeon.roomPos.X + 15, dungeon.room.Bottom - 1); WorldGen.KillTile(dungeon.roomPos.X + 19, dungeon.room.Bottom - 1);
                        }
                    }

                    if (!altWall && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 3) && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y, 4))
                    {
                        if (dungeon.targetCell.X == dungeon.grid.Left && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) && (dungeon.targetCell.Y != dungeon.grid.Height - 1 || dungeon.X < Main.maxTilesX * 0.5f) || dungeon.FindMarker(dungeon.targetCell.X - 1, dungeon.targetCell.Y, 3))
                        {
                            MiscTools.Rectangle(dungeon.roomPos.X, dungeon.roomPos.Y + (!MiscTools.Solid(dungeon.roomPos.X + 13, dungeon.roomPos.Y + 17) ? 19 : 18), dungeon.roomPos.X + 11, dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                            MiscTools.Rectangle(dungeon.roomPos.X, dungeon.roomPos.Y + 18, dungeon.roomPos.X + 8, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                            if (MiscTools.Solid(dungeon.roomPos.X + 13, dungeon.roomPos.Y + 17))
                            {
                                MiscTools.Rectangle(dungeon.roomPos.X + 12, dungeon.roomPos.Y + 18, dungeon.roomPos.X + 14, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                                if (!MiscTools.Solid(dungeon.roomPos.X + 13, dungeon.roomPos.Y + 19))
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/TheDungeon/hallway-end", new Point16(dungeon.roomPos.X + 12, dungeon.roomPos.Y + 19), ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                        if (dungeon.targetCell.X == dungeon.grid.Right - 1 && !dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) && (dungeon.targetCell.Y != dungeon.grid.Height - 1 || dungeon.X > Main.maxTilesX * 0.5f) || dungeon.FindMarker(dungeon.targetCell.X + 1, dungeon.targetCell.Y, 3))
                        {
                            MiscTools.Rectangle(dungeon.roomPos.X + 23, dungeon.roomPos.Y + (!MiscTools.Solid(dungeon.roomPos.X + 21, dungeon.roomPos.Y + 17) ? 19 : 18), dungeon.room.Right - 1, dungeon.roomPos.Y + 29, TileID.BlueDungeonBrick);
                            MiscTools.Rectangle(dungeon.roomPos.X + 26, dungeon.roomPos.Y + 18, dungeon.room.Right - 1, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                            if (MiscTools.Solid(dungeon.roomPos.X + 21, dungeon.roomPos.Y + 17))
                            {
                                MiscTools.Rectangle(dungeon.roomPos.X + 20, dungeon.roomPos.Y + 18, dungeon.roomPos.X + 22, dungeon.roomPos.Y + 18, TileID.BlueDungeonBrick);

                                if (!MiscTools.Solid(dungeon.roomPos.X + 21, dungeon.roomPos.Y + 19))
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
                            MiscTools.Rectangle(dungeon.area.Left - 20, dungeon.room.Bottom - 5, dungeon.roomPos.X, dungeon.room.Bottom - 1, -1);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/sideexit", 1, new Point16(dungeon.roomPos.X - 12, dungeon.roomPos.Y + 9), ModContent.GetInstance<Remnants>());
                            Vector2 pos = dungeon.roomPos.ToVector2();
                            MiscTools.Terraform(new Vector2(dungeon.area.Left - 20, dungeon.room.Bottom - 3), 3.5f);

                            //if (dungeon.targetCell.Y == dungeon.grid.Height - 1 && X > Main.maxTilesX * 0.5f)
                            //{
                            //    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/railnetwork/TheDungeonstation", new Point16(dungeon.roomPos.X - 35, dungeon.roomPos.Y + 11), ModContent.GetInstance<Remnants>(), 1);
                            //    Infrastructure.AddTrack(dungeon.roomPos.X - 35, dungeon.roomPos.Y + 21);
                            //}
                        }
                        if (dungeon.targetCell.X == dungeon.grid.Right - 1 && dungeon.FindMarker(dungeon.targetCell.X, dungeon.targetCell.Y) || dungeon.targetCell.X == dungeon.grid.Right - 1 && dungeon.targetCell.Y == dungeon.grid.Height - 1 && dungeon.X < Main.maxTilesX * 0.5f)
                        {
                            MiscTools.Rectangle(dungeon.room.Right - 1, dungeon.room.Bottom - 5, dungeon.area.Right + 19, dungeon.room.Bottom - 1, -1);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheDungeon/sideexit", 0, new Point16(dungeon.roomPos.X + dungeon.room.Width - 1, dungeon.roomPos.Y + 9), ModContent.GetInstance<Remnants>());
                            Vector2 pos = dungeon.roomPos.ToVector2();
                            MiscTools.Terraform(new Vector2(dungeon.area.Right + 19, dungeon.room.Bottom - 3), 3.5f);

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

                    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && MiscTools.Tile(x, y + 1).TileType == TileID.BlueDungeonBrick && MiscTools.Tile(x + 1, y + 1).TileType == TileID.BlueDungeonBrick && MiscTools.Tile(x - 1, y).TileType != TileID.ClosedDoor && MiscTools.Tile(x + 2, y).TileType != TileID.ClosedDoor)
                    {
                        int chestIndex = WorldGen.PlaceChest(x, y, style: 2, notNearOtherChests: true);
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                        {
                            ChestLoot(chestIndex, objects);

                            MiscTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Candelabras, 22);
                            if (devMode || brick == TileID.BlueDungeonBrick)
                            {
                                MiscTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Statues, 46);
                            }
                            else if (brick == TileID.GreenDungeonBrick)
                            {
                                MiscTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Statues, 47);
                            }
                            else if (brick == TileID.PinkDungeonBrick)
                            {
                                MiscTools.PlaceObjectsInArea(x - 10, y, x + 9, y, TileID.Statues, 48);
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

                    for (; !MiscTools.Tile(x, y - 1).HasTile || MiscTools.Tile(x, y).HasTile; y += MiscTools.Tile(x, y).HasTile ? 1 : -1)
                    {

                    }

                    int length = WorldGen.genRand.Next(10, 20);

                    bool valid = true;
                    if (!MiscTools.Solid(x, y - 1) || MiscTools.Tile(x, y).WallType != ModContent.WallType<dungeonblue>() || MiscTools.Tile(x, y - 1).TileType == TileID.TrapdoorClosed)
                    {
                        valid = false;
                    }
                    else for (int j = y + 1; j <= y + length + 3; j++)
                        {
                            for (int i = x - 1; i <= x + 1; i++)
                            {
                                if (i == x && MiscTools.Tile(i, j).HasTile || MiscTools.Solid(i, j) || MiscTools.Tile(i, j).TileType == TileID.Chain)
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
                        if (MiscTools.Tile(x, y).TileType == TileID.Candles && WorldGen.genRand.NextBool(2) || MiscTools.Tile(x, y).TileType == TileID.HangingLanterns)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }
                }

                StructureTools.AddDecorations(new Rectangle(dungeon.area.Left, entranceY - 18, dungeon.area.Width, dungeon.area.Height + (dungeon.area.Top - (entranceY - 18))));
                StructureTools.AddVariation(dungeon.area);
                StructureTools.AddTraps(dungeon.area);

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
                        Tile tile = MiscTools.Tile(x, y);

                        if (MiscTools.Solid(x, y) && tile.TileType != TileID.Spikes && x >= dungeon.area.Left - 3 && x <= dungeon.area.Right + 3 && y >= dungeon.area.Top - 7 && y <= dungeon.area.Bottom + 8)
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

                        if (MiscTools.Tile(x, y).HasTile)
                        {
                            if (MiscTools.Tile(x, y).TileType == TileID.HangingLanterns && MiscTools.Tile(x, y - 1).TileType != TileID.HangingLanterns)
                            {
                                MiscTools.Tile(x, y).TileFrameY += (short)(lanternStyle * 36);
                                MiscTools.Tile(x, y + 1).TileFrameY += (short)(lanternStyle * 36);
                            }
                            if (MiscTools.Tile(x, y).TileType == TileID.Banners && MiscTools.Tile(x, y - 1).TileType != TileID.Banners)
                            {
                                MiscTools.Tile(x, y).TileFrameX += (short)(bannerStyle * 18);
                                MiscTools.Tile(x, y + 1).TileFrameX += (short)(bannerStyle * 18);
                                MiscTools.Tile(x, y + 2).TileFrameX += (short)(bannerStyle * 18);
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
                            if (MiscTools.Tile(x, y).TileType == TileID.BlueDungeonBrick)
                            {
                                MiscTools.Tile(x, y).TileType = TileID.GreenDungeonBrick;
                            }
                            else if (MiscTools.Tile(x, y).TileType == TileID.CrackedBlueDungeonBrick)
                            {
                                MiscTools.Tile(x, y).TileType = TileID.CrackedGreenDungeonBrick;
                            }
                            if (MiscTools.Tile(x, y).WallType == ModContent.WallType<dungeonblue>())
                            {
                                MiscTools.Tile(x, y).WallType = (ushort)ModContent.WallType<dungeongreen>();
                            }
                            else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeonUnsafe || MiscTools.Tile(x, y).WallType == WallID.BlueDungeon)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.GreenDungeonUnsafe;
                            }
                            else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeonSlabUnsafe)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.GreenDungeonSlabUnsafe;
                            }
                            else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeonTileUnsafe)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.GreenDungeonTileUnsafe;
                            }
                        }
                        else if (brick == TileID.PinkDungeonBrick)
                        {
                            if (MiscTools.Tile(x, y).TileType == TileID.BlueDungeonBrick)
                            {
                                MiscTools.Tile(x, y).TileType = TileID.PinkDungeonBrick;
                            }
                            else if (MiscTools.Tile(x, y).TileType == TileID.CrackedBlueDungeonBrick)
                            {
                                MiscTools.Tile(x, y).TileType = TileID.CrackedPinkDungeonBrick;
                            }
                            if (MiscTools.Tile(x, y).WallType == ModContent.WallType<dungeonblue>())
                            {
                                MiscTools.Tile(x, y).WallType = (ushort)ModContent.WallType<dungeonpink>();
                            }
                            else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeonUnsafe || MiscTools.Tile(x, y).WallType == WallID.BlueDungeon)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.PinkDungeonUnsafe;
                            }
                            else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeonSlabUnsafe)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.PinkDungeonSlabUnsafe;
                            }
                            else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeonTileUnsafe)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.PinkDungeonTileUnsafe;
                            }
                        }
                        else if (MiscTools.Tile(x, y).WallType == WallID.BlueDungeon)
                        {
                            MiscTools.Tile(x, y).WallType = WallID.BlueDungeonUnsafe;
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

            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

            StructureTools.FillChest(chestIndex, itemsToAdd);
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

            StructureTools.GenericLoot(chestIndex, itemsToAdd, 3, new int[] { ItemID.BiomeSightPotion });

            StructureTools.FillChest(chestIndex, itemsToAdd);
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

        private void Spikes(StructureTools.Dungeon dungeon, float multiplier = 1)
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
                                MiscTools.Tile(num7, num8 - num10).TileType = 48;
                                MiscTools.Tile(num7, num8 - num10).HasTile = true;
                            }
                            num7--;
                            num12--;
                        }
                        if (MiscTools.Tile(num7, num8).TileType == TileID.WoodBlock)
                        {
                            MiscTools.Tile(num7, num8).TileType = TileID.BlueDungeonBrick;
                        }
                        num12 = WorldGen.genRand.Next(5, 13);
                        num7 = num9 + 1;
                        while (SpikeValidation(num7, num8) && Main.tile[num7 + 1, num8].HasTile && Main.tile[num7, num8 + num10].HasTile && Main.tile[num7, num8].HasTile && !Main.tile[num7, num8 - num10].HasTile && num12 > 0)
                        {
                            Main.tile[num7, num8].TileType = 48;
                            if (!Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                            {
                                MiscTools.Tile(num7, num8 - num10).TileType = 48;
                                MiscTools.Tile(num7, num8 - num10).HasTile = true;
                            }
                            num7++;
                            num12--;
                        }
                        if (MiscTools.Tile(num7, num8).TileType == TileID.WoodBlock)
                        {
                            MiscTools.Tile(num7, num8).TileType = TileID.BlueDungeonBrick;
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
                    if (MiscTools.Tile(i, y).HasTile && (MiscTools.Tile(i, y).TileType == TileID.Platforms || MiscTools.Tile(i, y).TileType == TileID.TrapdoorClosed || MiscTools.Tile(i, y).TileType == TileID.PlanterBox))
                    {
                        return false;
                    }
                }
                if (MiscTools.Tile(i, y - 1).HasTile && (MiscTools.Tile(i, y - 1).TileType == TileID.ClosedDoor || MiscTools.Tile(i, y - 1).TileType == ModContent.TileType<LockedIronDoor>()))
                {
                    return false;
                }
            }
            return MiscTools.Solid(x, y) && MiscTools.Tile(x, y).TileType != TileID.CrackedBlueDungeonBrick;
        }
    }

    public class JungleTemple : GenPass
    {
        public int X;
        public int Y;

        int[,,] layout;

        int roomWidth => 63;
        int roomHeight => 63;

        int roomsVertical;
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
            roomsVertical = (int)(1 + MiscTools.GetSafeWorldScale() * 2f);
            layout = new int[roomsHorizontal, roomsVertical, 6];

            X = (int)(Jungle.Center * biomes.CellSize);
            Y = Main.maxTilesY - 310 - roomsVertical * roomHeight;

            GenVars.tLeft = location.Left;
            GenVars.tRight = location.Right;
            GenVars.tTop = location.Top;
            GenVars.tBottom = location.Bottom;

            MiscTools.Terraform(new Vector2(X, Y - 42), 42, killWall: true);

            StructureTools.FillEdges(location.Left - roomWidth + 1, location.Bottom, location.Right + roomWidth - 2, location.Bottom + 21, TileID.LihzahrdBrick, false);

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

            StructureTools.AddTraps(location, 1, true);

            #region objects
            if (!devMode)
            {
                int objects;

                objects = roomsVertical * roomsHorizontal / 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    int y = WorldGen.genRand.Next(location.Top + 12, location.Bottom);

                    for (; !MiscTools.Solid(x, y + 1) || MiscTools.Solid(x, y); y += MiscTools.Solid(x, y) ? -1 : 1)
                    {
                    }

                    bool valid = true;
                    if (MiscTools.Tile(x, y).LiquidAmount > 0 || MiscTools.Tile(x, y).WallType != ModContent.WallType<temple>() && MiscTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe)
                    {
                        valid = false;
                    }
                    else for (int i = x - 3; i <= x + 3; i++)
                        {
                            if (MiscTools.Tile(i, y).HasTile || !MiscTools.Solid(i, y + 1))
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        MiscTools.CandleBunch(x - 3, y, x + 3, y, WorldGen.genRand.Next(3, 6));
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
                    else if (MiscTools.Tile(x - 1, y).HasTile && MiscTools.Tile(x - 1, y).TileType == TileID.ClosedDoor || MiscTools.Tile(x + 2, y).HasTile && MiscTools.Tile(x + 2, y).TileType == TileID.ClosedDoor)
                    {
                        valid = false;
                    }
                    else if (MiscTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && MiscTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else for (int i = -1; i <= 2; i++)
                        {
                            if (!MiscTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[MiscTools.Tile(x + i, y + 1).TileType] || MiscTools.Tile(x + i, y + 1).TileType != TileID.LihzahrdBrick)
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
                    else if (MiscTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && MiscTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else if (MiscTools.Tile(x, y + 1).TileType != TileID.LihzahrdBrick && MiscTools.Tile(x, y + 1).TileType != TileID.JungleGrass)
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
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Pots || MiscTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && MiscTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else if (MiscTools.Tile(x, y + 1).TileType != TileID.LihzahrdBrick && MiscTools.Tile(x, y + 1).TileType != TileID.JungleGrass)
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
                    if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot || MiscTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe && MiscTools.Tile(x, y).WallType != ModContent.WallType<temple>())
                    {
                        valid = false;
                    }
                    else if (MiscTools.Tile(x, y + 1).TileType != TileID.LihzahrdBrick && MiscTools.Tile(x, y + 1).TileType != TileID.JungleGrass)
                    {
                        valid = false;
                    }
                    else if (MiscTools.Tile(x, y - 1).HasTile)
                    {
                        valid = false;
                    }
                    else for (int i = -1; i <= 1; i++)
                        {
                            if (!MiscTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[MiscTools.Tile(x + i, y + 1).TileType])
                            {
                                valid = false;
                                break;
                            }
                            else if (MiscTools.Tile(x + i, y).HasTile && MiscTools.Tile(x + i, y).TileType == TileID.ClosedDoor)
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

                StructureTools.AddVariation(location);

                for (int y = location.Top + 1; y <= location.Bottom + 21; y++)
                {
                    for (int x = location.Left - 21 - 38; x <= location.Right + 21 + 38; x++)
                    {
                        Tile tile = MiscTools.Tile(x, y);

                        if (tile.TileType == TileID.LihzahrdBrick && MiscTools.Tile(x, y - 1).TileType != TileID.LihzahrdAltar)
                        {
                            float _weathering = weathering.GetNoise(x, y);
                            if (_weathering > -0.05f && _weathering < 0.05f)
                            {
                                tile.TileType = !MiscTools.SurroundingTilesActive(x, y, true) ? TileID.JungleGrass : TileID.Mud;
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

            StructureTools.GenericLoot(chestIndex, itemsToAdd, 3);

            StructureTools.FillChest(chestIndex, itemsToAdd);
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
            MiscTools.Rectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top - roomHeight / 2, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom - roomHeight / 2, tile, wall, add, replace, style, liquid, liquidType);
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
                                MiscTools.Tile(num7, num8 - num10).TileType = TileID.WoodenSpikes;
                                MiscTools.Tile(num7, num8 - num10).HasTile = true;
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
                                MiscTools.Tile(num7, num8 - num10).TileType = TileID.WoodenSpikes;
                                MiscTools.Tile(num7, num8 - num10).HasTile = true;
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
                    if (MiscTools.Tile(i, y).HasTile && (MiscTools.Tile(i, y).TileType == TileID.Platforms || MiscTools.Tile(i, y).TileType == TileID.TrapdoorClosed))
                    {
                        return false;
                    }
                }
                if (MiscTools.Tile(i, y - 1).HasTile && MiscTools.Tile(i, y - 1).TileType == TileID.ClosedDoor)
                {
                    return false;
                }
            }
            return MiscTools.Solid(x, y);
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
            StructureTools.Dungeon temple = new StructureTools.Dungeon(0, (int)GenVars.rockLayer, 5, (int)(Main.maxTilesY / 1200f * 4), 48, 48, 5);

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

            StructureTools.FillEdges(temple.area.Left, temple.area.Top, temple.area.Right - 1, temple.area.Bottom - 1, ModContent.TileType<MarineSlab>(), false);
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
                        MiscTools.Terraform(new Vector2(temple.room.X + temple.room.Width / 2, temple.room.Bottom + 12), 3.5f, scaleX: 1, scaleY: 2);
                        for (int i = -4; i <= 4; i++)
                        {
                            MiscTools.Rectangle(temple.room.X + temple.room.Width / 2 + i, temple.room.Bottom, temple.room.X + temple.room.Width / 2 + i, temple.room.Bottom + WorldGen.genRand.Next(6, 12), i < -2 || i > 2 ? TileID.LivingWood : -1, i > -4 && i < 4 ? WallID.LivingWoodUnsafe : -2);
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
            StructureTools.Dungeon stronghold = new StructureTools.Dungeon(0, 0, Main.maxTilesX / 420, 2, 96, 66, 3);
            stronghold.X = Main.maxTilesX / 2 - stronghold.area.Width / 2;
            stronghold.Y = Main.maxTilesY - 190;

            GenVars.structures.AddProtectedStructure(stronghold.area, 25);

            StructureTools.FillEdges(stronghold.area.Left, stronghold.area.Top + stronghold.room.Height, stronghold.area.Right - 1, stronghold.area.Bottom - 1, ModContent.TileType<HellishBrick>(), false);
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
                    if (MiscTools.Tile(x, y).WallType == ModContent.WallType<stronghold>() || MiscTools.Tile(x, y).WallType == ModContent.WallType<HellishBrickWallUnsafe>())
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
                        if (MiscTools.Solid(x, y - 1) && (!MiscTools.Solid(x - 1, y - 1) || !MiscTools.Solid(x + 1, y - 1)))
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

                    for (; !MiscTools.Solid(x, y + 1) || MiscTools.Solid(x, y); y += MiscTools.Solid(x, y) ? -1 : 1)
                    {
                    }

                    bool valid = true;
                    if (MiscTools.Tile(x, y).LiquidAmount > 0 || MiscTools.Tile(x, y).WallType != ModContent.WallType<stronghold>() && MiscTools.Tile(x, y).WallType != ModContent.WallType<HellishBrickWallUnsafe>())
                    {
                        valid = false;
                    }
                    else for (int i = x - 3; i <= x + 3; i++)
                        {
                            if (MiscTools.Tile(i, y).HasTile || !MiscTools.Solid(i, y + 1))
                            {
                                valid = false;
                                break;
                            }
                        }

                    if (valid)
                    {
                        MiscTools.CandleBunch(x - 3, y, x + 3, y, WorldGen.genRand.Next(3, 6));
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
                    else if (MiscTools.Tile(x, y).LiquidAmount > 0 || MiscTools.Tile(x, y).WallType != ModContent.WallType<stronghold>() && MiscTools.Tile(x, y).WallType != ModContent.WallType<HellishBrickWallUnsafe>())
                    {
                        valid = false;
                    }
                    else for (int i = -2; i <= 2; i++)
                        {
                            if (!MiscTools.Tile(x + i, y + 2).HasTile || !Main.tileSolid[MiscTools.Tile(x + i, y + 2).TileType])
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
                    else if (MiscTools.Tile(x, y).LiquidAmount > 0 || MiscTools.Tile(x, y).WallType != ModContent.WallType<stronghold>() && MiscTools.Tile(x, y).WallType != ModContent.WallType<HellishBrickWallUnsafe>())
                    {
                        valid = false;
                    }
                    else for (int i = -1; i <= 2; i++)
                        {
                            if (!MiscTools.Tile(x + i, y + 2).HasTile || !Main.tileSolid[MiscTools.Tile(x + i, y + 2).TileType])
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

                            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.ObsidianSkinPotion, ItemID.InfernoPotion });

                            StructureTools.FillChest(chestIndex, itemsToAdd);

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

        private void Spikes(StructureTools.Dungeon dungeon, float multiplier = 1)
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
                                MiscTools.Tile(num7, num8 - num10).TileType = 48;
                                MiscTools.Tile(num7, num8 - num10).HasTile = true;
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
                                MiscTools.Tile(num7, num8 - num10).TileType = 48;
                                MiscTools.Tile(num7, num8 - num10).HasTile = true;
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
                    if (MiscTools.Tile(i, y).HasTile && MiscTools.Tile(i, y).TileType == ModContent.TileType<HellishPlatform>())
                    {
                        return false;
                    }
                }
            }
            return MiscTools.Solid(x, y);
        }
    }

    public class MagicalLab : GenPass
    {
        public MagicalLab(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureTools.Dungeon lab = new StructureTools.Dungeon(0, 0, 5, (int)(Main.maxTilesY / 1200f * 4), 54, 54, 5);

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

            MiscTools.Rectangle(lab.area.Left - 5, lab.area.Top - 10, lab.area.Right + 5, lab.area.Bottom + 10, ModContent.TileType<EnchantedBrick>(), ModContent.WallType<EnchantedBrickWallUnsafe>(), liquid: 0, liquidType: LiquidID.Shimmer);

            StructureTools.FillEdges(lab.area.Left - 5, lab.area.Top - 10, lab.area.Right + 4, lab.area.Bottom + 9, ModContent.TileType<EnchantedBrick>(), false);

            for (int y = lab.area.Top - 35; y <= lab.area.Top - 10; y++)
            {
                for (int x = lab.area.Left; x <= lab.area.Right; x++)
                {
                    if (MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).TileType == TileID.Stone && !MiscTools.SurroundingTilesActive(x, y))
                    {
                        MiscTools.Tile(x, y).TileType = TileID.VioletMoss;
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

                StructureTools.GenericLoot(chestIndex, itemsToAdd, 3, new int[] { ItemID.BiomeSightPotion });

                StructureTools.FillChest(chestIndex, itemsToAdd);
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
            MiscTools.Rectangle(lab.roomPos.X - 2, lab.roomPos.Y + 30, lab.roomPos.X - 1, lab.roomPos.Y + 30, TileID.Platforms, style: 10);
            MiscTools.Rectangle(lab.roomPos.X - 2, lab.roomPos.Y + 36, lab.roomPos.X - 1, lab.roomPos.Y + 36, TileID.Platforms, style: 10);
            MiscTools.Rectangle(lab.roomPos.X + lab.room.Width, lab.roomPos.Y + 30, lab.roomPos.X + lab.room.Width + 1, lab.roomPos.Y + 30, TileID.Platforms, style: 10);
            MiscTools.Rectangle(lab.roomPos.X + lab.room.Width, lab.roomPos.Y + 36, lab.roomPos.X + lab.room.Width + 1, lab.roomPos.Y + 36, TileID.Platforms, style: 10);

            #endregion

            #region cleanup
            StructureTools.AddDecorations(lab.area);

            for (int y = lab.area.Top; y <= lab.area.Bottom; y++)
            {
                for (int x = lab.area.Left; x <= lab.area.Right; x++)
                {
                    if (MiscTools.Tile(x, y + 1).HasTile)
                    {
                        //if (WGTools.Tile(x, y + 1).TileType == TileID.Platforms)
                        //{
                        //    WorldGen.PlaceTile(x, y, WorldGen.genRand.NextBool(2) ? TileID.Bottles : TileID.Books);
                        //}
                        //else
                        if (MiscTools.Tile(x, y + 1).TileType == ModContent.TileType<EnchantedBrick>() && MiscTools.Tile(x + 1, y + 1).HasTile && MiscTools.Tile(x + 1, y + 1).TileType == ModContent.TileType<EnchantedBrick>())
                        {
                            if (MiscTools.Tile(x - 2, y).HasTile && MiscTools.Tile(x - 2, y).TileType == TileID.ClosedDoor || MiscTools.Tile(x + 3, y).HasTile && MiscTools.Tile(x + 3, y).TileType == TileID.ClosedDoor)
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

            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.MagicPowerPotion, ItemID.ManaRegenerationPotion }, true);

            if (WorldGen.genRand.NextBool(2))
            {
                itemsToAdd.Add((ModContent.ItemType<DreamJelly>(), Main.rand.Next(5, 11)));
            }

            StructureTools.FillChest(chestIndex, itemsToAdd);
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
            StructureTools.Dungeon maze = new StructureTools.Dungeon(0, 0, (int)(1 + (float)(MiscTools.GetSafeWorldScale()) * 12), (int)(1 + (float)(MiscTools.GetSafeWorldScale()) * 12), 18, 18, 7);

            if (WorldGen.gen)
            {
                maze.X = GenVars.dungeonX - maze.area.Width / 2;
                maze.Y = Main.maxTilesY - 350 - maze.area.Height;

                MiscTools.Terraform(new Vector2(maze.area.Center.X, maze.Y - 50), 50, killWall: true);

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
                StructureTools.FillEdges(maze.area.Left - 57, maze.area.Top - 18, maze.area.Right + 56, maze.area.Bottom + 40, ModContent.TileType<LabyrinthBrick>());
                MiscTools.Rectangle(maze.area.Left - 57, maze.area.Top - 18, maze.area.Right + 56, maze.area.Bottom + 40, ModContent.TileType<LabyrinthBrick>(), liquid: 0);
                MiscTools.Rectangle(maze.area.Left - 66, maze.area.Top - 17, maze.area.Right + 65, maze.area.Bottom + 39, wall: ModContent.WallType<LabyrinthTileWall>(), liquidType: 0);

                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/EchoingHalls/entrance", new Point16(maze.area.Center.X - 39, maze.Y - 54), ModContent.GetInstance<Remnants>());
                RemSystem.whisperingMazeX = maze.area.Center.X;
                RemSystem.whisperingMazeY = maze.Y - 36;


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
                    maze.AddMarker(i, j, 5);
                }
            }

            maze.AddMarker(maze.grid.Center.X, maze.grid.Height - 1, 1);

            int walls = maze.grid.Width * maze.grid.Height / 16;
            while (walls > 0)
            {
                int x = WorldGen.genRand.Next(maze.grid.Left, maze.grid.Right + 1);
                int y = WorldGen.genRand.Next(maze.grid.Top, maze.grid.Bottom + 1);

                if ((x != maze.grid.Center.X || y > maze.grid.Top) && !maze.FindMarker(x, y))
                {
                    if (!maze.FindMarker(x - 1, y - 1) && !maze.FindMarker(x + 1, y - 1) && !maze.FindMarker(x - 1, y + 1) && !maze.FindMarker(x + 1, y + 1))
                    {
                        maze.AddMarker(x, y);
                        
                        walls--;
                    }
                }
            }

            for (maze.targetCell.Y = maze.grid.Top; maze.targetCell.Y < maze.grid.Bottom; maze.targetCell.Y++)
            {
                for (maze.targetCell.X = maze.grid.Left; maze.targetCell.X < maze.grid.Right; maze.targetCell.X++)
                {
                    if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y))
                    {
                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/room", maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6) ? 1 : 0, maze.roomPos, ModContent.GetInstance<Remnants>());
                    }
                }
            }

            maze.targetCell = new Point(maze.grid.Center.X, 0);
            List<Point> cellStack = new List<Point>();

            bool stop = false;
            while (!stop)
            {
                List<int> adjacencies = new List<int>();
                AdjacentCells(maze, adjacencies);

                if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y))
                {
                    maze.AddMarker(maze.targetCell.X, maze.targetCell.Y);
                    cellStack.Add(maze.targetCell);

                    if (adjacencies.Count == 0)
                    {
                        maze.AddMarker(maze.targetCell.X, maze.targetCell.Y, 6);
                    }
                }

                if (adjacencies.Count > 0 && maze.targetCell != new Point(maze.grid.Center.X, maze.grid.Height - 1))
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
                for (maze.targetCell.X = maze.grid.Left; maze.targetCell.X < maze.grid.Right; maze.targetCell.X++)
                {
                    if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y) && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 5))
                    {
                        if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6))
                        {
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/room", 1, maze.roomPos, ModContent.GetInstance<Remnants>());
                        }
                        if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 1))
                        {
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/EchoingHalls/ladder", maze.targetCell == new Point(maze.grid.Center.X, maze.grid.Top) || maze.targetCell == new Point(maze.grid.Center.X, maze.grid.Height - 1) ? 1 : 0, new Point16(maze.room.X + 4, maze.room.Y), ModContent.GetInstance<Remnants>());

                            if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y - 1, 1) && maze.FindMarker(maze.targetCell.X, maze.targetCell.Y - 1, 3))
                            {
                                MiscTools.Rectangle(maze.room.Left + 4, maze.room.Top + 7 - maze.room.Height, maze.room.Left + 6, maze.room.Bottom - maze.room.Height, -2, ModContent.WallType<LabyrinthTileWall>());
                                MiscTools.Rectangle(maze.room.Right - 6, maze.room.Top + 7 - maze.room.Height, maze.room.Right - 4, maze.room.Bottom - maze.room.Height, -2, ModContent.WallType<LabyrinthTileWall>());
                            }
                        }

                        if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 2))
                        {
                            MiscTools.Rectangle(maze.room.Right - 3, maze.room.Top + 4, maze.room.Right, maze.room.Bottom, ModContent.TileType<LabyrinthBrick>());
                        }
                        if (!maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 4))
                        {
                            MiscTools.Rectangle(maze.room.Left, maze.room.Top + 4, maze.room.Left + 3, maze.room.Bottom, ModContent.TileType<LabyrinthBrick>());
                        }

                        if (maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6))
                        {
                            MiscTools.Rectangle(maze.room.Left + 8, maze.room.Top + 1, maze.room.Left + 10, maze.room.Bottom, wall: ModContent.WallType<LabyrinthTileWall>());
                            MiscTools.Rectangle(maze.room.Left + 8, maze.room.Top, maze.room.Left + 10, maze.room.Top, wall: ModContent.WallType<LabyrinthBrickWall>());

                            MiscTools.Tile(maze.room.Left + 9, maze.room.Top + 2).WallType = (ushort)ModContent.WallType<LabyrinthBrickWall>();
                            MiscTools.Tile(maze.room.Left + 9, maze.room.Top + 4).WallType = (ushort)ModContent.WallType<LabyrinthBrickWall>();
                            MiscTools.Tile(maze.room.Left + 9, maze.room.Top + 6).WallType = (ushort)ModContent.WallType<LabyrinthBrickWall>();

                            WorldGen.PlaceObject(maze.room.Center.X - 1, maze.room.Top + 8, ModContent.TileType<LabyrinthSpawner>());
                            ModContent.GetInstance<TEmazeguardianspawner>().Place(maze.room.Center.X - 1, maze.room.Top + 8);
                        }
                    }
                }
            }

            int roomCount = 5;
            int attempts = 0;
            while (roomCount > 0)
            {
                int width = (int)((roomCount <= 4 ? 2 : 4) * MiscTools.GetSafeWorldScale());
                int height = (int)((roomCount <= 4 ? 2 : 4) * MiscTools.GetSafeWorldScale());
                int x = WorldGen.genRand.Next(maze.grid.Left + 1, maze.grid.Right - width);
                int y = WorldGen.genRand.Next(maze.grid.Top + 1, maze.grid.Bottom - height);

                bool valid = true;

                if (roomCount == 5)
                {
                    int score = 0;
                    for (int i = x; i < x + width; i++)
                    {
                        int j = y - 1;
                        while (maze.FindMarker(i, j, 1) && maze.FindMarker(i, j, 3))
                        {
                            j--;
                        }
                        if (maze.FindMarker(i, j, 6) && maze.FindMarker(i, j, 3))
                        {
                            score++;
                        }

                        j = y + height;
                        while (maze.FindMarker(i, j, 1) && maze.FindMarker(i, j, 3))
                        {
                            j++;
                        }
                        if (maze.FindMarker(i, j, 6) && maze.FindMarker(i, j, 1))
                        {
                            score++;
                        }
                    }

                    for (int j = y; j < y + height; j++)
                    {
                        int i = x - 1;
                        while (maze.FindMarker(i, j, 2) && maze.FindMarker(i, j, 4))
                        {
                            i--;
                        }
                        if (maze.FindMarker(i, j, 6) && maze.FindMarker(i, j, 2))
                        {
                            score++;
                        }

                        i = x + width;
                        while (maze.FindMarker(i, j, 2) && maze.FindMarker(i, j, 4))
                        {
                            i++;
                        }
                        if (maze.FindMarker(i, j, 6) && maze.FindMarker(i, j, 4))
                        {
                            score++;
                        }
                    }

                    if (score < Math.Sqrt(width * height) / 2 - attempts / 1000)
                    {
                        valid = false;
                        attempts++;
                    }
                }

                if (valid)
                {
                    for (int j = y; j < y + height; j++)
                    {
                        for (int i = x; i < x + width; i++)
                        {
                            if (maze.FindMarker(i, j, 5) || maze.FindMarker(i, j, 6) && roomCount <= 4 && attempts < 1000)
                            {
                                valid = false;
                                attempts++;
                            }
                        }
                    }
                }

                if (valid)
                {
                    attempts = 0;

                    for (int j = y; j < y + height; j++)
                    {
                        for (int i = x; i < x + width; i++)
                        {
                            maze.AddMarker(i, j, 5);
                        }
                    }

                    maze.targetCell.X = x;
                    maze.targetCell.Y = y;

                    MiscTools.Rectangle(maze.room.Left + 4, maze.room.Top + 7, maze.room.Right + maze.room.Width * (width - 1) - 4, maze.room.Bottom + maze.room.Height * (height - 1) - 1, -1, ModContent.WallType<LabyrinthTileWall>());

                    for (maze.targetCell.X = x; maze.targetCell.X < x + width; maze.targetCell.X++)
                    {
                        maze.targetCell.Y = y;

                        MiscTools.Rectangle(maze.room.X + maze.room.Width - 3, maze.room.Top + 7, maze.room.X + maze.room.Width - 3, maze.room.Bottom + maze.room.Height * (height - 1) - 1, -2, ModContent.WallType<LabyrinthBrickWall>());
                        MiscTools.Rectangle(maze.room.X + maze.room.Width + 3, maze.room.Top + 7, maze.room.X + maze.room.Width + 3, maze.room.Bottom + maze.room.Height * (height - 1) - 1, -2, ModContent.WallType<LabyrinthBrickWall>());

                        for (int j = maze.room.Top + 7; j <= maze.room.Bottom + maze.room.Height * (height - 1) - 1; j++)
                        {
                            MiscTools.Rectangle(maze.room.X + (WorldGen.genRand.NextBool(3) ? 4 : 5), j, maze.room.X + maze.room.Width - (WorldGen.genRand.NextBool(3) ? 4 : 5), j, -2, ModContent.WallType<whisperingmaze>());
                            //WGTools.Rectangle(maze.room.X + 4, j, maze.room.X + maze.room.Width - 4, j, -2, ModContent.WallType<whisperingmaze>());
                        }

                        for (maze.targetCell.Y = y; maze.targetCell.Y < y + height - 1; maze.targetCell.Y++)
                        {
                            WorldGen.PlaceTile(maze.room.Left + 4, maze.room.Bottom, ModContent.TileType<LabyrinthPlatform>());
                            WorldGen.PlaceTile(maze.room.Right - 4, maze.room.Bottom, ModContent.TileType<LabyrinthPlatform>());
                        }
                    }

                    for (maze.targetCell.Y = y; maze.targetCell.Y < y + height; maze.targetCell.Y++)
                    {
                        for (maze.targetCell.X = x; maze.targetCell.X < x + width - 1; maze.targetCell.X++)
                        {
                            WorldGen.PlaceTile(maze.room.Right, maze.room.Bottom - 3, ModContent.TileType<LabyrinthWallLamp>(), style: 1);
                            WorldGen.PlaceTile(maze.room.Right, maze.room.Bottom - 5, ModContent.TileType<LabyrinthWallLamp>(), style: 1);
                            WorldGen.PlaceTile(maze.room.Right, maze.room.Bottom - 7, ModContent.TileType<LabyrinthWallLamp>(), style: 1);
                            WorldGen.PlaceTile(maze.room.Right, maze.room.Bottom - 9, ModContent.TileType<LabyrinthWallLamp>(), style: 1);
                        }
                    }

                    for (maze.targetCell.Y = y; maze.targetCell.Y < y + height - 1; maze.targetCell.Y++)
                    {
                        for (maze.targetCell.X = x; maze.targetCell.X < x + width - 1; maze.targetCell.X++)
                        {
                            for (int k = 0; k < 7; k++)
                            {
                                MiscTools.Rectangle(maze.room.X + maze.room.Width - 3 + k, maze.room.Y + maze.room.Height, maze.room.X + maze.room.Width - 3 + k, maze.room.Y + maze.room.Height + WorldGen.genRand.Next(3, 7), ModContent.TileType<LabyrinthBrick>());
                            }
                        }
                    }

                    roomCount--;
                }
            }

            roomCount = maze.grid.Width * maze.grid.Height / 16;
            while (roomCount > 0)
            {
                maze.targetCell.X = WorldGen.genRand.Next(maze.grid.Left + 1, maze.grid.Right);
                maze.targetCell.Y = WorldGen.genRand.Next(maze.grid.Top, maze.grid.Bottom);

                if (!maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y, 5) && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 5) && !maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y, 6) && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 6))
                {
                    if (!maze.FindMarker(maze.targetCell.X - 1, maze.targetCell.Y, 2) && !maze.FindMarker(maze.targetCell.X, maze.targetCell.Y, 4))
                    {
                        MiscTools.Rectangle(maze.roomPos.X - 14, maze.roomPos.Y + 13, maze.roomPos.X + 14, maze.roomPos.Y + 17, -1);

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
                        if (tile.WallType == ModContent.WallType<LabyrinthBrickWall>() && WorldGen.genRand.NextBool(5))
                        {
                            tile.WallType = (ushort)ModContent.WallType<LabyrinthTileWall>();
                        }

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

        private void AdjacentCells(StructureTools.Dungeon maze, List<int> adjacencies)
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

    public class Vault : GenPass
    {
        public Vault(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Vault");

            bool devMode = false;

            #region setup
            StructureTools.Dungeon vault = new StructureTools.Dungeon(0, 0, (int)(Main.maxTilesX / 4200f * 12), Main.maxTilesY / 600, 60, 60, 5);

            vault.X = (int)(Jungle.Center * biomes.CellSize) - vault.area.Width / 2;
            vault.Y = Main.maxTilesY - 210 - vault.area.Height;

            MiscTools.Rectangle(vault.area.Left - 1, vault.area.Top - 1, vault.area.Right, vault.area.Bottom, ModContent.TileType<VaultPlating>());
            MiscTools.Rectangle(vault.area.Left, vault.area.Top, vault.area.Right - 1, vault.area.Bottom - 1, wall: ModContent.WallType<VaultWallUnsafe>(), liquid: 0);
            StructureTools.FillEdges(vault.area.Left - 1, vault.area.Top - 1, vault.area.Right, vault.area.Bottom, ModContent.TileType<VaultPlating>());
            GenVars.structures.AddProtectedStructure(vault.area, 22);
            #endregion

            #region rooms

            int roomCount;

            #region special

            #endregion

            #region standard
            int roomID = 0;
            int attempts = 0;
            while (attempts < 10000)
            {
                vault.targetCell.X = WorldGen.genRand.Next(vault.grid.Left, vault.grid.Right - (roomID == 0 || roomID == 1 ? 1 : 0));
                vault.targetCell.Y = WorldGen.genRand.Next(0, vault.grid.Bottom - (roomID == 2 || roomID == 3 ? 1 : 0));
                if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
                {
                    if (roomID == 0 || roomID == 1)
                    {
                        if (!vault.FindMarker(vault.targetCell.X + 1, vault.targetCell.Y) && vault.CheckConnection(4, true) && vault.CheckConnection(2, true, 1))
                        {
                            if (roomID == 0)
                            {
                                if (vault.CheckConnection(1, true) && vault.CheckConnection(3, false) && vault.CheckConnection(1, false, 1) && vault.CheckConnection(3, true, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 3); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y, 1);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/2x1", 0, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 1)
                            {
                                if (vault.CheckConnection(1, false) && vault.CheckConnection(3, true) && vault.CheckConnection(1, true, 1) && vault.CheckConnection(3, false, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 1); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y, 3);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/2x1", 1, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                    }
                    else if (roomID == 2 || roomID == 3)
                    {
                        if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y + 1) && vault.CheckConnection(1, true) && vault.CheckConnection(3, true, 0, 1))
                        {
                            if (roomID == 2)
                            {
                                if (vault.CheckConnection(2, true) && vault.CheckConnection(4, false) && vault.CheckConnection(2, false, 0, 1) && vault.CheckConnection(4, true, 0, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 4); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1, 2);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x2", 0, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 3)
                            {
                                if (vault.CheckConnection(2, false) && vault.CheckConnection(4, true) && vault.CheckConnection(2, true, 0, 1) && vault.CheckConnection(4, false, 0, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 2); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1, 4);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x2", 1, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                    }

                    if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
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
                if (roomID >= 4)
                {
                    roomID = 0;
                }
            }

            roomID = 0;
            attempts = 0;
            while (attempts < 10000)
            {
                vault.targetCell.X = WorldGen.genRand.Next(vault.grid.Left, vault.grid.Right);
                vault.targetCell.Y = WorldGen.genRand.Next(0, vault.grid.Bottom);

                if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
                {
                    if (roomID == 0)
                    {
                        if (vault.CheckConnection(1, false) && vault.CheckConnection(2, true) && vault.CheckConnection(3, true) && vault.CheckConnection(4, false))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 1); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 0, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 1)
                    {
                        if (vault.CheckConnection(1, false) && vault.CheckConnection(2, false) && vault.CheckConnection(3, true) && vault.CheckConnection(4, true))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 1); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 1, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 2)
                    {
                        if (vault.CheckConnection(1, true) && vault.CheckConnection(2, false) && vault.CheckConnection(3, false) && vault.CheckConnection(4, true))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 2); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 3);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 2, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 3)
                    {
                        if (vault.CheckConnection(1, true) && vault.CheckConnection(2, true) && vault.CheckConnection(3, false) && vault.CheckConnection(4, false))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 3); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 3, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }

                    if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
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
            #endregion

            #region cleanup
            if (!devMode)
            {
                FastNoiseLite ores = new FastNoiseLite();
                ores.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                ores.SetFrequency(0.04f);
                ores.SetFractalType(FastNoiseLite.FractalType.FBm);
                ores.SetFractalOctaves(3);

                for (int y = vault.area.Top; y <= vault.area.Bottom; y++)
                {
                    for (int x = vault.area.Left; x <= vault.area.Right; x++)
                    {
                        Tile tile = MiscTools.Tile(x, y);

                        if (MiscTools.Solid(x, y) && tile.TileType != ModContent.TileType<VaultPipe>() && tile.TileType != TileID.Ash && tile.TileType != ModContent.TileType<VaultPlatform>())
                        {
                            if (tile.TileType == ModContent.TileType<VaultPlating>() || tile.TileType == TileID.ConveyorBeltLeft || tile.TileType == TileID.ConveyorBeltRight)
                            {
                                tile.WallType = (ushort)ModContent.WallType<VaultWallUnsafe>();
                            }
                            else tile.WallType = (ushort)ModContent.WallType<vault>();
                        }

                        if (tile.TileType == ModContent.TileType<Hardstone>())
                        {
                            if (ores.GetNoise(x, y) > -0.4f && ores.GetNoise(x, y) < -0.25f)
                            {
                                tile.TileType = TileID.Adamantite;
                            }
                            else if (ores.GetNoise(x, y) > 0.25f && ores.GetNoise(x, y) < 0.4f)
                            {
                                tile.TileType = TileID.Titanium;
                            }
                        }
                        else if (tile.TileType == TileID.Ash)
                        {
                            if (tile.LiquidAmount > 0)
                            {
                                WorldGen.KillTile(x, y);
                            }
                            else if (!MiscTools.Tile(x, y - 1).HasTile || MiscTools.Tile(x, y - 1).TileType != TileID.Ash)
                            {
                                bool left = !MiscTools.Tile(x - 1, y).HasTile || MiscTools.Tile(x - 1, y).TileType == ModContent.TileType<VaultPipe>();
                                bool right = !MiscTools.Tile(x + 1, y).HasTile || MiscTools.Tile(x + 1, y).TileType == ModContent.TileType<VaultPipe>();

                                if (left && right)
                                {
                                    MiscTools.Tile(x, y).IsHalfBlock = true;
                                }
                                else if (left)
                                {
                                    MiscTools.Tile(x, y).Slope = SlopeType.SlopeDownRight;
                                }
                                else if (right)
                                {
                                    MiscTools.Tile(x, y).Slope = SlopeType.SlopeDownLeft;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region objects
            if (!devMode)
            {
                int objects;

                objects = vault.grid.Height * vault.grid.Width * 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(vault.area.Left, vault.area.Right);
                    int y = WorldGen.genRand.Next(vault.area.Top, vault.area.Bottom);

                    bool valid = true;
                    if (MiscTools.Tile(x, y + 1).TileType != ModContent.TileType<VaultPlating>() || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneLarge>())
                    {
                        valid = false;
                    }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, ModContent.TileType<DeadDroneLarge>(), style: WorldGen.genRand.NextBool(10) ? Main.rand.Next(4, 6) : Main.rand.Next(4));
                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneLarge>())
                        {
                            objects--;
                        }
                    }
                }
                objects = vault.grid.Height * vault.grid.Width * 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(vault.area.Left, vault.area.Right);
                    int y = WorldGen.genRand.Next(vault.area.Top, vault.area.Bottom);

                    bool valid = true;
                    if (MiscTools.Tile(x, y + 1).TileType != ModContent.TileType<VaultPlating>() || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneSmall>())
                    {
                        valid = false;
                    }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, ModContent.TileType<DeadDroneSmall>(), style: Main.rand.Next(4));
                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneSmall>())
                        {
                            objects--;
                        }
                    }
                }
            }
            #endregion
        }

        #region functions
        //private void AshPiles()
        //{
        //    int structureCount = vault.grid.Height * vault.grid.Width;

        //    while (structureCount > 0)
        //    {
        //        int x = WorldGen.genRand.Next(location.Left + 21, location.Right - 20);
        //        int y = WorldGen.genRand.Next(location.Top + 21, location.Bottom - 20);

        //        if (!WGTools.Tile(x, y).HasTile && WGTools.Solid(x, y + 1) && WGTools.Tile(x, y).LiquidAmount == 0 && WGTools.Tile(x, y + 1).TileType != ModContent.TileType<VaultPipe>())
        //        {
        //            WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 9) * 2, 1, TileID.Ash, addTile: true, overRide: false);

        //            structureCount--;
        //        }
        //    }

        //    for (int y = location.Bottom; y >= location.Top; y--)
        //    {
        //        for (int x = location.Left; x <= location.Right; x++)
        //        {
        //            if (WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).TileType == TileID.Ash)
        //            {
        //                bool killTile = false;
        //                for (int i = -1; i <= 1; i++)
        //                {
        //                    if (!WGTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 1).TileType] || WGTools.Tile(x + i, y + 1).TileType == ModContent.TileType<VaultPipe>())
        //                    {
        //                        killTile = true;
        //                        break;
        //                    }
        //                }
        //                if (killTile)
        //                {
        //                    WorldGen.KillTile(x, y);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        private bool SpikeValidation(int x, int y)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (!MiscTools.Tile(i, y).HasTile || MiscTools.Tile(i, y).TileType == ModContent.TileType<VaultPlatform>())
                {
                    return false;
                }
            }
            return MiscTools.Solid(x, y) && !MiscTools.Solid(x, y - 1) && MiscTools.Tile(x, y - 1).LiquidAmount == 0;
        }
    }
}
