﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;
using Terraria.GameContent.ItemDropRules;
using Remnants.Content.World;
using Terraria.Localization;
using Remnants.Content.Items.Consumable;

namespace Remnants.Content.NPCs.Monsters.Undergrowth
{
    public class CentipedeHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Remnants/Content/NPCs/Monsters/Undergrowth/CentipedeBestiary",
                Position = new Vector2(0f, 36f),
                PortraitPositionXOverride = -12f,
                PortraitPositionYOverride = 20f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        bool TailSpawned = false;

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantWormHead);

            NPC.aiStyle = -1;

            NPC.width = 10;
            NPC.height = 18;

            NPC.lifeMax = 60;
            NPC.defense = 4;
            NPC.damage = 12;

            NPC.value = 100;

            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;

            NPC.buffImmune[BuffID.Poisoned] = true;

            SpawnModBiomes = new int[] { ModContent.GetInstance<Biomes.Undergrowth>().Type };
        }

        ref float aiTimer => ref NPC.ai[1];

        ref float stalkDuration => ref NPC.ai[2];

        ref float orbitDirection => ref NPC.ai[3];

        public override void AI()
        {
            #region segments
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    int thisSegment = NPC.whoAmI;
                    int segmentAmt = Main.rand.Next(20, 31);

                    for (int j = 0; j <= segmentAmt; j++)
                    {
                        int segmentType = j == segmentAmt ? ModContent.NPCType<CentipedeTail>() : ModContent.NPCType<CentipedeBody>();

                        int newSegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + NPC.width / 2), (int)(NPC.position.Y + NPC.height), segmentType, NPC.whoAmI);

                        Main.npc[newSegment].ai[3] = NPC.whoAmI;
                        Main.npc[newSegment].realLife = NPC.whoAmI;
                        Main.npc[newSegment].ai[1] = thisSegment;
                        Main.npc[thisSegment].ai[0] = newSegment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, newSegment, 0f, 0f, 0f, 0, 0, 0);
                        thisSegment = newSegment;
                    }
                    TailSpawned = true;
                }
            }
            #endregion

            #region behaviour
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            Player target = Main.player[NPC.target];
            Point point = NPC.Center.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(point);
            bool collision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
            int orbitRadius = 16 * (16 + NPC.whoAmI % 3 * 8);

            bool attacking = stalkDuration <= 0 && Vector2.Distance(NPC.Center, target.Center) < orbitRadius + 64;

            if (collision || tile.WallType != 0 || point.Y > Main.worldSurface && point.Y < Main.maxTilesY - 200)
            {
                if (target.InModBiome<Biomes.Undergrowth>() || target.Center.Y / 16 > Main.worldSurface)
                {
                    Vector2 destination = attacking ? target.Center : target.Center + new Vector2((float)Math.Sin((aiTimer + NPC.whoAmI * 50) % 300 / (300 / MathHelper.TwoPi)), (float)Math.Cos((aiTimer + NPC.whoAmI * 50) % 300 / (300 / MathHelper.TwoPi)) * orbitDirection) * orbitRadius;
                    NPC.velocity += Vector2.Normalize(destination - NPC.Center) / (attacking ? 5 : 9);
                }
                else NPC.velocity.Y += 1f / 9;

                NPC.velocity *= attacking ? 0.96f : 0.98f;

                if (collision && NPC.soundDelay == 0)
                {
                    NPC.soundDelay = (int)Math.Clamp(Vector2.Distance(NPC.Center, target.Center) / 40f, 10, 20);

                    SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
                }
            }
            else
            {
                NPC.velocity.Y += tile.LiquidAmount > 64 ? 0.05f : 0.1f;
                NPC.velocity *= 0.99f;
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            #endregion

            #region aistate
            aiTimer++;

            if (stalkDuration <= 0)
            {
                if (aiTimer >= 60 + NPC.whoAmI % 3 * 30)
                {
                    aiTimer = 0;
                    stalkDuration = Main.rand.Next(300, 601);
                    orbitDirection = Main.rand.NextBool(2) ? -1 : 1;

                    NPC.netUpdate = true;
                }
            }
            else
            {
                if (aiTimer >= stalkDuration)
                {
                    stalkDuration = 0;
                    aiTimer = 0;

                    NPC.netUpdate = true;
                }
            }
            #endregion

            //if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            //{
            //    NPC.netUpdate = true;
            //}
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Bezoar, 100));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < hit.Damage; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2, -1f);
                }
                return;
            }
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2, -2f, NPC.alpha);
                if (Main.rand.NextBool(8))
                {
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/NPCs/Monsters/Undergrowth/CentipedeAntenna").Value, NPC.Center + new Vector2(3, -7).RotatedBy(NPC.rotation) - Main.screenPosition + Vector2.UnitY * 6, new Rectangle(0, 0, 20, 52), drawColor, NPC.rotation + (float)(Math.Sin(Main.GameUpdateCount / 5 + (3 + NPC.whoAmI % 3)) + 1) / 3, new Vector2(10, 26), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/NPCs/Monsters/Undergrowth/CentipedeAntenna").Value, NPC.Center + new Vector2(-3, -7).RotatedBy(NPC.rotation) - Main.screenPosition + Vector2.UnitY * 6, new Rectangle(0, 0, 20, 52), drawColor, NPC.rotation - (float)(Math.Sin(Main.GameUpdateCount / 5 + (3 + NPC.whoAmI * 3 % 3)) + 1) / 3, new Vector2(10, 26), 1f, SpriteEffects.FlipHorizontally, 0f);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Poisoned, 600, true);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.Centipede"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.Undergrowth>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class CentipedeBody : ModNPC
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.Remnants.NPCs.CentipedeHead.DisplayName");

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantWormBody);

            NPC.width = 10;
            NPC.height = 18;

            NPC.lifeMax = 60;
            NPC.defense = 8;
            NPC.damage = 8;

            NPC.buffImmune[BuffID.Poisoned] = true;

            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override void AI()
        {
            bool shouldDie = false;
            if (!Main.npc[(int)NPC.ai[1]].active || NPC.ai[1] <= 0f)
            {
                shouldDie = true;
            }
            else if (Main.npc[(int)NPC.ai[1]].life <= 0)
            {
                shouldDie = true;
            }
            if (shouldDie)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
            }
        }

        Vector2 oldPos;
        public override void FindFrame(int frameHeight)
        {
            if (oldPos != Vector2.Zero)
            {
                NPC.frameCounter += (Main.npc[(int)NPC.ai[3]].position - oldPos).Length();
            }

            oldPos = Main.npc[(int)NPC.ai[3]].position;

            NPC.frame.Y = (int)((NPC.frameCounter / 10 - NPC.whoAmI + Main.maxNPCs) % Main.npcFrameCount[Type]) * 14;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < hit.Damage; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2, -1f);
                }
                return;
            }
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2, -2f, NPC.alpha);
                if (Main.rand.NextBool(8))
                {
                    Main.dust[dust].noGravity = true;
                }
            }
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreKill()
        {
            return false;
        }
    }

    public class CentipedeTail : ModNPC
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.Remnants.NPCs.CentipedeHead.DisplayName");

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantWormTail);

            NPC.width = 10;
            NPC.height = 18;

            NPC.lifeMax = 60;
            NPC.defense = 8;
            NPC.damage = 8;

            NPC.buffImmune[BuffID.Poisoned] = true;

            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            bool shouldDie = false;
            if (!Main.npc[(int)NPC.ai[1]].active || NPC.ai[1] <= 0f)
            {
                shouldDie = true;
            }
            else if (Main.npc[(int)NPC.ai[1]].life <= 0)
            {
                shouldDie = true;
            }
            if (shouldDie)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < hit.Damage; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2, -1f);
                }
                return;
            }
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2, -2f, NPC.alpha);
                if (Main.rand.NextBool(8))
                {
                    Main.dust[dust].noGravity = true;
                }
            }
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreKill()
        {
            return false;
        }
    }
}
