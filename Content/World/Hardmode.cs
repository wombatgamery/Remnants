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
using static Remnants.Content.World.PrimaryBiomes;

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
}