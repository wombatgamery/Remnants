using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Items.Placeable.Plants;
using Remnants.Items.Consumable;
using System.Collections.Generic;
using Terraria.GameContent.ObjectInteractions;

namespace Remnants.Tiles.Plants
{
    [LegacyName("jungleflowerhead")]
    public class PrismbudHead : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CoordinateHeights = new[] { 18 };
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.newTile.CoordinateWidth = 32;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(166, 255, 122), CreateMapEntryName());
            AddMapEntry(new Color(109, 255, 255), CreateMapEntryName());
            AddMapEntry(new Color(255, 140, 255), CreateMapEntryName());

            DustType = DustID.Shadewood;
		}

		//public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override ushort GetMapOption(int i, int j)
        {
			return (ushort)(Main.tile[i, j].TileFrameY / 18);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<PrismbudSeeds>(), Main.rand.Next(1, 4));
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<PrismbudSeeds>();
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;

            WorldGen.KillTile(i, j);
			NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);

            return true;
        }

        //public override void RandomUpdate(int i, int j)
        //{
        //	if (Main.rand.NextBool(10)) // grow
        //	{
        //		if (RemTile.FlowerGetLength(i, j) < 10)
        //		{
        //			if (!Main.tile[i, j - 1].HasTile)
        //			{
        //				WGTools.Tile(i, j).HasTile = false;
        //				WorldGen.PlaceTile(i, j, ModContent.TileType<jungleflowerstem>(), true);

        //				WorldGen.PlaceTile(i, j - 1, ModContent.TileType<jungleflowerhead>(), true, style: Main.rand.Next(3));

        //				Framing.GetTileSafely(i, j).TileFrameX = (short)(Main.rand.Next(3) * 18);
        //				Framing.GetTileSafely(i, j - 1).TileFrameX = 0;
        //			}
        //		}
        //	}
        //}

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			bool anchor = true;

			if (!Main.tile[i, j + 1].HasTile)
			{
				anchor = false;
			}
			else if (Main.tile[i, j + 1].TileType != TileID.JungleGrass && !Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].TileType != ModContent.TileType<PrismbudStem>())
			{
				anchor = false;
			}
			else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
				return false;
			}

			return true;
		}

		//public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		//{
		//	if (Main.rand.NextBool(125))
		//	{
		//		Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 4), 0, 0, ModContent.DustType<growthspore>());
		//	}
		//}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 0)
            {
				r = (float)(42f / 255f);
				g = (float)(211f / 255f);
				b = (float)(135f / 255f);
			}
			else if (tile.TileFrameY == 20)
			{
				r = (float)(0f / 255f);
				g = (float)(169f / 255f);
				b = (float)(255f / 255f);
			}
			else if (tile.TileFrameY == 40)
			{
				r = (float)(198f / 255f);
				g = (float)(66f / 255f);
				b = (float)(255f / 255f);
			}
			//r /= 2f;
			//g /= 2f;
			//b /= 2f;
		}

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameY == 18)
			{
                tile.TileFrameY = 20;
            }
            else if (tile.TileFrameY == 36)
            {
                tile.TileFrameY = 40;
            }

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            SpriteEffects effect = i % 2 == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - 8 - (int)Main.screenPosition.X, j * 16 - 2 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 32, 18), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);

            return false;
        }
	}

	[LegacyName("jungleflowerstem")]
	public class PrismbudStem : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;

			Main.tileMerge[Type][ModContent.TileType<PrismbudHead>()] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			AddMapEntry(new Color(162, 82, 109));
			DustType = DustID.RichMahogany;
		}

        public override bool CanDrop(int i, int j)
        {
			return false;
        }

        public override bool CanPlace(int i, int j)
        {
            if (!Main.tile[i, j + 1].HasTile)
            {
                return false;
            }
            else if (Main.tile[i, j + 1].TileType != TileID.JungleGrass)// && !Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].TileType != ModContent.TileType<PrismbudStem>())
            {
                return false;
            }
            else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
            {
                return false;
            }

			return true;
        }

        public override void RandomUpdate(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			if (Main.rand.NextBool(10) && Main.tile[i, j].LiquidAmount == 0) // turn into flower
			{
				if (!Main.tile[i, j - 1].HasTile || Main.tile[i, j - 1].TileType != ModContent.TileType<PrismbudStem>() && Main.tile[i, j - 1].TileType != ModContent.TileType<PrismbudHead>())
				{
					tile.HasTile = false;
					WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<PrismbudHead>(), true, style: Main.rand.Next(3));
				}
			}
            else if (!Main.tile[i, j - 1].HasTile)
            {
				if (RemTile.FlowerGetLength(i, j) < 10)
				{
                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<PrismbudStem>(), true);
                }
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];

			bool anchor = true;

			if (!Main.tile[i, j + 1].HasTile)
			{
				anchor = false;
			}
			else if (Main.tile[i, j + 1].TileType != TileID.JungleGrass && !Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].TileType != ModContent.TileType<PrismbudStem>())
			{
				anchor = false;
			}
			else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
			{
				anchor = false;
			}

			if (!anchor)
			{
				WorldGen.KillTile(i, j);
				return false;
			}

			return true;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
	}
}