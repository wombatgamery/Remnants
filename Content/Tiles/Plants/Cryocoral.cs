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
	[LegacyName("endocoral")]
	public class Cryocoral : ModTile
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

			AddMapEntry(new Color(146, 255, 204), CreateMapEntryName());
            AddMapEntry(new Color(139, 254, 255), CreateMapEntryName());
            AddMapEntry(new Color(255, 159, 255), CreateMapEntryName());
            DustType = DustID.Stone;
			//HitSound = SoundID.Grass;
		}

        public override ushort GetMapOption(int i, int j)
        {
			return (ushort)(Main.tile[i, j].TileFrameX <= 18 * 2 ? 0 : Main.tile[i, j].TileFrameX >= 18 * 6 ? 2 : 1);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX < 18 * 3)
            {
				r = (float)(75f / 255f);
				g = (float)(179f / 255f);
				b = (float)(189f / 255f);
			}
			else if (tile.TileFrameX < 18 * 6)
			{
				r = (float)(87f / 255f);
				g = (float)(125f / 255f);
				b = (float)(186f / 255f);
			}
			else
			{
				r = (float)(152f / 255f);
				g = (float)(88f / 255f);
				b = (float)(192f / 255f);
			}
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

			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - 4 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 22), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
		}
	}
}