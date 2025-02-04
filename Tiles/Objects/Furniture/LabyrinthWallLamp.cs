using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Objects.Furniture
{
	[LegacyName("mazelamp")]
	public class LabyrinthWallLamp : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;

			TileID.Sets.FramesOnKillWall[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.CoordinateHeights = new int[1] { 22 };
			TileObjectData.newTile.CoordinateWidth = 24;
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.newTile.WaterDeath = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

			AddMapEntry(new Color(120, 242, 255), Language.GetText("MapObject.Lantern"));

			DustType = DustID.Iron;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 0)
			{
				float mult = Main.rand.NextFloat(1f, 1.1f);

				r = (float)(65f / 255f) * mult;
				g = (float)(103f / 255f) * mult;
				b = (float)(155f / 255f) * mult;
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Tile tile = Main.tile[i, j];
			Player player = Main.LocalPlayer;

			Vector2 position = new Vector2(i * 16 + 8, j * 16 + 8);

			if (!player.dead)
			{
				if (tile.TileFrameY == 24)
				{
					if (player.Center.X > position.X - 16 * 4 && player.Center.X < position.X + 16 * 4)
					{
						if (player.Center.Y > position.Y - 16 * 8 && player.Center.Y < position.Y + 16 * 8)
						{
							tile.TileFrameY = 0;
							if (Main.netMode == NetmodeID.MultiplayerClient)
							{
								NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1);
							}
							DustCloud(i, j);
						}
					}
				}
			}
			else
			{
				if (tile.TileFrameY == 0)
				{
					DustCloud(i, j);
					tile.TileFrameY = 24;
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1);
					}
				}
			}
		}

		private void DustCloud(int i, int j)
        {
			for (int k = 0; k < 10; k++)
			{
				int dustIndex = Dust.NewDust(new Vector2(i * 16, j * 16), 8, 8, ModContent.DustType<spiritenergy>());
				Main.dust[dustIndex].velocity = Vector2.Zero;
			}
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0 && tile.TileFrameY == 0 && Main.rand.NextBool(5))
			{
				int dustIndex = Dust.NewDust(new Vector2(i * 16, j * 16), 8, 8, ModContent.DustType<spiritenergy>());
				Main.dust[dustIndex].velocity = Vector2.Zero;
			}
		}

		public override void HitWire(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX < 24)
			{
				tile.TileFrameX = 24;
			}
			else tile.TileFrameX = 0;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 0)
            {
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen)
				{
					zero = Vector2.Zero;
				}
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + 26 + 4, tile.TileFrameY + 4, 14, 14), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}