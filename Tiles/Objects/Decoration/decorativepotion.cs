//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.Enums;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;
//using Terraria.ObjectData;

//namespace Remnants.Tiles.Objects
//{
//	public class decorativepotion : ModTile
//	{
//		public override void SetStaticDefaults()
//		{
//			Main.tileLighted[Type] = true;
//			Main.tileFrameImportant[Type] = true;
//			Main.tileSolid[Type] = false;
//			Main.tileWaterDeath[Type] = false;
//			Main.tileLavaDeath[Type] = true;
//			Main.tileTable[Type] = true;

//			TileID.Sets.DisableSmartCursor[Type] = true;

//			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
//			TileObjectData.newTile.CoordinateHeights = new int[1] { 18 };
//			TileObjectData.newTile.CoordinateWidth = 16;
//			TileObjectData.newTile.DrawYOffset = -2;
//			TileObjectData.newTile.StyleHorizontal = true;
//			TileObjectData.newTile.RandomStyleRange = 6;
//			TileObjectData.newTile.StyleWrapLimit = 3;
//			TileObjectData.addTile(Type);

//			LocalizedText name = CreateMapEntryName();
//			// name.SetDefault("Bottle");
//			AddMapEntry(new Color(217, 225, 255), CreateMapEntryName());

//			RegisterItemDrop(ModContent.ItemType<Items.placeable.objects.decorativepotion>());
//			DustType = DustID.Glass;
//			HitSound = SoundID.Shatter;
//		}

//		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
//		{
//			Tile tile = Main.tile[i, j];
//			if (tile.TileFrameX == 0)
//			{
//				r = (float)(101f / 255f);
//				g = (float)(61f / 255f);
//				b = (float)(74f / 255f);
//			}
//			else if (tile.TileFrameX == 18)
//			{
//				r = (float)(54f / 255f);
//				g = (float)(90f / 255f);
//				b = (float)(78f / 255f);
//			}
//			else if (tile.TileFrameX == 36)
//			{
//				r = (float)(61f / 255f);
//				g = (float)(66f / 255f);
//				b = (float)(102f / 255f);
//			}
//		}

//        public override bool RightClick(int i, int j)
//        {
//			WorldGen.KillTile(i, j);
//			return true;
//        }

//		public override void MouseOver(int i, int j)
//		{
//			Player player = Main.LocalPlayer;
//			player.noThrow = 2;
//			player.cursorItemIconEnabled = true;
//			player.cursorItemIconID = ModContent.ItemType<Items.placeable.objects.decorativepotion>();
//		}

//		//public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
//		//{
//		//	Tile tile = Main.tile[i, j];
//		//	Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
//		//	if (Main.drawToScreen)
//		//	{
//		//		zero = Vector2.Zero;
//		//	}
//		//	if (tile.TileFrameX == 0)
//		//	{
//		//		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("WombatQOL/Tiles/decorativepotion").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 18, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//		//	}
//		//}
//	}
//}