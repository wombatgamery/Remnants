using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Tiles
{
	public class nothing : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			DustType = 8;
			AddMapEntry(new Color(255, 0, 0));
		}

		public override bool CanExplode(int i, int j) => false;
	}
}
