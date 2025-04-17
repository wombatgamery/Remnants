using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Accessories
{
    public class DivineShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24 * 2;
            Item.height = 22 * 2;
            Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void UpdateEquip(Player player)
        {
            player.longInvince = true;

            player.GetModPlayer<RemPlayer>().magicShield = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<ManaShield>());
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}