using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles;
using Remnants.Content.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Parallax
{
    public class magicallab : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = DustID.Stone;

            AddMapEntry(new Color(36, 33, 53));

            VanillaFallbackOnModDeletion = WallID.VioletMossBlockWall;
        }

        const int Width = 8;
        const int Height = 6;
        const int ScrollSpeed = 8;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile || !Main.tileSolid[tile.TileType] || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
            {
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

                Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
                Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / ScrollSpeed) % (Width * 16), (int)(j * 16 - Main.screenPosition.Y / ScrollSpeed) % (Height * 16), 16, 16);                

                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Walls/Parallax/magicallabglow").Value, position, frame, RemTile.MagicalLabLightColour(j), 0f, Vector2.Zero, 1f, 0, 0f);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Walls/Parallax/magicallabglow2").Value, position, frame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
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
