using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles;
using Terraria;
using Remnants.Items.Placeable.Blocks;

namespace Remnants.Items.Placeable.Plants
{
    public class DreampodSeeds : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodenChair);
            Item.width = 10 * 2;
            Item.height = 10 * 2;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 1);
            Item.createTile = ModContent.TileType<Tiles.Plants.DreampodVine>();
        }
    }
}