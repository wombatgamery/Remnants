using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
    [LegacyName("TidalSlab")]
    public class MarineSlab : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            //Main.tileMerge[Type][(ushort)ModContent.TileType<MarinePlatform>()] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            MinPick = 110;
            MineResist = 3;
            DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(98, 99, 128));
		}
	}
}
