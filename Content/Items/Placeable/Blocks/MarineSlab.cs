using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
    [LegacyName("TidalSlab")]
    public class MarineSlab : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GrayBrick);
            Item.createTile = ModContent.TileType <Tiles.Ocean.WaterTemple.MarineSlab>();
		}

		//public override void AddRecipes()
		//{
		//	Recipe recipe;

		//	recipe = Recipe.Create(Type, 5);
		//	recipe.AddIngredient(ItemID.StoneBlock, 10);
		//	recipe.AddIngredient(ItemID.GrassSeeds);
		//	recipe.AddTile(TileID.Furnaces);
		//	recipe.Register();
		//}
	}
}