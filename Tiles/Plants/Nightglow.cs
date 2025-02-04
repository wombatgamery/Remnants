using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Remnants.Items.Consumable;
using Remnants.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Plants
{
	[LegacyName("nightglow")]
	public class Nightglow : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 24;
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.StyleWrapLimit = 6;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(198, 255, 74), CreateMapEntryName());
			DustType = DustID.Dirt;

			//HitSound = SoundID.Grass;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => Main.tile[i, j].TileFrameX < 72;

		public override void RandomUpdate(int i, int j)
        {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 16 && tile.TileFrameX >= 72 && Main.rand.NextBool(10))
            {
				int style = Main.rand.Next(3);
				Main.tile[i, j].TileFrameX = (short)(style * 24);
				Main.tile[i, j - 1].TileFrameX = (short)(style * 24);
			}
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
			yield return new Item(ModContent.ItemType<NightglowBerry>(), Main.rand.Next(1, 4));
        }

		public override bool CanDrop(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.TileFrameX < 72;
		}

		public override void MouseOver(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX < 72)
			{
				Player player = Main.LocalPlayer;

				player.noThrow = 2;
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = ModContent.ItemType<NightglowBerryIcon>();
			}
		}

		public override bool RightClick(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX < 72)
			{
				Main.mouseRightRelease = false;

				WorldGen.KillTile(i, j);

				return true;
			}
			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			if (!Main.dayTime)
            {
				Tile tile = Main.tile[i, j];
				if (tile.TileFrameX < 72)
				{
					r = (float)(97f / 255f);
					g = (float)(151f / 255f);
					b = (float)(53f / 255f);
				}
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
			if (!Main.dayTime)
            {
				Tile tile = Main.tile[i, j];
				if (tile.TileFrameX < 72)
                {
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

					Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X - 4, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 34, 24, 16), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
				}
			}
		}
	}
}