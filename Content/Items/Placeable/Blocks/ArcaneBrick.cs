using Remnants.Content.Items.Placeable.Walls;
using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	[LegacyName("labtiles")]
    [LegacyName("EnchantedBrick")]
    public class ArcaneBrick : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = ModContent.TileType<Tiles.Shimmer.EnchantedBrick>();
		}

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Hardstone>());
            recipe.AddIngredient(ItemID.ShimmerBlock);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}