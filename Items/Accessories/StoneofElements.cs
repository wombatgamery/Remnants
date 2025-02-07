using System.Linq;
using Microsoft.Xna.Framework;
using Remnants.Dusts;
using Remnants.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Items.Accessories
{
    [LegacyName("ringofelements")]
    public class RingofElements : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14 * 2;
            Item.height = 14 * 2;
            Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
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
    }
}