using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Walls;
using Remnants.Items.Placeable.Blocks;

namespace Remnants.Items.Placeable.Walls
{
	public class WaterBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<global::Remnants.Walls.WaterBrickWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<WaterBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<WaterBrick>());
			recipe.AddIngredient(this, 4);
			recipe.Register();
		}
	}
}