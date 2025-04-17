using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
	public class frostburn : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.MolotovFire;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frostfire");     //The English name of the projectile
			//ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ModContent.ProjectileType<burn>());
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.05f;

			int dustIndex;
			if (Main.rand.NextBool(15))
			{
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(1f, 1f);
				Main.dust[dustIndex].velocity.Y -= Main.rand.NextFloat(2);
			}
			if (Main.rand.NextBool(2))
			{
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].noGravity = true;
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(1f, 1f);
				Main.dust[dustIndex].velocity.Y -= Main.rand.NextFloat(3);
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity.X *= 0.5f;
			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Frostburn, 10 * 60);
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Frostburn, 10 * 60, false);
		}

		//public override void ModifyDamageHitbox(ref Rectangle hitbox)
		//{
		//	// By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
		//	// Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
		//	int size = 10;
		//	hitbox.X -= size;
		//	hitbox.Y -= size;
		//	hitbox.Width += size * 2;
		//	hitbox.Height += size * 2;
		//}
	}
}