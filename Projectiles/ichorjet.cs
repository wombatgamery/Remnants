using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Projectiles;
using Remnants.Dusts;

namespace Remnants.Projectiles
{
	public class ichorjet : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.IchorSplash;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ichor Jet"); // The English name of the projectile
		}

		public float baseDamage = 1;
		public float damageMult = 1;
		public float scalething = 1;

		public override void SetDefaults()
		{
			Player player = Main.LocalPlayer;

			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.alpha = 255; // This makes the projectile invisible, only showing the dust.
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = 9999; // How many monsters the projectile can penetrate. Change this to make the flamethrower pierce more mobs.
			Projectile.timeLeft = 9999;
			Projectile.ignoreWater = true;
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
				dustIndex = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), 64, Scale: Main.rand.Next(1, 4) * Projectile.scale);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(1f, 1f) * Projectile.scale;
				Main.dust[dustIndex].velocity += Projectile.velocity * 0.2f;
				Main.dust[dustIndex].noGravity = true;
			}

			if (scalething < 0.001f)
            {
				Projectile.Kill();
            }
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Ichor, 12 * 60); //Gives cursed flames to target for 4 seconds. (60 = 1 second, 240 = 4 seconds)
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Ichor, 12 * 60, false);
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