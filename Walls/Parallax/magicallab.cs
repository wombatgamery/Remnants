using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Remnants.Tiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls.Parallax
{
    public class magicallab : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = DustID.Stone;

            AddMapEntry(new Color(36, 33, 53));
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile || !Main.tileSolid[tile.TileType] || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

                int width = 8;
                int height = 6;
                int parallax = 8;

                Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
                Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / parallax) % (width * 16), (int)(j * 16 - Main.screenPosition.Y / parallax) % (height * 16), 16, 16);



                Main.spriteBatch.Draw(texture, position, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Walls/Parallax/magicallabglow").Value, position, frame, RemTile.MagicalLabLightColour(j), 0f, Vector2.Zero, 1f, 0, 0f);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Walls/Parallax/magicallabglow2").Value, position, frame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
            }

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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (!tile.HasTile || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
            {
                Color color = RemTile.MagicalLabLightColour(j, true);

                r = color.R / 255f;
                g = color.G / 255f;
                b = color.B / 255f;
            }
        }
    }
}
