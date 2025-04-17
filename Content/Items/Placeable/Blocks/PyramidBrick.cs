using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class PyramidBrick : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GrayBrick);
            Item.createTile = ModContent.TileType <Tiles.Blocks.PyramidBrick>();
		}
	}
}