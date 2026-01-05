using Remnants.Content.Items.Materials;
using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class PrototypeConduits : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = ModContent.TileType <Content.Tiles.Underworld.Prototypes.PrototypeConduits>();
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