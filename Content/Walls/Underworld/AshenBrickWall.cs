using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Underworld
{
    [LegacyName("HellishBrickWallUnsafe")]
    public class AshenBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/Underworld/AshenBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Ash;

			AddMapEntry(new Color(53, 43, 46));

            VanillaFallbackOnModDeletion = WallID.ObsidianBrickUnsafe;
        }

		public override void KillWall(int i, int j, ref bool fail)
		{
			if (!Main.hardMode)
			{
                fail = true;
            }
		}

		public override bool CanExplode(int i, int j) => false;
	}

    [LegacyName("HellishBrickWall")]
    public class AshenBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Ash;

            AddMapEntry(new Color(53, 43, 46));

            VanillaFallbackOnModDeletion = WallID.ObsidianBrick;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
