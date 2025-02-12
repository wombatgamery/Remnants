using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles;
using Terraria;
using Remnants.Items.Placeable.Blocks;

namespace Remnants.Items.Placeable.Plants
{
    public class PrismbudSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodenChair);
            Item.width = 8 * 2;
            Item.height = 8 * 2;
            Item.createTile = ModContent.TileType<Tiles.Plants.PrismbudStem>();
        }
    }
}