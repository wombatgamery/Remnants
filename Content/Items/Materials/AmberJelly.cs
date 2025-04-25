using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Items.Placeable.Plants;
using Remnants.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Materials
{
	public class AmberJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Gel;
		}

		public override void SetDefaults()
		{
			Item.width = 10 * 2;
			Item.height = 8 * 2;
			Item.maxStack = 9999;
            Item.value = 300;
		}

        public override void AddRecipes()
		{
            Recipe recipe;

            recipe = Recipe.Create(ItemID.Amber);
            recipe.AddIngredient(Type, 5);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
	}
}