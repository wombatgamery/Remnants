using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls
{
	public class HellishBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Walls/HellishBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Ash;

			AddMapEntry(new Color(62, 49, 44));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}

	public class HellishBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Ash;

            AddMapEntry(new Color(62, 49, 44));
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
