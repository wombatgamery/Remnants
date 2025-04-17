using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class MarinePlatform : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodPlatform);
			Item.createTile = ModContent.TileType <Tiles.Blocks.MarinePlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;
			recipe = Recipe.Create(Type, 2);
			recipe.AddIngredient(ModContent.ItemType<MarineSlab>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<MarineSlab>());
			recipe.AddIngredient(Type, 2);
			recipe.Register();
		}
	}
}