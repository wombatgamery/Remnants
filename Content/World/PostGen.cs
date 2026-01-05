using Microsoft.Xna.Framework;
using Remnants.Content.Tiles.Forest;
using Remnants.Content.Tiles.Space;
using Remnants.Content.Walls.Tomb;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Remnants.Content.World
{
    public class HardmodeWorld : ModSystem
    {
        public override void ModifyHardmodeTasks(List<GenPass> list)
        {
            RemWorld.RemovePass(list, RemWorld.FindIndex(list, "Hardmode Good Remix"));

            RemWorld.InsertPass(list, new Blessing("Blessing", 0), RemWorld.FindIndex(list, "Hardmode Good"), true);
            RemWorld.InsertPass(list, new Infection("Infection", 0), RemWorld.FindIndex(list, "Hardmode Evil"), true);

            RemWorld.RemovePass(list, RemWorld.FindIndex(list, "Hardmode Walls"));

            RemWorld.RemovePass(list, RemWorld.FindIndex(list, "PlentifulOres"));
        }
    }

    public class Blessing : GenPass
    {
        public Blessing(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            Rectangle area = new Rectangle((int)(Main.maxTilesX * 0.4f), (int)Main.worldSurface, (int)(Main.maxTilesX * 0.2f), Main.maxTilesY - 300 - (int)Main.worldSurface);

            int width = Main.maxTilesX / 14;
            int transition = Main.maxTilesX / 42;
            area.X += transition;
            area.Width -= transition * 2;

            int direction = Main.dungeonX > Main.maxTilesX / 2 ? -1 : 1;
            float center = direction == -1 ? area.Right - 1 : area.Left;

            FastNoiseLite borders = new FastNoiseLite();
            borders.SetFractalType(FastNoiseLite.FractalType.Ridged);
            borders.SetFrequency(0.0125f);

            for (int y = area.Bottom; y > 40; y--)
            {

                center += area.Width / (float)area.Height * direction;

                int left = (int)center - (width / 2 - Main.maxTilesX / 84);
                int right = (int)center + (width / 2 - Main.maxTilesX / 84);

                for (int x = left - transition; x < right + transition; x++)
                {
                    if (x > left && x < right || borders.GetNoise(x, y * 2) / 2 + 0.5f > MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, left, right)) / transition, 0, 1))
                    {
                        Tile tile = Main.tile[x, y];

                        if (!TileID.Sets.Conversion.JungleGrass[tile.TileType] && tile.WallType != WallID.JungleUnsafe && tile.WallType != ModContent.WallType<TombBrickWallUnsafe>() && tile.WallType != ModContent.WallType<forgottentomb>())
                        {
                            if (tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                            {
                                tile.TileType = TileID.Stone;
                            }
                            if (tile.WallType == WallID.RocksUnsafe1 || tile.WallType == WallID.Cave8Unsafe || tile.WallType == WallID.LavaUnsafe2 || tile.WallType == WallID.CaveUnsafe || tile.WallType == WallID.Cave2Unsafe || tile.WallType == WallID.Cave3Unsafe || tile.WallType == WallID.Cave4Unsafe || tile.WallType == WallID.Cave5Unsafe)
                            {
                                tile.WallType = WallID.PearlstoneBrickUnsafe;
                            }

                            WorldGen.Convert(x, y, BiomeConversionID.Hallow, 0);
                        }
                    }
                }
            }
        }
    }

    public class Infection : GenPass
    {
        public Infection(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int step = 5;

            int left = 40;
            int score = 0;
            for (; left < Main.maxTilesX - 40; left += step)
            {
                int infection = 0;

                for (int y = (int)Main.worldSurface; y < Main.maxTilesY - 300; y += 5)
                {
                    Tile tile = Main.tile[left, y];
                    if (tile.WallType == WallID.EbonstoneUnsafe || tile.WallType == WallID.CrimstoneUnsafe || TileID.Sets.Corrupt[tile.TileType] || TileID.Sets.Crimson[tile.TileType])
                    {
                        infection += 3;
                    }
                    else if (tile.TileType != TileID.Dirt)
                    {
                        infection--;
                    }
                }
                if (infection > 0)
                {
                    score++;
                }
                else score = 0;

                if (score >= 3)
                {
                    left -= score * step;
                    break;
                }
            }

            int right = Main.maxTilesX - 40;
            score = 0;
            for (; right > 40; right -= step)
            {
                int infection = 0;

                for (int y = (int)Main.worldSurface; y < Main.maxTilesY - 300; y += 5)
                {
                    Tile tile = Main.tile[right, y];
                    if (WallID.Sets.Corrupt[tile.WallType] || WallID.Sets.Crimson[tile.WallType] || TileID.Sets.Corrupt[tile.TileType] || TileID.Sets.Crimson[tile.TileType])
                    {
                        infection += 3;
                    }
                    else if (tile.TileType != TileID.Dirt)
                    {
                        infection--;
                    }
                }
                if (infection > 0)
                {
                    score++;
                }
                else score = 0;

                if (score >= 3)
                {
                    right += score * step;
                    break;
                }
            }

            if (right < left)
            {
                return;
            }

            int width = right - left;

            left -= width / 2 - Main.maxTilesX / 84;
            right += width / 2 - Main.maxTilesX / 84;

            FastNoiseLite borders = new FastNoiseLite();
            borders.SetFractalType(FastNoiseLite.FractalType.Ridged);
            borders.SetFrequency(0.0125f);

            FastNoiseLite overlaps = new FastNoiseLite();
            overlaps.SetFractalType(FastNoiseLite.FractalType.FBm);
            overlaps.SetFrequency(0.05f);

            int transition = Main.maxTilesX / 42;
            int threshold = (int)((Main.maxTilesY - 300 - Main.worldSurface) / 2 + Main.worldSurface);
            for (int y = (int)(Main.worldSurface * 0.5f); y < Main.maxTilesY - 250; y++)
            {
                for (int x = left - transition; x < right + transition; x++)
                {
                    if (x > left && x < right || borders.GetNoise(x, y * 2) / 2 + 0.5f > MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, left, right)) / transition, 0, 1))
                    {
                        Tile tile = Main.tile[x, y];
                        bool bottomEvil = y < threshold - 10 ? false : y > threshold + 10 ? true : overlaps.GetNoise(x, y * 2) / 2 + 0.5f < MathHelper.Clamp((y - threshold + 10) / 20f, 0, 1);

                        if (tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                        {
                            tile.TileType = TileID.Stone;
                        }
                        if (tile.WallType == WallID.RocksUnsafe1 || tile.WallType == WallID.Cave8Unsafe || tile.WallType == WallID.LavaUnsafe2 || tile.WallType == WallID.CaveUnsafe || tile.WallType == WallID.Cave2Unsafe || tile.WallType == WallID.Cave3Unsafe || tile.WallType == WallID.Cave4Unsafe || tile.WallType == WallID.Cave5Unsafe)
                        {
                            tile.WallType = WallID.PearlstoneBrickUnsafe;
                        }
                        WorldGen.Convert(x, y, bottomEvil && WorldGen.crimson || !bottomEvil && !WorldGen.crimson ? BiomeConversionID.Corruption : BiomeConversionID.Crimson, 0);

                        if (tile.WallType == WallID.IceUnsafe)
                        {
                            tile.WallColor = bottomEvil && WorldGen.crimson || !bottomEvil && !WorldGen.crimson ? PaintID.BluePaint : PaintID.RedPaint;
                        }
                    }
                }
            }
        }
    }

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