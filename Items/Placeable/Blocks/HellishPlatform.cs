using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Items.Placeable.Blocks
{
	public class HellishPlatform : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodPlatform);
			Item.createTile = ModContent.TileType <Tiles.Blocks.HellishPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;
			recipe = Recipe.Create(Type, 2);
			recipe.AddIngredient(ModContent.ItemType<HellishBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<HellishBrick>());
			recipe.AddIngredient(Type, 2);
			recipe.Register();
		}
	}
}