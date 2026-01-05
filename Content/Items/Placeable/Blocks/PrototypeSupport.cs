using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class PrototypeSupport : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenBeam);
			Item.createTile = ModContent.TileType <Content.Tiles.Underworld.Prototypes.PrototypeSupport>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

            recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<PrototypePlating>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<PrototypePlating>());
            recipe.AddIngredient(Type, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
	}
}