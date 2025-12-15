using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
    [LegacyName("HellishPlatform")]
    public class AshenPlatform : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodPlatform);
			Item.createTile = ModContent.TileType <Tiles.Blocks.AshenPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;
			recipe = Recipe.Create(Type, 2);
			recipe.AddIngredient(ModContent.ItemType<AshenBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<AshenBrick>());
			recipe.AddIngredient(Type, 2);
			recipe.Register();
		}
	}
}