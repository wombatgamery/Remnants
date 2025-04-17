using Microsoft.Xna.Framework;
using Remnants.Content.Items.Placeable.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items
{
    public class RunestalkBlessingIcon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[Type] = true;
        }
    }
}