using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Projectiles;
using Remnants.Dusts;
using Terraria.DataStructures;

namespace Remnants.Projectiles.vanilla
{
	public class BetterFlamethrowers : GlobalItem
	{
  //      public override void SetDefaults(Item item)
  //      {
  //          if (item.type == ItemID.Flamethrower || item.type == ItemID.ElfMelter)
  //          {
		//		item.damage = (int)(item.damage * 2);
		//		item.shoot = ModContent.ProjectileType<flamejet>();
		//		item.consumeAmmoOnFirstShotOnly = true;
		//	}
  //      }

  //      public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
  //      {
		//	if (item.type == ItemID.Flamethrower || item.type == ItemID.ElfMelter)
		//	{
		//		for (int i = 0; i < 6; i++)
		//		{
		//			int dustIndex = Dust.NewDust(player.Center + Vector2.Normalize(Main.MouseWorld - player.Center) * 40, 0, 0, 135, Scale: Main.rand.NextFloat(2f));
		//			Main.dust[dustIndex].position += Main.rand.NextVector2Circular(2.5f, 2.5f);
		//			Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
		//			Main.dust[dustIndex].velocity += Vector2.Normalize(Main.MouseWorld - player.Center) * Main.rand.NextFloat(item.shootSpeed);
		//			Main.dust[dustIndex].noGravity = true;
		//		}
		//	}

		//	return true;
		//}

  //      public override bool CanConsumeAmmo(Item item, Item ammo, Player player)
  //      {
		//	if (item.type == ItemID.Flamethrower || item.type == ItemID.ElfMelter)
		//	{
		//		return false;
		//	}
		//	else
		//	{
		//		return true;
		//	}
		//}
    }

	public class flamejet : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fire Jet");
		}

		public float baseDamage;
		public float damageMult = 1;
		public float scalething = 1;

		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.alpha = 255; // This makes the projectile invisible, only showing the dust.
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = 9999; // How many monsters the projectile can penetrate. Change this to make the flamethrower pierce more mobs.
			Projectile.timeLeft = 9999;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 2;

			Projectile.scale = 0.1f;
		}

		public bool spawnFrame = true;
		private void OnSpawn()
        {
			spawnFrame = false;

			baseDamage = Projectile.damage;
			Projectile.velocity *= 1.6f;
		}

		public override void AI()
		{
			if (spawnFrame)
			{
				OnSpawn();
			}

			scalething *= 0.95f;
			Projectile.scale = (1 - scalething);

			Projectile.velocity *= 0.98f;
			Projectile.velocity += Main.rand.NextVector2Circular(0.3f, 0.3f) * scalething;

			damageMult = Projectile.scale;
			Projectile.damage = (int)(baseDamage * damageMult);

			int dustIndex;

			if (Main.rand.NextBool(5))
            {
				dustIndex = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), DustID.Smoke, Scale: Main.rand.Next(1, 3) * Projectile.scale);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f) * Projectile.scale;
				Main.dust[dustIndex].velocity += Projectile.velocity * 0.2f;
			}

			if (Main.rand.NextBool(2))
			{
				dustIndex = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), DustID.Torch, Scale: Main.rand.Next(1, 4) * Projectile.scale);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(1f, 1f) * Projectile.scale;
				Main.dust[dustIndex].velocity += Projectile.velocity * 0.2f;
				Main.dust[dustIndex].noGravity = true;
			}

			if (scalething < 0.001f || Projectile.wet)
            {
				Projectile.Kill();
            }
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, 20 * 60);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.OnFire, 20 * 60, false);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.Kill();
			if (Main.myPlayer == Projectile.owner)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<burn>(), 12, 0f, Projectile.owner);
			}
			return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			// By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
			// Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
			int size = 30;
			hitbox.X -= size;
			hitbox.Y -= size;
			hitbox.Width += size * 2;
			hitbox.Height += size * 2;
		}
	}
}