using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Biomes;
using Remnants.Content.NPCs.Monsters;
using Remnants.Content.Tiles;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects.Hazards
{
    [LegacyName("mazeguardianspawner")]
	public class LabyrinthSpawner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Height = 9;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEmazeguardianspawner>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.WaterDeath = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

            AddMapEntry(new Color(65, 74, 82), CreateMapEntryName());

            DustType = DustID.Stone;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Point16 origin = TileUtils.GetTileOrigin(i, j);
			ModContent.GetInstance<TEmazeguardianspawner>().Kill(origin.X, origin.Y);
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

  //      public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
  //      {
		//	Tile tile = Main.tile[i, j];

		//	if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		//	{
		//		Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
		//	}
		//}

  //      public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
  //      {
		//	if (!NPC.AnyNPCs(ModContent.NPCType<mazeguardian>()))
		//	{
		//		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/NPCs/enemies/mazeguardian").Value, new Vector2((i + 8) * 16 - (int)Main.screenPosition.X - 2, (j + 11) * 16 - (int)Main.screenPosition.Y - 2), new Rectangle(0, 0, 180, 180), Lighting.GetColor(i + 1, j + 4));
		//	}
		//}
    }

	public class TEmazeguardianspawner : ModTileEntity
	{
		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<LabyrinthSpawner>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				int width = 3;
				int height = 9;
				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
				return -1;
			}
			Point16 tileOrigin = new Point16(0, 0);
			return Place(i - tileOrigin.X, j - tileOrigin.Y);
		}

        public override void Update()
        {
			if (!NPC.AnyNPCs(ModContent.NPCType<Ward>()))
            {
				Vector2 tileCenter = new Vector2(Position.X + 1.5f, Position.Y + 4.5f) * 16;

                bool valid = false;
                float closestDistance = 9999;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    var player = Main.player[k];

                    if (!player.active)
                    {
                        break;
                    }
                    else if (player.InModBiome<EchoingHalls>() && !player.DeadOrGhost)
                    {
						Vector2 difference = tileCenter - player.Center;

                        if ((Math.Abs(difference.X) >= 8 * 16 || Math.Abs(difference.Y) >= 8 * 16) && Collision.CanHitLine(tileCenter, 1, 1, player.Center, 1, 1))
                        {
                            valid = true;
                        }
                    }
                    if (valid)
                    {
                        break;
                    }
                }

                if (valid)
                {
                    Spawn((int)tileCenter.X, (int)tileCenter.Y);
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
			NPC npc = NPC.NewNPCDirect(new EntitySource_SpawnNPC(), i, j, ModContent.NPCType<Ward>());

			//for (int k = 0; k < 10; k++)
   //         {
			//	Dust.NewDust(new Vector2(i, j), 8, 8, Dust.dustWater());
			//}
			if (npc.whoAmI < Main.maxNPCs)
			{
				npc.Center = new Vector2(i, j + 2);
				npc.SpawnedFromStatue = true;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
				}
			}
		}
	}
}