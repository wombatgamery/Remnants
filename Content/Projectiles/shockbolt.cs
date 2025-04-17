using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
	public class shockbolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shock Bolt");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 4;               //The width of projectile hitbox
			Projectile.height = 4;              //The height of projectile hitbox
			Projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true;         //Can the projectile deal damage to enemies?
			Projectile.hostile = false;         //Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;          //Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.Bullet;
		}

		public override void AI()
		{
			int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 0.5f);
			Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(1f, 1f);
			Main.dust[dustIndex].velocity += Projectile.velocity * 0.2f;
			dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Scale: 0.5f);
			Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(1f, 1f);
			Main.dust[dustIndex].velocity += Projectile.velocity * 0.2f;
			Main.dust[dustIndex].noGravity = true;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			//if (Main.rand.NextBool(2))
   //         {
			//	target.AddBuff(BuffID.OnFire, 60);
			//}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item94, Projectile.position);

			for (int i = 0; i < 5; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.1f;
			}
			//for (int i = 0; i < 3; i++)
			//{
			//	int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Torch, Scale: Main.rand.Next(1, 3));
			//	Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f);
			//}
			for (int i = 0; i < 5; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Electric);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.1f;
			}
		}
	}
}