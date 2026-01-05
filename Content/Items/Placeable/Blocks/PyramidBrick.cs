using Remnants.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class PyramidBrick : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GrayBrick);
            Item.createTile = ModContent.TileType <Tiles.DesertRuins.PyramidBrick>();
		}

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Hardstone>());
            recipe.AddIngredient(ItemID.HardenedSand);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}