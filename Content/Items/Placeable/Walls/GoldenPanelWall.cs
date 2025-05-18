//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Content.Items.Placeable.Blocks;

//namespace Remnants.Content.Items.Placeable.Walls
//{
//	public class GoldenPanelWall : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			Item.ResearchUnlockCount = 400;
//		}

//		public override void SetDefaults()
//		{
//			Item.CloneDefaults(ItemID.StoneWall);
//			Item.width = 12;
//			Item.height = 12;
//            Item.createWall = ModContent.WallType<global::Remnants.Content.Walls.GoldenPanelWall>();
//		}

//		public override void AddRecipes()
//		{
//			Recipe recipe;

//			recipe = Recipe.Create(Type, 4);
//			recipe.AddIngredient(ModContent.ItemType<GoldenPanel>());
//			recipe.Register();

//			recipe = Recipe.Create(ModContent.ItemType<GoldenPanel>());
//			recipe.AddIngredient(Type, 4);
//			recipe.Register();
//		}
//	}
//}