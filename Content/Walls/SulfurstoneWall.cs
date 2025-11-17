using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	public class SulfurstoneWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			DustType = DustID.Stone;

            AddMapEntry(new Color(55, 57, 52));

            VanillaFallbackOnModDeletion = WallID.LavaUnsafe1;
        }
    }
}
