using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Underworld
{
    public class IronBars : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallLight[Type] = true;
            DustType = DustID.Iron;
            RegisterItemDrop(ModContent.ItemType<Items.Placeable.Walls.IronBars>());

            VanillaFallbackOnModDeletion = WallID.MetalFence;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<AshenBrickWallUnsafe>() || Main.tile[i, j + 1].WallType == ModContent.WallType<AshenBrickWall>())
            {
                Tile tile = Main.tile[i, j];

                Texture2D texture = ModContent.Request<Texture2D>("Remnants/Content/Walls/Underworld/AshenBrickWallSill").Value;

                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                if (Main.drawToScreen)
                {
                    zero = Vector2.Zero;
                }
                Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 + 16 - (int)Main.screenPosition.Y) + zero;

                Main.spriteBatch.Draw(texture, position, new Rectangle(16, 0, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (Main.tile[i - 1, j].WallType != Type)
                {
                    Main.spriteBatch.Draw(texture, position - Vector2.UnitX * 16, new Rectangle(0, 0, 16, 16), Lighting.GetColor(i - 1, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (Main.tile[i + 1, j].WallType != Type)
                {
                    Main.spriteBatch.Draw(texture, position + Vector2.UnitX * 16, new Rectangle(32, 0, 16, 16), Lighting.GetColor(i - 1, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }

    public class IronBarsSafe : ModWall
    {
        public override string Texture => "Remnants/Content/Walls/Underworld/IronBars";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLight[Type] = true;
            DustType = DustID.Iron;

            VanillaFallbackOnModDeletion = WallID.MetalFence;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<AshenBrickWallUnsafe>() || Main.tile[i, j + 1].WallType == ModContent.WallType<AshenBrickWall>())
            {
                Tile tile = Main.tile[i, j];

                Texture2D texture = ModContent.Request<Texture2D>("Remnants/Content/Walls/Underworld/AshenBrickWallSill").Value;

                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                if (Main.drawToScreen)
                {
                    zero = Vector2.Zero;
                }
                Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 + 16 - (int)Main.screenPosition.Y) + zero;

                Main.spriteBatch.Draw(texture, position, new Rectangle(16, 0, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (Main.tile[i - 1, j].WallType != Type)
                {
                    Main.spriteBatch.Draw(texture, position - Vector2.UnitX * 16, new Rectangle(0, 0, 16, 16), Lighting.GetColor(i - 1, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (Main.tile[i + 1, j].WallType != Type)
                {
                    Main.spriteBatch.Draw(texture, position + Vector2.UnitX * 16, new Rectangle(32, 0, 16, 16), Lighting.GetColor(i - 1, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
