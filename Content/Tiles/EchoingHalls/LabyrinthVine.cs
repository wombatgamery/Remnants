using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.EchoingHalls
{
	[LegacyName("mazevine")]
	public class LabyrinthVine : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;

            TileID.Sets.IsVine[Type] = true;
            TileID.Sets.ReplaceTileBreakDown[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			AddMapEntry(new Color(82, 144, 153));
            AddMapEntry(new Color(221, 51, 79));

            DustType = DustID.HallowedPlants;
			HitSound = SoundID.Grass;
		}

        //public override ushort GetMapOption(int i, int j)
        //{
        //    return (ushort)(Main.tile[i, j].TileFrameY == 36 ? 1 : 0);
        //}

        public override void RandomUpdate(int i, int j)
		{
			bool maxLength = true;

			if (!Main.tile[i, j + 1].HasTile && Main.rand.NextBool(10))
			{
				for (int a = 0; a < 10; a++)
				{
					if (Main.tile[i, j - a].TileType != ModContent.TileType<LabyrinthVine>())
					{
						maxLength = false;
						break;
					}
				}

				if (!maxLength)
				{
					WorldGen.PlaceTile(i, j + 1, ModContent.TileType<LabyrinthVine>(), true);
					Main.tile[i, j + 1].TileFrameX = (short)(Main.rand.Next(3) * 18);
				}
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			bool anchor = true;
			if (!Main.tile[i, j - 1].HasTile || !RemTile.SolidBottom(i, j - 1) && Main.tile[i, j - 1].TileType != ModContent.TileType<LabyrinthVine>())
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
			}
			else
			{
				if (!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != ModContent.TileType<LabyrinthVine>())
				{
					tile.TileFrameY = 36;
				}
				else if (Main.tile[i, j - 1].HasTile && Main.tileSolid[Main.tile[i, j - 1].TileType])
				{
					tile.TileFrameY = 0;
				}
				else tile.TileFrameY = 18;
			}

			return false;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
	}
}