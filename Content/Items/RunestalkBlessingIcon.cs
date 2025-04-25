using Microsoft.Xna.Framework;
using Remnants.Content.Items.Placeable.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items
{
    public class RunestalkBlessingIcon : ModItem
    {
        public override LocalizedText DisplayName => null;
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[Type] = true;
        }
    }
}