using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Remnants.Content.World.BiomeGeneration;
using Remnants.Content.Walls;
using Remnants.Content.Tiles;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Walls.Vanity;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects;
using Remnants.Content.Tiles.Objects.Furniture;
using static Remnants.Content.World.BiomeMap;
using System.Reflection;
using rail;
using Microsoft.CodeAnalysis;
using Remnants.Content.Tiles.Objects.Hazards;
using Remnants.Content.Tiles.Objects.Decoration;
using SteelSeries.GameSense;
using Remnants.Content.Tiles.Plants;

namespace Remnants.Content.World
{
    public class StructurePasses : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Surface Chests"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Buried Chests"));

            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (genIndex != -1)
            {
                RemWorld.InsertPass(tasks, new ThermalRigs("Thermal Engines", 0), genIndex + 1);
                RemWorld.InsertPass(tasks, new GemCaves("Gem Caves", 0), genIndex);
                RemWorld.InsertPass(tasks, new IceTemples("Ice Temples", 0), genIndex);
                RemWorld.InsertPass(tasks, new Microdungeons("Microdungeons", 0), genIndex);
            }

            RemWorld.InsertPass(tasks, new DesertRuins("Desert Ruins", 100), RemWorld.FindIndex(tasks, "Pyramids"), true);
            RemWorld.InsertPass(tasks, new Beehives("Beehives", 100), RemWorld.FindIndex(tasks, "Hives"), true);

            RemWorld.InsertPass(tasks, new Mineshafts("Mineshafts", 1), RemWorld.FindIndex(tasks, "Living Trees"));
            RemWorld.InsertPass(tasks, new GiantTrees("Giant Trees", 1), RemWorld.FindIndex(tasks, "Living Trees"), true);
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Wood Tree Walls"));


            if (!ModContent.GetInstance<Worldgen>().ExperimentalWorldgen)
            {
                RemWorld.InsertPass(tasks, new HellStructures("Hell Structures", 0), RemWorld.FindIndex(tasks, "Underworld") + 1);
            }

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Hellforge"));

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Jungle Chests"));
        }
    }

    public class Mineshafts : GenPass
    {
        public Mineshafts(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Mineshafts");

            int structureCount = 0;
            while (structureCount < 1)
            {
                StructureTools.Dungeon mines = new StructureTools.Dungeon(Main.maxTilesX / 2, (int)Main.worldSurface + 30, (int)((Main.maxTilesX / 840f) / 2) * 2 + 1, (int)(Main.maxTilesY / 600f) + 1, 36, 36, 5);

                mines.X = WorldGen.genRand.NextBool(2) ? (int)(Main.maxTilesX * 0.425f) : (int)(Main.maxTilesX * 0.575f) - mines.area.Width;

                if (true)//GenVars.structures.CanPlace(mines.area))
                {
                    GenVars.structures.AddProtectedStructure(mines.area, 25);

                    MiscTools.Rectangle(mines.area.Left, mines.area.Top, mines.area.Right - 1, mines.area.Bottom - 1, TileID.Dirt);
                    StructureTools.FillEdges(mines.area.Left, mines.area.Top, mines.area.Right - 1, mines.area.Bottom - 1, ignoreTop: false);

                    #region entrance
                    int entranceX = mines.area.Center.X - 1;
                    int entranceY = (int)(Main.worldSurface * 0.5f);
                    bool left = true;
                    while (!WorldGen.SolidTile(entranceX - 32, entranceY) || !WorldGen.SolidTile(entranceX + 32, entranceY))
                    {
                        if (WorldGen.SolidTile(entranceX - 32, entranceY))
                        {
                            left = false;
                        }

                        entranceY++;
                    }

                    FastNoiseLite hill = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                    hill.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                    hill.SetFrequency(0.1f);
                    hill.SetFractalType(FastNoiseLite.FractalType.FBm);
                    hill.SetFractalOctaves(3);

                    MiscTools.CustomTileRunner(entranceX, entranceY, 32, hill, TileID.Dirt, strength: 8, replace: false);
                    MiscTools.CustomTileRunner(entranceX, entranceY, 30, hill, TileID.Stone, WallID.RocksUnsafe1, strength: 8);

                    for (int y = entranceY; y <= mines.area.Top; y++)
                    {
                        for (int x = (int)(entranceX - 4 + hill.GetNoise(0, y) * 4); x < entranceX + 4 + hill.GetNoise(0, -y) * 4 + 1; x++)
                        { 
                            WorldGen.KillTile(x, y);
                            if (x == entranceX && y <= mines.area.Top + 1)
                            {
                                WorldGen.PlaceTile(x, y, TileID.Chain);
                            }
                        }
                    }

                    for (int x = left ? entranceX - 32 : entranceX + 17; x <= (left ? entranceX - 17 : entranceX + 32); x++)
                    {
                        for (int y = (int)(entranceY - 7 + hill.GetNoise(x, 0) * 4); y < entranceY - 1 + hill.GetNoise(-x, 0) * 4; y++)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }

                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/Mines/entrance", left ? 0 : 1, new Point16(entranceX - 16, entranceY - 19), ModContent.GetInstance<Remnants>());

                    #endregion

                    #region rooms
                    int roomCount = 0;
                    while (roomCount < mines.grid.Width * mines.grid.Height / 8)
                    {
                        int width = 1;// Main.maxTilesX / 2100;
                        mines.targetCell.X = WorldGen.genRand.Next(mines.grid.Left, mines.grid.Right + 1 - width);
                        mines.targetCell.Y = WorldGen.genRand.Next(0, mines.grid.Bottom);

                        bool valid = true;

                        if (mines.targetCell.X == mines.grid.Center.X && mines.targetCell.Y == 0)
                        {
                            valid = false;
                        }
                        else if (mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y - 1) && !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || mines.FindMarker(mines.targetCell.X + width, mines.targetCell.Y - 1) && !mines.FindMarker(mines.targetCell.X + width - 1, mines.targetCell.Y - 1) || mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y + 1) && !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || mines.FindMarker(mines.targetCell.X + width, mines.targetCell.Y + 1) && !mines.FindMarker(mines.targetCell.X + width - 1, mines.targetCell.Y + 1))
                        {
                            valid = false;
                        }
                        else
                        {
                            for (int i = mines.targetCell.X; i < mines.targetCell.X + width; i++)
                            {
                                if (mines.FindMarker(i, mines.targetCell.Y))
                                {
                                    valid = false;
                                }

                                if (!valid)
                                {
                                    break;
                                }
                            }
                        }

                        if (valid)
                        {
                            for (int i = mines.targetCell.X; i < mines.targetCell.X + width; i++)
                            {
                                mines.AddMarker(i, mines.targetCell.Y);
                                mines.AddMarker(i, mines.targetCell.Y, 1); mines.AddMarker(i, mines.targetCell.Y, 3);
                            }
                            mines.AddMarker(mines.targetCell.X + width - 1, mines.targetCell.Y, 2);
                            mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                            roomCount++;
                        }
                    }

                    int roomID = 0;
                    int attempts = 0;
                    while (attempts < 100000)
                    {
                        mines.targetCell.X = WorldGen.genRand.Next(mines.grid.Left, mines.grid.Right);
                        mines.targetCell.Y = WorldGen.genRand.Next(0, mines.grid.Bottom);
                        if (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y))
                        {
                            bool openLeft = mines.targetCell.X > mines.grid.Left && (!mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y) || !mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y, 2));
                            bool openRight = mines.targetCell.X < mines.grid.Right - 1 && (!mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y) || !mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y, 4));
                            bool openTop = (mines.targetCell.Y > 0 || mines.targetCell.X == mines.grid.Center.X) && (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1, 3));
                            bool openBottom = mines.targetCell.Y < mines.grid.Bottom - 1 && (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1, 1));

                            bool closedLeft = mines.targetCell.X == mines.grid.Left || !mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y) || mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y, 2);
                            bool closedRight = mines.targetCell.X == mines.grid.Right - 1 || !mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y) || mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y, 4);
                            bool closedTop = mines.targetCell.Y == 0 && mines.targetCell.X != mines.grid.Center.X || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1, 3);
                            bool closedBottom = mines.targetCell.Y == mines.grid.Bottom - 1 || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1, 1);

                            if (roomID == 0)
                            {
                                if (openLeft && openRight && closedTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/Mines/ew", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            if (roomID == 1)
                            {
                                if (closedLeft && closedRight && openTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/Mines/ns", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            if (roomID == 2)
                            {
                                if (closedLeft && openRight && openTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/ne", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            if (roomID == 3)
                            {
                                if (closedLeft && openRight && closedTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/Mines/es", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            if (roomID == 4)
                            {
                                if (openLeft && closedRight && closedTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/sw", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            if (roomID == 5)
                            {
                                if (openLeft && closedRight && openTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/Mines/nw", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y))
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

                    roomID = 0;
                    attempts = 0;
                    while (attempts < 100000)
                    {
                        mines.targetCell.X = WorldGen.genRand.Next(mines.grid.Left, mines.grid.Right);
                        mines.targetCell.Y = WorldGen.genRand.Next(0, mines.grid.Bottom);
                        if (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y))
                        {
                            bool openLeft = mines.targetCell.X > mines.grid.Left && (!mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y) || !mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y, 2));
                            bool openRight = mines.targetCell.X < mines.grid.Right - 1 && (!mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y) || !mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y, 4));
                            bool openTop = (mines.targetCell.Y > 0 || mines.targetCell.X == mines.grid.Center.X) && (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1, 3));
                            bool openBottom = mines.targetCell.Y < mines.grid.Bottom - 1 && (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1, 1));

                            bool closedLeft = mines.targetCell.X == mines.grid.Left || !mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y) || mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y, 2);
                            bool closedRight = mines.targetCell.X == mines.grid.Right - 1 || !mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y) || mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y, 4);
                            bool closedTop = mines.targetCell.Y == 0 && mines.targetCell.X != mines.grid.Center.X || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1, 3);
                            bool closedBottom = mines.targetCell.Y == mines.grid.Bottom - 1 || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1, 1);

                            if (roomID == 0)
                            {
                                if (openLeft && openRight && openTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/new", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 1)
                            {
                                if (openLeft && openRight && closedTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/esw", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 2)
                            {
                                if (closedLeft && openRight && openTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/nes", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 3)
                            {
                                if (openLeft && closedRight && openTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/nsw", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y))
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
                    while (attempts < 100000)
                    {
                        mines.targetCell.X = WorldGen.genRand.Next(mines.grid.Left, mines.grid.Right);
                        mines.targetCell.Y = WorldGen.genRand.Next(0, mines.grid.Bottom);
                        if (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y))
                        {
                            bool openLeft = mines.targetCell.X > mines.grid.Left && (!mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y) || !mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y, 2));
                            bool openRight = mines.targetCell.X < mines.grid.Right - 1 && (!mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y) || !mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y, 4));
                            bool openTop = (mines.targetCell.Y > 0 || mines.targetCell.X == mines.grid.Center.X) && (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1, 3));
                            bool openBottom = mines.targetCell.Y < mines.grid.Bottom - 1 && (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1, 1));

                            bool closedLeft = mines.targetCell.X == mines.grid.Left || !mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y) || mines.FindMarker(mines.targetCell.X - 1, mines.targetCell.Y, 2);
                            bool closedRight = mines.targetCell.X == mines.grid.Right - 1 || !mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y) || mines.FindMarker(mines.targetCell.X + 1, mines.targetCell.Y, 4);
                            bool closedTop = mines.targetCell.Y == 0 && mines.targetCell.X != mines.grid.Center.X || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1) || mines.FindMarker(mines.targetCell.X, mines.targetCell.Y - 1, 3);
                            bool closedBottom = mines.targetCell.Y == mines.grid.Bottom - 1 || !mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1) || mines.FindMarker(mines.targetCell.X, mines.targetCell.Y + 1, 1);

                            if (roomID == 0)
                            {
                                if (openLeft && openRight && openTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/nesw", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 1)
                            {
                                if (closedLeft && closedRight && openTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/n", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 2)
                            {
                                if (closedLeft && openRight && closedTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/e", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 3)
                            {
                                if (closedLeft && closedRight && closedTop && openBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 4);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/s", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 4)
                            {
                                if (openLeft && closedRight && closedTop && closedBottom)
                                {
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y);
                                    mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 1); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 2); mines.AddMarker(mines.targetCell.X, mines.targetCell.Y, 3);

                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/w", mines.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (!mines.FindMarker(mines.targetCell.X, mines.targetCell.Y))
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
                        if (roomID >= 5)
                        {
                            roomID = 0;
                        }
                    }
                    #endregion

                    MiscTools.WoodenBeam(entranceX - 4, entranceY);
                    MiscTools.WoodenBeam(entranceX + 4, entranceY);

                    StructureTools.AddVariation(mines.area);

                    int lengthSubtract = 0;

                    for (int y = mines.area.Top; y <= mines.area.Bottom; y++)
                    {
                        for (int x = mines.area.Left; x <= mines.area.Right; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (tile.WallType == WallID.Wood || tile.WallType == ModContent.WallType<WoodSafe>())
                            {
                                tile.WallType = (ushort)ModContent.WallType<Wood>();
                            }

                            if (tile.TileType == TileID.ClayBlock)
                            {
                                tile.TileType = TileID.Dirt;
                            }

                            //if (tile.WallType != ModContent.WallType<Wood>() && tile.WallType != ModContent.WallType<WoodLattice>())
                            //{
                            //    bool nearTiles = false;
                            //    for (int i = x - 1; i <= x + 1; i++)
                            //    {
                            //        for (int j = y - WorldGen.genRand.Next(2, 4); j <= y + WorldGen.genRand.Next(1, 3); j++)
                            //        {
                            //            if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.Stone || Main.tile[i, j].TileType == TileID.Dirt))
                            //            {
                            //                nearTiles = true;
                            //            }
                            //        }
                            //    }
                            //    if (!nearTiles && walls.GetNoise(x, y / 2f) < -0.75f)
                            //    {
                            //        tile.WallType = 0;
                            //    }
                            //    else tile.WallType = WallID.RocksUnsafe1;
                            //}


                            bool exposed = false;
                            for (int i = x - 2; i <= x + 2; i++)
                            {
                                for (int j = y - 2; j <= y + 2; j++)
                                {
                                    if (!Main.tile[i, j].HasTile)
                                    {
                                        exposed = true;
                                    }
                                }
                            }
                            if (biomes.MaterialBlend(x, y, frequency: 2) >= (exposed ? 0 : 0.2f))
                            {
                                if (tile.TileType == TileID.Dirt)
                                {
                                    tile.TileType = TileID.Stone;
                                }
                                if (tile.WallType == WallID.DirtUnsafe)
                                {
                                    tile.WallType = WallID.RocksUnsafe1;
                                }
                            }
                            else
                            {
                                if (tile.TileType == TileID.Stone)
                                {
                                    tile.TileType = TileID.Dirt;
                                }
                                if (tile.WallType == WallID.RocksUnsafe1 || tile.WallType == WallID.Rocks1Echo)
                                {
                                    tile.WallType = WallID.Cave6Unsafe;
                                }
                            }

                            if (MiscTools.HasTile(x, y, TileID.Chain))
                            {
                                if (!MiscTools.HasTile(x, y - 1, TileID.Chain))
                                {
                                    lengthSubtract = WorldGen.genRand.Next(0, 12);
                                }

                                if (!MiscTools.Tile(x, y + 8 + lengthSubtract).HasTile && !MiscTools.HasTile(x, y + 24, ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>()))
                                {
                                    if (MiscTools.HasTile(x, y + 1, TileID.Platforms))
                                    {
                                        WorldGen.PlaceTile(x, y + 2, TileID.Chain);
                                    }
                                    else WorldGen.PlaceTile(x, y + 1, TileID.Chain);
                                }
                            }
                        }
                    }

                    for (int y = mines.area.Bottom; y >= mines.area.Top; y--)
                    {
                        for (int x = mines.area.Left; x <= mines.area.Right; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (MiscTools.HasTile(x, y, TileID.Chain))
                            {
                                for (int j = y + 1; j <= y + 8; j++)
                                {
                                    if (WorldGen.SolidTile(x, j))
                                    {
                                        WorldGen.KillTile(x, y);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //for (int y = entranceY - 19; y <= mines.area.Bottom; y++)
                    //{
                    //    for (int x = mines.area.Left; x <= mines.area.Right; x++)
                    //    {
                    //        Tile tile = Main.tile[x, y];

                    //        if (MiscTools.HasTile(x, y, TileID.Chain))
                    //        {
                    //            //if (!MiscTools.HasTile(x, y - 1, TileID.Chain) && !MiscTools.HasTile(x, y - 1, TileID.Platforms) && !MiscTools.HasTile(x, y - 1, TileID.IronBrick) && !MiscTools.HasTile(x, y - 1, TileID.WoodBlock))
                    //            //{
                    //            //    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/motor", new Point16(x - 1, y - 2), ModContent.GetInstance<Remnants>());
                    //            //}
                    //            if (!WorldGen.SolidTile3(x - 4, y - 3) && !WorldGen.SolidTile3(x + 4, y - 3) && !WorldGen.SolidTile3(x - 4, y + 3) && !WorldGen.SolidTile3(x + 4, y + 3) && !MiscTools.HasTile(x, y + 1, TileID.Chain) && !MiscTools.HasTile(x, y + 1, TileID.Platforms) && !MiscTools.HasTile(x, y + 1, TileID.WoodBlock))
                    //            {
                    //                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Mines/platform", new Point16(x - 3, y), ModContent.GetInstance<Remnants>());
                    //            }
                    //        }
                    //    }
                    //}

                    int[] blocksToReplace = new int[] { TileID.Dirt, TileID.Stone, TileID.Copper };

                    int copper = (mines.area.Width * mines.area.Height) / (15 * 100);
                    while (copper > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);
                        int size = WorldGen.genRand.Next(16, 24);

                        bool valid = true;
                        for (int j = y - size / 2; j <= y + size / 2; j++)
                        {
                            for (int i = x - size / 3; i <= x + size / 3; i++)
                            {
                                if (MiscTools.HasTile(i, j, TileID.WoodBlock) || MiscTools.HasTile(i, j, ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>()))
                                {
                                    valid = false;
                                }
                            }
                        }
                        if (valid && WorldGen.SolidTile3(x, y) && !MiscTools.HasTile(x, y, TileID.Copper))
                        {
                            Ores.OreVein(x, y, size, 0, TileID.Copper, blocksToReplace, 20, 0.4f, 4, 3);

                            copper--;
                        }
                    }

                    int iron = (mines.area.Width * mines.area.Height) / (45 * 100);
                    while (iron > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);
                        int size = WorldGen.genRand.Next(24, 36);

                        bool valid = true;
                        for (int j = y - size / 2; j <= y + size / 2; j++)
                        {
                            for (int i = x - size / 3; i <= x + size / 3; i++)
                            {
                                if (MiscTools.HasTile(i, j, TileID.WoodBlock) || MiscTools.HasTile(i, j, ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>()))
                                {
                                    valid = false;
                                }
                            }
                        }
                        if (valid && WorldGen.SolidTile3(x, y) && !MiscTools.HasTile(x, y, TileID.Iron))
                        {
                            Ores.OreVein(x, y, size, 0, TileID.Iron, blocksToReplace, 20, 0.6f, 6, 4);

                            iron--;
                        }
                    }

                    #region objects
                    int objects = (int)(mines.grid.Height * (Main.maxTilesX / 4200f));
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom + 1);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && MiscTools.Tile(x, y + 1).TileType == TileID.WoodBlock && MiscTools.Tile(x + 1, y + 1).TileType == TileID.WoodBlock && MiscTools.Tile(x - 1, y).TileType != TileID.WoodenBeam && MiscTools.Tile(x + 2, y).TileType != TileID.WoodenBeam)
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                            {
                                #region chest
                                var itemsToAdd = new List<(int type, int stack)>();

                                int[] specialItems = new int[5];
                                specialItems[0] = ItemID.ShoeSpikes;
                                specialItems[1] = ItemID.Aglet;
                                specialItems[2] = ItemID.Radar;
                                specialItems[3] = ItemID.Spear;
                                specialItems[4] = ItemID.PortableStool;

                                int specialItem = specialItems[(objects - 1) % specialItems.Length];
                                itemsToAdd.Add((specialItem, 1));

                                StructureTools.GenericLoot(chestIndex, itemsToAdd, 1, new int[] { ItemID.MiningPotion, ItemID.ShinePotion });

                                if (Main.rand.NextBool(2))
                                {
                                    itemsToAdd.Add((biomes.FindBiome(x, y) == BiomeID.Jungle ? ItemID.LeadOre : ItemID.IronOre, Main.rand.Next(15, 45)));
                                }
                                else itemsToAdd.Add((biomes.FindBiome(x, y) == BiomeID.Jungle ? ItemID.TinOre : ItemID.CopperOre, Main.rand.Next(30, 90)));

                                StructureTools.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }

                    objects = mines.grid.Height * mines.grid.Width / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.LargePiles2 && Framing.GetTileSafely(x, y + 1).TileType == TileID.WoodBlock)
                        {
                            WorldGen.PlaceObject(x, y, TileID.LargePiles2, style: 23);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles2)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = mines.grid.Height * mines.grid.Width / 4;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.LargePiles2 && Framing.GetTileSafely(x, y + 1).TileType != TileID.WoodBlock && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms)
                        {
                            WorldGen.PlaceObject(x, y, TileID.LargePiles2, style: 27);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles2)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = mines.grid.Height * mines.grid.Width / 4;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.LargePiles2 && Framing.GetTileSafely(x, y + 1).TileType != TileID.WoodBlock && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms)
                        {
                            WorldGen.PlaceObject(x, y, TileID.LargePiles2, style: 25);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles2)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = mines.grid.Height * mines.grid.Width / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.LargePiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms)
                        {
                            WorldGen.PlaceObject(x, y, TileID.LargePiles, style: Main.rand.Next(7, 15));
                            if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = mines.grid.Height * mines.grid.Width / 4;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType == TileID.WoodBlock)
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 1);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = mines.grid.Height * mines.grid.Width * 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType == TileID.WoodBlock)
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(12), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    objects = mines.grid.Height * mines.grid.Width * 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(mines.area.Left, mines.area.Right);
                        int y = WorldGen.genRand.Next(mines.area.Top, mines.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 33), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    #endregion

                    structureCount++;
                }
            }
        }
    }

    public class GiantTrees : GenPass
    {
        public GiantTrees(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        int X;
        int Y;

        int UpperLimit;
        int LowerLimit;

        int EntranceX;
        int EntranceY;

        FastNoiseLite roughness;
        FastNoiseLite distortion;
        FastNoiseLite overgrowth;
        FastNoiseLite walls;
        FastNoiseLite leaves;

        Rectangle bounds;

        List<Point16> roomLocations;
        List<Point16> platformLocations;
        List<Point16> ropeLocations;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.LivingTrees");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int countInitial = 1;
            int count = countInitial;
            while (count > 0)
            {
                X = countInitial == 1 ? Main.maxTilesX / 2 : WorldGen.genRand.Next(600, Main.maxTilesX - 600);
                Y = (int)(Main.worldSurface * 0.5f);

                while (!WorldGen.SolidTile(X - 12, Y) && !WorldGen.SolidTile(X + 12, Y))
                {
                    Y++;
                }

                UpperLimit = Y + 50;
                LowerLimit = (int)Main.rockLayer;

                roughness = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                roughness.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                roughness.SetFrequency(0.1f);
                roughness.SetFractalType(FastNoiseLite.FractalType.FBm);
                roughness.SetFractalOctaves(3);

                distortion = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                distortion.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                distortion.SetFrequency(0.01f);
                distortion.SetFractalType(FastNoiseLite.FractalType.FBm);
                distortion.SetFractalOctaves(3);

                overgrowth = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                overgrowth.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                overgrowth.SetFrequency(0.1f);
                overgrowth.SetFractalType(FastNoiseLite.FractalType.FBm);
                overgrowth.SetFractalOctaves(3);

                walls = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                walls.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                walls.SetFrequency(0.1f);
                walls.SetFractalType(FastNoiseLite.FractalType.None);
                walls.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);

                leaves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                leaves.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                leaves.SetFrequency(0.05f);
                leaves.SetFractalType(FastNoiseLite.FractalType.FBm);
                leaves.SetFractalOctaves(3);

                bounds = new Rectangle(X, Y, 1, (int)Main.rockLayer - Y);

                roomLocations = new List<Point16>();
                platformLocations = new List<Point16>();
                ropeLocations = new List<Point16>();

                Root(new Vector2(X, Y + 25), 24, Vector2.UnitY);

                EntranceY = (int)Math.Min(Y, Y - 10 + GetDistortion(new Vector2(X, Y + 25), 1).Y) - 3;
                EntranceX = (int)(X + GetDistortion(new Vector2(X, EntranceY), 1).X);

                Branch(new Vector2(X, Y + 25), 12, -Vector2.UnitY);

                int lanterns;
                int attempts;
                int lanternX;
                int lanternY;
                int rooms = (int)(Main.maxTilesX / 2100f * Main.maxTilesY / 1200f);
                int woodWand = WorldGen.genRand.Next(rooms);
                while (rooms > 0 && roomLocations.Count > 0)
                {
                    int radius = 32;

                    Point16 position = roomLocations[WorldGen.genRand.Next(roomLocations.Count)];
                    while ((position.Y > Main.worldSurface || MathHelper.Distance(X, position.X) < (bounds.Width) / 3) && !WorldGen.genRand.NextBool(100))
                    {
                        position = roomLocations[WorldGen.genRand.Next(roomLocations.Count)];
                    }

                    if (GenVars.structures.CanPlace(new Rectangle((int)(position.X - radius / 2), (int)(position.Y - radius / 2), (int)(radius), (int)(radius)), (int)radius) && MathHelper.Distance(X, position.X) > (bounds.Width) / 6)
                    {
                        Carve(position.ToVector2(), radius, true);

                        for (int j = (int)(position.Y - radius / 1.5f); j <= (position.Y + radius / 1.5f); j++)
                        {
                            for (int i = (position.X - radius + 8); i <= (position.X + radius - 8); i++)
                            {
                                Tile tile = Main.tile[i, j];

                                if (tile.TileType == ModContent.TileType<nothing>())
                                {
                                    tile.TileType = TileID.LivingWood;
                                    tile.HasTile = false;
                                }
                            }
                        }

                        List<int> widths = new List<int>();
                        for (int j = (position.Y + radius / 6); j > position.Y - radius / 6; j--)
                        {
                            int left = 0;
                            for (int i = position.X; !MiscTools.Tile(i, j).HasTile || MiscTools.Tile(i, j).TileType == ModContent.TileType<nothing>(); i--)
                            {
                                left = i;
                            }
                            int right = 0;
                            for (int i = position.X; !MiscTools.Tile(i, j).HasTile || MiscTools.Tile(i, j).TileType == ModContent.TileType<nothing>(); i++)
                            {
                                right = i;
                            }

                            widths.Add(right - left);
                        }
                        int width = widths.Min();
                        int height = (position.Y + radius / 6) - widths.IndexOf(width);
                        MiscTools.Rectangle((position.X - radius / 2), height, (position.X + radius / 2), height, TileID.WoodBlock, add: false);
                        MiscTools.Rectangle((position.X - radius / 2), height, (position.X + radius / 2), height, TileID.Platforms, style: 23, replace: false);

                        MiscTools.Rectangle((position.X - radius / 2 + 1), height + 2, (position.X + radius / 2 - 1), height + 2, wall: WallID.LivingWoodUnsafe);

                        MiscTools.Tile((position.X - radius / 6), height).TileType = TileID.WoodBlock;
                        MiscTools.WoodenBeam((position.X - radius / 6), height);
                        MiscTools.Tile((position.X + radius / 6), height).TileType = TileID.WoodBlock;
                        MiscTools.WoodenBeam((position.X + radius / 6), height);

                        if (MiscTools.HasTile((position.X - radius / 2 + 1), height, TileID.Platforms))
                        {
                            MiscTools.Tile((position.X - radius / 2 + 1), height).TileType = TileID.WoodBlock;
                            MiscTools.WoodenBeam((position.X - radius / 2 + 1), height);
                        }
                        if (MiscTools.HasTile((position.X + radius / 2 - 1), height, TileID.Platforms))
                        {
                            MiscTools.Tile((position.X + radius / 2 - 1), height).TileType = TileID.WoodBlock;
                            MiscTools.WoodenBeam((position.X + radius / 2 - 1), height);
                        }

                        WorldGen.PlaceObject(position.X, height - 1, TileID.Tables, style: 6);
                        WorldGen.PlaceObject(position.X - 2, height - 1, TileID.Chairs, style: 5, direction: 1);
                        WorldGen.PlaceObject(position.X + 2, height - 1, TileID.Chairs, style: 5, direction: -1);

                        MiscTools.PlaceObjectsInArea((position.X - radius / 2), height - 1, (position.X + radius / 2), height - 1, TileID.LivingLoom);

                        MiscTools.PlaceObjectsInArea((int)(position.X - radius / 2.5f), height + 1, (int)(position.X + radius / 2.5f), (int)(position.Y + radius / 1.5f), ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>(), count: 16);

                        bool success = false;

                        while (!success)
                        {
                            int chestX = WorldGen.genRand.Next((int)(position.X - radius / 2), (int)(position.X + radius / 2));
                            int chestY = height - 1;

                            if (Framing.GetTileSafely(chestX, chestY).TileType != TileID.Containers)
                            {
                                int chestIndex = WorldGen.PlaceChest(chestX, chestY, style: 12, notNearOtherChests: true);
                                if (Framing.GetTileSafely(chestX, chestY).TileType == TileID.Containers)
                                {
                                    #region chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    int[] specialItems = new int[5];
                                    specialItems[0] = ItemID.ClimbingClaws;
                                    specialItems[1] = ItemID.WandofSparking;
                                    specialItems[2] = ItemID.BabyBirdStaff;
                                    specialItems[3] = ItemID.Blowpipe;
                                    specialItems[4] = ItemID.WoodenBoomerang;

                                    int specialItem = specialItems[(rooms - 1) % specialItems.Length];
                                    itemsToAdd.Add((specialItem, 1));
                                    if (specialItem == ItemID.Blowpipe)
                                    {
                                        itemsToAdd.Add((ItemID.Seed, Main.rand.Next(15, 30)));
                                    }
                                    if (rooms - 1 == woodWand)
                                    {
                                        itemsToAdd.Add((ItemID.LivingWoodWand, 1));
                                        itemsToAdd.Add((ItemID.LeafWand, 1));
                                    }

                                    itemsToAdd.Add((ItemID.CanOfWorms, Main.rand.Next(1, 3)));

                                    StructureTools.GenericLoot(chestIndex, itemsToAdd, 1, new int[] { ItemID.BuilderPotion, ItemID.NightOwlPotion });

                                    itemsToAdd.Add((ItemID.Wood, Main.rand.Next(50, 100)));

                                    StructureTools.FillChest(chestIndex, itemsToAdd);

                                    success = true;
                                    #endregion
                                }
                            }
                        }

                        lanterns = 0;
                        attempts = 0;
                        while (lanterns < 3 && attempts < 1000)
                        {
                            lanternX = WorldGen.genRand.Next((int)(position.X - radius / 3), (int)(position.X + radius / 3 + 1));
                            lanternY = height - 1;
                            while (!MiscTools.Tile(lanternX, lanternY - 1).HasTile)
                            {
                                lanternY--;
                            }

                            attempts++;

                            if (lanternY >= position.Y - radius / 1.5f)
                            {
                                if (HangingLantern(lanternX, lanternY))
                                {
                                    lanterns++;
                                    attempts = 0;
                                }
                            }
                        }

                        GenVars.structures.AddProtectedStructure(new Rectangle((int)(position.X - radius / 2), (int)(position.Y - radius / 2), (int)(radius), (int)(radius)));

                        rooms--;
                    }

                    roomLocations.Remove(position);
                }

                for (int j = (int)(Main.worldSurface * 0.5f); j < bounds.Bottom; j++)
                {
                    for (int i = bounds.Left; i < bounds.Right; i++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (tile.HasTile)
                        {
                            if (tile.TileType == ModContent.TileType<nothing>())
                            {
                                tile.TileType = TileID.LivingWood;
                                tile.HasTile = false;
                            }
                            else if (tile.TileType == TileID.LivingWood || tile.TileType == TileID.Grass)
                            {
                                if (MiscTools.SurroundingTilesActive(i, j, true))
                                {
                                    tile.WallType = WallID.LivingWoodUnsafe;
                                }
                            }
                        }
                    }
                }

                while (platformLocations.Count > 0)
                {
                    int width = WorldGen.genRand.Next(4, 9) * 2;

                    Point16 position = platformLocations[WorldGen.genRand.Next(platformLocations.Count)];

                    bool valid = true;

                    for (int i = position.X - width / 2; i <= position.X + width / 2; i++)
                    {
                        if ((WorldGen.SolidTile3(i, position.Y - 3) || WorldGen.SolidTile3(i, position.Y - 2)) && !WorldGen.SolidTile3(i, position.Y - 1))
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid && GenVars.structures.CanPlace(new Rectangle((position.X - width / 2), position.Y - 20, (width), 41)))
                    {
                        MiscTools.Rectangle((position.X - width / 2), position.Y, (position.X + width / 2), position.Y, TileID.WoodBlock, add: false);
                        MiscTools.Rectangle((position.X - width / 2), position.Y, (position.X + width / 2), position.Y, TileID.Platforms, style: 23, replace: false);

                        MiscTools.Rectangle((position.X - width / 2 + 1), position.Y + 2, (position.X + width / 2 - 1), position.Y + 2, wall: WallID.LivingWoodUnsafe);

                        if (MiscTools.HasTile((position.X - width / 2 + 1), position.Y, TileID.Platforms))
                        {
                            MiscTools.Tile((position.X - width / 2 + 1), position.Y).TileType = TileID.WoodBlock;
                            MiscTools.WoodenBeam((position.X - width / 2 + 1), position.Y);
                        }
                        if (MiscTools.HasTile((position.X + width / 2 - 1), position.Y, TileID.Platforms))
                        {
                            MiscTools.Tile((position.X + width / 2 - 1), position.Y).TileType = TileID.WoodBlock;
                            MiscTools.WoodenBeam((position.X + width / 2 - 1), position.Y);
                        }

                        //MiscTools.PlaceObjectsInArea((position.X - radius / 2), position.Y - 1, (position.X + radius / 2), position.Y - 1, TileID.Beds, 19);

                        GenVars.structures.AddProtectedStructure(new Rectangle(position.X - width / 2 - 1, position.Y - 1, width + 2, 3));
                    }

                    platformLocations.Remove(position);
                }

                #region entrance
                MiscTools.Rectangle(EntranceX - 20, EntranceY - 2, EntranceX + 20, EntranceY, -1);

                //MiscTools.PlaceObjectsInArea(EntranceX - 20, EntranceY, EntranceX - 1, EntranceY, TileID.ClosedDoor, style2: 7);
                //MiscTools.PlaceObjectsInArea(EntranceX + 1, EntranceY, EntranceX + 20, EntranceY, TileID.ClosedDoor, style2: 7);

                MiscTools.Rectangle(EntranceX - 15, EntranceY + 1, EntranceX + 15, EntranceY + 1, TileID.Platforms, replace: false, style: 23);
                MiscTools.Rectangle(EntranceX - 10, EntranceY + 3, EntranceX + 10, EntranceY + 3, wall: WallID.LivingWoodUnsafe, add: false);
                MiscTools.Tile(EntranceX, EntranceY + 1).TileType = TileID.WoodBlock;
                MiscTools.WoodenBeam(EntranceX, EntranceY + 1);

                if (MiscTools.HasTile(EntranceX - 14, EntranceY + 1, TileID.Platforms))
                {
                    MiscTools.Tile(EntranceX - 14, EntranceY + 1).TileType = TileID.WoodBlock;
                    MiscTools.WoodenBeam(EntranceX - 14, EntranceY + 1);
                }
                if (MiscTools.HasTile(EntranceX + 14, EntranceY + 1, TileID.Platforms))
                {
                    MiscTools.Tile(EntranceX + 14, EntranceY + 1).TileType = TileID.WoodBlock;
                    MiscTools.WoodenBeam(EntranceX + 14, EntranceY + 1);
                }


                lanterns = 0;
                attempts = 0;
                while (lanterns < 1 && attempts < 1000)
                {
                    lanternX = WorldGen.genRand.Next(EntranceX - 5, EntranceX + 5);
                    lanternY = EntranceY - 5;
                    while (!MiscTools.Tile(lanternX, lanternY - 1).HasTile)
                    {
                        lanternY--;
                    }

                    attempts++;

                    if (HangingLantern(lanternX, lanternY))
                    {
                        lanterns++;
                        attempts = 0;
                    }
                }
                #endregion

                for (int j = (int)(Main.worldSurface * 0.5f); j < bounds.Bottom; j++)
                {
                    for (int i = bounds.Left; i < bounds.Right; i++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (tile.HasTile && (tile.TileType == TileID.LivingWood || tile.TileType == TileID.Grass || tile.TileType == TileID.Dirt))
                        {
                            if (MiscTools.AdjacentTiles(i, j, true) <= (j <= EntranceY ? 1 : 0))
                            {
                                tile.TileType = (ushort)ModContent.TileType<nothing>();

                                if (MiscTools.HasTile(i, j - 1, ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>()))
                                {
                                    WorldGen.KillTile(i, j - 1);
                                }
                            }
                        }
                    }
                }

                for (int j = (int)(Main.worldSurface * 0.5f); j < bounds.Bottom; j++)
                {
                    for (int i = bounds.Left; i < bounds.Right; i++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (tile.HasTile)
                        {
                            if (tile.TileType == ModContent.TileType<nothing>())
                            {
                                tile.TileType = TileID.LivingWood;
                                tile.HasTile = false;
                            }
                        }
                    }
                }

                while (ropeLocations.Count > 0)
                {
                    Point16 position = ropeLocations[WorldGen.genRand.Next(ropeLocations.Count)];

                    int left = position.X;
                    for (int i = position.X; !MiscTools.Tile(i, position.Y).HasTile; i--)
                    {
                        left = i;
                    }
                    int right = position.X;
                    for (int i = position.X; !MiscTools.Tile(i, position.Y).HasTile; i++)
                    {
                        right = i;
                    }

                    bool valid = true;

                    if (MiscTools.HasTile(left - 1, position.Y, TileID.Rope) || MiscTools.HasTile(right + 1, position.Y, TileID.Rope) || MiscTools.HasTile(left - 1, position.Y, TileID.Platforms) || MiscTools.HasTile(right + 1, position.Y, TileID.Platforms) || MiscTools.HasTile(left - 1, position.Y, TileID.FireflyinaBottle) || MiscTools.HasTile(right + 1, position.Y, TileID.FireflyinaBottle))
                    {
                        valid = false;
                    }
                    else
                    {
                        for (int i = left; i <= right; i++)
                        {
                            if (MiscTools.Tile(i, position.Y + 1).HasTile)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }

                    if (valid && GenVars.structures.CanPlace(new Rectangle(left, position.Y, right - left, 6)) && right - left >= 4 && (right - left <= 24))
                    {
                        MiscTools.Rectangle(left, position.Y, right, position.Y, TileID.Rope, replace: false);

                        GenVars.structures.AddProtectedStructure(new Rectangle(left, position.Y - 20, right - left, 41));

                        WorldGen.PlaceTile(Math.Clamp(position.X + WorldGen.genRand.Next(left - right, right - left + 1) / 8, left + 1, right - 1), position.Y + 1, TileID.Rope);

                        if (MiscTools.HasTile(left - 1, position.Y, TileID.WoodenBeam))
                        {
                            MiscTools.Tile(left - 1, position.Y).TileType = TileID.WoodBlock;
                        }
                        if (MiscTools.HasTile(right + 1, position.Y, TileID.WoodenBeam))
                        {
                            MiscTools.Tile(right + 1, position.Y).TileType = TileID.WoodBlock;
                        }

                        if (right - left >= 6)
                        {
                            attempts = 0;
                            while (!HangingLantern(WorldGen.genRand.Next(left + 1, right), position.Y + 1, WorldGen.genRand.Next(3, 7)) && attempts < 1000)
                            {
                                attempts++;
                            }
                        }
                    }

                    ropeLocations.Remove(position);
                }

                for (int j = (int)(Main.worldSurface * 0.5f); j < bounds.Bottom; j++)
                {
                    for (int i = bounds.Left; i < bounds.Right; i++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LivingWood && !MiscTools.HasTile(i - 1, j, TileID.Platforms) && !MiscTools.HasTile(i + 1, j, TileID.Platforms) && !MiscTools.HasTile(i - 1, j, TileID.Rope) && !MiscTools.HasTile(i + 1, j, TileID.Rope) && !MiscTools.HasTile(i, j - 1, ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>()))
                        {
                            bool left = WorldGen.SolidOrSlopedTile(i - 1, j) && !WorldGen.SolidOrSlopedTile(i + 1, j);
                            bool right = WorldGen.SolidOrSlopedTile(i + 1, j) && !WorldGen.SolidOrSlopedTile(i - 1, j);
                            bool top = WorldGen.SolidOrSlopedTile(i, j - 1) && !WorldGen.SolidOrSlopedTile(i, j + 1);
                            bool bottom = WorldGen.SolidOrSlopedTile(i, j + 1) && !WorldGen.SolidOrSlopedTile(i, j - 1);

                            if (bottom)
                            {
                                if (!WorldGen.SolidOrSlopedTile(i - 1, j) && !WorldGen.SolidOrSlopedTile(i + 1, j) || (left || right) && WorldGen.genRand.NextBool(2) && (WorldGen.SolidTile(i - 1, j + 1) && WorldGen.SolidTile(i + 1, j + 1)))
                                {
                                    tile.IsHalfBlock = true;
                                }
                                else if (left)
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

                for (int j = (int)(Main.worldSurface * 0.5f); j < bounds.Bottom; j++)
                {
                    for (int i = bounds.Left; i < bounds.Right; i++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (tile.HasTile && tile.TileType == TileID.Rope)
                        {
                            tile = Main.tile[i, j - 1];

                            if (tile.HasTile && (tile.TileType == TileID.Rope || tile.TileType == TileID.Platforms))
                            {
                                if (!MiscTools.Tile(i, j + 4).HasTile || MiscTools.Tile(i, j + 4).TileType == TileID.Platforms)
                                {
                                    if (MiscTools.HasTile(i, j + 1, TileID.Platforms) && !MiscTools.Tile(i, j + 2).HasTile)
                                    {
                                        WorldGen.PlaceTile(i, j + 2, TileID.Rope);
                                    }
                                    else if (!MiscTools.Tile(i, j + 1).HasTile)
                                    {
                                        WorldGen.PlaceTile(i, j + 1, TileID.Rope);
                                    }
                                }
                            }
                        }
                    }
                }

                count--;
            }

            //int attempts = 0;
            //while (attempts < 1000)
            //{
            //    X = WorldGen.genRand.Next((Jungle.Left + 1) * biomes.Scale, Jungle.Right * biomes.Scale);
            //    Y = (int)(Main.worldSurface * 0.5f);

            //    while (!WorldGen.SolidTile(X - 5, Y) && !WorldGen.SolidTile(X + 5, Y))
            //    {
            //        Y++;
            //    }
            //}
        }

        private void Carve(Vector2 position, float radius, bool room = false, bool hollow = true)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            for (int j = (int)(position.Y - radius - 12); j <= (int)(position.Y + radius + 12); j++)
            {
                for (int i = (int)(position.X - radius - 12); i <= (int)(position.X + radius + 12); i++)
                {
                    if (i < bounds.Left)
                    {
                        bounds.Width += bounds.Left - i;
                        bounds.X -= bounds.Left - i;
                    }
                    if (i > bounds.Right)
                    {
                        bounds.X += i - bounds.Right;
                    }
                    if (j > bounds.Bottom)
                    {
                        bounds.Height += j - bounds.Bottom;
                    }

                    Tile tile = Main.tile[i, j];
                    float distance = Vector2.Distance(new Vector2(i, j), position) + roughness.GetNoise(i / (room ? 2f : 1), j / (room ? 2f : 1)) * (room ? 8 : 2);

                    if (distance <= radius + 8)
                    {
                        if (radius > 12 || j > Main.worldSurface && j < Main.rockLayer)
                        {
                            if (!tile.HasTile)
                            {
                                tile.TileType = TileID.Dirt;
                            }

                            tile.HasTile = true;
                        }

                        if (distance <= radius)
                        {
                            tile.HasTile = true;

                            if (tile.TileType != ModContent.TileType<nothing>() && tile.TileType != TileID.Grass && tile.TileType != TileID.WoodenBeam)
                            {
                                tile.TileType = TileID.LivingWood;
                            }

                            if (tile.WallType != ModContent.WallType<undergrowth>() && (radius >= 12 || distance <= radius / 2 + (radius < 8 ? 1 : 2)) && radius >= 4)
                            {
                                tile.WallType = WallID.LivingWoodUnsafe;
                            }

                            if (tile.TileType != ModContent.TileType<nothing>())
                            {
                                bool mushroom = biomes.FindBiome(i, j) == BiomeID.Glowshroom;
                                if ((room || overgrowth.GetNoise(i * (mushroom ? 2 : 1), j * (mushroom ? 2 : 1)) > 0) && (distance <= radius / 2 + 2 && radius >= 4 && position.Y >= Y || distance >= radius - 2 && radius < 12 && position.Y >= Y - 25))
                                {
                                    tile.TileType = mushroom ? TileID.MushroomGrass : j > Main.worldSurface ? TileID.Dirt : TileID.Grass;
                                }
                            }

                            if (hollow && distance <= radius / 2 && radius >= 4)
                            {
                                tile.TileType = (ushort)ModContent.TileType<nothing>();

                                tile.LiquidAmount = WorldGen.genRand.NextBool(10) || radius < 3 ? byte.MaxValue : byte.MinValue;

                                if (radius >= 8 && distance <= radius / 2 + walls.GetNoise(i, j) * 4)
                                {
                                    tile.WallType = (ushort)ModContent.WallType<undergrowth>();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Branch(Vector2 position, float radius, Vector2 direction)
        {
            for (int k = 0; k < 100 * (radius / 10); k++)
            {
                Vector2 distortionOffset = GetDistortion(position, 1);

                Carve(position + distortionOffset, radius, false, position.Y + distortionOffset.Y + radius / 2 >= EntranceY);

                if (MathHelper.Distance((int)position.Y + distortionOffset.Y, (int)(EntranceY - radius / 2)) <= 1)
                {
                    EntranceX = (int)(position.X + distortionOffset.X);
                }

                position += Vector2.Normalize(direction * new Vector2(1, 1.5f) - Vector2.UnitY * 0.25f) - Vector2.UnitY * 0.25f;
                radius -= 0.04f;

                if (position.Y + distortionOffset.Y >= EntranceY + 3)
                {
                    ropeLocations.Add((position + distortionOffset + Vector2.UnitX * (direction.X / direction.Y) * (radius / 4)).ToPoint16());
                }

                if (radius < 1)
                {
                    MiscTools.CustomTileRunner(position.X + GetDistortion(position, 1).X, position.Y + GetDistortion(position, 1).Y, WorldGen.genRand.NextFloat(24, 48), leaves, TileID.LeafBlock, strength: 3, yFrequency: 1.5f, replace: false);
                    MiscTools.CustomTileRunner(position.X + GetDistortion(position, 1).X + WorldGen.genRand.Next(-6, 7), position.Y + GetDistortion(position, 1).Y + WorldGen.genRand.Next(-6, 7), WorldGen.genRand.NextFloat(24, 48), leaves, wall: WallID.LivingLeaf, strength: 3, yFrequency: 1.5f, replace: false);
                    return;
                }
            }

            if (radius >= 1)
            {
                Branch(position, radius - WorldGen.genRand.NextFloat(1), direction.RotatedBy(WorldGen.genRand.NextFloat(-MathHelper.PiOver2, -MathHelper.PiOver4) * (0.5f + radius / 32)));
                Branch(position, radius - WorldGen.genRand.NextFloat(1), direction.RotatedBy(WorldGen.genRand.NextFloat(MathHelper.PiOver4, MathHelper.PiOver2) * (0.5f + radius / 32)));

                if (radius <= 8)
                {
                    MiscTools.CustomTileRunner(position.X + GetDistortion(position, 1).X, position.Y + GetDistortion(position, 1).Y, WorldGen.genRand.NextFloat(16, 32), leaves, TileID.LeafBlock, strength: 3, yFrequency: 1.5f, replace: false);
                    MiscTools.CustomTileRunner(((position.X + GetDistortion(position, 1).X) - X) * 0.75f + X, position.Y + GetDistortion(position, 1).Y - 12, WorldGen.genRand.NextFloat(16, 32), leaves, wall: WallID.LivingLeaf, strength: 3, yFrequency: 1.5f, replace: false);
                }
            }
        }

        private void Root(Vector2 position, float radius, Vector2 direction)
        {
            for (int k = 0; k < WorldGen.genRand.Next(60, 80) * (Main.maxTilesY / 3600f + 0.5f); k++)
            {
                Vector2 distortionOffset = new Vector2(distortion.GetNoise(position.X, position.Y + 999), distortion.GetNoise(position.X + 999, position.Y) * 1.5f) * 50;

                Carve(position + distortionOffset, radius);

                if (position.Y - radius <= UpperLimit)
                {
                    if (direction.Y < 0)
                    {
                        direction.X += position.X > X ? 0.025f : -0.025f;
                    }
                    direction.Y += 0.025f;
                }
                if (position.Y + radius >= LowerLimit)
                {
                    direction.Y -= 0.025f;
                }
                position += Vector2.Normalize(direction * new Vector2(1, 1.5f));
                radius -= 0.05f / (Main.maxTilesY / 1800f);

                if (position.Y > UpperLimit + 50 && position.Y < LowerLimit - 50)
                {
                    if (radius > 4 && radius < 16)
                    {
                        roomLocations.Add((position + distortionOffset).ToPoint16());
                    }
                }

                if (Math.Abs(direction.X) < Math.Abs(direction.Y))
                {
                    if (radius > 8)
                    {
                        ropeLocations.Add((position + distortionOffset + Vector2.UnitX * (direction.X / direction.Y) * (radius / 4)).ToPoint16());
                    }
                }
                else if (radius > 8)
                {
                    platformLocations.Add((position + distortionOffset).ToPoint16());
                }

                if (radius < 1 || !GenVars.structures.CanPlace(new Rectangle((int)(position.X + distortionOffset.X), (int)(position.Y + distortionOffset.Y), 1, 1)))
                {
                    return;
                }
            }

            if (radius >= 1)
            {
                Root(position, radius - WorldGen.genRand.NextFloat(1), (direction.ToRotation() - MathHelper.PiOver4 + (WorldGen.genRand.NextFloat(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2) * (0.5f + radius / 48))).ToRotationVector2());
                Root(position, radius - WorldGen.genRand.NextFloat(1), (direction.ToRotation() + MathHelper.PiOver4 + (WorldGen.genRand.NextFloat(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2) * (0.5f + radius / 48))).ToRotationVector2());
            }
        }

        private Vector2 GetDistortion(Vector2 position, float frequency)
        {
            return new Vector2(distortion.GetNoise(position.X * frequency, position.Y * frequency + 999), distortion.GetNoise(position.X * frequency + 999, position.Y * frequency) * 1.5f) * 50;
        }

        private bool HangingLantern(int x, int y, int length = 0)
        {
            //while (!MiscTools.Tile(x, y - 1).HasTile)
            //{
            //    y--;
            //}
            if (Main.tile[x, y].HasTile || MiscTools.HasTile(x, y - 1, ModContent.TileType<nothing>()) || Main.tile[x, y].WallType == 0)
            {
                return false;
            }

            bool adaptive = length == 0;

            if (adaptive)
            {
                while (!Main.tile[x, y + length].HasTile)
                {
                    length++;
                }
                length -= WorldGen.genRand.Next(6, 10);

                if (length < 1)
                {
                    return false;
                }
            }

            for (int j = y; j < y + length + 6; j++)
            {
                if (Main.tile[x, j].HasTile || MiscTools.HasTile(x - 1, j, TileID.Rope) || MiscTools.HasTile(x + 1, j, TileID.Rope))
                {
                    return false;
                }
            }

            if (Main.tile[x - 1, y + length].HasTile || Main.tile[x + 1, y + length].HasTile)
            {
                return false;
            }

            for (int j = y; j < y + length; j++)
            {
                WorldGen.PlaceTile(x, j, TileID.Rope);
            }
            WorldGen.PlaceTile(x, y + length, TileID.Platforms, style: 23);
            WorldGen.PlaceObject(x, y + length + 1, TileID.FireflyinaBottle);

            return true;
        }
    }

    public class DesertRuins : GenPass
    {
        public DesertRuins(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Pyramid");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int countInitial = 2;
            int count = countInitial;
            while (count > 0)
            {
                StructureTools.Dungeon ruins = new StructureTools.Dungeon(0, 0, 7, 2, 36, 48, 1);
                ruins.X = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(Desert.Left * biomes.Scale, (Desert.Center) * biomes.Scale - ruins.area.Width) : WorldGen.genRand.Next(Desert.Center * biomes.Scale, (Desert.Right + 1) * biomes.Scale - ruins.area.Width);
                ruins.Y = (int)Main.worldSurface - ruins.area.Height;

                if (GenVars.structures.CanPlace(ruins.area))
                {
                    GenVars.structures.AddProtectedStructure(ruins.area);

                    StructureTools.FillEdges(ruins.area.Left, ruins.area.Top, ruins.area.Right - 1, ruins.area.Bottom - 1, ModContent.TileType<PyramidBrick>(), false);

                    #region entrance
                    int entranceX = ruins.area.Center.X - 1;
                    int entranceY = ruins.area.Top - 6;
                    int entranceLength = 0;

                    while (entranceY > Terrain.Maximum || WorldGen.SolidTile(entranceX - 19, entranceY + 18) || WorldGen.SolidTile(entranceX + 20, entranceY + 18))
                    {
                        entranceLength++;

                        entranceY -= 6;
                    }

                    StructureTools.FillEdges(entranceX - 4, entranceY + 30, entranceX + 5, ruins.area.Top, ModContent.TileType<PyramidBrick>(), false);

                    for (int k = 1; k <= 7; k++)
                    {
                        MiscTools.Rectangle(entranceX - 18 - k * 4, entranceY + k * 6, entranceX + 19 + k * 4, entranceY + k * 6 + 7, ModContent.TileType<PyramidBrick>());

                        WorldGen.PlaceTile(entranceX - 19 - k * 4, entranceY + k * 6, ModContent.TileType<PyramidPlatform>());
                        WorldGen.PlaceTile(entranceX + 20 + k * 4, entranceY + k * 6, ModContent.TileType<PyramidPlatform>());

                        if (k == 7)
                        {
                            StructureTools.FillEdges(entranceX - 18 - k * 4, entranceY + k * 6 + 7, entranceX + 19 + k * 4, entranceY + k * 6 + 7, ModContent.TileType<PyramidBrick>(), false);
                        }
                    }
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/entrance", new Point16(entranceX - 19, entranceY - 12), ModContent.GetInstance<Remnants>());

                    for (int k = 1; k <= entranceLength; k++)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/shaft", new Point16(entranceX - 4, entranceY + k * 6), ModContent.GetInstance<Remnants>());
                    }

                    int exitY = ruins.area.Bottom + 6;
                    int exitLength = 0;

                    while (!MiscTools.NonSolidInArea(entranceX - 8, exitY - 4, entranceX - 6, exitY - 2) && !MiscTools.NonSolidInArea(entranceX + 7, exitY - 4, entranceX + 9, exitY - 2) || !MiscTools.SolidInArea(entranceX - 2, exitY + 7, entranceX + 3, exitY + 7))
                    {
                        exitLength++;

                        exitY += 6;
                    }

                    StructureTools.FillEdges(entranceX - 4, ruins.area.Bottom, entranceX + 5, exitY - 24, ModContent.TileType<PyramidBrick>(), false);

                    for (int k = 1; k <= exitLength + 1; k++)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/shaft", new Point16(entranceX - 4, exitY - k * 6), ModContent.GetInstance<Remnants>());
                    }

                    MiscTools.Terraform(new Vector2(entranceX - 7, exitY - 2.5f), 2.5f);
                    MiscTools.Terraform(new Vector2(entranceX + 8, exitY - 2.5f), 2.5f);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/exit", new Point16(entranceX - 5, exitY - 12), ModContent.GetInstance<Remnants>());

                    WorldGen.PlaceTile(entranceX - 6, exitY, ModContent.TileType<PyramidPlatform>());
                    WorldGen.PlaceTile(entranceX + 7, exitY, ModContent.TileType<PyramidPlatform>());

                    GenVars.structures.AddProtectedStructure(new Rectangle(entranceX - 19, entranceY - 18, 38, ruins.area.Top - (entranceY - 12)), 5);
                    #endregion

                    #region rooms
                    for (ruins.targetCell.Y = 0; ruins.targetCell.Y < ruins.grid.Bottom; ruins.targetCell.Y++)
                    {
                        for (ruins.targetCell.X = ruins.grid.Left; ruins.targetCell.X != -1; ruins.targetCell.X = (ruins.targetCell.X == ruins.grid.Left ? ruins.grid.Center.X : ruins.targetCell.X == ruins.grid.Center.X ? ruins.grid.Right - 1 : -1))
                        {
                            if (ruins.AddRoom(1, 1))
                            {
                                if (ruins.targetCell.X == ruins.grid.Left)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/left", ruins.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                if (ruins.targetCell.X == ruins.grid.Center.X)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/middle", ruins.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                if (ruins.targetCell.X == ruins.grid.Right - 1)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/right", ruins.roomPos, ModContent.GetInstance<Remnants>());
                                }

                                if (WorldGen.genRand.NextBool(2))
                                {
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/DesertRuins/sandtrap", new Point16(ruins.roomPos.X + 5, ruins.roomPos.Y), ModContent.GetInstance<Remnants>());
                                }
                                if (WorldGen.genRand.NextBool(2))
                                {
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/DesertRuins/sandtrap", new Point16(ruins.roomPos.X + 21, ruins.roomPos.Y), ModContent.GetInstance<Remnants>());
                                }

                                if (ruins.targetCell.X == ruins.grid.Left || ruins.targetCell.X == ruins.grid.Right - 1)
                                {
                                    WorldGen.PlaceObject(ruins.room.Center.X - 1, ruins.room.Bottom - 19, ModContent.TileType<PyramidPot>());

                                    WorldGen.PlaceTile(ruins.room.Center.X - 2, ruins.room.Bottom - 19, TileID.Torches, style: 16);
                                    WorldGen.PlaceTile(ruins.room.Center.X + 1, ruins.room.Bottom - 19, TileID.Torches, style: 16);
                                }
                            }
                        }
                    }

                    List<int> rooms = new List<int>();
                    for (int k = 1; k <= 1; k++)
                    {
                        rooms.Add(k);
                    }

                    int roomCount = 0;
                    while (roomCount > 0)
                    {
                        bool right = WorldGen.genRand.NextBool(2);
                        ruins.targetCell.X = right ? 4 : 1;
                        ruins.targetCell.Y = WorldGen.genRand.Next(ruins.grid.Bottom);

                        if (ruins.AddRoom(2))
                        {
                            int room = rooms[WorldGen.genRand.Next(rooms.Count)];

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/DesertRuins/large" + room, ruins.roomPos, ModContent.GetInstance<Remnants>());

                            rooms.Remove(room);

                            roomCount--;
                        }
                    }

                    rooms = new List<int>();
                    for (int k = 1; k <= 9; k++)
                    {
                        rooms.Add(k);
                    }

                    for (ruins.targetCell.Y = 0; ruins.targetCell.Y < ruins.grid.Bottom; ruins.targetCell.Y++)
                    {
                        for (ruins.targetCell.X = 1; ruins.targetCell.X < ruins.grid.Right - 1; ruins.targetCell.X++)
                        {
                            if (ruins.AddRoom(1, 1))
                            {
                                int room = rooms[WorldGen.genRand.Next(rooms.Count)];

                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Special/DesertRuins/" + room, ruins.roomPos, ModContent.GetInstance<Remnants>());

                                rooms.Remove(room);
                            }
                        }
                    }
                    #endregion

                    //StructureTools.AddTraps(ruins.area, wireDepth: 5);
                    StructureTools.AddVariation(ruins.area, 0);

                    #region objects
                    int objects = 8;
                    while (objects > 0)
                    {
                        Rectangle desert = GenVars.UndergroundDesertLocation;
                        int x = WorldGen.genRand.Next(desert.Left + 50, desert.Right - 50);
                        int y = WorldGen.genRand.Next((int)Main.worldSurface + 100, desert.Bottom - 50);

                        bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true);

                        if (biomes.FindBiome(x, y) == BiomeID.Desert && GenVars.structures.CanPlace(new Rectangle(x - 10, y - 11, 22, 18), validTiles, 5) && MiscTools.SolidInArea(x - 10, y + 7, x + 11, y + 9) && (MiscTools.NonSolidInArea(x - 12, y - 7, x - 10, y) || MiscTools.NonSolidInArea(x + 12, y - 7, x + 30, y)))
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Special/DesertRuins/shrine", new Point16(x - 10, y - 11), ModContent.GetInstance<Remnants>());

                            WorldGen.PlaceTile(x - 11, y + 1, ModContent.TileType<PyramidPlatform>());
                            WorldGen.PlaceTile(x + 12, y + 1, ModContent.TileType<PyramidPlatform>());
                            WorldGen.PlaceTile(x - 11, y - 11, ModContent.TileType<PyramidPlatform>());
                            WorldGen.PlaceTile(x + 12, y - 11, ModContent.TileType<PyramidPlatform>());

                            GenVars.structures.AddProtectedStructure(new Rectangle(x - 10, y - 11, 22, 18), 5);

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

                                StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                                StructureTools.FillChest(chestIndex, itemsToAdd);

                                objects--;
                            }

                            WorldGen.PlaceTile(x - 1, y, TileID.Torches, style: 16);
                            WorldGen.PlaceTile(x + 2, y, TileID.Torches, style: 16);
                        }
                    }

                    //objects = WorldGen.genRand.Next(4, 7);
                    //while (objects > 0)
                    //{
                    //    Rectangle desert = GenVars.UndergroundDesertLocation;
                    //    int x = WorldGen.genRand.Next(ruins.area.Left, ruins.area.Right);
                    //    int y = (int)(Main.worldSurface * 0.5f);

                    //    while (!WorldGen.SolidTile3(x, y))
                    //    {
                    //        y++;
                    //    }

                    //    y += WorldGen.genRand.Next(4);

                    //    if (biomes.FindBiome(x, y) == BiomeID.Desert && GenVars.structures.CanPlace(new Rectangle(x, y - 6, 2, 13), 5) && MathHelper.Distance(x, entranceX) > 50 && MathHelper.Distance(x, Desert.OasisX) > 25)
                    //    {
                    //        MiscTools.Rectangle(x, y - 6, x, y + 6, wall: ModContent.WallType<PyramidBrickWallUnsafe>(), replace: false);

                    //        GenVars.structures.AddProtectedStructure(new Rectangle(x, y - 6, 2, 13), 5);

                    //        objects--;
                    //    }
                    //}

                    objects = WorldGen.genRand.Next(4, 7);
                    while (objects > 0)
                    {
                        Rectangle desert = GenVars.UndergroundDesertLocation;
                        int x = WorldGen.genRand.Next(ruins.area.Left, ruins.area.Right);
                        int y = (int)(Main.worldSurface * 0.5f);

                        while (!WorldGen.SolidTile3(x, y))
                        {
                            y++;
                        }

                        y += WorldGen.genRand.Next(4);

                        if (biomes.FindBiome(x, y) == BiomeID.Desert && GenVars.structures.CanPlace(new Rectangle(x - 1, y - 6, 4, 13), 5) && MathHelper.Distance(x, entranceX) > 50 && MathHelper.Distance(x, Desert.OasisX) > 25)
                        {
                            MiscTools.Rectangle(x - 1, y - 6, x + 2, y + 6, ModContent.TileType<PyramidBrick>());

                            WorldGen.PlaceTile(x - 2, y - 6, ModContent.TileType<PyramidPlatform>());
                            WorldGen.PlaceTile(x + 3, y - 6, ModContent.TileType<PyramidPlatform>());

                            GenVars.structures.AddProtectedStructure(new Rectangle(x, y - 6, 2, 13), 5);

                            objects--;
                        }
                    }

                    objects = ruins.grid.Height * ruins.grid.Width * 4;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(ruins.area.Left, ruins.area.Right);
                        int y = WorldGen.genRand.Next(ruins.area.Top - 12, ruins.area.Bottom);

                        bool valid = true;
                        if (Framing.GetTileSafely(x, y).TileType == TileID.Pots || MiscTools.Tile(x, y).WallType != ModContent.WallType<pyramid>() && MiscTools.Tile(x, y).WallType != ModContent.WallType<PyramidBrickWallUnsafe>())
                        {
                            valid = false;
                        }
                        else if (MiscTools.Tile(x, y + 1).TileType == ModContent.TileType<PyramidPlatform>() || MiscTools.Tile(x, y + 1).TileType == ModContent.TileType<PyramidPlatform>())
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

                    count--;
                }
            }
        }
    }

    public class Beehives : GenPass
    {
        public Beehives(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Beehives");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            Main.tileSolid[TileID.Hive] = true;

            int count = 0;
            int countTotal = (int)(Main.maxTilesX / 4200f * Main.maxTilesY / 1200f);
            while (count < countTotal)
            {
                float radius = WorldGen.genRand.NextFloat(128, 192);

                int x = WorldGen.genRand.Next(400, Main.maxTilesX - 400);
                int y = WorldGen.genRand.Next((int)(Main.rockLayer + radius / 2), GenVars.lavaLine);

                bool valid = true;

                if (!GenVars.structures.CanPlace(new Rectangle((int)(x - radius / 2), (int)(y - radius), (int)(radius), (int)(radius * 2))))
                {
                    valid = false;
                }
                else
                {
                    for (int j = (int)(y - radius * 2); j <= y + radius * 2; j++)
                    {
                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            if (MiscTools.HasTile(i, j, TileID.Hive))
                            {
                                valid = false;
                            }
                            else if (Vector2.Distance(new Vector2(i * 2, j), new Vector2(x * 2, y)) < radius && biomes.FindBiome(i, j) != BiomeID.Jungle)
                            {
                                valid = false;
                            }

                            if (!valid)
                            {
                                break;
                            }
                        }
                        if (!valid)
                        {
                            break;
                        }
                    }
                }

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(new Rectangle((int)(x - radius / 2), (int)(y - radius), (int)(radius), (int)(radius * 2)));

                    FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                    noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                    noise.SetFrequency(0.02f);
                    noise.SetFractalType(FastNoiseLite.FractalType.FBm);
                    //noise.SetFractalGain(0.4f);
                    //noise.SetFractalOctaves(1);

                    for (int j = (int)(y - radius); j <= y + radius; j++)
                    {
                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            float _noise = noise.GetNoise(i, j * 2) + 1;
                            float distance = Vector2.Distance(new Vector2(i * 2, j), new Vector2(x * 2, y)) + (_noise) * radius / 1.5f;

                            if (distance < radius)
                            {
                                if (distance < radius - 8)
                                {
                                    tile.TileType = TileID.Hive;
                                    tile.LiquidType = LiquidID.Honey;

                                    if (distance < radius - 10)
                                    {
                                        tile.WallType = WallID.HiveUnsafe;
                                    }

                                    if (distance < radius / 1.6f && _noise < 0.35f)
                                    {
                                        WorldGen.KillTile(i, j);
                                        tile.LiquidAmount = 51;

                                        if (distance < radius / 2.2f && _noise < 0.25f)
                                        {
                                            MiscTools.Tile(i, j).WallType = 0;
                                        }
                                    }
                                    else tile.HasTile = true;
                                }
                                else
                                {
                                    tile.TileType = distance < radius - 4 || WorldGen.SolidTile3(i, j) ? TileID.Mud : TileID.JungleGrass;
                                    tile.HasTile = true;
                                }
                            }
                        }
                    }

                    MiscTools.PlaceObjectsInArea((int)(x - radius / 5f), (int)(y - radius / 2.5f), (int)(x + radius / 5f), y, TileID.Larva);

                    count++;
                }
            }
        }
    }

    public class FloatingIslands : GenPass
    {
        public FloatingIslands(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.SkyIslands");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int countInitial = (int)(Main.maxTilesX / 525f);
            int count = countInitial;
            while (count > 0)
            {
                bool valid = true;

                int height = WorldGen.genRand.Next((int)(25 * (Main.maxTilesY / 1200f)), (int)(50 * (Main.maxTilesY / 1200f)) + 1);
                if (count < countInitial / 2)
                {
                    height /= 3;
                }

                int width = height * 4;

                int padding = 40;

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(600 + padding, Main.maxTilesX / 2 - 200 - width) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 200, Main.maxTilesX - 600 - width - padding);
                int y = WorldGen.genRand.Next(100, (int)(Main.worldSurface * 0.4f) - height);

                if (!StructureTools.AvoidsBiomes(new Rectangle(x, y, width, height), new[] { BiomeID.Tundra }))
                {
                    valid = false;
                }
                else if (count == countInitial && biomes.FindBiome(x + width / 2, (int)Main.worldSurface - 50, false) != BiomeID.Jungle)
                {
                    valid = false;
                }
                else if (count == countInitial - 1 && biomes.FindBiome(x + width / 2, (int)Main.worldSurface - 50, false) == BiomeID.Jungle)
                {
                    valid = false;
                }
                //else if (MathHelper.Distance(x + width / 2, Main.dungeonX) < width / 2 + 50)
                //{
                //    valid = false;
                //}
                else
                {
                    for (int j = y - padding; j <= y + height * 1.5f + padding; j++)
                    {
                        for (int i = x - padding; i <= x + width + padding; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.HasTile && Main.tileSolid[tile.TileType])
                            {
                                valid = false;
                            }
                        }
                    }
                }

                if (valid)
                {
                    GenVars.structures.AddStructure(new Rectangle(x, y, width, height), padding);

                    CreateIsland(x + width / 2, y + height / 2, width / 3, true);
                    List<int> rooms = new List<int>() { 0, 1, 2 };

                    if (count >= countInitial / 2)
                    {

                        StructureTools.Dungeon tower = new StructureTools.Dungeon(x + width / 2, y, 1, count % 3 + 2, 23, 12);

                        while (true)
                        {
                            tower.Y++;

                            if (MiscTools.Tile(tower.X - 11, tower.Y + 1).HasTile || MiscTools.Tile(tower.X + 11, tower.Y + 1).HasTile)
                            {
                                break;
                            }
                        }

                        tower.X -= 11;
                        tower.Y -= tower.area.Height;

                        int type = TileID.Dirt;
                        MiscTools.Rectangle(tower.area.Left, tower.area.Bottom, tower.area.Right - 1, tower.area.Bottom + width / 15, type, replace: false);
                        WorldGen.TileRunner(tower.area.Left, tower.area.Bottom + 2, 8, 1, type, true, overRide: false);
                        WorldGen.TileRunner(tower.area.Right, tower.area.Bottom + 2, 8, 1, type, true, overRide: false);

                        GenVars.structures.AddProtectedStructure(tower.area, 25);

                        for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Bottom; tower.targetCell.Y++)
                        {
                            if (tower.targetCell.Y == 0)
                            {
                                int index = WorldGen.genRand.Next(2);
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/SkyObservatory/top", index, tower.roomPos, ModContent.GetInstance<Remnants>());

                                #region chest
                                int chestIndex;
                                if (index == 0)
                                {
                                    chestIndex = WorldGen.PlaceChest(tower.roomPos.X + 9, tower.roomPos.Y + 11, style: 13);
                                }
                                else chestIndex = WorldGen.PlaceChest(tower.roomPos.X + 12, tower.roomPos.Y + 11, style: 13);

                                var itemsToAdd = new List<(int type, int stack)>();

                                int[] specialItems = new int[4];
                                specialItems[0] = ItemID.Starfury;
                                specialItems[1] = ItemID.ShinyRedBalloon;
                                specialItems[2] = ItemID.LuckyHorseshoe;
                                specialItems[3] = ItemID.Binoculars;

                                int specialItem = specialItems[(count - 1) % specialItems.Length];

                                itemsToAdd.Add((specialItem, 1));

                                StructureTools.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.GravitationPotion, ItemID.FeatherfallPotion });

                                StructureTools.FillChest(chestIndex, itemsToAdd);
                                #endregion
                            }
                            else
                            {
                                int room = rooms[WorldGen.genRand.Next(0, rooms.Count)];
                                rooms.Remove(room);

                                if (room == 0)
                                {
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/SkyObservatory/study", tower.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                else if (room == 1)
                                {
                                    int index = WorldGen.genRand.Next(2);
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/SkyObservatory/bedroom", index, tower.roomPos, ModContent.GetInstance<Remnants>());

                                    #region dresser
                                    int chestIndex = WorldGen.PlaceChest(tower.roomPos.X + (index == 0 ? 13 : 9), tower.roomPos.Y + 11, TileID.Dressers, style: 13);

                                    var itemsToAdd = new List<(int type, int stack)>();

                                    StructureTools.GenericLoot(chestIndex, itemsToAdd);

                                    StructureTools.FillChest(chestIndex, itemsToAdd);
                                    #endregion

                                    WorldGen.PlaceTile(tower.roomPos.X + (index == 0 ? 13 : 9), tower.roomPos.Y + 9, TileID.Candles, style: 12);
                                    MiscTools.Tile(tower.roomPos.X + (index == 0 ? 13 : 9), tower.roomPos.Y + 9).TileFrameX = 18;
                                }
                                else if (room == 2)
                                {
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/SkyObservatory/bathroom", tower.roomPos, ModContent.GetInstance<Remnants>());
                                }

                                if (tower.targetCell.Y == tower.grid.Bottom - 1)
                                {
                                    MiscTools.Rectangle(tower.area.Left - 2, tower.roomPos.Y + 9, tower.area.Left, tower.roomPos.Y + 11, -1);
                                    MiscTools.Rectangle(tower.area.Right - 1, tower.roomPos.Y + 9, tower.area.Right + 1, tower.roomPos.Y + 11, -1);

                                    WorldGen.PlaceObject(tower.area.Left, tower.roomPos.Y + 10, TileID.ClosedDoor, style: 9);
                                    WorldGen.PlaceObject(tower.area.Right - 1, tower.roomPos.Y + 10, TileID.ClosedDoor, style: 9);
                                }

                                int objects = 3;
                                while (objects > 0)
                                {
                                    int pos = WorldGen.genRand.Next(tower.area.Left + 1, tower.area.Right - 1);
                                    if (MiscTools.Tile(pos, tower.roomPos.Y + 5).TileType != TileID.Banners)
                                    {
                                        WorldGen.PlaceObject(pos, tower.roomPos.Y + 4, TileID.Banners, style: 6 + objects);
                                        if (MiscTools.Tile(pos, tower.roomPos.Y + 4).TileType == TileID.Banners)
                                        {
                                            objects--;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int islands = 0;
                    while (islands < ModContent.GetInstance<Worldgen>().CloudDensity * 4)
                    {
                        int size = WorldGen.genRand.Next(width / 12, width / 6);

                        if (CreateIsland(WorldGen.genRand.Next(x + size / 2, x + width - size / 2), WorldGen.genRand.Next(y, y + height), size))
                        {
                            islands++;
                        }
                    }

                    count--;
                }
            }
        }

        private bool CreateIsland(int x, int y, int size, bool forced = false)
        {
            if (!forced)
            {
                for (int j = y - size / 2; j <= y + size; j++)
                {
                    for (int i = x - size / 2; i <= x + size / 2; i++)
                    {
                        Tile tile = Main.tile[i, j];
                        if (tile.HasTile && Main.tileSolid[tile.TileType])
                        {
                            return false;
                        }
                    }
                }
            }

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetFrequency(1f / size);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int thickness = size / 2;

            for (int j = y - thickness; j <= y + size / 2 + thickness; j++)
            {
                Vector2 point = new Vector2(x, Math.Clamp(j, y, y + size / 2));

                thickness = (int)((0.25f + (point.Y - y) / (size / 2)) / 1.25f * size / 2);

                for (int i = x - size / 2 - thickness; i <= x + size / 2 + thickness; i++)
                {
                    point.X = Math.Clamp(i, x - size / 2 + (point.Y - y), x + size / 2 - (point.Y - y));
                    Tile tile = Main.tile[i, j];

                    if (!tile.HasTile)
                    {
                        tile.TileType = TileID.Dirt;
                    }

                    if (noise.GetNoise(i * 2, j) / 2 + 0.5f > Vector2.Distance(new Vector2(i, j), point) / thickness)
                    {
                        tile.HasTile = true;

                        if (biomes.MaterialBlend(i, j, frequency: 2) <= 0.2f)
                        {
                            tile.TileType = TileID.Stone;
                        }
                    }

                    if (MiscTools.SurroundingTilesActive(i - 1, j - 1))
                    {
                        MiscTools.Tile(i - 1, j - 1).WallType = WallID.DirtUnsafe;
                    }
                }
            }

            for (int j = y - thickness; j <= y + size / 2 + thickness; j++)
            {
                Vector2 point = new Vector2(x, Math.Clamp(j, y, y + size / 2));

                thickness = (int)((0.25f + (point.Y - y) / (size / 2)) / 1.25f * size / 2);

                for (int i = x - size / 2 - thickness; i <= x + size / 2 + thickness; i++)
                {
                    Tile tile = Main.tile[i, j];

                    if (tile.HasTile)
                    {
                        if (tile.TileType == TileID.Stone || tile.TileType == TileID.ClayBlock)
                        {
                            for (int k = 1; k <= WorldGen.genRand.Next(5, 7); k++)
                            {
                                if (!MiscTools.Tile(i, j - k).HasTile)
                                {
                                    tile.TileType = TileID.Dirt;
                                    break;
                                }
                            }

                            if (tile.TileType != TileID.Dirt)
                            {
                                for (int k = 1; k <= WorldGen.genRand.Next(2, 4); k++)
                                {
                                    if (!MiscTools.Tile(i, j + k).HasTile)
                                    {
                                        tile.TileType = TileID.Dirt;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            GenVars.structures.AddProtectedStructure(new Rectangle(x - size / 2, y, size, size));

            return true;
        }
    }

    public class GemCaves : GenPass
    {
        public GemCaves(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.GemCaves");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int count = 0;
            int countTotal = (int)((Main.maxTilesX - 800) * (Main.maxTilesY - 200 - Main.rockLayer) / 100000);
            while (count < countTotal)
            {
                progress.Set((float)count / (float)countTotal);

                float radius = WorldGen.genRand.NextFloat(24, 36);

                float waterLavaRatio = (float)((GenVars.lavaLine - Main.rockLayer) / (Main.maxTilesY - 200 - Main.rockLayer));

                int x = WorldGen.genRand.Next(400, Main.maxTilesX - 400);
                int y = ((float)count / (float)countTotal > waterLavaRatio) ? WorldGen.genRand.Next((int)GenVars.lavaLine, Main.maxTilesY - 200) : WorldGen.genRand.Next((int)Main.rockLayer, GenVars.lavaLine);

                bool valid = true;

                if (!GenVars.structures.CanPlace(new Rectangle((int)(x - radius), (int)(y - radius / 2), (int)(radius * 2), (int)(radius))))
                {
                    valid = false;
                }
                else
                {
                    for (int j = (int)(y - radius); j <= y + radius; j++)
                    {
                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j * 2), new Vector2(x, y * 2)) < radius && (!WorldGen.SolidTile3(i, j) || biomes.FindBiome(i, j) != BiomeID.None))
                            {
                                valid = false;
                            }
                        }
                    }
                }

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(new Rectangle((int)(x - radius), (int)(y - radius / 2), (int)(radius * 2), (int)(radius)));

                    FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                    noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                    noise.SetFrequency(0.05f);
                    noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                    noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                    noise.SetFractalType(FastNoiseLite.FractalType.None);
                    //noise.SetFractalOctaves(1);

                    for (int j = (int)(y - radius); j <= y + radius; j++)
                    {
                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            float distance = Vector2.Distance(new Vector2(i, j * 2), new Vector2(x, y * 2)) + (noise.GetNoise(i, j * 2) / 2 + 0.5f) * (radius);

                            if (distance < radius)
                            {
                                int gemType = RemTile.GetGemType(j);

                                tile.TileType = TileID.Stone;
                                tile.WallType = gemType == 5 ? WallID.DiamondUnsafe : gemType == 4 ? WallID.RubyUnsafe : gemType == 3 ? WallID.EmeraldUnsafe : gemType == 2 ? WallID.SapphireUnsafe : gemType == 1 ? WallID.TopazUnsafe : WallID.AmethystUnsafe;

                                if (distance < radius / 1.5f)
                                {
                                    WorldGen.KillTile(i, j);
                                    tile.LiquidAmount = 0;

                                    if (distance < radius / 2.5f)
                                    {
                                        MiscTools.Tile(i, j).WallType = 0;
                                    }
                                }
                                else tile.HasTile = true;
                            }
                        }
                    }

                    MiscTools.PlaceObjectsInArea((int)(x - radius / 1.5f), y, (int)(x + radius / 1.5f), (int)(y + radius / 2), ModContent.TileType<Runestalk>(), count: (radius >= 28 ? 3 : radius >= 26 ? 2 : 1));

                    for (int j = (int)(y - radius); j <= y + radius; j++)
                    {
                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            if (Vector2.Distance(new Vector2(i, j * 2), new Vector2(x, y * 2)) < radius && WorldGen.genRand.NextBool(2))
                            {
                                int gemType = RemTile.GetGemType(j);

                                if (!tile.HasTile)
                                {
                                    if (MiscTools.AdjacentTiles(i, j, true) > 0)
                                    {
                                        WorldGen.PlaceTile(i, j, TileID.ExposedGems, style: gemType);
                                    }
                                }
                                else if (tile.TileType == TileID.Stone && !MiscTools.SurroundingTilesActive(i, j, true))
                                {
                                    tile.TileType = gemType == 5 ? TileID.Diamond : gemType == 4 ? TileID.Ruby : gemType == 3 ? TileID.Emerald : gemType == 2 ? TileID.Sapphire : gemType == 1 ? TileID.Topaz : TileID.Amethyst;
                                }
                            }
                        }
                    }

                    count++;
                }
            }
        }
    }

    public class Microdungeons : GenPass
    {
        public Microdungeons(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Microdungeons");
            int structureCount;

            int uniqueStructures = 7;
            int progressCounter = 0;

            structureCount = 0; // JUNGLE PLATFORM
            int attempts = 0;
            while (attempts < 10000)
            {
                int x = WorldGen.genRand.Next((Jungle.Left + 1) * biomes.Scale, Jungle.Right * biomes.Scale);
                int y = WorldGen.genRand.Next(attempts < 500 ? (Terrain.Middle + Terrain.Maximum * 2) / 3 : (Terrain.Middle + Terrain.Maximum) / 2, (Terrain.Middle + Terrain.Maximum * 5) / 6);

                int left = x - WorldGen.genRand.Next(3, 10);
                int right = x + WorldGen.genRand.Next(3, 10);

                int stacks = WorldGen.genRand.Next(1, 3);

                bool goLeft = WorldGen.genRand.NextBool(2);

                if (GenVars.structures.CanPlace(new Rectangle(left, Terrain.Minimum, right - left, Terrain.Maximum - Terrain.Minimum), 15) && CreatePlatform(left, right, y))
                {
                    while (stacks > 0)
                    {
                        if (goLeft)
                        {
                            right = left;
                            left -= WorldGen.genRand.Next(6, 19);
                        }
                        else
                        {
                            left = right;
                            right += WorldGen.genRand.Next(6, 19);
                        }
                        y -= WorldGen.genRand.Next(2, 5) * 3;

                        if (CreatePlatform(left, right, y))
                        {
                            stacks--;
                        }
                        else break;
                    }
                }
                else attempts++;
            }

            structureCount = 0; // MARBLE BATHHOUSE
            while (structureCount < Main.maxTilesX / 1050)
            {
                progress.Set((progressCounter + structureCount / (float)(Main.maxTilesX / 1050)) / uniqueStructures);

                #region spawnconditions
                StructureTools.Dungeon temple = new StructureTools.Dungeon(0, 0, 3, structureCount == 0 ? 4 : structureCount == 1 ? 2 : Math.Max(WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 4)), 18, 12, 3);
                temple.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.425f), (int)(Main.maxTilesX * 0.575f) - temple.area.Width);
                temple.Y = (MarbleCave.Y + 1) * biomes.Scale - temple.area.Height;

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(false, TileID.Marble, TileID.Obsidian);

                bool valid = true;
                if (!GenVars.structures.CanPlace(temple.area, validTiles, 10))
                {
                    valid = false;
                }
                //else if (!Structures.InsideBiome(temple.area, BiomeID.Marble))
                //{
                //    valid = false;
                //}
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(temple.area, 10);

                    for (int j = 0; j < 20; j += 3)
                    {
                        WorldGen.TileRunner(temple.area.Left - 7, temple.area.Bottom + 2 + j, WorldGen.genRand.Next(2, 7) * 2 + j / 2, 1, TileID.Marble, true, 0, 0, false, false);
                        WorldGen.TileRunner(temple.area.Right + 7, temple.area.Bottom + 2 + j, WorldGen.genRand.Next(2, 7) * 2 + j / 2, 1, TileID.Marble, true, 0, 0, false, false);
                    }

                    #region structure
                    bool[] marble = TileID.Sets.Factory.CreateBoolSet(false, TileID.Marble);
                    MiscTools.Terraform(new Vector2(temple.area.Left - 5, temple.area.Bottom - 3), 5, marble);
                    MiscTools.Terraform(new Vector2(temple.area.Right + 5, temple.area.Bottom - 3), 5, marble);

                    #region rooms
                    int roomCount;

                    for (int i = 0; i < temple.grid.Width; i++)
                    {
                        for (int j = 1; j < temple.grid.Height; j++)
                        {
                            temple.AddMarker(i, j);
                        }
                    }
                    int width = structureCount == 2 ? 2 : WorldGen.genRand.Next(1, temple.grid.Width + 1);
                    int pos = WorldGen.genRand.Next(0, temple.grid.Width - width);
                    for (int i = pos; i < pos + width; i++)
                    {
                        temple.AddMarker(i, 0);
                    }

                    //temple.AddMarker(0, 0, 1);
                    //roomCount = (temple.grid.Height - 1) * temple.grid.Width / 2;
                    //while (roomCount > 0)
                    //{
                    //    temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width);
                    //    temple.targetCell.Y = WorldGen.genRand.Next(0, temple.grid.Height);
                    //    if (roomCount < temple.grid.Height)
                    //    {
                    //        temple.targetCell.Y = roomCount;
                    //    }

                    //    if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = temple.grid.Height * temple.grid.Width / 8;
                    //while (roomCount > 0)
                    //{
                    //    temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width - 1);
                    //    temple.targetCell.Y = WorldGen.genRand.Next(0, temple.grid.Height);

                    //    if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y + 1))
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y + 1);
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                    //        if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                    //        {
                    //            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1);
                    //        }

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/thermalrig/solid", temple.roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < temple.grid.Height - 1)
                    {
                        temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width);
                        temple.targetCell.Y = roomCount + 1;

                        if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1))
                        {
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1);

                            roomCount++;
                        }
                    }

                    #endregion

                    for (temple.targetCell.Y = temple.grid.Height - 1; temple.targetCell.Y >= 0; temple.targetCell.Y--)
                    {
                        while (true)
                        {
                            temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width);

                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/MarbleBathhouse/room", 2, temple.roomPos, ModContent.GetInstance<Remnants>());
                                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                                int chestIndex = WorldGen.PlaceChest(temple.roomPos.X + 9, temple.roomPos.Y + temple.room.Height - 1, TileID.Dressers, style: 27);

                                var itemsToAdd = new List<(int type, int stack)>();

                                StructureTools.GenericLoot(chestIndex, itemsToAdd);

                                StructureTools.FillChest(chestIndex, itemsToAdd);

                                break;
                            }
                        }

                        for (temple.targetCell.X = 0; temple.targetCell.X < temple.grid.Width; temple.targetCell.X++)
                        {
                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 2))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/MarbleBathhouse/room", WorldGen.genRand.Next(2), temple.roomPos, ModContent.GetInstance<Remnants>());
                            }

                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && (temple.targetCell.X == 0 || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y)))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/MarbleBathhouse/left", new Point16(temple.roomPos.X - 5, temple.roomPos.Y), ModContent.GetInstance<Remnants>());
                                WorldGen.PlaceTile(temple.roomPos.X - 6, temple.roomPos.Y, TileID.Platforms, style: 29);
                            }
                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && (temple.targetCell.X == temple.grid.Width - 1 || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y)))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/MarbleBathhouse/right", new Point16(temple.roomPos.X + temple.room.Width, temple.roomPos.Y), ModContent.GetInstance<Remnants>());
                                WorldGen.PlaceTile(temple.roomPos.X + temple.room.Width + 6, temple.roomPos.Y, TileID.Platforms, style: 29);
                            }

                            if (temple.targetCell.Y == temple.grid.Height - 1)
                            {
                                if (temple.targetCell.X == 0)
                                {
                                    WorldGen.PlaceTile(temple.roomPos.X - 6, temple.roomPos.Y + temple.room.Height, TileID.Platforms, style: 29);
                                }
                                if (temple.targetCell.X == temple.grid.Width - 1)
                                {
                                    WorldGen.PlaceTile(temple.roomPos.X + temple.room.Width + 6, temple.roomPos.Y + temple.room.Height, TileID.Platforms, style: 29);
                                }
                            }
                        }
                    }

                    for (temple.targetCell.Y = 0; temple.targetCell.Y < temple.grid.Height; temple.targetCell.Y++)
                    {
                        for (temple.targetCell.X = 0; temple.targetCell.X < temple.grid.Width; temple.targetCell.X++)
                        {
                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y))
                            {
                                if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/MarbleBathhouse/ladder", temple.roomPos, ModContent.GetInstance<Remnants>());

                                    //if (temple.targetCell.Y < temple.grid.Height - 1 && temple.targetCell.X == 0)
                                    //{
                                    //    WGTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + 7, temple.roomPos.X, temple.roomPos.Y + 11, TileID.MarbleBlock);
                                    //    WGTools.Rectangle(temple.roomPos.X - 1, temple.roomPos.Y + 6, temple.roomPos.X - 1, temple.roomPos.Y + 12, TileID.Marble);
                                    //}
                                    //else if (temple.targetCell.Y < temple.grid.Height - 1 && temple.targetCell.X == temple.grid.Width - 1)
                                    //{
                                    //    WGTools.Rectangle(temple.roomPos.X + temple.room.Width, temple.roomPos.Y + 7, temple.roomPos.X + temple.room.Width + 2, temple.roomPos.Y + 11, TileID.MarbleBlock);
                                    //    WGTools.Rectangle(temple.roomPos.X + temple.room.Width + 1, temple.roomPos.Y + 6, temple.roomPos.X + temple.room.Width + 1, temple.roomPos.Y + 12, TileID.Marble);
                                    //}
                                }

                                if (temple.targetCell.X > 0 && temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 1) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                                {
                                    MiscTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + 4, temple.roomPos.X + 2, temple.roomPos.Y + temple.room.Height - 1, -1, WallID.MarbleBlock);

                                    MiscTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + 3, temple.roomPos.X + 2, temple.roomPos.Y + 3, TileID.MarbleBlock);
                                    MiscTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + temple.room.Height, temple.roomPos.X + 2, temple.roomPos.Y + temple.room.Height, TileID.MarbleBlock);

                                    WorldGen.PlaceObject(temple.roomPos.X, temple.roomPos.Y + 11, TileID.Pianos, style: 29);
                                    //WorldGen.PlaceObject(temple.roomPos.X, temple.roomPos.Y + 8, TileID.Painting3X3, style: 45);

                                    PlacePainting(temple.roomPos.X, temple.roomPos.Y + 7);
                                }
                            }
                        }
                    }

                    #region cleanup

                    for (int y = temple.area.Top - 4; y <= temple.area.Bottom + 50; y++)
                    {
                        for (int x = temple.area.Left - 6; x <= temple.area.Right + 6; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (tile.WallType == ModContent.WallType<WoodLattice>())
                            {
                                tile.WallType = 0;
                                Main.tile[x, y - 1].WallType = 0;
                            }

                            if (y > temple.area.Bottom && biomes.FindBiome(x, y) == BiomeID.Marble)
                            {
                                if (!tile.HasTile)
                                {
                                    WorldGen.PlaceTile(x, y, TileID.Marble);
                                    tile.LiquidAmount = 0;
                                }
                            }
                        }
                    }

                    StructureTools.AddVariation(temple.area);
                    StructureTools.AddDecorations(temple.area);
                    #endregion

                    #region objects
                    int objects;

                    objects = 1;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(temple.area.Left, temple.area.Right);
                        int y = WorldGen.genRand.Next(temple.area.Top, temple.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && MiscTools.Tile(x, y + 1).TileType != TileID.Platforms && MiscTools.Tile(x + 1, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y, 2))
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, style: 51, notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                            {
                                #region chestloot
                                var itemsToAdd = new List<(int type, int stack)>();

                                itemsToAdd.Add((structureCount % 2 == 0 ? ItemID.HermesBoots : ItemID.AncientChisel, 1));

                                StructureTools.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.TitanPotion });

                                StructureTools.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }

                    //objects = temple.grid.Height * temple.grid.Width;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(temple.area.Left, temple.area.Right);
                    //    int y = WorldGen.genRand.Next(temple.area.Top, temple.area.Bottom + 2);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                    //    {
                    //        WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    //objects = temple.grid.Height * temple.grid.Width;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(temple.area.Left, temple.area.Right);
                    //    int y = WorldGen.genRand.Next(temple.area.Top, temple.area.Bottom + 2);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                    //    {
                    //        WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // MINECART RAIL
            while (structureCount < Main.maxTilesY / 150 * ModContent.GetInstance<Worldgen>().RailroadFrequency)
            {
                progress.Set((progressCounter + structureCount / (float)(Main.maxTilesY / 150)) / uniqueStructures);

                #region spawnconditions
                StructureTools.Dungeon rail = new StructureTools.Dungeon(0, WorldGen.genRand.Next((int)Main.rockLayer, GenVars.lavaLine - 50), WorldGen.genRand.Next(15, 30) * (Main.maxTilesX / 4200), 2, 12, 6, 2);
                rail.X = WorldGen.genRand.Next(400, Main.maxTilesX - 400 - rail.area.Width);// (structureCount < Main.maxTilesY / 240 ^ Tundra.Center > biomes.width / 2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - rail.area.Width / 2) : WorldGen.genRand.Next(Main.maxTilesX / 2 - rail.area.Width / 2, Main.maxTilesX - 400 - rail.area.Width);
                rail.X = rail.X / 4 * 4;

                bool[] invalidTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.Ash, TileID.Ebonstone, TileID.Crimstone, TileID.LihzahrdBrick, TileID.LivingWood);

                bool valid = true;
                if (!GenVars.structures.CanPlace(rail.area, invalidTiles, 25))
                {
                    valid = false;
                }
                //else if (Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Granite }) && structureCount < Main.maxTilesY / 600f || !Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Granite }) && structureCount >= Main.maxTilesY / 600f)
                //{
                //    valid = false;
                //}
                else if (!StructureTools.AvoidsBiomes(rail.area, new int[] { BiomeID.Tundra, BiomeID.Desert, BiomeID.Marble, BiomeID.Toxic, BiomeID.SunkenSea }))
                {
                    valid = false;
                }
                else if (!StructureTools.AvoidsBiomes(rail.area, new int[] { BiomeID.Glowshroom }) && !WorldGen.genRand.NextBool(10))
                {
                    valid = false;
                }
                else if (!StructureTools.AvoidsBiomes(rail.area, new int[] { BiomeID.Jungle }) ^ structureCount % 5 == 0)
                {
                    valid = false;
                }
                else
                {
                    for (int i = rail.area.Left; i <= rail.area.Right; i++)
                    {
                        for (int j = rail.area.Y - 100; j <= rail.area.Y + 100; j++)
                        {
                            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.MinecartTrack)
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (!valid)
                        {
                            break;
                        }
                    }
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(rail.area, 10);

                    #region structure
                    #region rooms

                    for (int i = rail.area.Left; i <= rail.area.Right; i++)
                    {
                        for (int j = rail.area.Y - 5; j <= rail.area.Y - 1; j++)
                        {
                            if (!TileID.Sets.IsBeam[Main.tile[i, j].TileType])
                            {
                                WorldGen.KillTile(i, j);
                            }
                        }

                        MiscTools.Terraform(new Vector2(i, rail.area.Y - 3), 5);
                        WorldGen.PlaceTile(i, rail.area.Y - 1, TileID.MinecartTrack);
                        WorldGen.TileFrame(i, rail.area.Y - 1);
                    }

                    bool hasStation = StructureTools.AvoidsBiomes(rail.area, new int[] { BiomeID.Desert, BiomeID.Granite });
                    int stationWidth = 2;
                    int stationX = WorldGen.genRand.Next(1, rail.grid.Width - stationWidth);
                    if (hasStation)
                    {
                        int ladderX = WorldGen.genRand.Next(stationX, stationX + stationWidth);

                        rail.targetCell.X = stationX;
                        rail.targetCell.Y = 0;

                        MiscTools.Terraform(new Vector2(rail.room.Left, rail.room.Bottom - 3), 5);
                        MiscTools.Terraform(new Vector2(rail.room.Right + rail.room.Width, rail.room.Bottom - 3), 5);

                        for (rail.targetCell.X = stationX; rail.targetCell.X < stationX + stationWidth; rail.targetCell.X++)
                        {
                            rail.AddMarker(rail.targetCell.X, 0);

                            if (rail.targetCell.X == ladderX)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Railroad/ladder", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                            }
                            else StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Railroad/room", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                        }

                        rail.targetCell.X = stationX;
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Railroad/wall", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                        rail.targetCell.X = stationX + stationWidth;
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Railroad/wall", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                    }

                    int roomCount = rail.grid.Width / 10;
                    while (roomCount > 0)
                    {
                        rail.targetCell.X = WorldGen.genRand.Next(1, rail.grid.Width - 1);

                        if (!rail.FindMarker(rail.targetCell.X - 1, 0) && !rail.FindMarker(rail.targetCell.X, 0) && !rail.FindMarker(rail.targetCell.X + 1, 0) && !rail.FindMarker(rail.targetCell.X, 0, 1))
                        {
                            rail.AddMarker(rail.targetCell.X, 0, 1);

                            roomCount--;
                        }
                    }

                    for (rail.targetCell.Y = 0; rail.targetCell.Y < rail.grid.Height; rail.targetCell.Y++)
                    {
                        for (rail.targetCell.X = 0; rail.targetCell.X < rail.grid.Width; rail.targetCell.X++)
                        {
                            if (rail.FindMarker(rail.targetCell.X, rail.targetCell.Y) || rail.FindMarker(rail.targetCell.X, rail.targetCell.Y, 1))
                            {

                            }
                            else if (rail.targetCell.Y == 0 || rail.FindMarker(rail.targetCell.X, 0))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Railroad/bottom", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());

                                for (int i = rail.roomPos.X; i <= rail.room.Right; i += 4)
                                {
                                    MiscTools.WoodenBeam(i, rail.roomPos.Y + 1);
                                }
                            }
                        }
                    }

                    bool[] tiles = TileID.Sets.Factory.CreateBoolSet(false, TileID.WoodBlock, TileID.WoodenBeam, TileID.BorealBeam, TileID.RichMahoganyBeam, TileID.MushroomBeam, TileID.GraniteColumn);

                    for (rail.targetCell.X = 0; rail.targetCell.X < rail.grid.Width; rail.targetCell.X++)
                    {
                        if (rail.FindMarker(rail.targetCell.X, 0, 1))
                        {
                            for (int i = rail.roomPos.X + 1; i < rail.roomPos.X + rail.room.Width; i++)
                            {
                                WorldGen.KillTile(i, rail.area.Y - 1);
                            }

                            MiscTools.Terraform(new Vector2(rail.roomPos.X + 6, rail.area.Y + 1), 6, tiles);
                            MiscTools.Terraform(new Vector2(rail.roomPos.X + 6, rail.area.Y - 9), 8);
                        }
                    }

                    #endregion

                    #region cleanup
                    if (hasStation)
                    {
                        rail.targetCell.X = stationX;
                        rail.targetCell.Y = 0;

                        MiscTools.Rectangle(rail.roomPos.X - 2, rail.room.Bottom, rail.roomPos.X - 1, rail.room.Bottom, TileID.Platforms, replace: false);
                        MiscTools.Rectangle(rail.room.Right + rail.room.Width + 1, rail.room.Bottom, rail.room.Right + rail.room.Width + 2, rail.room.Bottom, TileID.Platforms, replace: false);

                        MiscTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, biomes.FindBiome(rail.room.Right, rail.room.Bottom) == BiomeID.Glowshroom ? ModContent.TileType<Shroomcart>() : ModContent.TileType<Tiles.Objects.Minecart>());
                        //WGTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, TileID.GrandfatherClocks); 
                        MiscTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, TileID.WorkBenches);
                        MiscTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, TileID.Chairs);
                    }

                    StructureTools.AddDecorations(rail.area);
                    StructureTools.AddTheming(rail.area);
                    StructureTools.AddVariation(rail.area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // GRANITE TOWER
            int missingPiece = WorldGen.genRand.NextBool(3) ? ItemID.AncientCobaltLeggings : WorldGen.genRand.NextBool(2) ? ItemID.AncientCobaltBreastplate : ItemID.AncientCobaltHelmet;
            while (structureCount < Main.maxTilesY / 300)
            {
                progress.Set((progressCounter + structureCount / (float)(Main.maxTilesY / 300)) / uniqueStructures);

                #region spawnconditions
                StructureTools.Dungeon tower = new StructureTools.Dungeon(0, 0, 2, structureCount == 0 ? 5 : structureCount == 1 ? 3 : Math.Max(WorldGen.genRand.Next(4, 7), WorldGen.genRand.Next(4, 6)), 17, 18, 3);
                bool left = WorldGen.genRand.NextBool(2);
                tower.X = Tundra.Center * biomes.Scale + biomes.Scale / 2 + (left ? -tower.area.Width - 35 : 35);
                tower.Y = WorldGen.genRand.Next((int)Main.worldSurface + 50, Main.maxTilesY - 500 - tower.area.Height);

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true);

                bool valid = true;
                if (!GenVars.structures.CanPlace(tower.area, validTiles, 10))
                {
                    valid = false;
                }
                else if (tower.Y + tower.area.Height < Main.rockLayer && WorldGen.genRand.NextBool(2))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(tower.area, 10);

                    MiscTools.Rectangle(tower.area.Left - tower.room.Width * (left ? 1 : -1), tower.area.Top, tower.area.Right - tower.room.Width * (left ? 1 : -1), tower.area.Bottom, TileID.Granite);
                    StructureTools.FillEdges(tower.area.Left - tower.room.Width * (left ? 1 : -1), tower.area.Top, tower.area.Right - tower.room.Width * (left ? 1 : -1), tower.area.Bottom, TileID.Granite, false);

                    #region structure

                    #region rooms
                    for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Height; tower.targetCell.Y++)
                    {
                        tower.AddMarker(left ? 1 : 0, tower.targetCell.Y);
                        MiscTools.Terraform(new Vector2(left ? tower.area.Right + 6 : tower.area.Left - 7, tower.roomPos.Y + 11), 6.5f, scaleX: 1);
                    }
                    int height = WorldGen.genRand.Next(3, tower.grid.Height + 1);
                    int pos = WorldGen.genRand.Next(0, tower.grid.Height - height);
                    for (int j = pos; j < pos + height; j++)
                    {
                        tower.AddMarker(left ? 0 : 1, j);
                    }

                    int roomCount;

                    roomCount = 0;
                    while (roomCount < tower.grid.Height - 1)
                    {
                        tower.targetCell.X = WorldGen.genRand.Next(0, tower.grid.Width);
                        tower.targetCell.Y = roomCount + 1;

                        if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && tower.FindMarker(tower.targetCell.X, tower.targetCell.Y - 1))
                        {
                            tower.AddMarker(tower.targetCell.X, tower.targetCell.Y, 1);

                            roomCount++;
                        }
                    }

                    #endregion

                    for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Height; tower.targetCell.Y++)
                    {
                        for (tower.targetCell.X = 0; tower.targetCell.X < tower.grid.Width; tower.targetCell.X++)
                        {
                            if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && !tower.FindMarker(tower.targetCell.X, tower.targetCell.Y, 1))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/GraniteTower/room", !tower.FindMarker(tower.targetCell.X, tower.targetCell.Y - 1, 1) && tower.targetCell.Y > 0 && tower.FindMarker(tower.targetCell.X, tower.targetCell.Y - 1) ? 1 : 0, tower.roomPos, ModContent.GetInstance<Remnants>());

                                if (WorldGen.genRand.NextBool(2))
                                {
                                    WorldGen.PlaceObject(tower.roomPos.X + 3, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: 1);
                                    WorldGen.PlaceObject(tower.roomPos.X + 5, tower.roomPos.Y + tower.room.Height - 1, TileID.Tables, style: 33);
                                    WorldGen.PlaceObject(tower.roomPos.X + 5, tower.roomPos.Y + tower.room.Height - 3, TileID.Candles, style: 28);
                                    WorldGen.PlaceObject(tower.roomPos.X + 7, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: -1);

                                    WorldGen.PlaceObject(tower.roomPos.X + 9, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: 1);
                                    WorldGen.PlaceObject(tower.roomPos.X + 11, tower.roomPos.Y + tower.room.Height - 1, TileID.Tables, style: 33);
                                    WorldGen.PlaceObject(tower.roomPos.X + 11, tower.roomPos.Y + tower.room.Height - 3, TileID.Candles, style: 28);
                                    WorldGen.PlaceObject(tower.roomPos.X + 13, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: -1);
                                }
                                else
                                {
                                    WorldGen.PlaceObject(tower.roomPos.X + 4, tower.roomPos.Y + tower.room.Height - 1, TileID.Beds, style: 29, direction: 1);
                                    WorldGen.PlaceObject(tower.roomPos.X + 11, tower.roomPos.Y + tower.room.Height - 1, TileID.Beds, style: 29, direction: -1);

                                    int chestIndex = WorldGen.PlaceChest(tower.roomPos.X + 8, tower.roomPos.Y + tower.room.Height - 1, TileID.Dressers, style: 26);
                                    var itemsToAdd = new List<(int type, int stack)>();
                                    StructureTools.GenericLoot(chestIndex, itemsToAdd);
                                    StructureTools.FillChest(chestIndex, itemsToAdd);
                                }
                            }
                        }
                    }

                    for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Height; tower.targetCell.Y++)
                    {
                        for (tower.targetCell.X = 0; tower.targetCell.X < tower.grid.Width; tower.targetCell.X++)
                        {
                            if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y))
                            {
                                if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y, 1))
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/GraniteTower/ladder", tower.roomPos, ModContent.GetInstance<Remnants>());
                                }

                                if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && (tower.targetCell.X == 0 || !tower.FindMarker(tower.targetCell.X - 1, tower.targetCell.Y)))
                                {
                                    if (!left)
                                    {
                                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/GraniteTower/left", new Point16(tower.roomPos.X - 6, tower.roomPos.Y), ModContent.GetInstance<Remnants>());
                                        WorldGen.PlaceTile(tower.roomPos.X - 7, tower.roomPos.Y, TileID.Platforms, style: 28);
                                    }

                                    if (left || !tower.FindMarker(tower.targetCell.X - 1, tower.targetCell.Y))
                                    {
                                        MiscTools.Rectangle(tower.roomPos.X - 2, tower.roomPos.Y + 12, tower.roomPos.X - 1, tower.roomPos.Y + tower.room.Height, TileID.GraniteBlock, WallID.GraniteBlock);
                                        MiscTools.Rectangle(tower.roomPos.X - 1, tower.roomPos.Y + 13, tower.roomPos.X - 1, tower.roomPos.Y + tower.room.Height - 1, -1);
                                    }
                                }
                                if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && (tower.targetCell.X == tower.grid.Width - 1 || !tower.FindMarker(tower.targetCell.X + 1, tower.targetCell.Y)))
                                {
                                    if (left)
                                    {
                                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/GraniteTower/right", new Point16(tower.roomPos.X + tower.room.Width, tower.roomPos.Y), ModContent.GetInstance<Remnants>());
                                        WorldGen.PlaceTile(tower.roomPos.X + tower.room.Width + 6, tower.roomPos.Y, TileID.Platforms, style: 28);
                                    }

                                    if (!left || !tower.FindMarker(tower.targetCell.X + 1, tower.targetCell.Y))
                                    {
                                        MiscTools.Rectangle(tower.roomPos.X + tower.room.Width, tower.roomPos.Y + 12, tower.roomPos.X + tower.room.Width + 1, tower.roomPos.Y + tower.room.Height, TileID.GraniteBlock, WallID.GraniteBlock);
                                        MiscTools.Rectangle(tower.roomPos.X + tower.room.Width, tower.roomPos.Y + 13, tower.roomPos.X + tower.room.Width, tower.roomPos.Y + tower.room.Height - 1, -1);
                                    }
                                }
                                if (tower.targetCell.Y == tower.grid.Height - 1)
                                {
                                    MiscTools.Rectangle(tower.roomPos.X - (tower.targetCell.X == 0 ? 6 : 0), tower.roomPos.Y + tower.room.Height + 1, tower.roomPos.X + tower.room.Width + (tower.targetCell.X == tower.grid.Width - 1 ? 5 : -1), tower.roomPos.Y + tower.room.Height + 4, TileID.Granite);

                                    if (tower.targetCell.X == 0)
                                    {
                                        WorldGen.PlaceTile(tower.roomPos.X - 7, tower.roomPos.Y + tower.room.Height, TileID.Platforms, style: 28);
                                    }
                                    if (tower.targetCell.X == tower.grid.Width - 1)
                                    {
                                        WorldGen.PlaceTile(tower.roomPos.X + tower.room.Width + 6, tower.roomPos.Y + tower.room.Height, TileID.Platforms, style: 28);
                                    }
                                }
                            }
                        }
                    }

                    #region cleanup

                    //for (int y = temple.area.Top - 4; y <= temple.area.Bottom + 50; y++)
                    //{
                    //    for (int x = temple.area.Left - 6; x <= temple.area.Right + 6; x++)
                    //    {
                    //        Tile tile = Main.tile[x, y];
                    //    }
                    //}

                    StructureTools.AddVariation(tower.area);
                    StructureTools.AddDecorations(tower.area);
                    #endregion

                    #region objects
                    int objects;

                    objects = 1;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(tower.area.Left, tower.area.Right);
                        int y = WorldGen.genRand.Next(tower.area.Top, tower.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && MiscTools.Tile(x, y + 1).TileType != TileID.Platforms && MiscTools.Tile(x + 1, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y, 2))
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, style: 50, notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                            {
                                #region chestloot
                                var itemsToAdd = new List<(int type, int stack)>();

                                int piece = structureCount % 4 == 3 ? ItemID.AncientCobaltLeggings : structureCount % 4 == 2 ? ItemID.AncientCobaltBreastplate : structureCount % 4 == 1 ? ItemID.AncientCobaltHelmet : ItemID.CelestialMagnet;
                                if (piece != missingPiece)
                                {
                                    itemsToAdd.Add((piece, 1));
                                }

                                StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                                StructureTools.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // OVERGROWN CABIN
            while (structureCount < Main.maxTilesX * Main.maxTilesY / 1200f / (ModContent.GetInstance<Worldgen>().ExperimentalWorldgen ? 840 : 420) * ModContent.GetInstance<Worldgen>().CabinFrequency)
            {
                progress.Set((progressCounter + structureCount / (float)(Main.maxTilesX * Main.maxTilesY / 1200f) / (ModContent.GetInstance<Worldgen>().ExperimentalWorldgen ? 840 : 420)) / uniqueStructures);

                #region spawnconditions
                StructureTools.Dungeon cabin = new StructureTools.Dungeon(WorldGen.genRand.Next(400, Main.maxTilesX - 400), 0, WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(1, 3), 12, 9, 3);

                cabin.Y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200 - cabin.area.Height);

                bool valid = true;
                if (!GenVars.structures.CanPlace(cabin.area, 25))
                {
                    valid = false;
                }
                else if (!StructureTools.InsideBiome(cabin.area, BiomeID.Jungle))
                {
                    valid = false;
                }
                else if (cabin.Y > GenVars.lavaLine && WorldGen.genRand.NextBool(2))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(cabin.area, 10);

                    #region structure
                    MiscTools.Rectangle(cabin.area.Left, cabin.area.Top, cabin.area.Right, cabin.area.Bottom, -1);

                    MiscTools.Terraform(new Vector2(cabin.area.Left, cabin.area.Bottom - 3), 5);
                    MiscTools.Terraform(new Vector2(cabin.area.Right, cabin.area.Bottom - 3), 5);

                    MiscTools.Rectangle(cabin.area.Left - 2, cabin.area.Bottom, cabin.area.Left - 1, cabin.area.Bottom, TileID.Platforms, replace: false);
                    MiscTools.Rectangle(cabin.area.Right + 1, cabin.area.Bottom, cabin.area.Right + 2, cabin.area.Bottom, TileID.Platforms, replace: false);

                    #region rooms
                    int roomCount;

                    for (int i = 0; i < cabin.grid.Width; i++)
                    {
                        cabin.AddMarker(i, cabin.grid.Height - 1, 1);
                    }
                    if (cabin.grid.Height > 1)
                    {
                        int width = WorldGen.genRand.Next(1, cabin.grid.Width + 1);
                        int x = WorldGen.genRand.Next(0, cabin.grid.Width - width);
                        for (int i = x; i < x + width; i++)
                        {
                            cabin.AddMarker(i, cabin.grid.Height - 2, 1);
                        }
                    }

                    //AddMarker(0, 0, 1);
                    //roomCount = (cabin.grid.Height - 1) * cabin.grid.Width / 2;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);
                    //    if (roomCount < cabin.grid.Height)
                    //    {
                    //        cabin.targetCell.Y = roomCount;
                    //    }

                    //    if (!FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //    {
                    //        AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = cabin.grid.Height * cabin.grid.Width / 8;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width - 1);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);

                    //    if (!FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && !FindMarker(cabin.targetCell.X + 1, cabin.targetCell.Y + 1))
                    //    {
                    //        AddMarker(cabin.targetCell.X, cabin.targetCell.Y); AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1);
                    //        AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                    //        if (FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //        {
                    //            AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1, 1);
                    //        }

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/thermalrig/solid", roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < cabin.grid.Height - 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = roomCount + 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = cabin.grid.Height - 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            int index = WorldGen.genRand.Next(2);
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/OvergrownCabin/bathroom", index, cabin.roomPos, ModContent.GetInstance<Remnants>());

                            PlacePainting(cabin.roomPos.X + 6, cabin.roomPos.Y + 4);

                            int chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + (index == 0 ? 3 : 9), cabin.roomPos.Y + 8, TileID.Dressers, style: 2);

                            var itemsToAdd = new List<(int type, int stack)>();

                            itemsToAdd.Add((ItemID.FlareGun, 1));
                            itemsToAdd.Add((ItemID.Flare, WorldGen.genRand.Next(15, 30)));

                            StructureTools.GenericLoot(chestIndex, itemsToAdd);

                            StructureTools.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = 0;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            int index = WorldGen.genRand.Next(2);
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/OvergrownCabin/bed", index, cabin.roomPos, ModContent.GetInstance<Remnants>());

                            int chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + (index == 0 ? 2 : 9), cabin.roomPos.Y + 8, style: 10);

                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[5];
                            specialItems[0] = ItemID.AnkletoftheWind;
                            specialItems[1] = ItemID.FeralClaws;
                            specialItems[2] = ItemID.Boomstick;
                            specialItems[3] = ItemID.StaffofRegrowth;
                            specialItems[4] = ItemID.FiberglassFishingPole;

                            int specialItem = specialItems[structureCount % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));
                            if (specialItem == ItemID.Boomstick)
                            {
                                itemsToAdd.Add((ItemID.MusketBall, Main.rand.Next(30, 60)));
                            }

                            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                            StructureTools.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    #endregion

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width)
                            {
                                if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                                {
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/OvergrownCabin/seat", cabin.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                if (cabin.targetCell.Y == cabin.grid.Height - 1)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/OvergrownCabin/bottom", new Point16(cabin.roomPos.X, cabin.roomPos.Y + cabin.room.Height), ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && (cabin.targetCell.Y == 0 || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1)))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/OvergrownCabin/roof-middle", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || !cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X < cabin.grid.Width - 1 && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/OvergrownCabin/roof-left", new Point16(cabin.roomPos.X - 2, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X > 0 && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/OvergrownCabin/roof-right", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/OvergrownCabin/ladder", cabin.roomPos, ModContent.GetInstance<Remnants>());
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) || !cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/OvergrownCabin/wall", cabin.targetCell.Y == cabin.grid.Height - 1 ? 0 : 1, new Point16(cabin.roomPos.X, cabin.roomPos.Y), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    #region objects
                    int objects;

                    //objects = 1;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                    //    int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType == TileID.GrayBrick && WGTools.Tile(x + 1, y + 1).TileType == TileID.GrayBrick && WGTools.NoDoors(x, y, 2))
                    //    {
                    //        int chestIndex = WorldGen.PlaceChest(x, y, style: 1, notNearOtherChests: true);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    //        {
                    //            #region chestloot
                    //            var itemsToAdd = new List<(int type, int stack)>();

                    //            int[] specialItems = new int[3];
                    //            specialItems[0] = ItemID.HermesBoots;
                    //            specialItems[1] = ItemID.CloudinaBottle;
                    //            specialItems[2] = ItemID.MagicMirror;

                    //            int specialItem = specialItems[structureCount % specialItems.Length];
                    //            itemsToAdd.Add((specialItem, 1));

                    //            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                    //            StructureTools.FillChest(chestIndex, itemsToAdd);
                    //            #endregion

                    //            objects--;
                    //        }
                    //    }
                    //}

                    objects = cabin.grid.Height * cabin.grid.Width / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.ClayPot && !MiscTools.Tile(x, y - 1).HasTile && Framing.GetTileSafely(x, y + 1).TileType == TileID.RichMahogany && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceObject(x, y, TileID.ClayPot);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                            {
                                WorldGen.PlaceTile(x, y - 1, TileID.ImmatureHerbs, style: 1);
                                objects--;
                            }
                        }
                    }

                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    #endregion

                    #region cleanup
                    for (int y = cabin.area.Top - 4; y <= cabin.area.Bottom + 2; y++)
                    {
                        for (int x = cabin.area.Left - 2; x <= cabin.area.Right + 2; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (y == cabin.area.Bottom + 1 && tile.HasTile && TileID.Sets.IsBeam[tile.TileType] && (!MiscTools.Tile(x, y + 1).HasTile || MiscTools.Tile(x, y + 1).TileType != TileID.RichMahoganyBeam))
                            {
                                int j = y;
                                MiscTools.WoodenBeam(x, j);
                            }
                        }
                    }

                    StructureTools.AddDecorations(cabin.area);
                    StructureTools.AddTheming(cabin.area);
                    StructureTools.AddVariation(cabin.area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // BURIED CABIN
            while (structureCount < Main.maxTilesX * Main.maxTilesY / 1200f / 300 * ModContent.GetInstance<Worldgen>().CabinFrequency)
            {
                progress.Set((progressCounter + structureCount / (float)(Main.maxTilesX * Main.maxTilesY / 1200f / 300)) / uniqueStructures);

                #region spawnconditions
                StructureTools.Dungeon cabin = new StructureTools.Dungeon(0, 0, WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(1, 3), 8, 9, 3);

                cabin.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f) - cabin.area.Width);
                cabin.Y = structureCount > Main.maxTilesX * Main.maxTilesY / 1200 / 630 ? WorldGen.genRand.Next((int)Main.rockLayer, GenVars.lavaLine - cabin.area.Height) : WorldGen.genRand.Next(GenVars.lavaLine, Main.maxTilesY - 200 - cabin.area.Height);

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.MushroomGrass, TileID.SnowBlock, TileID.IceBlock, TileID.Mud, TileID.JungleGrass, TileID.Sand, TileID.HardenedSand, TileID.Sandstone, TileID.Ebonstone, TileID.Crimstone, TileID.Ash, TileID.Marble, TileID.LihzahrdBrick);

                bool valid = true;
                if (!GenVars.structures.CanPlace(cabin.area, validTiles, 25))
                {
                    valid = false;
                }
                else if (!StructureTools.AvoidsBiomes(cabin.area, new int[] { BiomeID.Granite, BiomeID.Toxic, BiomeID.Obsidian, BiomeID.SunkenSea }))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(cabin.area, 10);

                    #region structure
                    MiscTools.Rectangle(cabin.area.Left, cabin.area.Top, cabin.area.Right, cabin.area.Bottom, -1);

                    MiscTools.Terraform(new Vector2(cabin.area.Left, cabin.area.Bottom - 3), 5);
                    MiscTools.Terraform(new Vector2(cabin.area.Right, cabin.area.Bottom - 3), 5);

                    MiscTools.Rectangle(cabin.area.Left - 2, cabin.area.Bottom, cabin.area.Left - 1, cabin.area.Bottom, TileID.Platforms, replace: false);
                    MiscTools.Rectangle(cabin.area.Right + 1, cabin.area.Bottom, cabin.area.Right + 2, cabin.area.Bottom, TileID.Platforms, replace: false);

                    #region rooms
                    int roomCount;

                    for (int i = 0; i < cabin.grid.Width; i++)
                    {
                        cabin.AddMarker(i, cabin.grid.Height - 1, 1);
                    }
                    if (cabin.grid.Height > 1)
                    {
                        int width = WorldGen.genRand.Next(2, cabin.grid.Width + 1);
                        int x = WorldGen.genRand.Next(0, cabin.grid.Width - width);
                        for (int i = x; i < x + width; i++)
                        {
                            cabin.AddMarker(i, cabin.grid.Height - 2, 1);
                        }
                    }

                    //cabin.AddMarker(0, 0, 1);
                    //roomCount = (cabin.grid.Height - 1) * cabin.grid.Width / 2;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);
                    //    if (roomCount < cabin.grid.Height)
                    //    {
                    //        cabin.targetCell.Y = roomCount;
                    //    }

                    //    if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //    {
                    //        cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = cabin.grid.Height * cabin.grid.Width / 8;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width - 1);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);

                    //    if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && !cabin.FindMarker(cabin.targetCell.X + 1, cabin.targetCell.Y + 1))
                    //    {
                    //        cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y); cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1);
                    //        cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                    //        if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //        {
                    //            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1, 1);
                    //        }

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/thermalrig/solid", cabin.roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < cabin.grid.Height - 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = roomCount + 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = 0;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            int index = WorldGen.genRand.Next(2);
                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/BuriedCabin/bed", index, cabin.roomPos, ModContent.GetInstance<Remnants>());

                            int chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + (index == 0 ? 2 : 6), cabin.roomPos.Y + 8, TileID.Dressers, style: biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Jungle ? 2 : biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Tundra ? 18 : 0);

                            var itemsToAdd = new List<(int type, int stack)>();

                            itemsToAdd.Add((ItemID.FlareGun, 1));
                            itemsToAdd.Add((ItemID.Flare, WorldGen.genRand.Next(15, 30)));

                            StructureTools.GenericLoot(chestIndex, itemsToAdd);

                            StructureTools.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = cabin.grid.Height - 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/BuriedCabin/blank", cabin.roomPos, ModContent.GetInstance<Remnants>());

                            WorldGen.PlaceObject(cabin.roomPos.X + 4, cabin.roomPos.Y + 5, TileID.Painting3X3, style: 45);

                            int chestIndex = 0;
                            int style = biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Jungle ? 10 : biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Tundra ? 11 : 1;
                            if (WorldGen.genRand.NextBool(2))
                            {
                                chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + 2, cabin.roomPos.Y + 8, style: style);

                                //if (structureCount % 2 == 0)
                                //{
                                //    WorldGen.PlaceObject(cabin.roomPos.X + 5, cabin.roomPos.Y + 8, TileID.Anvils);
                                //}
                                //else
                                 WorldGen.PlaceObject(cabin.roomPos.X + 5, cabin.roomPos.Y + 8, TileID.SharpeningStation);
                            }
                            else
                            {
                                chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + 5, cabin.roomPos.Y + 8, style: style);

                                //if (structureCount % 2 == 0)
                                //{
                                //    WorldGen.PlaceObject(cabin.roomPos.X + 2, cabin.roomPos.Y + 8, TileID.Anvils);
                                //}
                                //else
                                WorldGen.PlaceObject(cabin.roomPos.X + 3, cabin.roomPos.Y + 8, TileID.SharpeningStation);
                            }
                            //if (biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) != BiomeID.Jungle)
                            //{
                            //    WorldGen.PlaceTile(cabin.roomPos.X + 4, cabin.roomPos.Y + 8, TileID.MetalBars, style: 2);
                            //    WorldGen.PlaceTile(cabin.roomPos.X + 4, cabin.roomPos.Y + 7, TileID.MetalBars, style: 2);
                            //}

                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[5];
                            specialItems[0] = ItemID.MagicMirror;
                            specialItems[1] = ItemID.CloudinaBottle;
                            specialItems[2] = ItemID.BandofRegeneration;
                            specialItems[3] = ItemID.Mace;
                            specialItems[4] = ItemID.TreasureMagnet;

                            int specialItem = specialItems[structureCount % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));

                            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                            StructureTools.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/BuriedCabin/books", cabin.roomPos, ModContent.GetInstance<Remnants>());

                            roomCount++;
                        }
                    }

                    #endregion

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width)
                            {
                                if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/BuriedCabin/seat", WorldGen.genRand.Next(1, 3), cabin.roomPos, ModContent.GetInstance<Remnants>());
                                        PlacePainting(cabin.roomPos.X + 4, cabin.roomPos.Y + 4);
                                    }
                                    else StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/BuriedCabin/seat", 0, cabin.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                if (cabin.targetCell.Y == cabin.grid.Height - 1)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/BuriedCabin/bottom", new Point16(cabin.roomPos.X, cabin.roomPos.Y + cabin.room.Height), ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && (cabin.targetCell.Y == 0 || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1)))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/BuriedCabin/roof-middle", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || !cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X < cabin.grid.Width - 1 && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/BuriedCabin/roof-left", new Point16(cabin.roomPos.X - 2, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X > 0 && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1))
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/BuriedCabin/roof-right", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/BuriedCabin/ladder", cabin.roomPos, ModContent.GetInstance<Remnants>());
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) || !cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/BuriedCabin/wall", cabin.targetCell.Y == cabin.grid.Height - 1 ? 0 : 1, new Point16(cabin.roomPos.X, cabin.roomPos.Y), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    #region objects
                    int objects;

                    //objects = 1;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                    //    int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType == TileID.GrayBrick && WGTools.Tile(x + 1, y + 1).TileType == TileID.GrayBrick && WGTools.NoDoors(x, y, 2))
                    //    {
                    //        int chestIndex = WorldGen.PlaceChest(x, y, style: 1, notNearOtherChests: true);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    //        {
                    //            #region chestloot
                    //            var itemsToAdd = new List<(int type, int stack)>();

                    //            int[] specialItems = new int[3];
                    //            specialItems[0] = ItemID.HermesBoots;
                    //            specialItems[1] = ItemID.CloudinaBottle;
                    //            specialItems[2] = ItemID.MagicMirror;

                    //            int specialItem = specialItems[structureCount % specialItems.Length];
                    //            itemsToAdd.Add((specialItem, 1));

                    //            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                    //            StructureTools.FillChest(chestIndex, itemsToAdd);
                    //            #endregion

                    //            objects--;
                    //        }
                    //    }
                    //}

                    objects = cabin.grid.Height * cabin.grid.Width / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.ClayPot && !MiscTools.Tile(x, y - 1).HasTile && Framing.GetTileSafely(x, y + 1).TileType == TileID.WoodBlock && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceObject(x, y, TileID.ClayPot);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                            {
                                WorldGen.PlaceTile(x, y - 1, TileID.ImmatureHerbs, style: 2);
                                objects--;
                            }
                        }
                    }

                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && MiscTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    #endregion

                    #region cleanup
                    for (int y = cabin.area.Top - 4; y <= cabin.area.Bottom + 2; y++)
                    {
                        for (int x = cabin.area.Left - 2; x <= cabin.area.Right + 2; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (y == cabin.area.Bottom + 1 && tile.HasTile && TileID.Sets.IsBeam[tile.TileType] && (!MiscTools.Tile(x, y + 1).HasTile || MiscTools.Tile(x, y + 1).TileType != TileID.WoodenBeam))
                            {
                                int j = y;
                                MiscTools.WoodenBeam(x, j);
                            }
                        }
                    }

                    StructureTools.AddDecorations(cabin.area);
                    StructureTools.AddTheming(cabin.area);
                    StructureTools.AddVariation(cabin.area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            //structureCount = 0; // CACHE
            //while (structureCount < Main.maxTilesX / 84 * ModContent.GetInstance<Worldgen>().CacheFrequency)
            //{
            //    progress.Set((progressCounter + structureCount / (float)(Main.maxTilesY / 84)) / uniqueStructures);

            //    #region spawnconditions
            //    Structures.Dungeon cache = new Structures.Dungeon(0, 0, WorldGen.genRand.Next(1, 4), 1, 7, 6, 1);
            //    cache.X = WorldGen.genRand.Next(400, Main.maxTilesX - 400 - cache.area.Width);
            //    cache.Y = WorldGen.genRand.Next((int)(Main.worldSurface * 0.5f), (int)Main.worldSurface - cache.area.Height);

            //    bool[] invalidTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.HardenedSand, TileID.Sandstone, TileID.Ash, TileID.Ebonstone, TileID.Crimstone, TileID.LihzahrdBrick, TileID.LivingWood);

            //    bool openLeft = true;
            //    bool openRight = true;

            //    bool valid = true;
            //    if (!GenVars.structures.CanPlace(cache.area, invalidTiles, 25))
            //    {
            //        valid = false;
            //    }
            //    else if (!Structures.AvoidsBiomes(cache.area, new int[] { BiomeID.Desert }))
            //    {
            //        valid = false;
            //    }
            //    else
            //    {
            //        for (int j = cache.area.Top; j < cache.area.Bottom; j++)
            //        {
            //            for (int i = cache.area.Left + 1; i <= cache.area.Right - 1; i++)
            //            {
            //                if (!Main.tile[i, j].HasTile)
            //                {
            //                    valid = false;
            //                }
            //            }
            //        }

            //        if (valid)
            //        {
            //            for (int j = cache.area.Top + 2; j < cache.area.Bottom - 1; j++)
            //            {
            //                if (Main.tile[cache.area.Left - 1, j].HasTile || Main.tile[cache.area.Left - 1, j].WallType == 0)
            //                {
            //                    openLeft = false;
            //                }
            //                if (Main.tile[cache.area.Right + 1, j].HasTile || Main.tile[cache.area.Right + 1, j].WallType == 0)
            //                {
            //                    openRight = false;
            //                }
            //            }

            //            if (!openLeft && !openRight)
            //            {
            //                valid = false;
            //            }
            //        }
            //    }
            //    #endregion

            //    if (valid)
            //    {
            //        GenVars.structures.AddProtectedStructure(cache.area, 10);

            //        #region structure
            //        #region rooms
            //        cache.targetCell.Y = 0;
            //        int chestRoom = WorldGen.genRand.Next(cache.grid.Width);

            //        WGTools.Rectangle(cache.area.Left, cache.area.Top, cache.area.Right, cache.area.Bottom - 1, TileID.WoodBlock, liquid: 0);
            //        WGTools.Rectangle(cache.area.Left, cache.area.Top + 1, cache.area.Right, cache.area.Bottom - 2, TileID.Dirt, ModContent.WallType<Wood>());
            //        WGTools.Rectangle(cache.area.Left, cache.area.Top + 1, cache.area.Right, cache.area.Bottom - 2, -1);

            //        for (cache.targetCell.X = 0; cache.targetCell.X < cache.grid.Width; cache.targetCell.X++)
            //        {
            //            WGTools.Rectangle(cache.room.Left + 2, cache.room.Top + 1, cache.room.Right - 2, cache.room.Bottom - 2, wall: ModContent.WallType<BrickStone>());

            //            WGTools.Rectangle(cache.room.Left, cache.room.Top + 1, cache.room.Left, cache.room.Bottom - 2, !openLeft && cache.targetCell.X == 0 ? TileID.WoodBlock : TileID.WoodenBeam);
            //            WGTools.Rectangle(cache.room.Right, cache.room.Top + 1, cache.room.Right, cache.room.Bottom - 2, !openRight && cache.targetCell.X == cache.grid.Width - 1 ? TileID.WoodBlock : TileID.WoodenBeam);
            //            if (cache.targetCell.X == chestRoom)
            //            {
            //                int chestIndex = WorldGen.PlaceChest(cache.room.X + 3, cache.room.Bottom - 2);
            //                var itemsToAdd = new List<(int type, int stack)>();

            //                Structures.GenericLoot(chestIndex, itemsToAdd, 1);
            //                Structures.FillChest(chestIndex, itemsToAdd);
            //            }
            //        }
            //        #endregion

            //        #region cleanup
            //        WGTools.PlaceObjectsInArea(cache.area.Left + 1, cache.room.Bottom - 2, cache.area.Right - 2, cache.room.Bottom - 2, TileID.WorkBenches);
            //        WGTools.PlaceObjectsInArea(cache.area.Left + 1, cache.room.Bottom - 2, cache.area.Right - 1, cache.room.Bottom - 2, TileID.Chairs);
            //        Structures.AddDecorations(cache.area);
            //        Structures.AddTheming(cache.area);
            //        Structures.AddVariation(cache.area);
            //        #endregion
            //        #endregion

            //        structureCount++;
            //    }
            //}

            progressCounter++;

            structureCount = 0; // MINING PLATFORM
            while (false)//structureCount < Main.maxTilesX * Main.maxTilesY / 1200f / 175)// * ModContent.GetInstance<Worldgen>().PlatformFrequency)
            {
                progress.Set((progressCounter + structureCount / (float)(Main.maxTilesX * Main.maxTilesY / 1200f / 175)) / uniqueStructures);

                #region spawnconditions

                int x = structureCount < Main.maxTilesX * Main.maxTilesY / 1200f / (175 * 5) ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.4f), (int)(Main.maxTilesX * 0.6f)) : WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.4f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.6f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200);
                int height = Math.Max(WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 7));
                Rectangle area = new Rectangle(x - 3, y - height * 6 - 8, 7, height * 6 + 8);
                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.Sand, TileID.HardenedSand, TileID.Sandstone, TileID.Ebonstone, TileID.Crimstone, TileID.Ash, TileID.LihzahrdBrick);

                bool valid = true;
                if (!GenVars.structures.CanPlace(area, validTiles, 25))
                {
                    valid = false;
                }
                else if (!StructureTools.AvoidsBiomes(area, new int[] { BiomeID.Glowshroom, BiomeID.Granite, BiomeID.Toxic, BiomeID.Obsidian, BiomeID.SunkenSea }))
                {
                    valid = false;
                }
                else
                {
                    for (int j = y - height * 6 - 6; j <= y + 3; j++)
                    {
                        for (int i = x - 3; i <= x + 3; i++)
                        {
                            if (j < y - 1)
                            {
                                if (MiscTools.Solid(i, j))
                                {
                                    valid = false;
                                }
                            }
                            else if (j > y)
                            {
                                if (!MiscTools.Solid(i, j))
                                {
                                    valid = false;
                                }
                            }
                            else if (MiscTools.Tile(i, j).LiquidAmount > 0 && MiscTools.Tile(i, j).LiquidType == LiquidID.Lava)
                            {
                                valid = false;
                            }
                        }
                    }

                    if (valid)
                    {
                        int length = 0;
                        for (int i = x + 2; !MiscTools.Solid(i, y - height * 6); i++)
                        {
                            length++;
                        }
                        for (int i = x - 2; !MiscTools.Solid(i, y - height * 6); i--)
                        {
                            length++;
                        }
                        if (length > 40)
                        {
                            valid = false;
                        }
                    }
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(area, 10);

                    #region structure
                    MiscTools.Rectangle(x - 3, y, x + 3, y, TileID.WoodBlock);
                    for (int k = 0; k < height; k++)
                    {
                        y -= 6;
                        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Platform/ladder", new Point16(x - 3, y), ModContent.GetInstance<Remnants>());
                    }
                    for (int i = x + 2; !MiscTools.Solid(i, y); i++)
                    {
                        MiscTools.Tile(i, y).TileType = biomes.FindBiome(i, y) == BiomeID.Glowshroom ? TileID.MushroomBlock : biomes.FindBiome(i, y) == BiomeID.Jungle ? TileID.RichMahogany : biomes.FindBiome(i, y) == BiomeID.Tundra ? TileID.BorealWood : TileID.WoodBlock;
                        MiscTools.Tile(i, y).HasTile = true;
                    }
                    for (int i = x - 2; !MiscTools.Solid(i, y); i--)
                    {
                        MiscTools.Tile(i, y).TileType = biomes.FindBiome(i, y) == BiomeID.Glowshroom ? TileID.MushroomBlock : biomes.FindBiome(i, y) == BiomeID.Jungle ? TileID.RichMahogany : biomes.FindBiome(i, y) == BiomeID.Tundra ? TileID.BorealWood : TileID.WoodBlock;
                        MiscTools.Tile(i, y).HasTile = true;
                    }

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/Platform/cabin", new Point16(x - 5, y - 8), ModContent.GetInstance<Remnants>());

                    #region cleanup

                    StructureTools.AddTheming(area);
                    StructureTools.AddVariation(area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }
        }

        private bool CreatePlatform(int left, int right, int y)
        {
            if (!GenVars.structures.CanPlace(new Rectangle(left, y, right - left, 1), 1))
            {
                return false;
            }
            else
            {
                for (int j = y - 3; j <= y + 3; j++)
                {
                    for (int i = left - 3; i <= right + 3; i++)
                    {
                        if (WorldGen.SolidTile(i, j))
                        {
                            return false;
                        }
                    }
                }
            }

            GenVars.structures.AddProtectedStructure(new Rectangle(left, y, right - left + 1, Terrain.Maximum - y), 1);

            MiscTools.Rectangle(left - 1, y, right + 1, y, TileID.Platforms, style: 2);
            MiscTools.Rectangle(left, y, right, y, wall: ModContent.WallType<WoodLattice>(), replace: false);
            MiscTools.Rectangle(left + 1, y + 1, right - 1, y + 1, wall: ModContent.WallType<WoodMahogany>());

            MiscTools.Tile(left, y).TileType = TileID.RichMahogany;
            MiscTools.WoodenBeam(left, y);

            MiscTools.Tile(right, y).TileType = TileID.RichMahogany;
            MiscTools.WoodenBeam(right, y);

            StructureTools.AddVariation(new Rectangle(left, y, right - left, 1), 1);

            return true;
        }

        private void PlacePainting(int x, int y)
        {
            int style2 = Main.rand.Next(10);

            if (style2 == 0)
            {
                style2 = 20;
            }
            else if (style2 == 1)
            {
                style2 = 21;
            }
            else if (style2 == 2)
            {
                style2 = 22;
            }
            else if (style2 == 3)
            {
                style2 = 24;
            }
            else if (style2 == 4)
            {
                style2 = 25;
            }
            else if (style2 == 5)
            {
                style2 = 26;
            }
            else if (style2 == 6)
            {
                style2 = 28;
            }
            else if (style2 == 7)
            {
                style2 = 33;
            }
            else if (style2 == 8)
            {
                style2 = 34;
            }
            else if (style2 == 9)
            {
                style2 = 35;
            }

            WorldGen.PlaceObject(x, y, TileID.Painting3X3, style: style2);
        }
    }

    public class BoulderTraps : GenPass
    {
        public BoulderTraps(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Boulders");

            int count = 0;

            while (count < Main.maxTilesX * Main.maxTilesY / 1200f / 21 * ModContent.GetInstance<Worldgen>().TrapFrequency)
            {
                progress.Set(count / (float)(Main.maxTilesX * Main.maxTilesY / 1200f / 21 * ModContent.GetInstance<Worldgen>().TrapFrequency));

                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300);

                if ((WorldGen.genRand.NextBool(2) || y > Main.rockLayer) && (biomes.FindBiome(x, y) == BiomeID.None || biomes.FindBiome(x, y) == BiomeID.Jungle) && GenVars.structures.CanPlace(new Rectangle(x - 2, y - 3, 6, 6), 6))
                {
                    bool valid = true;
                    for (int j = y - 3; j <= y + 2; j++)
                    {
                        for (int i = x - 2; i <= x + 3; i++)
                        {
                            if (!MiscTools.Solid(i, j) || Main.tileOreFinderPriority[Main.tile[i, j].TileType] > 0)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        int length = 0;
                        while (true)
                        {
                            if (!WorldGen.SolidOrSlopedTile(Main.tile[x, y + 3 + length]) && !WorldGen.SolidOrSlopedTile(Main.tile[x + 1, y + 3 + length]))
                            {
                                length++;
                            }
                            else break;
                        }
                        if (length > 10)
                        {
                            int plateX = x + WorldGen.genRand.Next(2);
                            int plateY = y + 3;

                            while (!MiscTools.Solid(plateX, plateY + 1))
                            {
                                plateY++;
                            }

                            if (!MiscTools.Solid(plateX - 1, plateY) && !MiscTools.Solid(plateX + 1, plateY) && MiscTools.Tile(plateX, plateY).LiquidAmount < 255 && MiscTools.Tile(plateX, plateY).TileType != TileID.MinecartTrack)
                            {
                                WorldGen.KillTile(plateX, plateY);
                                WorldGen.PlaceTile(plateX, plateY, TileID.PressurePlates, style: 3);
                                MiscTools.Wire(plateX, plateY, plateX, y + 3);

                                MiscTools.Rectangle(x - 1, y - 2, x + 2, y + 1, TileID.GrayBrick);
                                MiscTools.Rectangle(x, y - 1, x + 1, y, -1, ModContent.WallType<BrickStone>());
                                WorldGen.PlaceTile(x + 1, y, TileID.Boulder);

                                for (int j = y + 1; j <= y + 2; j++)
                                {
                                    for (int i = x; i <= x + 1; i++)
                                    {
                                        MiscTools.Tile(i, j).RedWire = true;
                                        MiscTools.Tile(i, j).HasActuator = true;
                                    }
                                }

                                GenVars.structures.AddProtectedStructure(new Rectangle(x - 2, y - 3, 6, 6));

                                count++;
                            }
                        }
                    }
                }
            }
        }
    }

    public class IceTemples : GenPass
    {
        public IceTemples(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        List<Marker> markers;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.IceTemples");

            int structureCount;

            structureCount = 0;
            while (structureCount <= Main.maxTilesX * (Main.maxTilesY / 1200) / 1400 * ModContent.GetInstance<Worldgen>().FrozenRuinFrequency)
            {
                #region spawnconditions
                int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int structureY = WorldGen.genRand.Next((int)(Main.worldSurface + 50), Main.maxTilesY - 300);

                bool valid = true;
                if (!GenVars.structures.CanPlace(new Rectangle(structureX - 13, structureY - 25, 28, 27), 50))
                {
                    valid = false;
                }
                else if (biomes.FindBiome(structureX, structureY) != BiomeID.Tundra)
                {
                    valid = false;
                }
                else if (ModLoader.TryGetMod("Stellamod", out Mod lv) && lv.TryFind("AbyssalDirt", out ModTile aDirt) && Main.tile[structureX, structureY].TileType == aDirt.Type)
                {
                    valid = false;
                }
                #endregion

                #region structure
                if (valid)
                {
                    markers = new List<Marker>();

                    int chestIndex;
                    if (WorldGen.genRand.NextBool(2))
                    {
                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/icetemple/treasureroom", 0, new Point16(structureX - 13, structureY - 33), ModContent.GetInstance<Remnants>());
                        chestIndex = WorldGen.PlaceChest(structureX + 2, structureY - 2, style: 11);
                    }
                    else
                    {
                        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/icetemple/treasureroom", 1, new Point16(structureX - 13, structureY - 33), ModContent.GetInstance<Remnants>());
                        chestIndex = WorldGen.PlaceChest(structureX - 2, structureY - 2, style: 11);
                    }
                    #region chestloot
                    var itemsToAdd = new List<(int type, int stack)>();

                    int[] specialItems = new int[6];
                    specialItems[0] = ItemID.IceSkates;
                    specialItems[1] = ItemID.BlizzardinaBottle;
                    specialItems[2] = ItemID.IceBoomerang;
                    specialItems[3] = ItemID.SnowballCannon;
                    specialItems[4] = ItemID.IceBlade;
                    specialItems[5] = ItemID.IceMirror;

                    int specialItem = specialItems[WorldGen.genRand.Next(0, specialItems.Length)];
                    if (structureCount < specialItems.Length)
                    {
                        specialItem = specialItems[structureCount];
                    }

                    itemsToAdd.Add((specialItem, 1));
                    if (specialItem == ItemID.SnowballCannon)
                    {
                        itemsToAdd.Add((ItemID.Snowball, Main.rand.Next(25, 50)));
                    }

                    if (WorldGen.genRand.NextBool(25))
                    {
                        itemsToAdd.Add((ItemID.Fish, 1));
                    }

                    StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                    StructureTools.FillChest(chestIndex, itemsToAdd);
                    #endregion
                    GenVars.structures.AddProtectedStructure(new Rectangle(structureX - 13, structureY - 33, 28, 35));
                    AddMarker(structureX - 14, structureY - 2, 4, true);
                    AddMarker(structureX + 15, structureY - 2, 2, true);
                    AddMarker(structureX - 14, structureY - 19, 4, true);
                    AddMarker(structureX + 15, structureY - 19, 2, true);

                    #region rooms
                    int index = WorldGen.genRand.Next(0, markers.Count);
                    //PlaceRoom(4);
                    for (int i = 0; i < 20; i++)
                    {
                        index = WorldGen.genRand.Next(0, markers.Count);
                        PlaceRoom(index);
                    }
                    #endregion

                    while (markers.Count > 0)
                    {
                        int x = (int)markers[0].position.X;
                        int y = (int)markers[0].position.Y;
                        if (markers[0].direction == 2 || markers[0].direction == 4)
                        {
                            Tunnel(new Vector2(x, y - 5.5f), 7f, 3);

                            if (markers[0].direction == 2)
                            {
                                WorldGen.PlaceTile(x, y + 2, TileID.Platforms, style: 35);
                            }
                            else WorldGen.PlaceTile(x, y + 2, TileID.Platforms, style: 35);
                        }
                        else Tunnel(new Vector2(x, y), 1.5f, scaleY: 2);

                        RemoveMarker(0);
                    }

                    structureCount++;
                }
                #endregion
            }
            //Spikes();
            #region cleanup
            FastNoiseLite weathering = new FastNoiseLite();
            weathering.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            weathering.SetFrequency(0.2f);
            weathering.SetFractalType(FastNoiseLite.FractalType.FBm);
            weathering.SetFractalOctaves(3);

            for (int y = (int)Main.worldSurface; y < Main.maxTilesY - 200; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = MiscTools.Tile(x, y);

                    if (MiscTools.Tile(x, y + 1).TileType == TileID.PlanterBox && MiscTools.Tile(x, y + 1).TileFrameY == 6 * 18 && WorldGen.genRand.NextBool(2))
                    {
                        WorldGen.PlaceTile(x, y, TileID.ImmatureHerbs, style: 6);
                    }

                    if (tile.WallType == ModContent.WallType<BrickIce>())
                    {
                        if (tile.TileType == TileID.Platforms)
                        {
                            WorldGen.KillTile(x, y, true);
                        }
                        else if (MiscTools.Solid(x, y) && !MiscTools.Tile(x, y + 1).HasTile && weathering.GetNoise(x + WorldGen.genRand.Next(-3, 4), y + WorldGen.genRand.Next(-3, 4)) > 0)
                        {
                            if (tile.TileType == TileID.IceBrick)
                            {
                                tile.TileType = TileID.IceBlock;
                            }
                        }
                        if (MiscTools.Tile(x, y - 1).TileType == TileID.Torches && !WorldGen.genRand.NextBool(4))
                        {
                            MiscTools.Tile(x, y - 1).TileFrameX += 18 * 3;
                        }
                    }
                }
            }
            #endregion
        }

        #region functions
        private void PlaceRoom(int index, int id = -1)
        {
            Marker savedMarker = markers[index];

            #region setup
            int x = (int)savedMarker.position.X;
            int y = (int)savedMarker.position.Y;

            if (id == -1)
            {
                if (savedMarker.direction == 3)
                {
                    id = 2;
                }
                else if (savedMarker.direction == 1)
                {
                    id = 3;
                }
                else if (savedMarker.door)
                {
                    id = 0;
                }
                else if (WorldGen.genRand.NextBool(2))
                {
                    id = WorldGen.genRand.Next(2, 4);
                }
                //else if (WorldGen.genRand.NextBool(3))
                //{
                //    id = 4;
                //}
                else id = 1;
            }

            Rectangle room = new Rectangle(x, y, 7, 18);
            if (id == 4)
            {
                room.Width *= 2;
            }

            int doorY = 0;

            doorY += room.Height - 4;

            if (savedMarker.direction == 4)
            {
                room.X -= room.Width - 1;
            }
            if (savedMarker.direction == 1)
            {
                room.Y -= room.Height - 1;
            }
            if (savedMarker.direction == 1 || savedMarker.direction == 3)
            {
                room.X -= (room.Width - 1) / 2;
            }
            else room.Y -= doorY;
            #endregion

            bool[] tiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.Granite);
            if (GenVars.structures.CanPlace(new Rectangle(room.X + 1, room.Y + 1, room.Width - 2, room.Height - 2), tiles))
            {
                Point16 position = new Point16(room.X, room.Y);

                //Fill(room.Center.ToVector2(), room.Height);

                RemoveMarker(index);
                if (id == 0)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/icetemple/doorway", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 1)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/icetemple/basic", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 2)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/icetemple/shaft-bottom", position, ModContent.GetInstance<Remnants>());
                    if (savedMarker.direction != 3)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y, 1, false);
                    }
                }
                else if (id == 3)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/icetemple/shaft-top", position, ModContent.GetInstance<Remnants>());
                    if (savedMarker.direction != 1)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y + (room.Height - 1), 3, false);
                    }
                }
                else if (id == 4)
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/icetemple/pool", position, ModContent.GetInstance<Remnants>());
                    //WGTools.DrawRectangle(position.X + 5, position.Y + 11, position.X + 8, position.Y + 11, liquid: 255, liquidType: 0);
                }

                if (savedMarker.direction != 2)
                {
                    AddMarker(room.X - 1, room.Y + doorY, 4, id != 0 && WorldGen.genRand.NextBool(2));
                }
                if (savedMarker.direction != 4)
                {
                    AddMarker(room.X + room.Width, room.Y + doorY, 2, id != 0 && WorldGen.genRand.NextBool(2));
                }

                GenVars.structures.AddProtectedStructure(room);
            }
        }

        private void Tunnel(Vector2 position, float size, float scaleX = 1, float scaleY = 1)
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFrequency(0.2f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalLacunarity(1.75f);

            for (int y = (int)(position.Y - size * 2 * scaleY); y <= position.Y + size * 2 * scaleY; y++)
            {
                for (int x = (int)(position.X - size * 2 * scaleX); x <= position.X + size * 2 * scaleX; x++)
                {
                    float threshold = Vector2.Distance(position, new Vector2((x - position.X) / scaleX + position.X, (y - position.Y) / scaleY + position.Y)) / size;
                    Tile tile = MiscTools.Tile(x, y);
                    if (GenVars.structures.CanPlace(new Rectangle(x, y, 1, 1)) && MiscTools.Solid(x, y) && tile.TileType != TileID.IceBrick && tile.TileType != TileID.ClosedDoor)
                    {
                        if (noise.GetNoise(x, y) <= 1 - threshold)
                        {
                            tile.HasTile = false;
                        }
                    }
                }
            }
        }

        private void AddMarker(int x, int y, int direction, bool shaft)
        {
            markers.Add(new Marker(new Vector2(x, y), direction, shaft));

            if (x <= Main.maxTilesX * 0.1f || x >= Main.maxTilesX * 0.9f)
            {
                RemoveMarker(markers.Count - 1);
            }
        }

        private void RemoveMarker(int index)
        {
            int x = (int)markers[index].position.X;
            int y = (int)markers[index].position.Y;

            if (markers[index].direction == 1)
            {
                MiscTools.Rectangle(x - 1, y + 1, x + 1, y + 1, type: -1, style: 35);
                WorldGen.PlaceTile(x, y + 1, TileID.Chain);
            }
            else if (markers[index].direction == 3)
            {
                MiscTools.Rectangle(x - 1, y - 1, x + 1, y - 1, TileID.Platforms, style: 35, replace: false);
            }

            markers.RemoveAt(index);
        }
        #endregion

        internal struct Marker
        {
            public Vector2 position;
            public int direction;
            public bool door;
            public Marker(Vector2 _position, int _direction, bool _shaft)
            {
                position = _position;
                direction = _direction;
                door = _shaft;
            }
        }
    }

    public class ThermalRigs : GenPass
    {
        public ThermalRigs(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        int X;
        int Y;

        int[,,] layout;

        int roomsHorizontal;
        int roomsVertical;
        int roomWidth = 8;
        int roomHeight = 12;

        Rectangle location => new Rectangle(X, Y, roomWidth * roomsHorizontal - 1, roomHeight * roomsVertical - 1);

        int cellX;
        int cellY;

        Point16 roomPos => new Point16(X + cellX * roomWidth, Y + cellY * roomHeight);

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.ThermalRigs");

            int structureCount;

            structureCount = Main.maxTilesX / 525;
            while (structureCount > 0)
            {
                #region spawnconditions
                roomsHorizontal = WorldGen.genRand.Next(3, 7);
                roomsVertical = WorldGen.genRand.Next(2, 5);

                X = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(100, (int)(Main.maxTilesX * 0.25f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.75f), Main.maxTilesX - 100) - location.Width;
                Y = WorldGen.genRand.Next(Main.maxTilesY - 120 - location.Height, Main.maxTilesY - 100 - location.Height);

                bool valid = true;
                if (!GenVars.structures.CanPlace(location, 25))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(location);

                    #region structure
                    layout = new int[roomsHorizontal, roomsVertical, 3];
                    MiscTools.Rectangle(location.Left, location.Top, location.Right, location.Bottom, -1);

                    for (cellY = roomsVertical - 1; cellY >= 0; cellY--)
                    {
                        for (cellX = 0; cellX < roomsHorizontal; cellX++)
                        {
                            if (cellX == 0)
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/thermalrig/side", 0, new Point16(roomPos.X - 2, roomPos.Y), ModContent.GetInstance<Remnants>());
                            }
                            if (cellX == roomsHorizontal - 1)
                            {
                                StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/thermalrig/side", 1, new Point16(roomPos.X + roomWidth, roomPos.Y), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    #region rooms
                    int roomCount;

                    //AddMarker(0, 0, 1);
                    //roomCount = (roomsVertical - 1) * roomsHorizontal / 2;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                    //    cellY = WorldGen.genRand.Next(0, roomsVertical);
                    //    if (roomCount < roomsVertical)
                    //    {
                    //        cellY = roomCount;
                    //    }

                    //    if (!FindMarker(cellX, cellY, 1))
                    //    {
                    //        AddMarker(cellX, cellY, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = roomsVertical * roomsHorizontal / 8;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(0, roomsHorizontal - 1);
                    //    cellY = WorldGen.genRand.Next(0, roomsVertical);

                    //    if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY + 1))
                    //    {
                    //        AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);
                    //        AddMarker(cellX, cellY, 2);

                    //        if (FindMarker(cellX, cellY, 1))
                    //        {
                    //            AddMarker(cellX, cellY + 1, 1);
                    //        }

                    //        StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/thermalrig/solid", roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < roomsVertical - 1)
                    {
                        cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                        cellY = roomCount;

                        if (!FindMarker(cellX, cellY))
                        {
                            AddMarker(cellX, cellY);
                            AddMarker(cellX, cellY, 1);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/thermalrig/open", 1, roomPos, ModContent.GetInstance<Remnants>());

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < roomsVertical * roomsHorizontal / 8 + roomsHorizontal)
                    {
                        cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                        if (roomCount < roomsHorizontal)
                        {
                            cellY = roomsVertical - 1;
                        }
                        else cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/thermalrig/solid", roomPos, ModContent.GetInstance<Remnants>());

                            roomCount++;
                        }
                    }

                    roomCount = roomsVertical * roomsHorizontal / 4;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY);
                            AddMarker(cellX, cellY, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/thermalrig/partial", roomPos, ModContent.GetInstance<Remnants>());

                            roomCount--;
                        }
                    }
                    #endregion

                    for (cellY = roomsVertical - 1; cellY >= 0; cellY--)
                    {
                        for (cellX = 0; cellX < roomsHorizontal; cellX++)
                        {
                            if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("IndustrialPanel", out ModTile IndustrialPanel))
                            {
                                if (!FindMarker(cellX, cellY))
                                {
                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Common/thermalrig/open", 0, roomPos, ModContent.GetInstance<Remnants>());
                                }
                                if (FindMarker(cellX, cellY, 2))
                                {
                                    WorldGen.PlaceObject(roomPos.X + 4, roomPos.Y + 11, TileID.Hellforge);
                                }

                                if (!FindMarker(cellX, cellY - 1) || FindMarker(cellX, cellY - 1, 1))
                                {
                                    //if (FindMarker(cellX, cellY) && !FindMarker(cellX, cellY, 1))
                                    //{
                                    //    for (int i = roomPos.X + 1; i < roomPos.X + roomWidth - 1; i++)
                                    //    {
                                    //        if (WGTools.Solid(i - 1, roomPos.Y) && WGTools.Solid(i, roomPos.Y) && WGTools.Solid(i + 1, roomPos.Y))
                                    //        {
                                    //            WGTools.Tile(i, roomPos.Y).TileType = TileID.IronBrick;
                                    //        }
                                    //    }
                                    //}
                                    if (wombatqol.TryFind("IndustrialPlatform", out ModTile IndustrialPlatform))
                                    {
                                        WorldGen.PlaceTile(roomPos.X, roomPos.Y, IndustrialPanel.Type); WorldGen.PlaceTile(roomPos.X + 8, roomPos.Y, IndustrialPanel.Type);
                                        MiscTools.Rectangle(roomPos.X + 1, roomPos.Y, roomPos.X + 7, roomPos.Y, IndustrialPlatform.Type, replace: false);
                                    }
                                }
                            }
                        }
                    }

                    #region objects
                    int objects;
                    //objects = 6;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    //    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                    //    valid = true;

                    //    if (Framing.GetTileSafely(x, y).TileType == TileID.FishingCrate)
                    //    {
                    //        valid = false;
                    //    }
                    //    else for (int i = 0; i <= 1; i++)
                    //        {
                    //            if (WGTools.GetTile(x + i, y + 1).TileType == ModContent.TileType<factoryplatform>() || WGTools.GetTile(x + i, y + 1).TileType == TileID.Platforms || WGTools.GetTile(x + i, y + 1).TileType == TileID.Tables)
                    //            {
                    //                valid = false;
                    //                break;
                    //            }
                    //        }

                    //    if (valid)
                    //    {
                    //        WorldGen.PlaceObject(x, y, TileID.FishingCrate, style: 0);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.FishingCrate)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    //objects = 3;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    //    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                    //    valid = true;

                    //    if (Framing.GetTileSafely(x, y).TileType == TileID.AmmoBox)
                    //    {
                    //        valid = false;
                    //    }
                    //    else for (int i = 0; i <= 1; i++)
                    //        {
                    //            if (WGTools.GetTile(x + i, y + 1).TileType == ModContent.TileType<factoryplatform>() || WGTools.GetTile(x + i, y + 1).TileType == TileID.Platforms || WGTools.GetTile(x + i, y + 1).TileType == TileID.Tables)
                    //            {
                    //                valid = false;
                    //                break;
                    //            }
                    //        }

                    //    if (valid)
                    //    {
                    //        WorldGen.PlaceObject(x, y, TileID.AmmoBox);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.AmmoBox)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion

                    structureCount--;
                }
            }
        }

        private void DungeonRoom(int left, int top, int right, int bottom, int cellX, int cellY, int tile = -2, int wall = -2, bool add = true, bool replace = true, int style = 0, int liquid = -1, int liquidType = -1)
        {
            MiscTools.Rectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top - roomHeight / 2, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom - roomHeight / 2, tile, wall, add, replace, style, liquid, liquidType);
        }
        private void AddMarker(int cellX, int cellY, int layer = 0)
        {
            layout[cellX, cellY, layer] = -1;
        }
        private bool FindMarker(int cellX, int cellY, int layer = 0)
        {
            //if (cellY >= roomsVertical)
            //{
            //    return true;
            //}
            if (cellX < 0 || cellX >= roomsHorizontal || cellY < 0 || cellY >= roomsVertical)
            {
                return false;
            }
            return layout[cellX, cellY, layer] == -1;
        }
    }
}
