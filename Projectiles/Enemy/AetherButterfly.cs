using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Projectiles.Enemy
{
	public class AetherButterfly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8 * 2;
			Projectile.height = 8 * 2;
			Projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false;         //Can the projectile deal damage to enemies?
			Projectile.hostile = true;         //Can the projectile deal damage to the player?
			Projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
		}

		int target = 0;

        public override void OnSpawn(IEntitySource source)
        {
			float closest = Vector2.Distance(Main.player[0].Center, Projectile.Center);
			for (int k = 1; k < Main.maxPlayers; k++)
			{
				if (Main.player[k].active)
				{
					float distance = Vector2.Distance(Main.npc[k].Center, Projectile.Center);
					if (distance < closest)
					{
						closest = distance;
						target = k;
					}
				}
				else break;
			}
		}

		int timer = 0;

		public override void AI()
		{
			Dust dust;

			if (++timer >= 60)
			{
				if (timer == 60)
				{
					Projectile.velocity = Vector2.Normalize(Main.player[target].Center - Projectile.Center) * 10;

					Projectile.frame = 1;

					for (int i = 0; i < 25; i++)
					{
						dust = Dust.NewDustPerfect(Projectile.Center, DustID.ShimmerTorch, Main.rand.NextVector2CircularEdge(5, 5));
						dust.noGravity = true;
					}

					//SoundEngine.PlaySound(SoundID.Item42, Projectile.position);
				}

				Projectile.velocity += Main.rand.NextVector2Circular(1, 1);

				Projectile.rotation = Projectile.velocity.ToRotation();
			}
			else
			{
				Projectile.velocity += Vector2.Normalize(Main.player[target].Center - Projectile.Center) / 60;
				Projectile.velocity += Main.rand.NextVector2Circular(0.2f, 0.2f);

				Projectile.rotation = 0;
				Projectile.spriteDirection = Main.player[target].Center.X < Projectile.Center.X ? -1 : 1;

				if (++Projectile.frameCounter >= 2)
				{
					Projectile.frameCounter = 0;
					if (++Projectile.frame >= Main.projFrames[Type])
					{
						Projectile.frame = 0;
					}
				}
			}

			Lighting.AddLight(Projectile.Center, RemTile.MagicalLabLightColour((int)Main.GameUpdateCount + Projectile.whoAmI).ToVector3());

			dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerTorch);
			dust.noGravity = true;

			Projectile.netUpdate = true;
		}

  //      public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
  //      {
		//	target.AddBuff(BuffID.Confused, 5 * 60);
		//	Projectile.Kill();
		//}

  //      public override void OnHitPlayer(Player target, Player.HurtInfo info)
		//{
		//	target.AddBuff(BuffID.Confused, 5 * 60);
		//	Projectile.Kill();
		//}

        public override void PostDraw(Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin;// + new Vector2(0f, Projectile.gfxOffY);
			Rectangle rect = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Projectiles/Enemy/AetherButterfly").Value, drawPos, rect, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Projectiles/Enemy/AetherButterflyGlow").Value, drawPos, rect, RemTile.MagicalLabLightColour((int)Main.GameUpdateCount + Projectile.whoAmI), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.ShimmerTorch, Main.rand.NextVector2CircularEdge(5, 5));
				dust.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
	}
}