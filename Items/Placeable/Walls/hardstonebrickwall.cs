//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Walls;
//using Remnants.Items.Placeable.Blocks;

//namespace Remnants.Items.Placeable.Walls
//{
//	public class hardstonebrickwall : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			// DisplayName.SetDefault("Hardstone Brick Wall"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
//		}

//		public override void SetDefaults()
//		{
//			Item.CloneDefaults(ItemID.StoneWall);
//			Item.width = 12;
//			Item.height = 12;
//            Item.createWall = ModContent.WallType<global::Remnants.Walls.hardstonebrickwall>();
//		}

//		public override void AddRecipes()
//		{
//			Recipe recipe;

//			recipe = Recipe.Create(Type, 4);
//			recipe.AddIngredient(ModContent.ItemType<hardstonebrick>());
//			recipe.Register();

//			recipe = Recipe.Create(ModContent.ItemType<hardstonebrick>());
//			recipe.AddIngredient(this, 4);
//			recipe.Register();
//		}
//	}
//}