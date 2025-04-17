using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Parallax
{
    public class vault : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(46, 43, 56));
        }

        public const int Width = 14;
        public const int Height = 14;
        public const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (ShouldDrawRailing(i, j))
            {
                bool left = ShouldDrawRailing(i - 1, j) || !EmptyTile(i - 1, j);
                bool right = ShouldDrawRailing(i + 1, j) || !EmptyTile(i + 1, j);
                if (left || right)
                {
                    int frameX;
                    if (!left)
                    {
                        frameX = 2;
                    }
                    else if (!right)
                    {
                        frameX = 3;
                    }
                    else frameX = i % 2 == 0 ? 0 : 1;

                    frameX *= 16;

                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
                    Rectangle frame = new(frameX, 0, 16, 32);

                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Walls/Parallax/railing").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, (j - 1) * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
                }
            }
        }

        private bool ShouldDrawRailing(int i, int j)
        {
            Tile tile = Main.tile[i, j + 1];
            return EmptyTile(i, j) && EmptyTile(i, j - 1) && EmptyTile(i, j - 2) && tile.HasTile && (tile.TileType == ModContent.TileType<VaultPlating>() || tile.TileType == ModContent.TileType<VaultPlatform>());
        }

        private bool EmptyTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return !tile.HasTile || tile.TileType != ModContent.TileType<VaultPlating>() && tile.TileType != ModContent.TileType<VaultPipe>();
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
