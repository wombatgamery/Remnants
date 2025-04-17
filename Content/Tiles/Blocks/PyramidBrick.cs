using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	public class PyramidBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.AvoidedByMeteorLanding[Type] = true;

			AddMapEntry(new Color(190, 109, 73));

			MinPick = 65;
			MineResist = 2;
			DustType = DustID.Dirt;
			HitSound = SoundID.Tink;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;
	}
}