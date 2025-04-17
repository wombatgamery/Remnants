using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Consumable
{
    [LegacyName("ironkey")]
    public class IronKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.width = 7 * 2;
            Item.height = 12 * 2;
        }
    }
}