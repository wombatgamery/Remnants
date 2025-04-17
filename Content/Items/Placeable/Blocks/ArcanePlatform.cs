using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Blocks
{
    public class ArcanePlatform : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodPlatform);
            Item.createTile = ModContent.TileType<Tiles.Blocks.ArcanePlatform>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;
            recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<EnchantedBrick>());
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<EnchantedBrick>());
            recipe.AddIngredient(Type, 2);
            recipe.Register();
        }
    }
}