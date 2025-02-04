//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Audio;
//using Terraria.GameContent;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Remnants.Projectiles;

//namespace Remnants.Projectiles
//{
//	public class dragonbulletprojectile : ModProjectile
//	{
//		public override void SetStaticDefaults()
//		{
//			DisplayName.SetDefault("Dragon Round");     //The English name of the projectile
//			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;    //The length of old position to be recorded
//			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        //The recording mode
//		}

//		public override void SetDefaults()
//		{
//			Projectile.width = 10;               //The width of projectile hitbox
//			Projectile.height = 3;              //The height of projectile hitbox
//			Projectile.aiStyle = 1;             //The ai style of the projectile, please reference the source code of Terraria
//			Projectile.friendly = true;         //Can the projectile deal damage to enemies?
//			Projectile.hostile = false;         //Can the projectile deal damage to the player?
//			Projectile.DamageType = DamageClass.Ranged;           //Is the projectile shoot by a ranged weapon?
//			Projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
//			Projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
//			Projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
//			Projectile.light = 1f;            //How much light emit around the projectile
//			Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
//			Projectile.tileCollide = true;          //Can the projectile collide with tiles?
//			Projectile.extraUpdates = 2;
//			AIType = ProjectileID.Bullet;
//		}

//        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
//        {
//			target.AddBuff(BuffID.OnFire, 300);

//			Projectile.Kill();
//		}

//        public override bool OnTileCollide(Vector2 oldVelocity)
//		{
//			Projectile.Kill();

//			return false;
//		}

//		public override bool PreDraw(ref Color lightColor)
//		{
//			//Redraw the projectile with the color not influenced by light
//			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
//			for (int k = 0; k < Projectile.oldPos.Length; k++)
//			{
//				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
//				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
//				spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
//			}
//			return true;
//		}

//		public override void Kill(int timeLeft)
//		{
//			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
//			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
//			SoundEngine.PlaySound(SoundID.Item62, Projectile.position);

//			for (int i = 0; i < 50; i++)
//			{
//				int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 2f);
//				Main.dust[dustIndex].velocity *= 1.4f;
//			}
//			// Fire Dust spawn
//			for (int i = 0; i < 80; i++)
//			{
//				int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 3f);
//				Main.dust[dustIndex].noGravity = true;
//				Main.dust[dustIndex].velocity *= 5f;
//				dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 2f);
//				Main.dust[dustIndex].velocity *= 3f;
//			}
//			// Large Smoke Gore spawn
//			for (int g = 0; g < 2; g++)
//			{
//				int goreIndex = Gore.NewGore(new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
//				Main.gore[goreIndex].scale = 1.5f;
//				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
//				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
//				goreIndex = Gore.NewGore(new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
//				Main.gore[goreIndex].scale = 1.5f;
//				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
//				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
//				goreIndex = Gore.NewGore(new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
//				Main.gore[goreIndex].scale = 1.5f;
//				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
//				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
//				goreIndex = Gore.NewGore(new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
//				Main.gore[goreIndex].scale = 1.5f;
//				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
//				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
//			}

//			Projectile.width = 200;
//			Projectile.height = 200;
//			Projectile.position.X -= 100;
//			Projectile.position.Y -= 100;
//			Projectile.Damage();
//		}
//	}
//}