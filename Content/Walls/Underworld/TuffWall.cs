using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Underworld
{
	public class TuffWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			DustType = DustID.Ash;

            AddMapEntry(new Color(44, 36, 40));

            VanillaFallbackOnModDeletion = WallID.LavaUnsafe1;
        }
    }
}
