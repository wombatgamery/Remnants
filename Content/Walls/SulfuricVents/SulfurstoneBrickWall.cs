using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.SulfuricVents
{
	public class SulfurstoneBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = DustID.Stone;

            AddMapEntry(new Color(55, 57, 52));

            VanillaFallbackOnModDeletion = WallID.GrayBrick;
        }
    }
}
