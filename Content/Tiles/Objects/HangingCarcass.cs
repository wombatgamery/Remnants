using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles.Objects.Decoration;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects
{
    [LegacyName("MeatHook")]
	public class HangingCarcass : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.CanDropFromRightClick[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.DrawXOffset = -2;
            TileObjectData.newTile.DrawYOffset = -16;
			TileObjectData.newTile.CoordinateWidth = 24;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 32 };
			TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 6;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { ModContent.TileType<ChainHook>() };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(154, 72, 51));

			DustType = DustID.Blood;
			HitSound = SoundID.NPCHit18;
		}

        //public override void MouseOver(int i, int j)
        //{
        //	Player player = Main.LocalPlayer;
        //	Tile tile = Main.tile[i, j];

        //	player.cursorItemIconID = ItemID.Minecart;
        //	player.cursorItemIconText = "";

        //	player.noThrow = 2;
        //	player.cursorItemIconEnabled = true;
        //}

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 position = new Vector2(i * 16 - 5 - (int)Main.screenPosition.X, j * 16 - 16 - (int)Main.screenPosition.Y) + zero;

            Tile tile = Main.tile[i, j];

            Main.spriteBatch.Draw(texture, position, new Rectangle(tile.TileFrameX, tile.TileFrameY, 24, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            if (tile.TileFrameY == 16)
            {
                Main.spriteBatch.Draw(texture, position + Vector2.UnitY * 16, new Rectangle(tile.TileFrameX, tile.TileFrameY + 16, 24, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            return false;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, new Vector2(i + 0.5f, j + 0.5f) * 16);
            }
            return false;
        }
    }
}