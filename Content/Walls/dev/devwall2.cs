using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.dev
{
	public class devwall2 : ModWall
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
