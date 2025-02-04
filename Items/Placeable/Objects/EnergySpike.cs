using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles;
using Terraria;
using Remnants.Items.Placeable.Blocks;

namespace Remnants.Items.Placeable.Objects
{
	public class EnergySpike : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenChair);
			Item.width = 5 * 2;
			Item.height = 13 * 2;
			Item.createTile = ModContent.TileType<Tiles.Objects.Hazards.EnergySpike>();
		}
	}
}