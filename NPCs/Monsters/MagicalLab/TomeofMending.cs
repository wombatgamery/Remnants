using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System;
using Remnants.Projectiles.Enemy;

namespace Remnants.NPCs.Monsters.MagicalLab
{
    public class TomeofMending : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            //NPCID.Sets.ShimmerTransformToNPC[Type] = ModContent.NPCType<TomeofSummoning>();

            NPCID.Sets.NeedsExpertScaling[Type] = true;

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

        public void SpawnTome(NPC parent)
        {
            NPC npc = NPC.NewNPCDirect(NPC.GetSource_NaturalSpawn(), (int)(NPC.position.X / 16), (int)(NPC.position.Y), ModContent.NPCType<TomeofMending>());
            npc.position = parent.position;

            NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void DrawEffects(ref Color drawColor)
        {
            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0, 0);
            dust.noGravity = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < hit.Damage; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Smoke, 0, 0, 100);
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
                int goreIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
            }
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Alpha: 100);
                Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
            }
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.CursedTorch, Scale: Main.rand.Next(1, 3));
                Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.TomeofMending"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class TomeofMendingAI : EnemyAI
    {
        public override bool AffectsModdedNPCS => true;

        public override float bouncyness => 1;

        public override bool IsValidNPC(NPC npc)
        {
            return npc.type == ModContent.NPCType<TomeofMending>();
        }

        public int target;

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


            foreach (NPC index in Main.ActiveNPCs)
            {
                if (index.whoAmI != npc.whoAmI)
                {
                    if ((target == -1 || index.lifeMax - index.life > Main.npc[target].lifeMax - Main.npc[target].life) && !index.friendly && index.lifeMax > 5 && LineOfSight(npc.Center, index.Center))
                    {
                        target = index.whoAmI;
                    }
                }
            }
        }

        public override void AIState_Passive(NPC npc)
        {
            npc.velocity += wanderAcceleration;

            foreach (NPC index in Main.ActiveNPCs)
            {
                if (index.whoAmI != npc.whoAmI)
                {
                    if (!index.friendly && index.lifeMax > 5 && LineOfSight(npc.Center, index.Center))
                    {
                        target = index.whoAmI;
                        break;
                    }
                }
            }
        }

        public override void AIState_Hostile(NPC npc)
        {
            if (!CanSeeTarget(npc))
            {
                if (Vector2.Distance(npc.Center, lastKnownTargetPosition) <= 16 || !CanSeeLastKnownTargetPosition(npc))
                {
                    target = -1;
                }
                else npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
            }
            else
            {
                lastKnownTargetPosition = Main.npc[target].Center;

                npc.velocity += wanderAcceleration * 0.5f;
                if (Vector2.Distance(npc.Center, lastKnownTargetPosition) >= 4 * 16)
                {
                    npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                }

                if (Vector2.Distance(npc.Center, lastKnownTargetPosition) < 8 * 16)
                {
                    NPC targetNpc = Main.npc[target];
                    Dust dust;

                    if (targetNpc.life < targetNpc.lifeMax)
                    {
                        if (Main.GameUpdateCount % 3 == 0)
                        {
                            int amountHealed = Math.Min(1, targetNpc.lifeMax - targetNpc.life);

                            if (amountHealed > 0)
                            {
                                targetNpc.life += amountHealed;
                                targetNpc.HealEffect(amountHealed);
                            }
                        }
                        dust = Dust.NewDustDirect(targetNpc.position, targetNpc.width, targetNpc.height, DustID.CursedTorch, 0, 0);
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Vector2 offset = Vector2.UnitX * 32;
                            dust = Dust.NewDustDirect(targetNpc.Center - Vector2.One * 2.5f + offset.RotatedBy(Main.GameUpdateCount * 0.15f + i * MathHelper.TwoPi * (1 / 3f)), 5, 5, DustID.CursedTorch, 0, 0);
                            dust.noGravity = true;
                        }
                    }
                }
            }
        }

        public override bool CanSeeTarget(NPC npc)
        {
            return target != -1 && Main.npc[target].active && Main.npc[target].whoAmI != npc.whoAmI && LineOfSight(npc.Center, Main.npc[target].Center);
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
    }
}
