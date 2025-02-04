using Microsoft.Xna.Framework;
using Remnants.Tiles.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Tiles.Blocks
{
	public class TombBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.AvoidedByMeteorLanding[Type] = true;

			AddMapEntry(new Color(77, 76, 70));

			MinPick = 100;
			MineResist = 2;
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;
	}
}