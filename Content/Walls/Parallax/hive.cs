using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Parallax
{
    public class hive : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = DustID.Hive;

            AddMapEntry(new Color(62, 29, 15));
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 8;
            int height = 12;
            int parallax = 8;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / parallax) % (width * 16), (int)(j * 16 - Main.screenPosition.Y / parallax) % (height * 16), 16, 16);

            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);

            //Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Walls/Backgrounds/hiveglow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, frame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
            //ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
            //Color color = new Color(100, 100, 100, 0);
            //if (Main.drawToScreen)
            //{
            //	zero = Vector2.Zero;
            //}
            //for (int k = 0; k < 7; k++)
            //{
            //	float x = Terraria.Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
            //	float y = Terraria.Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
            //	Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Walls/Backgrounds/hiveglow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X + x, (j * 16) - (int)Main.screenPosition.Y + y) + zero, frame, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            //}
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

        //public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        //{
        //	Tile tile = Main.tile[i, j];

        //	if (!tile.HasTile || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
        //	{
        //		r = (44 / 255f);
        //		g = (25 / 255f);
        //		b = (23 / 255f);
        //	}
        //}
    }
}
