using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;
using Terraria;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class HellishBrick : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GrayBrick);
            Item.createTile = ModContent.TileType <Tiles.Blocks.HellishBrick>();
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