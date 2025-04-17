using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Items.Placeable.Blocks;

namespace Remnants.Content.Items.Placeable.Walls
{
	public class EnchantedBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<global::Remnants.Content.Walls.EnchantedBrickWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<EnchantedBrick>());
			recipe.Register();

			recipe = Recipe.Create(ModContent.ItemType<EnchantedBrick>());
			recipe.AddIngredient(Type, 4);
			recipe.Register();
		}
	}
}