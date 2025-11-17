using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Items.Placeable.Blocks;

namespace Remnants.Content.Items.Placeable.Walls
{
	public class SulfurstoneBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.SulfurstoneBrickWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<SulfurstoneBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<SulfurstoneBrick>());
			recipe.AddIngredient(Type, 4);
			recipe.Register();
		}
	}
}