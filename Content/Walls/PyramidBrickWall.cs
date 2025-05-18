using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	[LegacyName("pyramidwall")]
	public class PyramidBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/PyramidBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.Sand;

			AddMapEntry(new Color(127, 75, 50));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}

	public class PyramidBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Sand;

			AddMapEntry(new Color(116, 63, 48));
		}

		public override bool CanExplode(int i, int j) => false;
	}
}
