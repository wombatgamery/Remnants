using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
    public class ArcanePlatform : ModItem
    {
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodPlatform);
            Item.createTile = ModContent.TileType<Tiles.Shimmer.ArcanePlatform>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;
            recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<ArcaneBrick>());
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<ArcaneBrick>());
            recipe.AddIngredient(Type, 2);
            recipe.Register();
        }
    }
}