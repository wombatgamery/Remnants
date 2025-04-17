using System.Linq;
using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Remnants.Content.Items.Weapons;
using Remnants.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Accessories
{
    public class StoneofElements : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 12 * 2;
            Item.height = 17 * 2;
            Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void UpdateEquip(Player player)
        {
            player.pStone = true;

            int[] invalidBuffs = new int[2];
            invalidBuffs[0] = BuffID.PotionSickness;
            invalidBuffs[1] = BuffID.ManaSickness;

            for (int i = 0; i < player.CountBuffs(); i++)
            {
                int buff = player.buffType[i];
                if (Main.debuff[buff] && !invalidBuffs.Contains(buff) && player.buffTime[i] > 2)
                {
                    player.buffTime[i]--;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<RingofElements>());
            recipe.AddIngredient(ItemID.PhilosophersStone);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}