using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Remnants.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls.Parallax
{
    public class stronghold : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;
            Main.wallLight[Type] = true;

            DustType = DustID.Ash;

            AddMapEntry(new Color(38, 30, 28));
        }

        //public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        //{
        //    Tile tile = Main.tile[i, j];

        //    if (!tile.HasTile || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
        //    {
        //        float num48 = 0.55f / 2;

        //        r = num48;
        //        g = num48 * 0.3f;
        //        b = num48 * 0.1f;
        //    }
        //}

        public const int Width = 10;
        public const int Height = 12;
        public const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            return false;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
