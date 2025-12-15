//using System.Linq;
//using Microsoft.Xna.Framework;
//using Remnants.Content.Dusts;
//using Remnants.Content.Items.Weapons;
//using Remnants.Content.NPCs;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Content.Items.Accessories
//{
//    public class CosmicStone : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            Item.ResearchUnlockCount = 1;
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 17 * 2;
//            Item.height = 17 * 2;
//            Item.accessory = true;
//            Item.maxStack = 1;
//            Item.value = Item.sellPrice(gold: 14);
//            Item.rare = ItemRarityID.Yellow;
//        }

//        public override void UpdateEquip(Player player)
//        {
//            player.skyStoneEffects = true;
//            player.pStone = true;

//            int[] invalidBuffs = new int[2];
//            invalidBuffs[0] = BuffID.PotionSickness;
//            invalidBuffs[1] = BuffID.ManaSickness;

//            for (int i = 0; i < player.CountBuffs(); i++)
//            {
//                int buff = player.buffType[i];
//                if (Main.debuff[buff] && !invalidBuffs.Contains(buff) && player.buffTime[i] > 2)
//                {
//                    player.buffTime[i]--;
//                }
//            }
//        }

//        //public override void AddRecipes()
//        //{
//        //    Recipe recipe;

//        //    recipe = Recipe.Create(Type);
//        //    recipe.AddIngredient(ModContent.ItemType<StoneofElements>());
//        //    recipe.AddIngredient(ItemID.CelestialStone);
//        //    recipe.AddIngredient(ItemID.FragmentSolar, 10);
//        //    recipe.AddIngredient(ItemID.FragmentVortex, 10);
//        //    recipe.AddIngredient(ItemID.FragmentNebula, 10);
//        //    recipe.AddIngredient(ItemID.FragmentStardust, 10);
//        //    recipe.AddTile(TileID.TinkerersWorkbench);
//        //    recipe.Register();
//        //}
//    }
//}