//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Content.Tiles;

//namespace Remnants.Content.Items.Placeable.Blocks
//{
//	public class GoldenPanel : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			Item.ResearchUnlockCount = 100;
//		}

//		public override void SetDefaults()
//		{
//			Item.CloneDefaults(ItemID.StoneBlock);
//			Item.width = 8;
//			Item.height = 8;
//            Item.createTile = ModContent.TileType<Tiles.Blocks.GoldenPanel>();
//		}

//		public override void AddRecipes()
//		{
//			Recipe recipe;

//			recipe = Recipe.Create(ItemID.GoldenPlatform, 2);
//			recipe.AddIngredient(Type);
//			recipe.Register();

//			recipe = Recipe.Create(Type);
//			recipe.AddIngredient(ItemID.GoldenPlatform, 2);
//			recipe.Register();
//		}
//	}
//}