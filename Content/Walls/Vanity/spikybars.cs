using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Vanity
{
	public class spikybars : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = false;
			Main.wallLight[Type] = true;
			HitSound = SoundID.Tink;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}
