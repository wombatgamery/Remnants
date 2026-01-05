using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Placeable.Walls;
using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	[LegacyName("vaultbrick")]
	public class PrototypePlating : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = ModContent.TileType <Content.Tiles.Underworld.Prototypes.PrototypePlating>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

            recipe = Recipe.Create(Type, 25);
            recipe.AddIngredient(ModContent.ItemType<HeavyAlloy>());
            recipe.AddIngredient(ItemID.IronBar);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.Register();

            recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<PrototypeWall>(), 4);
			recipe.Register();
		}
	}
}