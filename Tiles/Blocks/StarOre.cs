using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Tiles.Blocks
{
	[LegacyName("starore")]
	public class StarOre : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileOreFinderPriority[Type] = 125;

			Main.tileMerge[Type][TileID.Cloud] = true; Main.tileMerge[TileID.Cloud][Type] = true;
			Main.tileMerge[Type][TileID.RainCloud] = true; Main.tileMerge[TileID.RainCloud][Type] = true;

			TileID.Sets.Ore[Type] = true;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			DustType = DustID.YellowStarDust;
			HitSound = SoundID.Item52;// Tink;

			AddMapEntry(new Color(251, 251, 91), CreateMapEntryName());
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = (float)(203f / 255f);
			g = (float)(123f / 255f);
			b = (float)(45f / 255f);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

			for (int y = -1; y <= 1; y++)
            {
				for (int x = -1; x <= 1; x++)
				{
					if ((x != 0 || y != 0) && CanMerge(i + x, j + y) > 0)
                    {
						Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Blocks/cloudmerge").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle((-x + 1) * 16 + (CanMerge(i + x, j + y) == 2 ? 48 : 0), (-y + 1) * 16, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
					}
				}
			}
		}

		public override bool HasWalkDust() => true;
		public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
			dustType = DustID.YellowStarDust;
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(100))
            {
				Dust dust = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, DustID.YellowStarDust);
				dust.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
            }
        }

        private int CanMerge(int i, int j)
        {
			if (Main.tile[i, j].HasTile)
            {
				if (Main.tile[i, j].TileType == TileID.Cloud)
                {
					return 1;
                }
				else if (Main.tile[i, j].TileType == TileID.RainCloud)
				{
					return 2;
				}
			}
			return 0;
		}
	}
}
