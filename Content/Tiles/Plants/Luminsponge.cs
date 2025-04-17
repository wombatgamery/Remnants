using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Plants
{
	[LegacyName("lumensponge")]
	public class Luminsponge : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 22 };
			//TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 9;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(255, 228, 74), CreateMapEntryName());
			DustType = DustID.Stone;
			//HitSound = SoundID.Grass;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX < 18 * 4)
            {
				//r = (float)(255f / 255f);
				g = (float)(119f / 255f);
				//b = (float)(43f / 255f);
			}
			else if (tile.TileFrameX < 18 * 8)
			{
				//r = (float)(255f / 255f);
				g = (float)(151f / 255f);
				//b = (float)(43f / 255f);
			}
			else
			{
				//r = (float)(255f / 255f);
				g = (float)(172f / 255f);
				//b = (float)(43f / 255f);
			}
			r = (float)(255f / 255f);
			b = (float)(43f / 255f);
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
			int height = tile.TileFrameY == 36 ? 18 : 16;
			SpriteEffects effect = SpriteEffects.None;
			if (i % 2 == 1)
			{
				effect = SpriteEffects.FlipHorizontally;
			}

			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - 4 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 24, 16, 22), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
		}
	}
}