using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Walls;
using Remnants.Items.Placeable.Blocks;

namespace Remnants.Items.Placeable.Walls
{
	public class TombBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
			Item.height = 16;
            Item.createWall = ModContent.WallType<global::Remnants.Walls.TombBrickWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<TombBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<TombBrick>());
			recipe.AddIngredient(Type, 4);
			recipe.Register();
		}
	}
}