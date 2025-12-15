using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Gores;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.World;
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
            TileID.Sets.IsBeam[Type] = true;

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
                float strength = (RemSystem.vaultExhaustIntensity + 1) / 2;
                if (Main.rand.NextFloat(1f) < strength)
                {
                    Dust dust = Dust.NewDustPerfect(new Vector2((i * 16) + Main.rand.NextFloat(7f, 9f), j * 16 + 8), DustID.Smoke, new Vector2(0f, Main.rand.NextFloat(0, -4) * strength), 255 - (int)(strength * 25), Scale: Main.rand.NextFloat(2, 4));
                    dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                if (Main.rand.NextFloat(1f) < strength)
                {
                    Gore gore = Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), new Vector2((i * 16) - 16 + Main.rand.NextFloat(7f, 9f), j * 16 - 8), new Vector2(0f, Main.rand.NextFloat(0, -4) * strength), Main.rand.Next(220, 222), Main.rand.NextFloat(0.5f, 1));
                    gore.alpha = 255 - (int)(strength * 50);
                    gore.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }

                if (Main.rand.NextBool(64))
                {
                    Vector2 position = new Vector2(i + 0.5f, j + 0.5f) * 16;
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 1f);
                    Gore gore = Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), position, velocity, ModContent.GoreType<ToxicFog>(), Main.rand.NextFloat(8, 16));
                    gore.position -= new Vector2(6f * gore.scale, 3f * gore.scale);
                }
            }
        }

        public override bool CanPlace(int i, int j)
        {
            if (!Main.tile[i, j + 1].HasTile)
            {
                return false;
            }
            else if (Main.tile[i, j + 1].TileType != ModContent.TileType<Sulfurstone>() && Main.tile[i, j + 1].TileType != ModContent.TileType<SulfurstoneDiamond>() && Main.tile[i, j + 1].TileType != Type)
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
			else if (Main.tile[i, j + 1].TileType != ModContent.TileType<Sulfurstone>() && Main.tile[i, j + 1].TileType != ModContent.TileType<SulfurstoneDiamond>() && Main.tile[i, j + 1].TileType != Type)
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

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}