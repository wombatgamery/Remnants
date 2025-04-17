using Microsoft.Xna.Framework;
using Remnants.Content.Projectiles.Weapons;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Weapons
{
	public class ArmCannon : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() 
		{
			Item.damage = 48;
			Item.mana = 4;
			Item.knockBack = 6;

			Item.useTime = 9;
			Item.useAnimation = 9;
			Item.shoot = ModContent.ProjectileType<pulsebolt>();
			Item.shootSpeed = 6f;
			Item.useStyle = ItemUseStyleID.Shoot;
			//Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/dkmarauder2");

			Item.width = 15 * 2;
			Item.height = 7 * 2;

			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(0, 4);

			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;
		}

  //              public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
  //      {
		//	for (int i = 0; i < 6; i++)
		//	{
		//		int dustIndex = Dust.NewDust(player.Center + Vector2.Normalize(Main.MouseWorld - player.Center) * 30, 0, 0, ModContent.DustType<energyparticle>(), Scale: Main.rand.NextFloat(2f));
		//		Main.dust[dustIndex].position += Main.rand.NextVector2Circular(3f, 3f);
		//		Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f);
		//		Main.dust[dustIndex].velocity += Vector2.Normalize(Main.MouseWorld - player.Center) * Main.rand.Next(16);
		//		Main.dust[dustIndex].noGravity = true;
		//	}

		//	return true;
		//}

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}
