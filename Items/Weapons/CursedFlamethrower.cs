using Microsoft.Xna.Framework;
using Remnants.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Items.Weapons
{
	public class CursedFlamethrower : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() 
		{
			Item.CloneDefaults(ItemID.Flamethrower);
			Item.damage += 2;
			Item.width = 33 * 2;
			Item.height = 10 * 2;
			Item.useTime = 4;
			Item.useAnimation = 20;
			Item.value = Item.sellPrice(gold: 15);
			Item.rare += 1;
			Item.shoot = ModContent.ProjectileType<cursedflamejet>();
			Item.shootSpeed = 8f;
			Item.consumeAmmoOnFirstShotOnly = true;
		}

   //     public override void AddRecipes()
   //     {
			//Recipe recipe;

   //         recipe = Recipe.Create(ModContent.ItemType<cursedflamethrower>());

   //         recipe.AddIngredient(ItemID.Flamethrower);
   //         recipe.AddIngredient(ItemID.ChlorophyteBar, 8);
   //         recipe.AddIngredient(ItemID.CursedFlame, 20);
   //         recipe.AddIngredient(ItemID.SoulofNight, 10);
   //         recipe.AddTile(TileID.MythrilAnvil);
   //         //recipe.Register();
   //     }
    }
}