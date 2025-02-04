using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles;
using Remnants.Tiles.Blocks;

namespace Remnants.Items.Placeable.Blocks
{
	[LegacyName("poisonrock")]
	public class ToxicWaste : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.width = 8;
			Item.height = 8;
			Item.createTile = ModContent.TileType <Tiles.Blocks.ToxicWaste>();
		}
	}
}