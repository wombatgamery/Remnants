using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class Tuff : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.AshBlock;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType <Content.Tiles.Underworld.Tuff>();
		}
    }
}