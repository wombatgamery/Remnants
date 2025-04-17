using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Items.Placeable.Blocks;

namespace Remnants.Content.Items.Placeable.Walls
{
    [LegacyName("TidalSlabWall")]
    public class MarineSlabWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
			Item.height = 16;
            Item.createWall = ModContent.WallType<global::Remnants.Content.Walls.MarineSlabWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<MarineSlab>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<MarineSlab>());
			recipe.AddIngredient(Type, 4);
			recipe.Register();
		}
	}
}