using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls
{
	public class hardstonewall : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = false;

			DustType = 8;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(56, 49, 54));
		}

		public override bool CanExplode(int i, int j) => false;
	}
}
