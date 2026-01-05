using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class WoodenSpike : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodenChair);
			Item.width = 16;
			Item.height = 28;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<Content.Tiles.Forest.WoodenSpike>();
		}

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Wood);
			recipe.AddTile(TileID.Sawmill);
            recipe.Register();
        }
    }
}