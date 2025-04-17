using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
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
}