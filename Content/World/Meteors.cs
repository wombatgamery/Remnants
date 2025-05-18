using Microsoft.Xna.Framework;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Walls.Vanity;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Remnants.Content.World.BiomeGeneration;
using Remnants.Content.Tiles.Plants;
using System.Reflection;
using Terraria.Chat;
using Terraria.Localization;

namespace Remnants.Content.World
{
    public class MeteorWorld : ModSystem
    {
        public override void Load()
        {
            On_WorldGen.dropMeteor += Meteor;
        }

        private void Meteor(On_WorldGen.orig_dropMeteor orig)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            int blocks = 0;
            for (int y = 40; y < Main.worldSurface; y++)
            {
                for (int x = 40; x <= Main.maxTilesX - 40; x++)
                {
                    if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Meteorite)
                    {
                        blocks++;

                        if (blocks >= Main.maxTilesX / 1050)
                        {
                            return;
                        }
                    }
                }
            }

            bool success = false;

            while (!success)
            {
                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, (int)(Main.maxTilesX * 0.45f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.55f), Main.maxTilesX - 400);
                int y = (int)(Main.worldSurface * 0.4f);

                while (!IgnoredByMeteor(x, y))
                {
                    y++;
                }

                bool falling = true;

                while (falling)
                {
                    y++;

                    falling = false;

                    for (int j = y - 5; j <= y + 5; j++)
                    {
                        for (int i = x - 5; i <= x + 5; i++)
                        {
                            if (IgnoredByMeteor(i, j))
                            {
                                falling = true;
                            }
                        }
                    }
                }

                if (y < Main.worldSurface)
                {
                    bool safe = true;
                    int radius = 25;

                    for (int j = y - radius * 2; j <= y + radius; j++)
                    {
                        for (int i = x - radius * 2; i <= x + radius * 2; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (TileID.Sets.BasicChest[tile.TileType] || TileID.Sets.AvoidedByMeteorLanding[tile.TileType])
                            {
                                safe = false;
                            }
                            else if (tile.TileType == TileID.Meteorite || Main.tileDungeon[tile.TileType] || tile.TileType == TileID.LivingWood || tile.TileType == TileID.LeafBlock)
                            {
                                safe = false;
                            }
                        }
                    }

                    if (safe)
                    {
                        typeof(WorldGen).GetField("stopDrops", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, true);

                        FastNoiseLite shaping = new FastNoiseLite();
                        shaping.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        shaping.SetFrequency(0.15f);
                        shaping.SetFractalType(FastNoiseLite.FractalType.None);

                        for (int j = y - radius * 2; j <= y; j++)
                        {
                            for (int i = x - radius; i <= x + radius; i++)
                            {
                                float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y - radius)) + shaping.GetNoise(i, j) * 10 + 10;

                                Tile tile = Main.tile[i, j];

                                if (distance < radius * 0.9f)
                                {
                                    WorldGen.KillTile(i, j, noItem: true);

                                    if (distance < radius * 0.8f)
                                    {
                                        tile.WallType = 0;
                                    }
                                    else if (tile.WallType != 0)
                                    {
                                        tile.WallType = WallID.LavaUnsafe1;
                                    }
                                }
                                else if (distance < radius)
                                {
                                    if (tile.HasTile)
                                    {
                                        if (Main.tileCut[tile.TileType])
                                        {
                                            WorldGen.KillTile(i, j, noItem: true);
                                        }
                                        else if (Main.tileSolid[tile.TileType])
                                        {
                                            SlopeType slope = tile.Slope;
                                            WorldGen.KillTile(i, j, noItem: true);
                                            WorldGen.PlaceTile(i, j, TileID.Ash, true);
                                            tile.Slope = slope;
                                        }
                                    }
                                }

                                WorldGen.SquareTileFrame(i, j);
                            }
                        }

                        FastNoiseLite lava = new FastNoiseLite();
                        lava.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        lava.SetFrequency(0.1f);
                        lava.SetFractalType(FastNoiseLite.FractalType.None);
                        lava.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);

                        for (int j = y - radius; j <= y + radius; j++)
                        {
                            for (int i = x - radius; i <= x + radius; i++)
                            {
                                float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + shaping.GetNoise(i, j) * 10 + 10;

                                Tile tile = Main.tile[i, j];

                                if (tile.HasTile)
                                {
                                    if (tile.TileType == ModContent.TileType<Nightglow>())
                                    {
                                        tile.HasTile = false;
                                    }
                                    else if (TileID.Sets.GetsDestroyedForMeteors[tile.TileType])
                                    {
                                        WorldGen.KillTile(i, j, noItem: true);
                                        tile.HasTile = false;
                                    }
                                }
                                else if (tile.LiquidAmount > 0)
                                {
                                    Main.tile[i + Main.rand.Next(-radius * 2, radius * 2 + 1), j - Main.rand.Next(radius, radius * 2)].LiquidAmount = tile.LiquidAmount;
                                    tile.LiquidAmount = 0;
                                }

                                if (distance < radius / 1.5f)
                                {
                                    WorldGen.KillTile(i, j, noItem: true);
                                    tile.LiquidType = LiquidID.Lava;

                                    if (distance < radius / 2f && lava.GetNoise(i, j) > -0.3f)
                                    {
                                        tile.LiquidAmount = 255;
                                    }
                                    else WorldGen.PlaceTile(i, j, TileID.Meteorite, true);

                                    if (tile.WallType != 0 || distance < radius / 1.75f)
                                    {
                                        tile.WallType = 0;
                                        WorldGen.PlaceWall(i, j, WallID.LavaUnsafe2);
                                    }
                                }
                                else if (distance < radius)
                                {
                                    if (tile.HasTile)
                                    {
                                        if (Main.tileCut[tile.TileType])
                                        {
                                            WorldGen.KillTile(i, j, noItem: true);
                                        }
                                        else if (Main.tileSolid[tile.TileType])
                                        {
                                            SlopeType slope = tile.Slope;
                                            WorldGen.KillTile(i, j, noItem: true);
                                            WorldGen.PlaceTile(i, j, TileID.Ash, true);
                                            tile.Slope = slope;
                                        }
                                    }

                                    if (tile.WallType != 0)
                                    {
                                        tile.WallType = WallID.LavaUnsafe1;
                                    }
                                }

                                WorldGen.SquareTileFrame(i, j);
                            }
                        }

                        for (int j = y - radius; j <= y + radius; j++)
                        {
                            for (int i = x - radius; i <= x + radius; i++)
                            {
                                Tile tile = Main.tile[i, j];

                                if (tile.HasTile && tile.TileType == TileID.Meteorite && Main.rand.NextBool(2))
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

                        NetMessage.SendTileSquare(-1, x, y, radius * 2);

                        typeof(WorldGen).GetField("stopDrops", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false);

                        success = true;
                    }
                }
            }

            if (Main.netMode == 0)
            {
                Main.NewText(Lang.gen[59].Value, 50, byte.MaxValue, 130);
            }
            else if (Main.netMode == 2)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.gen[59].Key), new Color(50, 255, 130));
            }

            return;
        }

        private bool IgnoredByMeteor(int i, int j)
        {
            if (!Main.tile[i, j].HasTile || !Main.tileSolid[Main.tile[i, j].TileType] || TileID.Sets.Platforms[Main.tile[i, j].TileType])
            {
                return true;
            }
            if (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == ModContent.TileType<StarOre>())
            {
                return true;
            }
            return false;
        }
    }
}