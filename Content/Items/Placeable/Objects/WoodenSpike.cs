using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles.Objects;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class WoodenSpike : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenChair);
			Item.width = 16;
			Item.height = 28;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>();
		}
	}
}