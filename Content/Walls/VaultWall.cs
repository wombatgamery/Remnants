using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	[LegacyName("vaultwall")]
	public class VaultWall : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Iron;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(60, 53, 67));
		}

		public override bool CanExplode(int i, int j) => false;
	}

	[LegacyName("vaultwallunsafe")]
	public class VaultWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/VaultWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.Iron;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(60, 53, 67));
		}

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = true;
		}

        public override bool CanExplode(int i, int j) => false;
    }
}
