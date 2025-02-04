using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Walls;
using Terraria;
using Remnants.Items.Placeable.Blocks;

namespace Remnants.Items.Placeable.Walls
{
	public class VaultWall : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
			Item.createWall = ModContent.WallType <global::Remnants.Walls.VaultWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
			recipe.Register();
		}
	}
}