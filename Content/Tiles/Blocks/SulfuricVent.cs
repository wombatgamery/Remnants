using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles.Blocks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	public class SulfuricVent : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = false;

			TileID.Sets.DisableSmartCursor[Type] = true;

			AddMapEntry(new Color(100, 100, 93));
			DustType = DustID.Stone;

            VanillaFallbackOnModDeletion = TileID.JungleThorns;
        }

        public override bool CanDrop(int i, int j)
        {
			return false;
        }

        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
			if (!Main.tile[i, j - 1].HasTile || Main.tile[i, j - 1].TileType != Type)
			{
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(new Vector2((i * 16) + Main.rand.NextFloat(7f, 9f), j * 16 + 8), DustID.Smoke, new Vector2(0f, Main.rand.NextFloat(0, -3)), 204, Scale: Main.rand.NextFloat(2, 4));
                    dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                if (Main.rand.NextBool(2))
                {
                    Gore gore = Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), new Vector2((i * 16) - 16 + Main.rand.NextFloat(7f, 9f), j * 16 - 8), new Vector2(0f, Main.rand.NextFloat(0, -3)), Main.rand.Next(220, 222), Main.rand.NextFloat(1, 2));
                    gore.alpha = 229;
                    gore.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }

                if (Main.rand.NextBool(64))
                {
                    Vector2 position = new Vector2(i + 0.5f, j) * 16;
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 1f);
                    Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), position, velocity, 1202, Main.rand.NextFloat(8, 16));
                }
            }
        }

        public override bool CanPlace(int i, int j)
        {
            if (!Main.tile[i, j + 1].HasTile)
            {
                return false;
            }
            else if (Main.tile[i, j + 1].TileType != ModContent.TileType<Sulfurstone>() && Main.tile[i, j + 1].TileType != Type)
            {
                return false;
            }
            else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
            {
                return false;
            }

			return true;
        }

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			bool anchor = true;

			if (!Main.tile[i, j + 1].HasTile)
			{
				anchor = false;
			}
			else if (Main.tile[i, j + 1].TileType != ModContent.TileType<Sulfurstone>() && Main.tile[i, j + 1].TileType != Type)
			{
				anchor = false;
			}
			else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
				return false;
			}

			return true;
		}
	}
}