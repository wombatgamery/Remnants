using Microsoft.Xna.Framework;
using Remnants.Content.Items.Placeable.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Consumable
{
    public class NightglowBerry : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            Item.ResearchUnlockCount = 5;

            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(198, 255, 74),
                new Color(80, 120, 47),
                new Color(63, 255, 90)
            };

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Ambrosia;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;

            Item.DefaultToFood(14, 16, BuffID.WellFed2, 1 * 1800);

            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.Blue;
        }

        public override void OnConsumeItem(Player player)
        {
            //player.AddBuff(BuffID.NightOwl, 1 * 3600);

            player.QuickSpawnItemDirect(player.GetSource_FromThis(), ModContent.ItemType<NightglowSeed>());
        }
    }

    public class NightglowBerryIcon : ModItem
    {
        public override LocalizedText DisplayName => null;
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[Type] = true;
        }
    }
}