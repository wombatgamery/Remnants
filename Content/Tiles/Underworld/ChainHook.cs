using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Walls.Underworld;
using Remnants.Content.World;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Underworld
{
	public class ChainHook : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileMerge[TileID.Chain][Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.CoordinateHeights = new[] { 20 };
			TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.Chain };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(163, 138, 139), Language.GetText("ItemName.Hook"));

			DustType = DustID.t_SteampunkMetal;
			HitSound = SoundID.Dig;
		}

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].WallType == ModContent.WallType<AshenBrickWallUnsafe>() || Main.tile[i, j].WallType == ModContent.WallType<IronBars>() && j > Main.maxTilesY - 200)
            {
                if (Main.rand.NextBool(10) && !WorldGen.SolidOrSlopedTile(i, j + 1) && !WorldGen.SolidOrSlopedTile(i, j + 2))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<HangingCarcass>(), style: Main.rand.Next(3));

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 2, 3, TileChangeType.None);
                    }
                }
            }
        }

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
            Vector2 position = new Vector2(i * 16 - 1 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            Tile tile = Main.tile[i, j];

            Main.spriteBatch.Draw(texture, position, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 20), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return false;
        }
    }
}