using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.AerialGarden
{
	[LegacyName("gardenbrick")]
	public class GardenBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileMerge[Type][TileID.Grass] = true;
			Main.tileMerge[TileID.Grass][Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.AvoidedByMeteorLanding[Type] = true;

			MinPick = 65;
			MineResist = 2;
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(196, 141, 111));

            VanillaFallbackOnModDeletion = TileID.MarbleBlock;
        }

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;
	}
}