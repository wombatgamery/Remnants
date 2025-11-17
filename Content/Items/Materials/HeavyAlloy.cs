using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Materials
{
    public class HeavyAlloy : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 17 * 2;
            Item.height = 11 * 2;
            Item.maxStack = 999;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.AdamantiteBar);
            recipe.AddIngredient(ItemID.HallowedBar);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.Register();

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.TitaniumBar);
            recipe.AddIngredient(ItemID.HallowedBar);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.Register();
        }
    }
}