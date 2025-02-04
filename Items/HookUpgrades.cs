//using System.Media;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Items
//{
//	public class HookUpgrades : GlobalItem
//	{
//		public override void AddRecipes() 
//		{
//			RecipeFinder finder = new RecipeFinder();
//			finder.SetResult(ItemID.AmethystHook);
//			foreach (Recipe recipefinder in finder.SearchRecipes())
//			{
//				RecipeEditor editor = new RecipeEditor(recipefinder);
//				editor.DeleteRecipe();
//			}
//			finder = new RecipeFinder();
//			finder.SetResult(ItemID.TopazHook);
//			foreach (Recipe recipefinder in finder.SearchRecipes())
//			{
//				RecipeEditor editor = new RecipeEditor(recipefinder);
//				editor.DeleteRecipe();
//			}
//			finder = new RecipeFinder();
//			finder.SetResult(ItemID.SapphireHook);
//			foreach (Recipe recipefinder in finder.SearchRecipes())
//			{
//				RecipeEditor editor = new RecipeEditor(recipefinder);
//				editor.DeleteRecipe();
//			}
//			finder = new RecipeFinder();
//			finder.SetResult(ItemID.EmeraldHook);
//			foreach (Recipe recipefinder in finder.SearchRecipes())
//			{
//				RecipeEditor editor = new RecipeEditor(recipefinder);
//				editor.DeleteRecipe();
//			}
//			finder = new RecipeFinder();
//			finder.SetResult(ItemID.RubyHook);
//			foreach (Recipe recipefinder in finder.SearchRecipes())
//			{
//				RecipeEditor editor = new RecipeEditor(recipefinder);
//				editor.DeleteRecipe();
//			}
//			finder = new RecipeFinder();
//			finder.SetResult(ItemID.DiamondHook);
//			foreach (Recipe recipefinder in finder.SearchRecipes())
//			{
//				RecipeEditor editor = new RecipeEditor(recipefinder);
//				editor.DeleteRecipe();
//			}

//			ModRecipe modrecipe = Recipe.Create(mod);
//			modrecipe.AddIngredient(ItemID.GrapplingHook);
//			modrecipe.AddIngredient(ItemID.Amethyst, 10);
//			modrecipe.AddTile(TileID.Anvils);
//			modrecipe.SetResult(ItemID.AmethystHook);
//			modrecipe.Register();
//			modrecipe = Recipe.Create(mod);
//			modrecipe.AddIngredient(ItemID.GrapplingHook);
//			modrecipe.AddIngredient(ItemID.Topaz, 10);
//			modrecipe.AddTile(TileID.Anvils);
//			modrecipe.SetResult(ItemID.TopazHook);
//			modrecipe.Register();
//			modrecipe = Recipe.Create(mod);
//			modrecipe.AddIngredient(ItemID.GrapplingHook);
//			modrecipe.AddIngredient(ItemID.Sapphire, 10);
//			modrecipe.AddTile(TileID.Anvils);
//			modrecipe.SetResult(ItemID.SapphireHook);
//			modrecipe.Register();
//			modrecipe = Recipe.Create(mod);
//			modrecipe.AddIngredient(ItemID.GrapplingHook);
//			modrecipe.AddIngredient(ItemID.Emerald, 10);
//			modrecipe.AddTile(TileID.Anvils);
//			modrecipe.SetResult(ItemID.EmeraldHook);
//			modrecipe.Register();
//			modrecipe = Recipe.Create(mod);
//			modrecipe.AddIngredient(ItemID.GrapplingHook);
//			modrecipe.AddIngredient(ItemID.Ruby, 10);
//			modrecipe.AddTile(TileID.Anvils);
//			modrecipe.SetResult(ItemID.RubyHook);
//			modrecipe.Register();
//			modrecipe = Recipe.Create(mod);
//			modrecipe.AddIngredient(ItemID.GrapplingHook);
//			modrecipe.AddIngredient(ItemID.Diamond, 10);
//			modrecipe.AddTile(TileID.Anvils);
//			modrecipe.SetResult(ItemID.DiamondHook);
//			modrecipe.Register();
//		}
//	}
//}