using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	[LegacyName("gardenbrickwall")]
	public class GardenBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			WallID.Sets.AllowsPlantsToGrow[Type] = true;

			DustType = DustID.Stone;

			AddMapEntry(new Color(99, 61, 51));

            VanillaFallbackOnModDeletion = WallID.MarbleBlock;
        }
	}
}
