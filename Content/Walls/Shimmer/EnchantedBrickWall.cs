using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Shimmer
{
	public class EnchantedBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(71, 65, 99));

            VanillaFallbackOnModDeletion = WallID.VioletMossBlockWall;
        }
	}

	[LegacyName("labtilewallunsafe")]
	public class EnchantedBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/Shimmer/EnchantedBrickWall";

		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.Stone;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(71, 65, 99));

            VanillaFallbackOnModDeletion = WallID.VioletMossBlockWall;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
			fail = true;
        }

        public override bool CanExplode(int i, int j) => false;
	}
}
