using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Shimmer
{
	public class Ascension : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(82, 89, 101));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
			Rectangle frame = new(16, (i + j) % 3 == 0 ? 16 : 0, 16, 16);
			if (Main.tile[i - 1, j].WallType != Type)
            {
				frame.X -= 16;
            }
			if (Main.tile[i + 1, j].WallType != Type)
			{
				frame.X += 16;
			}

			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, RemTile.MagicalLabLightColour(j), 0f, Vector2.Zero, 1f, 0, 0f);

			if (Main.rand.NextBool(25))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, DustID.TreasureSparkle);
				//dust.noGravity = true;
				dust.velocity = new Vector2(0, -5);
			}

			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];

			if (!tile.HasTile || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
			{
				Color color = RemTile.MagicalLabLightColour(j);

				r = color.R / 255f;
				g = color.G / 255f;
				b = color.B / 255f;
			}
		}
	}
}
