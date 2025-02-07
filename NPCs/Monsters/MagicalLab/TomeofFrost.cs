using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Remnants.Biomes;
using Remnants.Projectiles.Enemy;
using Terraria.DataStructures;
using System;
using Terraria.Audio;

namespace Remnants.NPCs.Monsters.MagicalLab
{
    public class TomeofFrost : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

            //NPCID.Sets.ShimmerTransformToNPC[Type] = ModContent.NPCType<TomeofSummoning>();

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}



		public override void SetDefaults()
		{
			NPC.width = 22 * 2;
			NPC.height = 16 * 2;

			NPC.lifeMax = 80;
			NPC.damage = 0;
			NPC.defense = 8;

			NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath35;
			NPC.value = 200f;
			NPC.aiStyle = -1;

			NPC.noGravity = true;

			SpawnModBiomes = new int[] { ModContent.GetInstance<Biomes.MagicalLab>().Type };
		}

		public override void FindFrame(int frameHeight)
		{
			if (++NPC.frameCounter > 2)
			{
				NPC.frameCounter = 0;

				NPC.frame.Y += NPC.height;
				if (NPC.frame.Y >= NPC.height * Main.npcFrameCount[Type])
				{
					NPC.frame.Y = 0;
				}
			}
		}

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool(3))
            {
				ModContent.GetInstance<TomeofMending>().SpawnTome(NPC);
            }
        }

		public override bool? CanFallThroughPlatforms()
		{
			return true;
		}

		public override void DrawEffects(ref Color drawColor)
		{
			Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 0, 0);
			dust.noGravity = true;
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
			for (int i = 0; i < hit.Damage; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.IceTorch, 0, 0, 100);
				dust.velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(2, 2);
			}
			for (int i = 0; i < hit.Damage / 2; i++)
            {
				Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.DynastyWall);
				dust.velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(2, 2);
			}
		}
        public override void OnKill()
		{
			for (int i = 0; i < 5; i++)
			{
				int goreIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[goreIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
			}
			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Alpha: 100, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
			}
			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.IceTorch, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.TomeofFrost"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
			});
		}
	}

    public class TomeofFrostAI : EnemyAI
    {
        public override bool AffectsModdedNPCS => true;

        public override float bouncyness => 1;

        public override bool IsValidNPC(NPC npc)
        {
            return npc.type == ModContent.NPCType<TomeofFrost>();
        }

        public override void ConstantBehaviour(NPC npc)
        {
            speed = 0.2f;

            if (Main.rand.NextBool((int)(3 / speed)))
            {
                wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
            }
            if (Main.rand.NextBool((int)(3 / speed)))
            {
                wanderAcceleration = Vector2.Zero;
            }

            npc.velocity *= 0.97f;
        }

        public override void SetDirection(NPC npc)
        {
            if (aiState == 0)
            {
                if (npc.velocity.X < 0)
                {
                    npc.direction = -1;
                }
                else if (npc.velocity.X > 0)
                {
                    npc.direction = 1;
                }
            }
            else
            {
                if (lastKnownTargetPosition.X < npc.Center.X)
                {
                    npc.direction = -1;
                }
                else npc.direction = 1;
            }
            npc.spriteDirection = npc.direction;

            npc.rotation = npc.velocity.X / 10 + (float)Math.Sin(Main.GameUpdateCount / 10f + npc.whoAmI * 7) / 5;
        }

        public override void AIState_Passive(NPC npc)
        {
            npc.velocity += wanderAcceleration;
        }

        public override void AIState_Hostile(NPC npc)
        {
            if (CanSeeTarget(npc))
            {
                npc.velocity += wanderAcceleration * 0.5f;
                if (Vector2.Distance(npc.Center, lastKnownTargetPosition) <= 16 * 16)
                {
                    npc.velocity -= Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                }
                else if (Vector2.Distance(npc.Center, lastKnownTargetPosition) >= 32 * 16)
                {
                    npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                }
            }
            else npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;

            if (attackTimer <= 60)
            {
                if (attackTimer == 0 || attackTimer == 10 || attackTimer == 20)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), Main.player[npc.target].Center + Vector2.UnitX * Main.rand.Next(-48, 49) + Vector2.UnitY * 16, Vector2.Zero, ModContent.ProjectileType<IceSpike>(), 25, 0f, Main.myPlayer);
                }

                //for (int i = 0; i < 3; i++)
                //{
                //	Dust dust = Dust.NewDustDirect(NPC.Center - Vector2.One * 2.5f + Main.rand.NextVector2CircularEdge(4, 4), 5, 5, DustID.IceTorch, 0, 0);
                //	dust.noGravity = true;
                //}
            }
            if (++attackTimer >= 120)
            {
                attackTimer = 0;
            }
        }
    }
}
