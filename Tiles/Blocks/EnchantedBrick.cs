using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Tiles.Plants;
using Remnants.Worldgen;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Tiles.Blocks
{
    [LegacyName("labtiles")]
	public class EnchantedBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileBlendAll[Type] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
			TileID.Sets.AvoidedByMeteorLanding[Type] = true;


            MinPick = 110;
			MineResist = 3;
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(136, 119, 149));
            AddMapEntry(new Color(255, 178, 248));
            //TileBlend(TileID.Stone);
            //TileBlend(TileID.Dirt);
            //TileBlend(TileID.Mud);
            //TileBlend(TileID.Silt);
            //TileBlend(TileID.Ebonstone);
            //TileBlend(TileID.Crimstone);
            //TileBlend(TileID.Pearlstone);
        }

        public override ushort GetMapOption(int i, int j)
        {
			return (ushort)(!WGTools.Solid(i, j - 1) ? 1 : 0);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
			if (!Main.tile[i, j - 1].HasTile || !Main.tileSolid[Main.tile[i, j - 1].TileType] || Main.tile[i, j - 1].TileType == TileID.ClosedDoor)
            {
				Color color = RemTile.MagicalLabLightColour(i);
				//RemTile.RGBLight(r, g, b, 112, 93, 133);
				r = color.R / 255f;// (241f / 255f) * mult;
                g = color.G / 255f;// (195f / 255f) * mult;
				b = color.B / 255f;// (233f / 255f) * mult;
			}
        }

        public override bool HasWalkDust() => true;
        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.TreasureSparkle;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			if (Main.tile[i - 1, j].HasTile && Main.tile[i - 1, j].TileType == Type && Main.tile[i + 1, j].HasTile && Main.tile[i + 1, j].TileType == Type)
            {
				if (Main.tile[i, j - 1].HasTile && Main.tileSolid[Main.tile[i, j - 1].TileType] && Main.tile[i, j + 1].HasTile && Main.tileSolid[Main.tile[i, j + 1].TileType])
                {
					bool top = false;
					bool bottom = false;
					if (!Main.tile[i - 1, j - 1].HasTile || !Main.tileSolid[Main.tile[i - 1, j - 1].TileType] || !Main.tile[i + 1, j - 1].HasTile || !Main.tileSolid[Main.tile[i + 1, j - 1].TileType])
					{
						top = true;
					}
					else if (!Main.tile[i - 1, j + 1].HasTile || !Main.tileSolid[Main.tile[i - 1, j + 1].TileType] || !Main.tile[i + 1, j + 1].HasTile || !Main.tileSolid[Main.tile[i + 1, j + 1].TileType])
					{
						bottom = true;
					}
					else return true;

					bool left = !Main.tile[i - 1, j + (bottom ? 1 : -1)].HasTile || !Main.tileSolid[Main.tile[i - 1, j + (bottom ? 1 : -1)].TileType];
					bool right = !Main.tile[i + 1, j + (bottom ? 1 : -1)].HasTile || !Main.tileSolid[Main.tile[i + 1, j + (bottom ? 1 : -1)].TileType];

					Tile tile = Main.tile[i, j];

					if (left && !right)
					{
						tile.TileFrameX = 10 * 18;
						tile.TileFrameY = (short)(bottom ? 18 : 0);// (short)(Main.rand.Next(3) * 18);
						return false;
					}
					else if (!left && right)
					{
						tile.TileFrameX = 11 * 18;
						tile.TileFrameY = (short)(bottom ? 18 : 0); // (short)(Main.rand.Next(3) * 18);
						return false;
					}
				}
			}
			return true;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (!Main.tile[i, j - 1].HasTile || !Main.tileSolid[Main.tile[i, j - 1].TileType] || Main.tile[i, j - 1].TileType == TileID.ClosedDoor)
			{
				Tile tile = Main.tile[i, j];
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Blocks/EnchantedBrickGlow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), RemTile.MagicalLabLightColour(i), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Blocks/EnchantedBrickShine").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		private void DrawCosmos(int i, int j)
        {
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			//Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/blocks/labtiles-layer1").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(0, 0, 16, 16), RemTile.MagicalLabLightColour(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			for (int k = 0; k < 8; k++)
            {
				Rectangle frame = new((i * 16 + (int)(Math.Sin(Main.GameUpdateCount / 256f - (j + k / 8f + Main.GameUpdateCount / 64f)) * 32)) % (4 * 16), (int)(j * 16 + k * 2 + Main.GameUpdateCount * 0.5f) % (4 * 16), 16, 2);

				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/blocks/cosmos").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 + k * 2 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			
			//frame = new((i * 16) % (20 * 16), (int)(j * 16 + Main.GameUpdateCount * 0.25f) % (20 * 16), 16, 16);

			//Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/blocks/labtiles-layer3").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, RemTile.MagicalLabLightColour(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;

        private void TileBlend(ushort tile)
		{
			Main.tileMerge[Type][tile] = true;
			Main.tileMerge[tile][Type] = true;
		}

		private bool CanDrawCosmos(int i, int j)
        {
			for (int y = j - 1; y <= j + 1; y++)
            {
				for (int x = i - 1; x <= i + 1; x++)
				{
					if (!Main.tile[x, y].HasTile || Main.tile[x, y].TileType != Type)
                    {
						return false;
                    }
				}
			}
			return true;
        }
    }
}
