using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;
using Terraria;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class SulfuricVent : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
            Item.height = 28;
            Item.createTile = ModContent.TileType <Tiles.Blocks.SulfuricVent>();
		}

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Sulfurstone>());
            recipe.AddCondition(Condition.InGraveyard);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}