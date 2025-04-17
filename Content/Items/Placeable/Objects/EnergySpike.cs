using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;
using Terraria;
using Remnants.Content.Items.Placeable.Blocks;

namespace Remnants.Content.Items.Placeable.Objects
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