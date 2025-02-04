//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace Remnants.Items.Materials
//{
//    public class HeavyAlloy : ModItem
//    {
//        public override void SetDefaults()
//        {
//            Item.width = 17 * 2;
//            Item.height = 11 * 2;
//            Item.maxStack = 999;
//            Item.value = 10000;
//            Item.rare = ItemRarityID.LightRed;
//        }

//        public override void AddRecipes()
//        {
//            Recipe recipe;

//            recipe = Recipe.Create(Type, 10);
//            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
//            recipe.AddIngredient(ItemID.AdamantiteBar);
//            recipe.AddIngredient(ItemID.TitaniumBar);
//            recipe.AddTile(TileID.AdamantiteForge);
//            recipe.Register();
//        }
//    }
//}