using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.Projectiles.Enemy;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles.Weapons
{
	public class FragmentGrenade : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;

            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
		{
            Projectile.CloneDefaults(ProjectileID.Grenade);

            Projectile.timeLeft = 180;

            AIType = ProjectileID.Grenade;

            DrawOriginOffsetX = -1;
            DrawOriginOffsetY = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = oldVelocity.X * -0.4f;
            }

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.4f;
            }

            return false;
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.Resize(80, 80);

            Projectile.knockBack = 8f;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.Resize(22, 22);

            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.position);

            for (int k = 0; k < 20; k++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ResinShrapnel>(), 10, 0);
                proj.velocity = Main.rand.NextVector2Circular(8, 8);
                if (proj.velocity.Y > 0)
                {
                    proj.velocity.Y *= -1;
                }
                proj.position += Vector2.Normalize(proj.velocity) * 16;
                proj.netUpdate = true;
            }

            for (int k = 0; k < 40; k++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmber, 0, -2f, Projectile.alpha);
                if (Main.rand.NextBool(8))
                {
                    dust.noGravity = true;
                }
            }

            for (int i = 0; i < 30; i++)
            {
                var smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smoke.velocity *= 1.4f;
            }

            for (int j = 0; j < 20; j++)
            {
                var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                fireDust.noGravity = true;
                fireDust.velocity *= 7f;
                fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                fireDust.velocity *= 3f;
            }

            for (int k = 0; k < 2; k++)
            {
                float speedMulti = 0.4f;
                if (k == 1)
                {
                    speedMulti = 0.8f;
                }

                var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity += Vector2.One;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity.X -= 1f;
                smokeGore.velocity.Y += 1f;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity.X += 1f;
                smokeGore.velocity.Y -= 1f;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity -= Vector2.One;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.expertMode && target.type >= 13 && target.type <= 15)
            {
                modifiers.FinalDamage /= 5;
            }
        }
    }
}