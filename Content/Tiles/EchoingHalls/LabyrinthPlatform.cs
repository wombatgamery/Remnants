using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.EchoingHalls
{
	[LegacyName("mazeplatform")]
	public class LabyrinthPlatform : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileTable[Type] = true;

			TileID.Sets.Platforms[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true;

            TileObjectData.newTile.FullCopyFrom(TileID.Platforms);
			TileObjectData.newTile.WaterDeath = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

			AddMapEntry(new Color(104, 120, 127));
			//AddMapEntry(new Color(191, 142, 111));

			MineResist = 4;
			MinPick = 9999;
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			AdjTiles = new int[] { TileID.Platforms };
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j) => false;

		public override void RandomUpdate(int i, int j)
		{
			if (Main.rand.NextBool(5))
			{
				Tile tile = Main.tile[i, j - 1];
				if (!tile.HasTile)
				{
					WorldGen.PlaceTile(i, j - 1, ModContent.TileType<LabyrinthGrass>(), true, style: Main.rand.Next(6));
				}
			}
		}
	}
}