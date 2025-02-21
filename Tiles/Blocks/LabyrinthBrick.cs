using Microsoft.Xna.Framework;
using Remnants.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Tiles.Blocks
{
	[LegacyName("mazebrick")]
	public class LabyrinthBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileBlendAll[Type] = true;

			TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            MineResist = 4;
			MinPick = 9999;
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(72, 79, 86));
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

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

				tile = Main.tile[i, j + 1];
				if (!tile.HasTile)
				{
					WorldGen.PlaceTile(i, j + 1, ModContent.TileType<LabyrinthVine>(), true);
					tile.TileFrameX = (short)(Main.rand.Next(3) * 18);
                }
			}
        }
    }
}
