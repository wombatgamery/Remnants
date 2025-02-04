using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Blocks
{
    public class ArcanePlatform : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileTable[Type] = true;

            TileID.Sets.Platforms[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.FullCopyFrom(TileID.Platforms);
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            MinPick = 110;
            DustType = DustID.Stone;
			HitSound = SoundID.Tink;
            AdjTiles = new int[] { TileID.Platforms };

            AddMapEntry(new Color(255, 255, 255));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasWalkDust() => true;
        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.TreasureSparkle;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float mult = 0.75f + (float)Math.Sin((Main.GameUpdateCount - i / 0.25f) * 0.06f) * 0.25f;

            if (!Main.tile[i, j - 1].HasTile || !Main.tileSolid[Main.tile[i, j - 1].TileType] || Main.tile[i, j - 1].TileType == TileID.ClosedDoor)
            {
                Color color = RemTile.MagicalLabLightColour(i);
                //RemTile.RGBLight(r, g, b, 112, 93, 133);
                r = color.R / 255f;// (241f / 255f) * mult;
                g = color.G / 255f;// (195f / 255f) * mult;
                b = color.B / 255f;// (233f / 255f) * mult;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Blocks/ArcanePlatformGlow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), RemTile.MagicalLabLightColour(i), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Blocks/ArcanePlatformShine").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}