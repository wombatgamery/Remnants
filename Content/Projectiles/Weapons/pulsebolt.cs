using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles.Weapons
{
	public class pulsebolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pulse Bolt");
		}

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

			Projectile.width = 12;
			Projectile.height = 12;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;         //Can the projectile deal damage to enemies?
			Projectile.hostile = false;         //Can the projectile deal damage to the player?
			Projectile.ignoreWater = false;          //Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;          //Can the projectile collide with tiles?
			Projectile.extraUpdates = 2;
		}

		public int effectTimer = 0;

		float homingDistance = 24 * 16;

		public override void AI()
		{
			Projectile.velocity += Main.rand.NextVector2Circular(0.2f, 0.2f);

			int target = -1;
			float closest = homingDistance;
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].CanBeChasedBy())
				{
					float distance = Vector2.Distance(Main.npc[k].Center, Projectile.Center);
					if (distance < closest)
					{
						closest = distance;
						target = k;
					}
				}
			}

			if (target != -1)
            {
				Vector2 newMove = Main.npc[target].Center + Main.npc[target].velocity - Projectile.Center;
				float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
				if (distanceTo <= homingDistance)
				{
					Projectile.velocity *= 0.99f;
					//projectile.velocity += Vector2.Normalize(newMove) * (1 - (distanceTo / homingDistance)) * 0.4f;
					Projectile.velocity += Vector2.Normalize(newMove) * 0.1f;
				}
			}

			int dustIndex;

            if (Main.rand.NextBool(3))
            {
				dustIndex = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), 0, 0, ModContent.DustType<energyparticle>(), Scale: Main.rand.NextFloat(2f));
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity;
				Main.dust[dustIndex].noGravity = true;
			}
            if (--effectTimer <= 0)
			{
				effectTimer = 4;

				Dust.NewDust(Projectile.position, 0, 0, ModContent.DustType<pulsering>(), 0, 0);
			}
			Lighting.AddLight(Projectile.Center, 73f / 255f, 180f / 255f, 242f / 255f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Frostburn, 60);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.5f;
			}
			//for (int i = 0; i < 3; i++)
			//{
			//	int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Torch, Scale: Main.rand.Next(1, 3));
			//	Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f);
			//}
			for (int i = 0; i < 5; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Electric);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.5f;
				Main.dust[dustIndex].noGravity = true;
			}

			//Dust.NewDust(projectile.Center, 0, 0, ModContent.DustType<pulseburst>(), 0, 0);
		}
	}
}