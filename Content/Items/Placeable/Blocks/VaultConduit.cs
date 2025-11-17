using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;
using Remnants.Content.Items.Materials;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class VaultConduit : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = ModContent.TileType <Tiles.Blocks.VaultConduit>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

            recipe = Recipe.Create(Type, 25);
            recipe.AddIngredient(ModContent.ItemType<HeavyAlloy>());
            recipe.AddIngredient(ItemID.CopperBar);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.Register();
		}
	}
}