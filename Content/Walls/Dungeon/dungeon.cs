using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Dungeon
{
	public class dungeonblue : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallDungeon[Type] = true;
			Main.wallHouse[Type] = false;
			Main.wallBlend[Type] = 1;

			DustType = DustID.DungeonBlue;

			AddMapEntry(new Color(24, 24, 37));

            VanillaFallbackOnModDeletion = WallID.BlueDungeonUnsafe;
        }

        const int Width = 13;
        const int Height = 8;
        const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            if (Lighting.GetColor(i, j) == Color.Black)
            {
                return false;
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
			Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);

			Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
			return false;
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}

	public class dungeongreen : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallDungeon[Type] = true;
			Main.wallHouse[Type] = false;
			Main.wallBlend[Type] = 1;

			DustType = DustID.DungeonGreen;

			AddMapEntry(new Color(27, 36, 38));

            VanillaFallbackOnModDeletion = WallID.GreenDungeonUnsafe;
        }

        const int Width = 13;
        const int Height = 8;
        const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            if (Lighting.GetColor(i, j) == Color.Black)
            {
                return false;
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
			Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);

			Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
			return false;
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}

	public class dungeonpink : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallDungeon[Type] = true;
			Main.wallHouse[Type] = false;
			Main.wallBlend[Type] = 1;

			DustType = DustID.DungeonPink;

			AddMapEntry(new Color(41, 25, 36));

            VanillaFallbackOnModDeletion = WallID.PinkDungeonUnsafe;
        }

        const int Width = 13;
        const int Height = 8;
        const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            if (Lighting.GetColor(i, j) == Color.Black)
            {
                return false;
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
			Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);

			Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
			return false;
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}
}
