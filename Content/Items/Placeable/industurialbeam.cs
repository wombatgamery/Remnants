//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Content.Tiles;

//namespace Remnants.Content.Items.Placeable
//{
//	public class industurialbeam : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			// DisplayName.SetDefault("Factory Beam"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
//		}

//		public override void SetDefaults()
//		{
//			Item.width = 12;
//			Item.height = 12;
//			Item.maxStack = 999;
//			Item.useTurn = true;
//			Item.autoReuse = true;
//			Item.useAnimation = 15;
//			Item.useTime = 10;
//			Item.useStyle = ItemUseStyleID.Swing;
//			Item.consumable = true;
//			Item.createTile = ModContent.TileType <industurialbeamtile>();
//		}

//		//public override void AddRecipes()
//		//{
//		//	recipe = Recipe.Create(mod);
//		//	recipe.AddIngredient(mod, "industurialwall", 4);
//		//	recipe.AddTile(TileID.WorkBenches);
//		//	recipe.SetResult(this);
//		//	recipe.Register();

//		//	recipe = Recipe.Create(mod);
//		//	recipe.AddIngredient(mod, "industurialplatform", 2);
//		//	recipe.SetResult(this);
//		//	recipe.Register();
//		//}
//	}
//}