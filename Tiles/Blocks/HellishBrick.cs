using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Tiles.Blocks
{
    [LegacyName("hardstonebrick")]
    public class HellishBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            //Main.tileBlendAll[Type] = true;

            Main.tileMerge[Type][TileID.Ash] = true;
            Main.tileMerge[TileID.Ash][Type] = true;
            Main.tileMerge[Type][TileID.Hellstone] = true;
            Main.tileMerge[TileID.Hellstone][Type] = true;
            Main.tileMerge[Type][TileID.Obsidian] = true;
            Main.tileMerge[TileID.Obsidian][Type] = true;

            Main.tileMerge[TileID.Spikes][Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			MinPick = 110;

			DustType = DustID.Ash;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(99, 82, 75));
		}

		public override bool CanExplode(int i, int j) => false;
	}
}
