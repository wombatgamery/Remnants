//using Microsoft.Xna.Framework;
//using Remnants.Content.Dusts;
//using Remnants.Content.NPCs;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Content.Items.Accessories
//{
//    public class alchemybook : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("Kinetic Warp Field");
//            // Tooltip.SetDefault("Tears enemies apart at a molecular level, ignoring their defense, more effective when not in motion.");
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 17 * 2;
//            Item.height = 18 * 2;
//            Item.accessory = true;
//            Item.maxStack = 1;
//            Item.value = Item.sellPrice(gold: 1);
//            Item.rare = ItemRarityID.Orange;
//        }

//        public override void UpdateEquip(Player player)
//        {
//            if (player.HasBuff(BuffID.Swiftness))
//            {
//                player.moveSpeed += 0.125f;
//            }
//            if (player.HasBuff(BuffID.Ironskin))
//            {
//                player.statDefense += 4;
//            }
//            if (player.HasBuff(BuffID.Regeneration))
//            {
//                player.statDefense += 4;
//            }
//        }
//    }
//}