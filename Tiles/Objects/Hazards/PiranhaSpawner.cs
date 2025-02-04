using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Remnants.Tiles.Objects.Hazards
{
    [LegacyName("piranhaspawner")]
	public class PiranhaSpawner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEpiranhaspawner>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);

            AddMapEntry(new Color(75, 74, 70), CreateMapEntryName());

            DustType = 8;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Point16 origin = TileUtils.GetTileOrigin(i, j);
			ModContent.GetInstance<TEpiranhaspawner>().Kill(origin.X, origin.Y);
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

		public override bool IsTileDangerous(int i, int j, Player player)
		{
			return true;
		}
    }

	public class TEpiranhaspawner : ModTileEntity
	{
		public int spawnTimer = -1;
		public bool canSpawn;

		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<PiranhaSpawner>() && tile.TileFrameX % (18 * 3) == 0 && tile.TileFrameY == 0;
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				int width = 3;
				int height = 3;
				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
				return -1;
			}
			Point16 tileOrigin = new Point16(1, 1);
			return Place(i - tileOrigin.X, j - tileOrigin.Y);
		}

        public override void Update()
        {
			if (!NPC.downedMoonlord)
			{
                Tile tile = Main.tile[Position.X + 1, Position.Y + 2];
                if (tile.LiquidAmount < 255)
                {
                    if (tile.LiquidAmount > 255 - 15)
                    {
                        tile.LiquidAmount = 255;
                    }
                    else tile.LiquidAmount += 15;
                    NetMessage.SendData(MessageID.LiquidUpdate, number: Position.X + 1, number2: Position.Y + 2);
                }


                float closestDistance = 9999;

                Vector2 tileCenter = new Vector2(Position.X + 1.5f, Position.Y + 1.5f) * 16;

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
                            closestDistance = distance;
                            if (distance < 32 * 16)
                            {
                                break;
                            }
                        }
                    }
                }

                if (closestDistance < 32 * 16)
                {
                    if (spawnTimer <= 0)
                    {
                        if (NPC.CountNPCS(NPCID.Piranha) < 20)
                        {
                            spawnTimer = 60;
                            Spawn((int)tileCenter.X, (int)tileCenter.Y);
                        }
                    }
                    else spawnTimer--;
                }
            }
		}

        public override void OnNetPlace()
		{
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
			}
		}

		private void Spawn(int i, int j)
		{
			if (WorldGen.gen)
			{
				return;
			}

			NPC npc = NPC.NewNPCDirect(new EntitySource_SpawnNPC(), i, j, NPCID.Piranha);

			//for (int k = 0; k < 10; k++)
   //         {
			//	Dust.NewDust(new Vector2(i, j), 8, 8, Dust.dustWater());
			//}
			if (npc.whoAmI < Main.maxNPCs)
			{
				npc.SpawnedFromStatue = true;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
				}
			}
		}
	}
}