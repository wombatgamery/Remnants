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
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;
using static Remnants.Content.World.BiomeMap;
using static Terraria.WorldGen;

namespace Remnants.Content.World
{
    public class BiomeMap : ModSystem
    {
        public int[,] Map;

        public int Scale => 50;
        public int Width => Main.maxTilesX / Scale;
        public int Height => Main.maxTilesY / Scale;

        private float[,] BlendX;
        private float[,] BlendY;
        private int BlendDistance => ModContent.GetInstance<Worldgen>().ExperimentalWorldgen ? 0 : 40;

        public float[,] Materials;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            RemWorld.InsertPass(tasks, new BiomeMapPopulation("Biome Map Population", 1), RemWorld.FindIndex(tasks, "Terrain") + 1);
            RemWorld.InsertPass(tasks, new BiomeMapSetup("Biome Map Setup", 1), 1);
        }

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
                blendingNoise.SetFrequency(0.01f);
                blendingNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
                blendingNoise.SetFractalOctaves(5);
                //biomes.blendingNoise.SetFractalLacunarity(2.25f);

                FastNoiseLite materialNoise = new FastNoiseLite();
                materialNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
                materialNoise.SetSeed(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                materialNoise.SetFrequency(0.05f);
                materialNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
                //biomes.materialsNoise.SetFractalLacunarity(2.25f);

                biomes.Materials = new float[Main.maxTilesX, Main.maxTilesY];

                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    progress.Set((float)y / Main.maxTilesY);

                    for (int x = 0; x < Main.maxTilesX; x++)
                    {
                        biomes.BlendX[x, y] = blendingNoise.GetNoise(x, y * 2 + 999);
                        biomes.BlendY[x, y] = blendingNoise.GetNoise(x + 999, y * 2);

                        biomes.Materials[x, y] = materialNoise.GetNoise(x, y * 2);
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

        internal class Tundra
        {
            public static int Left;
            public static int Right;

            public static int Center => (Left + Right + 1) / 2;
            public static int Width => Right - Left + 1;


            public static int Bottom;
        }

        internal class Jungle
        {
            public static int Left;
            public static int Right;

            public static float Center => (Left + Right + 1) / 2;
            public static int Width => Right - Left + 1;
        }

        internal class Desert
        {
            public static int Left;
            public static int Right;

            public static int Center => (Left + Right + 1) / 2;
            public static int Width => Right - Left + 1;


            public static int Bottom;

            public static int OasisX;
        }

        internal class Corruption
        {
            public static int X;
            public static int Y;
            public static int Size;
            public static float heightMultiplier;

            public static int Left => X - Size;
            public static int Right => X + Size;

            public static int orbX;
            public static int orbYPrimary => (int)Main.rockLayer;
            public static int orbYSecondary => Main.maxTilesY - 300 - (orbYPrimary - (int)Main.worldSurface);

            public static void CreateOrb(bool alternate)
            {
                int orbY = alternate ? orbYSecondary : orbYPrimary;

                int radius = (int)(20 * Main.maxTilesX / 4200f);

                FastNoiseLite noise = new FastNoiseLite();
                noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                noise.SetFrequency(0.1f);
                noise.SetFractalType(FastNoiseLite.FractalType.None);

                FastNoiseLite noise2 = new FastNoiseLite();
                noise2.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                noise2.SetFrequency(0.2f);
                noise2.SetFractalType(FastNoiseLite.FractalType.None);

                bool crimson = WorldGen.crimson ^ alternate;

                for (int j = (int)(orbY - radius * 1.5f); j <= orbY + radius * 1.5f; j++)
                {
                    for (int i = (int)(orbX - radius * 1.5f); i <= orbX + radius * 1.5f; i++)
                    {
                        float distance = Vector2.Distance(new Vector2(i, j), new Vector2(orbX, orbY)) + noise.GetNoise(i, j) * 10;

                        Tile tile = Main.tile[i, j];

                        if (distance < 16 * Main.maxTilesX / 4200f)
                        {
                            tile.TileType = crimson ? TileID.FleshBlock : TileID.LesionBlock;
                            //tile.WallType = (WorldGen.crimson ^ alternate) ? WallID.Flesh : WallID.LesionBlock;
                            tile.WallType = crimson ? WallID.CrimsonUnsafe3 : WallID.CorruptionUnsafe3;

                            tile.WallColor = crimson ? PaintID.DeepRedPaint : PaintID.OrangePaint;
                        }

                        if (distance < 12 * Main.maxTilesX / 4200f)
                        {
                            if (noise2.GetNoise(i, j) > -0.7f)
                            {
                                tile.HasTile = true;
                            }
                            else
                            {
                                tile.HasTile = false;

                                if (crimson && noise2.GetNoise(i, j) < -0.9f)
                                {
                                    tile.WallType = WallID.CrimsonUnsafe2;
                                }
                            }

                            tile.LiquidAmount = 51;
                        }
                        else if (distance < 16 * Main.maxTilesX / 4200f)
                        {
                            tile.HasTile = true;
                        }
                        else if (distance < 20 * Main.maxTilesX / 4200f)
                        {
                            tile.TileType = crimson ? TileID.Crimstone : TileID.Ebonstone;
                            tile.HasTile = true;
                        }
                    }
                }

                int count = (int)(8 * Main.maxTilesX / 4200f);
                while (count > 0)
                {
                    int x = orbX + (int)(WorldGen.genRand.NextFloat(-12, 12) * Main.maxTilesX / 4200f);
                    int y = orbY + (int)(WorldGen.genRand.NextFloat(-12, 12) * Main.maxTilesX / 4200f);

                    bool valid = true;

                    for (int j = y - 1; j <= y + 2; j++)
                    {
                        for (int i = x - 1; i <= x + 2; i++)
                        {
                            if (MiscTools.Tile(i, j).HasTile)
                            {
                                valid = false;
                            }
                        }
                    }
                    for (int j = y - 3; j <= y + 4; j++)
                    {
                        for (int i = x - 3; i <= x + 4; i++)
                        {
                            if (MiscTools.Tile(i, j).HasTile && MiscTools.Tile(i, j).TileType == TileID.ShadowOrbs)
                            {
                                valid = false;
                            }
                        }
                    }

                    if (valid)
                    {
                        //WorldGen.PlaceTile(x, y, TileID.BubblegumBlock);

                        for (int j = y; j <= y + 1; j++)
                        {
                            for (int i = x; i <= x + 1; i++)
                            {
                                Tile tile = Main.tile[i, j];

                                tile.TileType = TileID.ShadowOrbs;
                                tile.HasTile = true;
                                tile.TileFrameX = (short)((i - x) * 18);
                                tile.TileFrameY = (short)((j - y) * 18);
                                if (WorldGen.crimson ^ alternate)
                                {
                                    tile.TileFrameX += 18 * 2;
                                }
                            }
                        }

                        //WorldGen.PlaceObject(x, y, TileID.ShadowOrbs, style: (WorldGen.crimson ^ alternate) ? 1 : 0);
                        count--;
                    }
                }
            }
        }

        internal class MarbleCave
        {
            public static int Y;

            public static int Left => (int)(Main.maxTilesX * 0.4f);
            public static int Right => (int)(Main.maxTilesX * 0.6f);
        }

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

                GenVars.worldSurfaceLow = GenVars.worldSurfaceHigh = GenVars.worldSurface = Main.worldSurface;
                GenVars.rockLayerLow = GenVars.rockLayerHigh = GenVars.rockLayer = Main.rockLayer;
                #endregion

                bool calamity = ModLoader.TryGetMod("CalamityMod", out Mod cal);
                bool spiritReforged = ModLoader.TryGetMod("SpiritReforged", out Mod sr);

                #region tundra+corruption
                bool tundraCorruptionSwap = WorldGen.genRand.NextBool(2); //GenVars.dungeonSide == 1;

                int tundraSize = (int)(biomes.Width / 7);
                Corruption.Size = biomes.Width / 42;

                Tundra.Bottom = biomes.lavaLayer - 1;

                if (tundraCorruptionSwap)
                {
                    if (GenVars.dungeonSide != 1)
                    {
                        Corruption.X = (int)(biomes.Width * 0.4f) - 1 - Corruption.Size * 2;

                        Tundra.Right = (Corruption.X - Corruption.Size * 2) - 1;
                        Tundra.Left = Tundra.Right - tundraSize;
                    }
                    else
                    {
                        Corruption.X = (int)(biomes.Width * 0.6f) + 1 + Corruption.Size * 2;

                        Tundra.Left = (Corruption.X + Corruption.Size * 2) + 1;
                        Tundra.Right = Tundra.Left + tundraSize;
                    }
                }
                else
                {
                    if (GenVars.dungeonSide != 1)
                    {
                        Tundra.Right = (int)(biomes.Width * 0.4f) - 1;
                        Tundra.Left = Tundra.Right - tundraSize;

                        Corruption.X = Tundra.Left - 1 - Corruption.Size * 2;
                    }
                    else
                    {
                        Tundra.Left = (int)(biomes.Width * 0.6f) + 1;
                        Tundra.Right = Tundra.Left + tundraSize;

                        Corruption.X = Tundra.Right + 1 + Corruption.Size * 2;
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

                        if (spiritReforged)
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

                        if (spiritReforged)
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
                        biomes.AddBiome(x, y, BiomeID.Obsidian);
                    }
                }

                bool thorium = ModLoader.TryGetMod("ThoriumMod", out Mod mod);

                for (int y = 0; y < biomes.Height - 6; y++)
                {
                    for (int x = 0; x <= 6; x++)
                    {
                        bool jungleSide = GenVars.dungeonSide == 1 && x <= 5 || GenVars.dungeonSide != 1 && x >= biomes.Width - 6;
                        bool thoriumCompat = jungleSide && thorium;

                        if (!thoriumCompat && x <= 5 && x > 0 && y < biomes.caveLayer - 1 && y > biomes.surfaceLayer)
                        {
                            biomes.AddBiome(x, y, BiomeID.OceanCave);
                        }
                        else biomes.AddBiome(x, y, BiomeID.Beach);
                    }

                    for (int x = biomes.Width - 7; x < biomes.Width; x++)
                    {
                        bool jungleSide = GenVars.dungeonSide == 1 && x <= 5 || GenVars.dungeonSide != 1 && x >= biomes.Width - 6;
                        bool thoriumCompat = jungleSide && thorium;

                        if (!thoriumCompat && x >= biomes.Width - 6 && x < biomes.Width - 1 && y < biomes.caveLayer - 1 && y > biomes.surfaceLayer)
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

                for (int y = 1; y < biomes.Height; y++)
                {
                    for (int x = 6; x < biomes.Width - 6; x++)
                    {
                        int i = x + (y >= biomes.surfaceLayer ? (int)(noise.GetNoise(x / 2f / (Main.maxTilesX / 4200f), y / (Main.maxTilesY / 1200f)) * (Main.maxTilesX / 700f)) : 0);
                        int j = y;// + (int)(noise.GetNoise(x + 999, y) * (Main.maxTilesY / 600f));

                        if (biomes.Map[x, y] != BiomeID.Obsidian && biomes.Map[x, y] != BiomeID.Beach)
                        {
                            if (i >= (Tundra.Left + (j == Tundra.Bottom - 1 ? 1 : 0)) && i <= (Tundra.Right - (j == Tundra.Bottom - 1 ? 1 : 0)) && j < Tundra.Bottom)
                            {
                                biomes.AddBiome(x, y, BiomeID.Tundra);
                            }
                            else if (i >= (Desert.Left + (j == Desert.Bottom - 1 ? 1 : 0)) && i <= (Desert.Right - (j == Desert.Bottom - 1 ? 1 : 0)) && j < Desert.Bottom)
                            {
                                if (ModContent.GetInstance<Worldgen>().SunkenSeaRework && calamity && j >= (biomes.lavaLayer - biomes.caveLayer) / 2 + biomes.caveLayer)
                                {
                                    biomes.AddBiome(x, y, BiomeID.SunkenSea);
                                }
                                else biomes.AddBiome(x, y, BiomeID.Desert);
                            }
                            else if (i >= Jungle.Left && i <= Jungle.Right)
                            {
                                if (y < biomes.Height - 4)
                                {
                                    if (ModContent.GetInstance<Worldgen>().ExperimentalWorldgen && j > biomes.lavaLayer)
                                    {
                                        biomes.AddBiome(x, y, BiomeID.Toxic);
                                    }
                                    else biomes.AddBiome(x, y, BiomeID.Jungle);
                                }
                                else biomes.AddBiome(x, y, BiomeID.AshForest);
                            }
                            //else if (spiritReforged && y < biomes.surfaceLayer)
                            //{
                            //    if (i > Desert.Center && i < Jungle.Center || i < Desert.Center && i > Jungle.Center)
                            //    {
                            //        biomes.AddBiome(x, y, BiomeID.Savanna);
                            //    }
                            //}
                        }
                    }
                }
                GenVars.UndergroundDesertLocation = new Rectangle((Desert.Left) * biomes.Scale, (biomes.surfaceLayer - 1) * biomes.Scale, (Desert.Width) * biomes.Scale, (Desert.Bottom - (biomes.surfaceLayer - 1)) * biomes.Scale);
                GenVars.structures.AddStructure(GenVars.UndergroundDesertLocation);

                #region corruption
                Corruption.Y = (int)Main.worldSurface / biomes.Scale;
                Corruption.orbX = (int)((Corruption.X + 0.5f) * biomes.Scale);
                noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                noise.SetFrequency(0.2f);
                noise.SetFractalType(FastNoiseLite.FractalType.FBm);

                for (int j = biomes.skyLayer; j < biomes.Height - 6; j++)
                {
                    for (int i = 0; i < biomes.Width; i++)
                    {
                        Vector2 point = new Vector2(Corruption.X, MathHelper.Clamp(j, 1, biomes.caveLayer));
                        if ((j < biomes.surfaceLayer ? 0 : noise.GetNoise(i, j)) <= (1 - Vector2.Distance(point, new Vector2(i, j)) / Corruption.Size) * 2)
                        {
                            if (WorldGen.crimson)
                            {
                                biomes.AddBiome(i, j, BiomeID.Crimson);
                            }
                            else biomes.AddBiome(i, j, BiomeID.Corruption);
                        }

                        point = new Vector2(Corruption.X, MathHelper.Clamp(j, (Main.maxTilesY - 300 - (int)(Main.rockLayer - Main.worldSurface)) / biomes.Scale - 1, biomes.Height - 6));
                        if (noise.GetNoise(i, j) <= (1 - Vector2.Distance(point, new Vector2(i, j)) / Corruption.Size) * 2)
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
                glowshroom.SetFrequency(0.1f);

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
                                    if (j >= biomes.caveLayer && glowshroom.GetNoise(i, j * 2) < -0.93625f && j < biomes.lavaLayer)
                                    {
                                        biomes.AddBiome(i, j, BiomeID.Glowshroom);
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

                        if (GenVars.dungeonSide != 1 || !thorium)
                        {
                            biomes.AddBiome(1, biomes.surfaceLayer, BiomeID.OceanCave); biomes.AddBiome(1, biomes.surfaceLayer - 1, BiomeID.OceanCave);
                        }
                        if (GenVars.dungeonSide == 1 || !thorium)
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
                return Map[i / Scale, j / Scale];
            }
            else
            {
                int i = (int)MathHelper.Clamp(x + BlendX[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesX - 20);
                int j = (int)MathHelper.Clamp(y + BlendY[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesY - 20);
                return Map[i / Scale, j / Scale];
            }
        }

        public int FindLayer(int x, int y)
        {
            return (int)MathHelper.Clamp(y + BlendY[x, y] * BlendDistance, 20, Main.maxTilesY - 20) / Scale;
        }

        public bool UpdatingBiome(float x, float y, bool[] biomesToUpdate, int type)
        {
            return biomesToUpdate[type] && FindBiome(x, y) == type;
        }

        public int skyLayer => (int)(Main.worldSurface * 0.4) / Scale;
        public int surfaceLayer => (int)(Main.worldSurface + Scale / 2) / Scale;
        public int caveLayer => (int)(Main.rockLayer + Scale / 2) / Scale;
        public int lavaLayer => (int)(GenVars.lavaLine + Scale / 2) / Scale - 1;

        public void UpdateMap(int[] biomes, GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.BiomeUpdate");

            FastNoiseLite caves1 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            FastNoiseLite caves2 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            FastNoiseLite caves3 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));

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
            if (!biomesToUpdate[BiomeID.Clouds])
            {
                if (!biomesToUpdate[BiomeID.Tundra] && !biomesToUpdate[BiomeID.Jungle] && !biomesToUpdate[BiomeID.Desert] && !biomesToUpdate[BiomeID.Corruption] && !biomesToUpdate[BiomeID.Crimson])
                {
                    startY = (int)Main.worldSurface - BlendDistance * 2;
                    if (biomesToUpdate[BiomeID.Beach])
                    {
                        startY -= 100;
                    }
                }
                //else if (biomes.Contains(BiomeID.Underworld) || biomes.Contains("sulfurlayer"))
                //{
                //    startY = Main.maxTilesY - 200 - blendDistance;
                //}
            }

            //for (int j = -1; j <= 1; j++)
            //{
            //    for (int i = -1; i <= 1; i++)
            //    {

            //    }
            //}


            bool iceFlip = WorldGen.genRand.NextBool(2);

            bool calamity = ModLoader.TryGetMod("CalamityMod", out Mod cal);
            bool lunarVeil = ModLoader.TryGetMod("Stellamod", out Mod lv);

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

            for (float y = startY; y < Main.maxTilesY - 40; y++)
            {
                progress.Set((y - startY) / (Main.maxTilesY - 20 - startY));

                for (float x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = MiscTools.Tile(x, y);

                    int i = (int)MathHelper.Clamp(x + BlendX[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesX - 20);
                    int j = (int)MathHelper.Clamp(y + BlendY[(int)x, (int)y] * BlendDistance, 20, Main.maxTilesY - 20);

                    int layer = j / Scale;

                    bool underground = layer >= surfaceLayer;
                    bool sky = layer < skyLayer;

                    #region surface
                    if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Tundra))
                    {
                        if (layer >= surfaceLayer)
                        {
                            if (MaterialBlend(x, y, frequency: 2) >= 0.2f)
                            {
                                tile.TileType = TileID.SnowBlock;
                            }
                            else tile.TileType = TileID.IceBlock;

                            if (tile.WallType != 0 || layer >= surfaceLayer)
                            {
                                if (MaterialBlend(x, y, frequency: 2) >= 0.2f)
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

                        SetDefaultValues(caves1);
                        SetDefaultValues(caves2);
                        SetDefaultValues(caves3);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFractalType(FastNoiseLite.FractalType.PingPong);
                        caves1.SetFractalOctaves(3);
                        caves1.SetFrequency(0.0075f);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.015f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);

                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.03f);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        float distortedY = y;

                        distortedY -= (int)Main.worldSurface;
                        //if (layer < surfaceLayer)
                        //{
                        //    distortedY /= (float)(2 - Math.Clamp(MathHelper.Distance(x / Scale, Tundra.Center) / (Tundra.Width / 2), 0, 1));
                        //}
                        distortedY *= 2;
                        distortedY += (int)Main.worldSurface;

                        float sinewave = (int)(Math.Sin(x / 30f + y / (60f * (iceFlip ? -1 : 1))) * 30);
                        distortedY += sinewave;

                        if (layer >= surfaceLayer || y + sinewave >= (Terrain.Middle + Terrain.Minimum * 2) / 3 && i > (Tundra.Left + Main.maxTilesX / 2100f) * Scale && i < (Tundra.Right + 1 - Main.maxTilesX / 2100f) * Scale)
                        {
                            if (layer >= surfaceLayer)
                            {
                                caves1.SetFractalGain(0.5f);
                                caves1.SetFractalPingPongStrength(caves2.GetNoise(x, y * 2 + (int)(Math.Sin(x / 30f + y / (60f * (iceFlip ? -1 : 1))) * 50)) + 2);
                            }
                            else
                            {
                                caves1.SetFractalGain(0.2f);
                                caves1.SetFractalPingPongStrength(caves2.GetNoise(x, y * 2 + (int)(Math.Sin(x / 30f + y / (60f * (iceFlip ? -1 : 1))) * 50)) + 1);
                            }

                            float _caves = caves1.GetNoise(x, distortedY);
                            float _size = caves3.GetNoise(x, distortedY) / (layer < surfaceLayer ? 16 : 8);

                            //if (layer < surfaceLayer)
                            //{
                            //    _caves = ((_caves - 0.8f) * 2) + 0.8f;
                            //}

                            if (_caves > (_size / 2 + 0.5f) * (layer >= surfaceLayer ? 0.05f : 1.2f))
                            {
                                tile.HasTile = false;
                                if (WorldGen.genRand.NextBool(25) && y < GenVars.lavaLine)
                                {
                                    tile.LiquidAmount = 255;
                                }
                            }
                            else if (layer >= surfaceLayer)
                            {
                                tile.HasTile = true;
                            }


                            if (_caves > (_size / 2 + 0.5f) * (layer >= surfaceLayer ? 0.75f : 1.8f) && j + sinewave >= (Terrain.Middle * 2 + Terrain.Minimum) / 3)
                            {
                                tile.WallType = 0;
                            }
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Jungle))
                    {
                        if (tile.TileType == TileID.Grass)
                        {
                            tile.TileType = TileID.JungleGrass;
                        }
                        else if (tile.TileType == TileID.Dirt)
                        {
                            tile.TileType = TileID.Mud;
                        }

                        if (tile.WallType == WallID.DirtUnsafe)
                        {
                            tile.WallType = WallID.JungleUnsafe;
                        }
                        else if (tile.WallType == WallID.RocksUnsafe1)
                        {
                            tile.WallType = WallID.JungleUnsafe3;
                        }

                        SetDefaultValues(caves1);
                        SetDefaultValues(caves2);
                        SetDefaultValues(caves3);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFractalType(FastNoiseLite.FractalType.PingPong);
                        caves1.SetFractalOctaves(3);
                        caves1.SetFrequency(0.0125f);
                        //caves.SetFractalGain(0.25f);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.025f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);

                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.075f);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        background.SetFrequency(0.04f);
                        background.SetFractalType(FastNoiseLite.FractalType.FBm);
                        background.SetFractalOctaves(2);
                        background.SetFractalWeightedStrength(-1);
                        background.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
                        background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);

                        if (MaterialBlend(x, y, frequency: 2) >= (layer >= surfaceLayer && layer < caveLayer ? -0.3f : 0f))//(layer < caveLayer ? -0.2f : 0.2f))
                        {
                            tile.TileType = TileID.Mud;
                        }
                        else tile.TileType = TileID.Stone;

                        if (tile.WallType != 0)
                        {
                            if (MaterialBlend(x, y, frequency: 2) >= (layer >= surfaceLayer && layer < caveLayer ? -0.3f : 0f))//(layer < caveLayer ? -0.2f : 0.2f))
                            {
                                tile.WallType = WallID.JungleUnsafe;
                            }
                            else tile.WallType = WallID.JungleUnsafe3; //layer >= lavaLayer ? WallID.LavaUnsafe2 :
                        }

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

                            if (tile.TileType != TileID.Mud)
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
                        }

                        if (layer >= surfaceLayer)
                        {
                            if (!tile.HasTile && WorldGen.genRand.NextBool(50) && y < GenVars.lavaLine)
                            {
                                tile.LiquidAmount = 255;
                            }

                            //caves1.SetFractalPingPongStrength(caves2.GetNoise(x, y * 2) + 2);
                            //float _caves = caves1.GetNoise(x, y * 2);
                            //float _size = caves3.GetNoise(x, y * 2) / 2 + 1;

                            //if (_caves > -_size * 0.1f)
                            //{
                            //    tile.HasTile = false;
                            //    if (WorldGen.genRand.NextBool(50) && y < GenVars.lavaLine)
                            //    {
                            //        tile.LiquidAmount = 255;
                            //    }
                            //}
                            //else
                            //{
                            //    tile.HasTile = true;
                            //}

                            //if (_caves < _size * 0.4f)
                            //{
                            //    if (MaterialBlend(x, y, frequency: 2) >= -0.2f)
                            //    {
                            //        tile.WallType = WallID.JungleUnsafe;
                            //    }
                            //    else tile.WallType = WallID.JungleUnsafe3; //layer >= lavaLayer ? WallID.LavaUnsafe2 :
                            //}
                            //else if (y > Main.worldSurface)
                            //{
                            //    tile.WallType = 0;
                            //}
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Desert))
                    {
                        SetDefaultValues(caves1);
                        SetDefaultValues(caves2);
                        SetDefaultValues(caves3);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.03f / 2);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalLacunarity(1.75f);
                        caves1.SetFractalGain(0.8f);
                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.03f / 2);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.015f / 2);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        background.SetFrequency(0.1f);
                        background.SetFractalType(FastNoiseLite.FractalType.None);
                        background.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
                        background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);

                        if (layer >= surfaceLayer)
                        {
                            if (MaterialBlend(x, y, frequency: 2) >= 0.25f)
                            {
                                tile.TileType = TileID.HardenedSand;
                                if (tile.WallType != 0 || FindBiome(x - 1, y) == BiomeID.Desert && FindBiome(x + 1, y) == BiomeID.Desert && FindBiome(x, y + 1) == BiomeID.Desert)
                                {
                                    tile.WallType = WallID.HardenedSand;
                                }
                            }
                            else
                            {
                                tile.TileType = TileID.Sandstone;
                                if (tile.WallType != 0 || FindBiome(x - 1, y) == BiomeID.Desert && FindBiome(x + 1, y) == BiomeID.Desert && FindBiome(x, y + 1) == BiomeID.Desert)
                                {
                                    tile.WallType = WallID.Sandstone;
                                }
                            }

                            float _tunnels = caves1.GetNoise(x, y * 2);
                            //float _nests = nests.GetNoise(x, y + ((float)Math.Cos(x / 60) * 20)) * ((nests2.GetNoise(x, y + ((float)Math.Cos(x / 60) * 20)) + 1) / 2);
                            float _fossils = (fossils.GetNoise(x, y * 2) + 1) / 2 * 0.4f;

                            float _background = (background.GetNoise(x, y * 2) + 1) / 2 * 0.3f;


                            float _size = (caves2.GetNoise(x, y * 2 + (float)Math.Cos(x / 60) * 20) + 1) / 2 / 4;
                            float _offset = caves3.GetNoise(x, y * 2 + (float)Math.Cos(x / 60) * 20);

                            //if (MaterialBlend(x, y, frequency: 2) <= 0.2f)
                            //{
                            //    WGTools.Tile(x, y).TileType = TileID.HardenedSand;
                            //}
                            //else tile.TileType = TileID.Sand;
                            tile.HasTile = true;
                            tile.Slope = 0;
                            tile.LiquidAmount = 0;
                            if (_tunnels + 0.1f + _fossils < _offset - _size || _tunnels - 0.1f - _fossils > _offset + _size)
                            {
                                tile.TileType = TileID.DesertFossil;
                                tile.WallType = WallID.DesertFossil;
                            }
                            else if (_tunnels + 0.1f > _offset - _size && _tunnels - 0.1f < _offset + _size)
                            {
                                if (_tunnels > _offset - _size && _tunnels < _offset + _size)
                                {
                                    tile.HasTile = false;
                                    tile.LiquidAmount = 0;

                                    //if (_tunnels - 0.05f - _background > _offset - _size && _tunnels + 0.05f + _background < _offset + _size)
                                    //{
                                    //    tile.WallType = WallID.HardenedSand;
                                    //}
                                }
                            }
                        }
                        else
                        {
                            if (MaterialBlend(x, y, frequency: 2) <= 0.2f)
                            {
                                MiscTools.Tile(x, y).TileType = TileID.HardenedSand;
                            }
                            else tile.TileType = TileID.Sand;

                            if (layer == surfaceLayer - 1)
                            {
                                tile.HasTile = true;
                                tile.TileType = TileID.HardenedSand;
                                tile.WallType = WallID.HardenedSand;
                            }
                            if (tile.HasTile)// && y > Terrain.Middle)
                            {
                                int var = 1;
                                //if (y / biomes.scale >= (int)Main.worldSurface / biomes.scale - 1 || dunes.GetNoise(x, y + 2) > 0)
                                //{
                                //    var = 2;
                                //}
                                for (int k = -var; k <= var; k++)
                                {
                                    if (!MiscTools.Tile(x + k, y + 1).HasTile)
                                    {
                                        MiscTools.Tile(x + k, y + 1).TileType = TileID.Sand;
                                        MiscTools.Tile(x + k, y + 1).HasTile = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Corruption))
                    {
                        SetDefaultValues(caves1);

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
                            if (layer >= surfaceLayer || tile.TileType == TileID.Stone)
                            {
                                tile.TileType = TileID.Ebonstone;
                            }
                            if (layer >= surfaceLayer || tile.WallType == WallID.RocksUnsafe1)
                            {
                                tile.WallType = WallID.EbonstoneUnsafe;
                            }

                            if (layer >= surfaceLayer)
                            {
                                tile.HasTile = true;
                            }

                            if (y > Terrain.Maximum - 5 && i / Scale > Corruption.Left && i / Scale < Corruption.Right)
                            {
                                float _size = 1;// (caves2.GetNoise(x * 2, y) + 1) / 2;
                                float _caves = caves1.GetNoise(i * 2, j - Terrain.Maximum) / 2 + 0.5f;

                                _caves -= (layer >= surfaceLayer ? 0f : 0.1f);

                                float orbDistance = Vector2.Distance(new Vector2(x * 2, y), new Vector2(Corruption.orbX * 2, !WorldGen.crimson ? Corruption.orbYPrimary : Corruption.orbYSecondary));
                                _caves += MathHelper.Clamp(1 - orbDistance / (Main.maxTilesX / 4200f * 36), 0, 1);

                                if (_caves > 0.55f)//thing > _offset - _size / (ug ? 2 : 3) && thing < _offset + _size / (ug ? 2 : 3) && !sky)
                                {
                                    tile.HasTile = false;

                                    if (layer >= surfaceLayer && _caves > 0.7f)
                                    {
                                        tile.WallType = WallID.CorruptionUnsafe2;
                                    }
                                }
                            }

                            if (layer < surfaceLayer && MiscTools.Tile(x - 1, y - 1).TileType == TileID.Dirt && !MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.CorruptGrass;
                            }
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Crimson))
                    {
                        SetDefaultValues(caves1);

                        //caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        //caves1.SetFrequency(0.015f);
                        //caves1.SetFractalType(FastNoiseLite.FractalType.Ridged);
                        //caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                        //caves1.SetFractalOctaves(3);
                        //caves1.SetFractalGain(0.75f);
                        //caves1.SetFractalWeightedStrength(0.5f);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
                        caves1.SetFrequency(0.01f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.Ridged);
                        caves1.SetFractalOctaves(4);
                        //caves1.SetFractalGain(0.6f);
                        caves1.SetFractalWeightedStrength(0.5f);

                        if (!Main.wallDungeon[tile.WallType])
                        {
                            float _size = 1;// (caves2.GetNoise(x, y) + 1) / 2 + 0.25f;
                            //caves1.SetFractalPingPongStrength(caves3.GetNoise(x, y) / 2 + 2);
                            float _caves = caves1.GetNoise(x, y);// / 2;
                            //float _offset = caves3.GetNoise(x, y) + 0.5f;

                            float thing = _caves / (1 + 1 / 2);

                            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(Corruption.orbX, WorldGen.crimson ? Corruption.orbYPrimary : Corruption.orbYSecondary));
                            thing += MathHelper.Clamp((1 - dist / (Main.maxTilesX / 4200f * 48)) * 4, 0, 1);

                            bool ug = layer >= surfaceLayer - 1;
                            if (ug || tile.WallType != 0)
                            {
                                tile.HasTile = true;
                            }
                            if (!sky)
                            {
                                //if (thing > _offset - _size / (underground ? 2 : 3) && thing < _offset + _size / (underground ? 2 : 3) && !sky)
                                if (thing > 0.5f - _size / (underground ? 1.75f : 2.25f))
                                {
                                    tile.TileType = TileID.Crimstone;
                                    if (ug || tile.WallType != 0)
                                    {
                                        tile.WallType = WallID.CrimstoneUnsafe;
                                        //tile.WallColor = PaintID.DeepRedPaint;
                                    }
                                    if (thing > 0.225f)
                                    {
                                        tile.HasTile = false;

                                        //if (ug && thing > 0.75f)
                                        //{
                                        //    tile.WallType = WallID.CrimsonUnsafe3;
                                        //    //tile.WallColor = PaintID.DeepRedPaint;
                                        //}
                                    }
                                }
                                else
                                {
                                    tile.TileType = TileID.Dirt;
                                    if (ug)
                                    {
                                        tile.WallType = WallID.DirtUnsafe;
                                    }
                                }
                            }

                            if (!underground && MiscTools.Tile(x - 1, y - 1).TileType == TileID.Dirt && !MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
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
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Glowshroom))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.01f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.7f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);

                        if (false)//MaterialBlend(x + WorldGen.genRand.Next(-1, 2), y + WorldGen.genRand.Next(-1, 2), true, 2) <= -0.3f)
                        {
                            MiscTools.Tile(x, y).TileType = TileID.MushroomBlock;
                        }
                        else
                        {
                            if (MaterialBlend(x, y, frequency: 2) >= 0f)
                            {
                                tile.TileType = TileID.Mud;
                            }
                            else tile.TileType = TileID.Stone;

                            //if (tile.TileType == TileID.Grass || WorldGen.genRand.NextBool(25) || FindBiome(x + 1, y + 1) != BiomeID.Glowshroom && !WGTools.Solid(x + 1, y + 1))
                            //{
                            //    tile.TileType = TileID.MushroomGrass;
                            //}
                            //else tile.TileType = TileID.Mud;
                        }

                        float _caves = caves1.GetNoise(x, y * 2); //+((float)Math.Cos(x / 60) * 20)

                        float _layering = (float)Math.Cos(MathHelper.Pi * ((-y / 25f + 0.5f) % 1)) * 0.2f;

                        //shroomcaves.SetFractalPingPongStrength(shroomcaves2.GetNoise(x, y * 2) + 1.625f);
                        //float _caves = shroomcaves.GetNoise(x, y * 2);
                        //float _size = (shroomcaves3.GetNoise(x, y * 2) / 2) + 1;

                        if (_caves + _layering < -0.25f)
                        {
                            tile.HasTile = false;
                            if (WorldGen.genRand.NextBool(25))
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

                        if (caves1.GetNoise(x + 999, y * 2 + 999) + _layering > -0.3f)
                        {
                            tile.WallType = WallID.MushroomUnsafe;
                        }
                        else tile.WallType = 0;
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Obsidian))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.02f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.75f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Sub);

                        if (lunarVeil && lv.TryFind("CindersparkDirt", out ModTile csDirt))
                        {
                            tile.TileType = csDirt.Type;
                        }
                        else if (MaterialBlend(x, y, frequency: 2) >= -0.1f)
                        {
                            tile.TileType = TileID.Obsidian;
                            tile.WallType = WallID.ObsidianBackUnsafe;
                        }
                        else
                        {
                            tile.TileType = TileID.Ash;
                            tile.WallType = WallID.LavaUnsafe1;
                        }

                        float _caves = caves1.GetNoise(x, y * 2); //+((float)Math.Cos(x / 60) * 20)

                        //shroomcaves.SetFractalPingPongStrength(shroomcaves2.GetNoise(x, y * 2) + 1.625f);
                        //float _caves = shroomcaves.GetNoise(x, y * 2);
                        //float _size = (shroomcaves3.GetNoise(x, y * 2) / 2) + 1;

                        if (_caves > -0.7f)
                        {
                            tile.HasTile = false;
                            tile.LiquidAmount = 0;

                            if (_caves > -0.55f)
                            {
                                tile.WallType = 0;
                            }
                        }
                        else
                        {
                            tile.HasTile = true;
                            tile.Slope = 0;
                        }
                        //if (tile.WallType == 0 && caves1.GetNoise(x + 999, y * 2 + 999) > -0.2f)
                        //{
                        //    //if (tile.type == TileID.Stone || _materials < -0.7f)
                        //    //{
                        //    //    tile.wall = WallID.RocksUnsafe1;
                        //    //}
                        //    //else
                        //    tile.WallType = WallID.MushroomUnsafe;
                        //}
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Marble))
                    {
                        tile.TileType = TileID.Marble;
                        tile.HasTile = true;

                        tile.LiquidType = 0;

                        if (FindBiome(x, y - 1) == BiomeID.Marble && FindBiome(x, y + 1) == BiomeID.Marble)
                        {
                            tile.WallType = WallID.MarbleUnsafe;
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Granite))
                    {
                        tile.TileType = TileID.Granite;
                        tile.HasTile = true;

                        tile.LiquidType = 0;

                        if (FindBiome(x - 1, y) == BiomeID.Granite && FindBiome(x + 1, y) == BiomeID.Granite)
                        {
                            tile.WallType = WallID.GraniteUnsafe;
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.OceanCave))
                    {
                        SetDefaultValues(caves1);

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
                            SetDefaultValues(caves2);

                            caves2.SetNoiseType(FastNoiseLite.NoiseType.Value);
                            caves2.SetFrequency(0.01f);
                            caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                            caves2.SetFractalOctaves(3);

                            float _caves = caves1.GetNoise(x, y * 2);
                            float _size = caves3.GetNoise(x, y * 2) / 2 + 1;

                            if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Grass || tile.TileType == TileID.ClayBlock || tile.TileType == TileID.Stone || tile.TileType == TileID.Silt)
                            {
                                if (MaterialBlend(x, y, frequency: 2) < 0f)
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
                            if (_caves + _size * 0.2f < 0.2f)
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
                        if (layer >= surfaceLayer - 1)
                        {
                            float _background = caves1.GetNoise(x + 999, y * 2 + 999);

                            if (_background < 0 && layer >= surfaceLayer)
                            {
                                if (MaterialBlend(x, y, frequency: 4) < 0)
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
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Toxic))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves1.SetFrequency(0.02f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.6f);

                        if (MaterialBlend(x, y, true) <= -0.25f)
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
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Underworld) || UpdatingBiome(x, y, biomesToUpdate, BiomeID.AshForest))
                    {
                        background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        background.SetFrequency(0.025f);
                        background.SetFractalType(FastNoiseLite.FractalType.Ridged);
                        background.SetFractalWeightedStrength(-0.5f);
                        background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                        background.SetFractalOctaves(2);

                        tile.HasTile = true;

                        if (tile.TileType != ModContent.TileType<Hardstone>())
                        {
                            if (layer >= Height - 1)
                            {
                                tile.TileType = (ushort)ModContent.TileType<Hardstone>();
                                tile.HasTile = true;
                            }
                            //else if (MaterialBlend(x, j, true, 2) <= -0.1f && FindBiome(x, y) != BiomeID.AshForest)
                            //{
                            //    tile.TileType = TileID.Obsidian;
                            //    //if (obsidian.GetNoise(x, y) > 0.5f)
                            //    //{
                            //    //    tile.HasTile = false;
                            //    //    tile.WallType = WallID.ObsidianBackUnsafe;
                            //    //}
                            //}
                            //else if (gems.GetNoise(x, j) > 0.4f)
                            //{
                            //    tile.TileType = (ushort)ModContent.TileType<ashdiamond>();
                            //}
                            //else if (ash.GetNoise(x, y * 5 + (float)Math.Cos(x / 30) * 25) > 0)
                            //{
                            //    tile.TileType = TileID.Ash;
                            //}
                            //else tile.TileType = (ushort)ModContent.TileType<lavastone>();

                            else tile.TileType = TileID.Ash;
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Aether))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.015f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.Ridged);

                        float _caves = caves1.GetNoise(x, y * 2); //+((float)Math.Cos(x / 60) * 20)

                        //shroomcaves.SetFractalPingPongStrength(shroomcaves2.GetNoise(x, y * 2) + 1.625f);
                        //float _caves = shroomcaves.GetNoise(x, y * 2);
                        //float _size = (shroomcaves3.GetNoise(x, y * 2) / 2) + 1;

                        tile.LiquidType = LiquidID.Shimmer;

                        if (_caves < 0.6f)
                        {
                            tile.HasTile = false;
                            if (WorldGen.genRand.NextBool(5))
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

                        if (caves1.GetNoise(x + 999, y * 2 + 999) > 0.6f)
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
                    #region modded
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.SunkenSea))
                    {
                        SetDefaultValues(caves1);
                        SetDefaultValues(caves2);
                        SetDefaultValues(caves3);

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

                        float _caves = caves1.GetNoise(x, y * 2);
                        float _prisms = caves2.GetNoise(x + caves3.GetNoise(x, y * 2 + 999) * 10, y * 2 + caves3.GetNoise(x + 999, y * 2) * 5);

                        tile.HasTile = true;
                        if (MaterialBlend(x, y) >= -0.1f)
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
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Savanna))
                    {
                        if (MaterialBlend(x, y, frequency: 2) <= 0.2f)
                        {
                            MiscTools.Tile(x, y).TileType = TileID.HardenedSand;
                        }
                        else tile.TileType = TileID.Sand;

                        if (tile.WallType != 0)
                        {
                            tile.WallType = WallID.HardenedSand;
                        }
                    }
                    #endregion
                    #endregion

                    if (y >= Main.maxTilesY - 20)
                    {
                        break;
                    }
                    //if (biomes.Contains(FindBiome(x, y, false)) || biomes.Contains(FindBiomeFast(x + scale / 2, y)) || biomes.Contains(FindBiomeFast(x - scale / 2, y)) || biomes.Contains(FindBiomeFast(x, y + scale / 2)) || biomes.Contains(FindBiomeFast(x, y - scale / 2)))

                    if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Beach))
                    {
                        if (layer > surfaceLayer)
                        {
                            tile.HasTile = true;
                        }
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
                        else
                        {
                            if (tile.TileType == TileID.Dirt)
                            {
                                for (int k = 1; k <= WorldGen.genRand.Next(5, 7); k++)
                                {
                                    if (!MiscTools.Tile(x, y - k).HasTile)
                                    {
                                        tile.TileType = TileID.Sand;
                                        break;
                                    }
                                }
                            }
                        }

                        if (tile.WallType == WallID.RocksUnsafe1)
                        {
                            if (x < Main.maxTilesX / 2)
                            {
                                tile.WallType = WallID.RocksUnsafe4;
                            }
                            else tile.WallType = WallID.RocksUnsafe3;
                        }
                    }
                }
            }
        }

        public float MaterialBlend(float x, float y, bool flip = false, float frequency = 1)
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

        private void SetDefaultValues(FastNoiseLite noise)
        {
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(3);
            noise.SetFractalGain(0.5f);
            noise.SetFractalLacunarity(2);
            noise.SetFractalWeightedStrength(0);
            noise.SetFractalPingPongStrength(2);
            noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
            noise.SetCellularJitter(1);
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

        public const int Clouds = 7;

        public const int Glowshroom = 8;

        public const int Marble = 9;

        public const int Granite = 10;

        public const int Beach = 11;

        public const int OceanCave = 12;

        public const int Aether = 13;

        public const int AshForest = 14;

        public const int Obsidian = 15;

        public const int Toxic = 16;



        public const int SunkenSea = 100;

        public const int Savanna = 101;

        public const int Abysm = 102;
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


            biomes.UpdateMap(new int[] { BiomeID.Tundra, BiomeID.Jungle, BiomeID.Desert, BiomeID.Corruption, BiomeID.Crimson, BiomeID.Underworld, BiomeID.AshForest, BiomeID.Obsidian, BiomeID.Beach, BiomeID.Toxic, BiomeID.SunkenSea, BiomeID.Savanna, BiomeID.Abysm, BiomeID.Glowshroom, BiomeID.Marble, BiomeID.Granite, BiomeID.Aether, BiomeID.OceanCave }, progress);

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
                    for (int j = (int)(y - radius * 2); j <= y + radius * 2; j++)
                    {
                        for (int i = (int)(x - radius * 2); i <= x + radius * 2; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) < radius * 2)
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
                                if (tile.TileType == TileID.Ebonstone)
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
            //Corruption.CreateOrb(false);
            Corruption.CreateOrb(!WorldGen.crimson);

            int altars = Main.maxTilesX / 210;
            while (altars > 0)
            {
                #region spawnconditions
                int x = WorldGen.genRand.Next(Corruption.Left * biomes.Scale, Corruption.Right * biomes.Scale);
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
            for (int y = (int)Main.rockLayer - 50; y < GenVars.lavaLine + 50; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (biomes.FindBiome(x, y) == BiomeID.Glowshroom)
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

                        if (!MiscTools.SurroundingTilesActive(x, y, true))
                        {
                            if (tile.TileType == TileID.Mud || tile.TileType == TileID.MushroomBlock)
                            {
                                tile.TileType = TileID.MushroomGrass;
                            }
                        }
                    }
                }
            }
            #endregion

            Vector2 point;
            float threshold;
            FastNoiseLite terrain;

            #region marblecave
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.MarbleCave");

            terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            terrain.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            terrain.SetFrequency(0.02f);
            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrain.SetFractalOctaves(4);

            for (int y = (int)Main.rockLayer; y < GenVars.lavaLine + 50; y++)
            {
                progress.Set((y - Main.rockLayer) / (GenVars.lavaLine + 50 - Main.rockLayer));

                for (int x = (int)(Main.maxTilesX * 0.35f) - 100; x < (int)(Main.maxTilesX * 0.65f) + 100; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Marble)
                    {
                        point = new Vector2(MathHelper.Clamp(x, Main.maxTilesX * 0.4f + 50, Main.maxTilesX * 0.6f - 50), (MarbleCave.Y + 0.5f) * biomes.Scale);

                        //if (y > point.Y)
                        //{
                        //    threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 40)) * 1.5f, 0, 1);
                        //}
                        //else
                        threshold = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(x, y), point) / 80, 0, 1) * 2 - 1;

                        if (terrain.GetNoise(x * 3, y) * 2f < threshold)
                        {
                            MiscTools.Tile(x, y).HasTile = false;

                            if (y > point.Y + 32)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 153;
                            }
                            else MiscTools.Tile(x, y).LiquidAmount = 0;
                        }
                        if (terrain.GetNoise(x * 3, y) * 3f + 0.35f < threshold)
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }
            #endregion

            #region granitecave
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.GraniteCave");

            terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            terrain.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            terrain.SetFrequency(0.03f);
            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrain.SetFractalOctaves(2);

            for (int y = (int)Main.worldSurface - 25; y < Main.maxTilesY - 175; y++)
            {
                progress.Set((y - (Main.worldSurface - 25)) / (Main.maxTilesY - 175 - (Main.worldSurface - 25)));

                for (int x = 400; x < Main.maxTilesX - 400; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Granite)
                    {
                        point = new Vector2((Tundra.Center + 0.5f) * biomes.Scale, MathHelper.Clamp(y, (int)Main.worldSurface + 50, Main.maxTilesY - 350));
                        threshold = MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y), point) / 75) * 1.5f, 0, 1);

                        float _terrain = terrain.GetNoise(x, y * 2) * 1.5f;

                        if (_terrain + 0.5f < threshold * 2 - 1)
                        {
                            MiscTools.Tile(x, y).HasTile = false;

                            if (Vector2.Distance(new Vector2(x, y), point) > 35)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                            else MiscTools.Tile(x, y).LiquidAmount = 0;
                        }

                        point.Y = MathHelper.Clamp(y, (int)Main.worldSurface + 50, Main.maxTilesY - 450);
                        threshold = MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y), point) / 75) * 1.5f, 0, 1);

                        if (_terrain + 1 < threshold * 2 - 1)
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }
            #endregion

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
                        if (biomes.FindLayer(x, y) <= biomes.surfaceLayer)
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
                    for (int x = (Desert.Left - 1) * biomes.Scale; x <= (Desert.Right + 2) * biomes.Scale; x++)
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
                    for (int x = (Desert.Left - 1) * biomes.Scale; x <= (Desert.Right + 2) * biomes.Scale; x++)
                    {
                        if (MiscTools.Tile(x, y).WallType == (ushort)ModContent.WallType<Walls.dev.nothing>())
                        {
                            MiscTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }

            #region underworld
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Underworld");

            terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            terrain.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            terrain.SetFrequency(0.03f);
            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrain.SetFractalOctaves(3);
            //terrain.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            //terrain.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);

            FastNoiseLite elevation = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            elevation.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            elevation.SetFrequency(0.01f);
            elevation.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite spacing = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            spacing.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            spacing.SetFrequency(0.06f);
            spacing.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite background = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            background.SetFrequency(0.01f);
            background.SetFractalType(FastNoiseLite.FractalType.Ridged);
            background.SetFractalWeightedStrength(-0.5f);
            background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
            background.SetFractalOctaves(2);

            for (float y = Main.maxTilesY - 250; y < Main.maxTilesY - 21; y++)
            {
                progress.Set((y - (Main.maxTilesY - 200)) / 200);

                for (float x = 20; x < Main.maxTilesX - 20; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Underworld || biomes.FindBiome(x, y) == BiomeID.AshForest)
                    {
                        point = new Vector2(x, Main.maxTilesY - 150 + elevation.GetNoise(x, y) * 30 + WorldGen.genRand.Next(2));

                        if (y > point.Y)
                        {
                            threshold = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(x, y), point) / (50 + spacing.GetNoise(x, y) * 50), 0, 1);
                        }
                        else threshold = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(x, y), point) / (50 + spacing.GetNoise(x, y) * 50), 0, 1);

                        float _terrain = terrain.GetNoise(x, y * 2);
                        if (_terrain < threshold - 0.25f && MiscTools.Tile(x, y).TileType != ModContent.TileType<Hardstone>())
                        {
                            MiscTools.Tile(x, y).HasTile = false;
                            if (MiscTools.Tile(x, y).WallType == WallID.ObsidianBackUnsafe)
                            {
                                MiscTools.Tile(x, y).WallType = 0;
                            }
                            if (y <= point.Y - 40 && y >= point.Y - 50 || y > point.Y + 40 + spacing.GetNoise(x, y) * 20)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                            //float _background = background.GetNoise(x, y);
                        }

                        if (_terrain / 2 > threshold - 0.5f && MiscTools.Tile(x, y).WallType == 0)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.AshForest)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.LavaUnsafe4;
                            }
                            else MiscTools.Tile(x, y).WallType = WallID.LavaUnsafe3;
                        }

                        if (biomes.FindBiome(x, y) == BiomeID.AshForest && !MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Ash)
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.AshGrass;
                            }
                        }

                        //else if (y > point.Y)
                        //{
                        //    WGTools.GetTile(x, y).active(true);
                        //    //if (WGTools.GetTile(x, y).wall == 0 && WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        //    //{
                        //    //    WGTools.GetTile(x, y).wall = WallID.LavaUnsafe1;
                        //    //}
                        //}

                        //if (WGTools.Tile(x, y).WallType == WallID.ObsidianBackUnsafe && WGTools.Tile(x, y).HasTile == false && WorldGen.genRand.NextBool(2))
                        //{
                        //    int adjacentTiles = 0;
                        //    if (WGTools.Tile(x + 1, y).HasTile && WGTools.Tile(x + 1, y).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }
                        //    if (WGTools.Tile(x, y + 1).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }
                        //    if (WGTools.Tile(x - 1, y).HasTile && WGTools.Tile(x - 1, y).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }
                        //    if (WGTools.Tile(x, y - 1).HasTile && WGTools.Tile(x, y - 1).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }

                        //    if (adjacentTiles >= 1)
                        //    {
                        //        WorldGen.PlaceTile((int)x, (int)y, TileID.ExposedGems);
                        //    }
                        //}
                    }

                    MiscTools.Tile(x, y).LiquidType = 1;
                }
            }
            #endregion
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
            while (attempts < 10000 && trees < 8)
            {
                int x = WorldGen.genRand.Next(Desert.OasisX - 60, Desert.OasisX + 61);
                int y = (int)(Main.worldSurface * 0.5f);
                while (!MiscTools.HasTile(x, y + 1, TileID.Sand) && y < Main.worldSurface)
                {
                    y++;
                }

                if (!Main.tile[x, y].HasTile && MiscTools.HasTile(x, y + 1, TileID.Sand) && WorldGen.SolidTile3(x, y + 1) && y < Main.worldSurface)
                {
                    attempts++;

                    WorldGen.PlaceTile(x, y, TileID.Saplings, style: 6);
                    WorldGen.AttemptToGrowTreeFromSapling(x, y, false);
                    //WorldGen.TryGrowingTreeByType(TileID.PalmTree, x, y + 1);

                    if (MiscTools.HasTile(x, y, TileID.PalmTree))
                    {
                        attempts = 0;
                        trees++;
                    }
                    else WorldGen.KillTile(x, y);
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
                        if (biomes.FindBiome(x, y) == BiomeID.AshForest)
                        {
                            if (MiscTools.Tile(x, y + 1).TileType == TileID.AshGrass && WorldGen.genRand.NextBool(2))
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

    public class Hell : GenPass
    {
        public Hell(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Underworld");

            FastNoiseLite terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            terrain.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            terrain.SetFrequency(0.03f);
            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrain.SetFractalOctaves(3);
            //terrain.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            //terrain.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);

            FastNoiseLite elevation = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            elevation.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            elevation.SetFrequency(0.01f);
            elevation.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite distance = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            distance.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            distance.SetFrequency(0.06f);
            distance.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite background = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            background.SetFrequency(0.01f);
            background.SetFractalType(FastNoiseLite.FractalType.Ridged);
            background.SetFractalWeightedStrength(-0.5f);
            background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
            background.SetFractalOctaves(2);

            for (float y = Main.maxTilesY - 250; y < Main.maxTilesY - 21; y++)
            {
                progress.Set((y - (Main.maxTilesY - 200)) / 200);

                for (float x = 20; x < Main.maxTilesX - 20; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Underworld || biomes.FindBiome(x, y) == BiomeID.AshForest)
                    {
                        Vector2 point = new Vector2(x, Main.maxTilesY - 150 + elevation.GetNoise(x, y) * 30 + WorldGen.genRand.Next(2));
                        float threshold;

                        if (y > point.Y)
                        {
                            threshold = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(x, y), point) / (50 + distance.GetNoise(x, y) * 50), 0, 1);
                        }
                        else threshold = MathHelper.Clamp(1 - Vector2.Distance(new Vector2(x, y), point) / (50 + distance.GetNoise(x, y) * 50), 0, 1);

                        float _terrain = terrain.GetNoise(x, y * 2);
                        if (_terrain < threshold - 0.25f && MiscTools.Tile(x, y).TileType != ModContent.TileType<Hardstone>())
                        {
                            MiscTools.Tile(x, y).HasTile = false;
                            if (MiscTools.Tile(x, y).WallType == WallID.ObsidianBackUnsafe)
                            {
                                MiscTools.Tile(x, y).WallType = 0;
                            }
                            if (y <= point.Y - 40 && y >= point.Y - 50 || y > point.Y + 40 + distance.GetNoise(x, y) * 20)
                            {
                                MiscTools.Tile(x, y).LiquidAmount = 255;
                            }
                            //float _background = background.GetNoise(x, y);
                        }

                        if (_terrain / 2 > threshold - 0.6f && MiscTools.Tile(x, y).WallType == 0)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.AshForest)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.LavaUnsafe4;
                            }
                            else if (_terrain / 2 > threshold - 0.5f)
                            {
                                MiscTools.Tile(x, y).WallType = WallID.LavaUnsafe1;
                            }
                            else MiscTools.Tile(x, y).WallType = WallID.LavaUnsafe3;
                        }

                        if (biomes.FindBiome(x, y) == BiomeID.AshForest && !MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Ash)
                            {
                                MiscTools.Tile(x - 1, y - 1).TileType = TileID.AshGrass;
                            }
                        }

                        //else if (y > point.Y)
                        //{
                        //    WGTools.GetTile(x, y).active(true);
                        //    //if (WGTools.GetTile(x, y).wall == 0 && WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        //    //{
                        //    //    WGTools.GetTile(x, y).wall = WallID.LavaUnsafe1;
                        //    //}
                        //}

                        //if (WGTools.Tile(x, y).WallType == WallID.ObsidianBackUnsafe && WGTools.Tile(x, y).HasTile == false && WorldGen.genRand.NextBool(2))
                        //{
                        //    int adjacentTiles = 0;
                        //    if (WGTools.Tile(x + 1, y).HasTile && WGTools.Tile(x + 1, y).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }
                        //    if (WGTools.Tile(x, y + 1).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }
                        //    if (WGTools.Tile(x - 1, y).HasTile && WGTools.Tile(x - 1, y).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }
                        //    if (WGTools.Tile(x, y - 1).HasTile && WGTools.Tile(x, y - 1).TileType == TileID.Obsidian)
                        //    {
                        //        adjacentTiles++;
                        //    }

                        //    if (adjacentTiles >= 1)
                        //    {
                        //        WorldGen.PlaceTile((int)x, (int)y, TileID.ExposedGems);
                        //    }
                        //}
                    }

                    if (biomes.FindBiome(x, y) != BiomeID.Granite)
                    {
                        MiscTools.Tile(x, y).LiquidType = 1;
                    }
                }
            }
        }
    }

    public class HellStructures : GenPass
    {
        public HellStructures(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        List<Marker> markers;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int structureCount;

            #region treasurevault
            structureCount = Main.maxTilesX / 525;
            while (structureCount > 0)
            {
                #region spawnconditions
                int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.3f), (int)(Main.maxTilesX * 0.7f));
                int structureY = Main.maxTilesY - 90;

                bool valid = true;
                while (!valid)
                {
                    if (MiscTools.Tile(structureX, structureY).HasTile)
                    {
                        valid = true;
                    }
                    if (MiscTools.Tile(structureX, structureY).LiquidAmount == 255)
                    {
                        valid = false;
                        break;
                    }
                    structureY++;
                }
                valid = false;
                while (!valid)
                {
                    valid = true;
                    structureY--;
                    for (int x = -5; x <= 5; x++)
                    {
                        if (MiscTools.Tile(structureX + x, structureY).HasTile || MiscTools.Tile(structureX + x, structureY).LiquidAmount == 255)
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!GenVars.structures.CanPlace(new Rectangle(structureX - 25, structureY, 50, Main.maxTilesY - 40 - structureY)))
                {
                    valid = false;
                }

                #endregion

                #region structure
                if (valid)
                {
                    markers = new List<Marker>();

                    Rectangle location = new Rectangle(structureX - 75, structureY, 150, Main.maxTilesY - 45 - structureY);

                    GenVars.structures.AddStructure(new Rectangle(structureX - 7, structureY - 11, 15, 12));

                    AddMarker(structureX, structureY, 3, 2, location);

                    for (int i = 0; i < 200; i++)
                    {
                        int index = WorldGen.genRand.Next(0, markers.Count);
                        PlaceRoom(index, location);
                    }

                    while (markers.Count > 0)
                    {
                        RemoveMarker(0);
                    }

                    Fill(new Vector2(structureX, structureY + 25), 25);
                    Fill(new Vector2(structureX, structureY - 10), 10, true);

                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/treasurevault/entrance", new Point16(structureX - 7, structureY - 11), ModContent.GetInstance<Remnants>());

                    structureCount--;
                }
                #endregion
            }
            LavaPools();
            Spikes();
            #region cleanup
            for (int y = Main.maxTilesY - 200; y < Main.maxTilesY - 20; y++)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    Tile tile = MiscTools.Tile(x, y);

                    if (MiscTools.Tile(x, y).WallType == WallID.ObsidianBrick)
                    {
                        MiscTools.Tile(x, y).WallType = WallID.ObsidianBrickUnsafe;
                    }
                    if (MiscTools.Tile(x, y).WallType == WallID.HellstoneBrick)
                    {
                        MiscTools.Tile(x, y).WallType = WallID.HellstoneBrickUnsafe;
                    }

                    if (!GenVars.structures.CanPlace(new Rectangle(x - 2, y - 2, 5, 5)))
                    {
                        if (tile.TileType != TileID.ObsidianBrick && tile.TileType != TileID.HellstoneBrick && tile.TileType != TileID.Spikes && tile.WallType != WallID.ObsidianBrickUnsafe && tile.WallType != WallID.HellstoneBrickUnsafe)
                        {
                            if (MiscTools.Solid(x, y) && tile.TileType != TileID.ClosedDoor)
                            {
                                tile.TileType = (ushort)ModContent.TileType<HellishBrick>();
                                if (tile.LiquidAmount == 255)
                                {
                                    tile.HasTile = true;
                                }
                            }
                        }
                        tile.LiquidAmount = 0;
                    }
                }
            }
            #endregion
            #region objects 
            int objects = Main.maxTilesX / 525 * 20;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next(Main.maxTilesY - 100, Main.maxTilesY - 40);

                bool valid = true;

                if (Framing.GetTileSafely(x, y).TileType == TileID.Banners || Framing.GetTileSafely(x, y - 1).TileType != TileID.ObsidianBrick || MiscTools.Tile(x, y).WallType != WallID.ObsidianBrickUnsafe)
                {
                    valid = false;
                }
                else for (int i = -1; i <= 1; i++)
                    {
                        for (int j = 0; j <= 5; j++)
                        {
                            if (WorldGen.SolidTile3(x + i, y + j) || Framing.GetTileSafely(x + i, y + j).TileType == TileID.Banners)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (!valid) { break; }
                    }

                if (valid)
                {
                    //WorldGen.PlaceObject(x, y, TileID.Banners, style: Main.rand.Next(16, 22));
                    WorldGen.PlaceObject(x, y, TileID.Banners, style: 16);
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Banners)
                    {
                        objects--;
                    }
                }
            }
            #endregion
            #endregion
            #region lavageode
            //structureCount = Main.maxTilesX / 200;
            //while (structureCount > 0)
            //{
            //    int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            //    int structureY = WorldGen.genRand.Next(Main.maxTilesY - 190, Main.maxTilesY - 150);
            //    int size = WorldGen.genRand.Next(3, 10);
            //    Rectangle structure = new Rectangle(structureX - size, structureY - size, size * 2, size * 2);

            //    bool valid = true;

            //    if (!WorldGen.structures.CanPlace(structure))
            //    {
            //        valid = false;
            //    }
            //    else
            //    {
            //        int num = 0;
            //        for (int y = structure.Top; y < structure.Bottom; y++)
            //        {
            //            for (int x = structure.Left; x < structure.Right; x++)
            //            {
            //                if (Framing.GetTileSafely(x, y).HasTile && Main.tileSolid[Framing.GetTileSafely(x, y).TileType])
            //                {
            //                    num++;
            //                }
            //                else
            //                {
            //                    num--;
            //                }
            //            }
            //        }
            //        if (num < (size * size) / 2)
            //        {
            //            valid = false;
            //        }
            //    }

            //    if (valid)
            //    {
            //        FastNoiseLite noise = new FastNoiseLite();
            //        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //        noise.SetFrequency(0.1f);
            //        noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            //        WorldGen.structures.AddProtectedStructure(structure);

            //        for (int y = structure.Top; y < structure.Bottom; y++)
            //        {
            //            for (int x = structure.Left; x < structure.Right; x++)
            //            {
            //                float threshold = (1 - (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) / size)) * 2;
            //                if (noise.GetNoise(x, y) <= threshold && WGTools.GetTile(x, y).TileType != TileID.ObsidianBrick && WGTools.GetTile(x, y).WallType != WallID.ObsidianBrickUnsafe)
            //                {
            //                    if (noise.GetNoise(x, y) <= threshold - 0.6f)
            //                    {
            //                        WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<devtile>();
            //                    }
            //                    else WGTools.GetTile(x, y).TileType = TileID.Obsidian;
            //                    WGTools.GetTile(x, y).HasTile = true;
            //                }
            //            }
            //        }

            //        for (int y = structure.Top; y < structure.Bottom; y++)
            //        {
            //            for (int x = structure.Left; x < structure.Right; x++)
            //            {
            //                if (WGTools.GetTile(x, y).TileType == TileID.Obsidian || WGTools.GetTile(x, y).TileType == ModContent.TileType<devtile>())
            //                {
            //                    if (WGTools.SurroundingTilesActive(x, y, true)) { WGTools.GetTile(x, y).WallType = WallID.ObsidianBackUnsafe; }
            //                }
            //            }
            //        }

            //        for (int y = structure.Top; y < structure.Bottom; y++)
            //        {
            //            for (int x = structure.Left; x < structure.Right; x++)
            //            {
            //                if (WGTools.GetTile(x, y).TileType == ModContent.TileType<devtile>())
            //                {
            //                    WGTools.GetTile(x, y).HasTile = false;
            //                }
            //            }
            //        }

            //        for (int y = structure.Top; y < structure.Bottom; y++)
            //        {
            //            for (int x = structure.Left; x < structure.Right; x++)
            //            {
            //                if (WGTools.GetTile(x, y).WallType == WallID.ObsidianBackUnsafe && WGTools.GetTile(x, y).HasTile == false && WorldGen.genRand.NextBool(2))
            //                {
            //                    int adjacentTiles = 0;
            //                    if (Framing.GetTileSafely(x + 1, y).HasTile && Framing.GetTileSafely(x + 1, y).TileType == TileID.Obsidian)
            //                    {
            //                        adjacentTiles++;
            //                    }
            //                    if (Framing.GetTileSafely(x, y + 1).HasTile && Framing.GetTileSafely(x, y + 1).TileType == TileID.Obsidian)
            //                    {
            //                        adjacentTiles++;
            //                    }
            //                    if (Framing.GetTileSafely(x - 1, y).HasTile && Framing.GetTileSafely(x - 1, y).TileType == TileID.Obsidian)
            //                    {
            //                        adjacentTiles++;
            //                    }
            //                    if (Framing.GetTileSafely(x, y - 1).HasTile && Framing.GetTileSafely(x, y - 1).TileType == TileID.Obsidian)
            //                    {
            //                        adjacentTiles++;
            //                    }

            //                    if (adjacentTiles >= 1)
            //                    {
            //                        WorldGen.PlaceTile(x, y, TileID.ExposedGems);
            //                    }
            //                }
            //            }
            //        }

            //        structureCount--;
            //    }
            //}
            #endregion
            #region cage
            //structureCount = (int)(Main.maxTilesX * 0.8f) / 100;
            //while (structureCount > 0)
            //{
            //    #region spawnconditions
            //    int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            //    int structureY = WorldGen.genRand.Next(Main.maxTilesY - 170, Main.maxTilesY - 140);

            //    bool valid = true;

            //    for (int j = -1; j <= 7; j++)
            //    {
            //        for (int i = -3; i <= 3; i++)
            //        {
            //            if (WGTools.Tile(structureX + i, structureY + j).HasTile)
            //            {
            //                valid = false;
            //                break;
            //            }
            //            else if (i >= -1 && i <= 1 && j <= 6 && WGTools.Tile(structureX + i, structureY + j).WallType != 0)
            //            {
            //                valid = false;
            //                break;
            //            }
            //        }
            //        if (!valid)
            //        {
            //            break;
            //        }
            //    }
            //    #endregion

            //    #region structure
            //    if (valid)
            //    {
            //        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/cage", new Point16(structureX - 2, structureY), ModContent.GetInstance<Remnants>());
            //        int y = structureY - 1;
            //        while (!WGTools.Tile(structureX, y).HasTile)
            //        {
            //            WorldGen.PlaceTile(structureX, y, TileID.Chain);
            //            y--;
            //        }
            //        WGTools.Rectangle(structureX - 1, y, structureX + 1, y, TileID.IronBrick);
            //        WGTools.Tile(structureX, y).WallType = WallID.IronBrick;

            //        structureCount--;
            //    }
            //    #endregion
            //}
            #endregion

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null && chest.y >= Main.maxTilesY - 200)
                {
                    Chest.DestroyChestDirect(chest.x, chest.y, i);
                    WorldGen.KillTile(chest.x, chest.y);
                }
            }

            #region objects
            int objectCount;
            objectCount = Main.maxTilesX / 525;
            while (objectCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next(Main.maxTilesY - 100, Main.maxTilesY - 40);

                bool valid = true;
                if (Framing.GetTileSafely(x, y).TileType == TileID.Hellforge || MiscTools.Tile(x, y).WallType != WallID.ObsidianBrickUnsafe)
                {
                    valid = false;
                }
                if (WorldGen.SolidTile3(x - 2, y) || WorldGen.SolidTile3(x + 2, y))
                {
                    valid = false;
                }
                for (int i = -1; i <= 1; i++)
                {
                    if (MiscTools.Tile(x + i, y + 1).TileType != TileID.ObsidianBrick)
                    {
                        valid = false;
                    }
                }

                if (valid)
                {
                    WorldGen.PlaceObject(x, y, TileID.Hellforge);
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Hellforge)
                    {
                        objectCount--;
                    }
                }
            }
            objectCount = Main.maxTilesX / 525;
            while (objectCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next(Main.maxTilesY - 100, Main.maxTilesY - 40);

                bool valid = true;
                if (Framing.GetTileSafely(x, y).TileType == TileID.Containers || MiscTools.Tile(x, y).WallType != WallID.ObsidianBrickUnsafe)
                {
                    valid = false;
                }
                if (WorldGen.SolidTile3(x - 1, y) || WorldGen.SolidTile3(x + 2, y))
                {
                    valid = false;
                }
                for (int i = 0; i <= 1; i++)
                {
                    if (MiscTools.Tile(x + i, y + 1).TileType != TileID.ObsidianBrick)
                    {
                        valid = false;
                    }
                }

                if (valid)
                {
                    int chestIndex = WorldGen.PlaceChest(x, y, notNearOtherChests: true, style: 4);
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    {
                        #region chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        int[] specialItems = new int[6];
                        specialItems[0] = ItemID.HellwingBow;
                        specialItems[1] = ItemID.LavaCharm;
                        specialItems[2] = ItemID.Sunfury;
                        specialItems[3] = ItemID.Flamelash;
                        specialItems[4] = ItemID.DarkLance;
                        specialItems[5] = ItemID.FlowerofFire;

                        int specialItem = specialItems[objectCount % specialItems.Length];
                        itemsToAdd.Add((specialItem, 1));

                        StructureTools.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.ObsidianSkinPotion, ItemID.InfernoPotion }, true);

                        itemsToAdd.Add((ItemID.Hellstone, Main.rand.Next(3, 6) * 3));
                        if (Main.rand.NextBool(2))
                        {
                            itemsToAdd.Add((ItemID.Meteorite, Main.rand.Next(6, 12) * 3));
                        }
                        if (Main.rand.NextBool(2))
                        {
                            itemsToAdd.Add((ItemID.Obsidian, Main.rand.Next(3, 6)));
                        }

                        StructureTools.FillChest(chestIndex, itemsToAdd);
                        #endregion

                        objectCount--;
                    }
                }
            }
            #endregion
        }

        #region functions
        private void PlaceRoom(int index, Rectangle location)
        {
            Marker savedMarker = markers[index];

            #region setup
            int x = (int)savedMarker.position.X;
            int y = (int)savedMarker.position.Y;

            int id = WorldGen.genRand.Next(1, 9);
            if (savedMarker.type == 1)
            {
                id = -1;
            }
            else if (savedMarker.type == 2)
            {
                id = 0;
            }

            Rectangle room = new Rectangle(x, y, 13, 9);
            int doorY = 0;
            if (id == -1)
            {
                room = new Rectangle(x, y, 5, 5);
            }
            else if (id == 1)
            {
                room = new Rectangle(x, y, 21, 11);
            }
            else if (id == 2)
            {
                room = new Rectangle(x, y, 26, 11);
            }
            else if (id == 3)
            {
                room = new Rectangle(x, y, 19, 11);
            }
            else if (id == 4)
            {
                room = new Rectangle(x, y, 35, 15);
            }
            else if (id == 5)
            {
                room = new Rectangle(x, y, 31, 15);
            }
            else if (id == 6)
            {
                room = new Rectangle(x, y, 32, 15);
            }
            else if (id == 7)
            {
                room = new Rectangle(x, y, 41, 19);
            }
            else if (id == 8)
            {
                room = new Rectangle(x, y, 45, 19);
            }

            doorY += room.Height - 3;

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

            if (savedMarker.direction == 2)
            {
                room.X += 2;
            }
            else if (savedMarker.direction == 4)
            {
                room.X -= 2;
            }
            #endregion

            if (GenVars.structures.CanPlace(new Rectangle(room.X + 1, room.Y + 1, room.Width - 2, room.Height - 2)))
            {
                Point16 position = new Point16(room.X, room.Y);

                Fill(room.Center.ToVector2(), room.Height);

                RemoveMarker(index);
                if (id == -1)
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/treasurevault/shaft", position, ModContent.GetInstance<Remnants>());
                    if (savedMarker.direction != 3)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y, 1, 1, location);
                    }
                    if (savedMarker.direction != 1)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y + (room.Height - 1), 3, 1, location);
                    }
                }
                if (id == 0)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/centralshaft", position, ModContent.GetInstance<Remnants>());
                    if (savedMarker.direction != 3)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y, 1, 2, location);
                    }
                    if (savedMarker.direction != 1)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y + (room.Height - 1), 3, 2, location);
                    }
                }
                else if (id == 1)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/small1", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 2)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/small2", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 3)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/small3", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 4)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/medium1", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 5)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/medium2", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 6)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/medium3", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 7)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/large1", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 8)
                {
                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureRandom("Content/World/Structures/Common/treasurevault/large2", position, ModContent.GetInstance<Remnants>());
                }

                if (savedMarker.direction != 2)
                {
                    AddMarker(room.X, room.Y + doorY, 4, id > 0 ? 1 : 0, location);
                }
                if (savedMarker.direction != 4)
                {
                    AddMarker(room.X + (room.Width - 1), room.Y + doorY, 2, id > 0 ? 1 : 0, location);
                }

                if (savedMarker.direction == 2)
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/treasurevault/doorway", new Point16(room.X - 2, room.Y + (doorY - 2)), ModContent.GetInstance<Remnants>());
                }
                else if (savedMarker.direction == 4)
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/Common/treasurevault/doorway", new Point16(room.X + (room.Width - 1), room.Y + (doorY - 2)), ModContent.GetInstance<Remnants>());
                }

                GenVars.structures.AddProtectedStructure(room);

                if (room.Height == 11)
                {
                    //for (int j = room.Top + 1; j < room.Bottom; j++)
                    //{
                    //    for (int i = room.Left + 1; i < room.Right; i++)
                    //    {
                    //        Tile tile = Main.tile[i, j];
                    //        if (tile.WallType == ModContent.WallType<spikybars>())
                    //        {
                    //            tile.WallType = WallID.ObsidianBrickUnsafe;
                    //        }
                    //    }
                    //}
                    //int objects = 2;
                    //while (objects > 0)
                    //{
                    //    int i = WorldGen.genRand.Next(room.Left + 6, room.Right - 5);
                    //    int j = room.Bottom - 6;
                    //    int type = 0;
                    //    int style = 0;
                    //    switch (WorldGen.genRand.Next(3))
                    //    {
                    //        case 0:
                    //            type = TileID.Painting3X3;
                    //            break;
                    //        case 1:
                    //            type = TileID.Painting3X2;
                    //            break;
                    //        case 2:
                    //            type = TileID.Painting2X3; i = WorldGen.genRand.Next(room.Left + 6, room.Right - 4);
                    //            break;
                    //    }
                    //    i--;

                    //    if (!WGTools.GetTile(i, j).HasTile && !WGTools.GetTile(i - 2, j).HasTile && !WGTools.GetTile(i + (type == TileID.Painting2X3 ? 1 : 2), j).HasTile)
                    //    {
                    //        if (type == TileID.Painting3X3)
                    //        {
                    //            switch (WorldGen.genRand.Next(5))
                    //            {
                    //                case 0:
                    //                    style = 27; break;
                    //                case 1:
                    //                    style = 29; break;
                    //                case 2:
                    //                    style = 30; break;
                    //                case 3:
                    //                    style = 31; break;
                    //                case 4:
                    //                    style = 32; break;
                    //            }
                    //        }
                    //        else if (type == TileID.Painting3X2)
                    //        {
                    //            switch (WorldGen.genRand.Next(3))
                    //            {
                    //                case 0:
                    //                    style = 0; break;
                    //                case 1:
                    //                    style = 16; break;
                    //                case 2:
                    //                    style = 17; break;
                    //            }
                    //        }
                    //        if (type == TileID.Painting2X3)
                    //        {
                    //            switch (WorldGen.genRand.Next(3))
                    //            {
                    //                case 0:
                    //                    style = 1; break;
                    //                case 1:
                    //                    style = 2; break;
                    //                case 2:
                    //                    style = 4; break;
                    //            }
                    //        }
                    //        WorldGen.PlaceObject(i, j, type, style: style);
                    //        if (WGTools.GetTile(i, j).TileType == type)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                }
            }
        }

        private void AddMarker(int x, int y, int direction, int type, Rectangle location)
        {
            markers.Add(new Marker(new Vector2(x, y), direction, type));

            if (x<= location.Left || x >= location.Right || direction != 3 && y <= Main.maxTilesY - 90 || y >= Main.maxTilesY - 50)
            {
                RemoveMarker(markers.Count - 1);
            }
        }

        private void RemoveMarker(int index)
        {
            int x = (int)markers[index].position.X;
            int y = (int)markers[index].position.Y;
            if (markers[index].direction == 2 || markers[index].direction == 4)
            {
                WorldGen.KillTile(x, y);
                MiscTools.Rectangle(x, y - 1, x, y + 1, TileID.ObsidianBrick);
            }
            else if (markers[index].direction == 1 && !WorldGen.SolidTile3(x, y - 1))
            {
                if (markers[index].type == 2)
                {
                    MiscTools.Rectangle(x - 2, y, x + 2, y, TileID.Platforms, style: 13);
                }
                else MiscTools.Rectangle(x - 1, y, x + 1, y, TileID.Platforms, style: 13);
            }
            else
            {
                if (markers[index].type == 2)
                {
                    MiscTools.Rectangle(x - 2, y, x + 2, y, TileID.ObsidianBrick);
                }
                else MiscTools.Rectangle(x - 1, y, x + 1, y, TileID.ObsidianBrick);
            }
            //if (Framing.GetTileSafely(x, y).TileType != TileID.Platforms)
            //{
            //    WGTools.DrawRectangle(x - 1, y, x + 1, y, TileID.ObsidianBrick);
            //}

            markers.RemoveAt(index);
        }

        private void Fill(Vector2 position, float size, bool empty = false)
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFrequency(0.05f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalLacunarity(1.75f);

            for (int y = (int)(position.Y - size * 2); y <= position.Y + size * 2; y++)
            {
                for (int x = (int)(position.X - size * 2); x <= position.X + size * 2; x++)
                {
                    float threshold = Vector2.Distance(position, new Vector2(x, y)) / size;
                    Tile tile = MiscTools.Tile(x, y);

                    if (noise.GetNoise(x * 3, y) / 2 <= 1 - threshold)
                    {
                        if (empty)
                        {
                            if (!tile.HasTile && tile.TileType != TileID.ObsidianBrick && tile.WallType != WallID.ObsidianBrickUnsafe && tile.WallType != WallID.HellstoneBrickUnsafe && tile.WallType != WallID.ObsidianBrick && tile.WallType != WallID.HellstoneBrick && tile.WallType != ModContent.WallType<spikybars>())
                            {
                                tile.HasTile = false;
                            }
                        }
                        else
                        {
                            if (!tile.HasTile && tile.TileType != TileID.ObsidianBrick && tile.WallType != WallID.ObsidianBrickUnsafe && tile.WallType != WallID.HellstoneBrickUnsafe && tile.WallType != WallID.ObsidianBrick && tile.WallType != WallID.HellstoneBrick && tile.WallType != ModContent.WallType<spikybars>())
                            {
                                tile.HasTile = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void LavaPools()
        {
            int attempts = 0;
            int structureCount = 0;
            while (structureCount < Main.maxTilesX / 350 * 5)
            {
                attempts++;

                int width = WorldGen.genRand.Next(9, 28);
                int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int structureY = WorldGen.genRand.Next(Main.maxTilesY - 100, Main.maxTilesY - 40);
                int num9 = structureX;
                if ((Main.tile[structureX, structureY].WallType == WallID.ObsidianBrickUnsafe || Main.tile[structureX, structureY].WallType == ModContent.WallType<spikybars>()) && !Main.tile[structureX, structureY].HasTile)
                {
                    for (; !MiscTools.Tile(structureX, structureY).HasTile; structureY += 1)
                    {
                    }
                    bool valid = true;
                    for (int i = structureX - 1; i <= structureX + width + 1; i++)
                    {
                        if (MiscTools.Tile(i, structureY - 1).HasTile || !MiscTools.Tile(i, structureY + 1).HasTile || MiscTools.Tile(i, structureY - 1).WallType != WallID.ObsidianBrickUnsafe)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        MiscTools.Rectangle(structureX - 1, structureY + 1, structureX + width + 1, structureY + 2, TileID.HellstoneBrick);
                        MiscTools.Rectangle(structureX, structureY, structureX + width, structureY + 1, -1);
                        MiscTools.Rectangle(structureX, structureY, structureX + width, structureY, wall: WallID.ObsidianBrickUnsafe);
                        MiscTools.Rectangle(structureX, structureY + 1, structureX + width, structureY + 1, wall: WallID.HellstoneBrickUnsafe, liquid: 255, liquidType: 1);

                        WorldGen.PlaceTile(structureX, structureY, TileID.Platforms, style: 13); WorldGen.PlaceTile(structureX + width, structureY, TileID.Platforms, style: 13);

                        GenVars.structures.AddProtectedStructure(new Rectangle(structureX - 1, structureY, width + 3, 3));

                        structureCount++;
                        attempts = 0;
                    }
                    else if (attempts > 10000)
                    {
                        break;
                    }
                }
            }
        }

        private void Spikes()
        {
            int num4 = 0;
            int num5 = 1000;
            int num6 = 0;
            while (num6 < Main.maxTilesX / 350 * 10)
            {
                num4++;
                int num7 = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f),(int)(Main.maxTilesX * 0.9f));
                int num8 = WorldGen.genRand.Next(Main.maxTilesY - 100, Main.maxTilesY - 40);
                int num9 = num7;
                if ((Main.tile[num7, num8].WallType == WallID.ObsidianBrickUnsafe || Main.tile[num7, num8].WallType == ModContent.WallType<spikybars>()) && !Main.tile[num7, num8].HasTile)
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
                                WorldGen.PlaceTile(num7, num8 - num10, 48);
                            }
                            num7--;
                            num12--;
                        }
                        num12 = WorldGen.genRand.Next(5, 13);
                        num7 = num9 + 1;
                        while (SpikeValidation(num7, num8) && Main.tile[num7 + 1, num8].HasTile && Main.tile[num7, num8 + num10].HasTile && Main.tile[num7, num8].HasTile && !Main.tile[num7, num8 - num10].HasTile && num12 > 0)
                        {
                            Main.tile[num7, num8].TileType = 48;
                            if (!Main.tile[num7 - 1, num8 - num10].HasTile && !Main.tile[num7 + 1, num8 - num10].HasTile)
                            {
                                WorldGen.PlaceTile(num7, num8 - num10, 48);
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
            for (int i = x - 3; i <= x + 3; i++)
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
            return MiscTools.Solid(x, y) && MiscTools.Tile(x, y).TileType == TileID.ObsidianBrick;
        }

        internal struct Marker
        {
            public Vector2 position;
            public int direction;
            public int type;
            public Marker(Vector2 _position, int _direction, int _type)
            {
                position = _position;
                direction = _direction;
                type = _type;
            }
        }
    }

    public class SpikeBalls : GenPass
    {
        public SpikeBalls(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int structureCount;
            #region spikeball
            structureCount = (int)(Main.maxTilesX * 0.8f) / 25;
            while (structureCount > 0)
            {
                #region spawnconditions
                int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int structureY = WorldGen.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY - 60);

                bool valid = true;

                if (MiscTools.Tile(structureX, structureY).LiquidAmount != 255)
                {
                    valid = false;
                }
                else for (int j = -2; j <= 3; j++)
                    {
                        for (int i = -2; i <= 2; i++)
                        {
                            if (MiscTools.Tile(structureX + i, structureY + j).HasTile)
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
                #endregion

                #region structure
                if (valid)
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/spikeball", new Point16(structureX - 1, structureY - 2), ModContent.GetInstance<Remnants>());

                    structureCount--;
                }
                #endregion
            }
            #endregion
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
