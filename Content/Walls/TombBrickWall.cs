using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	public class TombBrickWallUnsafe : ModWall
	{
		//public override string Texture => "Remnants/Content/Walls/TombBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Stone;

			AddMapEntry(new Color(56, 55, 56));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}

	public class TombBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Stone;

			AddMapEntry(new Color(56, 55, 56));
		}
	}
}
