using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Items.Placeable.Plants;
using Remnants.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Remnants.Content.Items.Weapons;
using Remnants.Content.Items.Consumable;

namespace Remnants.Content.Items.Materials
{
	public class ResinGel : ModItem
	{
		public override void SetStaticDefaults()
		{
            //ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Gel;
		}

		public override void SetDefaults()
		{
			Item.width = 10 * 2;
			Item.height = 8 * 2;
			Item.maxStack = 9999;
            Item.value = 10;
		}

        public override void AddRecipes()
		{
            Recipe recipe;

            recipe = Recipe.Create(ItemID.Amber);
            recipe.AddIngredient(Type, 4);
            recipe.AddTile(TileID.Solidifier);
            recipe.Register();

            recipe = Recipe.Create(ItemID.Torch, 6);
            recipe.AddIngredient(Type);
            recipe.AddRecipeGroup(RecipeGroupID.Wood);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<FragmentGrenade>(), 3);
            recipe.AddIngredient(ItemID.Grenade, 3);
            recipe.AddIngredient(Type);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<FragmentGlowstick>(), 5);
            recipe.AddIngredient(ItemID.Glowstick, 5);
            recipe.AddIngredient(Type);
            recipe.Register();

            recipe = Recipe.Create(ItemID.RestorationPotion, 2);
            recipe.AddIngredient(ItemID.Mushroom);
            recipe.AddIngredient(ItemID.GlowingMushroom);
            recipe.AddIngredient(Type);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.Register();
        }
	}
}