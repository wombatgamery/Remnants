using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.DesertRuins
{
	[LegacyName("pyramidwall")]
	public class PyramidBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/DesertRuins/PyramidBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.Sand;

			AddMapEntry(new Color(140, 82, 52));

            VanillaFallbackOnModDeletion = WallID.SandstoneBrick;
        }

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}

	public class PyramidBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Sand;

			AddMapEntry(new Color(140, 82, 52));

            VanillaFallbackOnModDeletion = WallID.SandstoneBrick;
        }

		public override bool CanExplode(int i, int j) => false;
	}

	public class PyramidRuneWall : ModWall
	{
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            DustType = DustID.Sand;

            AddMapEntry(new Color(148, 102, 81));

            VanillaFallbackOnModDeletion = WallID.SandstoneBrick;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new(16, 16, 16, 16);

            if (Main.tile[i - 1, j].WallType != Type)
            {
                frame.X -= 16;
            }
            if (Main.tile[i + 1, j].WallType != Type)
            {
                frame.X += 16;
            }
            if (Main.tile[i, j - 1].WallType != Type)
            {
                frame.Y -= 16;
            }
            if (Main.tile[i, j + 1].WallType != Type)
            {
                frame.Y += 16;
            }

            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);

            return false;
        }
    }
}
