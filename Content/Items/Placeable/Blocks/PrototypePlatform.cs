using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	[LegacyName("vaultplatform")]
	public class PrototypePlatform : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodPlatform);
			Item.createTile = ModContent.TileType <Content.Tiles.Underworld.Prototypes.PrototypePlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;
			recipe = Recipe.Create(Type, 2);
			recipe.AddIngredient(ModContent.ItemType<PrototypePlating>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<PrototypePlating>());
			recipe.AddIngredient(Type, 2);
			recipe.Register();
		}
	}
}