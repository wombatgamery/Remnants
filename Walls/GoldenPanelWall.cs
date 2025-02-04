using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls
{
	public class GoldenPanelWall : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Gold;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(128, 79, 51));
		}
	}

	public class GoldenPanelWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Walls/GoldenPanelWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Gold;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(128, 79, 51));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

		public override bool CanExplode(int i, int j) => false;
	}
}
