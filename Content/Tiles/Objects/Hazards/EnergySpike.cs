using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects.Hazards
{
    public class EnergySpike : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileID.Sets.TouchDamageImmediate[Type] = 100;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			DustType = DustID.Gold;

			AddMapEntry(new Color(255, 68, 40));
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Color color = RemTile.GoldenCityLightColour(i, j, true, true);

			r = (float)(color.R / 255f);
			g = (float)(color.G / 255f);
			b = (float)(color.B / 255f);
		}

		public override void NearbyEffects(int i, int j, bool closer)
        {
			//Player player = Main.LocalPlayer;
			//Rectangle hitbox = new Rectangle(i * 16, j * 16, 16, 16);
			//if (player.getRect().Intersects(hitbox))
   //         {
			//	player.Hurt(PlayerDeathReason.ByOther(), 100, 0, knockback: 0, dodgeable: 0);
   //         }
        }

  //      public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		//{
		//	Tile tile = Main.tile[i, j];
		//	if (tile.TileFrameY == 18)
  //          {
				
		//	}
		//}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 18)
            {
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen)
				{
					zero = Vector2.Zero;
				}

				int frame = ((int)(Main.GameUpdateCount * 0.2f) + i) % 6;

				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/Hazards/energyspikevisual1").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y - 16) + zero, new Rectangle(frame * 14, 0, 14, 24), RemTile.GoldenCityLightColour(i, j, red: true), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/Hazards/energyspikevisual2").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y - 16) + zero, new Rectangle(frame * 14, 0, 14, 24), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}