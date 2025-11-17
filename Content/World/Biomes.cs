using Microsoft.Xna.Framework;
using Remnants.Content.Walls;
using StructureHelper;
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
using Remnants.Content.Tiles;
using Remnants.Content.Walls.Vanity;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Plants;
using static Remnants.Content.World.RemWorld;
using static Remnants.Content.World.BiomeMap;
using Remnants.Content.Tiles.Objects.Hazards;
using static Remnants.Content.World.BiomeLocations;

namespace Remnants.Content.World
{
    public class BiomePasses : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            InsertPass(tasks, new BiomeGeneration("Biomes", 1), FindIndex(tasks, "Generate Ice Biome"), true);

            #region Primary Biomes
            RemovePass(tasks, FindIndex(tasks, "Ice"));
            RemovePass(tasks, FindIndex(tasks, "Jungle")); RemovePass(tasks, FindIndex(tasks, "Muds Walls In Jungle"));
            RemovePass(tasks, FindIndex(tasks, "Full Desert")); RemovePass(tasks, FindIndex(tasks, "Dunes"));

            //InsertPass(tasks, new Undergrowth("Undergrowth", 100), FindIndex(tasks, "Tunnels") + 1);

            //InsertPass(tasks, new Hell("Underworld", 0), FindIndex(tasks, "Underworld"), true);
            RemovePass(tasks, FindIndex(tasks, "Underworld"));
            RemovePass(tasks, FindIndex(tasks, "Corruption"));

            RemovePass(tasks, FindIndex(tasks, "Beaches"));
            RemovePass(tasks, FindIndex(tasks, "Ocean Sand"));

            if (calamityEnabled)
            {
                if (ModContent.GetInstance<Worldgen>().SunkenSeaRework)
                {
                    RemovePass(tasks, FindIndex(tasks, "Sunken Sea"));
                }
            }

            if (thoriumEnabled)
            {
                if (ModContent.GetInstance<Worldgen>().AquaticDepthsRework)
                {
                    RemovePass(tasks, FindIndex(tasks, "Thorium Mod: Aquatic Depths"));
                }
            }
            #endregion

            #region Secondary Biomes
            RemovePass(tasks, FindIndex(tasks, "Mushroom Patches"));
            RemovePass(tasks, FindIndex(tasks, "Marble"));
            RemovePass(tasks, FindIndex(tasks, "Granite"));
            RemovePass(tasks, FindIndex(tasks, "Spider Caves"));
            RemovePass(tasks, FindIndex(tasks, "Gem Caves")); RemovePass(tasks, FindIndex(tasks, "Gems")); RemovePass(tasks, FindIndex(tasks, "Gems In Ice Biome")); RemovePass(tasks, FindIndex(tasks, "Random Gems"));
            RemovePass(tasks, FindIndex(tasks, "Create Ocean Caves"));
            RemovePass(tasks, FindIndex(tasks, "Shimmer"));

            if (ruinSeekerEnabled)
            {
                RemovePass(tasks, FindIndex(tasks, "[RS] Thermal Caves"));
            }
            #endregion

            InsertPass(tasks, new SpecialTrees("Special Trees", 20), FindIndex(tasks, "Pots"));
        }
    }

    public class BiomeID
    {
        public const int None = 0;

        public const int Tundra = 1;

        public const int Jungle = 2;

        public const int Desert = 3;

        public const int Underworld = 4;

        public const int Corruption = 5;

        public const int Crimson = 6;

        public const int Glowshroom = 7;

        public const int Marble = 8;

        public const int Granite = 9;

        public const int SpiderNest = 10;

        public const int Beach = 11;

        public const int OceanCave = 12;

        public const int Aether = 13;

        public const int SulfuricVents = 14;

        public const int Toxic = 15;



        public const int SunkenSea = 100;

        public const int AquaticDepths = 101;

        public const int Savanna = 102;

        public const int ThermalCaves = 103;

        public const int Abysm = 104;
    }

    public class BiomeLocations
    {
        public class Tundra
        {
            public static int Left;
            public static int Right;

            public static int Center => (Left + Right + 1) / 2;
            public static int Width => Right - Left + 1;


            public static int Bottom;
        }

        public class Jungle
        {
            public static int Left;
            public static int Right;

            public static int Center => (Left + Right + 1) / 2;
            public static int Width => Right - Left + 1;
        }

        public class Desert
        {
            public static int Left;
            public static int Right;

            public static int Center => (Left + Right + 1) / 2;
            public static int Width => Right - Left + 1;


            public static int Bottom;

            public static int OasisX;
        }

        public class Corruption
        {
            public static int X;
            public static int Y;
            public static int Size;

            public static int Left => X - Size;
            public static int Right => X + Size;

            public static int orbX;
            public static int orbYPrimary => (int)Main.rockLayer;
            public static int orbYSecondary => Main.maxTilesY - 300 - (orbYPrimary - (int)Main.worldSurface);
        }

        public class MarbleCave
        {
            public static int Y;

            public static int Left => (int)(Main.maxTilesX * 0.4f);
            public static int Right => (int)(Main.maxTilesX * 0.6f);
        }
    }

    public class BiomeMap : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            InsertPass(tasks, new BiomeMapPopulation("Biome Map Population", 1), FindIndex(tasks, "Terrain") + 1);
            InsertPass(tasks, new BiomeMapSetup("Biome Map Setup", 1), 1);
        }

        public int[,] Map;

        public int CellSize => 50;
        public int Width => Main.maxTilesX / CellSize;
        public int Height => Main.maxTilesY / CellSize;

        private float[,] BlendX;
        private float[,] BlendY;
        private int BlendDistance => ModContent.GetInstance<Worldgen>().ExperimentalWorldgen ? 0 : 40;

        public float[,] Materials;

        internal class BiomeMapSetup : GenPass
        {
            public BiomeMapSetup(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Setup");

                BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

                biomes.Map = new int[biomes.Width, biomes.Height];

                biomes.BlendX = new float[Main.maxTilesX, Main.maxTilesY];
                biomes.BlendY = new float[Main.maxTilesX, Main.maxTilesY];

                FastNoiseLite blendingNoise = new FastNoiseLite();
                blendingNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
                blendingNoise.SetSeed(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                blendingNoise.SetFrequency(0.02f);
                blendingNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
                blendingNoise.SetFractalOctaves(4);

                FastNoiseLite materialNoise = new FastNoiseLite();
                materialNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
                materialNoise.SetSeed(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                materialNoise.SetFrequency(0.08f);
                materialNoise.SetFractalType(FastNoiseLite.FractalType.FBm);

                biomes.Materials = new float[Main.maxTilesX, Main.maxTilesY];

                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    progress.Set((float)y / Main.maxTilesY);

                    for (int x = 0; x < Main.maxTilesX; x++)
                    {
                        biomes.BlendX[x, y] = blendingNoise.GetNoise(x, y + 999);
                        biomes.BlendY[x, y] = blendingNoise.GetNoise(x + 999, y);

                        biomes.Materials[x, y] = materialNoise.GetNoise(x, y);
                    }
                }

                //for (int y = 0; y <= (int)(Main.worldSurface * 0.4 / scale); y++)
                //{
                //    for (int x = 0; x < width; x++)
                //    {
                //        AddBiome(x, y, "heaven");
                //    }
                //}
                //for (int y = 0; y < biomes.height - 4; y++)
                //{
                //    for (int x = 0; x <= 6; x++)
                //    {
                //        biomes.AddBiome(x, y, BiomeID.Beach);
                //    }
                //    for (int x = biomes.width - 7; x < biomes.width; x++)
                //    {
                //        biomes.AddBiome(x, y, BiomeID.Beach);
                //    }
                //}
            }
        }

        public void AddBiome(int i, int j, int type)
        {
            Map[i, j] = type;
        }

        public int FindBiome(float x, float y, bool blending = true)
        {
            if (!blending)
            {
                //if ((int)y >= Main.maxTilesY - 200)
                //{
                //    return null;
                //}
                int i = (int)MathHelper.Clamp(x, 20, Main.maxTilesX - 20);
                int j = (int)MathHelper.Clamp(y, 20, Main.maxTilesY - 20);
                return Map[i / CellSize, j / CellSize];
            }
            else
            {
                int i = (int)MathHelper.Clamp(x + BlendX[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesX - 20);
                int j = (int)MathHelper.Clamp(y + BlendY[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesY - 20);
                return Map[i / CellSize, j / CellSize];
            }
        }

        public int skyLayer => (int)(Main.worldSurface * 0.4) / CellSize;
        public int surfaceLayer => (int)(Main.worldSurface + CellSize / 2) / CellSize;
        public int caveLayer => (int)(Main.rockLayer + CellSize / 2) / CellSize;
        public int lavaLayer => (int)(GenVars.lavaLine + CellSize / 2) / CellSize - 1;

        internal class BiomeMapPopulation : GenPass
        {
            public BiomeMapPopulation(string name, float loadWeight) : base(name, loadWeight)
            {
            }

            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

                #region layers
                Worldgen WorldgenConfig = Worldgen.Instance;

                int SurfaceChange = (int)WorldgenConfig.FlatSurfaceRatioIncrease;
                int UndergroundChange = (int)WorldgenConfig.FlatUndergroundRatioIncrease;
                int LavaChange = (int)WorldgenConfig.FlatLavaRatioIncrease;

                Main.worldSurface = (int)(Main.maxTilesY / 3f);
                Main.rockLayer = (int)(Main.maxTilesY / 2.25f);
                //GenVars.waterLine = (int)(Main.maxTilesY * 0.75f);
                //GenVars.lavaLine = GenVars.waterLine + 50;

                if (!WorldgenConfig.Safeguard)
                {
                    Main.worldSurface += SurfaceChange;
                    Main.rockLayer += UndergroundChange;
                    GenVars.waterLine += LavaChange;
                    GenVars.lavaLine += LavaChange;
                }

                Main.worldSurface = (int)(Main.worldSurface / 6) * 6;
                Main.rockLayer = (int)(Main.rockLayer / 6) * 6;

                GenVars.worldSurfaceHigh = GenVars.worldSurface = Main.worldSurface;
                GenVars.rockLayerLow = GenVars.rockLayer = Main.rockLayer;

                GenVars.worldSurfaceLow = Main.worldSurface / 2;
                GenVars.rockLayerHigh = Main.maxTilesY - 300;
                #endregion

                bool calamity = ModLoader.TryGetMod("CalamityMod", out Mod cal);
                bool spiritReforged = ModLoader.TryGetMod("SpiritReforged", out Mod sr);

                #region tundra+corruption
                bool tundraCorruptionSwap = WorldGen.genRand.NextBool(2); //GenVars.dungeonSide == 1;

                int tundraSize = (int)(biomes.Width / 7);
                Corruption.Size = Math.Min(biomes.Width / 42, biomes.Height / 12);

                Tundra.Bottom = biomes.lavaLayer - 1;

                if (tundraCorruptionSwap)
                {
                    if (GenVars.dungeonSide != 1)
                    {
                        Corruption.X = (int)(biomes.Width * 0.4f) - 1 - Corruption.Size * 2;

                        Tundra.Right = (Corruption.X - Corruption.Size * 2) - 1 - WorldGen.genRand.Next(Main.maxTilesX / 2100);
                        Tundra.Left = Tundra.Right - tundraSize;
                    }
                    else
                    {
                        Corruption.X = (int)(biomes.Width * 0.6f) + 1 + Corruption.Size * 2;

                        Tundra.Left = (Corruption.X + Corruption.Size * 2) + 1 + WorldGen.genRand.Next(Main.maxTilesX / 2100);
                        Tundra.Right = Tundra.Left + tundraSize;
                    }
                }
                else
                {
                    if (GenVars.dungeonSide != 1)
                    {
                        Tundra.Right = (int)(biomes.Width * 0.4f) - 1;
                        Tundra.Left = Tundra.Right - tundraSize;

                        Corruption.X = Tundra.Left - 1 - Corruption.Size * 2 - WorldGen.genRand.Next(Main.maxTilesX / 2100);
                    }
                    else
                    {
                        Tundra.Left = (int)(biomes.Width * 0.6f) + 1;
                        Tundra.Right = Tundra.Left + tundraSize;

                        Corruption.X = Tundra.Right + 1 + Corruption.Size * 2 + WorldGen.genRand.Next(Main.maxTilesX / 2100);
                    }
                }
                #endregion

                #region jungle+desert
                bool jungleDesertSwap = WorldGen.genRand.NextBool(2); //GenVars.dungeonSide == 1;

                int jungleSize = biomes.Width / 6;
                int desertSize = biomes.Width / 10;

                Desert.Bottom = biomes.lavaLayer + 1;

                if (jungleDesertSwap)
                {
                    if (GenVars.dungeonSide == 1)
                    {
                        Jungle.Right = (int)(biomes.Width * 0.4f) - 1;
                        Jungle.Left = Jungle.Right - jungleSize;

                        if (WorldGen.genRand.NextBool(2) || spiritReforged)
                        {
                            Desert.Left = 7;
                            Desert.Right = Desert.Left + desertSize;
                        }
                        else
                        {
                            Desert.Right = Jungle.Left - 1;
                            Desert.Left = Desert.Right - desertSize;
                        }
                    }
                    else
                    {
                        Jungle.Left = (int)(biomes.Width * 0.6f) + 1;
                        Jungle.Right = Jungle.Left + jungleSize;

                        if (WorldGen.genRand.NextBool(2) || spiritReforged)
                        {
                            Desert.Right = biomes.Width - 8;
                            Desert.Left = Desert.Right - desertSize;
                        }
                        else
                        {
                            Desert.Left = Jungle.Right + 1;
                            Desert.Right = Desert.Left + desertSize;
                        }
                    }
                }
                else
                {
                    if (GenVars.dungeonSide == 1)
                    {
                        Desert.Right = (int)(biomes.Width * 0.4f) - 1;
                        Desert.Left = Desert.Right - desertSize;

                        if (spiritReforged)
                        {
                            Jungle.Left = 7;
                            Jungle.Right = Jungle.Left + jungleSize;
                        }
                        else
                        {
                            Jungle.Right = Desert.Left - 1;
                            Jungle.Left = Jungle.Right - jungleSize;
                        }
                    }
                    else
                    {
                        Desert.Left = (int)(biomes.Width * 0.6f) + 1;
                        Desert.Right = Desert.Left + desertSize;

                        if (spiritReforged)
                        {
                            Jungle.Right = biomes.Width - 8;
                            Jungle.Left = Jungle.Right - jungleSize;
                        }
                        else
                        {
                            Jungle.Left = Desert.Right + 1;
                            Jungle.Right = Jungle.Left + jungleSize;
                        }
                    }
                }

                if (spiritReforged)
                {
                    if (Jungle.Center > Desert.Center)
                    {
                        sr.Call("SetSavannaArea", new Rectangle((Desert.Right) * biomes.CellSize, Terrain.Minimum, (Jungle.Left - Desert.Right + 2) * biomes.CellSize, Terrain.Maximum - Terrain.Minimum));
                    }
                    else sr.Call("SetSavannaArea", new Rectangle((Jungle.Right) * biomes.CellSize, Terrain.Minimum, (Desert.Left - Jungle.Right + 2) * biomes.CellSize, Terrain.Maximum - Terrain.Minimum));
                }
                #endregion

                for (int y = biomes.Height - 4; y < biomes.Height; y++)
                {
                    for (int x = 0; x < biomes.Width; x++)
                    {
                        biomes.AddBiome(x, y, BiomeID.Underworld);
                    }
                }
                for (int y = biomes.Height - 6; y < biomes.Height - 4; y++)
                {
                    for (int x = 0; x < biomes.Width; x++)
                    {
                        biomes.AddBiome(x, y, BiomeID.SulfuricVents);
                    }
                }

                bool thorium = ModLoader.TryGetMod("ThoriumMod", out Mod mod);

                for (int y = 0; y < biomes.Height - 6; y++)
                {
                    for (int x = 0; x <= 6; x++)
                    {
                        bool jungleSide = GenVars.dungeonSide == 1 && x <= 6 || GenVars.dungeonSide != 1 && x >= biomes.Width - 7;
                        bool calamityCompat = !jungleSide && calamity;
                        bool thoriumCompat = jungleSide && thorium;

                        if (thoriumCompat && x <= 7 && y < biomes.caveLayer + 1 && y > biomes.surfaceLayer && ModContent.GetInstance<Worldgen>().AquaticDepthsRework)
                        {
                            biomes.AddBiome(x, y, BiomeID.AquaticDepths);
                        }
                        else if (!calamityCompat && !thoriumCompat && x <= 5 && x > 0 && y < biomes.caveLayer - 1 && y > biomes.surfaceLayer)
                        {
                            biomes.AddBiome(x, y, BiomeID.OceanCave);
                        }
                        else biomes.AddBiome(x, y, BiomeID.Beach);
                    }

                    for (int x = biomes.Width - 7; x < biomes.Width; x++)
                    {
                        bool jungleSide = GenVars.dungeonSide == 1 && x <= 6 || GenVars.dungeonSide != 1 && x >= biomes.Width - 7;
                        bool calamityCompat = !jungleSide && calamity;
                        bool thoriumCompat = jungleSide && thorium;

                        if (thoriumCompat && x >= biomes.Width - 8 && y < biomes.caveLayer + 1 && y > biomes.surfaceLayer && ModContent.GetInstance<Worldgen>().AquaticDepthsRework)
                        {
                            biomes.AddBiome(x, y, BiomeID.AquaticDepths);
                        }
                        else if (!calamityCompat && !thoriumCompat && x >= biomes.Width - 6 && x < biomes.Width - 1 && y < biomes.caveLayer - 1 && y > biomes.surfaceLayer)
                        {
                            biomes.AddBiome(x, y, BiomeID.OceanCave);
                        }
                        else biomes.AddBiome(x, y, BiomeID.Beach);
                    }
                }

                FastNoiseLite noise;

                noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                noise.SetFrequency(0.25f);
                noise.SetFractalType(FastNoiseLite.FractalType.FBm);

                for (int y = 1; y < biomes.Height - 4; y++)
                {
                    for (int x = 7; x < biomes.Width - 7; x++)
                    {
                        float shakeMultiplier = (float)(y - biomes.surfaceLayer) / (float)(biomes.Height - 4 - biomes.surfaceLayer) * (3 + (Main.maxTilesX / 350f));

                        int i = x + (y >= biomes.surfaceLayer ? (int)(noise.GetNoise(x / (Main.maxTilesX / 1050f), y / (Main.maxTilesY / 1200f)) * shakeMultiplier) : 0);
                        int j = y;// + (int)(noise.GetNoise(x + 999, y) * (Main.maxTilesY / 600f));

                        if (biomes.Map[x, y] != BiomeID.Beach)
                        {
                            if (i >= (Tundra.Left + (j == Tundra.Bottom - 1 ? 1 : 0)) && i <= (Tundra.Right - (j == Tundra.Bottom - 1 ? 1 : 0)))
                            {
                                if (j < Tundra.Bottom)
                                {
                                    biomes.AddBiome(x, y, BiomeID.Tundra);
                                }
                                else if (ruinSeekerEnabled && j < biomes.Height - 4)
                                {
                                    biomes.AddBiome(x, y, BiomeID.ThermalCaves);
                                }
                            }
                            else if (biomes.Map[x, y] != BiomeID.SulfuricVents)
                            {
                                if (i >= (Desert.Left + (j == Desert.Bottom - 1 ? 1 : 0)) && i <= (Desert.Right - (j == Desert.Bottom - 1 ? 1 : 0)) && j < Desert.Bottom)
                                {
                                    if (ModContent.GetInstance<Worldgen>().SunkenSeaRework && calamity && j >= (biomes.lavaLayer - biomes.caveLayer) / 2 + biomes.caveLayer)
                                    {
                                        biomes.AddBiome(x, y, BiomeID.SunkenSea);
                                    }
                                    else biomes.AddBiome(x, y, BiomeID.Desert);
                                }
                                else if (i >= Jungle.Left && i <= Jungle.Right)
                                {
                                    if (ModContent.GetInstance<Worldgen>().ExperimentalWorldgen && j > biomes.lavaLayer)
                                    {
                                        biomes.AddBiome(x, y, BiomeID.Toxic);
                                    }
                                    else biomes.AddBiome(x, y, BiomeID.Jungle);
                                }
                                else if (spiritReforged && y < biomes.surfaceLayer - 1)
                                {
                                    if (i > Desert.Center && i < Jungle.Center || i < Desert.Center && i > Jungle.Center)
                                    {
                                        biomes.AddBiome(x, y, BiomeID.Savanna);
                                    }
                                }
                            }
                        }
                    }
                }
                GenVars.UndergroundDesertLocation = new Rectangle((Desert.Left) * biomes.CellSize, (biomes.surfaceLayer - 1) * biomes.CellSize, (Desert.Width) * biomes.CellSize, (Desert.Bottom - (biomes.surfaceLayer - 1)) * biomes.CellSize);
                GenVars.structures.AddStructure(GenVars.UndergroundDesertLocation);

                #region corruption
                Corruption.Y = (int)Main.worldSurface / biomes.CellSize;
                Corruption.orbX = (int)((Corruption.X + 0.5f) * biomes.CellSize);
                noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                noise.SetFrequency(0.2f);
                noise.SetFractalType(FastNoiseLite.FractalType.FBm);

                for (int j = biomes.skyLayer; j < biomes.Height - 6; j++)
                {
                    for (int i = 7; i < biomes.Width - 7; i++)
                    {
                        Vector2 point = new Vector2(Corruption.X, MathHelper.Clamp(j, 1, biomes.caveLayer));
                        if ((j < biomes.caveLayer ? 0 : noise.GetNoise(i, j)) <= (1 - Vector2.Distance(point, new Vector2(i, j)) / Corruption.Size) * 2)
                        {
                            if (WorldGen.crimson)
                            {
                                biomes.AddBiome(i, j, BiomeID.Crimson);
                            }
                            else biomes.AddBiome(i, j, BiomeID.Corruption);
                        }

                        point = new Vector2(Corruption.X, MathHelper.Clamp(j, (Main.maxTilesY - 300 - (int)(Main.rockLayer - Main.worldSurface)) / biomes.CellSize - 1, biomes.Height - 6));
                        if ((j > biomes.Height - 6 - (biomes.caveLayer - biomes.surfaceLayer) ? 0 : noise.GetNoise(i, j)) <= (1 - Vector2.Distance(point, new Vector2(i, j)) / Corruption.Size) * 2)
                        {
                            if (!WorldGen.crimson)
                            {
                                biomes.AddBiome(i, j, BiomeID.Crimson);
                            }
                            else biomes.AddBiome(i, j, BiomeID.Corruption);
                        }
                    }
                }

                #endregion

                #region minibiomes
                FastNoiseLite glowshroom = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                glowshroom.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                glowshroom.SetFractalType(FastNoiseLite.FractalType.None);
                glowshroom.SetCellularJitter(0.75f);
                glowshroom.SetFrequency(0.2f);

                FastNoiseLite spidernest = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                spidernest.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                spidernest.SetFractalType(FastNoiseLite.FractalType.None);
                spidernest.SetCellularJitter(0.75f);
                spidernest.SetFrequency(0.125f);

                int marbleCaveLeft = (int)(biomes.Width * 0.4f);
                int marbleCaveRight = (int)(biomes.Width * 0.6f);
                MarbleCave.Y = Math.Min(biomes.lavaLayer, biomes.Height - 8 - Main.maxTilesY / 600);

                for (int i = 1; i < biomes.Width - 1; i++)
                {
                    for (int j = 0; j < biomes.Height - 5; j++)
                    {
                        if (j >= biomes.surfaceLayer)
                        {
                            if (i <= 5 && i > 0 || i >= biomes.Width - 6 && i < biomes.Width - 1)
                            {
                                bool jungleSide = GenVars.dungeonSide == 1 && i <= 5 || GenVars.dungeonSide != 1 && i >= biomes.Width - 6;

                                if (jungleSide && j > biomes.caveLayer + (thorium ? 2 : 0) && j < biomes.Height - 7)
                                {
                                    biomes.AddBiome(i, j, BiomeID.Aether);
                                }
                            }
                            else if (i > 6 && i < biomes.Width - 7)
                            {
                                if (i >= Tundra.Center - 1 && i <= Tundra.Center + 1)
                                {
                                    biomes.AddBiome(i, j, BiomeID.Granite);
                                }
                                else if (j >= MarbleCave.Y - 1 && j <= MarbleCave.Y + 1 && i >= marbleCaveLeft && i <= marbleCaveRight)
                                {
                                    biomes.AddBiome(i, j, BiomeID.Marble);
                                }
                                //else if (biomes.biomeMap[i, j] == BiomeID.Jungle)
                                //{
                                //    if (ModContent.GetInstance<Client>().ExperimentalWorldgen && j >= biomes.lavaLayer)
                                //    {
                                //        biomes.AddBiome(i, j, BiomeID.Toxic);
                                //    }
                                //}
                                else if (biomes.Map[i, j] == BiomeID.None)
                                {
                                    if (j > biomes.caveLayer)
                                    {
                                        if (glowshroom.GetNoise(i / 2f, j - (biomes.caveLayer + biomes.lavaLayer) / 2f) < -0.95f && j < biomes.lavaLayer)
                                        {
                                            biomes.AddBiome(i, j, BiomeID.Glowshroom);
                                        }
                                        else if (j < biomes.Height - 7 && spidernest.GetNoise(i, j - (biomes.lavaLayer + biomes.Height - 6) / 2f) < -0.9625f)
                                        {
                                            biomes.AddBiome(i, j, BiomeID.SpiderNest);
                                        }
                                    }
                                    //else if (flesh.GetNoise(i, j) > 0.5f && j >= WorldGen.lavaLine / biomes.scale - 1)
                                    //{
                                    //    biomes.AddBiome(i, j, "flesh");
                                    //}
                                    //else if (j > (int)Main.rockLayer / biomes.scale)
                                    //{
                                    //    if (_granitemarble > 0.55f)
                                    //    {
                                    //        biomes.AddBiome(i, j, "granite");
                                    //    }
                                    //}
                                }
                            }
                        }
                        //else if (biomes.biomeMap[i, j] == null && meadows.GetNoise(i, 0) > 0)
                        //{
                        //    biomes.AddBiome(i, j, "meadow");
                        //}

                        if ((GenVars.dungeonSide == 1 || !calamity))
                        {
                            biomes.AddBiome(1, biomes.surfaceLayer, BiomeID.OceanCave); biomes.AddBiome(1, biomes.surfaceLayer - 1, BiomeID.OceanCave);
                        }
                        if ((GenVars.dungeonSide != 1 || !calamity))
                        {
                            biomes.AddBiome(biomes.Width - 2, biomes.surfaceLayer, BiomeID.OceanCave); biomes.AddBiome(biomes.Width - 2, biomes.surfaceLayer - 1, BiomeID.OceanCave);
                        }
                    }
                }

                //int count = 0;
                //while (count < biomes.Width * (biomes.Height - 6 - biomes.caveLayer) / 40)
                //{
                //    int x = WorldGen.genRand.Next(7, biomes.Width - 7);
                //    int y = WorldGen.genRand.Next(biomes.caveLayer, biomes.Height - 6);

                //    if (biomes.Map[x, y] == BiomeID.None)
                //    {
                //        if (biomes.Map[x - 1, y] != BiomeID.GemCave && biomes.Map[x + 1, y] != BiomeID.GemCave && biomes.Map[x, y - 1] != BiomeID.GemCave && biomes.Map[x, y + 1] != BiomeID.GemCave && biomes.Map[x - 1, y - 1] != BiomeID.GemCave && biomes.Map[x + 1, y - 1] != BiomeID.GemCave && biomes.Map[x - 1, y + 1] != BiomeID.GemCave && biomes.Map[x + 1, y + 1] != BiomeID.GemCave)
                //        {
                //            biomes.AddBiome(x, y);

                //            count++;
                //        }
                //    }
                //}
                #endregion
            }
        }

        public bool IsUpdatingBiome(float x, float y, bool[] biomesToUpdate, int type)
        {
            return biomesToUpdate[type] && FindBiome(x, y) == type;
        }

        public float GetTileDistribution(float x, float y, bool flip = false, float frequency = 1)
        {
            x *= frequency;
            y *= frequency;
            if (frequency > 1)
            {
                x %= Main.maxTilesX;
                y %= Main.maxTilesY;
            }
            //double noiseX = 0;
            //double noiseY = 0;
            //float multiplier = 1;
            //for (int i = 0; i < materialsX.Length; i++)
            //{
            //    noiseX += Math.Sin((x + WorldGen.genRand.NextFloat(-0.5f, 0.5f)) * materialsX[i] * frequency * multiplier) / materialsX.Length;
            //    noiseY += Math.Sin((y + WorldGen.genRand.NextFloat(-0.5f, 0.5f)) * materialsY[i] * frequency * multiplier) / materialsY.Length;
            //    multiplier *= 1.75f;
            //}
            if (flip ? WorldGen.InWorld((int)x, Main.maxTilesY - (int)y) : WorldGen.InWorld((int)x, (int)y))
            {
                return Materials[(int)x, flip ? Main.maxTilesY - (int)y : (int)y];
            }
            else return 0;
        }

        public int GetLayer(int x, int y)
        {
            return (int)MathHelper.Clamp(y + BlendY[x, y] * BlendDistance, 20, Main.maxTilesY - 20) / CellSize;
        }

        private void ResetNoise(FastNoiseLite noise)
        {
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(0.1f);
            noise.SetFractalOctaves(3);
            noise.SetFractalGain(0.5f);
            noise.SetFractalLacunarity(2);
            noise.SetFractalWeightedStrength(0);
            noise.SetFractalPingPongStrength(2);
            noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
            noise.SetCellularJitter(1);
        }

        public void UpdateMap(int[] biomes, GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.BiomeUpdate");

            FastNoiseLite caves1 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            FastNoiseLite caves2 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            FastNoiseLite caves3 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));

            FastNoiseLite gems = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            gems.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            gems.SetFrequency(0.025f);
            gems.SetFractalType(FastNoiseLite.FractalType.None);

            FastNoiseLite fossils = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            fossils.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fossils.SetFrequency(0.1f);
            fossils.SetFractalType(FastNoiseLite.FractalType.Ridged);

            FastNoiseLite background = new FastNoiseLite();

            bool[] biomesToUpdate = new bool[129];
            for (int i = 0; i < biomes.Length; i++)
            {
                biomesToUpdate[biomes[i]] = true;
            }

            int startY = 40;

            bool calamity = ModLoader.TryGetMod("CalamityMod", out Mod cal);
            bool thorium = ModLoader.TryGetMod("ThoriumMod", out Mod th);
            bool lunarVeil = ModLoader.TryGetMod("Stellamod", out Mod lv);
            bool spiritReforged = ModLoader.TryGetMod("SpiritReforged", out Mod sr);
            bool ruinSeeker = ModLoader.TryGetMod("RuinSeeker", out Mod rs);

            #region sunkensea
            ushort eutrophicSand = 0;
            ushort hardenedEutrophicSand = 0;
            ushort navystone = 0;
            ushort seaPrism = 0;
            ushort prismShard = 0;
            ushort stalactite = 0;
            ushort eutrophicSandWall = 0;
            ushort navystoneWall = 0;
            if (calamity && biomesToUpdate[BiomeID.SunkenSea])
            {
                if (cal.TryFind("EutrophicSand", out ModTile sand))
                {
                    eutrophicSand = sand.Type;
                }
                if (cal.TryFind("HardenedEutrophicSand", out ModTile hardenedSand))
                {
                    hardenedEutrophicSand = hardenedSand.Type;
                }
                if (cal.TryFind("Navystone", out ModTile stone))
                {
                    navystone = stone.Type;
                }
                if (cal.TryFind("SeaPrism", out ModTile prism))
                {
                    seaPrism = prism.Type;
                }
                if (cal.TryFind("SeaPrismCrystals", out ModTile shard))
                {
                    prismShard = shard.Type;
                }
                if (cal.TryFind("SunkenStalactitesSmall", out ModTile stalac))
                {
                    stalactite = stalac.Type;
                }

                if (cal.TryFind("EutrophicSandWall", out ModWall sandWall))
                {
                    eutrophicSandWall = sandWall.Type;
                }
                if (cal.TryFind("NavystoneWall", out ModWall stoneWall))
                {
                    navystoneWall = stoneWall.Type;
                }
            }
            #endregion
            #region sulphursea
            ushort sulphurousSand = 0;
            ushort sulphurousSandstone = 0;
            ushort sulphurousSandstoneWall = 0;
            ushort hardenedSulphurousSandstone = 0;
            ushort hardenedSulphurousSandstoneWall = 0;
            if (calamity && biomesToUpdate[BiomeID.SunkenSea])
            {
                if (cal.TryFind("SulphurousSand", out ModTile sand))
                {
                    sulphurousSand = sand.Type;
                }
                if (cal.TryFind("SulphurousSandstone", out ModTile sandstone))
                {
                    sulphurousSandstone = sandstone.Type;
                }
                if (cal.TryFind("SulphurousSandstoneWall", out ModWall sandstoneWall))
                {
                    sulphurousSandstoneWall = sandstoneWall.Type;
                }
                if (cal.TryFind("HardenedSulphurousSandstone", out ModTile hardenedSandstone))
                {
                    hardenedSulphurousSandstone = hardenedSandstone.Type;
                }
                if (cal.TryFind("HardenedSulphurousSandstoneWall", out ModWall hardenedSandstoneWall))
                {
                    hardenedSulphurousSandstoneWall = hardenedSandstoneWall.Type;
                }
            }
            #endregion

            #region aquaticdepths
            ushort leakyMarine = 0;
            ushort leakyMossyMarine = 0;
            ushort brack = 0;
            ushort aquaite = 0;
            ushort aquamarine = 0;
            ushort marineWall = 0;

            if (thorium)
            {
                if (th.TryFind("LeakyMarineBlock", out ModTile leak))
                {
                    leakyMarine = leak.Type;
                }
                if (th.TryFind("LeakyMossyMarineBlock", out ModTile moss))
                {
                    leakyMossyMarine = moss.Type;
                }
                if (th.TryFind("BrackishClumpTile", out ModTile clump))
                {
                    brack = clump.Type;
                }

                if (th.TryFind("Aquaite", out ModTile aquite))
                {
                    aquaite = aquite.Type;
                }
                if (th.TryFind("DepthsAquamarine", out ModTile aqua))
                {
                    aquamarine = aqua.Type;
                }
                if (th.TryFind("MarineWall", out ModWall leakywall))
                {
                    marineWall = leakywall.Type;
                }
            }
            #endregion

            #region savanna
            ushort savannaDirt = 0;
            ushort savannaDirtWall = 0;
            if (spiritReforged && biomesToUpdate[BiomeID.Savanna])
            {
                if (sr.TryFind("SavannaDirt", out ModTile dirt))
                {
                    savannaDirt = dirt.Type;
                }
                if (sr.TryFind("SavannaDirtWall_Unsafe", out ModWall dirtWall))
                {
                    savannaDirtWall = dirtWall.Type;
                }
            }
            #endregion

            #region thermalcaves
            ushort gabbro = 0;
            ushort gabbroWall = 0;
            ushort basalt = 0;
            ushort basaltWall = 0;
            if (ruinSeeker && biomesToUpdate[BiomeID.ThermalCaves])
            {
                if (rs.TryFind("Gabbro_Tile", out ModTile stone))
                {
                    gabbro = stone.Type;
                }
                if (rs.TryFind("Gabbro_Wall_Unsafe", out ModWall stoneWall))
                {
                    gabbroWall = stoneWall.Type;
                }
                if (rs.TryFind("Basalt_Tile", out ModTile bas))
                {
                    basalt = bas.Type;
                }
                if (rs.TryFind("Basalt_Wall_Unsafe", out ModWall basWall))
                {
                    basaltWall = basWall.Type;
                }
            }
            #endregion

            if (biomesToUpdate[BiomeID.Glowshroom])
            {
                Main.tileSolid[TileID.MushroomBlock] = true;
            }
            if (biomesToUpdate[BiomeID.Aether])
            {
                Main.tileSolid[TileID.ShimmerBlock] = true;

                GenVars.shimmerPosition.X = GenVars.dungeonSide != 1 ? Main.maxTilesX - 175 : 175;
                GenVars.shimmerPosition.Y = Main.rockLayer + 200;
            }

            for (int x = 40; x < Main.maxTilesX - 40; x++)
            {
                progress.Set((x - 40) / (float)(Main.maxTilesX - 40 - 40));

                for (int y = startY; y < Main.maxTilesY - 40; y++)
                {
                    Tile tile = MiscTools.Tile(x, y);

                    int i = (int)MathHelper.Clamp(x + BlendX[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesX - 20);
                    int j = (int)MathHelper.Clamp(y + BlendY[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesY - 20);

                    int layer = j / CellSize;

                    bool underground = layer >= surfaceLayer;
                    bool sky = layer < skyLayer;

                    ResetNoise(caves1);
                    ResetNoise(caves2);
                    ResetNoise(caves3);
                    ResetNoise(background);

                    #region surface
                    if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Tundra))
                    {
                        if (layer >= surfaceLayer)
                        {
                            if (GetTileDistribution(x, y, frequency: 2) >= 0.2f)
                            {
                                tile.TileType = TileID.SnowBlock;
                            }
                            else tile.TileType = TileID.IceBlock;

                            if (tile.WallType != 0 || layer >= surfaceLayer)
                            {
                                if (GetTileDistribution(x, y, frequency: 2) >= 0.2f)
                                {
                                    tile.WallType = WallID.SnowWallUnsafe;
                                }
                                else tile.WallType = WallID.IceUnsafe;
                            }
                        }
                        else
                        {
                            if (tile.TileType == TileID.Silt)
                            {
                                tile.TileType = TileID.Slush;
                            }
                            else if (tile.TileType == TileID.Dirt)
                            {
                                tile.TileType = TileID.SnowBlock;
                            }
                            else if (tile.TileType == TileID.Stone)
                            {
                                tile.TileType = TileID.IceBlock;
                            }

                            if (tile.WallType == WallID.DirtUnsafe)
                            {
                                tile.WallType = WallID.SnowWallUnsafe;
                            }
                            else if (tile.WallType == WallID.RocksUnsafe1)
                            {
                                tile.WallType = WallID.IceUnsafe;
                            }
                        }

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFractalType(FastNoiseLite.FractalType.PingPong);
                        caves1.SetFractalOctaves(3);
                        caves1.SetFrequency(0.0075f);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.0075f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);

                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.015f);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        ResetNoise(background);;
                        background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        background.SetFrequency(0.1f);
                        background.SetFractalOctaves(1);

                        float _background = background.GetNoise(x * 2, y / 2) + 1f;

                        if (layer > surfaceLayer)
                        {
                            caves1.SetFractalOctaves(3);
                            caves1.SetFractalPingPongStrength(caves2.GetNoise(i, j * 2) * 1.5f + 1.5f);

                            float _caves = caves1.GetNoise(i, j * 2);
                            float _size = caves3.GetNoise(i, j * 2) / 2;

                            //if (layer < surfaceLayer)
                            //{
                            //    _caves = ((_caves - 0.8f) * 2) + 0.8f;
                            //}

                            if ((_caves / 2 + 0.5f) < 0.5f)// (_size / 2 + 0.5f) * 0.9f)
                            {
                                tile.HasTile = false;
                                if (WorldGen.genRand.NextBool(25) && y < GenVars.lavaLine)
                                {
                                    tile.LiquidAmount = 255;
                                }

                                if ((_caves / 2 + 0.5f) < 0.4f - _background * 0.2f)
                                {
                                    tile.WallType = 0;
                                }
                            }
                            else
                            {
                                tile.HasTile = true;
                            }
                        }
                        else if (j >= Main.worldSurface * 0.35f && i > (Tundra.Left + Main.maxTilesX / 3500f) * CellSize && i < (Tundra.Right + 1 - Main.maxTilesX / 3500f) * CellSize)
                        {
                            float graniteEntrance = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(i, j), new Vector2((Tundra.Center + 0.5f) * CellSize, (int)Main.worldSurface)) / 50, 0, 1);

                            caves1.SetFractalOctaves(1);
                            caves1.SetFractalPingPongStrength(1f);
                            float _caves = caves1.GetNoise(i, j * 2);

                            if ((_caves / 2 + 0.5f) > 0.9f - graniteEntrance)
                            {
                                tile.HasTile = false;
                            }

                            if ((Math.Min(caves1.GetNoise(i, (j - 6 - _background * 18) * 2), (j + 6 + _background * 18) > Main.worldSurface ? 0 : caves1.GetNoise(i, (j + 6 + _background * 18) * 2)) / 2 + 0.5f) > 0.9f - graniteEntrance / 2)
                            {
                                tile.WallType = 0;
                                tile.LiquidAmount = 51;
                            }
                        }

                        if (MiscTools.Tile(x - 1, y - 1).WallType == 0)
                        {
                            if (MiscTools.Tile(x - 2, y - 1).WallType != 0 && MiscTools.Tile(x, y - 1).WallType != 0 && MiscTools.Tile(x - 1, y - 2).WallType != 0 && MiscTools.Tile(x - 1, y).WallType != 0)
                            {
                                MiscTools.Tile(x - 1, y - 1).WallType = WallID.IceUnsafe;
                            }
                        }

                        //if (i >= (Tundra.Center + 0.5f) * CellSize - 8 && i <= (Tundra.Center + 0.5f) * CellSize + 8)
                        //{
                        //    tile.TileType = TileID.Granite;

                        //    if (i >= (Tundra.Center + 0.5f) * CellSize - 6 && i <= (Tundra.Center + 0.5f) * CellSize + 6)
                        //    {
                        //        if (tile.WallType != 0)
                        //        {
                        //            tile.WallType = WallID.GraniteUnsafe;
                        //        }

                        //        if (i >= (Tundra.Center + 0.5f) * CellSize - 4 && i <= (Tundra.Center + 0.5f) * CellSize + 4)
                        //        {
                        //            tile.HasTile = false;
                        //        }
                        //    }
                        //}
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Jungle))
                    {
                        if (tile.TileType == TileID.Grass)
                        {
                            tile.TileType = TileID.JungleGrass;
                        }
                        else if (tile.TileType == TileID.Dirt || tile.TileType == TileID.ClayBlock)
                        {
                            tile.TileType = TileID.Mud;
                        }
                        else if (tile.TileType == TileID.Silt)
                        {
                            tile.TileType = TileID.Stone;
                        }

                        if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe || tile.WallType == WallID.CaveWall2)
                        {
                            tile.WallType = WallID.JungleUnsafe;
                        }
                        else if (tile.WallType == WallID.RocksUnsafe1 || tile.WallType == WallID.Cave8Unsafe)
                        {
                            tile.WallType = WallID.JungleUnsafe3;
                        }

                        //if (GetTileDistribution(x, y, frequency: 2) >= (layer >= surfaceLayer && layer < caveLayer ? -0.3f : 0f))//(layer < caveLayer ? -0.2f : 0.2f))
                        //{
                        //    tile.TileType = TileID.Mud;
                        //}
                        //else tile.TileType = TileID.Stone;

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFrequency(0.02f);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves2.SetFrequency(0.02f);
                        //caves2.SetFractalType(FastNoiseLite.FractalType.None);
                        caves2.SetFractalLacunarity(4);
                        caves2.SetFractalGain(0.75f);

                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves3.SetFrequency(0.08f);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        //if (layer >= surfaceLayer)
                        //{
                        //    if (GetTileDistribution(x, y, frequency: 2) >= (layer >= surfaceLayer && layer < caveLayer ? -0.3f : 0f))//(layer < caveLayer ? -0.2f : 0.2f))
                        //    {
                        //        tile.TileType = TileID.Mud;
                        //    }
                        //    else tile.TileType = TileID.Stone;
                        //}

                        //if (tile.WallType != 0)
                        //{
                        //    if (GetTileDistribution(x, y, frequency: 2) >= (layer >= surfaceLayer && layer < caveLayer ? -0.3f : 0f))//(layer < caveLayer ? -0.2f : 0.2f))
                        //    {
                        //        tile.WallType = WallID.JungleUnsafe;
                        //    }
                        //    else tile.WallType = WallID.JungleUnsafe3; //layer >= lavaLayer ? WallID.LavaUnsafe2 :
                        //}

                        float _caves = caves1.GetNoise(x, y);
                        float _size = caves2.GetNoise(x, y) / 2 + 0.5f;
                        float _chasm = layer < caveLayer ? (1 - Math.Clamp(MathHelper.Distance(x + caves3.GetNoise(x / 2f, j) * 12, (Jungle.Center + 0.5f) * CellSize) / (layer < surfaceLayer ? 12 : 24), 0, 1)) / 2 : 0;

                        if (layer >= surfaceLayer || _chasm > 0)
                        {
                            if (layer >= surfaceLayer)
                            {
                                tile.HasTile = true;

                                //if (GetTileDistribution(x, y, frequency: 2) >= -0.1f)
                                //{
                                //    tile.WallType = WallID.JungleUnsafe;
                                //}
                                //else tile.WallType = WallID.JungleUnsafe3;
                            }

                            if (_chasm > (layer >= surfaceLayer ? 0.2f : 0.1f))
                            {
                                tile.WallType = 0;
                            }

                            if (_caves > - _size / (layer >= caveLayer ? 3 : 6) - _chasm && _caves < _size / (layer >= caveLayer ? 3 : 6) + _chasm)
                            {
                                tile.HasTile = false;

                                if (layer > surfaceLayer)
                                {
                                    //float _background = background.GetNoise(x, y);
                                    //_background *= 0.01f;

                                    if (_size > 0.6f)
                                    {
                                        tile.WallType = 0;
                                    }
                                }
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Desert))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                        caves1.SetFrequency(0.08f / 6);
                        caves1.SetFractalType(FastNoiseLite.FractalType.None);
                        //caves1.SetFractalOctaves(2);
                        caves1.SetFractalGain(0.8f);
                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.04f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves2.SetFractalOctaves(4);
                        caves2.SetFractalGain(0.75f);
                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.04f / 6);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        //background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        //background.SetFrequency(0.1f);
                        //background.SetFractalType(FastNoiseLite.FractalType.None);
                        //background.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
                        //background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);

                        if (layer >= surfaceLayer)
                        {
                            bool doWalls = FindBiome(x - 25, y) == BiomeID.Desert && FindBiome(x + 25, y) == BiomeID.Desert && FindBiome(x, y - 25) == BiomeID.Desert && FindBiome(x, y + 25) == BiomeID.Desert;

                            tile.HasTile = true;
                            tile.TileType = doWalls && GetLayer(x, y - 25) >= surfaceLayer ? TileID.Sandstone : TileID.HardenedSand;

                            if (!doWalls && caves2.GetNoise(x, y) > 0)
                            {
                                tile.WallType = 0;
                            }
                            else if (doWalls || tile.WallType != 0)
                            {
                                tile.WallType = doWalls && GetLayer(x, y - 25) >= surfaceLayer ? WallID.Sandstone : WallID.HardenedSand;
                            }
                            tile.LiquidAmount = 0;

                            float _tunnels = caves1.GetNoise(x, y);
                            //float _nests = nests.GetNoise(x, y + ((float)Math.Cos(x / 60) * 20)) * ((nests2.GetNoise(x, y + ((float)Math.Cos(x / 60) * 20)) + 1) / 2);
                            float _fossils = (fossils.GetNoise(x, y) + 1) / 4;

                            float _background = (background.GetNoise(i, j) + 1) / 2 * 0.4f;

                            float _size = caves2.GetNoise(x, y) / 4 + 0.1875f;
                            float _offset = caves3.GetNoise(x, y);

                            if (_tunnels + 0.2f < _offset - _size || _tunnels - 0.2f > _offset + _size)
                            {
                                if (_tunnels + 0.2f + _fossils < _offset - _size || _tunnels - 0.2f - _fossils > _offset + _size)
                                {
                                    if (_tunnels + 0.3f + _fossils < _offset - _size || _tunnels - 0.3f - _fossils > _offset + _size)
                                    {
                                        tile.TileType = TileID.DesertFossil;
                                    }
                                }
                                else if (_tunnels + 0.2f + _fossils > _offset - _size || _tunnels - 0.2f - _fossils < _offset + _size)
                                {
                                    tile.TileType = TileID.HardenedSand;
                                    if (doWalls)
                                    {
                                        tile.WallType = WallID.HardenedSand;
                                    }
                                }
                            }
                            else if (_tunnels > _offset - _size && _tunnels < _offset + _size)
                            {
                                tile.HasTile = false;
                                tile.LiquidAmount = 0;
                            }
                        }
                        else
                        {
                            MiscTools.Tile(x, y).TileType = TileID.HardenedSand;
                            if (tile.WallType != 0)
                            {
                                tile.WallType = WallID.HardenedSand;
                            }

                            if (layer == surfaceLayer - 1)
                            {
                                tile.HasTile = true;
                                tile.TileType = TileID.HardenedSand;
                            }
                            //if (tile.HasTile)// && y > Terrain.Middle)
                            //{
                            //    int var = 1;
                            //    //if (y / biomes.scale >= (int)Main.worldSurface / biomes.scale - 1 || dunes.GetNoise(x, y + 2) > 0)
                            //    //{
                            //    //    var = 2;
                            //    //}
                            //    for (int k = -var; k <= var; k++)
                            //    {
                            //        if (!MiscTools.Tile(x + k, y + 1).HasTile)
                            //        {
                            //            MiscTools.Tile(x + k, y + 1).TileType = TileID.Sand;
                            //            MiscTools.Tile(x + k, y + 1).HasTile = true;
                            //        }
                            //    }
                            //}
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Beach))
                    {
                        //bool sulphurSea = false;
                        //if (calamity)
                        //{
                        //    if (GenVars.dungeonSide == 1)
                        //    {
                        //        if (i > Main.maxTilesX / 2)
                        //        {
                        //            sulphurSea = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (i < Main.maxTilesX / 2)
                        //        {
                        //            sulphurSea = true;
                        //        }
                        //    }
                        //}

                        if (layer >= surfaceLayer)
                        {
                            if (layer < caveLayer)
                            {
                                tile.LiquidAmount = 255;
                            }

                            if (tile.TileType == TileID.ArgonMoss)
                            {
                                tile.TileType = TileID.Stone;
                            }
                            else if (tile.TileType == TileID.JungleGrass || tile.TileType == TileID.MushroomGrass)
                            {
                                tile.TileType = TileID.Mud;
                            }
                        }
                        //if (sulphurSea)
                        //{
                        //    if (layer < caveLayer)
                        //    {
                        //        if (layer < surfaceLayer)
                        //        {
                        //            tile.TileType = sulphurousSandstone;

                        //            if (tile.WallType != 0)
                        //            {
                        //                tile.WallType = sulphurousSandstoneWall;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            tile.TileType = hardenedSulphurousSandstone;

                        //            if (tile.WallType != 0)
                        //            {
                        //                tile.WallType = hardenedSulphurousSandstoneWall;
                        //            }
                        //        }
                        //    }
                        //}

                        if (layer > surfaceLayer)
                        {
                            tile.HasTile = true;
                        }
                        else
                        {
                            if (y >= Terrain.Maximum - 10 && tile.TileType != TileID.ShellPile)
                            {
                                for (int k = 1; k <= WorldGen.genRand.Next(5, 7); k++)
                                {
                                    if (!MiscTools.Tile(x, y - k).HasTile)
                                    {
                                        tile.TileType = TileID.Sand; //sulphurSea ? sulphurousSand : TileID.Sand;
                                        if (tile.WallType == WallID.DirtUnsafe)
                                        {
                                            tile.WallType = WallID.RocksUnsafe1;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Corruption))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.005f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.Ridged);
                        caves1.SetFractalOctaves(3);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                        caves1.SetFractalLacunarity(3);
                        caves1.SetFractalGain(0.25f);
                        caves1.SetCellularJitter(0.75f);

                        if (!Main.wallDungeon[tile.WallType])
                        {
                            if (tile.TileType == TileID.Stone)
                            {
                                tile.TileType = TileID.Ebonstone;
                            }
                            if (layer >= surfaceLayer && tile.WallType != WallID.CorruptionUnsafe1 || tile.WallType == WallID.RocksUnsafe1)
                            {
                                tile.WallType = WallID.EbonstoneUnsafe;
                            }

                            if (layer >= surfaceLayer)
                            {
                                tile.HasTile = true;
                            }

                            if (y > Terrain.Maximum - 5 && i / CellSize > Corruption.Left && i / CellSize < Corruption.Right)
                            {
                                float _size = 1;// (caves2.GetNoise(x * 2, y) + 1) / 2;
                                float _caves = caves1.GetNoise(i * 2, j - Terrain.Maximum) / 2 + 0.5f;

                                _caves -= (layer >= surfaceLayer ? 0f : 0.1f);

                                float orbDistance = Vector2.Distance(new Vector2(i * 2, j), new Vector2(Corruption.orbX * 2, !WorldGen.crimson ? Corruption.orbYPrimary : Corruption.orbYSecondary));
                                _caves += MathHelper.Clamp(1 - orbDistance / (Main.maxTilesX / 4200f * 36), 0, 1);

                                if (_caves > 0.4f)
                                {
                                    tile.TileType = TileID.Ebonstone;

                                    if (_caves > 0.55f)
                                    {
                                        tile.HasTile = false;

                                        if (layer >= surfaceLayer && _caves > 0.7f)
                                        {
                                            tile.WallType = WallID.CorruptionUnsafe2;
                                        }
                                    }
                                }
                            }

                            if (layer < surfaceLayer && MiscTools.Tile(x - 1, y - 1).TileType == TileID.Dirt && !MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.CorruptGrass;
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Crimson))
                    {
                        if (tile.TileType == TileID.Stone)
                        {
                            tile.TileType = TileID.Crimstone;
                        }
                        if (tile.WallType == WallID.RocksUnsafe1)
                        {
                            tile.WallType = WallID.CrimstoneUnsafe; //!MiscTools.SurroundingTilesActive(x, y) && layer < surfaceLayer ? WallID.CrimsonGrassUnsafe : WallID.CrimstoneUnsafe;
                        }

                        if (!Main.wallDungeon[tile.WallType])
                        {
                            if (layer < surfaceLayer)
                            {
                                if (tile.WallType == WallID.DirtUnsafe)
                                {
                                    tile.WallType = WallID.CrimsonGrassUnsafe;
                                }

                                caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                                caves1.SetFrequency(0.01f);
                                caves1.SetFractalType(FastNoiseLite.FractalType.None);
                                caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                                caves1.SetCellularJitter(0.5f);

                                float _oesophagi = caves1.GetNoise(i - (Corruption.X + 0.5f) * CellSize, 0) + 1;

                                int stomachMiddle = (Terrain.Maximum + (int)Main.worldSurface) / 2;
                                float stomachWidth = 0.55f - (float)MiscTools.LessSmoothStep(0, 1, Math.Clamp(1 - MathHelper.Distance(j, stomachMiddle) / 45, 0, 1)) * 0.6f + Math.Clamp(1 - MathHelper.Distance(j, Math.Clamp(j, (int)Main.worldSurface - 10, (int)Main.rockLayer)) / 5, 0, 1);

                                if (_oesophagi > stomachWidth && layer < surfaceLayer)
                                {
                                    if (tile.HasTile)
                                    {
                                        tile.WallType = WorldGen.genRand.NextBool(2) ? WallID.CrimsonUnsafe3 : WallID.CrimstoneUnsafe;
                                    }
                                    if ((float)i / CellSize > Corruption.Left + 0.5f && (float)i / CellSize < Corruption.Right + 0.5f)
                                    {
                                        tile.TileType = TileID.Crimstone;
                                    }

                                    if (_oesophagi > stomachWidth + 0.2f && MathHelper.Distance(i, Math.Clamp(i, (Corruption.Left + 1) * CellSize, Corruption.Right * CellSize)) < 1)
                                    {
                                        tile.HasTile = false;
                                        if (j >= Terrain.Maximum)
                                        {
                                            tile.LiquidAmount = (byte)WorldGen.genRand.Next(256);
                                        }
                                    }
                                }
                                else if (j > stomachMiddle && (float)i / CellSize > Corruption.Left + 0.5f && (float)i / CellSize < Corruption.Right + 0.5f)
                                {
                                    tile.TileType = TileID.Crimstone;
                                    tile.WallType = WallID.CrimstoneUnsafe;
                                }
                            }
                            else
                            {
                                tile.HasTile = true;

                                if (i / CellSize > Corruption.Left && i / CellSize < Corruption.Right)
                                {
                                    caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                                    caves1.SetFrequency(0.02f);
                                    caves1.SetFractalType(FastNoiseLite.FractalType.None);
                                    caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
                                    caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                                    caves1.SetCellularJitter(0.75f);

                                    caves2.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                                    caves2.SetFrequency(0.1f);
                                    caves2.SetFractalType(FastNoiseLite.FractalType.None);
                                    caves2.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                                    caves2.SetCellularJitter(0.75f);

                                    float mult = Math.Clamp(MathHelper.Distance(j, (int)Main.worldSurface) / 10, 0, 1);
                                    float orbDistance = Vector2.Distance(new Vector2(i, j), new Vector2(Corruption.orbX, WorldGen.crimson ? Corruption.orbYPrimary : Corruption.orbYSecondary));

                                    tile.LiquidAmount = 0;

                                    float _organs = caves1.GetNoise(i, j);
                                    if (_organs < -0.15f || _organs > 0.15f)
                                    {
                                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);

                                        float _organBorders = caves1.GetNoise(i, j) * Math.Clamp(MathHelper.Distance(j, (int)Main.worldSurface) / 20f, 0, 1);
                                        float _borderMult = Math.Clamp(MathHelper.Distance(i, (Corruption.Left + 1) * CellSize) / 20f, 0, 1) * Math.Clamp(MathHelper.Distance(i, (Corruption.Right) * CellSize) / 20f, 0, 1);

                                        if (_organBorders > -0.5f)
                                        {
                                            tile.TileType = TileID.Crimstone;
                                            tile.WallType = WallID.CrimstoneUnsafe;
                                        }

                                        if (_organBorders > -0.25f)
                                        {
                                            tile.HasTile = false;
                                        }
                                        else
                                        {
                                            _organBorders *= _borderMult;

                                            if (_organBorders < -0.5f)
                                            {
                                                if (_organs > 0)
                                                {
                                                    tile.TileType = TileID.Crimstone;
                                                    tile.WallType = WallID.CrimstoneUnsafe;

                                                    if (caves2.GetNoise(i, j) < -0.6f)
                                                    {
                                                        tile.HasTile = false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (caves2.GetNoise(i, j) > -0.6f && (_organs < -0.5f || _organs > 0.5f))
                                                    {
                                                        tile.HasTile = false;
                                                    }
                                                }
                                            }

                                            if (_organs < -0.5f || _organs > 0.5f)
                                            {
                                                if (_organBorders < -0.4f)
                                                {
                                                    tile.TileType = TileID.FleshBlock;
                                                    //tile.TileColor = PaintID.DeepRedPaint;
                                                    tile.WallType = WorldGen.genRand.NextBool(10) ? WallID.CrimsonUnsafe4 : WallID.CrimsonUnsafe2;
                                                    tile.WallColor = PaintID.DeepRedPaint;
                                                    tile.LiquidAmount = (byte)WorldGen.genRand.Next(byte.MaxValue);

                                                    //caves2.SetFrequency(0.1f);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tile.HasTile = false;
                                        tile.WallType = WallID.CrimstoneUnsafe;
                                    }
                                }
                                else if ((float)i / CellSize > Corruption.Left + 0.75f && (float)i / CellSize < Corruption.Right + 0.25f)
                                {
                                    tile.TileType = TileID.Crimstone;
                                    tile.WallType = WallID.CrimstoneUnsafe;
                                }
                            }

                            if (layer < surfaceLayer && MiscTools.Tile(x - 1, y - 1).TileType == TileID.Dirt && !MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.CrimsonGrass;
                            }
                        }
                    }
                    #region unused
                    //else if (biomes.Contains("meadow") && FindBiome(x, y) == "meadow")
                    //{
                    //    if (WGTools.Tile(x, y).WallType == WallID.GrassUnsafe)
                    //    {
                    //        WGTools.Tile(x, y).WallType = WallID.FlowerUnsafe;
                    //    }
                    //    if (WGTools.Tile(x, y).TileType == TileID.Vines)
                    //    {
                    //        WGTools.Tile(x, y).TileType = TileID.VineFlowers;
                    //    }
                    //    if (!WGTools.Tile(x, y).HasTile || WGTools.Tile(x, y).TileType == TileID.Plants || WGTools.Tile(x, y).TileType == TileID.Plants2)
                    //    {
                    //        WorldGen.KillTile((int)x, (int)y);
                    //        if (RemTile.SolidTop((int)x, (int)y + 1) && WGTools.Tile(x, y + 1).TileType == TileID.Grass && tile.LiquidAmount == 0)
                    //        {
                    //            //if (WorldGen.genRand.NextBool(30))
                    //            //{
                    //            //    WorldGen.PlaceObject(x, y, TileID.DyePlants, style: 4);
                    //            //}
                    //            //else
                    //            {
                    //                if (WorldGen.genRand.NextBool(25))
                    //                {
                    //                    WorldGen.PlaceObject((int)x, (int)y, TileID.DyePlants, style: WorldGen.genRand.Next(8, 12));
                    //                }
                    //                else if (WorldGen.genRand.NextBool(2))
                    //                {
                    //                    float _flowers = flowers.GetNoise(x, y);
                    //                    if (_flowers < -0.1f)
                    //                    {
                    //                        WorldGen.PlaceObject((int)x, (int)y, TileID.BloomingHerbs, style: 0);
                    //                    }
                    //                    else if (_flowers > 0.1f && WorldGen.genRand.NextBool(2))
                    //                    {
                    //                        WorldGen.PlaceObject((int)x, (int)y, TileID.DyePlants, style: 3);
                    //                    }
                    //                }
                    //                if (!tile.HasTile)
                    //                {
                    //                    tile.HasTile = true;
                    //                    tile.TileType = TileID.Plants2;
                    //                    int style = Main.rand.Next(7, 21);
                    //                    if (style == 8)
                    //                    {
                    //                        style = 6;
                    //                    }
                    //                    tile.TileFrameX = (short)(style * 18);
                    //                    tile.TileFrameY = 0;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion
                    #region underground
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Glowshroom))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.01f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.7f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);

                        if (GetTileDistribution(x, y, frequency: 2) >= 0f)
                        {
                            tile.TileType = TileID.Mud;
                        }
                        else tile.TileType = TileID.Stone;

                        //if (tile.TileType == TileID.Grass || WorldGen.genRand.NextBool(25) || FindBiome(x + 1, y + 1) != BiomeID.Glowshroom && !WGTools.Solid(x + 1, y + 1))
                        //{
                        //    tile.TileType = TileID.MushroomGrass;
                        //}
                        //else tile.TileType = TileID.Mud;

                        float _caves = caves1.GetNoise(x, y * 2); //+((float)Math.Cos(x / 60) * 20)

                        float _layering = (float)Math.Cos(MathHelper.Pi * ((-y / 25f + 0.5f) % 1)) * 0.2f;

                        //shroomcaves.SetFractalPingPongStrength(shroomcaves2.GetNoise(x, y * 2) + 1.625f);
                        //float _caves = shroomcaves.GetNoise(x, y * 2);
                        //float _size = (shroomcaves3.GetNoise(x, y * 2) / 2) + 1;

                        if (_caves + _layering < -0.25f && FindBiome(x, y + 2) != BiomeID.Marble && FindBiome(x, y - 2) != BiomeID.Marble)
                        {
                            if (FindBiome(x, y - 2) == BiomeID.Glowshroom && FindBiome(x, y + 2) == BiomeID.Glowshroom && FindBiome(x - 2, y) == BiomeID.Glowshroom && FindBiome(x + 2, y) == BiomeID.Glowshroom)
                            {
                                tile.HasTile = false;
                                if (WorldGen.genRand.NextBool(25))
                                {
                                    tile.LiquidAmount = 255;
                                }
                                else tile.LiquidAmount = 0;
                            }
                        }
                        else
                        {
                            tile.HasTile = true;
                            tile.Slope = 0;
                        }

                        if (caves1.GetNoise(x + 999, y * 2 + 999) + _layering > -0.3f)
                        {
                            tile.WallType = WallID.MushroomUnsafe;
                        }
                        else tile.WallType = 0;
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.SulfuricVents))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.05f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.1f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);

                        bool doCaves = FindBiome(x, y - 2) == BiomeID.SulfuricVents && FindBiome(x, y + 2) == BiomeID.SulfuricVents;

                        if (GetTileDistribution(x, y) >= 0.2f)
                        {
                            tile.TileType = TileID.Silt;
                            if (doCaves || tile.WallType != 0)
                            {
                                tile.WallType = WallID.LavaUnsafe1;
                            }
                        }
                        else
                        {
                            float _gems = gems.GetNoise(x * 3, y * 3) + caves2.GetNoise(x, y) * 0.02f;

                            if (GetTileDistribution(x, y) < 0f && _gems < -0.98f)
                            {
                                tile.TileType = (ushort)ModContent.TileType<SulfurstoneDiamond>();
                            }
                            else tile.TileType = (ushort)ModContent.TileType<Sulfurstone>();
                            if (doCaves || tile.WallType != 0)
                            {
                                tile.WallType = (ushort)ModContent.WallType<SulfurstoneWall>();
                            }
                        }

                        if (doCaves)
                        {
                            float _caves = caves1.GetNoise(x, y) + caves2.GetNoise(x, y) * 0.025f;

                            if (_caves < -0.8f)
                            {
                                tile.HasTile = false;
                                tile.LiquidType = LiquidID.Lava;

                                if (WorldGen.genRand.NextBool(25))
                                {
                                    tile.LiquidAmount = 255;
                                }

                                if (_caves < -0.875f)
                                {
                                    tile.WallType = 0;
                                }
                            }
                            else tile.HasTile = true;
                        }

                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Marble))
                    {
                        tile.TileType = TileID.Marble;
                        tile.HasTile = true;

                        tile.LiquidType = 0;

                        if (FindBiome(x, y - 1) == BiomeID.Marble && FindBiome(x, y + 1) == BiomeID.Marble)
                        {
                            tile.WallType = WallID.MarbleUnsafe;
                        }

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
                        caves1.SetFrequency(0.02f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalOctaves(4);

                        Vector2 point = new Vector2(MathHelper.Clamp(x, Main.maxTilesX * 0.4f + 50, Main.maxTilesX * 0.6f - 50), (MarbleCave.Y + 0.5f) * CellSize);
                        float threshold = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(x, y), point) / 80, 0, 1) * 2 - 1;

                        if (caves1.GetNoise(x * 3, y) * 2f < threshold)
                        {
                            MiscTools.Tile(x, y).HasTile = false;

                            if (y > point.Y + 32)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 153;
                            }
                            else MiscTools.Tile(x, y).LiquidAmount = 0;
                        }
                        if (caves1.GetNoise(x * 3, y) * 3f + 0.35f < threshold)
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Granite))
                    {
                        tile.TileType = TileID.Granite;
                        tile.HasTile = true;

                        tile.LiquidType = 0;

                        tile.WallType = WallID.GraniteUnsafe;

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Sub);
                        caves1.SetFrequency(0.025f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalOctaves(2);

                        Vector2 point = new Vector2((Tundra.Center + 0.5f) * CellSize, MathHelper.Clamp(y, (int)Main.worldSurface + 100, Main.maxTilesY - 350));
                        float threshold = MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y), point) / 75) * 1.5f, 0, 1);

                        if (caves1.GetNoise(x * 2, y) + 1.5f < threshold * 2 - 1)
                        {
                            MiscTools.Tile(x, y).HasTile = false;

                            if (Vector2.Distance(new Vector2(x, y), point) > 28)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                            else MiscTools.Tile(x, y).LiquidAmount = 0;
                        }

                        point.Y = MathHelper.Clamp(point.Y, 0, Main.maxTilesY - 450);
                        threshold = MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y), point) / 75) * 1.5f, 0, 1);

                        if (caves1.GetNoise(x * 2, y) + 1.75f < threshold * 2 - 1)
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }

                        if (i >= (Tundra.Center + 0.5f) * CellSize - 8 && i <= (Tundra.Center + 0.5f) * CellSize + 8)
                        {
                            if (layer <= surfaceLayer + 1 && i >= (Tundra.Center + 0.5f) * CellSize - 4 && i <= (Tundra.Center + 0.5f) * CellSize + 4)
                            {
                                tile.HasTile = false;
                            }
                        }
                        else if (FindBiome(x, y - WorldGen.genRand.Next(2, 4)) == BiomeID.Tundra)
                        {
                            tile.TileType = TileID.IceBlock;
                            if (tile.WallType != 0)
                            {
                                tile.WallType = WallID.IceUnsafe;
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.SpiderNest))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.025f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalOctaves(5);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves2.SetFrequency(0.1f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.None);

                        float _caves = caves1.GetNoise(x, y);
                        float _eggs = caves2.GetNoise(x, y);

                        if (_eggs < -0.95f)
                        {
                            //_caves += 0.1f;
                            _caves *= 1 + (0.9f + _eggs) * 5;
                            //_caves -= 0.1f;
                        }

                        bool noEdges = FindBiome(x, y - 2) == BiomeID.SpiderNest && FindBiome(x, y + 2) == BiomeID.SpiderNest && FindBiome(x - 2, y) == BiomeID.SpiderNest && FindBiome(x + 2, y) == BiomeID.SpiderNest;

                        if (noEdges || FindBiome(x, y + 2) == BiomeID.Marble || FindBiome(x, y - 2) == BiomeID.Marble)
                        {
                            tile.HasTile = true;
                        }
                        tile.LiquidAmount = 0;

                        if (_caves > -0.25f && _caves < 0.25f)
                        {
                            if (_eggs < -0.9f)
                            {
                                tile.TileType = (ushort)ModContent.TileType<SpiderEggs>();
                            }
                            else tile.TileType = (ushort)ModContent.TileType<InsectRemains>();

                            tile.WallType = WallID.SpiderUnsafe;

                            if (noEdges)
                            {
                                if (_caves > -0.1f && _caves < 0.1f)
                                {
                                    tile.HasTile = false;
                                }
                            }
                        }
                        else if (GetTileDistribution(x, y) >= 0.2f)
                        {
                            tile.TileType = TileID.Dirt;
                            tile.WallType = WallID.CaveWall2;
                        }
                        else
                        {
                            tile.TileType = TileID.Stone;
                            tile.WallType = layer >= lavaLayer ? WallID.Cave8Unsafe : WallID.RocksUnsafe1;
                        }

                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.OceanCave))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.04f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalOctaves(3);
                        //caves.SetFractalGain(1);
                        caves1.SetFractalLacunarity(2);
                        //caves.SetFractalWeightedStrength(-0.45f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Sub);

                        if (layer > surfaceLayer)
                        {
                            caves2.SetNoiseType(FastNoiseLite.NoiseType.Value);
                            caves2.SetFrequency(0.01f);
                            caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                            caves2.SetFractalOctaves(3);

                            float _caves = caves1.GetNoise(x, y);

                            if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Grass || tile.TileType == TileID.ClayBlock || tile.TileType == TileID.Stone || tile.TileType == TileID.Silt)
                            {
                                if (GetTileDistribution(x, y, frequency: 2) < 0f)
                                {
                                    if (WorldGen.genRand.NextBool(10))
                                    {
                                        tile.TileType = TileID.ArgonMoss;
                                    }
                                    else tile.TileType = TileID.Stone;
                                }
                                else //if (MaterialBlend(x, y, frequency: 2) <= 0f)
                                {
                                    tile.TileType = TileID.Coralstone;// Stone;
                                }
                                //else tile.TileType = TileID.Sand;
                            }
                            if (_caves < 0)
                            {
                                tile.HasTile = true;
                                tile.Slope = SlopeType.Solid;
                            }
                            else tile.HasTile = false;
                            //if (MaterialBlend(x, y, true, 2) <= -0.2f)
                            //{
                            //    tile.HasTile = true;
                            //    WGTools.GetTile(x, y).TileType = TileID.Coralstone;
                            //}
                            MiscTools.Tile(x, y).LiquidType = 0;
                            MiscTools.Tile(x, y).LiquidAmount = 255;

                            if (!MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Stone)
                                {
                                    MiscTools.Tile(x - 1, y - 1).TileType = TileID.ArgonMoss;
                                }
                            }
                        }
                        else
                        {
                            tile.HasTile = false;
                            MiscTools.Tile(x, y).LiquidType = 0;
                            if (y >= Main.worldSurface - 60)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                        }
                        if (tile.WallType != 0 || j > Main.worldSurface)
                        {
                            float _background = caves1.GetNoise(x + 999, y * 2 + 999);

                            if (_background < 0)
                            {
                                if (GetTileDistribution(x, y, frequency: 4) < 0)
                                {
                                    tile.WallType = WallID.HallowUnsafe4;
                                }
                                else if (x < Main.maxTilesX / 2)
                                {
                                    tile.WallType = WallID.RocksUnsafe4;
                                }
                                else tile.WallType = WallID.RocksUnsafe3;
                            }
                            else if (y > Main.worldSurface)
                            {
                                tile.WallType = 0;
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Toxic))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves1.SetFrequency(0.02f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.6f);

                        if (GetTileDistribution(x, y, true) <= -0.25f)
                        {
                            MiscTools.Tile(x, y).TileType = (ushort)ModContent.TileType<ToxicWaste>();
                        }
                        else MiscTools.Tile(x, y).TileType = TileID.Stone;

                        float _caves = caves1.GetNoise(x, y * 2);

                        if (_caves < -0.05f)
                        {
                            tile.HasTile = false;
                            if (WorldGen.genRand.NextBool(15))
                            {
                                tile.LiquidAmount = 255;
                            }
                            else tile.LiquidAmount = 0;
                        }
                        else
                        {
                            tile.HasTile = true;
                            tile.Slope = 0;
                        }

                        if (caves1.GetNoise(x + 999, y * 2 + 999) < -0.1f)
                        {
                            tile.WallType = WallID.JungleUnsafe3;
                        }

                        if (!MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Stone)
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.KryptonMoss;
                            }
                            else if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Mud)
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.JungleGrass;
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Underworld))
                    {
                        background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        background.SetFrequency(0.025f);
                        background.SetFractalType(FastNoiseLite.FractalType.Ridged);
                        background.SetFractalWeightedStrength(-0.5f);
                        background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                        background.SetFractalOctaves(2);

                        tile.TileType = TileID.Ash;

                        if (MiscTools.SurroundingTilesActive(x, y))
                        {
                            if (MiscTools.Tile(x, y).TileType == TileID.Ash)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.LavaUnsafe2;
                            }
                        }

                        if (tile.WallType == WallID.LavaUnsafe2)
                        {
                            tile.WallColor = PaintID.BlackPaint;
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Aether))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.04f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.Ridged);

                        float _caves = caves1.GetNoise(x, y); //+((float)Math.Cos(x / 60) * 20)

                        //shroomcaves.SetFractalPingPongStrength(shroomcaves2.GetNoise(x, y * 2) + 1.625f);
                        //float _caves = shroomcaves.GetNoise(x, y * 2);
                        //float _size = (shroomcaves3.GetNoise(x, y * 2) / 2) + 1;

                        tile.LiquidType = LiquidID.Shimmer;

                        if (_caves < 0.6f)
                        {
                            if (FindBiome(x, y - 2) == BiomeID.Aether && FindBiome(x, y + 2) == BiomeID.Aether && FindBiome(x - 2, y) == BiomeID.Aether && FindBiome(x + 2, y) == BiomeID.Aether)
                            {
                                tile.HasTile = false;
                                if (WorldGen.genRand.NextBool(5))
                                {
                                    tile.LiquidAmount = 255;
                                }
                                else tile.LiquidAmount = 0;
                            }
                        }
                        else
                        {
                            tile.HasTile = true;
                            tile.Slope = 0;
                        }

                        //float _materials = MaterialBlend(x, y, true, frequency: 3);

                        if (_caves >= 0.85f)
                        {
                            MiscTools.Tile(x, y).TileType = TileID.ShimmerBlock;
                        }
                        else MiscTools.Tile(x, y).TileType = TileID.Stone;

                        if (!MiscTools.SurroundingTilesActive(x - 1, y - 1))
                        {
                            if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Stone)
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.VioletMoss;
                            }
                        }

                        if (caves1.GetNoise(x + 999, y + 999) > 0.6f)
                        {
                            //if (_materials < -0.2f)
                            //{
                            //    tile.WallType = WallID.ShimmerBlockWall;
                            //}
                            //else
                            tile.WallType = WallID.HallowUnsafe1;
                            tile.WallColor = PaintID.PurplePaint;
                        }
                        else tile.WallType = 0;
                    }
                    #endregion
                    #region modded
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.SunkenSea))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.01f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves2.SetFrequency(0.01f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves2.SetFractalOctaves(2);

                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.05f);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        bool doCaves = FindBiome(i, j + 25, false) == BiomeID.SunkenSea;

                        float _caves = caves1.GetNoise(x, y * 2);
                        float _prisms = caves2.GetNoise(x + caves3.GetNoise(x, y * 2 + 999) * 10, y * 2 + caves3.GetNoise(x + 999, y * 2) * 5);

                        tile.HasTile = true;
                        if (doCaves && GetTileDistribution(x, y) >= -0.1f)
                        {
                            if (_caves < -0.05f || _caves > 0.05f)
                            {
                                tile.TileType = eutrophicSand;
                            }
                            else
                            {
                                tile.TileType = hardenedEutrophicSand;
                            }
                            tile.WallType = eutrophicSandWall;
                        }
                        else
                        {
                            tile.TileType = navystone;
                            tile.WallType = navystoneWall;
                        }

                        if (doCaves)
                        {
                            if (_caves < -0.25f || _caves > 0.25f || _prisms < -0.85f)
                            {
                                if (_caves < -0.3f || _caves > 0.3f)
                                {
                                    if (_caves < -0.35f || _caves > 0.35f)
                                    {
                                        tile.WallType = 0;
                                    }
                                    else tile.WallType = navystoneWall;
                                }
                                if (_prisms < -0.925f && _prisms > -0.98f)
                                {
                                    if (_prisms < -0.97f)
                                    {
                                        tile.TileType = seaPrism;
                                    }
                                    else tile.TileType = navystone;
                                }
                                else
                                {
                                    tile.HasTile = false;
                                    tile.LiquidAmount = 255;
                                }
                                if (_prisms < -0.925f)
                                {
                                    tile.WallType = navystoneWall;
                                }
                            }

                            if (!MiscTools.Tile(x, y).HasTile)
                            {
                                if (MiscTools.Tile(x, y - 1).HasTile)
                                {
                                    for (int k = 1; k < WorldGen.genRand.Next(4, 6); k++)
                                    {
                                        if (MiscTools.Tile(x, y - k).HasTile && MiscTools.Tile(x, y - k).TileType != seaPrism && FindBiome(x, y - k) == BiomeID.SunkenSea)
                                        {
                                            MiscTools.Tile(x, y - k).TileType = navystone;
                                        }
                                    }
                                }
                            }

                            if (!MiscTools.Tile(x - 1, y - 1).HasTile)
                            {
                                if (WorldGen.genRand.NextBool(8) && (MiscTools.SolidTileOf((int)x - 2, (int)y - 1, navystone) || MiscTools.SolidTileOf((int)x, (int)y - 1, navystone) || MiscTools.SolidTileOf((int)x - 1, (int)y - 2, navystone) || MiscTools.SolidTileOf((int)x - 1, (int)y, navystone)) || MiscTools.SolidTileOf((int)x - 2, (int)y - 1, seaPrism) || MiscTools.SolidTileOf((int)x, (int)y - 1, seaPrism) || MiscTools.SolidTileOf((int)x - 1, (int)y - 2, seaPrism) || MiscTools.SolidTileOf((int)x - 1, (int)y, seaPrism))
                                {
                                    WorldGen.PlaceTile((int)x - 1, (int)y - 1, prismShard);

                                    MiscTools.Tile(x - 1, y - 1).TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                                    MiscTools.Tile(x - 1, y - 1).TileFrameY = (short)((WorldGen.SolidTile((int)x - 1, (int)y) ? 0 : WorldGen.SolidTile((int)x - 1, (int)y - 2) ? 1 : WorldGen.SolidTile((int)x, (int)y - 1) ? 2 : 3) * 18);
                                }
                            }
                            if (!MiscTools.Tile(x - 1, y - 1).HasTile && WorldGen.genRand.NextBool(2) && MiscTools.SolidTileOf((int)x - 1, (int)y - 2, navystone))
                            {
                                WorldGen.PlaceTile((int)x - 1, (int)y - 1, stalactite);
                                MiscTools.Tile(x - 1, y - 1).TileFrameX = (short)(WorldGen.genRand.Next(3) * 18);
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.AquaticDepths))
                    {
                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.04f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalOctaves(3);
                        //caves.SetFractalGain(1);
                        caves1.SetFractalLacunarity(2);
                        //caves.SetFractalWeightedStrength(-0.45f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Sub);

                        bool inner = layer < caveLayer && (i / CellSize > 0 && i / CellSize < 6 || i / CellSize < Width && i / CellSize > Width - 7);

                        tile.HasTile = true;
                        //tile.Slope = SlopeType.Solid;

                        tile.WallType = marineWall;

                        if (GetTileDistribution(x, y + 111) >= 0.4f)
                        {
                            tile.TileType = brack;
                        }
                        else if (inner && GetTileDistribution(x, y + 111) < -0.5f)
                        {
                            tile.TileType = aquaite;
                        }
                        else if (GetTileDistribution(x, y) >= 0.1f)
                        {
                            tile.TileType = leakyMossyMarine;
                        }
                        else if (inner && GetTileDistribution(x, y) < -0.35f && WorldGen.genRand.NextBool(2))
                        {
                            tile.TileType = aquamarine;
                        }
                        else
                        {
                            tile.TileType = leakyMarine;
                        }


                        if (layer > surfaceLayer)
                        {
                            if (inner)
                            {
                                float _caves = caves1.GetNoise(x, y);

                                if (_caves > 0f)
                                {
                                    tile.HasTile = false;
                                }
                                //if (MaterialBlend(x, y, true, 2) <= -0.2f)
                                //{
                                //    tile.HasTile = true;
                                //    WGTools.GetTile(x, y).TileType = TileID.Coralstone;
                                //}
                                MiscTools.Tile(x, y).LiquidType = 0;
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                        }
                        else
                        {
                            tile.HasTile = false;
                            MiscTools.Tile(x, y).LiquidType = 0;
                            if (y >= Main.worldSurface - 60)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                        }
                        if (tile.WallType != 0 || j > Main.worldSurface)
                        {
                            float _background = caves1.GetNoise(x + 999, y * 2 + 999);

                            if (_background < 0)
                            {
                                tile.WallType = marineWall;
                            }
                            else if (y > Main.worldSurface)
                            {
                                tile.WallType = 0;
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.Savanna))
                    {
                        MiscTools.Tile(x, y).TileType = TileID.HardenedSand;
                        if (tile.WallType != 0)
                        {
                            tile.WallType = WallID.HardenedSand;
                        }

                        for (int k = 1; k <= WorldGen.genRand.Next(8, 10); k++)
                        {
                            if (!MiscTools.Tile(x, y - k).HasTile)
                            {
                                tile.TileType = savannaDirt;
                                if (tile.WallType != 0)
                                {
                                    tile.WallType = savannaDirtWall;
                                }
                                break;
                            }
                        }
                    }
                    else if (IsUpdatingBiome(x, y, biomesToUpdate, BiomeID.ThermalCaves))
                    {
                        if (GetTileDistribution(x, y, frequency: 2) >= 0.3f)
                        {
                            tile.TileType = TileID.Ash;
                        }
                        else if (GetTileDistribution(x, y) >= 0.2f)
                        {
                            tile.TileType = basalt;
                        }
                        else tile.TileType = gabbro;

                        tile.LiquidType = LiquidID.Lava;
                        if (!tile.HasTile)
                        {
                            if (WorldGen.genRand.NextBool(25))
                            {
                                tile.LiquidAmount = 255;
                            }
                        }

                        if (tile.WallType != 0)
                        {
                            if (GetTileDistribution(x, y) >= 0.2f)
                            {
                                tile.WallType = basaltWall;
                            }
                            else tile.WallType = gabbroWall;
                        }
                    }
                    #endregion

                    if (y >= Main.maxTilesY - 20)
                    {
                        break;
                    }
                    //if (biomes.Contains(FindBiome(x, y, false)) || biomes.Contains(FindBiomeFast(x + scale / 2, y)) || biomes.Contains(FindBiomeFast(x - scale / 2, y)) || biomes.Contains(FindBiomeFast(x, y + scale / 2)) || biomes.Contains(FindBiomeFast(x, y - scale / 2)))
                }
            }
        }
    }

    public class BiomeGeneration : GenPass
    {
        public BiomeGeneration(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            bool spiritReforged = ModLoader.TryGetMod("SpiritReforged", out Mod sr);
            bool lunarVeil = ModLoader.TryGetMod("Stellamod", out Mod lv);


            biomes.UpdateMap(new int[] { BiomeID.Tundra, BiomeID.Jungle, BiomeID.Desert, BiomeID.Corruption, BiomeID.Crimson, BiomeID.Underworld, BiomeID.SulfuricVents, BiomeID.Beach, BiomeID.Toxic, BiomeID.Glowshroom, BiomeID.Marble, BiomeID.Granite, BiomeID.SpiderNest, BiomeID.Aether, BiomeID.OceanCave, BiomeID.SunkenSea, BiomeID.AquaticDepths, BiomeID.Savanna, BiomeID.ThermalCaves, BiomeID.Abysm }, progress);

            #region tundra
            FastNoiseLite thinIce = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            thinIce.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            thinIce.SetFrequency(0.2f);
            thinIce.SetFractalType(FastNoiseLite.FractalType.FBm);
            thinIce.SetFractalOctaves(2);

            int attempts = 0;
            while (attempts < 10000)
            {
                int x = WorldGen.genRand.Next((Tundra.Left + 1) * biomes.CellSize, Tundra.Right * biomes.CellSize);
                int y = WorldGen.genRand.Next(Terrain.Minimum, Tundra.Bottom * biomes.CellSize);

                attempts++;

                int width = WorldGen.genRand.Next(4, 17);

                if (MiscTools.SolidInArea(x - width - 1, y, x + width + 1, y) && biomes.FindBiome(x - 50, y) == BiomeID.Tundra && biomes.FindBiome(x + 50, y) == BiomeID.Tundra && biomes.FindBiome(x, y + 50) == BiomeID.Tundra)
                {
                    int top = y;
                    while (!MiscTools.NonSolidInArea(x - width, top, x + width, top) && y > Terrain.Minimum - 25)
                    {
                        top--;
                    }
                    int bottom = y;
                    while (!MiscTools.NonSolidInArea(x - width, bottom, x + width, bottom) && biomes.FindBiome(x, y, false) == BiomeID.Tundra)
                    {
                        bottom++;
                    }

                    int padding = 10;
                    Rectangle rect = new Rectangle(x - width - padding, top, width * 2 + 1 + padding * 2, bottom - top);

                    if (bottom - top < 30 && GenVars.structures.CanPlace(rect))
                    {
                        GenVars.structures.AddProtectedStructure(new Rectangle(x - width, top, width * 2 + 1, bottom - top));

                        for (int j = top; j <= bottom; j++)
                        {
                            for (int i = x - width - (int)(thinIce.GetNoise(x, j) * 5); i <= x + width + (int)(thinIce.GetNoise(-x, j) * 5); i++)
                            {
                                Tile tile = Main.tile[i, j];
                                if (tile.HasTile && (tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBlock))
                                {
                                    tile.TileType = TileID.BreakableIce;
                                }
                            }
                        }

                        attempts = 0;
                    }
                }
            }
            #endregion

            #region corruption
            int orbs = Main.maxTilesX / 2100 * Main.maxTilesY / 600;
            while (orbs > 0)
            {
                #region spawnconditions
                int x = Corruption.orbX + WorldGen.genRand.Next(-(int)(Main.maxTilesX / 4200f * 48), (int)(Main.maxTilesX / 4200f * 48));
                int orbY = (!WorldGen.crimson ? Corruption.orbYPrimary : Corruption.orbYSecondary);
                int y = orbY + WorldGen.genRand.Next(-(int)(Main.maxTilesX / 4200f * 96), (int)(Main.maxTilesX / 4200f * 96));

                int radius = 10;

                bool valid = true;

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true);
                if (y < Main.worldSurface)
                {
                    valid = false;
                }
                else
                {
                    for (int j = (int)(y - radius * 3); j <= y + radius * 3; j++)
                    {
                        for (int i = (int)(x - radius * 3); i <= x + radius * 3; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) < radius * 3)
                            {
                                if ( biomes.FindBiome(i, j) != BiomeID.Corruption)
                                {
                                    valid = false;
                                }
                                else if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) < radius)
                                {
                                    if (!WorldGen.SolidTile3(i, j))
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
                    valid = false;

                    for (int j = (int)(y - radius); j <= y + radius; j++)
                    {
                        for (int i = (int)(x - radius); i <= x + radius; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) > radius && Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) < radius * 2)
                            {
                                if (!WorldGen.SolidTile3(i, j))
                                {
                                    valid = true;
                                }
                            }
                        }
                    }
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(new Rectangle((int)(x - radius), (int)(y - radius), (int)(radius * 2), (int)(radius * 2)));

                    FastNoiseLite noise = new FastNoiseLite();
                    noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                    noise.SetFrequency(0.1f);
                    noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
                    noise.SetFractalOctaves(1);

                    for (int j = (int)(y - radius * 2); j <= y + radius * 2; j++)
                    {
                        for (int i = (int)(x - radius * 2); i <= x + radius * 2; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            float _noise = noise.GetNoise(i, j);
                            float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x + 0.5f, y + 0.5f));

                            if (_noise + 1 > distance / radius)
                            {
                                if (tile.TileType == TileID.Ebonstone || tile.TileType == TileID.Dirt)
                                {
                                    tile.TileType = TileID.LesionBlock;
                                }

                                if (distance - _noise * 2 + 2 < radius / 2)
                                {
                                    tile.HasTile = false;
                                    tile.LiquidAmount = (byte)WorldGen.genRand.Next(255);
                                }
                                else if (distance < radius)
                                {
                                    tile.HasTile = true;
                                }

                                tile.WallType = WallID.CorruptionUnsafe3;
                                tile.WallColor = PaintID.OrangePaint;
                            }
                        }
                    }

                    for (int j = y; j <= y + 1; j++)
                    {
                        for (int i = x; i <= x + 1; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            tile.TileType = TileID.ShadowOrbs;
                            tile.HasTile = true;
                            tile.TileFrameX = (short)((i - x) * 18);
                            tile.TileFrameY = (short)((j - y) * 18);
                            //if (WorldGen.crimson ^ alternate)
                            //{
                            //    tile.TileFrameX += 18 * 2;
                            //}
                        }
                    }

                    orbs--;
                }
            }

            orbs = Main.maxTilesX / 2100 * Main.maxTilesY / 300;
            while (orbs > 0)
            {
                #region spawnconditions
                int x = WorldGen.genRand.Next(Corruption.Left * biomes.CellSize, Corruption.Right * biomes.CellSize);
                int y = WorldGen.genRand.Next(WorldGen.crimson ? (int)Main.worldSurface : (biomes.caveLayer + Corruption.Size) * biomes.CellSize, WorldGen.crimson ? (biomes.caveLayer + Corruption.Size) * biomes.CellSize : Main.maxTilesY - 300);

                bool valid = true;
                if (MiscTools.Tile(x, y).WallType != WallID.CrimsonUnsafe2 && MiscTools.Tile(x, y).WallType != WallID.CrimsonUnsafe4 || !MiscTools.NonSolidInArea(x - 1, y - 1, x + 2, y + 2))
                {
                    valid = false;
                }
                for (int j = y - 4; j <= y + 5; j++)
                {
                    for (int i = x - 4; i <= x + 5; i++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (MiscTools.HasTile(i, j, TileID.ShadowOrbs))
                        {
                            valid = false;
                        }
                    }
                }
                #endregion

                if (valid && biomes.FindBiome(x, y + 50) == BiomeID.Crimson && biomes.FindBiome(x, y - 50) == BiomeID.Crimson)
                {
                    for (int j = y; j <= y + 1; j++)
                    {
                        for (int i = x; i <= x + 1; i++)
                        {
                            Tile tile = Main.tile[i, j];

                            tile.TileType = TileID.ShadowOrbs;
                            tile.HasTile = true;
                            tile.TileFrameX = (short)((i - x + 2) * 18);
                            tile.TileFrameY = (short)((j - y) * 18);
                        }
                    }

                    orbs--;
                }
            }

            int altars = Main.maxTilesX / 210;
            while (altars > 0)
            {
                #region spawnconditions
                int x = WorldGen.genRand.Next(Corruption.Left * biomes.CellSize, Corruption.Right * biomes.CellSize);
                int y = altars > Main.maxTilesX / 420 ? WorldGen.genRand.Next((int)Main.worldSurface, (int)(Main.maxTilesY - 300 - Main.worldSurface) / 2 + (int)Main.worldSurface) : WorldGen.genRand.Next((int)(Main.maxTilesY - 300 - Main.worldSurface) / 2 + (int)Main.worldSurface, Main.maxTilesY - 300);

                bool valid = true;

                if (MiscTools.Tile(x, y).TileType == TileID.DemonAltar || Main.wallDungeon[MiscTools.Tile(x, y).WallType])
                {
                    valid = false;
                }
                else if (biomes.FindBiome(x, y) != BiomeID.Corruption && biomes.FindBiome(x, y) != BiomeID.Crimson)
                {
                    valid = false;
                }
                else if (MiscTools.Tile(x, y + 1).TileType != TileID.Ebonstone && MiscTools.Tile(x, y + 1).TileType != TileID.Crimstone)
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    WorldGen.PlaceObject(x, y, TileID.DemonAltar, style: biomes.FindBiome(x, y) == BiomeID.Crimson ? 1 : 0);

                    if (MiscTools.Tile(x, y).TileType == TileID.DemonAltar)
                    {
                        altars--;
                    }
                }
            }
            #endregion

            #region glowshroom
            for (int y = 40; y < Main.maxTilesY - 200; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (biomes.FindBiome(x, y) == BiomeID.Glowshroom || biomes.FindBiome(x, y) == BiomeID.Jungle)
                    {
                        if (tile.TileType == TileID.Stone)
                        {
                            for (int k = 1; k <= WorldGen.genRand.Next(8, 10); k++)
                            {
                                if (!MiscTools.Tile(x, y - k).HasTile)
                                {
                                    tile.TileType = TileID.Mud;
                                    break;
                                }
                            }
                        }
                        if (tile.TileType == TileID.Stone)
                        {
                            for (int k = 1; k <= WorldGen.genRand.Next(2, 4); k++)
                            {
                                if (!MiscTools.Tile(x, y + k).HasTile)
                                {
                                    tile.TileType = TileID.Mud;
                                    break;
                                }
                            }
                        }

                        if (biomes.FindBiome(x, y) == BiomeID.Glowshroom && !MiscTools.SurroundingTilesActive(x, y, true))
                        {
                            if (tile.TileType == TileID.Mud || tile.TileType == TileID.MushroomBlock)
                            {
                                tile.TileType = TileID.MushroomGrass;
                            }
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Beach && biomes.GetLayer(x, y) < biomes.surfaceLayer)
                    {
                        if (!MiscTools.SurroundingTilesActive(x, y, true))
                        {
                            if (tile.TileType == TileID.Stone)
                            {
                                tile.TileType = TileID.GreenMoss;
                            }
                        }
                    }
                }
            }
            #endregion

            #region sunkensea
            if (ModContent.GetInstance<Worldgen>().SunkenSeaRework && ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                int area = GenVars.UndergroundDesertLocation.Width * (int)((GenVars.lavaLine - Main.rockLayer) / 2);

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
                    int objects = area / 1600;
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
                    objects = area / 800;
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

            Vector2 point;
            float threshold;
            FastNoiseLite terrain;

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Cleanup");

            //#region desertrocks
            //FastNoiseLite materials = new FastNoiseLite();
            //materials.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //materials.SetFrequency(0.15f);
            //materials.SetFractalType(FastNoiseLite.FractalType.FBm);

            //float transit1 = (int)Main.worldSurface - 50;
            //float transit2 = (int)Main.worldSurface;

            for (int y = 40; y < Main.worldSurface; y++)
            {
                progress.Set((y - 40) / (Main.worldSurface - 40));

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand)
                    {
                        if (biomes.FindBiome(x, y) == BiomeID.Desert && biomes.GetLayer(x, y) <= biomes.surfaceLayer)
                        {
                            if (tile.TileType == TileID.HardenedSand)
                            {
                                for (int i = 1; i <= WorldGen.genRand.Next(4, 7); i++)
                                {
                                    if (!MiscTools.Tile(x, y - i).HasTile)
                                    {
                                        tile.TileType = TileID.Sand;
                                        break;
                                    }
                                }
                            }
                            else if (tile.TileType == TileID.Sand)
                            {
                                for (int i = 1; i <= WorldGen.genRand.Next(4, 7); i++)
                                {
                                    if (!MiscTools.Tile(x, y + i).HasTile)
                                    {
                                        tile.TileType = TileID.HardenedSand;
                                        break;
                                    }
                                }
                            }

                            if (tile.HasTile && MiscTools.SurroundingTilesActive(x, y))
                            {
                                tile.WallType = WallID.HardenedSand;
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < 10; k++)
            {
                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = (Desert.Left - 1) * biomes.CellSize; x <= (Desert.Right + 2) * biomes.CellSize; x++)
                    {
                        if (!MiscTools.SurroundingTilesActive(x, y) && MiscTools.Tile(x, y).WallType == WallID.HardenedSand)
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

                            if (k == 9)
                            {
                                if (adjacentWalls < 3)
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
                    for (int x = (Desert.Left - 1) * biomes.CellSize; x <= (Desert.Right + 2) * biomes.CellSize; x++)
                    {
                        if (MiscTools.Tile(x, y).WallType == (ushort)ModContent.WallType<Walls.dev.nothing>())
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }
        }
    }

    public class SpecialTrees : GenPass
    {
        public SpecialTrees(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int attempts = 0;
            int trees = 0;
            while (trees < 8)
            {
                attempts++;

                if (attempts > 10000)
                {
                    break;
                }
                else
                {
                    int x = WorldGen.genRand.Next(Desert.OasisX - 60, Desert.OasisX + 61);
                    int y = (int)(Main.worldSurface * 0.5f);
                    while (!MiscTools.HasTile(x, y + 1, TileID.Sand) && y < Main.worldSurface)
                    {
                        y++;
                    }

                    if (!Main.tile[x, y].HasTile && MiscTools.HasTile(x, y + 1, TileID.Sand) && WorldGen.SolidTile3(x, y + 1) && y < Main.worldSurface)
                    {
                        WorldGen.PlaceTile(x, y, TileID.Saplings, style: 6);
                        WorldGen.AttemptToGrowTreeFromSapling(x, y, false);
                        //WorldGen.TryGrowingTreeByType(TileID.PalmTree, x, y + 1);

                        if (MiscTools.HasTile(x, y, TileID.PalmTree))
                        {
                            trees++;
                            attempts = 0;
                        }
                        else WorldGen.KillTile(x, y);
                    }
                }
            }

            for (int y = (int)(Main.rockLayer); y < Main.maxTilesY - 40; y++)
            {
                progress.Set((y - (int)Main.rockLayer) / (Main.maxTilesY - 200 - (int)Main.rockLayer));

                for (int x = 300; x < Main.maxTilesX - 300; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (!tile.HasTile && WorldGen.SolidTile3(x, y + 1))
                    {
                        //if (biomes.FindBiome(x, y) == BiomeID.GemCave)
                        //{
                        //    if (MiscTools.Tile(x, y + 1).TileType == TileID.Stone)
                        //    {
                        //        int gemType = RemTile.GetGemType(y);
                        //        ushort gemTree = gemType == 5 ? TileID.TreeDiamond : gemType == 4 ? TileID.TreeRuby : gemType == 3 ? TileID.TreeEmerald : gemType == 2 ? TileID.TreeSapphire : gemType == 1 ? TileID.TreeTopaz : TileID.TreeAmethyst;

                        //        ushort wallType = tile.WallType;
                        //        WorldGen.KillWall(x, y);
                        //        WorldGen.TryGrowingTreeByType(gemTree, x, y + 1);
                        //        tile.WallType = wallType;
                        //    }
                        //}
                        //else
                        if (y > Main.maxTilesY - 200)
                        {
                            if (MiscTools.Tile(x, y + 1).TileType == TileID.AshGrass && WorldGen.genRand.NextBool(4))
                            {
                                ushort wallType = tile.WallType;
                                WorldGen.KillWall(x, y);
                                WorldGen.TryGrowingTreeByType(TileID.TreeAsh, x, y + 1);
                                tile.WallType = wallType;
                            }
                        }
                    }
                }
            }
        }
    }

    public class SpecialPlants : GenPass
    {
        public SpecialPlants(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.SpecialPlants");

            for (int y = 40; y <= Main.maxTilesY - 200; y++)
            {
                progress.Set((float)((y - 40) / (float)(Main.maxTilesY - 200 - 40)));

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    #region plants
                    if (!tile.HasTile || Main.tileCut[tile.TileType])
                    {
                        if (Framing.GetTileSafely(x, y + 1).TileType == TileID.SnowBlock || Framing.GetTileSafely(x, y + 1).TileType == TileID.IceBlock)
                        {
                            if (tile.LiquidAmount == 255 && RemTile.SolidTop(x, y + 1) && WorldGen.genRand.NextBool(2))
                            {
                                int style = Main.rand.Next(9);
                                WorldGen.PlaceTile(x, y, ModContent.TileType<Cryocoral>());
                                Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                            }
                        }
                        else if (y > Main.worldSurface)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.SpiderNest)
                            {
                                if (biomes.FindBiome(x - 2, y) == BiomeID.SpiderNest && biomes.FindBiome(x + 2, y) == BiomeID.SpiderNest && biomes.FindBiome(x, y - 2) == BiomeID.SpiderNest && biomes.FindBiome(x, y + 2) == BiomeID.SpiderNest)
                                {
                                    if (WorldGen.genRand.NextBool(20) && MiscTools.HasTile(x, y, TileID.Cobweb))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<SpiderCocoon>();
                                    }
                                }
                            }
                            else if (RemTile.SolidTop(x, y + 1))
                            {
                                if (Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<Sulfurstone>() || Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<SulfurstoneDiamond>())
                                {
                                    if (!WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<SulfuricVent>());

                                        for (int j = y - 1; j > (int)Main.worldSurface && !Framing.GetTileSafely(x, j).HasTile; j--)
                                        {
                                            if ((WorldGen.genRand.NextBool(3) || y - j >= 5 || Framing.GetTileSafely(x, j - 2).HasTile))
                                            {
                                                break;
                                            }
                                            else WorldGen.PlaceTile(x, j, ModContent.TileType<SulfuricVent>());
                                        }
                                    }
                                }
                                else if (tile.LiquidType == 0)
                                {
                                    if (biomes.FindBiome(x, y) == BiomeID.OceanCave)
                                    {
                                        if (tile.LiquidAmount == 255)
                                        {
                                            if (MiscTools.Tile(x, y + 1).TileType == TileID.Coralstone)
                                            {
                                                if (WorldGen.genRand.NextBool(2))
                                                {
                                                    WorldGen.PlaceTile(x, y, ModContent.TileType<Luminsponge>(), style: Main.rand.Next(12));
                                                }
                                                else WorldGen.PlaceTile(x, y, TileID.Coral, style: Main.rand.Next(6));
                                            }
                                            else if (WorldGen.genRand.NextBool(15))
                                            {
                                                WorldGen.PlaceTile(x, y, TileID.DyePlants, style: 5);
                                            }
                                        }
                                    }
                                    else if (Framing.GetTileSafely(x, y + 1).TileType == TileID.JungleGrass)
                                    {
                                        if (WorldGen.genRand.NextBool(6))
                                        {
                                            WorldGen.PlaceTile(x, y, ModContent.TileType<PrismbudStem>());

                                            for (int j = y - 1; j > (int)Main.worldSurface && !Framing.GetTileSafely(x, j).HasTile; j--)
                                            {
                                                if ((WorldGen.genRand.NextBool(10) || y - j >= 10 || Framing.GetTileSafely(x, j - 2).HasTile) && Framing.GetTileSafely(x, j).LiquidAmount == 0)
                                                {
                                                    WorldGen.PlaceTile(x, j, ModContent.TileType<PrismbudHead>(), style: Main.rand.Next(3));

                                                    break;
                                                }
                                                else
                                                {
                                                    WorldGen.PlaceTile(x, j, ModContent.TileType<PrismbudStem>());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (RemTile.SolidBottom(x, y - 1))
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Aether)
                                {
                                    if (WorldGen.genRand.NextBool(4) && Framing.GetTileSafely(x, y - 1).TileType == TileID.VioletMoss && tile.LiquidAmount == 0)
                                    {
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<DreampodVine>());

                                        for (int j = y + 1; j < Main.maxTilesY - 300 && !Framing.GetTileSafely(x, j).HasTile && Framing.GetTileSafely(x, j).LiquidAmount == 0; j++)
                                        {
                                            if (WorldGen.genRand.NextBool(20) || j - y >= 20 || Framing.GetTileSafely(x, j + 2).HasTile)
                                            {
                                                WorldGen.PlaceTile(x, j, ModContent.TileType<Dreampod>(), style: Main.rand.Next(3));

                                                break;
                                            }
                                            else
                                            {
                                                WorldGen.PlaceTile(x, j, ModContent.TileType<DreampodVine>());
                                            }
                                        }
                                    }
                                }
                                else if (biomes.FindBiome(x, y) == BiomeID.Corruption)
                                {
                                    if (TileID.Sets.Corrupt[Framing.GetTileSafely(x, y - 1).TileType])
                                    {
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<SinewCorruption>());
                                    }
                                }
                                else if (biomes.FindBiome(x, y) == BiomeID.Crimson)
                                {
                                    if (TileID.Sets.Crimson[Framing.GetTileSafely(x, y - 1).TileType])
                                    {
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<SinewCrimson>());
                                    }
                                }
                            }
                        }
                        else if (RemTile.SolidTop(x, y + 1))
                        {
                            //if (Framing.GetTileSafely(x, y + 1).TileType == TileID.JungleGrass && tile.LiquidAmount == 255 && Framing.GetTileSafely(x, y - 1).LiquidAmount == 255 && WorldGen.genRand.NextBool(6))
                            //{
                            //    WorldGen.PlaceTile(x, y, TileID.Bamboo);

                            //    for (int j = y - 1; !Framing.GetTileSafely(x, j - 2).HasTile; j--)
                            //    {
                            //        if (Framing.GetTileSafely(x, j - 1).HasTile || Framing.GetTileSafely(x, j + WorldGen.genRand.Next(5, 11)).LiquidAmount == 0)
                            //        {
                            //            break;
                            //        }
                            //        else
                            //        {
                            //            WorldGen.PlaceTile(x, j, TileID.Bamboo);
                            //        }
                            //    }
                            //}
                            //else
                            if (Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<PyramidBrick>() || Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<PyramidPlatform>() || Framing.GetTileSafely(x, y + 1).TileType == TileID.Sandstone || Framing.GetTileSafely(x, y + 1).TileType == TileID.HardenedSand)
                            {
                                if (WorldGen.genRand.NextBool(3) && !tile.HasTile)
                                {
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<DesertWeed>(), style: Main.rand.Next(3));
                                }
                            }
                            else if (WorldGen.genRand.NextBool(8))
                            {
                                if (Framing.GetTileSafely(x, y + 1).TileType == TileID.Grass || Framing.GetTileSafely(x, y + 1).TileType == TileID.JungleGrass)
                                {
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<Nightglow>(), style: Main.rand.Next(3));
                                }
                            }
                        }
                    }
                    #endregion
                    #region vines

                    //if (Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<mazevine>())
                    //{
                    //    if (!WorldGen.genRand.NextBool(10))
                    //    {
                    //        bool maxLength = true;

                    //        for (int a = 0; a < 10; a++)
                    //        {
                    //            if (Framing.GetTileSafely(x, y - 1 - a).TileType != ModContent.TileType<mazevine>())
                    //            {
                    //                maxLength = false;
                    //                break;
                    //            }
                    //        }

                    //        if (!maxLength && !Framing.GetTileSafely(x, y).HasTile)
                    //        {
                    //            WorldGen.PlaceTile(x, y, ModContent.TileType<mazevine>());
                    //            Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<mazevine>();
                    //            Framing.GetTileSafely(x, y).TileFrameX = (short)(Main.rand.Next(3) * 18);
                    //        }
                    //    }
                    //}

                    if (!Framing.GetTileSafely(x, y).HasTile && Framing.GetTileSafely(x, y - 1).HasTile)
                    {
                        if (Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<SinewCorruption>())
                        {
                            if (!WorldGen.genRand.NextBool(10))
                            {
                                bool maxLength = true;

                                for (int a = 1; a <= 10; a++)
                                {
                                    if (Framing.GetTileSafely(x, y - a).TileType != ModContent.TileType<SinewCorruption>())
                                    {
                                        maxLength = false;
                                        break;
                                    }
                                }

                                if (!maxLength && !WorldGen.SolidOrSlopedTile(x, y + 2))
                                {
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<SinewCorruption>());
                                    Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<SinewCorruption>();
                                }
                            }
                        }
                        else if (Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<SinewCrimson>())
                        {
                            if (!WorldGen.genRand.NextBool(10))
                            {
                                bool maxLength = true;

                                for (int a = 1; a <= 10; a++)
                                {
                                    if (Framing.GetTileSafely(x, y - a).TileType != ModContent.TileType<SinewCrimson>())
                                    {
                                        maxLength = false;
                                        break;
                                    }
                                }

                                if (!maxLength && !WorldGen.SolidOrSlopedTile(x, y + 2))
                                {
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<SinewCrimson>());
                                    Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<SinewCrimson>();
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }
    }

    //public class Meadows : GenPass
    //{
    //    public Meadows(string name, float loadWeight) : base(name, loadWeight)
    //    {
    //    }

    //    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    //    {
    //        BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

    //        FastNoiseLite meadows = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
    //        meadows.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
    //        meadows.SetFrequency(0.5f);
    //        meadows.SetFractalType(FastNoiseLite.FractalType.FBm);
    //        meadows.SetFractalOctaves(3);

    //        progress.Message = "Growing flower patches";

    //        biomes.UpdateMap(new int[] { "meadow" }, progress);
    //    }
    //}
}
