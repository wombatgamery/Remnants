using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Remnants.Content.Walls.Parallax;
using static Remnants.Content.World.BiomeMap;
using System.Linq;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles;
using Remnants.Content.Walls;
using System.Collections.Generic;

namespace Remnants.Content.World
{
    public class TerrainPasses : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            bool spiritReforged = ModLoader.TryGetMod("SpiritReforged", out Mod sr);

            RemWorld.InsertPass(tasks, new Terrain("Terrain", 1), RemWorld.FindIndex(tasks, "Terrain") + 2);
            RemWorld.InsertPass(tasks, new Caves("Caves", 2), RemWorld.FindIndex(tasks, "Tunnels") + 1);

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Dirt Wall Backgrounds"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Clean Up Dirt"));

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Rocks In Dirt"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Dirt In Rocks"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Dirt To Mud"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Grass"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Clay"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Silt"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Slush"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Spreading Grass"));

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Small Holes"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Dirt Layer Caves"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Rock Layer Caves"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Surface Caves"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Wavy Caves"));

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Tunnels"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Mount Caves"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Mountain Caves"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Lakes"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Wet Jungle"));

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Shinies"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Surface Ore and Stone"));
            if (ModContent.GetInstance<Worldgen>().OreFrequency > 0)
            {
                RemWorld.InsertPass(tasks, new Ores("Minerals", 1), spiritReforged ? RemWorld.FindIndex(tasks, "Savanna") + 1 : RemWorld.FindIndex(tasks, "Dirt Rock Wall Runner") + 1);
            }

            if (ModContent.GetInstance<Worldgen>().CloudDensity > 0)
            {
                RemWorld.InsertPass(tasks, new Clouds("Clouds", 1), RemWorld.FindIndex(tasks, "Settle Liquids Again"));
            }
        }
    }

    public class Terrain : GenPass
    {
        public Terrain(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        private static float scaleX => Main.maxTilesX / 4200f;
        private static float scaleY => Main.maxTilesY / 1200f;

        public static int Minimum => (int)(Main.worldSurface / 2);
        public static int Maximum => (int)(Main.worldSurface - 60);
        public static int Middle => (Minimum + Maximum) / 2;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            #region terrain
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Terrain");

            FastNoiseLite roughness = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            roughness.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            roughness.SetFrequency(0.025f);
            roughness.SetFractalType(FastNoiseLite.FractalType.FBm);
            roughness.SetFractalOctaves(3);

            FastNoiseLite forestHills = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            forestHills.SetNoiseType(FastNoiseLite.NoiseType.Value);
            forestHills.SetFrequency(0.005f);
            forestHills.SetFractalType(FastNoiseLite.FractalType.FBm);
            forestHills.SetFractalOctaves(3);
            forestHills.SetFractalGain(0.25f);

            int mountain1Pos = (Tundra.Left + Tundra.Width / 3) * biomes.Scale;
            int mountain2Pos = (Tundra.Right + 1 - Tundra.Width / 3) * biomes.Scale;
            float mountain1Height = WorldGen.genRand.NextFloat(1.5f, 2f);
            float mountain2Height = WorldGen.genRand.NextFloat(1.5f, 2f);
            if (WorldGen.genRand.NextBool(2))
            {
                mountain1Height -= 0.5f;
            }
            else mountain2Height -= 0.5f;

            FastNoiseLite mountainJags = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            mountainJags.SetNoiseType(FastNoiseLite.NoiseType.Value);
            mountainJags.SetFrequency(0.04f);
            mountainJags.SetFractalType(FastNoiseLite.FractalType.FBm);
            mountainJags.SetFractalOctaves(3);
            mountainJags.SetFractalGain(0.3f);

            FastNoiseLite mountainCaves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            mountainCaves.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            mountainCaves.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
            mountainCaves.SetFrequency(0.005f);
            mountainCaves.SetFractalType(FastNoiseLite.FractalType.None);

            FastNoiseLite mountainWalls = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            mountainWalls.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            mountainWalls.SetFrequency(0.05f);
            mountainWalls.SetFractalType(FastNoiseLite.FractalType.FBm);
            mountainWalls.SetFractalOctaves(3);

            FastNoiseLite jungleCrags = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            jungleCrags.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            jungleCrags.SetFrequency(0.025f);
            jungleCrags.SetFractalType(FastNoiseLite.FractalType.FBm);
            jungleCrags.SetFractalOctaves(2);
            jungleCrags.SetFractalGain(0.15f);
            jungleCrags.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            jungleCrags.SetCellularJitter(0.75f);

            FastNoiseLite junglePools = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            junglePools.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            junglePools.SetFrequency(0.05f);
            junglePools.SetFractalType(FastNoiseLite.FractalType.FBm);
            junglePools.SetFractalOctaves(3);

            for (int y = 40; y <= Main.maxTilesY - 200; y++)
            {
                progress.Set((y - (int)(Main.worldSurface * 0.35f)) / (float)(Main.maxTilesY - 200 - (int)(Main.worldSurface * 0.35f)) / 3);

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    #region terrain
                    if (y < Main.worldSurface)
                    {
                        float beachFactor = 1 - (float)SmootherStep(0, 1, MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, 0, 325)) / (150 * scaleX), 0, 1) * MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, Main.maxTilesX - 325, Main.maxTilesX)) / (150 * scaleX), 0, 1));


                        float i = x + roughness.GetNoise(x, y + 999) * 10;
                        float j = y + roughness.GetNoise(x + 999, y) * 10 + (5 * beachFactor);


                        float tundraDistance = Math.Min(MathHelper.Distance(i / (float)biomes.Scale, Tundra.Left), MathHelper.Distance(i / (float)biomes.Scale, Tundra.Right + 1));
                        float corruptionDistance = Math.Min(MathHelper.Distance(i / (float)biomes.Scale, Corruption.X - Corruption.Size), MathHelper.Distance(i / (float)biomes.Scale, Corruption.X + Corruption.Size + 1));
                        float jungleDistance = Math.Min(MathHelper.Distance(i / (float)biomes.Scale, Jungle.Left), MathHelper.Distance(i / (float)biomes.Scale, Jungle.Right + 1));
                        float desertDistance = Math.Min(MathHelper.Distance(i / (float)biomes.Scale, Desert.Left), MathHelper.Distance(i / (float)biomes.Scale, Desert.Right + 1));
                        float beachDistance = Math.Min(MathHelper.Distance(i / (float)biomes.Scale, 7), MathHelper.Distance(i / (float)biomes.Scale, biomes.Width - 7));

                        float forestDistance = Math.Min(beachDistance, Math.Min(Math.Min(tundraDistance, corruptionDistance), Math.Min(desertDistance, jungleDistance)));


                        float _terrain = 0.75f;

                        _terrain = ((_terrain - 1) * (1 - beachFactor)) + 1;

                        if (biomes.FindBiome(x, y, false) != BiomeID.Jungle && biomes.FindBiome(x, y, false) != BiomeID.Desert)
                        {
                            _terrain -= 0.2f * (float)SmoothStep(0, 1, MathHelper.Clamp(Math.Min(Math.Min(desertDistance, jungleDistance), beachDistance - (Main.maxTilesX / 700f)) / (Main.maxTilesX / 1050f), 0, 1));
                            //_terrain -= 0.25f * (float)SmootherStep(0, 1, MathHelper.Clamp(Math.Min(MathHelper.Distance(x, Main.maxTilesX * 0.4f), MathHelper.Distance(x, Main.maxTilesX * 0.6f)) / (Main.maxTilesX / 21), 0, 1));
                        }

                        bool mountainCave = false;
                        if (biomes.FindBiome(x, y, false) == BiomeID.None)
                        {
                            _terrain += forestHills.GetNoise(i, 0) / 6 * MathHelper.Clamp(forestDistance / (Main.maxTilesX / 2100f), 0, 1);
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Tundra)
                        {
                            _terrain += 0.2f * (float)SmoothStep(0, 1, MathHelper.Clamp(tundraDistance / (Main.maxTilesX / 2100f), 0, 1));
                            _terrain -= 0.8f * ((float)-Math.Cos(MathHelper.Pi * (1 - Math.Clamp(MathHelper.Distance(i, mountain1Pos) / (Main.maxTilesX / 25f), 0, 1)) / 2) + 1) * mountain1Height;
                            _terrain -= 0.8f * ((float)-Math.Cos(MathHelper.Pi * (1 - Math.Clamp(MathHelper.Distance(i, mountain2Pos) / (Main.maxTilesX / 25f), 0, 1)) / 2) + 1) * mountain2Height;
                            _terrain += mountainJags.GetNoise(i, 0) / 4 * (1 - MathHelper.Clamp(Math.Min(MathHelper.Distance(i, mountain1Pos), MathHelper.Distance(i, mountain2Pos)) / (Main.maxTilesX / 28f), 0, 1));

                            if (tundraDistance > Main.maxTilesX / 4200f && j < (Maximum * 2 + Middle) / 3 && j > Minimum)
                            {
                                if (mountainCaves.GetNoise(i, j * 2 + mountainJags.GetNoise(i, j / 2) * 50) > -0.2f)
                                {
                                    mountainCave = true;
                                }
                            }
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Jungle)
                        {
                            if (jungleDistance > (Main.maxTilesX / 8400f) && MathHelper.Distance(i / biomes.Scale, Jungle.Center) >= (Main.maxTilesX / 8400f) && MathHelper.Distance(i / biomes.Scale, (Jungle.Left + Jungle.Center) / 2) >= (Main.maxTilesX / 8400f) && MathHelper.Distance(i / biomes.Scale, (Jungle.Right + 1 + Jungle.Center) / 2) >= (Main.maxTilesX / 8400f))
                            {
                                _terrain += MathHelper.Clamp(jungleCrags.GetNoise(i, 0), -0.7f, MathHelper.Distance(i, (Jungle.Center) * biomes.Scale) <= Main.maxTilesX / 84 ? 0 : (i + junglePools.GetNoise(i, 0) * 25) % 20 < 8 && jungleCrags.GetNoise(i, 0) > 0.4f ? 0.3f : 0.5f) / 2;
                            }
                        }

                        bool jungleChasm = false;// MathHelper.Distance(i, (Jungle.Center) * biomes.Scale) <= 20;


                        float threshold = _terrain * (Maximum - Minimum) + Minimum;


                        float oceanMultiplier = (float)SmoothStep(0, 1, MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, 0, 150)) / 150, 0.15f, 1) * MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, Main.maxTilesX - 150, Main.maxTilesX)) / 150, 0.15f, 1));
                        threshold = (threshold - (int)Main.worldSurface) * oceanMultiplier + (int)Main.worldSurface;


                        if (beachFactor == 0 && biomes.FindBiome(x, y, false) != BiomeID.Tundra)
                        {
                            threshold = MathHelper.Clamp(threshold, Minimum, (int)Main.worldSurface - 30);
                        }

                        int waterLevel = (biomes.FindBiome(x, y, false) == BiomeID.Jungle ? (jungleChasm ? (int)Main.worldSurface : (Middle + Maximum * 5) / 6) : (int)Main.worldSurface - 60);

                        if (biomes.FindBiome(x, y, false) == BiomeID.None)
                        {
                            j = j * (1 - Math.Clamp(forestDistance, 0, 1)) + (float)SmoothStep((float)Math.Floor((float)j / 24) * 24, (float)Math.Ceiling((float)j / 24) * 24, (j % 24) / 24) * Math.Clamp(forestDistance, 0, 1);
                        }

                        if (j >= threshold && !mountainCave)
                        {
                            tile.HasTile = true;
                        }
                        else
                        {
                            tile.HasTile = false;
                            if (y >= waterLevel)
                            {
                                tile.LiquidAmount = 255;
                            }

                            if (j >= threshold && mountainCave)
                            {
                                if (j >= threshold + 50 && mountainCaves.GetNoise(i, j * 2 + mountainJags.GetNoise(i * 2, j) * 50) > -0.1f && mountainWalls.GetNoise(i, j) < 0f)
                                {
                                    tile.WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                                }
                            }
                        }

                        if (biomes.FindBiome(x, y, false) == BiomeID.Tundra)
                        {
                            //float caveDistance = Vector2.Distance(new Vector2(i, j + mountainJags.GetNoise(i * 4, 0) * 5), new Vector2(i > (mountain1Pos + mountain2Pos) / 2 ? mountain2Pos : mountain1Pos, (Minimum + Middle * 2) / 3));
                            //if (caveDistance < 30)
                            //{
                            //    tile.HasTile = false;
                            //    if (caveDistance < 20)
                            //    {
                            //        tile.WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                            //    }
                            //}
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Jungle)
                        {
                            if (!MiscTools.SurroundingTilesActive(x, y))
                            {
                                threshold = Maximum - ((Maximum - Minimum) * MathHelper.Clamp(jungleCrags.GetNoise(i * 3, 0), 0, 1)) / 2;
                                if (j > threshold + 7)
                                {
                                    tile.WallType = WallID.DirtUnsafe;
                                }
                                else if (j > threshold)
                                {
                                    tile.WallType = WallID.JungleUnsafe;
                                }
                            }

                            if (jungleDistance > 1)
                            {
                                j += (int)(roughness.GetNoise(x / 2f, y) * 40 * MathHelper.Clamp(jungleDistance - 1, 0, 1));

                                int tunnel1Height = (Middle + Maximum) / 2;
                                int tunnel2Height = (Middle + Maximum) / 2 - (waterLevel - (Middle + Maximum) / 2) + 5;
                                int tunnel3Height = waterLevel - 5;

                                if (j >= tunnel1Height - 11 && j < tunnel1Height || j >= tunnel2Height - 11 && j < tunnel2Height || j >= tunnel3Height - 11 && j < tunnel3Height && jungleDistance > 1.5f)// && !junglePool)
                                {
                                    tile.HasTile = false;
                                    if (y >= waterLevel)
                                    {
                                        tile.LiquidAmount = 255;
                                    }
                                }
                            }
                        }
                    }
                    else tile.HasTile = true;
                    #endregion

                    #region materials
                    int layer = biomes.FindLayer(x, y);

                    if (layer >= biomes.caveLayer && layer < biomes.lavaLayer && biomes.MaterialBlend(x, y, true) <= -0.35f)
                    {
                        tile.TileType = TileID.Silt;
                    }
                    else if (biomes.MaterialBlend(x, y, frequency: 2) >= (layer >= biomes.surfaceLayer && layer < biomes.caveLayer ? 0.2f : -0.2f))
                    {
                        tile.TileType = TileID.Stone;
                    }
                    else if (layer >= biomes.surfaceLayer && layer < biomes.caveLayer && biomes.MaterialBlend(x, y, true) >= 0.25f)
                    {
                        tile.TileType = TileID.ClayBlock;
                    }
                    else tile.TileType = TileID.Dirt;
                    #endregion
                }
            }
            #endregion

            #region features
            int count = 0;
            while (count < Main.maxTilesX / 420)
            {
                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - 75) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 75, Main.maxTilesX - 400);
                int y = Minimum;
                float radius = WorldGen.genRand.NextFloat(10, 20);

                while (!MiscTools.Tile(x, y).HasTile)
                {
                    y++;
                }

                bool valid = true;

                if (biomes.FindBiome(x, y) != BiomeID.None || Math.Min(MathHelper.Distance(x / biomes.Scale, Corruption.X - Corruption.Size), MathHelper.Distance(x / biomes.Scale, Corruption.X + Corruption.Size + 1)) < 2 || MathHelper.Distance(x, Main.maxTilesX * (GenVars.dungeonSide == 1 ? 0.9f : 0.1f)) < 100)
                {
                    valid = false;
                }
                else
                {
                    for (int j = y - (int)radius / 2; j <= y + (int)radius / 2; j++)
                    {
                        for (int i = x - (int)radius * 2; i <= x + (int)radius  * 2; i++)
                        {
                            if (MiscTools.Tile(i, j).LiquidAmount == 255)
                            {
                                valid = false;
                            }
                        }
                    }

                    if (valid)
                    {
                        for (int j = y - (int)radius / 2 - 5; j <= y + (int)radius / 2 + 5; j++)
                        {
                            for (int i = x - (int)radius - 5; i <= x + (int)radius + 5; i++)
                            {
                                if (j < y - 2)
                                {
                                    if (MiscTools.Tile(i, j).HasTile)
                                    {
                                        valid = false;
                                    }
                                }
                                else if (j > y)
                                {
                                    if (!MiscTools.Tile(i, j).HasTile)
                                    {
                                        valid = false;
                                    }
                                }
                            }
                        }
                    }
                }

                if (valid)
                {
                    MiscTools.CustomTileRunner(x, y - 2, radius + 1, roughness, -1, yFrequency: 2, strength: 3);
                    MiscTools.Rectangle(x - (int)radius, y + 1, x + (int)radius, y + (int)radius, liquid: 255);

                    count++;
                }
            }

            count = 0;
            while (count < Main.maxTilesX / 840)
            {
                int width = WorldGen.genRand.Next(8, 13);
                int elevation = WorldGen.genRand.Next(2, 5);
                int height = WorldGen.genRand.Next(18, 25) + elevation;
                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - 300 - width) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 300, Main.maxTilesX - 400 - width);
                int y = Minimum;

                while (!MiscTools.Tile(x, y).HasTile)
                {
                    y++;
                }

                bool valid = true;

                if (biomes.FindBiome(x, y) != BiomeID.None || Math.Min(MathHelper.Distance(x / biomes.Scale, Corruption.X - Corruption.Size), MathHelper.Distance(x / biomes.Scale, Corruption.X + Corruption.Size + 1)) < 2 || MathHelper.Distance(x, Main.maxTilesX * (GenVars.dungeonSide == 1 ? 0.9f : 0.1f)) < 100)
                {
                    valid = false;
                }
                else
                {
                    for (int j = y - height; j <= y + 5; j++)
                    {
                        for (int i = x - 3; i <= x + width + 3; i++)
                        {
                            if (MiscTools.Tile(i, j).LiquidAmount == 255)
                            {
                                valid = false;
                            }
                            else if (j < y - 1)
                            {
                                if (MiscTools.Tile(i, j).HasTile)
                                {
                                    valid = false;
                                }
                            }
                            else if (j > y + 2)
                            {
                                if (!MiscTools.Tile(i, j).HasTile)
                                {
                                    valid = false;
                                }
                            }
                        }
                    }
                }

                if (valid)
                {
                    MiscTools.Rectangle(x + 1 + WorldGen.genRand.Next(-1, 2), y - height, x + width - 2 + WorldGen.genRand.Next(-1, 2), WorldGen.genRand.Next(y - 12, y - 9) - elevation, TileID.Stone);
                    MiscTools.Rectangle(x + WorldGen.genRand.Next(-1, 2), WorldGen.genRand.Next(y - height + 3, y - height + 6), x + width + WorldGen.genRand.Next(-1, 2), y - 6 - elevation, TileID.Stone);

                    MiscTools.Rectangle(x - 1 + WorldGen.genRand.Next(-1, 2), y - elevation, x + width + 2 + WorldGen.genRand.Next(-1, 2), y + 3, TileID.Stone);

                    //WGTools.CustomTileRunner(x, )
                    MiscTools.Rectangle(x, y - 10, x + width, y + 1, wall: WallID.GrassUnsafe);

                    count++;
                }
            }

            count = 0;
            while (count < 2)
            {
                float radius = WorldGen.genRand.NextFloat(20, 30);

                int x = count == 1 ? WorldGen.genRand.Next(60 + (int)radius, 220 - (int)radius) : Main.maxTilesX - WorldGen.genRand.Next(60 + (int)radius, 220 - (int)radius);
                int y = Maximum;

                bool valid = true;

                for (int j = (int)(y - radius); j <= y + radius / 2; j++)
                {
                    for (int i = (int)(x - radius - 5); i <= x + radius + 5; i++)
                    {
                        if (Main.tile[i, j].HasTile)
                        {
                            valid = false;
                        }
                    }
                }

                if (valid)
                {
                    for (int j = (int)(y - radius); j <= y + radius; j++)
                    {
                        float flatness = j > y ? 2 : 4;

                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j * flatness), new Vector2(x, y * flatness)) < radius + roughness.GetNoise(i * 5, 0) * (40 / flatness) && WorldGen.InWorld(i, j))
                            {
                                Tile tile = Main.tile[i, j];

                                tile.HasTile = true;
                                tile.TileType = TileID.Stone;
                            }
                        }
                    }

                    count++;
                }
            }
            #endregion

            #region smoothing
            for (int k = 0; k < 5; k++)
            {
                progress.Set(1 / 3f + (k / 5f) * (0.65f - 1 / 3f));

                for (int y = (int)(Main.worldSurface * 0.35f); y <= Main.worldSurface; y++)
                {
                    for (int x = 40; x < Main.maxTilesX - 40; x++)
                    {
                        Tile tile = Main.tile[x, y];

                        if (tile.HasTile && MiscTools.AdjacentTiles(x, y) <= 1)
                        {
                            tile.TileType = (ushort)ModContent.TileType<nothing>();
                        }
                    }
                }

                for (int y = (int)(Main.worldSurface * 0.35f); y <= Main.worldSurface; y++)
                {
                    for (int x = 40; x < Main.maxTilesX - 40; x++)
                    {
                        Tile tile = Main.tile[x, y];

                        if (tile.HasTile && MiscTools.AdjacentTiles(x, y) <= 2 && (MiscTools.Tile(x, y - 1).HasTile || MiscTools.Tile(x, y + 1).HasTile))
                        {
                            int nearbyTiles = 0;
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                for (int i = x - 1; i <= x + 1; i++)
                                {
                                    if (MiscTools.Tile(i, j).HasTile)
                                    {
                                        nearbyTiles++;
                                    }
                                }
                            }

                            if (nearbyTiles == 4)
                            {
                                tile.TileType = (ushort)ModContent.TileType<nothing>();
                            }
                        }
                    }
                }

                for (int y = (int)(Main.worldSurface * 0.35f); y <= Main.worldSurface; y++)
                {
                    for (int x = 40; x < Main.maxTilesX - 40; x++)
                    {
                        Tile tile = Main.tile[x, y];

                        if (tile.HasTile && tile.TileType == ModContent.TileType<nothing>())
                        {
                            tile.TileType = TileID.Stone;
                            tile.HasTile = false;
                        }
                    }
                }
            }
            #endregion

            #region cleanup
            for (int y = Minimum; y <= Main.worldSurface; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (tile.HasTile)
                    {
                        if (biomes.FindBiome(x, y, false) == BiomeID.Jungle && !MiscTools.Tile(x, y - 1).HasTile && MiscTools.Tile(x, y - 1).LiquidAmount < 51)
                        {
                            MiscTools.Tile(x, y - 1).LiquidAmount = 85;
                        }

                        if (tile.TileType == TileID.Stone || tile.TileType == TileID.ClayBlock)
                        {
                            for (int i = 1; i <= WorldGen.genRand.Next(8, 10); i++)
                            {
                                if (!MiscTools.Tile(x, y - i).HasTile)
                                {
                                    tile.TileType = TileID.Dirt;
                                    break;
                                }
                            }

                            if (tile.TileType != TileID.Dirt && biomes.FindBiome(x, y, false) != BiomeID.Tundra)
                            {
                                for (int i = 1; i <= WorldGen.genRand.Next(2, 4); i++)
                                {
                                    if (!MiscTools.Tile(x, y + i).HasTile)
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
            #endregion

            #region walls
            for (int y = 40; y <= Main.maxTilesY - 200; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (y > Main.worldSurface || tile.WallType == WallID.DirtUnsafe || tile.WallType == 0 && (MiscTools.Tile(x, y - 1).WallType != 0 || MiscTools.SurroundingTilesActive(x, y)))
                    {
                        int layer = biomes.FindLayer(x, y);

                        if (tile.TileType == TileID.Stone)
                        {
                            tile.WallType = layer >= biomes.lavaLayer ? WallID.Cave8Unsafe : WallID.RocksUnsafe1;
                        }
                        else tile.WallType = layer >= biomes.lavaLayer ? WallID.CaveWall2 : layer >= biomes.surfaceLayer ? WallID.Cave6Unsafe : WallID.DirtUnsafe;
                    }
                }
            }

            for (int k = 0; k < 8; k++)
            {
                progress.Set(0.65f + (k / 8f) * 0.25f);

                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = 40; x <= Main.maxTilesX - 40; x++)
                    {
                        if (!MiscTools.SurroundingTilesActive(x, y))
                        {
                            int adjacentWalls = 0;

                            if (MiscTools.Tile(x + 1, y).WallType != 0 || MiscTools.Tile(x + 1, y).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (MiscTools.Tile(x, y + 1).WallType != 0 || MiscTools.Tile(x, y + 1).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (MiscTools.Tile(x - 1, y).WallType != 0 || MiscTools.Tile(x - 1, y).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (MiscTools.Tile(x, y - 1).WallType != 0 || MiscTools.Tile(x, y - 1).HasTile)
                            {
                                adjacentWalls++;
                            }

                            if (k >= 5)
                            {
                                if (adjacentWalls < 8 - k)
                                {
                                    MiscTools.Tile(x, y).WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                                }
                            }
                            else if (adjacentWalls < 4 && WorldGen.genRand.NextBool(4 / (4 - adjacentWalls)))
                            {
                                MiscTools.Tile(x, y).WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                            }
                        }
                    }
                }

                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = 40; x <= Main.maxTilesX - 40; x++)
                    {
                        if (MiscTools.Tile(x, y).WallType == (ushort)ModContent.WallType<Walls.dev.nothing>())
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }
            #endregion

            progress.Set(1);

            #region objects
            FastNoiseLite boulders = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            boulders.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            boulders.SetFrequency(0.05f);
            boulders.SetFractalType(FastNoiseLite.FractalType.FBm);
            boulders.SetFractalOctaves(3);

            count = 0;
            while (count < Main.maxTilesX / 42)
            {
                progress.Set(0.9f + count / (Main.maxTilesX / 42f) * 0.1f);

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - 75) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 75, Main.maxTilesX - 400);
                int y = Minimum;
                float radius = WorldGen.genRand.NextFloat(3, 4) - (count < Main.maxTilesX / 84 ? 0.5f : 0);

                while (!MiscTools.Tile(x, y).HasTile)
                {
                    y++;
                }

                bool valid = true;

                if (biomes.FindBiome(x, y) != BiomeID.None && biomes.FindBiome(x, y) != BiomeID.Jungle)
                {
                    valid = false;
                }
                else
                {
                    for (int j = y - (int)radius - 10; j <= y + (int)radius + 3; j++)
                    {
                        for (int i = x - (int)radius - 5; i <= x + (int)radius + 5; i++)
                        {
                            if (MiscTools.Tile(i, j).LiquidAmount == 255)
                            {
                                valid = false;
                            }
                            else if (j < y - 3)
                            {
                                if (MiscTools.Tile(i, j).HasTile)
                                {
                                    valid = false;
                                }
                            }
                            else if (j > y + 2)
                            {
                                if (!MiscTools.Tile(i, j).HasTile)
                                {
                                    valid = false;
                                }
                            }
                            else if (j == y)
                            {
                                if (MiscTools.Tile(i, j).HasTile && MiscTools.Tile(i, j).TileType == TileID.Stone)
                                {
                                    valid = false;
                                }
                            }
                        }
                    }
                }

                if (valid)
                {
                    MiscTools.CustomTileRunner(x, y, radius, boulders, count >= Main.maxTilesX / 84 ? TileID.Stone : -2, count < Main.maxTilesX / 84 ? WallID.RocksUnsafe1 : -2);

                    count++;
                }
            }
            #endregion

            #region swamps
            //progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Swamps");

            //FastNoiseLite distribution = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            //distribution.SetNoiseType(FastNoiseLite.NoiseType.Value);
            //distribution.SetFrequency(0.015f);
            //distribution.SetFractalType(FastNoiseLite.FractalType.FBm);
            //distribution.SetFractalOctaves(3);

            //FastNoiseLite islets = new FastNoiseLite();
            //islets.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            //islets.SetFrequency(0.1f);
            //islets.SetFractalType(FastNoiseLite.FractalType.FBm);
            //islets.SetFractalOctaves(3);
            ////islets.SetFractalGain(0.6f);

            //for (int y = 40; y < Main.rockLayer; y++)
            //{
            //    progress.Set(y / Main.rockLayer);

            //    for (int x = 300; x < Main.maxTilesX - 300; x++)
            //    {
            //        Vector2 point = new Vector2(MathHelper.Clamp(x, 600, Main.maxTilesX - 600), MathHelper.Clamp(y, Middle, (float)Main.worldSurface));
            //        float multiplier = MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y), point) / (Main.maxTilesY / 18)) * 2, 0, 1);
            //        multiplier *= MathHelper.Clamp(Vector2.Distance(new Vector2(x, y), new Vector2(Main.maxTilesX / 2, y)) / (Main.maxTilesY / 6), 0, 1);

            //        if (biomes.FindLayer(x, y) < biomes.surfaceLayer && (distribution.GetNoise(x, y * 2) + 1 / 2) * multiplier > 0.15f && biomes.FindBiome(x, y) != BiomeID.Desert && WGTools.Tile(x, y).HasTile)
            //        {
            //            //if ((noise.GetNoise(x, y * 3) + noise2.GetNoise(x * 1.5f, y)) / 2 > 0.06f)
            //            //{
            //            //    WGTools.GetTile(x, y).active(true);
            //            //}
            //            WGTools.Tile(x, y).WallType = biomes.FindBiome(x, y) == BiomeID.Tundra ? WallID.SnowWallUnsafe : WallID.DirtUnsafe;

            //            if (islets.GetNoise(x, y * 2) > -0.7f)
            //            {
            //                WGTools.Tile(x, y).HasTile = true;
            //                WGTools.Tile(x, y).TileType = biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.SnowBlock : biomes.FindBiome(x, y) == BiomeID.Jungle ? TileID.Mud : TileID.Dirt;
            //            }
            //            else
            //            {
            //                WGTools.Tile(x, y).HasTile = false;
            //                WGTools.Tile(x, y).LiquidAmount = 85;

            //                if (islets.GetNoise(x, y * 2) < WorldGen.genRand.NextFloat(0.9f, 0.95f))
            //                {
            //                    WGTools.Tile(x, y).WallType = 0;
            //                }
            //            }

            //            //if (WorldGen.genRand.Next(100) <= 60)
            //            //{
            //            //    WGTools.Tile(x, y).wall = (ushort)ModContent.WallType<devwall>();
            //            //}
            //            //else
            //            //{
            //            //    WGTools.Tile(x, y).LiquidAmount = 255;
            //            //}
            //        }
            //    }
            //}
            #endregion

            #region borders
            for (int y = 40; y < Main.maxTilesY - 40; y += 3)
            {
                WorldGen.TileRunner(41, y, WorldGen.genRand.Next(2, 5) * 2 + 1, 1, ModContent.TileType<Hardstone>());
                WorldGen.TileRunner(Main.maxTilesX - 41, y, WorldGen.genRand.Next(7, 10), 1, ModContent.TileType<Hardstone>());
                for (int x = 0; x < 40; x++)
                {
                    MiscTools.Tile(x, y).HasTile = true;
                    MiscTools.Tile(x, y).TileType = (ushort)ModContent.TileType<Hardstone>();
                }
                for (int x = Main.maxTilesX - 40; x < Main.maxTilesX; x++)
                {
                    MiscTools.Tile(x, y).HasTile = true;
                    MiscTools.Tile(x, y).TileType = (ushort)ModContent.TileType<Hardstone>();
                }
            }
            for (int x = 40; x < Main.maxTilesX - 40; x += 3)
            {
                WorldGen.TileRunner(x, Main.maxTilesY - 43, WorldGen.genRand.Next(2, 5) * 2 + 1, 1, ModContent.TileType<Hardstone>(), true);
                for (int y = Main.maxTilesY - 40; y < Main.maxTilesY; y++)
                {
                    MiscTools.Tile(x, y).HasTile = true;
                    MiscTools.Tile(x, y).TileType = (ushort)ModContent.TileType<Hardstone>();
                }
            }
            #endregion

            //WorldGen.PlaceTile(300, (int)Main.worldSurface - 120, TileID.BubblegumBlock);
            //WorldGen.PlaceTile(Main.maxTilesX - 300, (int)Main.worldSurface - 120, TileID.BubblegumBlock);

            //Debris();

            //for (int i = 0; i < 10; i++)
            //{
            //    WGTools.MoonstoneRock(Main.maxTilesX / 2 + i * 32, (int)Main.worldSurface - 200);
            //}
        }

        private double SmootherStep(double start, double end, float factor)
        {
            return start + ((1 - Math.Cos(MathHelper.Pi * factor)) / 2) * (end - start);
        }

        private double SmoothStep(double start, double end, float factor)
        {
            return start + ((1 - SharpCos(factor)) / 2) * (end - start);
        }

        private double SharpCos(float x)
        {
            return Math.Cos(MathHelper.Pi * (1 - Math.Cos(MathHelper.Pi * x)) / 2);
        }

        private void DoBlend(float x, float y, int type, int chance, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (WorldGen.genRand.NextBool(chance * 10))
                {
                    WorldGen.TileRunner((int)x + WorldGen.genRand.Next(-20, 21), (int)y + WorldGen.genRand.Next(-20, 21), WorldGen.genRand.Next(4, 25), 5, type, false, WorldGen.genRand.NextFloat(-10, 10), WorldGen.genRand.NextFloat(-10, 10));
                }
            }
        }
    }

    public class Caves : GenPass
    {
        public Caves(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Caves");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            float scale = 2f;// * (2 / 1.5f);
            float heightMultiplier = 2;

            float transit1 = (int)Main.worldSurface - 60;
            float transit2 = (int)Main.rockLayer;

            FastNoiseLite caves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caves.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            caves.SetFrequency(0.005f / scale);
            caves.SetFractalType(FastNoiseLite.FractalType.FBm);
            caves.SetFractalOctaves(3);

            FastNoiseLite caveSize = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveSize.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveSize.SetFrequency(0.01f / scale);
            caveSize.SetFractalType(FastNoiseLite.FractalType.FBm);
            caveSize.SetFractalGain(0.7f);
            caveSize.SetFractalOctaves(4);

            FastNoiseLite caveOffset = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveOffset.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveOffset.SetFrequency(0.01f / scale);
            caveOffset.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite caveRoughness = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveRoughness.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveRoughness.SetFrequency(0.075f / scale);
            caveRoughness.SetFractalType(FastNoiseLite.FractalType.FBm);
            FastNoiseLite caveRoughness2 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveRoughness2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveRoughness2.SetFrequency(0.025f / scale);
            caveRoughness2.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite background = new FastNoiseLite();
            background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            background.SetFrequency(0.04f);
            background.SetFractalType(FastNoiseLite.FractalType.FBm);
            background.SetFractalOctaves(2);
            background.SetFractalWeightedStrength(-1);
            background.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);

            for (int y = 40; y < Main.maxTilesY - 100; y++)
            {
                progress.Set((y - ((int)Main.worldSurface - 40)) / (Main.maxTilesY - 200 - ((int)Main.worldSurface - 40)));

                //float divider = -((y - transit1) / (transit2 - transit1)) + 1;
                //float intensity = 0.6f;
                //if (divider < 1) {
                //    divider = 1;
                //}
                //if (divider > 1 + intensity) {
                //    divider = 1 + intensity;
                //}
                //divider = (divider - 1) * intensity + 1;
                for (int x = 325; x < Main.maxTilesX - 325; x++)
                {
                    if (MiscTools.Tile(x, y).HasTile && biomes.FindLayer((int)x, (int)y) >= biomes.surfaceLayer)
                    {
                        //float transit = MathHelper.Clamp((y - transit1) / (transit2 - transit1), 0, 1);// / 2 + 0.5f;

                        float mult = 2;

                        float threshold = 1;
                        //threshold -= MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y * 2.5f), new Vector2(1, ((int)Main.worldSurface - 85) * 2.5f)) / 500) * 3, 0, 1);
                        //threshold -= MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y * 2.5f), new Vector2(Main.maxTilesX, ((int)Main.worldSurface - 85) * 2.5f)) / 500) * 3, 0, 1);

                        float _size = (caveSize.GetNoise(x * mult, y * mult * heightMultiplier) * 0.8f + 0.25f) * threshold;
                        float _caves = caves.GetNoise(x * mult, y * mult * heightMultiplier);
                        float _roughness = caveRoughness.GetNoise(x * mult, y * mult * heightMultiplier) * ((caveRoughness2.GetNoise(x * mult, y * mult * heightMultiplier) + 1) / 2);
                        float _offset = caveOffset.GetNoise(x * mult, y * mult * heightMultiplier);

                        float _overall = (_caves + _roughness / 3) / (1 + 1 / 3);

                        if (biomes.FindLayer(x, y) < biomes.caveLayer)
                        {
                            _caves = caves.GetNoise(x * mult * 4, y * mult * 4 * heightMultiplier);

                            if (_caves + _roughness / 5 > 0.15f)
                            {
                                MiscTools.Tile(x, y).HasTile = false;

                                if (_caves + _roughness / 5 < 0.175f)
                                {
                                    MiscTools.Tile(x, y).LiquidAmount = 255;
                                }

                                if (biomes.FindLayer(x, y) > biomes.surfaceLayer && _caves + _roughness / 5 > 0.25f)
                                {
                                    MiscTools.Tile(x, y).WallType = 0;
                                }
                            }
                        }
                        else
                        {
                            if (_overall - _offset > -_size / 2.5f && _overall - _offset < _size / 2.5f)
                            {
                                MiscTools.Tile(x, y).HasTile = false;
                            }

                            if (biomes.FindLayer(x, y) > biomes.surfaceLayer && biomes.FindLayer(x, y) < biomes.Height - 6)
                            {
                                if (_overall * 2 - _offset > -_size / 2f && _overall * 2 - _offset < _size / 2f)
                                {
                                    MiscTools.Tile(x, y).WallType = 0;
                                }
                            }
                        }


                        if (y > GenVars.lavaLine)
                        {
                            MiscTools.Tile(x, y).LiquidType = LiquidID.Lava;
                        }
                    }
                }
            }

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.FloodedCaves");

            int structureCount = Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 50000;
            while (structureCount > 0)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);

                if (!MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount != 255 && biomes.FindBiome(x, y) != BiomeID.Jungle)
                {
                    int radius = WorldGen.genRand.Next(25, 50);

                    for (int j = Math.Max(y - radius, (int)Main.rockLayer); j <= Math.Min(y + radius, Main.maxTilesY - 300); j++)
                    {
                        for (int i = x - radius; i <= x + radius; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) < radius && WorldGen.InWorld(i, j))
                            {
                                MiscTools.Tile(i, j).LiquidAmount = 255;
                            }
                        }
                    }

                    structureCount--;
                }
            }
        }
    }

    public class Ores : GenPass
    {
        public Ores(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Ores");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int[] blocksToReplace = new int[] { TileID.Dirt, TileID.Grass, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.Stone, TileID.Ebonstone, TileID.Crimstone, TileID.SnowBlock, TileID.IceBlock, TileID.Mud, TileID.JungleGrass, TileID.MushroomGrass, TileID.Sand, TileID.HardenedSand, TileID.Sandstone };

            int rarity;
            int type;

            for (int y = 60; y < Main.maxTilesY - 60; y++)
            {
                progress.Set(((float)y - 60) / (Main.maxTilesY - 60 - 60));

                for (int x = 60; x < Main.maxTilesX - 60; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (biomes.FindBiome(x, y) != BiomeID.Hive && biomes.FindBiome(x, y) != BiomeID.SunkenSea)
                    {
                        float radius = 3;
                        while (radius < 15)
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                break;
                            }
                            else radius++;
                        }

                        if (WorldGen.SolidTile3(x, y) && !Main.wallDungeon[MiscTools.Tile(x, y).WallType] && MiscTools.Tile(x, y).WallType != ModContent.WallType<GardenBrickWall>() && MiscTools.Tile(x, y).WallType != ModContent.WallType<undergrowth>() && MiscTools.Tile(x, y).WallType != WallID.LivingWoodUnsafe && MiscTools.Tile(x, y).WallType != ModContent.WallType<forgottentomb>() && MiscTools.Tile(x, y).WallType != ModContent.WallType<TombBrickWallUnsafe>() && biomes.FindBiome(x, y) != BiomeID.GemCave)
                        {
                            if (y > Main.worldSurface * 0.5f)
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Underworld || biomes.FindBiome(x, y) == BiomeID.AshForest)
                                {
                                    if (y >= Main.maxTilesY - 140)
                                    {
                                        rarity = 10;
                                        OreVein(x, y, WorldGen.genRand.Next(16, 24), rarity, TileID.Hellstone, new int[] { TileID.Ash, TileID.AshGrass, TileID.Obsidian }, 5, 0.55f, 6, 3);
                                    }
                                }
                                else if (x >= 350 && x <= Main.maxTilesX - 350)
                                {
                                    if (y < Main.worldSurface && biomes.FindBiome(x, y) == BiomeID.Desert)
                                    {
                                        rarity = 50;

                                        AddFossil(x, y, rarity);
                                    }
                                    if (y > Main.worldSurface && (biomes.FindBiome(x, y) == BiomeID.Corruption || biomes.FindBiome(x, y) == BiomeID.Crimson))
                                    {
                                        rarity = 5;

                                        type = biomes.FindBiome(x, y) == BiomeID.Crimson ? TileID.Crimtane : TileID.Demonite;

                                        OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 5, 0.55f, 6, 3);
                                    }
                                    else
                                    {
                                        #region tin/copper
                                        if (y < GenVars.lavaLine)
                                        {
                                            rarity = y > Main.worldSurface && y < Main.rockLayer ? 20 : 40;

                                            type = biomes.FindBiome(x, y, false) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Tin : TileID.Copper;

                                            OreVein(x, y, WorldGen.genRand.Next(16, 24), rarity, type, blocksToReplace, 20, 0.4f, 4, 3);
                                        }
                                        #endregion

                                        #region iron/lead
                                        if (y > Main.worldSurface)
                                        {
                                            rarity = y > Main.rockLayer && y < GenVars.lavaLine ? 30 : 60;

                                            type = biomes.FindBiome(x, y, false) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Lead : TileID.Iron;

                                            OreVein(x, y, WorldGen.genRand.Next(24, 36), rarity, type, blocksToReplace, 20, 0.6f, 6, 4);
                                        }
                                        #endregion

                                        #region silver/tungsten
                                        if (y > Main.rockLayer)
                                        {
                                            rarity = y > GenVars.lavaLine ? 20 : 40;

                                            type = biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Tungsten : TileID.Silver;

                                            OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                #region gold/platinum
                                if (biomes.FindBiome(x, y) != BiomeID.Tundra)
                                {
                                    rarity = 5;

                                    type = biomes.FindBiome(x, y) == BiomeID.Jungle ? TileID.Platinum : TileID.Gold; //TileID.Gold;

                                    OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                                }
                                #endregion
                            }

                            //if (y < Main.worldSurface * 0.35f)
                            //{
                            //    if (tile.TileType == TileID.Cloud || tile.TileType == TileID.RainCloud)
                            //    {
                            //        rarity = 4;

                            //        type = ModContent.TileType<starore>();

                            //        OreVein(x, y, WorldGen.genRand.Next(12, 16), rarity, type, new int[] { TileID.Cloud, TileID.RainCloud }, 5, 0.55f, 6, 3);
                            //    }
                            //    //else
                            //    //{
                            //    //    rarity = 2;

                            //    //    type = TileID.Platinum;

                            //    //    OreVein(x, y, WorldGen.genRand.Next(8, 12), rarity, type, 3, 0.5f, 5, 3);
                            //    //}
                            //}
                        }
                    }
                }
            }
        }

        public static void OreVein(int structureX, int structureY, int size, float rarity, int type, int[] blocksToReplace, int steps, float weight = 0.5f, int birthLimit = 4, int deathLimit = 4)
        {
            if (ModContent.GetInstance<Worldgen>().OreFrequency == 0) { return; }

            rarity /= ModContent.GetInstance<Worldgen>().OreFrequency;
            if ((rarity == 0 || WorldGen.genRand.NextBool((int)(rarity * 100))) && !Main.wallDungeon[MiscTools.Tile(structureX, structureY).WallType])
            {
                int width = size;
                int height = size;

                structureX -= width / 2;
                structureY -= height / 2;

                bool[,] cellMap = new bool[width, height];
                bool[,] cellMapNew = cellMap;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (WorldGen.genRand.NextFloat(0, 1) <= weight)
                        {
                            cellMap[x, y] = true;
                        }
                        else cellMap[x, y] = false;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    if (MiscTools.Tile(structureX, structureY + y).HasTile)
                    {
                        cellMap[0, y] = false;
                    }
                    if (MiscTools.Tile(structureX + width - 1, structureY + y).HasTile)
                    {
                        cellMap[width - 1, y] = false;
                    }
                }
                for (int x = 0; x < width; x++)
                {
                    if (MiscTools.Tile(structureX + x, structureY).HasTile)
                    {
                        cellMap[x, 0] = false;
                    }
                    if (MiscTools.Tile(structureX + x, structureY + height - 1).HasTile)
                    {
                        cellMap[x, height - 1] = false;
                    }
                }

                cellMapNew = cellMap;

                for (int i = 0; i < steps; i++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            bool left = cellMap[x - 1, y];
                            bool right = cellMap[x + 1, y];
                            bool top = cellMap[x, y - 1];
                            bool bottom = cellMap[x, y + 1];
                            bool topleft = cellMap[x - 1, y - 1];
                            bool topright = cellMap[x + 1, y - 1];
                            bool bottomleft = cellMap[x - 1, y + 1];
                            bool bottomright = cellMap[x + 1, y + 1];
                            int adjacentTiles = 0;
                            if (left) { adjacentTiles++; }
                            if (right) { adjacentTiles++; }
                            if (top) { adjacentTiles++; }
                            if (bottom) { adjacentTiles++; }
                            if (topleft) { adjacentTiles++; }
                            if (topright) { adjacentTiles++; }
                            if (bottomleft) { adjacentTiles++; }
                            if (bottomright) { adjacentTiles++; }

                            if (cellMap[x, y] == false && adjacentTiles > birthLimit)
                            {
                                cellMapNew[x, y] = true;
                            }
                            if (cellMap[x, y] && adjacentTiles < deathLimit)
                            {
                                cellMapNew[x, y] = false;
                            }
                        }
                    }
                    cellMap = cellMapNew;
                }

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (cellMap[x, y] && WorldGen.InWorld(x + structureX, y + structureY))
                        {
                            Tile tile = MiscTools.Tile(x + structureX, y + structureY);
                            if (tile.HasTile && blocksToReplace.Contains(tile.TileType) && (MiscTools.Solid(x + structureX, y + structureY - 1) || structureY > Main.worldSurface && tile.TileType != TileID.MushroomGrass))
                            {
                                tile.TileType = (ushort)type;
                            }
                        }
                    }
                }
            }
        }

        private void AddFossil(int x, int y, float rarity)
        {
            if (ModContent.GetInstance<Worldgen>().OreFrequency == 0) { return; }

            rarity /= ModContent.GetInstance<Worldgen>().OreFrequency;
            if (WorldGen.genRand.NextBool((int)(rarity * 100)))
            {
                int lifetime = WorldGen.genRand.Next(12, 25);

                Vector2 position = new Vector2(x, y);
                Vector2 velocity = WorldGen.genRand.NextVector2Circular(1, 1);

                float cooldown = 0;

                while (lifetime > 0)
                {
                    Tile tile = MiscTools.Tile(position.X, position.Y);
                    if (tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand)
                    {
                        MiscTools.Tile(position.X, position.Y).TileType = TileID.FossilOre;
                    }

                    position += new Vector2(velocity.X, velocity.Y);

                    velocity += Main.rand.NextVector2Circular(10f, 10f) * 0.1f;
                    if (velocity.Length() > 1)
                    {
                        velocity = Vector2.Normalize(velocity) * 1;
                    }

                    cooldown -= velocity.Length();
                    if (cooldown <= 0)
                    {
                        FossilRib(position, Vector2.Normalize(velocity).RotatedBy(-MathHelper.PiOver2));
                        FossilRib(position, Vector2.Normalize(velocity).RotatedBy(MathHelper.PiOver2));
                        cooldown += 3;
                    }

                    lifetime--;
                }
            }
        }

        private void FossilRib(Vector2 position, Vector2 velocity)
        {
            int lifetime = WorldGen.genRand.Next(3, 6);

            while (lifetime > 0)
            {
                Tile tile = MiscTools.Tile(position.X, position.Y);
                if (tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand)
                {
                    MiscTools.Tile(position.X, position.Y).TileType = TileID.FossilOre;
                }

                position += new Vector2(velocity.X, velocity.Y);


                lifetime--;
            }
        }
    }

    public class Gems : GenPass
    {
        public Gems(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Gems");

            for (int y = 40; y < Main.maxTilesY - 40; y++)
            {
                progress.Set(((float)y - 40) / (Main.maxTilesY - 40 - 40));

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.GemCave)
                    {
                        Tile tile = Main.tile[x, y];
                        if (tile.WallType == 0 || tile.WallType == WallID.AmethystUnsafe || tile.WallType == WallID.TopazUnsafe || tile.WallType == WallID.SapphireUnsafe || tile.WallType == WallID.EmeraldUnsafe || tile.WallType == WallID.RubyUnsafe || tile.WallType == WallID.DiamondUnsafe)
                        {
                            if (!tile.HasTile && MiscTools.AdjacentTiles(x, y, true) > 0 && WorldGen.genRand.NextBool(4))
                            {
                                WorldGen.PlaceTile(x, y, TileID.ExposedGems, style: RemTile.GetGemType(y));
                            }
                        }
                    }
                }
            }
        }
    }

    public class Clouds : GenPass
    {
        public Clouds(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Clouds");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int countInitial = (int)(Main.maxTilesX / 420f * ModContent.GetInstance<Worldgen>().CloudDensity);// (int)(((Main.maxTilesX / 4200f * Main.worldSurface * 0.5f) / 20f) * ModContent.GetInstance<Client>().CloudDensity);
            int count = countInitial;
            while (count > 0)
            {
                progress.Set(1 - count / (float)countInitial);

                bool valid = true;

                int height = WorldGen.genRand.Next((int)(20 * (Main.maxTilesY / 1200f)), (int)(40 * (Main.maxTilesY / 1200f)) + 1);
                if (count < countInitial / 2)
                {
                    height /= 4;
                }

                int width = height * 4;

                int padding = 40;

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(500 + padding, Main.maxTilesX / 2 - 200 - width) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 200, Main.maxTilesX - 500 - width - padding);
                int y = WorldGen.genRand.Next(100, (int)(Main.worldSurface * 0.4f) - height);

                bool rainCloud = count % 3 == 1;

                if (biomes.FindBiome(x + width / 2, (int)Main.worldSurface - 50, false) == BiomeID.Desert)
                {
                    valid = false;
                }
                //else if (MathHelper.Distance(x + width / 2, Main.dungeonX) < width / 2 + 50)
                //{
                //    valid = false;
                //}
                else for (int j = y - padding; j <= y + height + padding; j++)
                    {
                        for (int i = x - padding; i <= x + width + padding; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.HasTile)
                            {
                                valid = false;
                            }
                        }
                    }

                if (valid)
                {
                    float freq = 0.015f - (height / 20f - 1) * 0.002f;

                    FastNoiseLite clouds = new FastNoiseLite();
                    clouds.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                    clouds.SetFrequency(freq);
                    //clouds.SetFrequency(0.01f);
                    clouds.SetFractalType(FastNoiseLite.FractalType.FBm);
                    clouds.SetFractalOctaves(5);
                    clouds.SetFractalLacunarity(1.5f);
                    //caves1.SetFractalGain(0.4f);

                    GenVars.structures.AddStructure(new Rectangle(x, y, width, height), padding);

                    for (int j = y - padding; j <= y + height + padding; j++)
                    {
                        for (int i = x - padding; i <= x + width + padding; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            //float _caveSize = caves2.GetNoise(x, y * 4) + (1.5f / 4) - ModContent.GetInstance<Client>().CloudDensity / 4 + (2 / 4);

                            Vector2 point = new Vector2(MathHelper.Clamp(i, x, x + width), MathHelper.Clamp(j, y, y + height));
                            float _caves = clouds.GetNoise(i, j * 4) + MathHelper.Clamp(Vector2.Distance(new Vector2(i, j), point) / (padding * 6), 0, 1);

                            if (_caves < (rainCloud ? -0.8f : -0.825f))
                            {
                                tile.HasTile = true;
                                tile.TileType = clouds.GetNoise(i * 2, j * 8) < (rainCloud ? -0.7f : -0.85f) ? TileID.RainCloud : TileID.Cloud;
                                if (MiscTools.SurroundingTilesActive(i - 1, j - 1))
                                {
                                    MiscTools.Tile(i - 1, j - 1).WallType = WallID.Cloud;
                                }

                                if (count >= countInitial / 2)
                                {
                                    if (_caves < -0.85f)
                                    {
                                        int var = (int)(0.05 / freq);
                                        tile = Main.tile[i, j - var];
                                        if (tile.HasTile)
                                        {
                                            if (rainCloud)
                                            {
                                                tile.HasTile = false;
                                                tile.LiquidAmount = 255;
                                                tile.LiquidType = 0;
                                            }
                                            //else if (WGTools.SurroundingTilesActive(i, j - var))
                                            //{
                                            //    tile.TileType = biomes.FindBiome(i, j - var) == BiomeID.Jungle ? TileID.Mud : TileID.Dirt;
                                            //    //tile.WallType = WallID.DirtUnsafe;
                                            //}
                                            //else tile.TileType = biomes.FindBiome(i, j - var) == BiomeID.Jungle ? TileID.JungleGrass : TileID.Grass;
                                        }
                                    }
                                }
                            }

                            //_caves = clouds.GetNoise(i + 999, j * 4 + 999) + MathHelper.Clamp(Vector2.Distance(new Vector2(i, j), point) / (padding * 6), 0, 1);

                            //if (_caves < -0.9f)
                            //{
                            //    tile.WallType = WallID.Cloud;
                            //}

                            if (MiscTools.Tile(i - 1, j - 1).WallType == WallID.Cloud && rainCloud)
                            {
                                MiscTools.Tile(i - 1, j - 1).WallColor = PaintID.BlackPaint;
                            }
                        }
                    }

                    for (int j = y - padding; j <= y + height + padding; j++)
                    {
                        for (int i = x - padding; i <= x + width + padding; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            if (IsCloud(i, j))
                            {
                                bool left = IsCloud(i - 1, j) && !IsCloud(i + 1, j);
                                bool right = IsCloud(i + 1, j) && !IsCloud(i - 1, j);
                                bool top = IsCloud(i, j - 1) && !IsCloud(i, j + 1);
                                bool bottom = IsCloud(i, j + 1) && !IsCloud(i, j - 1);

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
                            else if (tile.LiquidAmount != 255)
                            {
                                ClearExcessWater(i, j);
                            }
                        }
                    }

                    FastNoiseLite ore = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                    ore.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                    ore.SetFrequency(0.015f);
                    ore.SetFractalType(FastNoiseLite.FractalType.FBm);
                    ore.SetFractalGain(0.4f);

                    for (int j = y - padding; j <= y + height + padding; j++)
                    {
                        for (int i = x - padding; i <= x + width + padding; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            if (tile.HasTile)
                            {
                                float _ore = ore.GetNoise(i, j * 4);

                                if (tile.TileType == TileID.Cloud || tile.TileType == TileID.RainCloud)
                                {
                                    if (_ore > 0.4f)
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<StarOre>();
                                    }
                                }
                                else if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Mud)
                                {
                                    if (_ore < -0.3f)
                                    {
                                        tile.TileType = TileID.Platinum;
                                    }
                                }
                            }
                        }
                    }
                    count--;
                }
            }
        }

        private bool IsCloud(int i, int j)
        {
            return Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud);
        }

        private void ClearExcessWater(int i, int j)
        {
            if (Main.tile[i - 1, j].LiquidAmount > 0)
            {
                int k = 1;
                while (Main.tile[i - k, j].LiquidAmount > 0)
                {
                    Main.tile[i - k, j].LiquidAmount = 0;

                    if (Main.tile[i - k, j - 1].LiquidAmount > 0)
                    {
                        ClearExcessWater(i - k, j - 1);
                    }

                    k++;
                }
            }
            if (Main.tile[i + 1, j].LiquidAmount > 0)
            {
                int k = 1;
                while (Main.tile[i + k, j].LiquidAmount > 0)
                {
                    Main.tile[i + k, j].LiquidAmount = 0;

                    if (Main.tile[i + k, j - 1].LiquidAmount > 0)
                    {
                        ClearExcessWater(i + k, j - 1);
                    }

                    k++;
                }
            }
        }
    }
}
