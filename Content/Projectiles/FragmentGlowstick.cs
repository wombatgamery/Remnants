using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
	public class FragmentGlowstick : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Glowstick);

            Projectile.light = 0;

			AIType = ProjectileID.Glowstick;
        }

        public override bool PreAI()
        {
            Lighting.AddLight(Projectile.Center, 200f / 255f, 120f / 255f, 0);

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != 0)
            {
                for (int k = 0; k < 5; k++)
                {
                    Projectile fragment = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FragmentGlowstickFragment>(), 0, 0, Projectile.owner);

                    fragment.velocity = Projectile.velocity + Main.rand.NextVector2Circular(4, 4);

                    if (oldVelocity.Y > 0 && fragment.velocity.Y > 0)
                    {
                        fragment.velocity.Y *= -1;
                    }
                    if (oldVelocity.Y < 0 && fragment.velocity.Y < 0)
                    {
                        fragment.velocity.Y *= -1;
                    }
                }

                Projectile.Kill();

                return false;
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item51, Projectile.Center);

            for (int k = 0; k < 10; k++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmber, 0, -2f, Projectile.alpha);
                if (Main.rand.NextBool(8))
                {
                    dust.noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }
    }

    public class FragmentGlowstickFragment : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Glowstick);

            Projectile.width = Projectile.height = 6;
            Projectile.light = 0;
            Projectile.timeLeft = 3600;

            AIType = ProjectileID.Glowstick;
        }

        public override bool PreAI()
        {
            Lighting.AddLight(Projectile.Center, 165f / 255f, 73f / 255f, 0);

            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }
    }
}