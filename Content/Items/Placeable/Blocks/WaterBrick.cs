using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class WaterBrick : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType<Tiles.Blocks.MarineSlab>();
		}
	}
}