using Microsoft.Xna.Framework;
using Remnants.Tiles;
using Remnants.Tiles.Blocks;
using Remnants.Tiles.Plants;
using Remnants.Walls;
using Remnants.Walls.Vanity;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Remnants.Worldgen.PrimaryBiomes;

namespace Remnants.Worldgen
{
    public class BiomeMap : ModSystem
    {
        public int[,] biomeMap;

        public int scale => 50;
        public int width => Main.maxTilesX / scale;
        public int height => Main.maxTilesY / scale;

        public FastNoiseLite blendingNoise = new FastNoiseLite();

        private float[,] blendingX;
        private float[,] blendingY;
        private int blendDistance => ModContent.GetInstance<Client>().ExperimentalWorldgen ? 0 : 35;

        public FastNoiseLite materialsNoise = new FastNoiseLite();

        public float[,] materials;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            RemWorld.InsertPass(tasks, new BiomeMapSetup("Biome Map Setup", 1), 1);
        }

        internal class BiomeMapSetup : GenPass
        {
            public BiomeMapSetup(string name, float loadWeight) : base(name, loadWeight)
            {
            }
            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Setting up biome map";

                BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

                biomes.biomeMap = new int[biomes.width, biomes.height];

                biomes.blendingNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                biomes.blendingNoise.SetSeed(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                biomes.blendingNoise.SetFrequency(0.025f);
                biomes.blendingNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
                //biomes.blending.SetFractalOctaves(3);
                //biomes.blendingNoise.SetFractalLacunarity(2.25f);

                biomes.blendingX = new float[Main.maxTilesX, Main.maxTilesY];
                biomes.blendingY = new float[Main.maxTilesX, Main.maxTilesY];

                biomes.materialsNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                biomes.materialsNoise.SetSeed(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
                biomes.materialsNoise.SetFrequency(0.025f);
                biomes.materialsNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
                //biomes.materialsNoise.SetFractalLacunarity(2.25f);

                biomes.materials = new float[Main.maxTilesX, Main.maxTilesY];

                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    progress.Set((float)y / Main.maxTilesY);

                    for (int x = 0; x < Main.maxTilesX; x++)
                    {
                        biomes.blendingX[x, y] = biomes.blendingNoise.GetNoise(x, y * 2 + 999);
                        biomes.blendingY[x, y] = biomes.blendingNoise.GetNoise(x + 999, y * 2);

                        biomes.materials[x, y] = biomes.materialsNoise.GetNoise(x, y * 2);
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

                float tundraCorruptionDistance = 0.225f;
                bool tundraCorruptionSwap = WorldGen.genRand.NextBool(2); //GenVars.dungeonSide == 1;

                Tundra.X = biomes.width / 2;
                Tundra.X += (int)(biomes.width * tundraCorruptionDistance * (GenVars.dungeonSide != 1 ? -1 : 1));
                Tundra.X += (int)(biomes.width * 0.05f * (!tundraCorruptionSwap ? -1 : 1));

                Corruption.X = biomes.width / 2;
                Corruption.X += (int)(biomes.width * tundraCorruptionDistance * (Tundra.X < biomes.width / 2 ? -1 : 1));
                Corruption.X += (int)(biomes.width * 0.075f * (tundraCorruptionSwap ? -1 : 1));

                float jungleDesertDistance = 0.25f;// WorldGen.genRand.NextFloat(0.275f, 0.325f);
                bool jungleDesertSwap = WorldGen.genRand.NextBool(2); //GenVars.dungeonSide == 1;

                Jungle.Center = biomes.width / 2;
                Jungle.Center += (int)(biomes.width * jungleDesertDistance * (GenVars.dungeonSide == 1 ? -1 : 1));
                Jungle.Center += (int)(biomes.width * 0.05f * (!jungleDesertSwap ? -1 : 1));

                Desert.Center = biomes.width / 2;
                Desert.Center += (int)(biomes.width * jungleDesertDistance * (Jungle.Center < biomes.width / 2 ? -1 : 1));
                Desert.Center += (int)(biomes.width * 0.1f * (jungleDesertSwap ? -1 : 1));

                for (int y = biomes.height - 4; y < biomes.height; y++)
                {
                    for (int x = 0; x < biomes.width; x++)
                    {
                        biomes.AddBiome(x, y, BiomeID.Underworld);
                    }
                }
                for (int y = biomes.height - 6; y < biomes.height - 4; y++)
                {
                    for (int x = 0; x < biomes.width; x++)
                    {
                        biomes.AddBiome(x, y, BiomeID.Obsidian);
                    }
                }

                for (int y = 0; y < biomes.height - 6; y++)
                {
                    for (int x = 0; x <= 6; x++)
                    {
                        biomes.AddBiome(x, y, BiomeID.Beach);
                    }

                    for (int x = biomes.width - 7; x < biomes.width; x++)
                    {
                        biomes.AddBiome(x, y, BiomeID.Beach);
                    }
                }
            }
        }

        public void AddBiome(int i, int j, int type)
        {
            biomeMap[i, j] = type;
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
                return biomeMap[(int)(i / scale), (int)(j / scale)];
            }
            else
            {
                int i = (int)MathHelper.Clamp(x + blendingX[(int)x, (int)y] * blendDistance, 20, Main.maxTilesX - 20);
                int j = (int)MathHelper.Clamp(y + blendingY[(int)x, (int)y] * blendDistance, 20, Main.maxTilesY - 20);
                return biomeMap[(int)(i / scale), (int)(j / scale)];
            }
        }

        public int FindLayer(int x, int y)
        {
            return (int)MathHelper.Clamp(y + blendingY[(int)x, (int)y] * blendDistance, 20, Main.maxTilesY - 20) / scale;
        }

        public bool UpdatingBiome(float x, float y, bool[] biomesToUpdate, int type)
        {
            return biomesToUpdate[type] && FindBiome(x, y) == type;
        }

        public int skyLayer => (int)(Main.worldSurface * 0.4) / scale;
        public int surfaceLayer => (int)(Main.worldSurface + 25) / scale;
        public int caveLayer => (int)(Main.rockLayer + 25) / scale;
        public int lavaLayer => GenVars.lavaLine / scale - 1;

        public void UpdateMap(int[] biomes, GenerationProgress progress)
        {
            progress.Message = "Updating biome map";

            FastNoiseLite caves1 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            FastNoiseLite caves2 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            FastNoiseLite caves3 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));

            FastNoiseLite fleshcaves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            fleshcaves.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            //fleshcaves.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Mul);
            fleshcaves.SetFrequency(0.04f);
            fleshcaves.SetFractalType(FastNoiseLite.FractalType.PingPong);
            fleshcaves.SetFractalGain(0.8f);
            fleshcaves.SetFractalWeightedStrength(0.25f);
            fleshcaves.SetFractalPingPongStrength(1.5f);

            FastNoiseLite fossils = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            fossils.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fossils.SetFrequency(0.1f);
            fossils.SetFractalType(FastNoiseLite.FractalType.Ridged);

            FastNoiseLite roots = new FastNoiseLite();
            roots.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            roots.SetFrequency(0.1f);
            roots.SetFractalType(FastNoiseLite.FractalType.FBm);
            roots.SetFractalOctaves(3);

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
                    startY = (int)Main.worldSurface - blendDistance * 2;
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
                if (cal.TryFind<ModTile>("EutrophicSand", out ModTile sand))
                {
                    eutrophicSand = sand.Type;
                }
                if (cal.TryFind<ModTile>("HardenedEutrophicSand", out ModTile hardenedSand))
                {
                    hardenedEutrophicSand = hardenedSand.Type;
                }
                if (cal.TryFind<ModTile>("Navystone", out ModTile stone))
                {
                    navystone = stone.Type;
                }
                if (cal.TryFind<ModTile>("SeaPrism", out ModTile prism))
                {
                    seaPrism = prism.Type;
                }
                if (cal.TryFind<ModTile>("SeaPrismCrystals", out ModTile shard))
                {
                    prismShard = shard.Type;
                }
                if (cal.TryFind<ModTile>("SunkenStalactitesSmall", out ModTile stalac))
                {
                    stalactite = stalac.Type;
                }

                if (cal.TryFind<ModWall>("EutrophicSandWall", out ModWall sandWall))
                {
                    eutrophicSandWall = sandWall.Type;
                }
                if (cal.TryFind<ModWall>("NavystoneWall", out ModWall stoneWall))
                {
                    navystoneWall = stoneWall.Type;
                }
            }


            for (float y = startY; y < Main.maxTilesY - 40; y++)
            {
                progress.Set((y - startY) / (Main.maxTilesY - 20 - startY));

                for (float x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = WGTools.Tile(x, y);

                    int i = (int)MathHelper.Clamp(x + blendingX[(int)x, (int)y] * blendDistance, 20, Main.maxTilesX - 20);
                    int j = (int)MathHelper.Clamp(y + blendingY[(int)x, (int)y] * blendDistance, 20, Main.maxTilesY - 20);

                    int layer = (j / scale);

                    bool beach = (i / scale <= 6 || i / scale >= width - 7);
                    bool underground = layer >= surfaceLayer;
                    bool sky = layer < skyLayer;

                    #region custom
                    if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Hive))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.02f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalOctaves(3);
                        //caves.SetFractalGain(1);
                        caves1.SetFractalLacunarity(2);
                        //caves.SetFractalWeightedStrength(-0.45f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Sub);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves2.SetFrequency(0.015f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves2.SetFractalOctaves(3);
                        caves2.SetFractalLacunarity(2);

                        float _caves = caves1.GetNoise(x, y * 2);
                        float _background = caves1.GetNoise(x + 999, y * 2 + 999);

                        tile.TileType = TileID.Hive;
                        if (_caves < -0.75f)
                        {
                            tile.HasTile = true;
                            if (_caves < -0.85f)
                            {
                                if (WorldGen.genRand.NextBool(25))
                                {
                                    tile.TileType = TileID.JungleGrass;
                                }
                                else tile.TileType = TileID.Mud;
                            }
                            tile.Slope = SlopeType.Solid;
                        }
                        else tile.HasTile = false;
                        if (_background < -0.75f)
                        {
                            tile.WallType = WallID.HiveUnsafe;
                        }
                        else tile.WallType = 0;

                        WGTools.Tile(x, y).LiquidType = 2;
                        if (WorldGen.genRand.NextBool(20))
                        {
                            WGTools.Tile(x, y).LiquidAmount = 255;
                        }
                        else WGTools.Tile(x, y).LiquidAmount = 0;
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
                            caves2.SetFrequency(0.02f);
                            caves2.SetFractalType(FastNoiseLite.FractalType.FBm);
                            caves2.SetFractalOctaves(3);

                            float _caves = caves1.GetNoise(x, y * 2);
                            float _size = (caves3.GetNoise(x, y * 2) / 2) + 1;

                            if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Grass || tile.TileType == TileID.ClayBlock || tile.TileType == TileID.Stone || tile.TileType == TileID.Silt)
                            {
                                if (MaterialBlend(x, y, frequency: 2) < -0.1f)
                                {
                                    if (WorldGen.genRand.NextBool(10))
                                    {
                                        tile.TileType = TileID.ArgonMoss;
                                    }
                                    else tile.TileType = TileID.Stone;
                                }
                                else if (MaterialBlend(x, y, frequency: 2) <= 0.1f)
                                {
                                    tile.TileType = TileID.Coralstone;// Stone;
                                }
                                else tile.TileType = TileID.Sand;
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
                            WGTools.Tile(x, y).LiquidType = 0;
                            WGTools.Tile(x, y).LiquidAmount = 255;

                            if (!WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Stone)
                                {
                                    WGTools.Tile(x - 1, y - 1).TileType = TileID.ArgonMoss;
                                }
                            }
                        }
                        else
                        {
                            tile.HasTile = false;
                            WGTools.Tile(x, y).LiquidType = 0;
                            if (y >= Main.worldSurface - 60)
                            {
                                WGTools.Tile(x, y).LiquidAmount = 255;
                            }
                        }
                        if (layer >= surfaceLayer - 1)
                        {
                            float _background = caves1.GetNoise(x + 999, y * 2 + 999);

                            if (_background < 0 && layer >= surfaceLayer)
                            {
                                if (MaterialBlend(x, y, frequency: 4) >= 0f)
                                {
                                    if (x < Main.maxTilesX / 2)
                                    {
                                        tile.WallType = WallID.RocksUnsafe4;
                                    }
                                    else tile.WallType = WallID.RocksUnsafe3;
                                }
                                else tile.WallType = WallID.HallowUnsafe4;
                            }
                            else tile.WallType = 0;
                        }
                    }
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

                        if (!WGTools.Tile(x, y).HasTile)
                        {
                            if (WGTools.Tile(x, y - 1).HasTile)
                            {
                                for (int k = 1; k < WorldGen.genRand.Next(4, 6); k++)
                                {
                                    if (WGTools.Tile(x, y - k).HasTile && WGTools.Tile(x, y - k).TileType != seaPrism && FindBiome(x, y - k) == BiomeID.SunkenSea)
                                    {
                                        WGTools.Tile(x, y - k).TileType = navystone;
                                    }
                                }
                            }
                        }

                        if (!WGTools.Tile(x - 1, y - 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(8) && (WGTools.SolidTileOf((int)x - 2, (int)y - 1, navystone) || WGTools.SolidTileOf((int)x, (int)y - 1, navystone) || WGTools.SolidTileOf((int)x - 1, (int)y - 2, navystone) || WGTools.SolidTileOf((int)x - 1, (int)y, navystone)) || (WGTools.SolidTileOf((int)x - 2, (int)y - 1, seaPrism) || WGTools.SolidTileOf((int)x, (int)y - 1, seaPrism) || WGTools.SolidTileOf((int)x - 1, (int)y - 2, seaPrism) || WGTools.SolidTileOf((int)x - 1, (int)y, seaPrism)))
                            {
                                WorldGen.PlaceTile((int)x - 1, (int)y - 1, prismShard);

                                WGTools.Tile(x - 1, y - 1).TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                                WGTools.Tile(x - 1, y - 1).TileFrameY = (short)((WorldGen.SolidTile((int)x - 1, (int)y) ? 0 : WorldGen.SolidTile((int)x - 1, (int)y - 2) ? 1 : WorldGen.SolidTile((int)x, (int)y - 1) ? 2 : 3) * 18);
                            }
                        }
                        if (!WGTools.Tile(x - 1, y - 1).HasTile && WorldGen.genRand.NextBool(2) && WGTools.SolidTileOf((int)x - 1, (int)y - 2, navystone))
                        {
                            WorldGen.PlaceTile((int)x - 1, (int)y - 1, stalactite);
                            WGTools.Tile(x - 1, y - 1).TileFrameX = (short)(WorldGen.genRand.Next(3) * 18);
                        }
                    }
                    //else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Flesh))
                    //{
                    //    caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                    //    //caves.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Mul);
                    //    caves1.SetFrequency(0.04f);
                    //    caves1.SetFractalType(FastNoiseLite.FractalType.PingPong);
                    //    caves1.SetFractalGain(0.8f);
                    //    caves1.SetFractalWeightedStrength(0.25f);
                    //    caves1.SetFractalPingPongStrength(1.5f);

                    //    if (MaterialBlend(x, y) < -0.7f)
                    //    {
                    //        tile.TileType = (ushort)ModContent.TileType<hardstone>();
                    //    }
                    //    else tile.TileType = (ushort)ModContent.TileType<flesh>();

                    //    float _caves = fleshcaves.GetNoise(x, y + ((float)Math.Cos(x / 40) * 20));
                    //    if (_caves > -0.25f)
                    //    {
                    //        tile.HasTile = false;
                    //        tile.LiquidAmount = 0;
                    //    }
                    //    else tile.HasTile = true;

                    //    if (_caves > 0.15f)
                    //    {
                    //        tile.WallType = 0;
                    //    }
                    //    else
                    //    {
                    //        if (MaterialBlend(x, y) < -0.7f)
                    //        {
                    //            tile.WallType = (ushort)ModContent.WallType<hardstonewall>();
                    //        }
                    //        else tile.WallType = (ushort)ModContent.WallType<fleshwall>();
                    //    }
                    //    //if (background.GetNoise(x, y * 1.5f + ((float)Math.Cos(x / 60) * 20)) > 0.1f)
                    //    //{
                    //    //    if (tile.type == ModContent.TileType<hardstone>())
                    //    //    {
                    //    //        tile.wall = (ushort)ModContent.WallType<hardstonewall>();
                    //    //    }
                    //    //    else tile.wall = (ushort)ModContent.WallType<fleshwall>();
                    //    //}
                    //    //else tile.wall = 0;
                    //}
                    #endregion
                    #region surface
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Tundra))
                    {
                        if (layer >= caveLayer && layer < caveLayer + 2 * (Main.maxTilesY / 1200f) && lunarVeil)
                        {
                            if (lv.TryFind<ModTile>("AbyssalDirt", out ModTile aDirt))
                            {
                                tile.TileType = aDirt.Type;
                            }
                        }
                        else if (tile.TileType == TileID.Silt)
                        {
                            tile.TileType = TileID.Slush;
                        }
                        else if (MaterialBlend(x, y, frequency: 2) >= 0.2f)
                        {
                            tile.TileType = TileID.SnowBlock;
                        }
                        else
                        {
                            tile.TileType = TileID.IceBlock;

                            if (layer < surfaceLayer)
                            {
                                for (int k = 1; k <= WorldGen.genRand.Next(4, 7); k++)
                                {
                                    if (!WGTools.Tile(x, y - k).HasTile)
                                    {
                                        tile.TileType = TileID.SnowBlock;
                                        break;
                                    }
                                }
                            }
                        }

                        //if (layer < surfaceLayer - 1 - (2 * Main.maxTilesY / 1200f * ModContent.GetInstance<Client>().TerrainAmplitude) || layer >= surfaceLayer)
                        //{
                        //    if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Grass || tile.TileType == TileID.ClayBlock || tile.TileType == TileID.Sand)
                        //    {
                        //        tile.TileType = TileID.SnowBlock;
                        //    }
                        //    else if (tile.TileType == TileID.Stone)
                        //    {
                        //        tile.TileType = TileID.IceBlock;
                        //    }
                        //    else if (tile.TileType == TileID.Silt)
                        //    {
                        //        tile.TileType = TileID.Slush;
                        //    }
                        //    if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe)
                        //    {
                        //        tile.WallType = WallID.SnowWallUnsafe;
                        //    }
                        //}

                        if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe)
                        {
                            tile.WallType = WallID.SnowWallUnsafe;
                        }

                        SetDefaultValues(caves1);
                        SetDefaultValues(caves2);
                        SetDefaultValues(caves3);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFractalType(FastNoiseLite.FractalType.PingPong);
                        caves1.SetFractalOctaves(3);
                        caves1.SetFrequency(0.0125f);

                        caves2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves2.SetFrequency(0.025f);
                        caves2.SetFractalType(FastNoiseLite.FractalType.FBm);

                        caves3.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves3.SetFrequency(0.075f);
                        caves3.SetFractalType(FastNoiseLite.FractalType.FBm);

                        if (layer >= surfaceLayer)
                        {
                            caves1.SetFractalPingPongStrength(caves2.GetNoise(x, y * 2 + (int)(Math.Sin(x / 30f + y / (60f * (iceFlip ? -1 : 1))) * 50)) + 2);
                            float _caves = caves1.GetNoise(x, y * 2 + (int)(Math.Sin(x / 30f + y / (60f * (iceFlip ? -1 : 1))) * 50));
                            float _size = (caves3.GetNoise(x, y * 2 + (int)(Math.Sin(x / 30f + y / (60f * (iceFlip ? -1 : 1))) * 50)) / 2) + 1;

                            if (_caves < -_size * 0.1f)
                            {
                                tile.HasTile = false;
                                if (WorldGen.genRand.NextBool(25) && y < GenVars.lavaLine)
                                {
                                    tile.LiquidAmount = 255;
                                }
                            }
                            else
                            {
                                tile.HasTile = true;
                            }


                            if (_caves + 1 > _size * 0.6f)
                            {
                                if (MaterialBlend(x, y, frequency: 2) >= 0.2f)
                                {
                                    tile.WallType = WallID.SnowWallUnsafe;
                                }
                                else tile.WallType = WallID.IceUnsafe;
                            }
                            else if (y > Main.worldSurface)
                            {
                                tile.WallType = 0;
                            }
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Jungle))
                    {
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

                        if (MaterialBlend(x, y, frequency: 2) >= -0.2f)
                        {
                            if (tile.TileType == TileID.Grass || WorldGen.genRand.NextBool(25))
                            {
                                tile.TileType = TileID.JungleGrass;
                            }
                            else tile.TileType = TileID.Mud;
                        }
                        else
                        {
                            tile.TileType = TileID.Stone;

                            if (layer < surfaceLayer)
                            {
                                for (int k = 1; k <= WorldGen.genRand.Next(4, 7); k++)
                                {
                                    if (!WGTools.Tile(x, y - k).HasTile)
                                    {
                                        tile.TileType = TileID.Mud;
                                        break;
                                    }
                                }
                            }
                        }

                        if (layer >= surfaceLayer && !beach)
                        {
                            caves1.SetFractalPingPongStrength(caves2.GetNoise(x, y * 2) + 2);
                            float _caves = caves1.GetNoise(x, y * 2);
                            float _size = (caves3.GetNoise(x, y * 2) / 2) + 1;

                            if (_caves > -_size * 0.1f)
                            {
                                tile.HasTile = false;
                                if (WorldGen.genRand.NextBool(50) && y < GenVars.lavaLine)
                                {
                                    tile.LiquidAmount = 255;
                                }
                            }
                            else
                            {
                                tile.HasTile = true;
                            }

                            if (_caves < _size * 0.4f)
                            {
                                if (MaterialBlend(x, y, frequency: 2) >= -0.2f)
                                {
                                    tile.WallType = WallID.JungleUnsafe;
                                }
                                else tile.WallType = WallID.JungleUnsafe3; //layer >= lavaLayer ? WallID.LavaUnsafe2 :
                            }
                            else if (y > Main.worldSurface)
                            {
                                tile.WallType = 0;
                            }
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
                            float _tunnels = caves1.GetNoise(x, y * 2);
                            //float _nests = nests.GetNoise(x, y + ((float)Math.Cos(x / 60) * 20)) * ((nests2.GetNoise(x, y + ((float)Math.Cos(x / 60) * 20)) + 1) / 2);
                            float _fossils = ((fossils.GetNoise(x, y * 2) + 1) / 2) * 0.4f;

                            float _background = ((background.GetNoise(x, y * 2) + 1) / 2) * 0.3f;


                            float _size = (caves2.GetNoise(x, y * 2 + ((float)Math.Cos(x / 60) * 20)) + 1) / 2 / 4;
                            float _offset = caves3.GetNoise(x, y * 2 + ((float)Math.Cos(x / 60) * 20));

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
                                //if (gems.GetNoise(x * 2, y * 2) > 0.4f)
                                //{
                                //    tile.TileType = (ushort)ModContent.TileType<sandstoneamber>();
                                //}
                                //else
                                //{

                                //}
                                tile.TileType = TileID.Sandstone;
                                tile.WallType = WallID.Sandstone;

                                if (_tunnels > _offset - _size && _tunnels < _offset + _size && !beach)
                                {
                                    tile.HasTile = false;
                                    tile.LiquidAmount = 0;

                                    if (_tunnels - 0.05f - _background > _offset - _size && _tunnels + 0.05f + _background < _offset + _size)
                                    {
                                        tile.WallType = WallID.HardenedSand;
                                    }
                                }
                            }
                            else
                            {
                                tile.TileType = TileID.HardenedSand;
                                tile.WallType = WallID.HardenedSand;
                            }
                        }
                        else
                        {
                            if (MaterialBlend(x, y, frequency: 2) <= 0)
                            {
                                WGTools.Tile(x, y).TileType = TileID.HardenedSand;
                            }
                            else tile.TileType = TileID.Sand;

                            if (layer == surfaceLayer - 1)
                            {
                                tile.HasTile = true;
                                tile.TileType = TileID.HardenedSand;
                                tile.WallType = WallID.HardenedSand;
                            }
                            else tile.LiquidAmount = 0;
                            if (tile.HasTile)// && y > Terrain.Middle)
                            {
                                int var = 1;
                                //if (y / biomes.scale >= (int)Main.worldSurface / biomes.scale - 1 || dunes.GetNoise(x, y + 2) > 0)
                                //{
                                //    var = 2;
                                //}
                                for (int k = -var; k <= var; k++)
                                {
                                    if (!WGTools.Tile(x + k, y + 1).HasTile)
                                    {
                                        WGTools.Tile(x + k, y + 1).TileType = TileID.Sand;
                                        WGTools.Tile(x + k, y + 1).HasTile = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Corruption))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        caves1.SetFrequency(0.015f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.Ridged);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
                        caves1.SetFractalOctaves(3);
                        caves1.SetFractalGain(0.75f);
                        caves1.SetFractalWeightedStrength(0.5f);

                        if (!Main.wallDungeon[tile.WallType])
                        {
                            float _size = 1;// (caves2.GetNoise(x * 2, y) + 1) / 2;
                            float _caves = caves1.GetNoise(x, y) / 2;
                            //float _offset = caves3.GetNoise(x, y);

                            float thing = (_caves) / (1 + 1 / 2);

                            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(Corruption.orbX, !WorldGen.crimson ? Corruption.orbYPrimary : Corruption.orbYSecondary));
                            thing += MathHelper.Clamp((1 - dist / (Main.maxTilesX / 4200f * 48)) * 2, 0, 1);

                            bool ug = layer >= surfaceLayer - 1;
                            if (ug || tile.WallType != 0)
                            {
                                tile.HasTile = true;
                            }

                            if (thing > 0.25f - _size / (underground ? 2.5f : 3))//thing > _offset - _size / (ug ? 2 : 3) && thing < _offset + _size / (ug ? 2 : 3) && !sky)
                            {
                                tile.TileType = TileID.Ebonstone;
                                if (ug || tile.WallType != 0)
                                {
                                    tile.WallType = WallID.EbonstoneUnsafe;
                                }
                                if (thing > 0) //thing > _offset - _size / 5 && thing < _offset + _size / 5)
                                {
                                    tile.HasTile = false;

                                    if (ug && thing > 0.2f)
                                    {
                                        tile.WallType = WallID.CorruptionUnsafe2;
                                    }
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

                            if (!ug && WGTools.Tile(x - 1, y - 1).TileType == TileID.Dirt && !WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.CorruptGrass;
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

                            float thing = (_caves) / (1 + 1 / 2);

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

                            if (!underground && WGTools.Tile(x - 1, y - 1).TileType == TileID.Dirt && !WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.CrimsonGrass;
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
                        caves1.SetFrequency(0.015f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.7f);
                        caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                        caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);

                        if (MaterialBlend(x + WorldGen.genRand.Next(-1, 2), y + WorldGen.genRand.Next(-1, 2), true, 2) <= -0.3f)
                        {
                            WGTools.Tile(x, y).TileType = TileID.MushroomBlock;
                        }
                        else
                        {
                            if (tile.TileType == TileID.Grass || WorldGen.genRand.NextBool(25) || FindBiome(x + 1, y + 1) != BiomeID.Glowshroom && !WGTools.Solid(x + 1, y + 1))
                            {
                                tile.TileType = TileID.MushroomGrass;
                            }
                            else tile.TileType = TileID.Mud;
                        }

                        float _caves = caves1.GetNoise(x, y * 2); //+((float)Math.Cos(x / 60) * 20)

                        //shroomcaves.SetFractalPingPongStrength(shroomcaves2.GetNoise(x, y * 2) + 1.625f);
                        //float _caves = shroomcaves.GetNoise(x, y * 2);
                        //float _size = (shroomcaves3.GetNoise(x, y * 2) / 2) + 1;

                        if (_caves < -0.275f)
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

                        if (!WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Mud || WGTools.Tile(x - 1, y - 1).TileType == TileID.MushroomBlock)
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.MushroomGrass;
                            }
                        }

                        if (caves1.GetNoise(x + 999, y * 2 + 999) > -0.2f)
                        {
                            tile.WallType = WallID.MushroomUnsafe;
                        }
                        else tile.WallType = 0;
                    }
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.GemCave))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                        caves1.SetFrequency(0.03f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);

                        float _caves = caves1.GetNoise(x, y * 2);

                        int gemType = RemTile.GetGemType(j);

                        ushort gemBlock = gemType == 5 ? TileID.Diamond : gemType == 4 ? TileID.Ruby : gemType == 3 ? TileID.Emerald : gemType == 2 ? TileID.Sapphire : gemType == 1 ? TileID.Topaz : TileID.Amethyst;
                        ushort gemWall = gemType == 5 ? WallID.DiamondUnsafe : gemType == 4 ? WallID.RubyUnsafe : gemType == 3 ? WallID.EmeraldUnsafe : gemType == 2 ? WallID.SapphireUnsafe : gemType == 1 ? WallID.TopazUnsafe : WallID.AmethystUnsafe;

                        if (_caves < -0.1f || _caves > 0.1f)
                        {
                            tile.HasTile = false;
                            tile.LiquidAmount = 0;
                        }
                        else
                        {
                            tile.HasTile = true;
                            tile.Slope = 0;
                        }

                        WGTools.Tile(x, y).TileType = TileID.Stone;

                        if (!WGTools.SurroundingTilesActive(x - 1, y - 1))
                        {
                            if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Stone && WGTools.Tile(x - 1, y - 2).TileType != TileID.GemSaplings && WorldGen.genRand.NextBool(5))
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = gemBlock;
                            }
                        }

                        if (_caves > -0.3f && _caves < 0.3f)
                        {
                            tile.WallType = gemWall;
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

                        if (lunarVeil && lv.TryFind<ModTile>("CindersparkDirt", out ModTile csDirt))
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
                    else if (UpdatingBiome(x, y, biomesToUpdate, BiomeID.Toxic))
                    {
                        SetDefaultValues(caves1);

                        caves1.SetNoiseType(FastNoiseLite.NoiseType.Value);
                        caves1.SetFrequency(0.02f);
                        caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                        caves1.SetFractalGain(0.6f);

                        if (MaterialBlend(x, y, true) <= -0.25f)
                        {
                            WGTools.Tile(x, y).TileType = (ushort)ModContent.TileType<ToxicWaste>();
                        }
                        else WGTools.Tile(x, y).TileType = TileID.Stone;

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

                        if (!WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Stone)
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.KryptonMoss;
                            }
                            else if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Mud)
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.JungleGrass;
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
                            if (layer >= height - 1)
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
                            WGTools.Tile(x, y).TileType = TileID.ShimmerBlock;
                        }
                        else WGTools.Tile(x, y).TileType = TileID.Stone;

                        if (!WGTools.SurroundingTilesActive(x - 1, y - 1))
                        {
                            if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Stone)
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.VioletMoss;
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
                    #region unused
                    //else if (toxic && FindBiome(x, y) == "toxic")
                    //{
                    //    if (MaterialBlend(x, y, true) <= -0.2f)
                    //    {
                    //        tile.TileType = (ushort)ModContent.TileType<poisonrock>();
                    //    }
                    //    else if (tile.TileType != TileID.Silt)
                    //    {
                    //        tile.TileType = TileID.Stone;
                    //    }

                    //    tile.WallType = WallID.JungleUnsafe3;

                    //    if (!WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                    //    {
                    //        if (WGTools.GetTile(x - 1, y - 1).TileType == TileID.Stone)
                    //        {
                    //            WGTools.GetTile(x - 1, y - 1).TileType = TileID.KryptonMoss;
                    //        }
                    //    }
                    //}
                    //else if (biomes.Contains("granite") && FindBiome(x, y) == "granite")
                    //{
                    //    caves1.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                    //    caves1.SetFrequency(0.03f);
                    //    caves1.SetFractalType(FastNoiseLite.FractalType.FBm);
                    //    caves1.SetFractalGain(0.6f);
                    //    caves1.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);
                    //    caves1.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

                    //    WGTools.GetTile(x, y).TileType = TileID.Granite;

                    //    //float _caves = granitecaves.GetNoise(x, y * 1.5f) + ((float)Math.Cos(y / 2 + ((float)Math.Cos(x / 50) * 8) + granitecaves2.GetNoise(x, y * 5) * 4) / 8);

                    //    float _caves = caves1.GetNoise(x, y * 2);

                    //    if (_caves > 0)
                    //    {
                    //        WGTools.GetTile(x, y).HasTile = false;
                    //    }
                    //    else
                    //    {
                    //        WGTools.GetTile(x, y).HasTile = true;
                    //        WGTools.GetTile(x, y).Slope = SlopeType.Solid;
                    //    }

                    //    WGTools.GetTile(x, y).WallType = WallID.GraniteUnsafe;
                    //}
                    //else if (biomes.Contains("slimy") && FindBiomeEfficient(x, y, "slimy"))
                    //{
                    //    if ((WGTools.GetTile(x, y).type == TileID.Dirt || WGTools.GetTile(x, y).type == TileID.Stone || WGTools.GetTile(x, y).type == TileID.Silt) && slime.GetNoise(x, y) > -0.5f)
                    //    {
                    //        WGTools.GetTile(x, y).type = TileID.SlimeBlock;
                    //        if (WGTools.GetTile(x, y).wall == 0)
                    //        {
                    //            WGTools.GetTile(x, y).wall = WallID.Slime;
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion

                    if (y >= Main.maxTilesY - 20)
                    {
                        break;
                    }
                    //if (biomes.Contains(FindBiome(x, y, false)) || biomes.Contains(FindBiomeFast(x + scale / 2, y)) || biomes.Contains(FindBiomeFast(x - scale / 2, y)) || biomes.Contains(FindBiomeFast(x, y + scale / 2)) || biomes.Contains(FindBiomeFast(x, y - scale / 2)))

                    if (biomesToUpdate[BiomeID.Beach] && beach)
                    {
                        tile.WallType = 0;
                        if (layer > surfaceLayer - 1)
                        {
                            tile.HasTile = true;
                        }
                        if (layer >= surfaceLayer)
                        {
                            if ((i / scale) == 0 || (i / scale) == width - 1)
                            {
                                tile.TileType = (ushort)ModContent.TileType<Hardstone>();
                            }
                            else if (tile.TileType == TileID.ArgonMoss)
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
                            if (MaterialBlend(x, y, frequency: 2) <= 0)
                            {
                                WGTools.Tile(x, y).TileType = TileID.HardenedSand;
                            }
                            else tile.TileType = TileID.Sand;

                            if (WGTools.Tile(x, y).WallType == WallID.DirtUnsafe || WGTools.SurroundingTilesActive(x, y))
                            {
                                tile.WallType = WallID.HardenedSand;
                            }
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
                return materials[(int)x, flip ? Main.maxTilesY - (int)y : (int)y];
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

        public const int Hive = 11;

        public const int GemCave = 12;

        public const int Beach = 13;

        public const int OceanCave = 14;

        public const int Aether = 15;

        public const int AshForest = 16;

        public const int Obsidian = 17;

        public const int Toxic = 18;



        public const int SunkenSea = 100;

        public const int Abysm = 101;
    }

    public class PrimaryBiomes : GenPass
    {
        public PrimaryBiomes(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        internal class Tundra
        {
            public static int X;
            public static int Y;
            public static float Size;
            public static float HeightMultiplier;
        }

        internal class Jungle
        {
            public static int Center;
            public static int Size;
        }

        internal class Desert
        {
            public static int Center;
            public static int Bottom;
            public static int Size;
        }

        internal class Corruption
        {
            public static int X;
            public static int Y;
            public static int Size;
            public static float heightMultiplier;

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

                bool crimson = (WorldGen.crimson ^ alternate);

                for (int j = (int)(orbY - radius * 1.5f); j <= orbY + radius * 1.5f; j++)
                {
                    for (int i = (int)(Corruption.orbX - radius * 1.5f); i <= Corruption.orbX + radius * 1.5f; i++)
                    {
                        float distance = Vector2.Distance(new Vector2(i, j), new Vector2(Corruption.orbX, orbY)) + noise.GetNoise(i, j) * 10;

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
                            if (WGTools.Tile(i, j).HasTile)
                            {
                                valid = false;
                            }
                        }
                    }
                    for (int j = y - 3; j <= y + 4; j++)
                    {
                        for (int i = x - 3; i <= x + 4; i++)
                        {
                            if (WGTools.Tile(i, j).HasTile && WGTools.Tile(i, j).TileType == TileID.ShadowOrbs)
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

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();
            FastNoiseLite noise;

            bool calamity = ModLoader.TryGetMod("CalamityMod", out Mod cal);
            bool lunarVeil = ModLoader.TryGetMod("Stellamod", out Mod lv);

            progress.Message = "Adding major biomes";

            #region tundra
            Tundra.Y = (int)Main.worldSurface / biomes.scale;
            Tundra.Size = biomes.width / 12.5f;
            Tundra.HeightMultiplier = (int)((RemWorld.lavaLevel) - Main.worldSurface) / biomes.scale / Tundra.Size;

            noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.2f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            //for (int y = 1; y < lavaLayer - 100; y++)
            //{
            //    for (int x = tundraX - (int)tundraSize; x < tundraX + tundraSize; x++)
            //    {

            FastNoiseLite thinice = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            thinice.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            thinice.SetFrequency(0.2f);
            thinice.SetFractalType(FastNoiseLite.FractalType.FBm);

            //for (int j = biomes.skyLayer; j < biomes.undergroundLayer - (Main.maxTilesY / 600f); j++)
            //{
            //    for (int i = 4; i < biomes.width - 4; i++)
            //    {
            //        if (Tundra.X < biomes.width / 2 && i < biomes.width / 2 || Tundra.X > biomes.width / 2 && i > biomes.width / 2)
            //        {
            //            biomes.AddBiome(i, j, BiomeID.Tundra);
            //        }
            //    }
            //}

            for (int j = 0; j < Math.Min(GenVars.lavaLine / biomes.scale - 1, biomes.height - 4); j++)
            {
                for (int i = 0; i < biomes.width; i++)
                {
                    if (noise.GetNoise(i, j) <= (1 - (Vector2.Distance(new Vector2(Tundra.X, MathHelper.Clamp(j, 0, Tundra.Y)), new Vector2(i, (j - Tundra.Y) / Tundra.HeightMultiplier + Tundra.Y)) / (Tundra.Size * (j < biomes.surfaceLayer ? 0.75f : 1)))) * 2)
                    {
                        biomes.AddBiome(i, j, BiomeID.Tundra);
                        //biomes.AddBiome(i, j, i == Tundra.X && j > biomes.undergroundLayer ? BiomeID.IceChasm : BiomeID.Tundra);
                    }
                }
            }
            #endregion

            #region jungle
            Jungle.Size = biomes.width / 10;
            Desert.Size = biomes.width / 20;
            Desert.Bottom = biomes.lavaLayer + 1;

            noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.2f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            for (int y = 1; y < biomes.height; y++)
            {
                for (int x = 6; x < biomes.width - 6; x++)
                {
                    int i = x + (int)(noise.GetNoise(x, y + 999) * (Main.maxTilesX / 1050f));
                    int j = y + (int)(noise.GetNoise(x + 999, y) * (Main.maxTilesY / 600f));

                    Vector2 point = new Vector2(Jungle.Center, y);
                    float _size = Jungle.Size;
                    if (biomes.biomeMap[x, y] != BiomeID.Obsidian && (biomes.biomeMap[x, y] != BiomeID.Beach || y >= biomes.surfaceLayer && y < biomes.caveLayer))
                    {
                        if (i >= Desert.Center - Desert.Size && i <= Desert.Center + Desert.Size && j < Desert.Bottom)
                        {
                            if (ModContent.GetInstance<Client>().SunkenSeaRework && calamity && j >= (biomes.lavaLayer - biomes.caveLayer) / 2 + biomes.caveLayer)
                            {
                                biomes.AddBiome(x, y, BiomeID.SunkenSea);
                            }
                            else biomes.AddBiome(x, y, BiomeID.Desert);
                        }
                        else if (i >= Jungle.Center - Jungle.Size && i <= Jungle.Center + Jungle.Size)
                        {
                            if (y < biomes.height - 4)
                            {
                                if (ModContent.GetInstance<Client>().ExperimentalWorldgen && j > biomes.lavaLayer)
                                {
                                    biomes.AddBiome(x, y, BiomeID.Toxic);
                                }
                                else biomes.AddBiome(x, y, BiomeID.Jungle);
                            }
                            else biomes.AddBiome(x, y, BiomeID.AshForest);
                        }
                    }
                }
            }

            GenVars.UndergroundDesertLocation = new Rectangle((int)((Desert.Center - Desert.Size) * biomes.scale), (biomes.surfaceLayer - 1) * biomes.scale, (int)((Desert.Size * 2 + 1) * biomes.scale), (int)((Desert.Bottom - (biomes.surfaceLayer - 1)) * biomes.scale));
            GenVars.structures.AddStructure(GenVars.UndergroundDesertLocation);
            #endregion

            #region corruption
            Corruption.Y = (int)Main.worldSurface / biomes.scale;

            Corruption.orbX = (int)((Corruption.X + 0.5f) * biomes.scale);

            Corruption.Size = biomes.width / 42;

            noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.2f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            for (int j = biomes.skyLayer; j < biomes.height - 6; j++)
            {
                for (int i = 0; i < biomes.width; i++)
                {
                    Vector2 point = new Vector2(Corruption.X, MathHelper.Clamp(j, 1, biomes.caveLayer));
                    if (noise.GetNoise(i, j) <= (1 - (Vector2.Distance(point, new Vector2(i, j)) / Corruption.Size)) * 2)
                    {
                        if (WorldGen.crimson)
                        {
                            biomes.AddBiome(i, j, BiomeID.Crimson);
                        }
                        else biomes.AddBiome(i, j, BiomeID.Corruption);
                    }

                    point = new Vector2(Corruption.X, MathHelper.Clamp(j, (Main.maxTilesY - 300 - (int)(Main.rockLayer - Main.worldSurface)) / biomes.scale - 1, biomes.height - 6));
                    if (noise.GetNoise(i, j) <= (1 - (Vector2.Distance(point, new Vector2(i, j)) / Corruption.Size)) * 2)
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

            biomes.UpdateMap(new int[] { BiomeID.Tundra, BiomeID.Jungle, BiomeID.Desert, BiomeID.Corruption, BiomeID.Crimson, BiomeID.Underworld, BiomeID.AshForest, BiomeID.Obsidian, BiomeID.Beach, BiomeID.Toxic, BiomeID.SunkenSea, BiomeID.Abysm }, progress);

            progress.Message = "Incubating infection";

            #region corruption
            for (int k = 0; k < 10; k++)
            {
                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = (Corruption.X - Corruption.Size - 1) * biomes.scale; x <= (Corruption.X + Corruption.Size + 2) * biomes.scale; x++)
                    {
                        if (!WGTools.SurroundingTilesActive(x, y) && (WGTools.Tile(x, y).WallType == WallID.EbonstoneUnsafe || WGTools.Tile(x, y).WallType == WallID.CrimstoneUnsafe))
                        {
                            int adjacentWalls = 0;

                            if (WGTools.Tile(x + 1, y).WallType != 0 || WGTools.Tile(x + 1, y).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (WGTools.Tile(x, y + 1).WallType != 0 || WGTools.Tile(x, y + 1).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (WGTools.Tile(x - 1, y).WallType != 0 || WGTools.Tile(x - 1, y).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (WGTools.Tile(x, y - 1).WallType != 0 || WGTools.Tile(x, y - 1).HasTile)
                            {
                                adjacentWalls++;
                            }

                            if (k == 9)
                            {
                                if (adjacentWalls < 3)
                                {
                                    WGTools.Tile(x, y).WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                                }
                            }
                            else if (adjacentWalls < 4 && WorldGen.genRand.NextBool(4 / (4 - adjacentWalls)))
                            {
                                WGTools.Tile(x, y).WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                            }
                        }
                    }
                }

                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = (Corruption.X - Corruption.Size - 1) * biomes.scale; x <= (Corruption.X + Corruption.Size + 2) * biomes.scale; x++)
                    {
                        if (WGTools.Tile(x, y).WallType == (ushort)ModContent.WallType<Walls.dev.nothing>())
                        {
                            WGTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }

            Corruption.CreateOrb(false);
            Corruption.CreateOrb(true);

            int structureCount = Main.maxTilesX / 210;
            while (structureCount > 0)
            {
                #region spawnconditions
                int x = WorldGen.genRand.Next((int)((Corruption.X - Corruption.Size) * biomes.scale), (int)((Corruption.X + Corruption.Size) * biomes.scale));
                int y = structureCount > Main.maxTilesX / 420 ? WorldGen.genRand.Next((int)Main.worldSurface, (int)(Main.maxTilesY - 300 - Main.worldSurface) / 2 + (int)Main.worldSurface) : WorldGen.genRand.Next((int)(Main.maxTilesY - 300 - Main.worldSurface) / 2 + (int)Main.worldSurface, Main.maxTilesY - 300);

                bool valid = true;

                if (WGTools.Tile(x, y).TileType == TileID.DemonAltar || Main.wallDungeon[WGTools.Tile(x, y).WallType])
                {
                    valid = false;
                }
                else if (biomes.FindBiome(x, y) != BiomeID.Corruption && biomes.FindBiome(x, y) != BiomeID.Crimson)
                {
                    valid = false;
                }
                else if (WGTools.Tile(x, y + 1).TileType != TileID.Ebonstone && WGTools.Tile(x, y + 1).TileType != TileID.Crimstone)
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    WorldGen.PlaceObject(x, y, TileID.DemonAltar, style: biomes.FindBiome(x, y) == BiomeID.Crimson ? 1 : 0);

                    if (WGTools.Tile(x, y).TileType == TileID.DemonAltar)
                    {
                        structureCount--;
                    }
                }
            }
            #endregion

            progress.Message = "Cleaning up ground";

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
                                    if (!WGTools.Tile(x, y - i).HasTile)
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
                                    if (!WGTools.Tile(x, y + i).HasTile)
                                    {
                                        tile.TileType = TileID.HardenedSand;
                                        break;
                                    }
                                }
                            }

                            if (WGTools.Tile(x, y).WallType == WallID.DirtUnsafe || WGTools.SurroundingTilesActive(x, y))
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
                    for (int x = (Desert.Center - Desert.Size - 1) * biomes.scale; x <= (Desert.Center + Desert.Size + 2) * biomes.scale; x++)
                    {
                        if (!WGTools.SurroundingTilesActive(x, y) && WGTools.Tile(x, y).WallType == WallID.HardenedSand)
                        {
                            int adjacentWalls = 0;

                            if (WGTools.Tile(x + 1, y).WallType != 0 || WGTools.Tile(x + 1, y).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (WGTools.Tile(x, y + 1).WallType != 0 || WGTools.Tile(x, y + 1).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (WGTools.Tile(x - 1, y).WallType != 0 || WGTools.Tile(x - 1, y).HasTile)
                            {
                                adjacentWalls++;
                            }
                            if (WGTools.Tile(x, y - 1).WallType != 0 || WGTools.Tile(x, y - 1).HasTile)
                            {
                                adjacentWalls++;
                            }

                            if (k == 9)
                            {
                                if (adjacentWalls < 3)
                                {
                                    WGTools.Tile(x, y).WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                                }
                            }
                            else if (adjacentWalls < 4 && WorldGen.genRand.NextBool(4 / (4 - adjacentWalls)))
                            {
                                WGTools.Tile(x, y).WallType = (ushort)ModContent.WallType<Walls.dev.nothing>();
                            }
                        }
                    }
                }

                for (int y = 40; y < Main.worldSurface; y++)
                {
                    for (int x = (Desert.Center - Desert.Size - 1) * biomes.scale; x <= (Desert.Center + Desert.Size + 2) * biomes.scale; x++)
                    {
                        if (WGTools.Tile(x, y).WallType == (ushort)ModContent.WallType<Walls.dev.nothing>())
                        {
                            WGTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }

            #region objects
            int area = GenVars.UndergroundDesertLocation.Width * GenVars.UndergroundDesertLocation.Height;

            int objects = area / 800;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right + 1);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.UndergroundDesertLocation.Bottom + 1);

                if (biomes.FindBiome(x, y) == BiomeID.Desert && WGTools.Tile(x, y + 1).HasTile && !WGTools.Tile(x + 1, y).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.Sandstone)
                {
                    WorldGen.PlaceObject(x, y, TileID.AntlionLarva, style: Main.rand.Next(3));
                    if (Framing.GetTileSafely(x, y).TileType == TileID.AntlionLarva)
                    {
                        objects--;
                    }
                }
            }

            if (ModContent.GetInstance<Client>().SunkenSeaRework && calamity)
            {
                area = GenVars.UndergroundDesertLocation.Width * (int)((GenVars.lavaLine - Main.rockLayer) / 2);

                ushort eutrophicSand = 0;
                ushort seaPrism = 0;
                if (cal.TryFind<ModTile>("EutrophicSand", out ModTile sand))
                {
                    eutrophicSand = sand.Type;
                }
                if (cal.TryFind<ModTile>("SeaPrism", out ModTile prism))
                {
                    seaPrism = prism.Type;
                }

                if (cal.TryFind<ModTile>("FanCoral", out ModTile fanCoral) && cal.TryFind<ModTile>("BrainCoral", out ModTile brainCoralLarge) && cal.TryFind<ModTile>("CoralPileLarge", out ModTile coralPileLarge) && cal.TryFind<ModTile>("SmallBrainCoral", out ModTile brainCoralSmall) && cal.TryFind<ModTile>("MediumCoral", out ModTile mediumCoral) && cal.TryFind<ModTile>("MediumCoral2", out ModTile mediumCoral2) && cal.TryFind<ModTile>("SmallTubeCoral", out ModTile smallTubeCoral) && cal.TryFind<ModTile>("SeaAnemone", out ModTile seaAnemone) && cal.TryFind<ModTile>("SmallCorals", out ModTile smallCoral) && cal.TryFind<ModTile>("TableCoral", out ModTile tableCoral))
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

                            if (WGTools.SolidTileOf(x - 2, y, seaPrism) || WGTools.SolidTileOf(x + 2, y, seaPrism) || WorldGen.SolidTile(x, y - 2) || WorldGen.SolidTile(x, y - 3))
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
                                        if (!WGTools.SolidTileOf(i, j, eutrophicSand))
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
                                        if (!WGTools.SolidTileOf(i, j, eutrophicSand))
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
                                        if (!WGTools.SolidTileOf(i, j, eutrophicSand))
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
                            else if (!WGTools.SolidTileOf(x, y + 1, eutrophicSand))
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

            #region underworld
            progress.Message = "Creating the underworld";

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
                            threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / (50 + distance.GetNoise(x, y) * 50))), 0, 1);
                        }
                        else threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / (50 + distance.GetNoise(x, y) * 50))), 0, 1);

                        float _terrain = terrain.GetNoise(x, y * 2);
                        if (_terrain < threshold - 0.25f && WGTools.Tile(x, y).TileType != ModContent.TileType<Hardstone>())
                        {
                            WGTools.Tile(x, y).HasTile = false;
                            if (WGTools.Tile(x, y).WallType == WallID.ObsidianBackUnsafe)
                            {
                                WGTools.Tile(x, y).WallType = 0;
                            }
                            if (y <= point.Y - 40 && y >= point.Y - 50 || y > point.Y + 40 + distance.GetNoise(x, y) * 20)
                            {
                                WGTools.Tile(x, y).LiquidAmount = 255;
                            }
                            //float _background = background.GetNoise(x, y);
                        }

                        if (_terrain / 2 > threshold - 0.5f && WGTools.Tile(x, y).WallType == 0)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.AshForest)
                            {
                                WGTools.Tile(x, y).WallType = WallID.LavaUnsafe4;
                            }
                            else WGTools.Tile(x, y).WallType = WallID.LavaUnsafe3;
                        }

                        if (biomes.FindBiome(x, y) == BiomeID.AshForest && !WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Ash)
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.AshGrass;
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

                    WGTools.Tile(x, y).LiquidType = 1;
                }
            }
            #endregion
        }

        public bool TundraLeft()
        {
            int tundraLeft = 0;
            int tundraRight = 0;

            for (int x = 1; x < Main.maxTilesX; x++)
            {
                if (Framing.GetTileSafely(x, (int)Main.worldSurface).TileType == TileID.SnowBlock || Framing.GetTileSafely(x, (int)Main.worldSurface).TileType == TileID.IceBlock)
                {
                    tundraLeft = x;
                    break;
                }
            }
            for (int x = Main.maxTilesX; x > 1; x--)
            {
                if (Framing.GetTileSafely(x, (int)Main.worldSurface).TileType == TileID.SnowBlock || Framing.GetTileSafely(x, (int)Main.worldSurface).TileType == TileID.IceBlock)
                {
                    tundraRight = x;
                    break;
                }
            }

            Tundra.X = (tundraLeft + tundraRight) / 2;

            return Tundra.X < Main.maxTilesX * 0.5 ? true : false;
        }
    }

    public class SecondaryBiomes : GenPass
    {
        public SecondaryBiomes(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        internal class Hive
        {
            public static int X;
            public static int Y;
            public static float Size;
        }

        internal class MarbleCave
        {
            public static int Y;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Adding minibiomes";

            #region glowshroom
            FastNoiseLite glowshroom = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            glowshroom.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            glowshroom.SetFractalType(FastNoiseLite.FractalType.None);
            glowshroom.SetFrequency(0.1f);

            FastNoiseLite gemcaves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            gemcaves.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            gemcaves.SetFractalType(FastNoiseLite.FractalType.FBm);
            gemcaves.SetFrequency(0.15f);
            gemcaves.SetFractalOctaves(1);

            Main.tileSolid[TileID.MushroomBlock] = true;
            #endregion

            #region marblecave
            int marbleCaveLeft = (int)(biomes.width * 0.4f);
            int marbleCaveRight = (int)(biomes.width * 0.6f);
            MarbleCave.Y = Math.Min(biomes.lavaLayer, biomes.height - 8 - (Main.maxTilesY / 600));
            #endregion

            #region hive
            Main.tileSolid[TileID.Hive] = true;
            Main.tileSolid[TileID.BeeHive] = false;

            //if (Jungle.Center > Desert.X)
            //{
            //    //X = WorldGen.genRand.Next((int)(biomes.width * 0.85), (int)(biomes.width * 0.9));
            //    Hive.X = GenVars.UndergroundDesertLocation.Right / biomes.scale + (Jungle.Center);
            //}
            //else
            //{
            //    //X = WorldGen.genRand.Next((int)(biomes.width * 0.1), (int)(biomes.width * 0.15));
            //    Hive.X = GenVars.UndergroundDesertLocation.Left / biomes.scale + (Jungle.Center);
            //}
            //Hive.X /= 2;

            Hive.X = Jungle.Center;
            if (Jungle.Center > Desert.Center)
            {
                Hive.X -= (int)(Jungle.Size * 0.75f);
            }
            else Hive.X += (int)(Jungle.Size * 0.75f);

            Hive.Size = biomes.width / 32;
            Hive.Y = (int)(Main.rockLayer / biomes.scale + Hive.Size);

            FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.2f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            for (int j = 1; j < biomes.height - 4; j++)
            {
                for (int i = 0; i < biomes.width; i++)
                {
                    if (noise.GetNoise(i, j) <= (1 - (Vector2.Distance(new Vector2(Hive.X, Hive.Y), new Vector2(i, j)) / Hive.Size)) * 2)
                    {
                        biomes.AddBiome(i, j, BiomeID.Hive);
                    }
                }
            }
            #endregion

            #region aether
            Main.tileSolid[TileID.ShimmerBlock] = true;

            GenVars.shimmerPosition.X = GenVars.dungeonSide != 1 ? Main.maxTilesX - 175 : 175;
            GenVars.shimmerPosition.Y = Main.rockLayer + 200;
            #endregion

            FastNoiseLite growth = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            growth.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            growth.SetFrequency(0.005f);
            growth.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite flesh = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            flesh.SetNoiseType(FastNoiseLite.NoiseType.Value);
            flesh.SetFrequency(0.25f);

            //FastNoiseLite thermalcaves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            //thermalcaves.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //thermalcaves.SetFrequency(0.3f);

            //for (int i = 0; i < biomes.width; i++)
            //{
            //    for (int j = biomes.height - 2; j < biomes.height; j++)
            //    {
            //        biomes.AddBiome(i, j, "sulfursprings");
            //    }
            //}

            FastNoiseLite meadows = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            meadows.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            meadows.SetFrequency(0.5f);
            meadows.SetFractalType(FastNoiseLite.FractalType.FBm);
            meadows.SetFractalOctaves(3);

            bool thorium = ModLoader.TryGetMod("ThoriumMod", out Mod mod) || ModLoader.TryGetMod("Aequus", out Mod mod2);

            for (int i = 1; i < biomes.width - 1; i++)
            {
                for (int j = 0; j < biomes.height - 5; j++)
                {
                    if (j >= biomes.surfaceLayer)
                    {
                        if (i <= 5 && i > 0 || i >= biomes.width - 6 && i < biomes.width - 1)
                        {
                            bool jungleSide = (GenVars.dungeonSide == 1 && i <= 5 || GenVars.dungeonSide != 1 && i >= biomes.width - 6);
                            bool thoriumCompat = jungleSide && thorium;

                            if (!thoriumCompat && j < biomes.caveLayer - 1 && j > biomes.surfaceLayer)
                            {
                                biomes.AddBiome(i, j, BiomeID.OceanCave);
                            }
                            else if (jungleSide && j > biomes.caveLayer + (thorium ? 2 : 0) && j < biomes.height - 7)
                            {
                                biomes.AddBiome(i, j, BiomeID.Aether);
                            }
                        }
                        else if (i > 6 && i < biomes.width - 7)
                        {
                            if (i >= Tundra.X - 1 && i <= Tundra.X + 1)
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
                            else if (biomes.biomeMap[i, j] == BiomeID.None)
                            {
                                if (j >= biomes.caveLayer && glowshroom.GetNoise(i, j * 2) < -0.95f && j < biomes.lavaLayer)
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
                        biomes.AddBiome(biomes.width - 2, biomes.surfaceLayer, BiomeID.OceanCave); biomes.AddBiome(biomes.width - 2, biomes.surfaceLayer - 1, BiomeID.OceanCave);
                    }
                }
            }

            int count = 0;
            while (count < (biomes.width * (biomes.height - 6 - biomes.caveLayer)) / 40)
            {
                int x = WorldGen.genRand.Next(7, biomes.width - 7);
                int y = WorldGen.genRand.Next(biomes.caveLayer, biomes.height - 6);

                if (biomes.biomeMap[x, y] == BiomeID.None)
                {
                    if (biomes.biomeMap[x - 1, y] != BiomeID.GemCave && biomes.biomeMap[x + 1, y] != BiomeID.GemCave && biomes.biomeMap[x, y - 1] != BiomeID.GemCave && biomes.biomeMap[x, y + 1] != BiomeID.GemCave && biomes.biomeMap[x - 1, y - 1] != BiomeID.GemCave && biomes.biomeMap[x + 1, y - 1] != BiomeID.GemCave && biomes.biomeMap[x - 1, y + 1] != BiomeID.GemCave && biomes.biomeMap[x + 1, y + 1] != BiomeID.GemCave)
                    {
                        biomes.AddBiome(x, y, BiomeID.GemCave);

                        count++;
                    }
                }
            }

            #region growth
                //int growthSize = biomes.width / 16;
                //int growthY = (int)(RemWorld.lavaLevel / biomes.scale) - growthSize / 2;
                //int growthX = Tundra.X;

                //for (int j = (int)Main.worldSurface / biomes.scale + 2; j < WorldGen.lavaLine / biomes.scale; j++)
                //{
                //    for (int i = 0; i < biomes.width; i++)
                //    {
                //        float num = Vector2.Distance(new Vector2(growthX, growthY), new Vector2(i + (j - growthY) / 2, j)) / growthSize;

                //        if ((growth.GetNoise(i * biomes.scale, j * biomes.scale) + 1) / 2 >= num)
                //        {
                //            biomes.AddBiome(i, j, "growth");
                //        }
                //    }
                //}
                #endregion

            biomes.UpdateMap(new int[] { BiomeID.Glowshroom, BiomeID.Marble, BiomeID.Granite, BiomeID.Aether, BiomeID.Hive, BiomeID.GemCave, BiomeID.OceanCave }, progress);

            progress.Message = "Carving marble";

            Vector2 point;
            float threshold;

            FastNoiseLite terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            terrain.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            terrain.SetFrequency(0.02f);
            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrain.SetFractalOctaves(4);

            for (int y = (int)Main.rockLayer; y < GenVars.lavaLine + 50; y++)
            {
                progress.Set((y - (Main.maxTilesY - 200)) / 200);

                for (int x = (int)(Main.maxTilesX * 0.35f) - 100; x < (int)(Main.maxTilesX * 0.65f) + 100; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Marble)
                    {
                        point = new Vector2(MathHelper.Clamp(x, Main.maxTilesX * 0.4f + 50, Main.maxTilesX * 0.6f - 50), (MarbleCave.Y + 0.5f) * biomes.scale);

                        //if (y > point.Y)
                        //{
                        //    threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 40)) * 1.5f, 0, 1);
                        //}
                        //else
                        threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 80)), 0, 1) * 2 - 1;

                        if (terrain.GetNoise(x * 3, y) * 2f < threshold)
                        {
                            WGTools.Tile(x, y).HasTile = false;

                            if (y > point.Y + 32)
                            {
                                WGTools.Tile(x, y).LiquidAmount = 153;
                            }
                            else WGTools.Tile(x, y).LiquidAmount = 0;
                        }
                        if (terrain.GetNoise(x * 3, y) * 3f + 0.35f < threshold)
                        {
                            WGTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }

            progress.Message = "Carving granite";

            terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            terrain.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            terrain.SetFrequency(0.03f);
            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrain.SetFractalOctaves(2);

            for (int y = (int)Main.worldSurface - 25; y < Main.maxTilesY - 175; y++)
            {
                progress.Set((y - (Main.maxTilesY - 200)) / 200);

                for (int x = 400; x < Main.maxTilesX - 400; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.Granite)
                    {
                        point = new Vector2((Tundra.X + 0.5f) * biomes.scale, MathHelper.Clamp(y, (int)Main.worldSurface + 50, Main.maxTilesY - 350));
                        threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 75)) * 1.5f, 0, 1);

                        float _terrain = terrain.GetNoise(x, y * 2) * 1.5f;

                        if (_terrain + 0.5f < threshold * 2 - 1)
                        {
                            WGTools.Tile(x, y).HasTile = false;

                            if (Vector2.Distance(new Vector2(x, y), point) > 35)
                            {
                                WGTools.Tile(x, y).LiquidAmount = 255;
                            }
                            else WGTools.Tile(x, y).LiquidAmount = 0;
                        }

                        point.Y = MathHelper.Clamp(y, (int)Main.worldSurface + 50, Main.maxTilesY - 450);
                        threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 75)) * 1.5f, 0, 1);

                        if (_terrain + 1 < threshold * 2 - 1)
                        {
                            WGTools.Tile(x, y).WallType = 0;
                        }
                    }
                }
            }
            //biomes.UpdateMap(new int[] { BiomeID.Glowshroom, "granite", "marble" }, progress);

            //for (int y = (int)Main.worldSurface; y < Main.maxTilesY - 300; y++)
            //{
            //    for (int x = 40; x < Main.maxTilesX - 40; x++)
            //    {
            //        if (biomes.FindBiome(x, y) == "growth")
            //        {
            //            if (WGTools.SurroundingTilesActive(x, y, true))
            //            {
            //                if (WGTools.GetTile(x, y).TileType == ModContent.TileType<hardstone>())
            //                {
            //                    WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<hardstonewall>();
            //                }
            //                else WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<elderdirtwall>();
            //            }
            //            else if (WGTools.GetTile(x, y).TileType == ModContent.TileType<hardstone>())
            //            {
            //                WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<elderdirt>();
            //            }
            //        }
            //    }
            //}
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

            progress.Message = "Growing alien plants";

            for (int y = 40; y <= Main.maxTilesY - 200; y++)
            {
                progress.Set((float)((y - 40) / (float)(Main.maxTilesY - 200 - 40)));

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    #region plants
                    if (!tile.HasTile || Main.tileCut[tile.TileType])
                    {
                        if (y > Main.worldSurface)
                        {
                            if (RemTile.SolidTop(x, y + 1))
                            {
                                if (tile.LiquidType == 0)
                                {
                                    if (Framing.GetTileSafely(x, y + 1).TileType == TileID.SnowBlock || Framing.GetTileSafely(x, y + 1).TileType == TileID.IceBlock)
                                    {
                                        if (WorldGen.genRand.NextBool(2) && tile.LiquidAmount == 255)
                                        {
                                            int style = Main.rand.Next(9);
                                            WorldGen.PlaceTile(x, y, ModContent.TileType<Cryocoral>());
                                            Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
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
                                    else if (Framing.GetTileSafely(x, y + 1).TileType == TileID.Sand)
                                    {
                                        WorldGen.PlaceTile(x, y, TileID.Seaweed);

                                        for (int j = y - 1; j > (int)Main.worldSurface && !Framing.GetTileSafely(x, j).HasTile; j--)
                                        {
                                            WorldGen.PlaceTile(x, j, TileID.Seaweed);

                                            if (WorldGen.genRand.NextBool(20) || Framing.GetTileSafely(x, j - 1).LiquidAmount == 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else if (Framing.GetTileSafely(x, y + 1).TileType == TileID.Coralstone)
                                    {
                                        if (WorldGen.genRand.NextBool(2) && tile.LiquidAmount == 255)
                                        {
                                            int style = Main.rand.Next(12);
                                            WorldGen.PlaceTile(x, y, ModContent.TileType<Luminsponge>());
                                            Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                                        }
                                    }
                                }
                                //if (WorldGen.genRand.NextBool(6) && tile.LiquidAmount == 255 && tile.LiquidType == 0 && (Framing.GetTileSafely(x, y + 1).TileType == TileID.Stone || Main.tileMoss[Framing.GetTileSafely(x, y + 1).TileType]))
                                //{
                                //    int style = Main.rand.Next(3);
                                //    WorldGen.PlaceTile(x, y, ModContent.TileType<thermopod>());
                                //    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 22);
                                //}
                                if (WorldGen.genRand.NextBool(6))
                                {
                                    if (Framing.GetTileSafely(x, y + 1).TileType == TileID.Stone && biomes.FindBiome(x, y) == BiomeID.GemCave && Framing.GetTileSafely(x - 1, y).TileType != ModContent.TileType<Runestalk>() && Framing.GetTileSafely(x + 1, y).TileType != ModContent.TileType<Runestalk>())
                                    {
                                        //int style = Main.rand.Next(3);
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<Runestalk>());
                                        //Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 22);
                                    }
                                }
                            }
                            else if (RemTile.SolidBottom(x, y - 1))
                            {
                                if (WorldGen.genRand.NextBool(4))
                                {
                                    if (biomes.FindBiome(x, y) == BiomeID.Aether && Framing.GetTileSafely(x, y - 1).TileType == TileID.VioletMoss && tile.LiquidAmount == 0)
                                    {
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<DreampodVine>());

                                        for (int j = y + 1; j < Main.maxTilesY - 300 && !Framing.GetTileSafely(x, j).HasTile && Framing.GetTileSafely(x, j).LiquidAmount == 0; j++)
                                        {
                                            if ((WorldGen.genRand.NextBool(20) || j - y >= 20 || Framing.GetTileSafely(x, j + 2).HasTile))
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
                                    else if ((biomes.FindBiome(x, y) == BiomeID.Corruption || biomes.FindBiome(x, y) == BiomeID.Crimson) && (TileID.Sets.Corrupt[Framing.GetTileSafely(x, y - 1).TileType] || TileID.Sets.Crimson[Framing.GetTileSafely(x, y - 1).TileType]) && tile.LiquidAmount == 0)
                                    {
                                        int style = Main.rand.Next(3) + (TileID.Sets.Corrupt[Framing.GetTileSafely(x, y - 1).TileType] ? 3 : 0);
                                        WorldGen.PlaceTile(x, y, ModContent.TileType<EyeballVine>());
                                        Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                                    }
                                }
                            }
                        }
                        else if (WorldGen.genRand.NextBool(8))
                        {
                            if (RemTile.SolidTop(x, y + 1) && (Framing.GetTileSafely(x, y + 1).TileType == TileID.Grass || Framing.GetTileSafely(x, y + 1).TileType == TileID.JungleGrass))
                            {
                                WorldGen.PlaceTile(x, y, ModContent.TileType<Nightglow>(), style: Main.rand.Next(3));
                            }
                        }

                        #region unused
                        //if (biomes.FindBiome(x, y) == "flesh")
                        //{
                        //    if (WorldGen.genRand.NextBool(2))
                        //    {
                        //        if (Framing.GetTileSafely(x, y - 1).HasTile && Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<flesh>())
                        //        {
                        //            int style = Main.rand.Next(3);
                        //            WorldGen.PlaceTile(x, y, ModContent.TileType<hangingeyeballvine>());
                        //            Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                        //        }
                        //    }
                        //}
                        //else if (biomes.FindBiome(x, y) == "growth")
                        //{
                        //    if (WorldGen.genRand.NextBool(10))
                        //    {
                        //        if (Framing.GetTileSafely(x, y + 1).HasTile && Main.tileSolid[Framing.GetTileSafely(x, y).TileType])
                        //        {
                        //            if (!Framing.GetTileSafely(x, y).HasTile || !Main.tileSolid[Framing.GetTileSafely(x, y).TileType])
                        //            {
                        //                if (!Framing.GetTileSafely(x, y - 1).HasTile || !Main.tileSolid[Framing.GetTileSafely(x, y - 1).TileType])
                        //                {
                        //                    int style = Main.rand.Next(3);
                        //                    WorldGen.PlaceObject(x, y, ModContent.TileType<waxpillar>());
                        //                    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                        //                    Framing.GetTileSafely(x, y - 1).TileFrameX = (short)(style * 18);
                        //                }
                        //            }
                        //        }
                        //    }

                        //    if (!tile.HasTile)
                        //    {
                        //        if (Framing.GetTileSafely(x, y + 1).HasTile && (Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<elderdirt>() || Framing.GetTileSafely(x, y + 1).TileType == ModContent.TileType<livingelderwood>()))
                        //        {
                        //            WorldGen.PlaceTile(x, y, ModContent.TileType<eldersprout>());
                        //        }
                        //        else if (WorldGen.genRand.NextBool(2) && Framing.GetTileSafely(x, y - 1).HasTile && (Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<elderdirt>() || Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<livingelderwood>()))
                        //        {
                        //            int style = Main.rand.Next(3);
                        //            WorldGen.PlaceTile(x, y, ModContent.TileType<elderbulbvine>());
                        //            Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                        //        }
                        //    }
                        //    else if (tile.TileType == ModContent.TileType<elderdirt>() || tile.TileType == ModContent.TileType<livingelderwood>())
                        //    {
                        //        if (!WGTools.Tile(x + 1, y).HasTile && WGTools.Tile(x + 1, y).LiquidAmount != 255 && RemTile.SolidRight(x, y) && WorldGen.genRand.NextBool(2))
                        //        {
                        //            WGTools.Tile(x + 1, y).TileType = (ushort)ModContent.TileType<eldermushroom>();
                        //            WGTools.Tile(x + 1, y).HasTile = true;
                        //            Framing.GetTileSafely(x + 1, y).TileFrameY = (short)((short)Main.rand.Next(3) * 16);
                        //        }
                        //        if (!WGTools.Tile(x - 1, y).HasTile && WGTools.Tile(x - 1, y).LiquidAmount != 255 && RemTile.SolidLeft(x, y) && WorldGen.genRand.NextBool(2))
                        //        {
                        //            WGTools.Tile(x - 1, y).TileType = (ushort)ModContent.TileType<eldermushroom>();
                        //            WGTools.Tile(x - 1, y).HasTile = true;
                        //            Framing.GetTileSafely(x - 1, y).TileFrameX = 22;
                        //            Framing.GetTileSafely(x - 1, y).TileFrameY = (short)((short)Main.rand.Next(3) * 16);
                        //        }
                        //    }

                        //    if (!tile.HasTile || !Main.tileSolid[tile.TileType])
                        //    {
                        //        if (tile.WallType == ModContent.WallType<elderdirtwall>() && tile.LiquidAmount != 255)
                        //        {
                        //            tile.WallType = (ushort)ModContent.WallType<elderflowerwall>();
                        //        }
                        //    }

                        //    if (tile.TileType == ModContent.TileType<eldersprout>())
                        //    {
                        //        if (WorldGen.genRand.NextBool(3))
                        //        {
                        //            if (tile.LiquidAmount == 255)
                        //            {
                        //                tile.TileType = (ushort)ModContent.TileType<elderflowerstem>();
                        //            }
                        //            else tile.TileType = (ushort)ModContent.TileType<elderblossom>();
                        //        }
                        //        else
                        //        {
                        //            tile.TileType = (ushort)ModContent.TileType<growthplants>();
                        //            tile.TileFrameX = (short)(Main.rand.Next(6) * 22);
                        //            if (tile.LiquidAmount == 255)
                        //            {
                        //                tile.TileFrameY = 34;
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion
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

                    if (!Framing.GetTileSafely(x, y).HasTile)
                    {
                        if (Framing.GetTileSafely(x, y - 1).TileType == ModContent.TileType<EyeballVine>())
                        {
                            bool maxLength = true;

                            for (int a = 1; a <= 20; a++)
                            {
                                if (Framing.GetTileSafely(x, y - a).TileType != ModContent.TileType<EyeballVine>())
                                {
                                    maxLength = false;
                                    break;
                                }
                            }

                            if (!maxLength && Framing.GetTileSafely(x, y).LiquidAmount == 0)
                            {
                                if (WorldGen.genRand.NextBool(20))
                                {
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<Eyeball>());
                                    Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<Eyeball>();
                                }
                                else
                                {
                                    WorldGen.PlaceTile(x, y, ModContent.TileType<EyeballVine>());
                                    Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<EyeballVine>();
                                }
                                Framing.GetTileSafely(x, y).TileFrameX = (short)((Main.rand.Next(3) + (Framing.GetTileSafely(x, y - 1).TileFrameX >= 18 * 3 ? 3 : 0)) * 18);
                            }
                        }
                    }
                    #endregion
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
            progress.Message = "Forming clouds";

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int countInitial = (int)(Main.maxTilesX / 350f * ModContent.GetInstance<Client>().CloudDensity);// (int)(((Main.maxTilesX / 4200f * Main.worldSurface * 0.5f) / 20f) * ModContent.GetInstance<Client>().CloudDensity);
            int count = countInitial;
            while (count > 0)
            {
                progress.Set(1 - (count / (float)countInitial));

                bool valid = true;

                int height = WorldGen.genRand.Next((int)(20 * (Main.maxTilesY / 1200f)), (int)(40 * (Main.maxTilesY / 1200f)) + 1);
                if (count < countInitial / 2)
                {
                    height /= 4;
                }

                int width = height * 4;

                int padding = 40;

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(500 + padding, Main.maxTilesX / 2 - 200 - width) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 200, Main.maxTilesX - 500 - width - padding);
                int y = WorldGen.genRand.Next(100, (int)(Main.worldSurface * 0.5f) - height);

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
                                if (WGTools.SurroundingTilesActive(i - 1, j - 1))
                                {
                                    WGTools.Tile(i - 1, j - 1).WallType = WallID.Cloud;
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

                            if (WGTools.Tile(i - 1, j - 1).WallType == WallID.Cloud && rainCloud)
                            {
                                WGTools.Tile(i - 1, j - 1).WallColor = PaintID.BlackPaint;
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
            if (Main.tile[i - 1, j].LiquidAmount == 255)
            {
                int k = 1;
                while (Main.tile[i - k, j].LiquidAmount == 255)
                {
                    Main.tile[i - k, j].LiquidAmount = 0;

                    if (Main.tile[i - k, j - 1].LiquidAmount == 255)
                    {
                        ClearExcessWater(i - k, j - 1);
                    }

                    k++;
                }
            }
            if (Main.tile[i + 1, j].LiquidAmount == 255)
            {
                int k = 1;
                while (Main.tile[i + k, j].LiquidAmount == 255)
                {
                    Main.tile[i + k, j].LiquidAmount = 0;

                    if (Main.tile[i + k, j - 1].LiquidAmount == 255)
                    {
                        ClearExcessWater(i + k, j - 1);
                    }

                    k++;
                }
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

            progress.Message = "Placing gems";

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
                            if (!tile.HasTile && WGTools.AdjacentTiles(x, y, true) > 0 && WorldGen.genRand.NextBool(4))
                            {
                                WorldGen.PlaceTile(x, y, TileID.ExposedGems, style: RemTile.GetGemType(y));
                            }
                        }
                    }
                }
            }
        }
    }

    public class GemTrees : GenPass
    {
        public GemTrees(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            for (int y = (int)Main.rockLayer; y < Main.maxTilesY - 40; y++)
            {
                progress.Set((y - (int)Main.rockLayer) / (Main.maxTilesY - 200 - (int)Main.rockLayer));

                for (int x = 300; x < Main.maxTilesX - 300; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (biomes.FindBiome(x, y) == BiomeID.GemCave)
                    {
                        if (!tile.HasTile && WGTools.Tile(x, y + 1).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.Stone)
                        {
                            int gemType = RemTile.GetGemType(y);
                            ushort gemTree = gemType == 5 ? TileID.TreeDiamond : gemType == 4 ? TileID.TreeRuby : gemType == 3 ? TileID.TreeEmerald : gemType == 2 ? TileID.TreeSapphire : gemType == 1 ? TileID.TreeTopaz : TileID.TreeAmethyst;

                            ushort wallType = tile.WallType;
                            WorldGen.KillWall(x, y);
                            WorldGen.TryGrowingTreeByType(gemTree, x, y + 1);
                            tile.WallType = wallType;
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.AshForest)
                    {
                        if (!tile.HasTile && WGTools.Tile(x, y + 1).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.AshGrass && WorldGen.genRand.NextBool(2))
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

    public class Hell : GenPass
    {
        public Hell(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Creating the underworld";

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
                            threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / (50 + distance.GetNoise(x, y) * 50))), 0, 1);
                        }
                        else threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / (50 + distance.GetNoise(x, y) * 50))), 0, 1);

                        float _terrain = terrain.GetNoise(x, y * 2);
                        if (_terrain < threshold - 0.25f && WGTools.Tile(x, y).TileType != ModContent.TileType<Hardstone>())
                        {
                            WGTools.Tile(x, y).HasTile = false;
                            if (WGTools.Tile(x, y).WallType == WallID.ObsidianBackUnsafe)
                            {
                                WGTools.Tile(x, y).WallType = 0;
                            }
                            if (y <= point.Y - 40 && y >= point.Y - 50 || y > point.Y + 40 + distance.GetNoise(x, y) * 20)
                            {
                                WGTools.Tile(x, y).LiquidAmount = 255;
                            }
                            //float _background = background.GetNoise(x, y);
                        }

                        if (_terrain / 2 > threshold - 0.6f && WGTools.Tile(x, y).WallType == 0)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.AshForest)
                            {
                                WGTools.Tile(x, y).WallType = WallID.LavaUnsafe4;
                            }
                            else if (_terrain / 2 > threshold - 0.5f)
                            {
                                WGTools.Tile(x, y).WallType = WallID.LavaUnsafe1;
                            }
                            else WGTools.Tile(x, y).WallType = WallID.LavaUnsafe3;
                        }

                        if (biomes.FindBiome(x, y) == BiomeID.AshForest && !WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Ash)
                            {
                                WGTools.Tile(x - 1, y - 1).TileType = TileID.AshGrass;
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
                        WGTools.Tile(x, y).LiquidType = 1;
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
                    if (WGTools.Tile(structureX, structureY).HasTile)
                    {
                        valid = true;
                    }
                    if (WGTools.Tile(structureX, structureY).LiquidAmount == 255)
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
                        if (WGTools.Tile(structureX + x, structureY).HasTile || WGTools.Tile(structureX + x, structureY).LiquidAmount == 255)
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

                    Generator.GenerateStructure("Structures/common/treasurevault/entrance", new Point16(structureX - 7, structureY - 11), ModContent.GetInstance<Remnants>());

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
                    Tile tile = WGTools.Tile(x, y);

                    if (WGTools.Tile(x, y).WallType == WallID.ObsidianBrick)
                    {
                        WGTools.Tile(x, y).WallType = WallID.ObsidianBrickUnsafe;
                    }
                    if (WGTools.Tile(x, y).WallType == WallID.HellstoneBrick)
                    {
                        WGTools.Tile(x, y).WallType = WallID.HellstoneBrickUnsafe;
                    }

                    if (!GenVars.structures.CanPlace(new Rectangle(x - 2, y - 2, 5, 5)))
                    {
                        if (tile.TileType != TileID.ObsidianBrick && tile.TileType != TileID.HellstoneBrick && tile.TileType != TileID.Spikes && tile.WallType != WallID.ObsidianBrickUnsafe && tile.WallType != WallID.HellstoneBrickUnsafe)
                        {
                            if (WGTools.Solid(x, y) && tile.TileType != TileID.ClosedDoor)
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

                if (Framing.GetTileSafely(x, y).TileType == TileID.Banners || Framing.GetTileSafely(x, y - 1).TileType != TileID.ObsidianBrick || WGTools.Tile(x, y).WallType != WallID.ObsidianBrickUnsafe)
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
            //        Generator.GenerateStructure("Structures/cage", new Point16(structureX - 2, structureY), ModContent.GetInstance<Remnants>());
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
                if (Framing.GetTileSafely(x, y).TileType == TileID.Hellforge || WGTools.Tile(x, y).WallType != WallID.ObsidianBrickUnsafe)
                {
                    valid = false;
                }
                if (WorldGen.SolidTile3(x - 2, y) || WorldGen.SolidTile3(x + 2, y))
                {
                    valid = false;
                }
                for (int i = -1; i <= 1; i++)
                {
                    if (WGTools.Tile(x + i, y + 1).TileType != TileID.ObsidianBrick)
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
                if (Framing.GetTileSafely(x, y).TileType == TileID.Containers || WGTools.Tile(x, y).WallType != WallID.ObsidianBrickUnsafe)
                {
                    valid = false;
                }
                if (WorldGen.SolidTile3(x - 1, y) || WorldGen.SolidTile3(x + 2, y))
                {
                    valid = false;
                }
                for (int i = 0; i <= 1; i++)
                {
                    if (WGTools.Tile(x + i, y + 1).TileType != TileID.ObsidianBrick)
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

                        Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.ObsidianSkinPotion, ItemID.InfernoPotion }, true);

                        itemsToAdd.Add((ItemID.Hellstone, Main.rand.Next(3, 6) * 3));
                        if (Main.rand.NextBool(2))
                        {
                            itemsToAdd.Add((ItemID.Meteorite, Main.rand.Next(6, 12) * 3));
                        }
                        if (Main.rand.NextBool(2))
                        {
                            itemsToAdd.Add((ItemID.Obsidian, Main.rand.Next(3, 6)));
                        }

                        Structures.FillChest(chestIndex, itemsToAdd);
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
                room.X -= (room.Width - 1);
            }
            if (savedMarker.direction == 1)
            {
                room.Y -= (room.Height - 1);
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
                    Generator.GenerateStructure("Structures/common/treasurevault/shaft", position, ModContent.GetInstance<Remnants>());
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
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/centralshaft", position, ModContent.GetInstance<Remnants>());
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
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/small1", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 2)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/small2", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 3)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/small3", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 4)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/medium1", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 5)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/medium2", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 6)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/medium3", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 7)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/large1", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 8)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/treasurevault/large2", position, ModContent.GetInstance<Remnants>());
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
                    Generator.GenerateStructure("Structures/common/treasurevault/doorway", new Point16(room.X - 2, room.Y + (doorY - 2)), ModContent.GetInstance<Remnants>());
                }
                else if (savedMarker.direction == 4)
                {
                    Generator.GenerateStructure("Structures/common/treasurevault/doorway", new Point16(room.X + (room.Width - 1), room.Y + (doorY - 2)), ModContent.GetInstance<Remnants>());
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
                WGTools.Rectangle(x, y - 1, x, y + 1, TileID.ObsidianBrick);
            }
            else if (markers[index].direction == 1 && !WorldGen.SolidTile3(x, y - 1))
            {
                if (markers[index].type == 2)
                {
                    WGTools.Rectangle(x - 2, y, x + 2, y, TileID.Platforms, style: 13);
                }
                else WGTools.Rectangle(x - 1, y, x + 1, y, TileID.Platforms, style: 13);
            }
            else
            {
                if (markers[index].type == 2)
                {
                    WGTools.Rectangle(x - 2, y, x + 2, y, TileID.ObsidianBrick);
                }
                else WGTools.Rectangle(x - 1, y, x + 1, y, TileID.ObsidianBrick);
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
                    float threshold = (Vector2.Distance(position, new Vector2(x, y)) / size);
                    Tile tile = WGTools.Tile(x, y);

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
                    for (; !WGTools.Tile(structureX, structureY).HasTile; structureY += 1)
                    {
                    }
                    bool valid = true;
                    for (int i = structureX - 1; i <= structureX + width + 1; i++)
                    {
                        if (WGTools.Tile(i, structureY - 1).HasTile || !WGTools.Tile(i, structureY + 1).HasTile || WGTools.Tile(i, structureY - 1).WallType != WallID.ObsidianBrickUnsafe)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        WGTools.Rectangle(structureX - 1, structureY + 1, structureX + width + 1, structureY + 2, TileID.HellstoneBrick);
                        WGTools.Rectangle(structureX, structureY, structureX + width, structureY + 1, -1);
                        WGTools.Rectangle(structureX, structureY, structureX + width, structureY, wall: WallID.ObsidianBrickUnsafe);
                        WGTools.Rectangle(structureX, structureY + 1, structureX + width, structureY + 1, wall: WallID.HellstoneBrickUnsafe, liquid: 255, liquidType: 1);

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
            return WGTools.Solid(x, y) && WGTools.Tile(x, y).TileType == TileID.ObsidianBrick;
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

                if (WGTools.Tile(structureX, structureY).LiquidAmount != 255)
                {
                    valid = false;
                }
                else for (int j = -2; j <= 3; j++)
                    {
                        for (int i = -2; i <= 2; i++)
                        {
                            if (WGTools.Tile(structureX + i, structureY + j).HasTile)
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
                    Generator.GenerateStructure("Structures/spikeball", new Point16(structureX - 1, structureY - 2), ModContent.GetInstance<Remnants>());

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