using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	public class HellishBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/HellishBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Ash;

			AddMapEntry(new Color(62, 49, 44));

            VanillaFallbackOnModDeletion = WallID.ObsidianBrickUnsafe;
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

            VanillaFallbackOnModDeletion = WallID.ObsidianBrick;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
