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
    public class GoldenCity : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = 1;

            DustType = DustID.Gold;

            AddMapEntry(new Color(60, 38, 32));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (!tile.HasTile || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
            {
                Color color = RemTile.GoldenCityLightColour(i, j, true, true);

                r = color.R / 255f / 2f;
                g = color.G / 255f / 2f;
                b = color.B / 255f / 2f;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 8;
            int height = 8;
            int parallax = 8;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new((int)(i * 16 - Main.screenPosition.X / parallax) % (width * 16), (int)(j * 16 - Main.screenPosition.Y / parallax) % (height * 16), 16, 16);

            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);

            Tile tile = Main.tile[i, j];
            if (!tile.HasTile || !Main.tileBlockLight[tile.TileType] || tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Walls/Parallax/goldencityglow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, RemTile.GoldenCityLightColour(i, j, red: true), 0f, Vector2.Zero, 1f, 0, 0f);
            }

            return false;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class goldenmonitor : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            AddMapEntry(new Color(76, 58, 57));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Color color = RemTile.GoldenCityLightColour(i, j, true);

            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Remnants/Content/Walls/Backgrounds/goldenmonitor").Value;
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

            if (frame.X == 0 || frame.Y == 0)
            {
                Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new(frame.X, frame.Y + 16 * 3, 16, 16), RemTile.GoldenCityLightColour(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            }

            Rectangle frame2 = new(16 + (int)(Main.GameUpdateCount * 0.2f) * 2 % 16, 16 * 4, 16, 16);
            Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            if (frame.X != 0 && frame.Y != 0)
            {
                if (frame.Y == 32)
                {
                    frame2.Height -= 6;
                }
                if (frame.X == 32)
                {
                    frame2.Width -= 4;
                }

                Main.spriteBatch.Draw(texture, position, frame2, RemTile.GoldenCityLightColour(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            }

            return false;
        }
    }

    public class goldenconsole : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            AddMapEntry(new Color(76, 58, 57));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Color color = RemTile.GoldenCityLightColour(i, j, true);

            r = (float)(color.R / 255f);
            g = (float)(color.G / 255f);
            b = (float)(color.B / 255f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Remnants/Content/Walls/Backgrounds/goldenconsole").Value;
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

            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);

            if (frame.Y == 0)
            {
                Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new(frame.X, frame.Y + 16 * 2, 16, 16), RemTile.GoldenCityLightColour(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            }

            return false;
        }
    }

    public class goldenserver : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            AddMapEntry(new Color(76, 58, 57));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Color color = RemTile.GoldenCityLightColour(i, j, true);

            r = (float)(color.R / 255f);
            g = (float)(color.G / 255f);
            b = (float)(color.B / 255f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Remnants/Content/Walls/Backgrounds/goldenserver").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);
            Rectangle frame = new(16, 0, 16, 16);

            if (Main.tile[i - 1, j].WallType != Type)
            {
                frame.X -= 16;
            }
            if (Main.tile[i + 1, j].WallType != Type)
            {
                frame.X += 16;
            }

            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            if (frame.X != 32)
            {
                Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new(frame.X, frame.Y + 16, 16, 16), RemTile.GoldenCityLightColour(i, j), 0f, Vector2.Zero, 1f, 0, 0f);
            }

            return false;
        }
    }
}
