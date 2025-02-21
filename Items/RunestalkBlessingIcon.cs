using Microsoft.Xna.Framework;
using Remnants.Items.Placeable.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Items
{
    public class RunestalkBlessingIcon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[Type] = true;
        }
    }
}