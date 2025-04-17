using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;
using Remnants.Content.Items.Placeable.Walls;

namespace Remnants.Content.Items.Placeable.Blocks
{
	[LegacyName("labtiles")]
	public class EnchantedBrick : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = ModContent.TileType<Tiles.Blocks.EnchantedBrick>();
		}
	}
}