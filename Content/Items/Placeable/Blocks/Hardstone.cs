using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	[LegacyName("hardstone")]
	public class Hardstone : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.width = 8;
			Item.height = 8;
            Item.createTile = ModContent.TileType<Tiles.Hardstone>();
		}
	}
}