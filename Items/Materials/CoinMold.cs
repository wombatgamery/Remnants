using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Items.Materials
{
	[LegacyName("coinmoldgold")]
	public class CoinMold : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults() 
		{
			Item.width = 12 * 2;
			Item.height = 14 * 2;
			Item.maxStack = 99;
			Item.value = (int)(Item.sellPrice(silver: 88) * 0.75f);
			Item.rare = ItemRarityID.Blue;
		}
		
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ItemID.GoldCoin);
			recipe.AddIngredient(this);
			recipe.AddIngredient(ItemID.GoldBar);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}