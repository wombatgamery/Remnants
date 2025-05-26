using Microsoft.Xna.Framework;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Walls.dev;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Collections.Generic;
using Remnants.Content.Walls.Vanity;

namespace Remnants.Content.Items.Tools
{
    public class WandofRefinement : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 17 * 2;
            Item.height = 16 * 2;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 2;
            Item.useAnimation = 2;

            Item.autoReuse = true;
            Item.useTurn = true;

            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;

            Tile tile = Main.tile[x, y];

            return tile.WallType != 0;
        }

        public override bool? UseItem(Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;

            Tile tile = Main.tile[x, y];

            Main.NewText(tile.TileFrameX);
            Main.NewText(tile.TileFrameY);

            bool success = true;

            //if (tile.WallType == ModContent.WallType<PyramidBrickWallUnsafe>())
            //{
            //    tile.WallType = (ushort)ModContent.WallType<PyramidRuneWall>();
            //}
            if (tile.WallType == WallID.Wood)
            {
                tile.WallType = (ushort)ModContent.WallType<WoodSafe>();
            }
            else if (tile.WallType == WallID.GrayBrick)
            {
                tile.WallType = (ushort)ModContent.WallType<BrickStoneSafe>();
            }
            else if (tile.WallType == WallID.WoodenFence)
            {
                tile.WallType = (ushort)ModContent.WallType<WoodLattice>();
            }
            else if (tile.WallType == WallID.BorealWood)
            {
                tile.WallType = (ushort)ModContent.WallType<WoodBorealSafe>();
            }
            else if (tile.WallType == WallID.IceBrick)
            {
                tile.WallType = (ushort)ModContent.WallType<BrickIceSafe>();
            }
            else if (tile.WallType == WallID.RichMaogany)
            {
                tile.WallType = (ushort)ModContent.WallType<WoodMahoganySafe>();
            }
            else success = false;

            Dust dust;

            if (success)
            {
                NetMessage.SendTileSquare(-1, x, y);

                SoundEngine.PlaySound(SoundID.Item30, new Vector2(x + 0.5f, y + 0.5f) * 16);

                for (int k = 0; k < 8; k++)
                {
                    dust = Dust.NewDustPerfect(new Vector2(x + Main.rand.NextFloat(1), y + Main.rand.NextFloat(1)) * 16, DustID.ShimmerTorch, Scale: Main.rand.NextFloat(1, 2));
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Circular(2, 2);
                }
            }
            else if (Main.rand.NextBool(2))
            {
                dust = Dust.NewDustPerfect(new Vector2(x + Main.rand.NextFloat(1), y + Main.rand.NextFloat(1)) * 16, DustID.Smoke, Alpha: 153);
                dust.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
            }

            dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -34f : 16f), player.itemLocation.Y - 30f), 14, 14, DustID.ShimmerTorch, Scale: Main.rand.NextFloat(1, 2));
            dust.noGravity = true;
            dust.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
            dust.position = player.RotatedRelativePoint(dust.position);

            return null;
        }
    }
}