using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Items.Consumable
{
    public class ArcaneKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.width = 9 * 2;
            Item.height = 13 * 2;
            Item.rare = ItemRarityID.Green;
        }
    }
}