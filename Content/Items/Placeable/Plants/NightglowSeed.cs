using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Remnants.Content.Items.Placeable.Plants
{
    [LegacyName("nightglowseed")]
    public class NightglowSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodenChair);
            Item.width = 6 * 2;
            Item.height = 6 * 2;
            Item.value = Item.sellPrice(copper: 10);
            Item.createTile = ModContent.TileType<Tiles.Forest.Nightglow>();
            Item.placeStyle = 3;
        }
    }
}