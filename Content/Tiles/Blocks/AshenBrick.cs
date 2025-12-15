using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
    [LegacyName("hardstonebrick")]
    [LegacyName("HellishBrick")]
    public class AshenBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            //Main.tileMerge[Type][TileID.Ash] = true;
            //Main.tileMerge[TileID.Ash][Type] = true;
            //Main.tileMerge[Type][TileID.AshWood] = true;
            //Main.tileMerge[TileID.AshWood][Type] = true;
            //Main.tileMerge[Type][TileID.Hellstone] = true;
            //Main.tileMerge[TileID.Hellstone][Type] = true;
            //Main.tileMerge[Type][TileID.HellstoneBrick] = true;
            //Main.tileMerge[TileID.HellstoneBrick][Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			MinPick = 110;

			DustType = DustID.Ash;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(98, 79, 86));

            VanillaFallbackOnModDeletion = TileID.ObsidianBrick;
        }

		public override bool CanExplode(int i, int j) => false;
	}
}
