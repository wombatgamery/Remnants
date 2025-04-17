using System.Media;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items
{
	//public class PreHMToolUpgrades : GlobalItem
	//{
	//	public override void AddRecipes()
	//	{
	//		RecipeFinder

	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldHelmet); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldChainmail); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldGreaves); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }

	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldAxe); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldBow); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldBroadsword); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldShortsword); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldHammer); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.GoldPickaxe); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }

	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumHelmet); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumChainmail); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumGreaves); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }

	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumAxe); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumBow); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumBroadsword); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumShortsword); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumHammer); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }
	//		finder = new RecipeFinder(); finder.SetResult(ItemID.PlatinumPickaxe); foreach (Recipe recipefinder in finder.SearchRecipes()) { RecipeEditor editor = new RecipeEditor(recipefinder); editor.DeleteRecipe(); }

	//		ModRecipe

	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverHelmet); modrecipe.AddIngredient(ItemID.GoldBar, 10); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldHelmet); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverChainmail); modrecipe.AddIngredient(ItemID.GoldBar, 15); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldChainmail); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverGreaves); modrecipe.AddIngredient(ItemID.GoldBar, 12); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldGreaves); modrecipe.Register();

	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverAxe); modrecipe.AddIngredient(ItemID.GoldBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldAxe); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverBow); modrecipe.AddIngredient(ItemID.GoldBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldBow); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverBroadsword); modrecipe.AddIngredient(ItemID.GoldBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldBroadsword); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverShortsword); modrecipe.AddIngredient(ItemID.GoldBar, 3); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldShortsword); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverHammer); modrecipe.AddIngredient(ItemID.GoldBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldHammer); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.SilverPickaxe); modrecipe.AddIngredient(ItemID.GoldBar, 6); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.GoldPickaxe); modrecipe.Register();

	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldHelmet); modrecipe.AddIngredient(ItemID.PlatinumBar, 10); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumHelmet); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.AncientGoldHelmet); modrecipe.AddIngredient(ItemID.PlatinumBar, 10); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumHelmet); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldChainmail); modrecipe.AddIngredient(ItemID.PlatinumBar, 15); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumChainmail); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldGreaves); modrecipe.AddIngredient(ItemID.PlatinumBar, 12); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumGreaves); modrecipe.Register();

	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldAxe); modrecipe.AddIngredient(ItemID.PlatinumBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumAxe); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldBow); modrecipe.AddIngredient(ItemID.PlatinumBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumBow); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldBroadsword); modrecipe.AddIngredient(ItemID.PlatinumBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumBroadsword); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldShortsword); modrecipe.AddIngredient(ItemID.PlatinumBar, 3); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumShortsword); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldHammer); modrecipe.AddIngredient(ItemID.PlatinumBar, 4); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumHammer); modrecipe.Register();
	//		modrecipe = Recipe.Create(mod); modrecipe.AddIngredient(ItemID.GoldPickaxe); modrecipe.AddIngredient(ItemID.PlatinumBar, 6); modrecipe.AddTile(TileID.Anvils); modrecipe.SetResult(ItemID.PlatinumPickaxe); modrecipe.Register();
	//	}
	//}
}
