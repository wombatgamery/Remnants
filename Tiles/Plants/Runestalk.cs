using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Buffs;
using Remnants.Dusts;
using Remnants.Items;
using Remnants.Items.Materials;
using Remnants.Items.Placeable.Objects;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Plants
{
    [LegacyName("gemstalk")]
	public class Runestalk : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 36;
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(228, 86, 21), CreateMapEntryName());
			DustType = DustID.Dirt;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 0)
            {
				Color color = GetColor(RemTile.GetGemType(j, false) + 1);

				r = color.R / 255f;
				g = color.G / 255f;
				b = color.B / 255f;
			}
			else
			{
				Color color = GetColor(RemTile.GetGemType(j, false));

				r = color.R / 255f;
				g = color.G / 255f;
				b = color.B / 255f;
			}

			r /= 3f;
			g /= 4f;
			b /= 3f;
		}

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];

            player.cursorItemIconID = ModContent.ItemType<RunestalkBlessingIcon>();
            player.cursorItemIconText = "";

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override bool RightClick(int i, int j)
        {
			Tile tile = Main.tile[i, j];
			Player player = Main.LocalPlayer;

			player.AddBuff(ModContent.BuffType<RunestalkBlessing>(), 1 * 60 * 60);
			NetMessage.SendData(MessageID.PlayerBuffs, number: player.whoAmI);

			int gemType = RemTile.GetGemType(j, false);
			for (int k = 0; k < 10; k++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(i * 16 - 4, j * 16 - 20 - tile.TileFrameY), 24, 40, gemType + Main.rand.Next(86, 88), Scale: Main.rand.NextFloat(1f, 2f));
				dust.noGravity = true;
				//dust.velocity *= 0.5f;
			}

			SoundEngine.PlaySound(SoundID.Item30, new Vector2(i * 16 + 8, j * 16 + 8));

			return true;
		}

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

		Color lightColor;

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];

			if (tile.TileFrameY == 0)
            {
				int gemType = RemTile.GetGemType(j, false);

				DrawCrystal(i, j, GetColor(gemType + 1), GetColor(gemType), i % 2 == 1);

				Dust dust;
				if (gemType >= 0)
                {
					dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16 - 12), 16, 32, gemType + 86, Scale: 0.5f);
					dust.noGravity = true;
					dust.velocity *= 0.5f;
					dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16 - 12), 16, 32, gemType + 87, Scale: 0.5f);
					dust.noGravity = true;
					dust.velocity *= 0.5f;

					dust = Dust.NewDustDirect(new Vector2(i * 16 + (i % 2 == 1 ? 4 : 2), j * 16 + 17), 8, 8, gemType + 86, Scale: 0.5f);
					dust.noGravity = true;
					dust.velocity *= 0.25f;
					dust = Dust.NewDustDirect(new Vector2(i * 16 + (i % 2 == 1 ? 4 : 2), j * 16 + 17), 8, 8, gemType + 87, Scale: 0.5f);
					dust.noGravity = true;
					dust.velocity *= 0.25f;
				}
			}
		}

		private void DrawCrystal(int i, int j, Color color1, Color color2, bool flip)
        {
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - 10 - (int)Main.screenPosition.Y + (int)(Math.Sin((Main.GameUpdateCount - i / 0.03f) * 0.03f) * 5)) + zero;

			if (flip)
			{
				position.X += 2;
			}

			Rectangle sourceRectangle = new Rectangle(0, 0, 14, 26);
			Color avgColor = new Color((color1.R + color2.R) / 2, (color1.G + color2.G) / 2, (color1.B + color2.B) / 2);
			lightColor = avgColor;

			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Plants/RunestalkCrystal").Value, position, sourceRectangle, avgColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Plants/RunestalkCrystalHighlight").Value, position, sourceRectangle, color1, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Plants/RunestalkCrystalBacklight").Value, position, sourceRectangle, color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Plants/RunestalkCrystalShine").Value, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		private Color GetColor(int gemType)
        {
			Color color = Color.White;

			if (gemType > 5)
			{
				gemType = 5;
			}

			switch (gemType)
            {
				case 0: color = new Color(204, 51, 255); break;

				case 1: color = new Color(255, 204, 51); break;

				case 2: color = new Color(51, 204, 255); break;

				case 3: color = new Color(51, 255, 102); break;

				case 4: color = new Color(255, 102, 102); break;

				case 5: color = new Color(153, 204, 255); break;
			}

			return color;
		}
	}
}