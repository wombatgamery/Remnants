//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Remnants.Content.Dusts;
//using Remnants.Content.Items.tools;
//using Remnants.Content.Worldgen.Subworlds;
////using SubworldLibrary;
//using Terraria;
//using Terraria.Audio;
//using Terraria.DataStructures;
//using Terraria.Enums;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;
//using Terraria.ModLoader.IO;
//using Terraria.ObjectData;

//namespace Remnants.Content.Tiles.Objects
//{
//    public class hivekeyholder : ModTile
//	{
//		public override void SetStaticDefaults()
//		{
//			Main.tileLighted[Type] = true;
//			Main.tileFrameImportant[Type] = true;
//			Main.tileLavaDeath[Type] = false;

//			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
//			TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
//			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
//			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
//			TileID.Sets.GetsDestroyedForMeteors[Type] = false;
//			TileID.Sets.DisableSmartCursor[Type] = true;

//			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
//			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18,};
//			TileObjectData.newTile.StyleHorizontal = true;
//			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEhivekeyholder>().Hook_AfterPlacement, -1, 0, false);
//			TileObjectData.addTile(Type);

//			LocalizedText name = CreateMapEntryName();
//			AddMapEntry(new Color(255, 174, 0));

//			DustType = DustID.Hive;
//		}

//        public override void KillMultiTile(int i, int j, int frameX, int frameY)
//        {
//			Point16 origin = TileUtils.GetTileOrigin(i, j);
//			ModContent.GetInstance<TEhivekeyholder>().Kill(origin.X, origin.Y);
//		}

//        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
//        {
//			return false;
//        }
//		public override bool CanExplode(int i, int j)
//		{
//			return false;
//		}

//		public override bool RightClick(int i, int j)
//        {
//			Player player = Main.LocalPlayer;
//			Tile tile = Main.tile[i, j];

//			Point16 origin = TileUtils.GetTileOrigin(i, j);

//			if (TileUtils.TryGetTileEntityAs(i, j, out TEhivekeyholder tileEntity))
//			{
//				if (tileEntity.hasKeyInIt)
//				{
//					int item = Item.NewItem(null, (origin.ToVector2() + Vector2.One) * 16, ModContent.ItemType<hivekey>());
//					if (Main.netMode == NetmodeID.MultiplayerClient)
//                    {
//						NetMessage.SendData(MessageID.SyncItem, number: item, number2: 1f);
//                    }

//					SoundEngine.PlaySound(SoundID.Unlock, new Vector2((origin.X + 1) * 16, (origin.Y + 2) * 16));

//					tileEntity.hasKeyInIt = false;

//					return true;
//				}
//			}
//			return false;
//		}

//		public override void MouseOver(int i, int j)
//		{
//			Player player = Main.LocalPlayer;
//			Tile tile = Main.tile[i, j];

//			if (TileUtils.TryGetTileEntityAs(i, j, out TEhivekeyholder tileEntity))
//			{
//				if (tileEntity.hasKeyInIt)
//				{
//					player.cursorItemIconEnabled = true;
//					player.cursorItemIconID = ModContent.ItemType<hivekey>();
//					player.cursorItemIconText = "";

//					player.noThrow = 2;
//				}
//			}
//		}

//		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
//        {
//			Tile tile = Main.tile[i, j];
//			if (TileUtils.TryGetTileEntityAs(i, j, out TEhivekeyholder tileEntity))
//            {
//				if (tileEntity.hasKeyInIt)
//                {
//					r = 234f / 255f;
//					g = 125f / 255f;
//					b = 0;
//				}
//			}
//		}

//        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
//        {
//            Tile tile = Main.tile[i, j];

//            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
//            {
//                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
//            }
//        }

//		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
//		{
//			Point p = new Point(i, j);
//			Tile tile = Main.tile[p.X, p.Y];
//			if (tile == null || !tile.HasTile)
//			{
//				return;
//			}

//			Vector2 offScreen = new Vector2(Main.offScreenRange);
//			if (Main.drawToScreen)
//			{
//				offScreen = Vector2.Zero;
//			}

//			Texture2D texture;
//			Rectangle frame = new Rectangle(0, 0, 16, 16);
//			Vector2 worldPos = p.ToWorldCoordinates(0f, 0f);

//			if (TileUtils.TryGetTileEntityAs(i, j, out TEhivekeyholder tileEntity))
//			{
//				if (tileEntity.hasKeyInIt)
//				{
//					// CODE ALWAYS REACHES THIS POINT AS INTENDED
//					Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
//					drawPos.X += 8;
//					drawPos.Y += 8;

//					texture = ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/hivegatewaykey").Value;
//					spriteBatch.Draw(texture, drawPos, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//					texture = ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/hivegatewaykeyglow").Value;
//					spriteBatch.Draw(texture, drawPos, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//				}
//			}
//		}
//	}

//    public class TEhivekeyholder : ModTileEntity
//    {
//		public bool hasKeyInIt = true;

//		public override bool IsTileValidForEntity(int x, int y)
//        {
//			Tile tile = Main.tile[x, y];
//			return tile.HasTile && tile.TileType == ModContent.TileType<hivekeyholder>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
//		}

//		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
//		{
//			if (Main.netMode == NetmodeID.MultiplayerClient)
//			{
//				int width = 2;
//				int height = 2;
//				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

//				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
//				return -1;
//			}
//			Point16 tileOrigin = new Point16(0, 1);
//			return Place(i - tileOrigin.X, j - tileOrigin.Y);
//		}

//		public override void OnNetPlace()
//		{
//			if (Main.netMode == NetmodeID.Server)
//			{
//				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
//			}
//		}

//        public override void SaveData(TagCompound tag)
//        {
//			tag["HasKeyInIt"] = hasKeyInIt;
//		}

//        public override void LoadData(TagCompound tag)
//        {
//			if (tag.TryGet("HasKeyInIt", out bool flag))
//			{
//				hasKeyInIt = flag;
//			}
//		}
//    }
//}