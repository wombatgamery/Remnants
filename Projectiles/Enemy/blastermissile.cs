using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Projectiles.Enemy
{
    public class blastermissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Missile");
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        //The recording mode
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
        }

        float direction;

        public override void AI()
        {
            if (!Main.player[(int)Projectile.ai[1]].DeadOrGhost)
            {
                direction = (Main.player[(int)Projectile.ai[1]].Center - Projectile.Center).ToRotation();
            }

            Projectile.velocity += direction.ToRotationVector2() / 5;
            Projectile.rotation = direction;

            int dustIndex;

            if (Main.rand.NextBool(2))
            {
                dustIndex = Dust.NewDust(Projectile.Center + Projectile.velocity - Projectile.rotation.ToRotationVector2() * 4, 0, 0, DustID.Smoke, Scale: Main.rand.NextFloat(1, 2));
                Main.dust[dustIndex].velocity = -Vector2.Normalize(Projectile.velocity);
            }
            dustIndex = Dust.NewDust(Projectile.position + Projectile.velocity - Projectile.rotation.ToRotationVector2() * 4, 0, 0, DustID.Torch, Scale: Main.rand.NextFloat(1, 2));
            Main.dust[dustIndex].noGravity = true;
            Main.dust[dustIndex].velocity = -Vector2.Normalize(Projectile.velocity);

            Projectile.velocity *= 0.99f;
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

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: Main.rand.NextFloat(1, 2));
                Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(4, 4);
            }
            for (int i = 0; i < 5; i++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].position = new Vector2(Projectile.Center.X - Main.gore[goreIndex].Width / 2, Projectile.Center.Y - Main.gore[goreIndex].Height / 2);
                Main.gore[goreIndex].velocity = Main.rand.NextVector2Circular(2, 2);
            }
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.NextFloat(1, 2));
                Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(4, 4);
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            //Projectile.position.X += Projectile.width / 2;
            //Projectile.position.Y += Projectile.height / 2;

            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.position.X -= 32;
            Projectile.position.Y -= 32;

            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.Damage();
        }
    }
}