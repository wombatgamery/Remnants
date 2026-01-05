using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Shimmer
{
	public class AlchemyBench : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileTable[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(2, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(136, 87, 69), CreateMapEntryName());

			DustType = DustID.WoodFurniture;
			HitSound = SoundID.Tink;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Color color = RemTile.MagicalLabLightColour(i);
            //RemTile.RGBLight(r, g, b, 112, 93, 133);
            r = color.R / 255f;// (241f / 255f) * mult;
            g = color.G / 255f;// (195f / 255f) * mult;
            b = color.B / 255f;// (233f / 255f) * mult;
        }

        public override bool HasWalkDust() => true;
        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.TreasureSparkle;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 0)
            {
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), RemTile.MagicalLabLightColour(i), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Shine").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}