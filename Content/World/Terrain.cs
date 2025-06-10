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
using Remnants.Content.Tiles.Objects.Furniture;
using Remnants.Content.Tiles.Plants;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

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
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Remove Water From Sand"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Oasis"));

            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Shinies"));
            RemWorld.RemovePass(tasks, RemWorld.FindIndex(tasks, "Surface Ore and Stone"));

            RemWorld.InsertPass(tasks, new ReturnThinIce("Return Thin Ice", 1), RemWorld.FindIndex(tasks, "Tile Cleanup") + 1);

            if (ModContent.GetInstance<Worldgen>().OreFrequency > 0)
            {
                RemWorld.InsertPass(tasks, new Ores("Minerals", 1), RemWorld.FindIndex(tasks, "Dirt Rock Wall Runner") + 1);
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

            int mountain1Pos = (Tundra.Left + Tundra.Width / 3) * biomes.CellSize;
            int mountain2Pos = (Tundra.Right + 1 - Tundra.Width / 3) * biomes.CellSize;
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

            FastNoiseLite iceSpikes = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            iceSpikes.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            iceSpikes.SetFrequency(0.1f);
            iceSpikes.SetFractalType(FastNoiseLite.FractalType.FBm);
            iceSpikes.SetFractalOctaves(3);

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

            int seaStack1Position = WorldGen.genRand.Next(70, 110);
            int seaStack2Position = WorldGen.genRand.Next(140, 180);
            int seaStack3Position = WorldGen.genRand.Next(210, 250);
            if (Jungle.Center >= biomes.Width / 2)
            {
                seaStack1Position = Main.maxTilesX - seaStack1Position;
                seaStack2Position = Main.maxTilesX - seaStack2Position;
                seaStack3Position = Main.maxTilesX - seaStack3Position;
            }

            FastNoiseLite savannaSpires = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            savannaSpires.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            savannaSpires.SetFrequency(0.005f);
            savannaSpires.SetFractalType(FastNoiseLite.FractalType.FBm);
            savannaSpires.SetFractalOctaves(2);
            savannaSpires.SetCellularJitter(0.5f);

            FastNoiseLite corruptionHills = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            corruptionHills.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            corruptionHills.SetFrequency(0.02f);
            corruptionHills.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite corruptionSpikes = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            corruptionSpikes.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            corruptionSpikes.SetFrequency(0.1f);
            corruptionSpikes.SetFractalType(FastNoiseLite.FractalType.FBm);
            corruptionSpikes.SetFractalOctaves(2);

            FastNoiseLite corruptionPlains = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            corruptionPlains.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            corruptionPlains.SetFrequency(0.025f);
            corruptionPlains.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite corruptionChasms = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            corruptionChasms.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            corruptionChasms.SetFrequency(0.01f);
            corruptionChasms.SetFractalType(FastNoiseLite.FractalType.None);
            corruptionChasms.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
            corruptionChasms.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            corruptionChasms.SetCellularJitter(0.75f);

            FastNoiseLite corruptionRoots = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            corruptionRoots.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            corruptionRoots.SetFrequency(0.1f);
            corruptionRoots.SetFractalType(FastNoiseLite.FractalType.Ridged);
            corruptionRoots.SetFractalOctaves(1);

            bool[,] corruptionDirt = new bool[Main.maxTilesX, Main.maxTilesY];

            FastNoiseLite crimsonHills = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            crimsonHills.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            crimsonHills.SetFrequency(0.01f);
            crimsonHills.SetFractalType(FastNoiseLite.FractalType.FBm);
            crimsonHills.SetFractalOctaves(2);
            //crimsonHills.SetFractalGain(0.4f);

            FastNoiseLite crimsonScabs = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            crimsonScabs.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            crimsonScabs.SetFrequency(0.01f);
            crimsonScabs.SetFractalType(FastNoiseLite.FractalType.FBm);
            crimsonScabs.SetFractalOctaves(3);
            crimsonScabs.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

            FastNoiseLite crimsonHairs = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            crimsonHairs.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            crimsonHairs.SetFrequency(0.1f);
            crimsonHairs.SetFractalType(FastNoiseLite.FractalType.FBm);

            bool[,] crimsonHair = new bool[Main.maxTilesX, Main.maxTilesY];

            for (int y = 40; y <= Main.maxTilesY - 200; y++)
            {
                progress.Set((y - (int)(Main.worldSurface * 0.35f)) / (float)(Main.maxTilesY - 200 - (int)(Main.worldSurface * 0.35f)) / 3);

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    #region terrain
                    float i = x + roughness.GetNoise(x, y + 999) * 10;
                    float j = y + roughness.GetNoise(x + 999, y) * 10;

                    if (y < Main.worldSurface)
                    {
                        float tundraDistance = Math.Min(MathHelper.Distance(i / (float)biomes.CellSize, Tundra.Left), MathHelper.Distance(i / (float)biomes.CellSize, Tundra.Right + 1));
                        float corruptionDistance = Math.Min(MathHelper.Distance(i / (float)biomes.CellSize, Corruption.X - Corruption.Size), MathHelper.Distance(i / (float)biomes.CellSize, Corruption.X + Corruption.Size + 1));
                        float jungleDistance = Math.Min(MathHelper.Distance(i / (float)biomes.CellSize, Jungle.Left), MathHelper.Distance(i / (float)biomes.CellSize, Jungle.Right + 1));
                        float desertDistance = Math.Min(MathHelper.Distance(i / (float)biomes.CellSize, Desert.Left), MathHelper.Distance(i / (float)biomes.CellSize, Desert.Right + 1));
                        float beachDistance = Math.Min(MathHelper.Distance(i / (float)biomes.CellSize, 7), MathHelper.Distance(i / (float)biomes.CellSize, biomes.Width - 7));
                        float forestDistance = Math.Min(beachDistance, Math.Min(Math.Min(tundraDistance, corruptionDistance), Math.Min(desertDistance, jungleDistance)));


                        float seaStackDistance = Math.Min(Math.Min(MathHelper.Distance(i, Jungle.Center < biomes.Width / 2 ? 340 : Main.maxTilesX - 340), MathHelper.Distance(i, seaStack1Position)), Math.Min(MathHelper.Distance(i, seaStack2Position), MathHelper.Distance(i, seaStack3Position)));
                        bool seaStacks = (seaStackDistance < 10 && (j < (Maximum * 7 + Middle) / 8 || beachDistance < 1));
                        float beachFactor = seaStacks ? 0 : 1;
                        if (biomes.FindBiome(i, j, false) != BiomeID.Beach && biomes.FindBiome(i, j, false) != BiomeID.OceanCave)
                        {
                            float mult = 1;
                            if (Jungle.Center >= biomes.Width / 2)
                            {
                                mult *= MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, 0, 325)) / (150 * scaleX), 0, 1);
                            }
                            else mult *= MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, Main.maxTilesX - 325, Main.maxTilesX)) / (150 * scaleX), 0, 1);
                            beachFactor = 1 - (float)MiscTools.LessSmoothStep(0, 1, mult);
                        }

                        j += 5 * beachFactor;


                        int waterLevel = (biomes.FindBiome(x, y, false) == BiomeID.Jungle ? ((Middle + Maximum * 5) / 6) : (int)Main.worldSurface - 60);
                        waterLevel = (int)((waterLevel - ((int)Main.worldSurface - 60)) * (1 - beachFactor) + ((int)Main.worldSurface - 60));


                        float _terrain = 0.75f;

                        _terrain = ((_terrain - 1) * (1 - beachFactor)) + 1;

                        if (biomes.FindBiome(x, y, false) != BiomeID.Jungle && biomes.FindBiome(x, y, false) != BiomeID.Desert && biomes.FindBiome(x, y, false) != BiomeID.Savanna)
                        {
                            _terrain -= 0.2f * (float)MiscTools.LessSmoothStep(0, 1, MathHelper.Clamp(Math.Min(Math.Min(desertDistance, jungleDistance), beachDistance - (Main.maxTilesX / 700f)) / (Main.maxTilesX / 1050f), 0, 1));
                            //_terrain -= 0.25f * (float)SmootherStep(0, 1, MathHelper.Clamp(Math.Min(MathHelper.Distance(x, Main.maxTilesX * 0.4f), MathHelper.Distance(x, Main.maxTilesX * 0.6f)) / (Main.maxTilesX / 21), 0, 1));
                        }

                        bool carve = false;
                        if (biomes.FindBiome(x, y, false) == BiomeID.None)
                        {
                            _terrain += forestHills.GetNoise(i, 0) / 4 * MathHelper.Clamp(forestDistance / (Main.maxTilesX / 2100f), 0, 1) * MathHelper.Clamp(MathHelper.Distance(i / biomes.CellSize, biomes.Width / 2) / (Main.maxTilesX / 2100f), 0, 1);
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Tundra)
                        {
                            _terrain += 0.2f * (float)MiscTools.LessSmoothStep(0, 1, MathHelper.Clamp(tundraDistance / (Main.maxTilesX / 2500f), 0, 1));
                            _terrain -= 0.8f * ((float)-Math.Cos(MathHelper.Pi * (1 - Math.Clamp(MathHelper.Distance(i, mountain1Pos) / (Main.maxTilesX / 25f), 0, 1)) / 2) + 1) * mountain1Height;
                            _terrain -= 0.8f * ((float)-Math.Cos(MathHelper.Pi * (1 - Math.Clamp(MathHelper.Distance(i, mountain2Pos) / (Main.maxTilesX / 25f), 0, 1)) / 2) + 1) * mountain2Height;
                            _terrain += mountainJags.GetNoise(i, 0) / 4 * (1 - MathHelper.Clamp(Math.Min(MathHelper.Distance(i, mountain1Pos), MathHelper.Distance(i, mountain2Pos)) / (Main.maxTilesX / 12.5f), 0, 1)) * MathHelper.Clamp(tundraDistance / (Main.maxTilesX / 4200f), 0, 1);
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Jungle)
                        {
                            if (jungleDistance > (Main.maxTilesX / 8400f) && MathHelper.Distance(i / biomes.CellSize, Jungle.Center + 0.5f) >= (Main.maxTilesX / 8400f) && MathHelper.Distance(i / biomes.CellSize, (Jungle.Left + Jungle.Center + 0.5f) / 2f) >= (Main.maxTilesX / 8400f) && MathHelper.Distance(i / biomes.CellSize, (Jungle.Right + 1 + Jungle.Center + 0.5f) / 2f) >= (Main.maxTilesX / 8400f))
                            {
                                _terrain += MathHelper.Clamp(jungleCrags.GetNoise(i, 0), -0.7f, (i + junglePools.GetNoise(i, 0) * 25) % 20 < 8 && jungleCrags.GetNoise(i, 0) > 0.4f ? 0.3f : 0.5f) / 2;
                            }

                            if (jungleDistance > (Main.maxTilesX / 8400f))
                            {
                                j += (int)(roughness.GetNoise(x / 2f, y) * 40 * MathHelper.Clamp(jungleDistance - 1, 0, 1));

                                int tunnel1Height = (Middle + Maximum) / 2;
                                int tunnel2Height = (Middle + Maximum) / 2 - (waterLevel - (Middle + Maximum) / 2) + 5;
                                int tunnel3Height = waterLevel - 5;

                                if (j >= tunnel1Height - 11 && j < tunnel1Height || j >= tunnel2Height - 11 && j < tunnel2Height || j >= tunnel3Height - 11 && j < tunnel3Height && jungleDistance > (Main.maxTilesX / 8400f) * 1.5f)// && !junglePool)
                                {
                                    carve = true;

                                    if (x == (Jungle.Center + 0.5f) * biomes.CellSize)
                                    {
                                        if (j >= tunnel1Height - 11 && j < tunnel1Height && WorldGen.SolidTile(x, y - 1))
                                        {
                                            Mineshafts.JungleEntranceY = y + 3;
                                        }
                                    }
                                }
                            }
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Corruption)
                        {
                            float _corruptionHills = corruptionHills.GetNoise(i, 0);
                            float _corruptionPlains = corruptionPlains.GetNoise(x, 0) * 40;
                            float _corruptionChasms = corruptionChasms.GetNoise((i - (Corruption.X + 0.5f) * biomes.CellSize) * 2, j - Middle);

                            if (corruptionDistance > 1 && (_corruptionChasms > -0.15f - MathHelper.Clamp((j - (Middle + Maximum * 3) / 4) / ((Middle + Maximum * 7) / 8 - (Middle + Maximum * 3) / 4), 0, 1) * 0.65f))
                            {
                                if (j < Maximum - 1)
                                {
                                    carve = true;

                                    int centre = (Middle + Maximum * 15) / 16;
                                    if (corruptionDistance > 1.2f && (_corruptionChasms + 1) > 0.6f + MathHelper.Clamp(MathHelper.Distance(j, centre) / ((Maximum - Middle) / 8), 0, 1) * 0.4f)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                                    }
                                }
                            }

                            if ((i + _corruptionPlains) % 80 <= 30)
                            {
                                _corruptionHills = corruptionHills.GetNoise((int)((i + _corruptionPlains) / 80) * 80 + 16, 0);
                                corruptionDirt[x, y] = true;
                            }             
                            else _terrain += corruptionSpikes.GetNoise(i, 0) / 6 * MathHelper.Clamp(corruptionDistance / (Main.maxTilesX / 8400f), 0, 1);

                            _corruptionHills *= MathHelper.Clamp(corruptionDistance / (Main.maxTilesX / 2100f), 0, 1);

                            _terrain += _corruptionHills;
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Crimson)
                        {
                            float _crimsonHills = crimsonHills.GetNoise(i, 0) / 2 * MathHelper.Clamp(corruptionDistance / (Main.maxTilesX / 4200f), 0, 1);

                            _terrain += _crimsonHills;

                            float _crimsonScabs = crimsonScabs.GetNoise(i, j * 4) / 4 * MathHelper.Clamp(corruptionDistance / (Main.maxTilesX / 4200f), 0, 1);

                            _terrain += _crimsonScabs;
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Beach)
                        {
                            if (beachDistance > 1 && (i > Main.maxTilesX / 2 ^ Jungle.Center < biomes.Width / 2) && j < Maximum - 30)
                            {
                                j += (int)(roughness.GetNoise(x / 2f, y) * 40);

                                if (j >= (Middle + Maximum * 3) / 4 - 11 && j < (Middle + Maximum * 3) / 4)
                                {
                                    carve = true;
                                }
                            }
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Savanna)
                        {
                            float _savannaSpires = (-savannaSpires.GetNoise(i, 0) - 0.9f) * MathHelper.Clamp(forestDistance / (Main.maxTilesX / 4200f), 0, 1);
                            _terrain += _savannaSpires;
                        }


                        float threshold = _terrain * (Maximum - Minimum) + Minimum;


                        if (!seaStacks)
                        {
                            float oceanMultiplier = (float)MiscTools.LessSmoothStep(0, 1, MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, 0, 150)) / 150, 0.15f, 1) * MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, Main.maxTilesX - 150, Main.maxTilesX)) / 150, 0.15f, 1));
                            threshold = (threshold - (int)Main.worldSurface) * oceanMultiplier + (int)Main.worldSurface;
                        }


                        if (beachFactor == 0 && biomes.FindBiome(x, y, false) != BiomeID.Tundra)
                        {
                            threshold = MathHelper.Clamp(threshold, Minimum, (int)Main.worldSurface - 30);
                        }

                        if (biomes.FindBiome(x, y, false) == BiomeID.None || biomes.FindBiome(x, y, false) == BiomeID.Savanna)
                        {
                            j = j * (1 - Math.Clamp(forestDistance, 0, 1)) + (float)MiscTools.LessSmoothStep((float)Math.Floor((float)j / 24) * 24, (float)Math.Ceiling((float)j / 24) * 24, (j % 24) / 24) * Math.Clamp(forestDistance, 0, 1);
                        }

                        if (j >= threshold && !carve)
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
                            //if (j < threshold)
                            //{
                            //    corruptionDirt[x, y] = false;
                            //}
                        }

                        if (biomes.FindBiome(x, y, false) == BiomeID.Tundra)
                        {
                            if (j <= threshold && j > threshold + (iceSpikes.GetNoise((int)i, 0) + 0.05f) * 100 * MathHelper.Clamp(tundraDistance / (Main.maxTilesX / 8400f), 0, 1))
                            {
                                tile.WallType = WallID.RocksUnsafe1;
                            }
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Jungle)
                        {
                            if (!MiscTools.SurroundingTilesActive(x, y) && jungleDistance > 0.25f)
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
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Corruption)
                        {
                            threshold += corruptionSpikes.GetNoise(i + 999, 0) * 100 * MathHelper.Clamp(corruptionDistance / (Main.maxTilesX / 8400f), 0, 1);
                            if (j > threshold && !carve)
                            {
                                tile.WallType = WallID.RocksUnsafe1;
                            }
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Crimson)
                        {
                            threshold = Terrain.Middle;
                            if (j > threshold + (crimsonHairs.GetNoise((int)i, 0) + 0.9f) * 1000 * MathHelper.Clamp((corruptionDistance + 0.5f) / (Main.maxTilesX / 8400f), 0, 1))
                            {
                                crimsonHair[x, y] = true;
                            }
                        }
                        else if (biomes.FindBiome(x, y, false) == BiomeID.Beach && (i > Main.maxTilesX / 2 ^ Jungle.Center < biomes.Width / 2))
                        {
                            threshold = (Middle + Maximum * 2) / 3 + (jungleCrags.GetNoise(i * 6, 0) / 2 + 0.5f) * ((int)Main.worldSurface - (Middle + Maximum * 2) / 3);
                            if (j > threshold)
                            {
                                tile.WallType = WallID.RocksUnsafe1;
                            }
                        }
                    }
                    else tile.HasTile = true;
                    #endregion

                    #region materials
                    int layer = biomes.GetLayer(x, y);

                    if (layer >= biomes.caveLayer && layer < biomes.lavaLayer && biomes.GetTileDistribution(x, y, true) <= -0.35f)
                    {
                        tile.TileType = TileID.Silt;
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Corruption || biomes.FindBiome(x, y) == BiomeID.Beach && j < Maximum || biomes.GetTileDistribution(x, y, frequency: 2) >= (layer >= biomes.surfaceLayer && layer < biomes.caveLayer ? 0.2f : (biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Crimson ? -0.1f : -0.2f)))
                    {
                        tile.TileType = TileID.Stone;
                    }
                    else if (layer >= biomes.surfaceLayer && layer < biomes.caveLayer && biomes.GetTileDistribution(x, y, true) >= 0.25f)
                    {
                        tile.TileType = TileID.ClayBlock;
                    }
                    else tile.TileType = TileID.Dirt;
                    #endregion
                }
            }
            #endregion

            #region features
            int attempts = 0;
            int count = 0;
            while (count < Main.maxTilesX / 840)
            {
                int x = count == 0 ? (int)((Desert.Center + 0.5f) * biomes.CellSize) + WorldGen.genRand.Next(-40, 41) : WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - 75) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 75, Main.maxTilesX - 400);
                int y = Minimum;
                float radius = count == 0 ? 20 : WorldGen.genRand.NextFloat(10, 20);

                while (!MiscTools.Tile(x, y).HasTile)
                {
                    y++;
                }

                bool valid = true;

                if (count != 0 && (biomes.FindBiome(x, y) != BiomeID.None || Math.Min(MathHelper.Distance(x / biomes.CellSize, Corruption.X - Corruption.Size), MathHelper.Distance(x / biomes.CellSize, Corruption.X + Corruption.Size + 1)) < 2 || MathHelper.Distance(x, Main.maxTilesX * (GenVars.dungeonSide == 1 ? 0.9f : 0.1f)) < 100))
                {
                    valid = false;
                }
                else
                {
                    if (count != 0)
                    {
                        for (int j = y - (int)radius / 2; j <= y + (int)radius / 2; j++)
                        {
                            for (int i = x - (int)radius * 2; i <= x + (int)radius * 2; i++)
                            {
                                if (MiscTools.Tile(i, j).LiquidAmount == 255)
                                {
                                    valid = false;
                                }
                            }
                        }
                    }

                    if (valid)
                    {
                        for (int j = y - (int)radius / 2 - 5; j <= y + (int)radius / 2 + 5; j++)
                        {
                            for (int i = x - (int)radius - 5; i <= x + (int)radius + 5; i++)
                            {
                                if (j < y - (count == 0 ? (2 + attempts / 1000) : 2))
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

                    if (count == 0)
                    {
                        Desert.OasisX = x;
                    }

                    count++;
                }
                else attempts++;
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

                if (biomes.FindBiome(x, y, false) != BiomeID.None || biomes.FindBiome(x - 50, y, false) != BiomeID.None || biomes.FindBiome(x + 50, y, false) != BiomeID.None)
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
            while (count < Main.maxTilesX / 1050)
            {
                int x = WorldGen.genRand.Next((Desert.Left + 1) * biomes.CellSize, Desert.Right * biomes.CellSize);
                int y = Minimum;

                while (!MiscTools.Tile(x, y).HasTile)
                {
                    y++;
                }

                bool valid = true;

                if (MathHelper.Distance(x, Desert.OasisX) < 50)
                {
                    valid = false;
                }

                int height = WorldGen.genRand.Next(5, 11);
                y -= height;

                if (valid)
                {
                    for (int k = 0; k < height + 20; k++)
                    {
                        for (int i = x - k * 2; i <= x + 1 + k * 2; i++)
                        {
                            if (biomes.FindBiome(i, y + k) == BiomeID.Desert)
                            {
                                WorldGen.PlaceTile(i, y + k, TileID.Dirt);
                            }
                        }
                    }

                    count++;
                }
            }

            count = 0;
            while (count < 1)
            {
                float radius = WorldGen.genRand.NextFloat(20, 30);

                int x = Jungle.Center >= biomes.Width / 2 ? WorldGen.genRand.Next(120 + (int)radius, 220 - (int)radius) : Main.maxTilesX - WorldGen.genRand.Next(120 + (int)radius, 220 - (int)radius);
                int y = Maximum + 1;

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
                        float flatness = j > y ? 2 : 8;

                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j * flatness), new Vector2(x, y * flatness)) < radius + roughness.GetNoise(i * 2, 0) * (40 / flatness) && WorldGen.InWorld(i, j))
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

            #region cleanup
            for (int y = Minimum; y <= Main.worldSurface; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (tile.HasTile)
                    {
                        if (biomes.FindBiome(x, y, false) == BiomeID.Jungle)
                        {
                            if (!MiscTools.Tile(x, y - 1).HasTile && MiscTools.Tile(x, y - 1).LiquidAmount < 85)
                            {
                                MiscTools.Tile(x, y - 1).LiquidAmount = 85;
                            }
                        }

                        if (biomes.FindBiome(x, y) != BiomeID.Beach && (tile.TileType == TileID.Stone || tile.TileType == TileID.ClayBlock) && (biomes.FindBiome(x, y, false) != BiomeID.Corruption || corruptionDirt[x, y]))
                        {
                            AddSurfaceDirt(x, y);
                        }
                    }
                    else if (!crimsonHair[x, y])
                    {
                        if (crimsonHair[x, y - 1] && !crimsonHair[x - 1, y - 1] && !crimsonHair[x + 1, y - 1] && !crimsonHair[x - 1, y] && !crimsonHair[x + 1, y])
                        {
                            crimsonHair[x, y] = true;
                        }
                    }
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

                        if (tile.HasTile && MiscTools.AdjacentTiles(x, y) <= 1 && (biomes.FindBiome(x, y, false) != BiomeID.Corruption || tile.TileType == TileID.Dirt))
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

                        if (tile.HasTile && MiscTools.AdjacentTiles(x, y) <= 2 && (MiscTools.Tile(x, y - 1).HasTile || MiscTools.Tile(x, y + 1).HasTile) && (biomes.FindBiome(x, y, false) != BiomeID.Corruption || tile.TileType == TileID.Dirt))
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

            #region walls
            for (int y = 40; y <= Main.maxTilesY - 200; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (y > Main.worldSurface || tile.WallType == WallID.DirtUnsafe || MiscTools.SurroundingTilesActive(x, y) || (tile.WallType == 0 && MiscTools.Tile(x, y - 1).WallType != 0 && (biomes.FindBiome(x, y) != BiomeID.Crimson || (MiscTools.Tile(x - 1, y - 1).WallType != 0 || MiscTools.Tile(x - 1, y - 2).WallType != 0) && (MiscTools.Tile(x + 1, y - 1).WallType != 0 || MiscTools.Tile(x + 1, y - 2).WallType != 0))))
                    {
                        int layer = biomes.GetLayer(x, y);

                        if (tile.TileType == TileID.Stone || biomes.FindBiome(x, y, false) == BiomeID.Beach && !MiscTools.SurroundingTilesActive(x, y))
                        {
                            tile.WallType = layer >= biomes.lavaLayer ? WallID.Cave8Unsafe : WallID.RocksUnsafe1;
                        }
                        else tile.WallType = layer >= biomes.lavaLayer ? WallID.CaveWall2 : layer >= biomes.surfaceLayer ? WallID.Cave6Unsafe : WallID.DirtUnsafe;
                    }


                    if (biomes.FindBiome(x, y, false) == BiomeID.Corruption)
                    {
                        if (tile.WallType == WallID.RocksUnsafe1 && corruptionDirt[x, y] && !MiscTools.SurroundingTilesActive(x, y) && corruptionRoots.GetNoise(x, y) > 0.2f)
                        {
                            tile.WallType = WallID.CorruptionUnsafe4;
                            tile.WallColor = PaintID.BluePaint;
                        }
                    }
                    else if (biomes.FindBiome(x, y, false) == BiomeID.Beach)
                    {
                        if (tile.WallType == WallID.RocksUnsafe1 && !MiscTools.SurroundingTilesActive(x, y) && corruptionRoots.GetNoise(x, y) > 0.2f)
                        {
                            tile.WallType = WallID.CaveUnsafe;
                        }
                    }
                }
            }

            for (int k = 0; k < 6; k++)
            {
                progress.Set(0.65f + (k / 6f) * 0.25f);

                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = 40; x <= Main.maxTilesX - 40; x++)
                    {
                        if (!MiscTools.SurroundingTilesActive(x, y) && (biomes.FindBiome(x, y, false) != BiomeID.Tundra && biomes.FindBiome(x, y, false) != BiomeID.Corruption || k >= 2) && MiscTools.Tile(x, y).WallType != WallID.CrimsonUnsafe2 && MiscTools.Tile(x, y).WallType != WallID.CrimsonUnsafe4)
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

                            if (k >= 3)
                            {
                                if (adjacentWalls < 6 - k)
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

            for (int y = 40; y <= Main.worldSurface; y++)
            {
                for (int x = Corruption.Left * biomes.CellSize; x < (Corruption.Right + 1) * biomes.CellSize; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (crimsonHair[x, y] && tile.WallType == 0)
                    {
                        tile.WallType = WorldGen.genRand.NextBool(3) ? WallID.CrimsonUnsafe4 : WallID.CrimsonUnsafe2;
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
            while (count < Main.maxTilesX / 84)
            {
                progress.Set(0.9f + count / (Main.maxTilesX / 84f) * 0.05f);

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - 75) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 75, Main.maxTilesX - 400);
                int y = Minimum;
                float radius = WorldGen.genRand.NextFloat(3, 4) - (count < Main.maxTilesX / 168 ? 0.5f : 0);

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
                    MiscTools.CustomTileRunner(x, y, radius, boulders, count >= Main.maxTilesX / 168 ? TileID.Stone : -2, count < Main.maxTilesX / 168 ? WallID.RocksUnsafe1 : -2);

                    count++;
                }
            }

            count = 0;
            while (count < Main.maxTilesX / 420)
            {
                progress.Set(0.95f + count / (Main.maxTilesX / 420f) * 0.05f);

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(60, 340) : Main.maxTilesX - WorldGen.genRand.Next(60, 340);
                int y = WorldGen.genRand.Next(Middle, Maximum);
                float radius = WorldGen.genRand.NextFloat(3, 4) - (count < Main.maxTilesX / 84 ? 0.5f : 0);

                while (!MiscTools.Tile(x, y).HasTile || y < Maximum - 10)
                {
                    y++;
                }

                bool valid = true;

                if (WorldGen.SolidTile(x, y - 1) || biomes.FindBiome(x, y) != BiomeID.Beach || (MiscTools.Tile(x - radius, y - 1).WallType == 0 || MiscTools.Tile(x + radius, y - 1).WallType == 0 || MiscTools.Tile(x, y - 1).LiquidAmount == 255) && !WorldGen.genRand.NextBool(100))
                {
                    valid = false;
                }
                else
                {
                    for (int j = y - (int)radius - 4; j <= y + (int)radius + 4; j++)
                    {
                        for (int i = x - (int)radius - 3; i <= x + (int)radius + 3; i++)
                        {
                            if (j > y + 3)
                            {
                                if (!MiscTools.Tile(i, j).HasTile)
                                {
                                    valid = false;
                                }
                            }
                            else if (j >= y)
                            {
                                if (MiscTools.Tile(i, j).HasTile && MiscTools.Tile(i, j).TileType == TileID.ShellPile)
                                {
                                    valid = false;
                                }
                            }
                        }
                    }
                }

                if (valid)
                {
                    MiscTools.CustomTileRunner(x, y, radius, boulders, TileID.ShellPile);

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

        public static void AddSurfaceDirt(int x, int y)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            Tile tile = Main.tile[x, y];

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

                if (tile.TileType != TileID.Dirt&& biomes.FindBiome(x, y, false) != BiomeID.Tundra)
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
            caveRoughness.SetFrequency(0.05f / scale);
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
                progress.Set((y - ((int)Main.worldSurface - 40)) / (float)(Main.maxTilesY - 200 - ((int)Main.worldSurface - 40)));

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
                    if (MiscTools.Tile(x, y).HasTile && biomes.GetLayer((int)x, (int)y) >= biomes.surfaceLayer && (biomes.FindBiome(x, y) == BiomeID.None || biomes.FindBiome(x, y) == BiomeID.Desert || biomes.FindBiome(x, y) == BiomeID.Glowshroom))
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

                        if (biomes.GetLayer(x, y) < biomes.caveLayer)
                        {
                            _caves = caves.GetNoise(x * mult * 4, y * mult * 4 * heightMultiplier);

                            if (_caves + _roughness / 5 > 0.15f)
                            {
                                MiscTools.Tile(x, y).HasTile = false;

                                if (_caves + _roughness / 5 < 0.175f)
                                {
                                    MiscTools.Tile(x, y).LiquidAmount = 255;
                                }

                                if (biomes.GetLayer(x, y) > biomes.surfaceLayer && _caves + _roughness / 5 > 0.25f)
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

                            if (biomes.GetLayer(x, y) > biomes.surfaceLayer && biomes.GetLayer(x, y) < biomes.Height - 6)
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

            int structureCount = Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 60000;
            while (structureCount > 0)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);

                if (!MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount != 255 && biomes.FindBiome(x, y) != BiomeID.Tundra && (biomes.FindBiome(x, y) != BiomeID.Jungle || y < GenVars.lavaLine - 50))
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

                    if (tile.HasTile && blocksToReplace.Contains(tile.TileType) && biomes.FindBiome(x, y) != BiomeID.SunkenSea)
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
                                else if (!(biomes.FindBiome(x, y) == BiomeID.Corruption || biomes.FindBiome(x, y) == BiomeID.Crimson))
                                {
                                    #region tin/copper
                                    if (y < GenVars.lavaLine)
                                    {
                                        rarity = y > Main.worldSurface && y < Main.rockLayer ? 20 : 40;

                                        type = biomes.FindBiome(x, y, false) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert || biomes.FindBiome(x, y, false) == BiomeID.Savanna ? TileID.Tin : TileID.Copper;

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

                                    #region gold/platinum
                                    if (y > GenVars.lavaLine)
                                    {
                                        rarity = 40;

                                        type = biomes.FindBiome(x, y) == BiomeID.Jungle ? TileID.Platinum : TileID.Gold;

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

                                type = TileID.Gold;

                                OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                            }
                            #endregion
                        }
                    }
                }
            }
        }

        public static void OreVein(int structureX, int structureY, int size, float rarity, int type, int[] blocksToReplace, int steps, float weight = 0.5f, int birthLimit = 4, int deathLimit = 4)
        {
            if (ModContent.GetInstance<Worldgen>().OreFrequency == 0) { return; }

            bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.DesertFossil, TileID.FleshBlock);

            rarity /= ModContent.GetInstance<Worldgen>().OreFrequency;
            if ((rarity == 0 || WorldGen.genRand.NextBool((int)(rarity * 100)) && (structureY < Main.worldSurface * 0.35f || GenVars.structures.CanPlace(new Rectangle(structureX, structureY, 1, 1), validTiles, 15))) && !Main.wallDungeon[MiscTools.Tile(structureX, structureY).WallType])
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
            if (WorldGen.genRand.NextBool((int)(rarity * 100)) && GenVars.structures.CanPlace(new Rectangle(x, y, 1, 1), 25))
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

    public class Grass : GenPass
    {
        public Grass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Grass");

            for (int y = 40; y < Main.worldSurface; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    progress.Set((y - 40) / (float)(Main.worldSurface - 40));

                    if (biomes.FindBiome(x, y) != BiomeID.Beach && biomes.FindBiome(x, y) != BiomeID.OceanCave && biomes.GetLayer(x, y) < biomes.surfaceLayer)
                    {
                        if (!MiscTools.SurroundingTilesActive(x, y, true))
                        {
                            Tile tile = Main.tile[x, y];

                            if (tile.TileType == TileID.Dirt || tile.TileType == TileID.ClayBlock)
                            {
                                tile.TileType = y > Main.worldSurface / 2 ? biomes.FindBiome(x, y) == BiomeID.Corruption ? TileID.CorruptGrass : biomes.FindBiome(x, y) == BiomeID.Crimson ? TileID.CrimsonGrass : TileID.Grass : TileID.Grass;
                            }
                            else if (tile.TileType == TileID.Mud)
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Jungle)
                                {
                                    tile.TileType = TileID.JungleGrass;
                                }
                            }
                            if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe)
                            {
                                tile.WallType = y > Main.worldSurface / 2 ? biomes.FindBiome(x, y) == BiomeID.Corruption ? WallID.CorruptGrassUnsafe : biomes.FindBiome(x, y) == BiomeID.Crimson ? WallID.CrimsonGrassUnsafe : WallID.GrassUnsafe : WallID.GrassUnsafe;
                            }
                        }
                    }
                }
            }

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Moss");

            int[] mossToChoose = new int[5];
            mossToChoose[0] = TileID.GreenMoss;
            mossToChoose[1] = TileID.BrownMoss;
            mossToChoose[2] = TileID.RedMoss;
            mossToChoose[3] = TileID.BlueMoss;
            mossToChoose[4] = TileID.PurpleMoss;

            int[] moss = new int[3];
            int[] mossWall = new int[3];
            int[] mossBrick = new int[3];
            int index = 0;
            while (index < 3)
            {
                int randomMoss = mossToChoose[WorldGen.genRand.Next(mossToChoose.Length)];
                if (!moss.Contains(randomMoss))
                {
                    moss[index] = randomMoss;
                    mossWall[index] = randomMoss == TileID.GreenMoss ? WallID.CaveUnsafe : randomMoss == TileID.BrownMoss ? WallID.Cave2Unsafe : randomMoss == TileID.RedMoss ? WallID.Cave3Unsafe : randomMoss == TileID.BlueMoss ? WallID.Cave4Unsafe : WallID.Cave5Unsafe;
                    mossBrick[index] = randomMoss == TileID.GreenMoss ? TileID.GreenMossBrick : randomMoss == TileID.BrownMoss ? TileID.BrownMossBrick : randomMoss == TileID.RedMoss ? TileID.RedMossBrick : randomMoss == TileID.BlueMoss ? TileID.BlueMossBrick : TileID.PurpleMossBrick;
                    index++;
                }
            }

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetFrequency(0.1f);

            int[] invalidWalls = new int[] { WallID.DiamondUnsafe, WallID.RubyUnsafe, WallID.EmeraldUnsafe, WallID.SapphireUnsafe, WallID.TopazUnsafe, WallID.AmethystUnsafe };

            int structureCount = 0;
            while (structureCount < Main.maxTilesX * (Main.maxTilesY - 200 - GenVars.lavaLine) / 100000)
            {
                progress.Set(structureCount / (float)(Main.maxTilesX * (Main.maxTilesY - 200 - GenVars.lavaLine) / 100000) / 2);

                int x = WorldGen.genRand.Next(40, Main.maxTilesX - 200);
                int y = WorldGen.genRand.Next(Math.Min(GenVars.lavaLine, Main.maxTilesY - 201), Main.maxTilesY - 200);

                if (biomes.FindBiome(x, y) == BiomeID.None && !MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount == 255 && MiscTools.Tile(x, y).LiquidType == 1)
                {
                    int radius = WorldGen.genRand.Next(30, 90);

                    for (int j = Math.Max(y - radius, GenVars.lavaLine); j <= y + radius; j++)
                    {
                        for (int i = Math.Max(x - radius, 40); i <= Math.Min(x + radius, Main.maxTilesX - 40); i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + (noise.GetNoise(i, j) + 1) * 5 < radius && WorldGen.InWorld(i, j) && biomes.FindBiome(i, j) == BiomeID.None && !MiscTools.SurroundingTilesActive(i, j, true))
                            {
                                Tile tile = Main.tile[i, j];

                                if (tile.HasTile && (tile.TileType == TileID.Stone || tile.TileType == TileID.GrayBrick) && !invalidWalls.Contains(tile.WallType))
                                {
                                    tile.TileType = tile.TileType == TileID.GrayBrick ? TileID.LavaMossBrick : TileID.LavaMoss;
                                }

                                if (tile.WallType == WallID.Cave8Unsafe)
                                {
                                    tile.WallType = WallID.LavaUnsafe2;
                                }
                            }
                        }
                    }

                    structureCount++;
                }
            }

            structureCount = 0;
            while (structureCount < Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 40000)
            {
                progress.Set(structureCount / (float)(Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 40000) / 2 + 0.5f);

                int x = WorldGen.genRand.Next(40, Main.maxTilesX - 40);
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);

                if (biomes.FindBiome(x, y, false) == BiomeID.None && !MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount == 255 && MiscTools.Tile(x, y).LiquidType == 0)
                {
                    int radius = WorldGen.genRand.Next(25, 75);

                    for (int j = Math.Max(y - radius, (int)Main.rockLayer); j <= y + radius; j++)
                    {
                        for (int i = Math.Max(x - radius, 40); i <= Math.Min(x + radius, Main.maxTilesX - 40); i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + (noise.GetNoise(i, j) + 1) * 5 < radius && WorldGen.InWorld(i, j) && biomes.FindBiome(i, j) == BiomeID.None && !MiscTools.SurroundingTilesActive(i, j, true))
                            {
                                Tile tile = Main.tile[i, j];
                                int mossType = x + (noise.GetNoise(i, j) + 1) * 10 <= Main.maxTilesX / 3 + Math.Cos(j / (Main.maxTilesY / 120f)) * (Main.maxTilesX / 200f) ? 0 : x <= Main.maxTilesX / 1.5f + Math.Cos(j / (Main.maxTilesY / 120f)) * (Main.maxTilesX / 200f) ? 1 : 2;

                                if (tile.HasTile && (tile.TileType == TileID.Stone || tile.TileType == TileID.GrayBrick) && !invalidWalls.Contains(tile.WallType))
                                {
                                    tile.TileType = tile.TileType == TileID.GrayBrick ? (ushort)mossBrick[mossType] : (ushort)moss[mossType];
                                }
                                if (tile.WallType == WallID.RocksUnsafe1 || tile.WallType == WallID.Cave8Unsafe)
                                {
                                    tile.WallType = (ushort)mossWall[mossType];
                                }
                            }
                        }
                    }

                    structureCount++;
                }
            }
        }
    }

    public class Piles : GenPass
    {
        public Piles(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Piles");

            //Main.tileSolid[229] = false;
            //Main.tileSolid[190] = false;
            //Main.tileSolid[196] = false;
            //Main.tileSolid[189] = false;
            //Main.tileSolid[202] = false;
            //Main.tileSolid[460] = false;

            Main.tileSolid[190] = true;
            Main.tileSolid[192] = true;
            Main.tileSolid[196] = true;
            Main.tileSolid[189] = true;
            Main.tileSolid[202] = true;
            Main.tileSolid[225] = true;
            Main.tileSolid[460] = true;

            int area = GenVars.UndergroundDesertLocation.Width * GenVars.UndergroundDesertLocation.Height;
            int objects = area / 800;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.UndergroundDesertLocation.Bottom + 1);

                if (biomes.FindBiome(x, y) == BiomeID.Desert && MiscTools.NonSolidInArea(x, y - 1, x + 1, y) && MiscTools.HasTile(x, y + 1, TileID.Sandstone) && MiscTools.HasTile(x + 1, y + 1, TileID.Sandstone))
                {
                    WorldGen.PlaceObject(x, y, TileID.AntlionLarva, style: Main.rand.Next(3));
                    if (Framing.GetTileSafely(x, y).TileType == TileID.AntlionLarva)
                    {
                        objects--;
                    }
                }
            }

            #region sunkensea
            if (ModContent.GetInstance<Worldgen>().SunkenSeaRework && ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                area = GenVars.UndergroundDesertLocation.Width * (int)((GenVars.lavaLine - Main.rockLayer) / 2);

                ushort eutrophicSand = 0;
                ushort seaPrism = 0;
                if (cal.TryFind("EutrophicSand", out ModTile sand))
                {
                    eutrophicSand = sand.Type;
                }
                if (cal.TryFind("SeaPrism", out ModTile prism))
                {
                    seaPrism = prism.Type;
                }

                if (cal.TryFind("FanCoral", out ModTile fanCoral) && cal.TryFind("BrainCoral", out ModTile brainCoralLarge) && cal.TryFind("CoralPileLarge", out ModTile coralPileLarge) && cal.TryFind("SmallBrainCoral", out ModTile brainCoralSmall) && cal.TryFind("MediumCoral", out ModTile mediumCoral) && cal.TryFind("MediumCoral2", out ModTile mediumCoral2) && cal.TryFind("SmallTubeCoral", out ModTile smallTubeCoral) && cal.TryFind("SeaAnemone", out ModTile seaAnemone) && cal.TryFind("SmallCorals", out ModTile smallCoral) && cal.TryFind("TableCoral", out ModTile tableCoral))
                {
                    objects = area / 1600;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                        int y = WorldGen.genRand.Next((int)Main.rockLayer, GenVars.UndergroundDesertLocation.Bottom + 1);

                        if (biomes.FindBiome(x, y) == BiomeID.SunkenSea)
                        {
                            bool valid = true;
                            for (int j = y - 1; j <= y; j++)
                            {
                                for (int i = x - 1; i <= x + 1; i++)
                                {
                                    if (Main.tile[i, j].HasTile)
                                    {
                                        valid = false;
                                    }
                                }
                            }

                            if (MiscTools.SolidTileOf(x - 2, y, seaPrism) || MiscTools.SolidTileOf(x + 2, y, seaPrism) || WorldGen.SolidTile(x, y - 2) || WorldGen.SolidTile(x, y - 3))
                            {
                                valid = false;
                            }

                            if (valid)
                            {
                                WorldGen.PlaceTile(x, y, tableCoral.Type);
                                if (Framing.GetTileSafely(x, y).TileType == tableCoral.Type)
                                {
                                    objects--;
                                }
                            }
                        }
                    }

                    objects = area / 3200;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                        int y = WorldGen.genRand.Next((int)Main.rockLayer, GenVars.UndergroundDesertLocation.Bottom + 1);

                        if (biomes.FindBiome(x, y) == BiomeID.SunkenSea)
                        {
                            bool valid = true;
                            for (int j = y - 2; j <= y + 1; j++)
                            {
                                for (int i = x - 1; i <= x + 1; i++)
                                {
                                    if (j == y + 1)
                                    {
                                        if (!MiscTools.SolidTileOf(i, j, eutrophicSand))
                                        {
                                            valid = false;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].HasTile)
                                        {
                                            valid = false;
                                        }
                                    }
                                }
                            }

                            if (valid)
                            {
                                WorldGen.PlaceObject(x, y, fanCoral.Type);
                                if (Framing.GetTileSafely(x, y).TileType == fanCoral.Type)
                                {
                                    objects--;
                                }
                            }
                        }
                    }

                    objects = area / 1600;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                        int y = WorldGen.genRand.Next((int)Main.rockLayer, GenVars.UndergroundDesertLocation.Bottom + 1);

                        if (biomes.FindBiome(x, y) == BiomeID.SunkenSea)
                        {
                            bool valid = true;
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                for (int i = x - 1; i <= x + 1; i++)
                                {
                                    if (j == y + 1)
                                    {
                                        if (!MiscTools.SolidTileOf(i, j, eutrophicSand))
                                        {
                                            valid = false;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].HasTile)
                                        {
                                            valid = false;
                                        }
                                    }
                                }
                            }

                            if (valid)
                            {
                                int type = WorldGen.genRand.NextBool(2) ? brainCoralLarge.Type : coralPileLarge.Type;
                                WorldGen.PlaceObject(x, y, type);
                                if (Framing.GetTileSafely(x, y).TileType == type)
                                {
                                    objects--;
                                }
                            }
                        }
                    }

                    objects = area / 800;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                        int y = WorldGen.genRand.Next((int)Main.rockLayer, GenVars.UndergroundDesertLocation.Bottom + 1);

                        if (biomes.FindBiome(x, y) == BiomeID.SunkenSea)
                        {
                            bool valid = true;
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                for (int i = x; i <= x + 1; i++)
                                {
                                    if (j == y + 1)
                                    {
                                        if (!MiscTools.SolidTileOf(i, j, eutrophicSand))
                                        {
                                            valid = false;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].HasTile)
                                        {
                                            valid = false;
                                        }
                                    }
                                }
                            }

                            if (valid)
                            {
                                int type = WorldGen.genRand.NextBool(5) ? seaAnemone.Type : WorldGen.genRand.NextBool(4) ? smallTubeCoral.Type : WorldGen.genRand.NextBool(3) ? brainCoralSmall.Type : WorldGen.genRand.NextBool(2) ? mediumCoral.Type : mediumCoral2.Type;
                                WorldGen.PlaceObject(x, y, type);
                                if (Framing.GetTileSafely(x, y).TileType == type)
                                {
                                    objects--;
                                }
                            }
                        }
                    }
                    objects = area / 400;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                        int y = WorldGen.genRand.Next((int)Main.rockLayer, GenVars.UndergroundDesertLocation.Bottom + 1);

                        if (biomes.FindBiome(x, y) == BiomeID.SunkenSea)
                        {
                            bool valid = true;
                            if (Main.tile[x, y].HasTile)
                            {
                                valid = false;
                            }
                            else if (!MiscTools.SolidTileOf(x, y + 1, eutrophicSand))
                            {
                                valid = false;
                            }

                            if (valid)
                            {
                                WorldGen.PlaceTile(x, y, smallCoral.Type);
                                if (Framing.GetTileSafely(x, y).TileType == smallCoral.Type)
                                {
                                    int style = WorldGen.genRand.Next(6);
                                    Main.tile[x, y].TileFrameX = (short)(style * 18);
                                    Main.tile[x, y].TileFrameY = 0;

                                    objects--;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            objects = 0;
            while (objects < 3)
            {
                int x = WorldGen.genRand.Next(Desert.OasisX - 60, Desert.OasisX + 61);
                int y = (int)(Main.worldSurface * 0.5f);
                while (!MiscTools.HasTile(x, y + 1, TileID.Sand) && y < Main.worldSurface)
                {
                    y++;
                }

                if (!MiscTools.HasTile(x, y, TileID.LargePiles2) && biomes.FindBiome(x, y) == BiomeID.Desert && Main.tile[x, y].LiquidAmount == 0)
                {
                    WorldGen.Place3x2(x, y, TileID.LargePiles2, 52 + objects);

                    if (MiscTools.HasTile(x, y, TileID.LargePiles2))
                    {
                        objects++;
                    }
                }
            }

            Main.tileSolid[ModContent.TileType<SacrificialAltar>()] = true;

            for (int y = 40; y < Main.maxTilesY - 40; y++)
            {
                progress.Set(((float)y - 40) / (Main.maxTilesY - 40 - 40));

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                    {
                        tile = Main.tile[x, y + 1];
                        if (tile.HasTile && Main.tileSolid[tile.TileType])
                        {
                            if (WorldGen.genRand.NextBool(20) && tile.TileType == TileID.Sandstone)
                            {
                                WorldGen.PlaceObject(x, y, TileID.RollingCactus);
                            }
                            else
                            {
                                if (biomes.FindBiome(x, y) != BiomeID.OceanCave && WorldGen.genRand.NextBool(biomes.FindBiome(x, y) == BiomeID.Crimson ? 2 : 4) && MiscTools.NoDoors(x - 1, y, 3) && MiscTools.Tile(x, y + 1).TileType != TileID.Platforms && tile.TileType != ModContent.TileType<SacrificialAltar>())
                                {
                                    LargePile(x, y);
                                    x++;
                                }
                                if (WorldGen.genRand.NextBool(3) && MiscTools.NoDoors(x, y, 2))
                                {
                                    MediumPile(x, y);
                                    x++;
                                }
                                if (WorldGen.genRand.NextBool(2) && MiscTools.NoDoors(x, y))
                                {
                                    SmallPile(x, y);
                                }
                            }
                        }
                    }
                }
            }

            Main.tileSolid[ModContent.TileType<SacrificialAltar>()] = false;
        }

        private void LargePile(int x, int y)
        {
            if (!MiscTools.Tile(x, y).HasTile && !MiscTools.Tile(x - 1, y).HasTile && !MiscTools.Tile(x + 1, y).HasTile)
            {
                Tile tile = Main.tile[x, y + 1];
                ushort type = TileID.LargePiles;
                int style = -1;

                if (Main.wallDungeon[tile.WallType] || y > Main.maxTilesY - 200 && tile.TileType != TileID.Ash || tile.TileType == TileID.BoneBlock)
                {
                    style = Main.rand.Next(7);
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    type = TileID.LargePiles2;
                    style = Main.rand.Next(9, 14);
                }
                else if (tile.TileType == TileID.Stone || tile.TileType == TileID.Dirt || tile.TileType == TileID.Sand && (x < 400 || x > Main.maxTilesX - 400) || tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                {
                    style = Main.rand.Next(7, 13);
                }
                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                {
                    style = Main.rand.Next(26, 32);
                }
                else if (tile.TileType == TileID.MushroomGrass)
                {
                    style = Main.rand.Next(32, 35);
                }
                else
                {
                    type = TileID.LargePiles2;

                    if (tile.TileType == TileID.JungleGrass)
                    {
                        style = Main.rand.Next(3);
                    }
                    else if (tile.TileType == TileID.LeafBlock)
                    {
                        style = Main.rand.Next(50, 52);
                    }
                    else if (tile.TileType == TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
                    {
                        style = Main.rand.Next(47, 50);
                    }
                    else if (tile.TileType == TileID.Grass)
                    {
                        style = Main.rand.Next(14, 17);
                    }
                    else if (tile.TileType == TileID.Ash)
                    {
                        style = Main.rand.Next(6, 9);
                    }
                    else if (tile.TileType == TileID.Sandstone)
                    {
                        style = Main.rand.Next(29, 35);
                    }
                    else if (tile.TileType == TileID.Granite)
                    {
                        style = Main.rand.Next(35, 41);
                    }
                    else if (tile.TileType == TileID.Marble || tile.TileType == TileID.MarbleBlock)
                    {
                        style = Main.rand.Next(41, 47);
                    }
                    else if (tile.TileType == TileID.Crimstone && MiscTools.Tile(x, y - 1).LiquidAmount == 255)
                    {
                        style = Main.rand.Next(52, 55);
                    }
                }

                if (style != -1)
                {
                    WorldGen.Place3x2(x, y, type, style);
                }
            }
        }

        private void MediumPile(int x, int y)
        {
            if (!MiscTools.Tile(x, y).HasTile && !MiscTools.Tile(x + 1, y).HasTile)
            {
                Tile tile = Main.tile[x, y + 1];
                int X = -1;
                int Y = 1;

                if (tile.TileType == ModContent.TileType<SacrificialAltar>())
                {
                    X = Main.rand.Next(11, 16);
                }
                else if (Main.wallDungeon[tile.WallType] || y > Main.maxTilesY - 200 || tile.TileType == TileID.BoneBlock)
                {
                    X = Main.rand.Next(6, 16);
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    X = Main.rand.Next(34, 38);
                }
                else if (tile.TileType == TileID.Stone || tile.TileType == TileID.Dirt || tile.TileType == TileID.Sand && (x < 400 || x > Main.maxTilesX - 400) || tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                {
                    X = Main.rand.Next(6);
                }
                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                {
                    X = Main.rand.Next(25, 31);
                }
                else if (tile.TileType == TileID.Grass && tile.WallType != WallID.LivingWoodUnsafe)
                {
                    X = Main.rand.Next(38, 41);
                }
                else if (tile.TileType == TileID.Sandstone || tile.TileType == ModContent.TileType<PyramidBrick>())
                {
                    X = Main.rand.Next(41, 47);
                }
                else if (tile.TileType == TileID.Granite)
                {
                    X = Main.rand.Next(47, 53);
                }
                //else
                //{
                //    Y = 2;
                //    if (tile.TileType == TileID.Marble)
                //    {
                //        X = Main.rand.Next(6);
                //    }
                //    else if (tile.TileType == TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
                //    {
                //        X = Main.rand.Next(6, 9);
                //    }
                //    else if (tile.TileType == TileID.Sand && MathHelper.Distance(x, Desert.OasisX) > 50 || tile.TileType == TileID.HardenedSand)
                //    {
                //        X = Main.rand.Next(9, 12);
                //    }
                //}

                if (X != -1)
                {
                    WorldGen.PlaceSmallPile(x, y, X, Y);
                }
            }
        }

        private void SmallPile(int x, int y)
        {
            if (!MiscTools.Tile(x, y).HasTile)
            {
                Tile tile = Main.tile[x, y + 1];
                int X = -1;

                if (tile.TileType == ModContent.TileType<SacrificialAltar>())
                {
                    X = Main.rand.Next(20, 28);
                }
                else if (Main.wallDungeon[tile.WallType] || tile.TileType == TileID.Ash || tile.TileType == TileID.BoneBlock)
                {
                    X = Main.rand.NextBool(2) ? Main.rand.Next(12, 28) : Main.rand.Next(28, 36);
                }
                else if (y > Main.maxTilesY - 200)
                {
                    X = Main.rand.Next(12, 28);
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    X = Main.rand.Next(48, 54);
                }
                else if (tile.TileType == TileID.Stone || tile.TileType == TileID.Sand && (x < 400 || x > Main.maxTilesX - 400) || tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                {
                    X = Main.rand.Next(6);
                }
                else if (tile.TileType == TileID.Dirt)
                {
                    X = Main.rand.Next(6, 12);
                }
                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                {
                    X = Main.rand.Next(36, 41);
                }
                else if (tile.TileType == TileID.Sandstone || tile.TileType == ModContent.TileType<PyramidBrick>())
                {
                    X = Main.rand.Next(54, 60);
                }
                else if (tile.TileType == TileID.Granite)
                {
                    X = Main.rand.Next(60, 66);
                }
                else if (tile.TileType == TileID.Marble)
                {
                    X = Main.rand.Next(66, 72);
                }
                else if (tile.TileType == TileID.LivingWood)
                {
                    X = 72;
                }
                else if (tile.TileType == TileID.Sand && MathHelper.Distance(x, Desert.OasisX) > 50 || tile.TileType == TileID.HardenedSand)
                {
                    X = Main.rand.Next(73, 77);
                }

                if (X != -1)
                {
                    WorldGen.PlaceSmallPile(x, y, X, 0);
                }
            }
        }
    }

    public class ReturnThinIce : GenPass
    {
        public ReturnThinIce(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            for (int y = 40; y < (Tundra.Bottom + 1) * biomes.CellSize; y++)
            {
                for (int x = 400; x < Main.maxTilesX - 400; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Tundra)
                    {
                        Tile tile = Main.tile[x, y];

                        if (tile.TileType == TileID.BreakableIce)
                        {
                            tile.HasTile = true;
                        }
                    }
                }
            }
        }
    }
}
