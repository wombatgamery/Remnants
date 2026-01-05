using Remnants.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Walls
{
	public class EnchantedBrickWall : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.Shimmer.EnchantedBrickWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<ArcaneBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<ArcaneBrick>());
			recipe.AddIngredient(Type, 4);
			recipe.Register();
		}
	}
}