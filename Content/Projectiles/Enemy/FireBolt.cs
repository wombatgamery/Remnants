using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles.Enemy
{
	public class FireBolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10 * 2;
			Projectile.height = 8 * 2;
			Projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false;         //Can the projectile deal damage to enemies?
			Projectile.hostile = true;         //Can the projectile deal damage to the player?
			Projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
		}

		//float direction;

		public override void AI()
		{
			//if (!Main.player[(int)Projectile.ai[1]].DeadOrGhost)
			//         {
			//	direction = (Main.player[(int)Projectile.ai[1]].Center - Projectile.Center).ToRotation();
			//}

			Projectile.rotation = Projectile.velocity.ToRotation();
			//Projectile.velocity += direction.ToRotationVector2() / 5;

			int dustIndex;

			//if (Main.rand.NextBool(2))
   //         {
			//	dustIndex = Dust.NewDust(Projectile.Center + Projectile.velocity - Projectile.rotation.ToRotationVector2() * 4, 0, 0, DustID.Smoke);
			//	Main.dust[dustIndex].velocity = -Vector2.Normalize(Projectile.velocity);
			//}
			for (int i = 0; i < 3; i++)
            {
				dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
				Main.dust[dustIndex].noGravity = true;
			}

			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Type])
				{
					Projectile.frame = 0;
				}
			}

			//Projectile.velocity *= 0.99f;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.OnFire, 10 * 60);
			Projectile.Kill();
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.OnFire, 10 * 60);
			Projectile.Kill();
		}

        public override void PostDraw(Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin;// + new Vector2(0f, Projectile.gfxOffY);
			Rectangle rect = new Rectangle(0, Projectile.frame * (Projectile.height + 2), Projectile.width, Projectile.height); 
			Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, rect, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 100);
				dust.velocity = Main.rand.NextVector2Circular(4, 4);
			}
			for (int i = 0; i < 5; i++)
			{
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, default, Main.rand.Next(61, 64), 1f);
				gore.position = new Vector2(Projectile.Center.X - gore.Width / 2, Projectile.Center.Y - gore.Height / 2);
				gore.velocity = Main.rand.NextVector2Circular(2, 2);
			}
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
				dust.noGravity = true; 
				dust.velocity = Main.rand.NextVector2Circular(4, 4);
			}

			SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
        }
	}
}