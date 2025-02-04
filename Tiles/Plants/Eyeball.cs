using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Tiles.Objects;
using Remnants.Dusts;
using Remnants.Projectiles;

namespace Remnants.Tiles.Plants
{
	public class Eyeball : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileCut[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = 5;
			HitSound = SoundID.NPCHit9;

			AddMapEntry(new Color(183, 146, 146), CreateMapEntryName());
			AddMapEntry(new Color(183, 159, 146), CreateMapEntryName());
		}

		public override ushort GetMapOption(int i, int j)
		{
			return (ushort)(Main.tile[i, j].TileFrameX >= 18 * 3 ? 1 : 0);
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			if (Main.tile[i, j].TileFrameX >= 18 * 3)
			{
				type = DustID.GreenBlood;
				return false;
			}
			return true;
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			if (!Main.tile[i, j + 1].HasTile && Main.rand.NextBool(10))
			{
				bool maxLength = true;

				for (int a = 0; a < 10; a++)
				{
					if (Main.tile[i, j - a].TileType != ModContent.TileType<EyeballVine>() && Main.tile[i, j - a].TileType != ModContent.TileType<Eyeball>())
					{
						maxLength = false;
						break;
					}
				}

				if (!maxLength)
				{
					tile.TileType = (ushort)ModContent.TileType<EyeballVine>();
					WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Eyeball>(), true);

					tile.TileFrameX = (short)((Main.rand.Next(3) + (tile.TileFrameX >= 18 * 3 ? 3 : 0)) * 18);
					Main.tile[i, j + 1].TileFrameX = (short)((Main.rand.Next(3) + (tile.TileFrameX >= 18 * 3 ? 3 : 0)) * 18);
				}
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			tile.TileFrameY = 0;

			bool anchor = true;
			if (!Main.tile[i, j - 1].HasTile || !RemTile.SolidBottom(i, j - 1) && Main.tile[i, j - 1].TileType != ModContent.TileType<EyeballVine>())
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
			}

			return false;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
			if (Main.tile[i, j].TileFrameX >= 18 * 3)
            {
				r = (float)(0f / 255f);
				g = (float)(127f / 255f);
				b = (float)(31f / 255f);
			}
			else
            {
                r = (float)(127f / 255f);
                g = (float)(74f / 255f);
                b = (float)(0f / 255f);
            }
		}

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			Projectile proj = Projectile.NewProjectileDirect(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16 + Vector2.One * 8, Vector2.Zero, ModContent.ProjectileType<FallingEyeball>(), 0, 0);
			proj.frame = Main.tile[i, j].TileFrameX / 18;
			NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
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
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 18, 16, 16), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
        }
    }

	public class EyeballVine : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileCut[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = DustID.Blood;
			HitSound = SoundID.NPCHit9;

			AddMapEntry(new Color(202, 50, 55));
			AddMapEntry(new Color(114, 114, 55));
		}

        public override ushort GetMapOption(int i, int j)
        {
			return (ushort)(Main.tile[i, j].TileFrameX >= 18 * 3 ? 1 : 0);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
			if (Main.tile[i, j].TileFrameX >= 18 * 3)
            {
				type = DustID.GreenBlood;
				return false;
			}
			return true;
        }

        public override void RandomUpdate(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			if (Main.rand.NextBool(10) && (!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != ModContent.TileType<EyeballVine>() && Main.tile[i, j + 1].TileType != ModContent.TileType<Eyeball>()))
			{
				tile.TileType = (ushort)ModContent.TileType<Eyeball>();
				tile.TileFrameX = (short)((Main.rand.Next(3) + (tile.TileFrameX >= 18 * 3 ? 3 : 0)) * 18);
				tile.TileFrameY = 0;
			}
			else if (Main.rand.NextBool(10) && !Main.tile[i, j + 1].HasTile)
			{
				bool maxLength = true;

				for (int a = 0; a < 10; a++)
				{
					if (Main.tile[i, j - a].TileType != ModContent.TileType<EyeballVine>())
					{
						maxLength = false;
						break;
					}
				}

				if (!maxLength)
				{
					WorldGen.PlaceTile(i, j + 1, ModContent.TileType<EyeballVine>(), true);
					Main.tile[i, j + 1].TileFrameX = (short)((Main.rand.Next(3) + (tile.TileFrameX >= 18 * 3 ? 3 : 0)) * 18);
				}
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			bool anchor = true;
			if (!Main.tile[i, j - 1].HasTile || !RemTile.SolidBottom(i, j - 1) && Main.tile[i, j - 1].TileType != ModContent.TileType<EyeballVine>())
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
			}
			else
			{
				if (!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != ModContent.TileType<EyeballVine>() && Main.tile[i, j + 1].TileType != ModContent.TileType<Eyeball>())
				{
					tile.TileFrameY = 36;
				}
				else if (Main.tile[i, j - 1].HasTile && Main.tileSolid[Main.tile[i, j - 1].TileType])
				{
					tile.TileFrameY = 0;
				}
				else tile.TileFrameY = 18;
			}

			return false;
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