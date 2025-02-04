//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Tiles;

//namespace Remnants.Items.Placeable.Blocks
//{
//	public class hardstonebrick : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			// DisplayName.SetDefault("Hardstone Brick");
//		}

//		public override void SetDefaults()
//		{
//			Item.CloneDefaults(ItemID.StoneBlock);
//			Item.width = 8;
//			Item.height = 8;
//            Item.createTile = ModContent.TileType<Tiles.Blocks.hardstonebrick>();
//		}

//		//public override void AddRecipes()
//		//{
//		//	Recipe recipe = Recipe.Create(ModContent.ItemType<hardstonetiles>(), 4);
//		//	recipe.AddIngredient(this, 4);
//		//	recipe.Register();
//		//}
//	}
//}