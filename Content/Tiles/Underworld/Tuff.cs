using Microsoft.Xna.Framework;
using Remnants.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Underworld
{
	public class Tuff : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            DustType = DustID.Ash;
            HitSound = SoundID.Dig;

            //AddMapEntry(new Color(91, 76, 76));
            AddMapEntry(new Color(99, 85, 84));

            VanillaFallbackOnModDeletion = TileID.Ash;
        }

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Main.tile[i, j];
            int x = tile.TileFrameX / 18;
            int y = tile.TileFrameY / 18;

            if (MiscTools.HasTile(i, j + 1, ModContent.TileType<TuffStalac>()) || MiscTools.HasTile(i, j + 1, ModContent.TileType<TuffStalacSmall>()))
            {
                if (x >= 1 && x <= 3 && y == 2)
                {
                    x = Main.rand.Next(6, 9);
                    y = 2;
                }
                else if (x <= 5 && y == 4)
                {
                    if (x % 2 == 0)
                    {
                        x = 0;
                    }
                    else
                    {
                        x = 4;
                    }
                    y = Main.rand.Next(3);
                }
                else if (x >= 6 && x <= 8 && y == 4)
                {
                    x = 5;
                    y = Main.rand.Next(3);
                }
            }
            else if (MiscTools.HasTile(i, j - 1, ModContent.TileType<TuffStalac>()) || MiscTools.HasTile(i, j - 1, ModContent.TileType<TuffStalacSmall>()))
            {
                if (x >= 1 && x <= 3 && y == 0)
                {
                    x = Main.rand.Next(6, 9);
                    y = 1;
                }
                else if (x <= 5 && y == 3)
                {
                    if (x % 2 == 0)
                    {
                        x = 0;
                    }
                    else
                    {
                        x = 4;
                    }
                    y = Main.rand.Next(3);
                }
                else if (x >= 6 && x <= 8 && y == 0)
                {
                    x = 5;
                    y = Main.rand.Next(3);
                }
            }

            if (x != tile.TileFrameX / 18)
            {
                tile.TileFrameX = (short)(x * 18);
            }
            if (y != tile.TileFrameY / 18)
            {
                tile.TileFrameY = (short)(y * 18);
            }
        }
    }
}