//using Microsoft.Xna.Framework;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.ID;
//using Terraria.IO;
//using Terraria.ModLoader;
//using Terraria.WorldBuilding;
////using SubworldLibrary;
//using Remnants.Tiles.blocks;
//using Remnants.Walls;
//using Remnants.Walls.dev;
//using Remnants.Tiles;
//using StructureHelper;
//using Terraria.DataStructures;
//using Remnants.Walls.bg;
//using System;
//using Terraria.Utilities;

//namespace Remnants.Worldgen.Subworlds
//{
//    public class TheColony : Subworld
//    {
//        public override int Width => 1000;
//        public override int Height => 400;

//        public override bool ShouldSave => false;
//        public override bool NormalUpdates => true;

//        public override List<GenPass> Tasks => new List<GenPass>()
//        {
//            new WorldSetup("World Setup", 1),
//            new Terrain("Terrain", 1),
//            new Structures("Structures", 1),
//            new SettleLiquids("Settle Liquids", 1),
//            new HoneyPatches("Honey Patches", 1),
//            new Decorations("Decorations", 1)
//        };

//        //public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
//        //{
//        //    bool tileBlock = tile.HasTile && Main.tileBlockLight[tile.TileType] && !(tile.Slope != SlopeType.Solid || tile.IsHalfBlock);
//        //    bool wallBlock = !Main.wallLight[tile.WallType];
//        //    if (!tileBlock && !wallBlock)
//        //    {
//        //        color = new Vector3(255, 239, 102) / 255;
//        //    }
//        //    return true;
//        //}
//    }

//    public class WorldSetup : GenPass
//    {
//        public WorldSetup(string name, float loadWeight) : base(name, loadWeight)
//        {
//        }

//        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//        {
//            progress.Message = "Setting things up";

//            Main.worldSurface = 1;
//            Main.rockLayer = Main.maxTilesY;

//            Main.spawnTileX = Main.maxTilesX / 2;
//            Main.spawnTileY = 150;
//        }
//    }

//    public class Terrain : GenPass
//    {
//        public Terrain(string name, float loadWeight) : base(name, loadWeight)
//        {
//        }

//        public bool flipDirection;

//        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//        {
//            progress.Message = "Generating terrain";

//            FastNoiseLite terrain = new FastNoiseLite(Main.rand.Next(-2147483648, 2147483647) + 1);
//            terrain.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//            terrain.SetFrequency(0.02f);
//            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
//            terrain.SetFractalOctaves(5);

//            FastNoiseLite honeycombs = new FastNoiseLite(Main.rand.Next(-2147483648, 2147483647) + 1);
//            honeycombs.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
//            honeycombs.SetFrequency(0.015f);
//            honeycombs.SetFractalType(FastNoiseLite.FractalType.FBm);
//            honeycombs.SetFractalOctaves(5);

//            for (float y = 20; y < Main.maxTilesY - 20; y++)
//            {
//                for (float x = 20; x < Main.maxTilesX - 20; x++)
//                {
//                    Tile tile = WGTools.GetTile(x, y);

//                    Vector2 point = new Vector2(MathHelper.Clamp(x, 300, Main.maxTilesX - 300), 150);

//                    float _terrain = terrain.GetNoise(x, y);

//                    float threshold;
//                    if (y > point.Y)
//                    {
//                        threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 150)), 0, 0.7f);
//                    }
//                    else threshold = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 100)) * 2, 0, 0.7f);

//                    WGTools.GetTile(x, y).LiquidType = 2;

//                    tile.TileType = TileID.Hive;
//                    if (_terrain > threshold - 0.7f)
//                    {
//                        tile.WallType = WallID.HiveUnsafe;
//                    }
//                    if (_terrain > threshold - 0.5f)
//                    {
//                        tile.HasTile = true;

//                        if (_terrain > threshold)
//                        {
//                            tile.TileType = TileID.Mud;
//                        }
//                    }
//                    else if (honeycombs.GetNoise(x, y) > 0.1f)
//                    {
//                        if (terrain.GetNoise(x * 5, y * 5) > (Math.Sin(y / 2) * 0.25f) - 0.3f)
//                        {
//                            tile.WallType = WallID.HiveUnsafe;

//                            if (terrain.GetNoise(x * 5, y * 5) > (Math.Sin(y / 2) * 0.25f) + 0.075f)
//                            {
//                                tile.HasTile = true;
//                            }
//                        }
//                        else tile.WallType = 0;
//                        tile.TileType = TileID.Hive;
//                    }

//                    if (!tile.HasTile && (y >= 210 || Main.rand.NextBool(20)))
//                    {
//                        WGTools.GetTile(x, y).LiquidAmount = 255;
//                    }
//                }
//            }

//            #region borders
//            for (int y = 0; y < Main.maxTilesY; y++)
//            {
//                for (int x = 0; x < 20; x++)
//                {
//                    WGTools.GetTile(x, y).HasTile = true;
//                    WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<hardstone>();
//                }
//                for (int x = Main.maxTilesX - 20; x < Main.maxTilesX; x++)
//                {
//                    WGTools.GetTile(x, y).HasTile = true;
//                    WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<hardstone>();
//                }
//            }
//            for (int x = 20; x < Main.maxTilesX - 20; x++)
//            {
//                for (int y = 0; y <20; y++)
//                {
//                    WGTools.GetTile(x, y).HasTile = true;
//                    WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<hardstone>();
//                }
//                for (int y = Main.maxTilesY - 20; y < Main.maxTilesY; y++)
//                {
//                    WGTools.GetTile(x, y).HasTile = true;
//                    WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<hardstone>();
//                }
//            }
//            #endregion

//            SmoothWorld();

//            #region walls
//            for (float y = 0; y < Main.maxTilesY; y++)
//            {
//                for (float x = 0; x < Main.maxTilesX; x++)
//                {
//                    Tile tile = WGTools.GetTile(x, y);

//                    if (WGTools.SurroundingTilesActive(x, y))
//                    {
//                        if (tile.TileType == TileID.Hive)
//                        {
//                            tile.WallType = WallID.HiveUnsafe;
//                        }
//                        else if (tile.TileType == TileID.Mud || tile.TileType == TileID.JungleGrass)
//                        {
//                            tile.WallType = WallID.MudUnsafe;
//                        }
//                        else if (tile.TileType == ModContent.TileType<honeybrick>())
//                        {
//                            tile.WallType = (ushort)ModContent.WallType<honeybrickwall>();
//                        }
//                        else if (tile.TileType == ModContent.TileType<hardstone>())
//                        {
//                            tile.WallType = (ushort)ModContent.WallType<hardstonewall>();
//                        }
//                    }
//                }
//            }
//            #endregion
//        }

//        private void SmoothWorld()
//        {
//            for (int x = 20; x < Main.maxTilesX - 20; x++)
//            {
//                for (int y = 20; y < Main.maxTilesY - 20; y++)
//                {
//                    if (Main.tile[x, y].TileType != 48 && Main.tile[x, y].TileType != 137 && Main.tile[x, y].TileType != 232 && Main.tile[x, y].TileType != 191 && Main.tile[x, y].TileType != 151 && Main.tile[x, y].TileType != 274)
//                    {
//                        if (!Main.tile[x, y - 1].HasTile && Main.tile[x - 1, y].TileType != 136 && Main.tile[x + 1, y].TileType != 136)
//                        {
//                            if (WorldGen.SolidTile(x, y) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[x, y].TileType])
//                            {
//                                if (!Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == SlopeType.Solid && Main.tile[x + 1, y].Slope == SlopeType.Solid)
//                                {
//                                    if (WorldGen.SolidTile(x, y + 1))
//                                    {
//                                        if (!WorldGen.SolidTile(x - 1, y) && !Main.tile[x - 1, y + 1].IsHalfBlock && WorldGen.SolidTile(x - 1, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x + 1, y - 1].HasTile)
//                                        {
//                                            if (Main.rand.NextBool(2))
//                                                WorldGen.SlopeTile(x, y, 2);
//                                            else
//                                                WorldGen.PoundTile(x, y);
//                                        }
//                                        else if (!WorldGen.SolidTile(x + 1, y) && !Main.tile[x + 1, y + 1].IsHalfBlock && WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x - 1, y - 1].HasTile)
//                                        {
//                                            if (Main.rand.NextBool(2))
//                                                WorldGen.SlopeTile(x, y, 1);
//                                            else
//                                                WorldGen.PoundTile(x, y);
//                                        }
//                                        else if (WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y + 1) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y].HasTile)
//                                        {
//                                            WorldGen.PoundTile(x, y);
//                                        }

//                                        if (WorldGen.SolidTile(x, y))
//                                        {
//                                            if (WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x + 1, y + 2) && !Main.tile[x + 1, y].HasTile && !Main.tile[x + 1, y + 1].HasTile && !Main.tile[x - 1, y - 1].HasTile)
//                                            {
//                                                WorldGen.KillTile(x, y);
//                                            }
//                                            else if (WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x - 1, y + 2) && !Main.tile[x - 1, y].HasTile && !Main.tile[x - 1, y + 1].HasTile && !Main.tile[x + 1, y - 1].HasTile)
//                                            {
//                                                WorldGen.KillTile(x, y);
//                                            }
//                                            else if (!Main.tile[x - 1, y + 1].HasTile && !Main.tile[x - 1, y].HasTile && WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x, y + 2))
//                                            {
//                                                if (Main.rand.NextBool(5))
//                                                    WorldGen.KillTile(x, y);
//                                                else if (Main.rand.NextBool(5))
//                                                    WorldGen.PoundTile(x, y);
//                                                else
//                                                    WorldGen.SlopeTile(x, y, 2);
//                                            }
//                                            else if (!Main.tile[x + 1, y + 1].HasTile && !Main.tile[x + 1, y].HasTile && WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x, y + 2))
//                                            {
//                                                if (Main.rand.NextBool(5))
//                                                    WorldGen.KillTile(x, y);
//                                                else if (Main.rand.NextBool(5))
//                                                    WorldGen.PoundTile(x, y);
//                                                else
//                                                    WorldGen.SlopeTile(x, y, 1);
//                                            }
//                                        }
//                                    }

//                                    if (WorldGen.SolidTile(x, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y].HasTile)
//                                        WorldGen.KillTile(x, y);
//                                }
//                            }
//                            else if (!Main.tile[x, y].HasTile && Main.tile[x, y + 1].TileType != 151 && Main.tile[x, y + 1].TileType != 274)
//                            {
//                                if (Main.tile[x + 1, y].TileType != 190 && Main.tile[x + 1, y].TileType != 48 && Main.tile[x + 1, y].TileType != 232 && WorldGen.SolidTile(x - 1, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y - 1].HasTile)
//                                {
//                                    if (Main.tile[x + 1, y].TileType == 495)
//                                        WorldGen.PlaceTile(x, y, Main.tile[x + 1, y].TileType);
//                                    else
//                                        WorldGen.PlaceTile(x, y, Main.tile[x, y + 1].TileType);

//                                    if (Main.rand.NextBool(2))
//                                        WorldGen.SlopeTile(x, y, 2);
//                                    else
//                                        WorldGen.PoundTile(x, y);
//                                }

//                                if (Main.tile[x - 1, y].TileType != 190 && Main.tile[x - 1, y].TileType != 48 && Main.tile[x - 1, y].TileType != 232 && WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y - 1].HasTile)
//                                {
//                                    if (Main.tile[x - 1, y].TileType == 495)
//                                        WorldGen.PlaceTile(x, y, Main.tile[x - 1, y].TileType);
//                                    else
//                                        WorldGen.PlaceTile(x, y, Main.tile[x, y + 1].TileType);

//                                    if (Main.rand.NextBool(2))
//                                        WorldGen.SlopeTile(x, y, 1);
//                                    else
//                                        WorldGen.PoundTile(x, y);
//                                }
//                            }
//                        }
//                        else if (!Main.tile[x, y + 1].HasTile && Main.rand.NextBool(2) && WorldGen.SolidTile(x, y) && !Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == SlopeType.Solid && Main.tile[x + 1, y].Slope == SlopeType.Solid && WorldGen.SolidTile(x, y - 1))
//                        {
//                            if (WorldGen.SolidTile(x - 1, y) && !WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x - 1, y - 1))
//                                WorldGen.SlopeTile(x, y, 3);
//                            else if (WorldGen.SolidTile(x + 1, y) && !WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x + 1, y - 1))
//                                WorldGen.SlopeTile(x, y, 4);
//                        }

//                        if (TileID.Sets.Conversion.Sand[Main.tile[x, y].TileType])
//                            Tile.SmoothSlope(x, y, applyToNeighbors: false);
//                    }
//                }
//            }

//            for (int num513 = 20; num513 < Main.maxTilesX - 20; num513++)
//            {
//                for (int num514 = 20; num514 < Main.maxTilesY - 20; num514++)
//                {
//                    if (Main.rand.NextBool(2) && !Main.tile[num513, num514 - 1].HasTile && Main.tile[num513, num514].TileType != 137 && Main.tile[num513, num514].TileType != 48 && Main.tile[num513, num514].TileType != 232 && Main.tile[num513, num514].TileType != 191 && Main.tile[num513, num514].TileType != 151 && Main.tile[num513, num514].TileType != 274 && Main.tile[num513, num514].TileType != 75 && Main.tile[num513, num514].TileType != 76 && WorldGen.SolidTile(num513, num514) && Main.tile[num513 - 1, num514].TileType != 137 && Main.tile[num513 + 1, num514].TileType != 137)
//                    {
//                        if (WorldGen.SolidTile(num513, num514 + 1) && WorldGen.SolidTile(num513 + 1, num514) && !Main.tile[num513 - 1, num514].HasTile)
//                            WorldGen.SlopeTile(num513, num514, 2);

//                        if (WorldGen.SolidTile(num513, num514 + 1) && WorldGen.SolidTile(num513 - 1, num514) && !Main.tile[num513 + 1, num514].HasTile)
//                            WorldGen.SlopeTile(num513, num514, 1);
//                    }

//                    if (Main.tile[num513, num514].Slope == SlopeType.SlopeDownLeft && !WorldGen.SolidTile(num513 - 1, num514))
//                    {
//                        WorldGen.SlopeTile(num513, num514);
//                        WorldGen.PoundTile(num513, num514);
//                    }

//                    if (Main.tile[num513, num514].Slope == SlopeType.SlopeDownRight && !WorldGen.SolidTile(num513 + 1, num514))
//                    {
//                        WorldGen.SlopeTile(num513, num514);
//                        WorldGen.PoundTile(num513, num514);
//                    }
//                }
//            }
//        }
//    }

//    public class Structures : GenPass
//    {
//        public Structures(string name, float loadWeight) : base(name, loadWeight)
//        {
//        }

//        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//        {
//            progress.Message = "Building structures";

//            bool flipDirection = Main.rand.NextBool(2);

//            int structureX = flipDirection ? Main.maxTilesX - 250 : 250;
//            int structureY = Main.rand.Next(125, 175);
//            bool valid = false;
//            while (!valid)
//            {
//                structureX += flipDirection ? 1 : -1;
//                valid = true;
//                for (int y = structureY - 13; y <= structureY + 3; y++)
//                {
//                    if (!WGTools.GetTile(flipDirection ? structureX - 9 : structureX + 9, y).HasTile)
//                    {
//                        valid = false;
//                    }
//                }
//            }
//            WGTools.Terraform(new Vector2(structureX, structureY - 5), 9, scaleY: 2);
//            WGTools.Terraform(new Vector2(flipDirection ? structureX - 9 : structureX + 9, structureY - 3), 3.5f, true, 2);
//            Generator.GenerateStructure("Structures/colony-exit", new Point16(structureX - 9, structureY - 13), ModContent.GetInstance<Remnants>());

//            WorldGen.KillTile(flipDirection ? structureX + 9 : structureX - 9, structureY - 1);
//            WGTools.DrawRectangle(flipDirection ? structureX + 9 : structureX - 9, structureY - 2, flipDirection ? structureX + 9 : structureX - 9, structureY, ModContent.TileType<honeybrick>());

//            Main.spawnTileX = structureX;
//            Main.spawnTileY = structureY - 2;

//            structureX = !flipDirection ? Main.maxTilesX - 250 : 250;
//            structureY = Main.rand.Next(125, 175);
//            valid = false;
//            while (!valid)
//            {
//                structureX += !flipDirection ? 1 : -1;
//                valid = true;
//                for (int y = structureY - 13; y <= structureY + 3; y++)
//                {
//                    if (!WGTools.GetTile(!flipDirection ? structureX - 44 : structureX + 44, y).HasTile)
//                    {
//                        valid = false;
//                    }
//                }
//            }
//            WGTools.Terraform(new Vector2(structureX, structureY - 16), 44, scaleY: 2);
//            WGTools.Terraform(new Vector2(!flipDirection ? structureX - 44 : structureX + 44, structureY - 8), 8.5f, true, 4);
//            Generator.GenerateStructure("Structures/colony-bossroom", new Point16(structureX - 44, structureY - 35), ModContent.GetInstance<Remnants>());

//            WorldGen.KillTile(!flipDirection ? structureX + 44 : structureX - 44, structureY - 1);
//            WGTools.DrawRectangle(!flipDirection ? structureX + 44 : structureX - 44, structureY - 2, !flipDirection ? structureX + 44 : structureX - 44, structureY, ModContent.TileType<honeybrick>());

//            WGTools.Terraform(new Vector2(structureX, structureY + 1), 2, true, scaleX: 4);
//            WGTools.DrawRectangle(structureX - 14, structureY + 2, structureX + 14, structureY + 8, liquid: 255, liquidType: 2);

//            #region larva
//            int larvaX = structureX + WorldGen.genRand.Next(-5, 6);
//            int larvaY = structureY;
//            while (!WGTools.GetTile(larvaX, larvaY + 1).HasTile)
//            {
//                larvaY++;
//            }
//            WGTools.DrawRectangle(larvaX - 1, larvaY - 2, larvaX + 1, larvaY, -1);
//            WGTools.GetTile(larvaX - 1, larvaY + 1).HasTile = true;
//            WGTools.GetTile(larvaX + 1, larvaY + 1).HasTile = true;

//            WorldGen.PlaceObject(larvaX, larvaY, TileID.Larva);
//            #endregion
//        }
//    }

//    public class SettleLiquids : GenPass
//    {
//        public SettleLiquids(string name, float loadWeight) : base(name, loadWeight)
//        {
//        }

//        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//        {
//            progress.Message = "Settling liquids";

//            Liquid.worldGenTilesIgnoreWater(ignoreSolids: true);
//            Liquid.QuickWater(3);
//            WorldGen.WaterCheck();
//            int num538 = 0;
//            Liquid.quickSettle = true;
//            while (num538 < 10)
//            {
//                int num539 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
//                num538++;
//                float num540 = 0f;
//                while (Liquid.numLiquid > 0)
//                {
//                    float num541 = (float)(num539 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num539;
//                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num539)
//                        num539 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;

//                    if (num541 > num540)
//                        num540 = num541;
//                    else
//                        num541 = num540;

//                    if (num538 == 1)
//                        progress.Set(num541 / 3f + 0.33f);

//                    int num542 = 10;
//                    if (num538 > num542)
//                        num542 = num538;

//                    Liquid.UpdateLiquid();
//                }

//                WorldGen.WaterCheck();
//                progress.Set((float)num538 * 0.1f / 3f + 0.66f);
//            }

//            Liquid.quickSettle = false;
//            Liquid.worldGenTilesIgnoreWater(ignoreSolids: false);
//        }
//    }

//    public class HoneyPatches : GenPass
//    {
//        public HoneyPatches(string name, float loadWeight) : base(name, loadWeight)
//        {
//        }

//        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//        {
//            progress.Message = "Condensing honey";

//            for (int y = 20; y < Main.maxTilesY - 20; y++)
//            {
//                for (int x = 20; x < Main.maxTilesX - 20; x++)
//                {
//                    if (WGTools.FullTile(x, y) && Main.rand.NextBool(2) && (HoneyTile(x, y - 1) || HoneyTile(x - 1, y) || HoneyTile(x + 1, y)))
//                    {
//                        WorldGen.TileRunner(x, y, Main.rand.Next(2, 4) * 2, 1, TileID.HoneyBlock, ignoreTileType: ModContent.TileType<honeybrick>());
//                    }
//                }
//            }
//        }

//        public bool HoneyTile(int x, int y)
//        {
//            return !WGTools.GetTile(x, y).HasTile && WGTools.GetTile(x, y).LiquidAmount == 255;
//        }
//    }

//    public class Decorations : GenPass
//    {
//        public Decorations(string name, float loadWeight) : base(name, loadWeight)
//        {
//        }

//        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//        {
//            progress.Message = "Adding decorations";

//            int structureCount = Main.maxTilesX * Main.maxTilesY / 2000;

//            while (structureCount > 0)
//            {
//                int structureX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
//                int structureY = WorldGen.genRand.Next(50, 250);
//                int radius = WorldGen.genRand.Next(4, 13);

//                bool valid = true;

//                if (!Framing.GetTileSafely(structureX, structureY).HasTile)
//                {
//                    valid = false;
//                }
//                else if (Framing.GetTileSafely(structureX, structureY).TileType != TileID.Hive)
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    for (int y = structureY - radius; y <= structureY + radius; y++)
//                    {
//                        for (int x = structureX - radius; x <= structureX + radius; x++)
//                        {
//                            Tile tile = Main.tile[x, y];
//                            if (tile.TileType == TileID.Hive)
//                            {
//                                if (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) < WorldGen.genRand.Next(0, radius) && WorldGen.genRand.NextBool(2))
//                                {
//                                    tile.TileType = (ushort)ModContent.TileType<honeybrick>();
//                                }
//                            }

//                            if (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) < WorldGen.genRand.Next(0, radius) && WorldGen.genRand.NextBool(2))
//                            {
//                                if (tile.WallType == WallID.HiveUnsafe)
//                                {
//                                    tile.WallType = (ushort)ModContent.WallType<honeybrickwall>();
//                                }
//                            }
//                        }
//                    }

//                    structureCount--;
//                }
//            }

//            for (int y = 20; y < 200; y++)
//            {
//                for (int x = 20; x < Main.maxTilesX - 20; x++)
//                {
//                    Tile tile = WGTools.GetTile(x, y);

//                    if (WGTools.FullTile(x, y) && WGTools.SurroundingTilesActive(x, y, true))
//                    {
//                        if (Main.rand.NextBool(15) && !WGTools.GetTile(x + 2, y).HasTile && WGTools.GetTile(x - 2, y).LiquidAmount != 255 && WGTools.GetTile(x + 1, y).TileType != ModContent.TileType<honeybrick>())
//                        {
//                            WGTools.GetTile(x + 1, y).IsHalfBlock = true;
//                            tile.HasTile = false;
//                            tile.LiquidAmount = 255;
//                        }
//                        if (Main.rand.NextBool(15) && !WGTools.GetTile(x - 2, y).HasTile && WGTools.GetTile(x - 2, y).LiquidAmount != 255 && WGTools.GetTile(x - 1, y).TileType != ModContent.TileType<honeybrick>())
//                        {
//                            WGTools.GetTile(x - 1, y).IsHalfBlock = true;
//                            tile.HasTile = false;
//                            tile.LiquidAmount = 255;
//                        }
//                    }
//                }
//            }

//            for (int y = 20; y < Main.maxTilesY - 20; y++)
//            {
//                for (int x = 20; x < Main.maxTilesX - 20; x++)
//                {
//                    Tile tile = Main.tile[x, y];

//                    //if (!tile.HasTile && Main.rand.NextBool(2))
//                    //{
//                    //    bool top = false;
//                    //    bool bottom = false;
//                    //    if (WGTools.FullTile(x, y - 1) && WGTools.GetTile(x, y - 1).TileType == TileID.Hive)
//                    //    {
//                    //        top = true;
//                    //    }
//                    //    if (WGTools.FullTile(x, y + 1) && WGTools.GetTile(x, y + 1).TileType == TileID.Hive)
//                    //    {
//                    //        bottom = true;
//                    //    }
//                    //    if (top || bottom)
//                    //    {
//                    //        tile.HasTile = true;
//                    //        tile.TileType = TileID.Stalactite;
//                    //        tile.TileFrameX = (short)(WorldGen.genRand.Next(9, 12) * 18);

//                    //        if (top && bottom)
//                    //        {
//                    //            tile.TileFrameY = (short)(WorldGen.genRand.Next(4, 6) * 18);
//                    //        }
//                    //        else if (top)
//                    //        {
//                    //            tile.TileFrameY = 4 * 18;
//                    //        }
//                    //        else if (bottom)
//                    //        {
//                    //            tile.TileFrameY = 5 * 18;
//                    //        }
//                    //    }
//                    //}
//                    if (!tile.HasTile && tile.LiquidAmount != 255 && WGTools.FullTile(x, y - 1) && Main.rand.NextBool(2))
//                    {
//                        WorldGen.PlaceTile(x, y, TileID.HoneyDrip);
//                    }
//                }
//            }
//        }
//    }
//}