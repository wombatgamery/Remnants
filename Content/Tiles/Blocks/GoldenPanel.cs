using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	public class GoldenPanel : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileBlendAll[Type] = true;

			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			DustType = DustID.Gold;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(255, 237, 101));
		}
	}
}
