using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles.Enemy
{
	public class ResinShrapnel : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 5 * 2;
			Projectile.height = 5 * 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;         //Can the projectile deal damage to enemies?
			Projectile.hostile = true;         //Can the projectile deal damage to the player?
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;

			Projectile.alpha = 50;
		}

		public override void AI()
		{
            Projectile.rotation += Projectile.velocity.X;

            Projectile.velocity.Y += 0.2f;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmber, Alpha: 50);
				dust.velocity = Main.rand.NextVector2Circular(5, 5);
			}

			SoundEngine.PlaySound(SoundID.Item51, Projectile.position);
        }
	}
}