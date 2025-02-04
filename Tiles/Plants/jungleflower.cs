using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Dusts;

namespace Remnants.Tiles.Plants
{
    public class jungleflowerhead : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 28;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(0, 253, 255));

			DustType = ModContent.DustType<growthblood>();
		}

		//public override void RandomUpdate(int i, int j)
		//{
		//	if (Main.rand.NextBool(10)) // grow
		//	{
		//		if (RemTile.FlowerGetLength(i, j) < 10)
		//		{
		//			if (!Main.tile[i, j - 1].HasTile)
		//			{
		//				WGTools.Tile(i, j).HasTile = false;
		//				WorldGen.PlaceTile(i, j, ModContent.TileType<jungleflowerstem>(), true);

		//				WorldGen.PlaceTile(i, j - 1, ModContent.TileType<jungleflowerhead>(), true, style: Main.rand.Next(3));

		//				Framing.GetTileSafely(i, j).TileFrameX = (short)(Main.rand.Next(3) * 18);
		//				Framing.GetTileSafely(i, j - 1).TileFrameX = 0;
		//			}
		//		}
		//	}
		//}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			tile.TileFrameX = 0;

			bool anchor = true;

			if (!Main.tile[i, j + 1].HasTile)
			{
				anchor = false;
			}
			if (!Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].TileType != ModContent.TileType<jungleflowerstem>())
			{
				anchor = false;
			}
			if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
			}

			return false;
		}

		//public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		//{
		//	if (Main.rand.NextBool(125))
		//	{
		//		Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 4), 0, 0, ModContent.DustType<growthspore>());
		//	}
		//}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 0)
            {
				r = (float)(27f / 255f);
				g = (float)(123f / 255f);
				b = (float)(81f / 255f);
			}
			else if (tile.TileFrameY == 18)
			{
				r = (float)(46f / 255f);
				g = (float)(58f / 255f);
				b = (float)(173f / 255f);
			}
			else if (tile.TileFrameY == 36)
			{
				r = (float)(102f / 255f);
				g = (float)(30f / 255f);
				b = (float)(168f / 255f);
			}
			//r /= 2f;
			//g /= 2f;
			//b /= 2f;
		}

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			SpriteEffects effect = SpriteEffects.None;
			if (i % 2 == 1)
			{
				effect = SpriteEffects.FlipHorizontally;
			}
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - 6 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + 30, tile.TileFrameY, 28, 16), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
		}
	}

	public class jungleflowerstem : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			AddMapEntry(new Color(87, 55, 55));
			DustType = DustID.RichMahogany;
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			if (Main.rand.NextBool(10) && Main.tile[i, j].LiquidAmount == 0) // turn into flower
			{
				if (!Main.tile[i, j - 1].HasTile || (Main.tile[i, j - 1].TileType != ModContent.TileType<jungleflowerstem>() && Main.tile[i, j - 1].TileType != ModContent.TileType<jungleflowerhead>()))
				{
					Main.tile[i, j].TileType = (ushort)ModContent.TileType<jungleflowerhead>();
					Main.tile[i, j].TileFrameX = 0;
					Main.tile[i, j].TileFrameY = (short)(Main.rand.Next(3) * 18);
				}
			}
			else if (Main.rand.NextBool(10)) // grow
			{
				if (RemTile.FlowerGetLength(i, j) < 10)
				{
					if (!Main.tile[i, j - 1].HasTile)
					{
						WorldGen.PlaceTile(i, j - 1, ModContent.TileType<jungleflowerstem>(), true);
						Main.tile[i, j - 1].TileFrameX = (short)(Main.rand.Next(3) * 18);
					}
				}
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			bool anchor = true;

			if (!Main.tile[i, j + 1].HasTile)
			{
				anchor = false;
			}
			if (!Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].TileType != ModContent.TileType<jungleflowerstem>())
			{
				anchor = false;
			}
			if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
			}
			else
			{
				if (Main.tile[i, j - 1].TileType != ModContent.TileType<jungleflowerstem>() && Main.tile[i, j - 1].TileType != ModContent.TileType<jungleflowerhead>())
				{
					tile.TileFrameY = 0;
				}
				else if (Main.tile[i, j + 1].HasTile && Main.tileSolid[Main.tile[i, j + 1].TileType])
				{
					tile.TileFrameY = 36;
				}
				else
				{
					tile.TileFrameY = 18;
				}
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