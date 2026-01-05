using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class Cage : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GrayBrick);
            Item.createTile = ModContent.TileType <Tiles.Underworld.Cage>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}