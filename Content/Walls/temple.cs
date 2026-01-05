using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
    public class temple : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = 148;

            AddMapEntry(new Color(17, 5, 4));

            VanillaFallbackOnModDeletion = WallID.LihzahrdBrickUnsafe;
        }

        const int Width = 10;
        const int Height = 10;
        const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Lighting.GetColor(i, j) == Color.Black)
            {
                return false;
            }

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
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

        //      public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        //      {
        //	Tile tile = Main.tile[i, j];
        //	int frameX = (i % 10) + ((j % 10) >= 8 ? 10 : 0) - ((i % 10) >= 5 && (j % 10) >= 8 ? 5 : 0);
        //	int frameY = (j % 10) - ((j % 10) >= 8 ? 8 : 0) + ((i % 10) >= 5 && (j % 10) >= 8 ? 2 : 0);

        //	frameX *= 36;
        //	frameY *= 36;
        //	if (tile.WallFrameX != frameX)
        //	{
        //		tile.WallFrameX = frameX;
        //	}
        //	if (tile.WallFrameY != frameY)
        //	{
        //		tile.WallFrameY = frameY;
        //	}
        //}
    }
}
