using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Dusts.Environment;
using Remnants.Content.Biomes;
using Remnants.Content.Walls;
using Remnants.Content.World;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Plants;
using Remnants.Content.Tiles.Objects.Furniture;
using Remnants.Content.Tiles.Objects.Hazards;
using Terraria.Audio;
using Remnants.Content.Dusts;

namespace Remnants.Content.Tiles
{
    public class RemTile : GlobalTile
    {
        public override void SetStaticDefaults()
        {
            //Main.tileBlendAll[TileID.Coralstone] = true;
            Main.tileBlockLight[TileID.Coralstone] = true;

            TileID.Sets.CanBeClearedDuringGeneration[TileID.WoodBlock] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.GrayBrick] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.BrownMossBrick] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.IridescentBrick] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.ObsidianBrick] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.HellstoneBrick] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.Glass] = false;

            WallID.Sets.AllowsPlantsToGrow[WallID.LivingWoodUnsafe] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.SnowWallUnsafe] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.IceUnsafe] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.JungleUnsafe3] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.CaveUnsafe] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.EbonstoneUnsafe] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.CorruptionUnsafe1] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.CorruptionUnsafe4] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.CrimsonUnsafe2] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.CrimsonUnsafe4] = true;
            WallID.Sets.AllowsPlantsToGrow[WallID.LavaUnsafe4] = true;
        }

        public override bool CanPlace(int i, int j, int type)
        {
            if (IsBackgroundWall(i, j) && !AdjacentTiles(i, j, TileID.Sets.Platforms[type], TileID.Sets.Torch[type]))
            {
                if (Main.tileSolid[type] || TileID.Sets.Torch[type])
                {
                    return false;
                }
            }
            return base.CanPlace(i, j, type);
        }

        public override bool KillSound(int i, int j, int tile, bool fail)
        {
            if (tile == TileID.ShimmerBlock)
            {
                SoundEngine.PlaySound(SoundID.Shatter);
            }
            return base.KillSound(i, j, tile, fail);
        }

        private bool AdjacentTiles(int i, int j, bool diagonal, bool torch)
        {
            if (diagonal)
            {
                if (Main.tile[i - 1, j - 1].HasTile || Main.tile[i + 1, j - 1].HasTile || Main.tile[i + 1, j + 1].HasTile || Main.tile[i - 1, j + 1].HasTile)
                {
                    return true;
                }
            }

            if (Main.tile[i - 1, j].HasTile && (Main.tileSolid[Main.tile[i - 1, j].TileType] && !Main.tileSolidTop[Main.tile[i - 1, j].TileType] || !torch) || Main.tile[i, j - 1].HasTile && !torch || Main.tile[i + 1, j].HasTile && (Main.tileSolid[Main.tile[i + 1, j].TileType] && !Main.tileSolidTop[Main.tile[i + 1, j].TileType] || !torch) || Main.tile[i, j + 1].HasTile && (Main.tileSolid[Main.tile[i, j + 1].TileType] || !torch))
            {
                return true;
            }
            return false;
        }

        private bool IsBackgroundWall(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.WallType == ModContent.WallType<undergrowth>() || tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<stronghold>() || tile.WallType == ModContent.WallType<magicallab>() || tile.WallType == ModContent.WallType<hive>() || tile.WallType == ModContent.WallType<dungeonblue>() || tile.WallType == ModContent.WallType<dungeongreen>() || tile.WallType == ModContent.WallType<dungeonpink>() || tile.WallType == ModContent.WallType<temple>() || tile.WallType == ModContent.WallType<whisperingmaze>();
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (IsBackgroundWall(i, j) && TileID.Sets.Torch[type])
            {
                if (!AdjacentTiles(i, j, false, true))
                {
                    WorldGen.KillTile(i, j);
                    return false;
                }
            }
            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType == ModContent.TileType<LockedIronDoor>())
            {
                fail = true;
            }
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick)
            {
                if (!Main.hardMode)
                {
                    return false;
                }
            }
            else if (WorldGen.gen)
            {
                if (type == TileID.LivingWood || type == TileID.Sunplate || type == TileID.Glass)
                {
                    return false;
                }
            }

            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced)
        {
            if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick)
            {
                if (!Main.hardMode)
                {
                    return false;
                }
            }

            return base.CanReplace(i, j, type, tileTypeBeingPlaced);
        }

        public override bool Slope(int i, int j, int type)
        {
            if (Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType == ModContent.TileType<LockedIronDoor>() || Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].TileType == ModContent.TileType<LockedIronDoor>())
            {
                return false;
            }
            return true;
        }

        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (tile.WallType == ModContent.WallType<dungeonblue>() || tile.WallType == ModContent.WallType<dungeongreen>() || tile.WallType == ModContent.WallType<dungeonpink>() || tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<temple>())
            {
                if (tile.TileType == TileID.Painting3X3 || tile.TileType == TileID.Painting6X4 || tile.TileType == TileID.Painting4X3)
                {
                    int yOffset = (int)(Math.Sin(Main.GameUpdateCount * 0.03f) * 8);
                    if (tile.TileType == TileID.Painting6X4)
                    {
                        yOffset *= -1;
                    }

                    Texture2D texture = Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);// ModContent.Request<Texture2D>("Terraria/Images/Tiles_" + tile.TileType).Value;
                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
                    Rectangle frame = new(tile.TileFrameX, tile.TileFrameY, 16, 16);

                    Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + yOffset) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);

                    if (!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != tile.TileType)
                    {
                        if (Main.rand.NextBool(1))
                        {
                            int dustType = tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<temple>() ? DustID.YellowTorch : DustID.BoneTorch;
                            float dustScale = dustType == DustID.YellowTorch ? 1f : 0.5f;

                            Dust dust = Dust.NewDustDirect(new Vector2(i * 16, (j + 0.5f) * 16 + yOffset), 16, 0, dustType, Scale: dustScale);
                            dust.velocity *= 0.5f;
                            dust.velocity.Y += 2f;
                            dust.noGravity = true;
                        }
                    }
                    return false;
                }
            }

            return true;
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            //if (!Framing.GetTileSafely(i, j - 1).HasTile && RemTile.SolidTop(i, j))
            //{
            //    Tile tile = Main.tile[i, j];
            //    if (j < Main.worldSurface && (tile.TileType == TileID.Grass || tile.TileType == TileID.JungleGrass) && Main.rand.NextBool(100))
            //    {
            //        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<nightglow>(), true, style: Main.rand.Next(3));
            //    }
            //}
            //if (Main.rand.NextBool(1000))
            //         {
            //	if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].LiquidAmount <= 32 && i >= 95 && i <= Main.maxTilesX - 95 && j >= 95 && j <= Main.maxTilesY - 95)
            //             {
            //		if (j < Main.worldSurface && Main.tile[i, j].TileType == TileID.SnowBlock)
            //		{
            //			WorldGen.PlaceTile(i, j - 1, TileID.DyePlants, mute: true, style: 4);
            //		}
            //	}
            //}

            //if (Framing.GetTileSafely(i, j).LiquidAmount == 255 && Framing.GetTileSafely(i, j).LiquidAmountType() == 1)
            //{
            //    if (Framing.GetTileSafely(i, j).wall == WallID.GrassUnsafe || Framing.GetTileSafely(i, j).wall == WallID.FlowerUnsafe || Framing.GetTileSafely(i, j).wall == WallID.CorruptGrassUnsafe || Framing.GetTileSafely(i, j).wall == WallID.CrimsonGrassUnsafe || Framing.GetTileSafely(i, j).wall == WallID.HallowedGrassUnsafe)
            //    {
            //        Framing.GetTileSafely(i, j).wall = WallID.DirtUnsafe;
            //    }
            //    if (Framing.GetTileSafely(i, j).wall == WallID.JungleUnsafe)
            //    {
            //        Framing.GetTileSafely(i, j).wall = WallID.MudUnsafe;
            //    }
            //}
        }

        public static bool SolidTop(int i, int j)
        {
            if (!Main.tile[i, j].HasTile || !Main.tileSolid[Main.tile[i, j].TileType] && !Main.tileSolidTop[Main.tile[i, j].TileType])
            {
                return false;
            }
            else if (Main.tile[i, j].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j].Slope == SlopeType.SlopeDownRight || Main.tile[i, j].IsHalfBlock)
            {
                return false;
            }
            return true;
        }
        public static bool SolidBottom(int i, int j)
        {
            if (!Main.tile[i, j].HasTile || !Main.tileSolid[Main.tile[i, j].TileType])
            {
                return false;
            }
            else if (Main.tile[i, j].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j].Slope == SlopeType.SlopeUpRight)
            {
                return false;
            }
            return true;
        }
        public static bool SolidLeft(int i, int j)
        {
            if (!Main.tile[i, j].HasTile || !Main.tileSolid[Main.tile[i, j].TileType])
            {
                return false;
            }
            else if (Main.tile[i, j].Slope == SlopeType.SlopeDownRight || Main.tile[i, j].Slope == SlopeType.SlopeUpRight || Main.tile[i, j].IsHalfBlock)
            {
                return false;
            }
            return true;
        }
        public static bool SolidRight(int i, int j)
        {
            if (!Main.tile[i, j].HasTile || !Main.tileSolid[Main.tile[i, j].TileType])
            {
                return false;
            }
            else if (Main.tile[i, j].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j].IsHalfBlock)
            {
                return false;
            }
            return true;
        }

        public static void RGBLight(float r, float g, float b, Color color)
        {
            r = (float)(color.R / 255f);
            g = (float)(color.G / 255f);
            b = (float)(color.B / 255f);
        }

        public static int GetGemType(int j, bool random = true)
        {
            int low = (int)Main.rockLayer;
            int high = Main.maxTilesY - 300;

            int gemType = (int)(((float)j - low) / (high - low) * 5);

            if (j <= low)
            {
                gemType = 0;
            }
            else if (j >= high)
            {
                gemType = 4;
            }

            return gemType + (random ? WorldGen.genRand.Next(2) : 0);
        }

        public static int FlowerGetLength(int i, int j)
        {
            int length = 0;
            for (int a = 0; !MiscTools.Solid(i, j + a) && Main.tile[i, j + a].LiquidAmount != 255; a++)
            {
                if (Main.tile[i, j + a].TileType == ModContent.TileType<PrismbudStem>() || Main.tile[i, j + a].TileType == ModContent.TileType<PrismbudHead>())
                {
                    length++;
                }
            }

            return length;
        }

        public static Color GoldenCityLightColour(int i, int j, bool light = false, bool red = false)
        {
            float r = 255;
            float g = 205 + (int)(Math.Sin((Main.GameUpdateCount - i) * 0.1f) * 50);
            float b = 100;
            if (red)
            {
                g /= 2f;
            }
            if (light)
            {
                r /= 2f;
                g /= 4f;
                b /= 4f;
            }
            return new Color((int)r, (int)g, (int)b);
        }

        public static Color MagicalLabLightColour(int j, bool light = false)
        {
            float frequency = MathHelper.TwoPi / 200;

            float r = 200 + (int)(Math.Sin(frequency * Main.GameUpdateCount + j / 3f) * 55);
            float g = 200 + (int)(Math.Sin(frequency * Main.GameUpdateCount + j / 3f + 2) * 55);
            float b = 200 + (int)(Math.Sin(frequency * Main.GameUpdateCount + j / 3f + 4) * 55);
            if (light)
            {
                r /= 1.5f;
                g /= 2f;
                b /= 2f;
            }
            return new Color((int)r, (int)g, (int)b);
        }

        public static void SmallPile(int x, int y, int style)
        {
            if (Framing.GetTileSafely(x, y + 1).HasTile && Main.tileSolid[Framing.GetTileSafely(x, y + 1).TileType])
            {
                if (!Framing.GetTileSafely(x, y).HasTile)
                {
                    WorldGen.PlaceTile(x, y, TileID.SmallPiles);
                    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                }
            }
        }

        public static void Stalactite(int x, int y, int style)
        {
            if (Framing.GetTileSafely(x, y - 1).HasTile && Main.tileSolid[Framing.GetTileSafely(x, y - 1).TileType])
            {
                if (!Framing.GetTileSafely(x, y).HasTile && !Framing.GetTileSafely(x, y + 1).HasTile)
                {
                    Framing.GetTileSafely(x, y).HasTile = true;
                    Framing.GetTileSafely(x, y + 1).HasTile = true;
                    Framing.GetTileSafely(x, y).TileType = TileID.Stalactite;
                    Framing.GetTileSafely(x, y + 1).TileType = TileID.Stalactite;

                    Framing.GetTileSafely(x, y).TileFrameX = (short)(style * 18);
                    Framing.GetTileSafely(x, y + 1).TileFrameX = (short)(style * 18);
                    Framing.GetTileSafely(x, y).TileFrameY = 0;
                    Framing.GetTileSafely(x, y + 1).TileFrameY = 18;
                }
            }
        }
    }

    public class TileBlend : GlobalTile
    {
        public override void SetStaticDefaults()
        {
            //Main.tileBlendAll[TileID.ClayBlock] = true;
            //Main.tileBlendAll[TileID.Mud] = true;

            Main.tileBlendAll[TileID.Grass] = true;
            Main.tileBlendAll[TileID.CorruptGrass] = true;
            Main.tileBlendAll[TileID.CrimsonGrass] = true;
            Main.tileBlendAll[TileID.JungleGrass] = true;
            Main.tileBlendAll[TileID.MushroomGrass] = true;
            Main.tileBlendAll[TileID.AshGrass] = true;

            //TileMerge(TileID.Grass, TileID.ClayBlock);

            TileMerge(TileID.Vines, TileID.VineFlowers);

            //TileMerge(TileID.Sandstone, TileID.SmoothSandstone);

            //TileMerge(TileID.FossilOre, TileID.Sand);
            //TileMerge(TileID.FossilOre, TileID.HardenedSand);

            Main.tileMerge[TileID.Sandstone][TileID.Stalactite] = false;//true;
            Main.tileMerge[TileID.Granite][TileID.Stalactite] = false;//true;
            Main.tileMerge[TileID.Marble][TileID.Stalactite] = false;//true;

            TileMerge(TileID.Dirt, TileID.Ash);
            TileMerge(TileID.ClayBlock, TileID.Ash);

            TileMerge(TileID.Silt, TileID.Stone);
            TileMerge(TileID.Slush, TileID.IceBlock);

            TileMerge(TileID.Granite, TileID.GraniteBlock);

            TileMerge(TileID.HardenedSand, TileID.Dirt);
            TileMerge(TileID.HardenedSand, TileID.ClayBlock);
            TileMerge(TileID.HardenedSand, TileID.Stone);

            TileMerge(TileID.Sandstone, TileID.SmoothSandstone);

            TileMerge(TileID.Silt, TileID.Mud);
            TileMerge(TileID.Silt, TileID.Ash);
            TileMerge(TileID.Silt, TileID.Slush);

            TileMerge(TileID.Obsidian, TileID.Stone);
            TileMerge(TileID.Obsidian, TileID.Mud);
            TileMerge(TileID.Obsidian, TileID.Silt);
            TileMerge(TileID.Obsidian, TileID.Ash);
            TileMerge(TileID.Obsidian, TileID.Hellstone);
            TileMerge(TileID.Obsidian, TileID.ObsidianBrick);
            TileMerge(TileID.Obsidian, TileID.HellstoneBrick);

            TileMerge(TileID.Hellstone, TileID.Stone);
            TileMerge(TileID.Hellstone, TileID.Mud);
            TileMerge(TileID.Hellstone, TileID.Silt);
            TileMerge(TileID.Hellstone, TileID.ObsidianBrick);
            TileMerge(TileID.Hellstone, TileID.HellstoneBrick);

            for (int type = 0; type <= 300; type++)
            {
                if (Main.tileMergeDirt[type])
                {
                    Main.tileMerge[type][TileID.ClayBlock] = true;
                    Main.tileMerge[TileID.ClayBlock][type] = true;
                }
            }
        }

        private void TileMerge(ushort tile1, ushort tile2)
        {
            Main.tileMerge[tile1][tile2] = true;
            Main.tileMerge[tile2][tile1] = true;
        }
    }

    public class WaterVisuals : GlobalTile
    {
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.waterStyle == ModContent.Find<ModWaterStyle>("Remnants/GardenWater").Slot)
            {
                if (Main.rand.NextBool(50))
                {
                    Tile tile = Main.tile[i, j];

                    if (!WorldGen.SolidOrSlopedTile(i, j - 1) && Main.tile[i, j - 1].LiquidAmount > 0)
                    {
                        int height = 0;
                        while (true)
                        {
                            if (Main.tile[i, j - height - 2].LiquidAmount > 0)
                            {
                                height++;
                            }
                            else break;
                        }

                        if (!WorldGen.SolidOrSlopedTile(i, j - height - 2) && Lighting.Brightness(i, j - height - 1) > 0.75f)
                        {
                            Dust dust = Dust.NewDustPerfect(new Vector2((i + Main.rand.NextFloat(1)) * 16, (j - height) * 16), DustID.TreasureSparkle, Vector2.Zero);
                            dust.position.Y -= Main.tile[i, j - height - 1].LiquidAmount / 255f * 16f;
                        }
                    }
                }
            }
        }
    }

    public class TileVisuals : GlobalWall
    {
        public override void SetStaticDefaults()
        {

        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            #region passivelight
            if (tile.TileType == TileID.AlchemyTable)
            {
                if (tile.TileFrameY == 0 || tile.TileFrameX == 18 && tile.TileFrameY == 18 * 2)
                {
                    r = (float)(105f / 255f);
                    g = (float)(213f / 255f);
                    b = (float)(89f / 255f);
                }
                else if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
                {
                    r = (float)(181f / 255f);
                    g = (float)(138f / 255f);
                    b = (float)(126f / 255f);
                }

                r /= 2f;
                g /= 2f;
                b /= 2f;
            }
            else if ((!tile.HasTile || !Main.tileSolid[tile.TileType]) && tile.LiquidAmount > 0)
            {
                //if (Main.LocalPlayer.ZoneSkyHeight)
                //{
                //	float sin = (float)(Math.Sin(Main.GameUpdateCount * 0.02f - i / 2) + 1) / 2;

                //	r = (float)(190f / 255f) * (tile.LiquidAmount / 255f) * (1 - sin / 5);
                //	g = (float)(195f / 255f) * (tile.LiquidAmount / 255f) * (1 - sin / 5);
                //	b = (float)(255f / 255f) * (tile.LiquidAmount / 255f);
                //}
                if (Main.LocalPlayer.InModBiome<GraniteCave>())
                {
                    float sin = (float)(Math.Sin(Main.GameUpdateCount * 0.02f - i / 2) + 1) / 2;

                    r = (float)(32f / 255f) * (tile.LiquidAmount / 255f) * (1 - sin / 2);
                    g = (float)(62f / 255f) * (tile.LiquidAmount / 255f) * (1 - sin / 2);
                    b = (float)(103f / 255f) * (tile.LiquidAmount / 255f) * (1 - sin / 4);
                }
            }
            #endregion
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileType == TileID.AlchemyTable && tile.TileFrameX == 0 && tile.TileFrameY == 18)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16 + 2), 8, 8, DustID.Torch, SpeedY: -100);
                dust.noGravity = true;
            }

            Player player = Main.LocalPlayer;
            if ((!tile.HasTile || !Main.tileSolid[tile.TileType]) && tile.LiquidAmount > 0)
            {
                if (player.InModBiome<GraniteCave>())
                {
                    if (Main.rand.NextBool(1000))
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, ModContent.DustType<granitespark>());
                        dust.velocity = Vector2.Zero;
                    }
                }
            }

            if (tile.WallType == WallID.SpiderUnsafe)
            {
                if (Main.rand.NextBool(500) && tile.HasTile && tile.TileType == TileID.Cobweb)
                {
                    Color color = Lighting.GetColor(i, j);
                    byte brightness = Math.Max(Math.Max(color.R, color.G), color.B);

                    if (brightness > 0 && brightness <= 17)
                    {
                        Dust.NewDust(new Vector2(i, j) * 16, 16, 16, ModContent.DustType<Spiderling>());
                    }
                }
            }
            else if (tile.WallType == ModContent.WallType<undergrowth>())
            {
                if (Main.rand.NextBool(5000) && !tile.HasTile && tile.LiquidAmount == 0)
                {
                    Dust.NewDust(new Vector2(i, j) * 16, 16, 16, ModContent.DustType<treefirefly>());
                }
            }
            else if (tile.WallType == ModContent.WallType<vault>())
            {
                if (Main.rand.NextBool(5000) && !tile.HasTile && tile.LiquidAmount == 0)
                {
                    Dust.NewDust(new Vector2(i, j) * 16, 16, 16, ModContent.DustType<hellfirefly>());
                }
            }
            else if (tile.WallType == ModContent.WallType<whisperingmaze>() || tile.WallType == ModContent.WallType<LabyrinthTileWall>() || tile.WallType == ModContent.WallType<LabyrinthBrickWall>())
            {
                if (Main.rand.NextBool(5000) && !tile.HasTile && tile.LiquidAmount == 0 && tile.TileType != ModContent.TileType<LabyrinthSpawner>())
                {
                    Dust.NewDust(new Vector2(i, j) * 16, 16, 16, ModContent.DustType<mazefirefly>());
                }
            }
        }
    }
}
