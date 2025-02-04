//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Tiles;
//using Remnants.Items.placeable.walls;

//namespace Remnants.Items.placeable.blocks
//{
//	public class laboratoryhull : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			// DisplayName.SetDefault("Laboratory Hull");
//		}

//		public override void SetDefaults()
//		{
//			Item.CloneDefaults(ItemID.StoneBlock);
//			Item.width = 12;
//			Item.height = 12;
//			Item.createTile = ModContent.TileType<Tiles.blocks.laboratoryhull>();
//		}

//		public override void AddRecipes()
//		{
//			Recipe recipe;

//			recipe = Recipe.Create(Type, 10);
//			recipe.AddIngredient(ItemID.SilverBar);
//			recipe.AddTile(TileID.HeavyWorkBench);
//			recipe.Register();

//            //recipe = Recipe.Create(<machinerypanel>());
//            //recipe.AddIngredient(Mod, "metalpanel");
//            //recipe.Register();

//            recipe = Recipe.Create(Type);
//			recipe.AddIngredient(ModContent.ItemType<machinerywall>(), 4);
//			recipe.Register();
//		}
//	}
//}