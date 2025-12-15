using Remnants.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Walls
{
	public class PyramidBrickWall : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
			Item.height = 16;
            Item.createWall = ModContent.WallType<global::Remnants.Content.Walls.PyramidBrickWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<PyramidBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<PyramidBrick>());
			recipe.AddIngredient(Type, 4);
			recipe.Register();
		}
	}
}