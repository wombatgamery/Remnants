using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles
{
	[LegacyName("hardstone")]
	public class Hardstone : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			MineResist = 4;
			MinPick = 110;

			DustType = DustID.Clay;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(107, 47, 47), CreateMapEntryName());
			//TileBlend(TileID.Stone);
			//         TileBlend(TileID.Mud);
			//TileBlend(TileID.ClayBlock);
			//TileBlend(TileID.Ash);

			VanillaFallbackOnModDeletion = TileID.Obsidian;
        }

		public override bool CanExplode(int i, int j) => false;

		private void TileBlend(ushort tile)
		{
			Main.tileMerge[Type][tile] = true;
			Main.tileMerge[tile][Type] = true;
		}
	}
}
