using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	[LegacyName("vaultplatform")]
	public class VaultPlatform : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodPlatform);
			Item.createTile = ModContent.TileType <Tiles.Blocks.VaultPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;
			recipe = Recipe.Create(Type, 2);
			recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<VaultPlating>());
			recipe.AddIngredient(Type, 2);
			recipe.Register();
		}
	}
}