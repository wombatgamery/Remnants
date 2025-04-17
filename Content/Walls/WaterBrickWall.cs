using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	public class WaterBrickWall : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = true;
			Main.wallBlend[Type] = 1;

			DustType = DustID.BeachShell;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(81, 77, 102));
		}
	}

	public class WaterBrickWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/WaterBrickWall";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallBlend[Type] = 1;

			DustType = DustID.BeachShell;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(81, 77, 102));
		}
	}
}
