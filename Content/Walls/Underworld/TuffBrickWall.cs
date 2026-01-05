using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Underworld
{
	public class TuffBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Ash;

            AddMapEntry(new Color(44, 36, 40));

            VanillaFallbackOnModDeletion = WallID.IridescentBrick;
        }
    }
}
