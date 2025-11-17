using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class VaultSupport : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenBeam);
			Item.createTile = ModContent.TileType <Tiles.Blocks.VaultSupport>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

            recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<VaultPlating>());
            recipe.AddIngredient(Type, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
	}
}