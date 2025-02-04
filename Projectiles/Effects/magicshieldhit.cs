using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Projectiles.Effects
{
	public class magicshieldhit : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magic Shield");

			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 9999;

			Projectile.width = 28 * 2;
			Projectile.height = 28 * 2;

			Projectile.friendly = false;
			Projectile.hostile = false;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			Projectile.Center = Main.player[Projectile.owner].Center;
			Projectile.velocity = Vector2.Zero;

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 4)
				{
					Projectile.Kill();
				}
			}
		}

        public override void PostDraw(Color lightColor)
        {
			Rectangle rect = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Projectiles/vfx/magicshieldhit").Value, Projectile.position, rect, Color.White, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0f);
		}
    }
}