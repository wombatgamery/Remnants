using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class VaultPipe : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType <Tiles.Blocks.VaultPipe>();
		}

		//public override void AddRecipes()
		//{
		//	Recipe recipe;

		//	recipe = Recipe.Create(ModContent.ItemType<VaultPipe>());
		//	recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
		//	recipe.Register();

		//	recipe = Recipe.Create(ModContent.ItemType<VaultPlating>());
		//	recipe.AddIngredient(this);
		//	recipe.Register();
		//}
	}
}