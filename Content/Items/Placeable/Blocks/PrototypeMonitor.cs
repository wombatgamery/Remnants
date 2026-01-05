using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class PrototypeMonitor : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Bubble);
			Item.createTile = ModContent.TileType <Content.Tiles.Underworld.Prototypes.PrototypeMonitor>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

            recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<PrototypeComputer>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<PrototypeComputer>());
            recipe.AddIngredient(Type, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
	}
}