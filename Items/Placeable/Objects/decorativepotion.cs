//using Terraria;
//using Terraria.GameContent.Creative;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace Remnants.Items.placeable.objects
//{
//	public class decorativepotion : ModItem
//	{
//		public override void SetStaticDefaults()
//		{
//			// DisplayName.SetDefault("Dull Potion");
//			// Tooltip.SetDefault("These assorted potions are useless for drinking, but work well as decoration");

//			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
//		}

//		public override void SetDefaults()
//		{
//			Item.CloneDefaults(ItemID.Bottle);
//			Item.width = 11;
//			Item.height = 8;
//			Item.createTile = ModContent.TileType<Tiles.Objects.decorativepotion>();
//		}

//		public override void AddRecipes()
//		{
//            Recipe recipe;

//            recipe = Recipe.Create(Type);
//            recipe.AddIngredient(ItemID.BottledWater);
//            recipe.AddIngredient(ItemID.Gel);
//            recipe.AddTile(TileID.Bottles);
//            recipe.Register();
//        }
//	}
//}