using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;
using Terraria;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class SulfurstoneDiamond : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.DiamondStoneBlock);
            Item.createTile = ModContent.TileType <Tiles.Blocks.SulfurstoneDiamond>();
		}

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Diamond);
            recipe.AddIngredient(ModContent.ItemType<Sulfurstone>());
            recipe.AddCondition(Condition.InGraveyard);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
}