using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class Moai : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenChair);
			Item.width = 32;
			Item.height = 48;
			Item.createTile = ModContent.TileType<Tiles.Ocean.Moai>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Blocks.Tuff>(), 50);
            recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}