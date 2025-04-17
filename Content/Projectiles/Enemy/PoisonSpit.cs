using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles.Enemy
{
	public class PoisonSpit : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 6 * 2;
			Projectile.height = 5 * 2;
			Projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false;         //Can the projectile deal damage to enemies?
			Projectile.hostile = true;         //Can the projectile deal damage to the player?
			Projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

			Projectile.alpha = 255;
		}

		//float direction;

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			int dustIndex;

			for (int i = 0; i < 5; i++)
            {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Poisoned, Alpha: 153);
				dust.velocity *= 0.5f;
				dust.velocity += Projectile.velocity * 0.5f;
				dust.noGravity = true;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Poisoned, 10 * 60);
			Projectile.Kill();
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Poisoned, 10 * 60);
			Projectile.Kill();
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Poisoned, Alpha: 153);
				dust.velocity = Main.rand.NextVector2Circular(5, 5);
				dust.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.position);
        }
	}
}