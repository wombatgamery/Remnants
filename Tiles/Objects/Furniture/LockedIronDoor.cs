using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Items.Consumable;
using Remnants.Worldgen;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Objects.Furniture
{
    [LegacyName("lockedirondoor")]
	public class LockedIronDoor : ModTile
	{
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileID.Sets.NotReallySolid[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.ClosedDoor, 0));
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

			AddMapEntry(new Color(119, 105, 79), CreateMapEntryName());

			AdjTiles = new int[] { TileID.ClosedDoor };
		}

        public override bool Slope(int i, int j)
        {
			Framing.GetTileSafely(i, j).Slope = SlopeType.Solid;
			return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}

        public override bool RightClick(int i, int j)
        {
			Player player = Main.LocalPlayer;
			if (player.ConsumeItem(ModContent.ItemType<IronKey>()))
			{
				Point16 origin = TileUtils.GetTileOrigin(i, j);

				for (int k = 0; k <= 2; k++)
				{
					Tile tile = WGTools.Tile(origin.X, origin.Y + k);
					tile.TileType = TileID.ClosedDoor;
					tile.TileFrameX = 0;
					tile.TileFrameY = (short)(15 * 54 + k * 18);

					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, origin.X, origin.Y + k);
					}

					for (int l = 0; l < 4; l++)
					{
						Dust.NewDust(new Vector2(origin.X * 16, (origin.Y + k) * 16), 16, 16, DustID.Iron);
					}
				}
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 2f, i, j);
				}

				SoundEngine.PlaySound(SoundID.Unlock, new Vector2(origin.X * 16 + 8, origin.Y * 16 + 24));
			}

			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<IronKey>();
		}

   //     public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
   //     {
			//fail = true;
   //     }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool CanExplode(int i, int j)
        {
			return false;
        }

        public override bool CreateDust(int i, int j, ref int type)
		{
			return false;
		}
	}
}