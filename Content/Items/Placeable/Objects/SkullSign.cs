using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles.Objects;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class SkullSign : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenChair);
			Item.width = 28;
			Item.height = 28;
			Item.createTile = ModContent.TileType<Tiles.Objects.Decoration.SkullSign>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.Wood, 6);
            recipe.AddIngredient(ItemID.WhitePaint);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}