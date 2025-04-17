//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Content.Tiles.Objects;
//using Remnants.Content.Tiles;
//using Remnants.Content.Tiles.Plants;
//using Remnants.Content.Tiles.Objects.Hazards;
//using Remnants.Content.Walls.Parallax;

//namespace Remnants.Content.Items.Placeable
//{
//    public class sprout : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("sprout"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 8;
//            Item.height = 8;
//            Item.maxStack = 999;
//            Item.useTurn = true;
//            Item.autoReuse = true;
//            Item.useAnimation = 15;
//            Item.useTime = 10;
//            Item.useStyle = ItemUseStyleID.Swing;
//            Item.consumable = true;
//            Item.createWall = ModContent.WallType<undergrowth>();
//            //item.placeStyle = 1;
//        }
//    }
//}