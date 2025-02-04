using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls
{
    [LegacyName("TidalSlabWallUnsafe")]
    public class MarineSlabWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Walls/MarineSlabWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Stone;

			AddMapEntry(new Color(41, 41, 69));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}

    [LegacyName("TidalSlabWall")]
    public class MarineSlabWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Stone;

            AddMapEntry(new Color(41, 41, 69));
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
