using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.World;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects.Hazards
{
	[LegacyName("hangingbat")]
	public class HangingBat : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileWaterDeath[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.CoordinateWidth = 28;
			TileObjectData.newTile.CoordinateHeights = new[] { 30 };
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			HitSound = SoundID.Zombie15;
		}

        public override void NearbyEffects(int i, int j, bool closer)
        {
            float closestDistance = 9999;
            Player closestPlayer = Main.player[0];

            Vector2 tileCenter = new Vector2(i + 0.5f, j + 0.5f) * 16;

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                var player = Main.player[k];

                if (!player.active)
                {
                    break;
                }
                else if (!player.DeadOrGhost)
                {
                    float distance = Vector2.Distance(tileCenter, player.Center);
                    if (distance < closestDistance)
                    {
                        closestPlayer = player;
                        closestDistance = distance;
                        if (distance < 16 * 16)
                        {
                            break;
                        }
                    }
                }
            }

            if (closestDistance < 16 * 16 && Collision.CanHit(new Vector2(i * 16, j * 16), 16, 16, closestPlayer.position, closestPlayer.width, closestPlayer.height))
            {
                WorldGen.KillTile(i, j);
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
			Spawn(i, j);
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			Tile tile = Main.tile[i, j];

			if (tile.TileFrameX == 30 * 2 || tile.TileFrameX == 30 * 6)
			{
				int num202 = Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 6, 0, 0, 100, default, 2f);
				Main.dust[num202].noGravity = true;
			}
			else if (tile.TileFrameX == 30 * 5 && Main.rand.NextBool(10))
			{
				int num203 = Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 67, 0, 0, 90, default, 1.5f);
				Main.dust[num203].noGravity = true;
				Main.dust[num203].noLight = true;
			}
			else if (tile.TileFrameX == 30 * 8 && Main.rand.NextBool(10))
			{
				int num11 = Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 165, 0, 0, 50);
				Main.dust[num11].noGravity = true;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 30 * 4)
			{
				r = 0.3f;
				g = 0f;
				b = 0.2f;
			}
			else if (tile.TileFrameX == 30 * 8)
			{
				float num3 = Main.rand.Next(28, 42) * 0.005f;
				num3 += (270 - Main.mouseTextColor) / 500f;
				float num4 = 0.1f;
				float num5 = 0.3f + num3 / 2f;
				float num6 = 0.6f + num3;
				float num7 = 0.65f;

				num4 *= num7;
				num5 *= num7;
				num6 *= num7;
				r = num4;
				g = num5;
				b = num6;
			}
		}

		public override void RandomUpdate(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (Vector2.Distance(new Vector2(i * 16 + 8, j * 16 + 8), player.Center) > 80 * 16)
			{
				//ModContent.GetInstance<TEhangingbat>().Kill(i, j);
				Framing.GetTileSafely(i, j).HasTile = false;
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];

			if (tile.TileFrameX == 30 * 4)
			{
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen)
				{
					zero = Vector2.Zero;
				}

				SpriteEffects effect = i % 2 == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/Hazards/HangingBat").Value, new Vector2(i * 16 - (int)Main.screenPosition.X - 6, j * 16 - (int)Main.screenPosition.Y - 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 32, 28, 30), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
			}
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		public override bool IsTileDangerous(int i, int j, Player player)
		{
			return true;
		}

		public override bool KillSound(int i, int j, bool fail)
		{
			return !fail;
		}
		public override bool CreateDust(int i, int j, ref int type)
		{
			return false;
		}

		public void Spawn(int i, int j)
		{
			if (WorldGen.gen)
			{
				return;
			}

			#region type
			Tile tile = Main.tile[i, j];

			int type = NPCID.CaveBat;
			int yOffset = 0;

			if (tile.TileFrameX == 30)
			{
				type = NPCID.JungleBat;
			}
			else if (tile.TileFrameX == 30 * 2)
			{
				type = NPCID.Hellbat;
			}
			else if (tile.TileFrameX == 30 * 3)
			{
				type = NPCID.GiantBat;
			}
			else if (tile.TileFrameX == 30 * 4)
			{
				type = NPCID.IlluminantBat;
			}
			else if (tile.TileFrameX == 30 * 5)
			{
				type = NPCID.IceBat;
			}
			else if (tile.TileFrameX == 30 * 6)
			{
				type = NPCID.Lavabat;
			}
			else if (tile.TileFrameX == 30 * 7)
			{
				type = NPCID.GiantFlyingFox;
			}
			else if (tile.TileFrameX == 30 * 8)
			{
				type = NPCID.SporeBat;
			}
			#endregion

			NPC npc = NPC.NewNPCDirect(new EntitySource_SpawnNPC(), i * 16 + 8, j * 16 + yOffset + 8, type, ai1: 1);
			if (npc.whoAmI < Main.maxNPCs)
			{
				npc.position.Y = j * 16;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
				}
			}

			//Kill(i, j);
		}
	}

    public class TEhangingbat : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.TileType == ModContent.TileType<HangingBat>();
        }

        public override void OnNetPlace()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public override void Update()
        {
			//float closestDistance = 9999;
			//Player closestPlayer = Main.player[0];

			//Vector2 tileCenter = new Vector2(Position.X + 0.5f, Position.Y + 0.5f) * 16;

			//for (int k = 0; k < Main.maxPlayers; k++)
			//{
			//    var player = Main.player[k];

			//    if (!player.active)
			//    {
			//        break;
			//    }
			//    else if (!player.DeadOrGhost)
			//    {
			//        float distance = Vector2.Distance(tileCenter, player.Center);
			//        if (distance < closestDistance)
			//        {
			//            closestPlayer = player;
			//            closestDistance = distance;
			//            if (distance < 16 * 16)
			//            {
			//                break;
			//            }
			//        }
			//    }
			//}

			//if (closestDistance < 16 * 16 && Collision.CanHit(new Vector2(Position.X * 16, Position.Y * 16), 16, 16, closestPlayer.position, closestPlayer.width, closestPlayer.height))
			//{
			//    WorldGen.KillTile(Position.X, Position.Y);
			//}
			Kill(Position.X, Position.Y);
        }

        public void Spawn(int i, int j)
        {
            if (WorldGen.gen)
            {
                return;
            }

            #region type
            Tile tile = Main.tile[i, j];

            int type = NPCID.CaveBat;
            int yOffset = 0;

            if (tile.TileFrameX == 30)
            {
                type = NPCID.JungleBat;
            }
            else if (tile.TileFrameX == 30 * 2)
            {
                type = NPCID.Hellbat;
            }
            else if (tile.TileFrameX == 30 * 3)
            {
                type = NPCID.GiantBat;
            }
            else if (tile.TileFrameX == 30 * 4)
            {
                type = NPCID.IlluminantBat;
            }
            else if (tile.TileFrameX == 30 * 5)
            {
                type = NPCID.IceBat;
            }
            else if (tile.TileFrameX == 30 * 6)
            {
                type = NPCID.Lavabat;
            }
            else if (tile.TileFrameX == 30 * 7)
            {
                type = NPCID.GiantFlyingFox;
            }
            else if (tile.TileFrameX == 30 * 8)
            {
                type = NPCID.SporeBat;
            }
            #endregion

            NPC npc = NPC.NewNPCDirect(new EntitySource_SpawnNPC(), i * 16 + 8, j * 16 + yOffset + 8, type, ai1: 1);
            if (npc.whoAmI < Main.maxNPCs)
            {
                npc.position.Y = j * 16;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                }
            }

            Kill(i, j);
        }
    }

    public class HangingBatSpawns : GlobalWall
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j];

            //if (j > Main.worldSurface)
            //         {
            //	TryToPlaceGuardian(i, j);
            //}

            if (ModContent.GetInstance<Gameplay>().HangingBats && Main.netMode == NetmodeID.SinglePlayer)
            {
                int[] badWalls = new int[5];

                //badWalls[0] = ModContent.WallType<temple>();
                //badWalls[1] = WallID.LihzahrdBrickUnsafe;
                //badWalls[2] = ModContent.WallType<pyramid>();
                //badWalls[3] = ModContent.WallType<PyramidBrickWallUnsafe>();
                //badWalls[4] = ModContent.WallType<whisperingmaze>();
                //badWalls[5] = ModContent.WallType<LabyrinthTileWall>();
                //badWalls[6] = ModContent.WallType<LabyrinthBrickWall>();
                //badWalls[7] = ModContent.WallType<magicallab>();
                //badWalls[8] = ModContent.WallType<EnchantedBrickWallUnsafe>();
                badWalls[0] = WallID.SpiderUnsafe;
                badWalls[1] = WallID.LivingWoodUnsafe;
                badWalls[2] = WallID.HiveUnsafe;
                badWalls[3] = WallID.CorruptionUnsafe3;
                badWalls[4] = WallID.CrimsonUnsafe3;

                Player player = Main.LocalPlayer;
                if (Main.rand.NextBool(50) && !tile.HasTile && MiscTools.Solid(i, j - 1) && !MiscTools.Solid(i, j + 1) && tile.LiquidAmount == 0 && Main.tileOreFinderPriority[Main.tile[i, j - 1].TileType] == 0 && Vector2.Distance(new Vector2(i * 16 + 8, j * 16 + 8), player.Center) > 80 * 16)
                {
                    if (!Main.wallHouse[tile.WallType] && tile.WallType < WallID.Count)
                    {
                        //tile.WallType == 0 || tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe || tile.WallType == WallID.DirtUnsafe1 || tile.WallType == WallID.DirtUnsafe2 || tile.WallType == WallID.DirtUnsafe3 || tile.WallType == WallID.DirtUnsafe4 || tile.WallType == WallID.SnowWallUnsafe || tile.WallType == WallID.IceUnsafe || tile.WallType == WallID.JungleUnsafe || tile.WallType == WallID.MudUnsafe || tile.WallType == WallID.JungleUnsafe1 || tile.WallType == WallID.JungleUnsafe2 || tile.WallType == WallID.JungleUnsafe3 || tile.WallType == WallID.JungleUnsafe4 || tile.WallType == WallID.LavaUnsafe1 || tile.WallType == WallID.LavaUnsafe2 || tile.WallType == WallID.LavaUnsafe3 || tile.WallType == WallID.LavaUnsafe4 || tile.WallType == WallID.ObsidianBackUnsafe)
                        // && tile.WallType != ModContent.WallType<vault>() && tile.WallType != ModContent.WallType<vaultwallunsafe>()
                        if (!Main.wallDungeon[tile.WallType] && !badWalls.Contains(tile.WallType) && !WallID.Sets.Conversion.Sandstone[tile.WallType] && !WallID.Sets.Conversion.HardenedSand[tile.WallType] && tile.WallType != WallID.DesertFossil)
                        {
                            tile = Main.tile[i, j - 1];
                            int style = -1;

                            if (j > Main.maxTilesY - 300)
                            {
                                if (Main.hardMode)
                                {
                                    style = 6;
                                }
                                else style = 2;
                            }
                            else if (j > Main.worldSurface)
                            {
                                if (TileID.Sets.Hallow[tile.TileType])
                                {
                                    style = 4;
                                }
                                else if (tile.TileType == TileID.JungleGrass || tile.TileType == TileID.LihzahrdBrick || tile.TileType == TileID.Hive || tile.TileType == TileID.Chlorophyte || tile.TileType == TileID.RichMahogany || tile.TileType == TileID.LivingMahogany || tile.TileType == TileID.LivingMahoganyLeaves)
                                {
                                    if (Main.hardMode)
                                    {
                                        style = 7;
                                    }
                                    else style = 1;
                                }
                                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBrick || tile.TileType == TileID.IceBrick || tile.TileType == TileID.BorealWood)
                                {
                                    style = 5;
                                }
                                else if (tile.TileType == TileID.MushroomGrass || tile.TileType == TileID.MushroomBlock)
                                {
                                    style = 8;
                                }
                                else if (!TileID.Sets.Corrupt[tile.TileType] && !TileID.Sets.Crimson[tile.TileType])
                                {
                                    if (Main.hardMode)
                                    {
                                        style = 3;
                                    }
                                    else style = 0;
                                }
                            }

                            if (style >= 0)
                            {
                                WorldGen.PlaceTile(i, j, ModContent.TileType<HangingBat>(), true, style: style);
                                ModContent.GetInstance<TEhangingbat>().Place(i, j);
                            }
                        }
                    }
                }
            }
        }
    }
}