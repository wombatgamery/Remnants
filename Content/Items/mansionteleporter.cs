////using SubworldLibrary;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Content.Items
//{
//    public class mansioninvitation : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("Invitation Letter");
//            Tooltip.SetDefault("'As a top student of magic, you've been invited to stay at the Grand Mansion.' \n'The enchantment on this letter will open a portal there, as long as you fuse it with the souls of light and dark.'");
//        }

//        public override void SetDefaults()
//        {
//            item.width = 10;
//            item.height = 10;
//            item.maxStack = 1;
//            item.value = 250;
//            item.rare = ItemRarityID.Blue;
//        }

//        public override void AddRecipes()
//        {
//            recipe = Recipe.Create(mod);
//            recipe.AddIngredient(this);
//            recipe.AddIngredient(ItemID.SoulofLight, 5);
//            recipe.AddIngredient(ItemID.SoulofNight, 5);
//            recipe.SetResult(ModContent.ItemType<mansionteleporter>());
//            recipe.Register();
//        }
//    }

//    public class mansionteleporter : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("Enchanted Letter");
//            Tooltip.SetDefault("The message has disappeared, but you already know exactly what this does.");
//        }

//        public override void SetDefaults()
//        {
//            item.width = 10;
//            item.height = 10;
//            item.maxStack = 1;
//            item.value = 250;
//            item.rare = ItemRarityID.Red;
//            item.useStyle = ItemUseStyleID.HoldingUp;
//        }

//        public override bool UseItem(Player player)
//        {
//            SubworldSystem.Enter<MansionSubworld>();

//            return true;
//        }
//    }
//}