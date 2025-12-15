using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class GardenPlatform : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodPlatform);
			Item.createTile = ModContent.TileType <Tiles.Blocks.GardenPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;
			recipe = Recipe.Create(Type, 2);
			recipe.AddIngredient(ModContent.ItemType<GardenBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<GardenBrick>());
			recipe.AddIngredient(Type, 2);
			recipe.Register();
		}
	}
}