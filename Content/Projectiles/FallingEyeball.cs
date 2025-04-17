using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
	public class FallingEyeball : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 600;
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.3f;

			if (Projectile.frame >= 3)
            {
				Lighting.AddLight(Projectile.Center, new Vector3(0f / 255f, 127f / 255f, 31f / 255f));
			}
			else Lighting.AddLight(Projectile.Center, new Vector3(127f / 255f, 74f / 255f, 0f / 255f));
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Projectile.frame >= 3 ? DustID.GreenBlood : DustID.Blood);
				dust.velocity = new Vector2(Main.rand.NextFloat(-oldVelocity.Y, oldVelocity.Y) / 2, -Main.rand.NextFloat(0, oldVelocity.Y)) / 2;
			}

			Projectile.Kill();

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
		}

        public override void PostDraw(Color lightColor)
        {
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.position - Main.screenPosition, new Rectangle(0, (Projectile.frame + 6) * 16, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
    }
}