using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Projectiles;
using Terraria.DataStructures;

namespace Remnants.Items.Weapons
{
	[LegacyName("vintagesniper")]
	public class VintageSniper : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() 
		{
			Item.CloneDefaults(ItemID.SniperRifle);
			Item.damage = 128;
			Item.crit = 24;
			Item.knockBack = 7;
			Item.useTime = 38;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 6);
			Item.UseSound = SoundID.Item11;

			Item.width = 31;
			Item.height = 10;

			Item.useAnimation = Item.useTime;
		}

        public override void HoldItem(Player player)
        {
			player.scope = true;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.Bullet)
			{
				type = ProjectileID.BulletHighVelocity;
			}
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			recipe = Recipe.Create(ModContent.ItemType<VintageSniper>());
			recipe.AddRecipeGroup("Wood", 12);
			recipe.AddRecipeGroup("IronBar", 8);
			recipe.AddIngredient(ItemID.SoulofSight, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}