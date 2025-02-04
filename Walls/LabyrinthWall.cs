using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls
{
	[LegacyName("mazebrickwall")]
	public class LabyrinthTileWall : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(44, 47, 54));
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
			fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
	}

	[LegacyName("mazebrickwall2")]
	public class LabyrinthBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(44, 47, 54));
        }

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}
}
