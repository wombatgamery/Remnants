//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Remnants.Dusts;
//using Remnants.Items.tools;
//using Remnants.Worldgen.Subworlds;
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

//namespace Remnants.Tiles.Objects
//{
//    public class hivegateway : ModTile
//	{
//		public override void SetStaticDefaults()
//		{
//			Main.tileLighted[Type] = true;
//			Main.tileFrameImportant[Type] = true;
//			Main.tileLavaDeath[Type] = false;
//			Main.tileNoAttach[Type] = true;

//			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
//			TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
//			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
//			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
//			TileID.Sets.GetsDestroyedForMeteors[Type] = false;
//			TileID.Sets.DisableSmartCursor[Type] = true;

//			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
//			TileObjectData.newTile.Height = 5;
//			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 18 };
//			TileObjectData.newTile.StyleHorizontal = true;
//			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEhivegateway>().Hook_AfterPlacement, -1, 0, false);
//			TileObjectData.addTile(Type);

//			LocalizedText name = CreateMapEntryName();
//			// name.SetDefault("Gateway");
//			AddMapEntry(new Color(255, 174, 0), CreateMapEntryName());

//			DustType = DustID.Hive;
//		}

//        public override void KillMultiTile(int i, int j, int frameX, int frameY)
//        {
//			Point16 origin = TileUtils.GetTileOrigin(i, j);
//			ModContent.GetInstance<TEhivegateway>().Kill(origin.X, origin.Y);
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

//			if (tile.TileFrameX >= 18 * 21)
//			{
//				//if (SubworldSystem.IsActive<TheColony>())
//    //            {
//				//	SubworldSystem.Exit();
//    //            }
//				//else SubworldSystem.Enter<TheColony>();
//				//return true;
//			}
//			else
//            {
//				Point16 origin = TileUtils.GetTileOrigin(i, j);

//				if (TileUtils.TryGetTileEntityAs(i, j, out TEhivegateway tileEntity))
//				{
//                    if (player.HasItem(ModContent.ItemType<hivekey>()) && tileEntity.canInsertKeys)
//                    {
//                        player.ConsumeItem(ModContent.ItemType<hivekey>());
//                        tileEntity.keysInserted++;

//                        if (tileEntity.keysInserted >= 3)
//                        {
//							tileEntity.animationTimer = 75;
//                        }
//                        SoundEngine.PlaySound(SoundID.Unlock, new Vector2((origin.X + 1) * 16, (origin.Y + 2) * 16));

//						if (tileEntity.keysInserted >= 3)
//						{
//							Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 4), 8, 8, DustID.YellowTorch);
//						}
//						return true;
//                    }
//                }
//			}
//			return false;
//		}

//		public override void MouseOver(int i, int j)
//		{
//			Player player = Main.LocalPlayer;
//			Tile tile = Main.tile[i, j];

//			if (tile.TileFrameX <= 18 * 2)
//			{
//				player.cursorItemIconEnabled = true;
//				player.cursorItemIconID = ModContent.ItemType<hivekey>();
//				player.cursorItemIconText = "";

//				player.noThrow = 2;
//			}
//			//else
//   //         {
//			//	player.cursorItemIconText = "ENTER";
//			//	player.noThrow = 2;
//			//}
//		}

//		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
//        {
//			Tile tile = Main.tile[i, j];
//			if (tile.TileFrameX >= 18 * 9)
//            {
//				r = 1;
//				g = 174f / 255f;
//				b = 0;
//			}
//			else if (TileUtils.TryGetTileEntityAs(i, j, out TEhivegateway tileEntity) && tileEntity.keysInserted > 0)
//			{
//				int num = tileEntity.keysInserted;
//				if (tileEntity.animationTimer != -1)
//                {
//					if (tileEntity.animationTimer <= 60)
//                    {
//						num--;
//					}
//					if (tileEntity.animationTimer <= 56)
//					{
//						num--;
//					}
//					if (tileEntity.animationTimer <= 50)
//					{
//						num--;
//					}
//				}

//				r = 234f / 255f * num / 3;
//				g = 125f / 255f * num / 3;
//				b = 0;
//			}
//		}

//        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
//		{
//			Tile tile = Main.tile[i, j];
//			if (tile.TileFrameX >= 18 * 9)
//            {
//				if (Main.rand.NextBool(25))
//				{
//					int dustIndex = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 4), 8, 8, DustID.YellowTorch);
//					//Main.dust[dustIndex].noGravity = true;
//					Main.dust[dustIndex].velocity.X = 0;
//					Main.dust[dustIndex].velocity.Y = Main.rand.Next(-5, 5);
//				}
//			}

//			if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
//			{
//				Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
//			}
//		}

//        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
//        {
//            Tile tile = Main.tile[i, j];
//            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
//            if (Main.drawToScreen)
//            {
//                zero = Vector2.Zero;
//            }
//            int height = tile.TileFrameY == 18 * 4 ? 18 : 16;
//			spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Tiles/Objects/hivegatewayglow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//		}

//        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
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

//			if (TileUtils.TryGetTileEntityAs(i, j, out TEhivegateway tileEntity))
//			{
//				for (int k = 0; k < tileEntity.keysInserted; k++)
//				{
//					// CODE ALWAYS REACHES THIS POINT AS INTENDED
//					Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
//					drawPos.X += 12;
//					drawPos.Y += 28 + k * 14;

//					if (tile.TileFrameX == 0)
//					{
//						texture = ModContent.Request<Texture2D>("Remnants/Tiles/Objects/hivegatewaykey").Value;
//						spriteBatch.Draw(texture, drawPos, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//					}

//					if (tileEntity.animationTimer == -1 || k == 0 && tileEntity.animationTimer > 50 || k == 1 && tileEntity.animationTimer > 55 || k == 2 && tileEntity.animationTimer > 60)
//                    {
//						texture = ModContent.Request<Texture2D>("Remnants/Tiles/Objects/hivegatewaykeyglow").Value;
//						spriteBatch.Draw(texture, drawPos, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//					}
//				}
//			}
//		}
//	}

//    public class TEhivegateway : ModTileEntity
//    {
//		public int keysInserted;
//		public bool canInsertKeys => keysInserted < 3;

//		public int animationTimer = -1;

//		public override bool IsTileValidForEntity(int x, int y)
//        {
//			Tile tile = Main.tile[x, y];
//			return tile.HasTile && tile.TileType == ModContent.TileType<hivegateway>() && tile.TileFrameX % (18 * 3) == 0 && tile.TileFrameY == 0;
//		}

//		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
//		{
//			if (Main.netMode == NetmodeID.MultiplayerClient)
//			{
//				int width = 3;
//				int height = 5;
//				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

//				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
//				return -1;
//			}
//			Point16 tileOrigin = new Point16(1, 3);
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
//			tag["NumberOfKeysInserted"] = keysInserted;
//			tag["DoorOpening"] = animationTimer > 0;
//		}

//        public override void LoadData(TagCompound tag)
//        {
//			if (tag.TryGet("NumberOfKeysInserted", out int number))
//			{
//				keysInserted = number;
//			}
//			if (tag.TryGet("DoorOpening", out bool flag))
//			{
//				if (flag)
//                {
//					SetFrame(7);
//					animationTimer = -1;
//				}
//			}
//		}

//        public override void Update()
//        {
//			if (animationTimer == 60 || animationTimer == 55 || animationTimer == 50)
//			{
//				SoundEngine.PlaySound(SoundID.Item89, new Vector2((Position.X + 1) * 16, (Position.Y + 2) * 16));

//				for (int i = 0; i < 5; i++)
//                {
//					if (animationTimer == 60)
//                    {
//                        Dust.NewDust(new Vector2(Position.X * 16 + 16, Position.Y * 16 + 32 + 28), 4, 4, DustID.YellowTorch);
//                    }
//                    if (animationTimer == 55)
//                    {
//                        Dust.NewDust(new Vector2(Position.X * 16 + 16, Position.Y * 16 + 32 + 14), 4, 4, DustID.YellowTorch);
//                    }
//                    if (animationTimer == 50)
//                    {
//                        Dust.NewDust(new Vector2(Position.X * 16 + 16, Position.Y * 16 + 32), 4, 4, DustID.YellowTorch);
//						SetFrame(1);
//					}
//                }
//            }
//			else if (animationTimer == 30)
//			{
//				SoundEngine.PlaySound(SoundID.Item62, new Vector2((Position.X + 1) * 16, (Position.Y + 2) * 16));
//				//RemPlayer.ScreenShake(new Vector2((Position.X + 1) * 16 + 8, (Position.Y + 2) * 16 + 8), 1);

//				for (int i = 0; i < 15; i++)
//				{
//					int dustIndex = Dust.NewDust(new Vector2(Position.X * 16 + Main.rand.Next(16 * 3) - 8, Position.Y * 16 + Main.rand.Next(16 * 5) - 8), 8, 8, DustID.Smoke);
//					Main.dust[dustIndex].noGravity = true;
//					Main.dust[dustIndex].velocity.X = 0;
//					Main.dust[dustIndex].velocity.Y = Main.rand.Next(3);
//				}
//			}
//            else if (animationTimer == 12)
//            {
//				SoundEngine.PlaySound(SoundID.Item113, new Vector2((Position.X + 1) * 16, (Position.Y + 2) * 16));
//				SetFrame(2);
//            }
//            else if (animationTimer == 10)
//			{
//				SetFrame(3);
//			}
//			else if (animationTimer == 8)
//			{
//				SetFrame(4);
//			}
//			else if (animationTimer == 6)
//			{
//				SetFrame(5);
//			}
//			else if (animationTimer == 4)
//			{
//				SetFrame(6);
//			}
//			else if (animationTimer == 2)
//			{
//				SetFrame(7);
//			}
//			else if (animationTimer == 0)
//			{
//				SetFrame(8);

//				animationTimer = -1;
//			}

//			if (animationTimer > 0)
//			{
//				animationTimer--;
//			}
//		}

//        public void SetFrame(int frame)
//        {
//			for (int y = Position.Y; y <= Position.Y + 4; y++)
//			{
//				for (int x = Position.X; x <= Position.X + 2; x++)
//				{
//					Main.tile[x, y].TileFrameX = (short)((54 * frame) + 18 * (x - Position.X));
//				}
//			}
//		}
//    }
//}