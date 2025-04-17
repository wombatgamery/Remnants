//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Content.Items.Materials
//{
//	public class coinmoldplatinum : ModItem
//	{
//		public override void SetStaticDefaults() 
//		{
//			// DisplayName.SetDefault("Platinum Coin Mold");
//			// Tooltip.SetDefault("Allows one to forge a coin with a platinum bar, but breaks after a single use\nExceedingly rare, due to its value");
//		}

//		public override void SetDefaults() 
//		{
//			Item.width = 13;
//			Item.height = 14;
//			Item.maxStack = 99;
//			Item.value = (int)(Item.sellPrice(gold: 99, silver: 82) * 0.9f);
//			Item.rare = ItemRarityID.Green;
//		}
		
//		public override void AddRecipes()
//		{
//			Recipe recipe = Recipe.Create(ItemID.PlatinumCoin);
//			recipe.AddIngredient(this);
//			recipe.AddIngredient(ItemID.PlatinumBar);
//			recipe.AddTile(TileID.Furnaces);
//			recipe.Register();
//		}
//	}
//}