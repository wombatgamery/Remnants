using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls.dev
{
	public class devwall : ModWall
	{
		public override void SetStaticDefaults()
		
		{
			AddMapEntry(new Color(100, 0, 0));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}
