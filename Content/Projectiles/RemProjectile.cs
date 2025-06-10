using Microsoft.Xna.Framework;
using Remnants.Content.Walls;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
    public class RemProjectile : GlobalProjectile
    {
        //public override void SetDefaults(Projectile projectile)
        //{
        //    if (ModContent.GetInstance<Gameplay>().ProjectileAI)
        //    {
        //        if (projectile.type == ProjectileID.ThrowingKnife || projectile.type == ProjectileID.PoisonedKnife)
        //        {
        //            projectile.width = 5 * 2;
        //            projectile.height = 12 * 2;
        //            projectile.penetrate = 1;
        //        }
        //        if (projectile.type == ProjectileID.MolotovCocktail || projectile.type == ProjectileID.BoneJavelin)
        //        {
        //            projectile.alpha = 0;
        //        }
        //        if (projectile.type == ProjectileID.Bone)
        //        {
        //            projectile.scale = 1;
        //        }
        //        if (projectile.type == ProjectileID.BoneDagger)
        //        {
        //            projectile.penetrate = 2;
        //        }
        //        if (projectile.type == ProjectileID.BoneJavelin)
        //        {
        //            projectile.aiStyle = 1;
        //        }
        //    }
        //}

        //public override void OnSpawn(Projectile projectile, IEntitySource source)
        //{
        //    if (ModContent.GetInstance<Gameplay>().ProjectileAI && projectile.hostile)
        //    {
        //        if (projectile.type == ProjectileID.JavelinHostile)
        //        {
        //            projectile.velocity.Y -= Main.rand.NextFloat(2, 4);
        //        }
        //        if (projectile.aiStyle == 2)
        //        {
        //            projectile.velocity.Y -= Main.rand.NextFloat(3, 5);
        //        }
        //    }
        //}

        //public override bool PreAI(Projectile projectile)
        //{
        //    if (ModContent.GetInstance<Gameplay>().ProjectileAI)
        //    {
        //        if (projectile.aiStyle == 1 && (projectile.type == ProjectileID.JavelinFriendly || projectile.type == ProjectileID.JavelinHostile || projectile.type == ProjectileID.BoneJavelin))
        //        {
        //            projectile.velocity.Y += 0.15f;
        //            projectile.rotation = projectile.velocity.ToRotation();
        //            projectile.rotation += MathHelper.PiOver2;

        //            return false;
        //        }
        //        else if (projectile.aiStyle == 2 && (projectile.DamageType == DamageClass.Ranged || projectile.type == ProjectileID.MagicDagger))// && !projectile.hostile || projectile.aiStyle == 68)
        //        {
        //            projectile.velocity.Y += 0.2f;

        //            if (projectile.velocity.X < 0)
        //            {
        //                projectile.rotation -= 0.5f;// (float)Math.Sqrt((projectile.velocity.X * projectile.velocity.X) + (projectile.velocity.Y * projectile.velocity.Y)) * 0.05f;
        //            }
        //            else
        //            {
        //                projectile.rotation += 0.5f;// (float)Math.Sqrt((projectile.velocity.X * projectile.velocity.X) + (projectile.velocity.Y * projectile.velocity.Y)) * 0.05f;
        //            }

        //            return false;
        //        }
        //    }

        //    return true;
        //}

        ////     public override bool PreKill(Projectile projectile, int timeLeft)
        ////     {
        ////if (ModContent.GetInstance<P2>().FriendlyExplosives)
        ////{
        ////	if (projectile.type == ProjectileID.BombSkeletronPrime || projectile.type == ProjectileID.RocketSkeleton || projectile.type == ProjectileID.SaucerMissile)
        ////	{
        ////		projectile.friendly = true;
        ////		projectile.hostile = true;
        ////	}
        ////}

        ////         return true;
        ////     }

        //public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        //{
        //    if (ModContent.GetInstance<Gameplay>().ProjectileAI && projectile.type == ProjectileID.BoneJavelin)
        //    {
        //        projectile.aiStyle = 113;
        //    }

        //    //if (projectile.type == ProjectileID.HolyWater)
        //    //{
        //    //             //if (target.type == NPCID.CorruptSlime || target.type == NPCID.Crimslime)
        //    //             //{

        //    //             //    target.type = NPCID.;
        //    //             //}
        //    //             if (target.type == NPCID.CorruptGoldfish || target.type == NPCID.CrimsonGoldfish)
        //    //             {
        //    //		target.type = NPCID.Goldfish;
        //    //		target.CloneDefaults(NPCID.Goldfish);
        //    //		target.life = target.lifeMax;
        //    //	}
        //    //	if (target.type == NPCID.BloodFeeder)
        //    //	{
        //    //		target.type = NPCID.Piranha;
        //    //		target.CloneDefaults(NPCID.Piranha);
        //    //		target.life = target.lifeMax;
        //    //	}

        //    //	if (target.type == NPCID.BloodCrawler)
        //    //	{
        //    //		target.type = NPCID.WallCreeper;
        //    //		target.CloneDefaults(NPCID.WallCreeper);
        //    //		target.life = target.lifeMax;
        //    //	}
        //    //	if (target.type == NPCID.BloodCrawlerWall)
        //    //	{
        //    //		target.type = NPCID.WallCreeperWall;
        //    //		target.CloneDefaults(NPCID.WallCreeperWall);
        //    //		target.life = target.lifeMax;
        //    //	}
        //    //}
        //    //if (projectile.type == ProjectileID.BloodWater)
        //    //{
        //    //	if (target.type == NPCID.Piranha)
        //    //	{
        //    //		target.type = NPCID.BloodFeeder;
        //    //		target.CloneDefaults(NPCID.BloodFeeder);
        //    //		target.life = target.lifeMax;
        //    //	}

        //    //	if (target.type == NPCID.WallCreeper)
        //    //	{
        //    //		target.type = NPCID.BloodCrawler;
        //    //		target.CloneDefaults(NPCID.BloodCrawler);
        //    //		target.life = target.lifeMax;
        //    //	}
        //    //	if (target.type == NPCID.WallCreeperWall)
        //    //	{
        //    //		target.type = NPCID.BloodCrawlerWall;
        //    //		target.CloneDefaults(NPCID.BloodCrawlerWall);
        //    //		target.life = target.lifeMax;
        //    //	}
        //    //}
        //}
    }
}