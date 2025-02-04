using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Items.Materials
{
	public class SalvagedMetal : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 50;
		}

		public override void SetDefaults() 
		{
			Item.width = 11 * 2;
			Item.height = 10 * 2;
			Item.maxStack = 999;
			Item.value = 100;
			Item.rare = ItemRarityID.White;
		}
		
		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(ItemID.IronBar);
			recipe.AddIngredient(Type, 2);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}