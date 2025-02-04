using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Projectiles;

namespace Remnants.Projectiles
{
	public class deathbolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Death Bolt");     //The English name of the projectile
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			AIType = ProjectileID.Bullet;
			Projectile.damage = 500;
			Projectile.DamageType = DamageClass.Magic;

			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;

			Projectile.friendly = true;         //Can the projectile deal damage to enemies?
			Projectile.hostile = false;         //Can the projectile deal damage to the player?

			Projectile.penetrate = 1;
			Projectile.timeLeft = 9999;
			Projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 4;
		}

        public override void AI()
		{
			NPC npc = Main.npc[(int)Projectile.ai[0]];

			Projectile.velocity *= 1.02f;

			int dustIndex;
			for (int i = 0; i < 2; i++)
            {
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].noGravity = true;
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].noGravity = true;
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].noGravity = true;
			}
			//Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, Scale: Main.rand.Next(1, 4));
			//Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			int fireLength = Main.rand.Next(8, 13) * 60;
			target.AddBuff(BuffID.OnFire, fireLength);
			target.AddBuff(BuffID.CursedInferno, fireLength);
			target.AddBuff(BuffID.Frostburn, fireLength);
			//target.AddBuff(BuffID.ShadowFlame, fireLength);
			target.AddBuff(BuffID.Daybreak, fireLength);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			//SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, Mod.GetSoundSlot(SoundType.Item, "Sounds/Item/hyperb"));

			for (int i = 0; i < 40; i++)
			{
				int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(5f, 5f);
			}
			for (int i = 0; i < 40; i++)
			{
				int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.Next(1, 4));
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(5f, 5f);
				Main.dust[dustIndex].noGravity = true;
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, Scale: Main.rand.Next(1, 4));
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(5f, 5f);
				Main.dust[dustIndex].noGravity = true;
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, Scale: Main.rand.Next(1, 4));
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(5f, 5f);
				Main.dust[dustIndex].noGravity = true;
			}
			//for (int i = 0; i < 10; i++)
			//{
			//	int goreIndex = Gore.NewGore(Projectile.position, Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(61, 64));
			//	Main.gore[goreIndex].position.X += Main.rand.Next(-Projectile.width / 2, Projectile.width / 2);
			//	Main.gore[goreIndex].position.Y += Main.rand.Next(-Projectile.height / 2, Projectile.height / 2);
			//}

			//RemPlayer.ScreenShake(Projectile.position, 3);

			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.position.X -= 50;
			Projectile.position.Y -= 50;
			Projectile.penetrate = 999;
			Projectile.Damage();
		}
	}
}