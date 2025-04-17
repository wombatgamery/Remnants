using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Content.Tiles;

namespace Remnants.Content.Tiles.Objects
{
    [LegacyName("sarcophagus")]
	public class Sarcophagus : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.StyleWrapLimit = 5;

			//TileObjectData.newTile.UsesCustomCanPlace = true;
			//TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEsarcophagus>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(128, 78, 50), CreateMapEntryName());

			DustType = DustID.Dirt;
		}

        //      public override void NearbyEffects(int i, int j, bool closer)
        //      {
        //	Tile tile = Main.tile[i, j];
        //	if (tile.TileFrameX == 36 * 4 && tile.TileFrameY == 0)
        //          {
        //		float closestDistance = 9999;

        //		Vector2 tileCenter = new Vector2(i + 1, j + 2) * 16;

        //		for (int k = 0; k < Main.maxPlayers; k++)
        //		{
        //			var player = Main.player[k];

        //			if (!player.active)
        //			{
        //				break;
        //			}
        //			else if (!player.DeadOrGhost)
        //			{
        //				float distance = Vector2.Distance(tileCenter, player.Center);
        //				if (distance < closestDistance)
        //				{
        //					closestDistance = distance;
        //					if (distance < 8 * 16)
        //					{
        //						break;
        //					}
        //				}
        //			}
        //		}

        //		if (closestDistance < 8 * 16)
        //		{
        //			ModContent.GetInstance<TEsarcophagus>().Kill(i, j);

        //			SetFrame(i, j, 3);
        //		}
        //	}
        //}

        public override bool RightClick(int i, int j)
        {
			Tile tile = Main.tile[i, j];

			if (tile.TileFrameX < 32 * 3)
            {
				int x = i - tile.TileFrameX / 16 % 2;
				int y = j - tile.TileFrameY / 16;

				for (int b = y; b <= y + 3; b++)
                {
					for (int a = x; a <= x + 1; a++)
					{
						tile = Main.tile[a, b];
						tile.TileFrameX = (short)(tile.TileFrameX % 32 + 32 * 3);
					}
				}
				SoundEngine.PlaySound(SoundID.NPCHit42, new Vector2(x + 1, y + 2) * 16);

				for (int k = 0; k < 20; k++)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(x, y) * 16, 32, 64, DustID.Dirt);
				}

				return true;
			}
			else return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Point16 origin = TileUtils.GetTileOrigin(i, j);
			ModContent.GetInstance<TEsarcophagus>().Kill(origin.X, origin.Y);
		}

		//public override bool IsTileDangerous(int i, int j, Player player)
		//{
		//	Tile tile = Main.tile[i, j];
		//	return tile.TileFrameX >= 32 * 4;
		//}
	}

	public class TEsarcophagus : ModTileEntity
	{
		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return false;// tile.HasTile && tile.TileType == ModContent.TileType<Sarcophagus>() && tile.TileFrameX == 32 * 4 && tile.TileFrameY == 0;
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				int width = 2;
				int height = 4;
				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
			}
			Point16 tileOrigin = new Point16(0, 3);
			return Place(i - tileOrigin.X, j - tileOrigin.Y);
		}

		public override void OnNetPlace()
		{
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
			}
		}

        public override void Update()
        {
			float closestDistance = 9999;

			Vector2 tileCenter = new Vector2(Position.X + 1, Position.Y + 2) * 16;

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
						if (distance < 8 * 16)
						{
							break;
						}
					}
				}
			}

			if (closestDistance < 8 * 16)
			{
				ModContent.GetInstance<TEsarcophagus>().Kill(Position.X, Position.Y);

				SetFrame(Position.X, Position.Y, 3);
			}
		}

        public override void OnKill()
        {
			SpawnMummy();
		}

        private void SpawnMummy()
		{
			if (WorldGen.gen)
			{
				return;
			}

			int i = Position.X;
			int j = Position.Y;

			NPC npc = NPC.NewNPCDirect(new EntitySource_SpawnNPC(), (i + 1) * 16, j * 16, NPCID.Mummy);
			if (npc.whoAmI < Main.maxNPCs)
			{
				//npc.color = new Color(255, 200, 150);
				//npc.GivenName = "Frail Mummy";
				npc.lifeMax /= 2;
				npc.life /= 2;
				npc.defense /= 2;
				npc.damage /= 2;

				npc.position.Y = (j + 4) * 16 - npc.height - 4;
				npc.SpawnedFromStatue = true;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
				}
			}
		}

		public void SetFrame(int i, int j, int frame)
		{
			for (int y = j; y <= j + 3; y++)
			{
				for (int x = i; x <= i + 1; x++)
				{
					Main.tile[x, y].TileFrameX = (short)(32 * frame + 16 * (x - i));
				}
			}
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendTileSquare(-1, i, j, 2, 4, TileChangeType.None);
			}
		}
	}
}