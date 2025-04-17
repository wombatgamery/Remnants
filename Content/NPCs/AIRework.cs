using Microsoft.Xna.Framework;
using Remnants.Content.Biomes;
using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Consumable;
using Remnants.Content.NPCs.Monsters;
using Remnants.Content.Walls;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.NPCs.Monsters.TheVault;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.NPCs.Monsters.MagicalLab;

namespace Remnants.Content.NPCs
{
    public enum EnemyTag
    {
        Melee,
        Ranged,
        Flying,
        Swimming,
        Blind,
        Bat,
        Hornet,
    }

    public class EnemyAI : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public virtual bool AffectsModdedNPCS => false;

        public Vector2 wanderAcceleration;
        public Vector2 lastKnownTargetPosition;
        public int aiState;
        public float speed;
        public int timer;
        public int attackTimer;

        public virtual float bouncyness => 0;

        public virtual bool IsValidNPC(NPC npc)
        {
            return false;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (IsValidNPC(npc) && (ModContent.GetInstance<Gameplay>().EnemyAI && npc.type <= NPCID.Count || AffectsModdedNPCS) && Main.netMode == NetmodeID.SinglePlayer)
            {
                npc.TargetClosest();

                lastKnownTargetPosition = Main.player[npc.target].Center;
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (IsValidNPC(npc) && (ModContent.GetInstance<Gameplay>().EnemyAI && npc.type <= NPCID.Count || AffectsModdedNPCS) && Main.netMode == NetmodeID.SinglePlayer)
            {
                timer++;

                if (npc.target < 0 || npc.target >= 255 || Main.player[npc.target].DeadOrGhost)
                {
                    npc.TargetClosest();
                }

                if (aiState == 0)
                {
                    AIState_Passive(npc);

                    if (npc.lifeMax > 5 && !Main.player[npc.target].DeadOrGhost && CanSeeTarget(npc))
                    {
                        aiState = 1;
                    }
                }
                else if (aiState == 1)
                {
                    AIState_Hostile(npc);

                    if (npc.type != ModContent.NPCType<TomeofMending>())
                    {
                        if (!Main.player[npc.target].DeadOrGhost && CanSeeTarget(npc))
                        {
                            lastKnownTargetPosition = Main.player[npc.target].Center;
                        }
                        else if (Vector2.Distance(npc.Center, lastKnownTargetPosition) <= 16 || !CanSeeLastKnownTargetPosition(npc))
                        {
                            aiState = 0;
                        }
                    }
                    //else
                    //            {
                    //	for (int i = 0; i < Main.maxNPCs; i++)
                    //	{
                    //		NPC _npc = Main.npc[i];
                    //		if (_npc.active)
                    //		{
                    //			if (!_npc.GetGlobalNPC<EnemyAI>().CanSeePlayerLastPosition(_npc))
                    //			{
                    //				_npc.GetGlobalNPC<EnemyAI>().lastKnownTargetPosition = npc.getRect().Center.ToVector2();
                    //			}
                    //		}
                    //		else break;
                    //	}
                    //}
                }
                else if (aiState == -1)
                {
                    if (npc.life < npc.lifeMax || Vector2.Distance(npc.Center, Main.player[npc.target].Center) <= 16 * 16)
                    {
                        aiState = 0;
                    }
                }

                WallBounce(npc);
                ConstantBehaviour(npc);
                SetDirection(npc);

                return false;
            }
            return true;
        }

        public virtual void AIState_Passive(NPC npc)
        {

        }

        public virtual void AIState_Hostile(NPC npc)
        {

        }

        public virtual void ConstantBehaviour(NPC npc)
        {

        }

        public void WallBounce(NPC npc)
        {
            if (npc.collideX)
            {
                npc.velocity.X *= -bouncyness;
                wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
                npc.netUpdate = true;
            }
            if (npc.collideY)
            {
                npc.velocity.Y *= -bouncyness;
                wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
                npc.netUpdate = true;
            }
        }

        public virtual bool WillSearchForPlayer(NPC npc)
        {
            return true;
        }

        public bool LineOfSight(Vector2 startPoint, Vector2 endPoint)
        {
            return Collision.CanHit(startPoint - Vector2.One / 2, 1, 1, endPoint - Vector2.One / 2, 1, 1);
        }

        public virtual bool CanSeeTarget(NPC npc)
        {
            if (!WillSearchForPlayer(npc))
            {
                return false;
            }
            if (npc.confused || Main.player[npc.target].invis || Main.player[npc.target].shimmering)
            {
                return false;
            }
            else if (npc.noTileCollide)
            {
                return true;
            }
            return LineOfSight(npc.Center, Main.player[npc.target].Center);
        }

        public bool CanSeeLastKnownTargetPosition(NPC npc)
        {
            if (npc.confused)
            {
                return false;
            }
            else if (npc.noTileCollide)
            {
                return true;
            }
            return LineOfSight(npc.Center, lastKnownTargetPosition);
        }

        public virtual void SetDirection(NPC npc)
        {

        }
    }

    public class Flyer : EnemyAI
    {
        int attackMode;

        public override bool IsValidNPC(NPC npc)
        {
            return npc.aiStyle == 2 || npc.aiStyle == 5 || npc.aiStyle == 14 && npc.type != NPCID.VampireBat || npc.aiStyle == 17 || npc.aiStyle == 44 && npc.type != NPCID.FlyingFish;
        }

        public override void AIState_Passive(NPC npc)
        {
            if ((HasTag(npc, eye) || npc.type == NPCID.Probe) && Main.dayTime)
            {
                if (npc.noTileCollide || Collision.CanHit(npc.position, npc.width, npc.height, new Vector2(npc.position.X, npc.position.Y - 16 * 8), npc.width, npc.height))
                {
                    npc.velocity += wanderAcceleration * 0.5f;
                    npc.velocity.Y -= speed;
                }
                else npc.velocity += wanderAcceleration;
            }
            else if (aiState == 0)
            {
                npc.velocity += wanderAcceleration;
            }
        }
        public override void AIState_Hostile(NPC npc)
        {
            if ((HasTag(npc, eye) || npc.type == NPCID.Probe) && Main.dayTime)
            {
                aiState = 0;
                return;
            }

            if (HasTag(npc, melee) && HasTag(npc, ranged))
            {
                if (HasTag(npc, bat))
                {
                    if (attackMode == 1 && timer >= 300)
                    {
                        attackMode = 0;
                    }
                }
                else
                {
                    if (timer > 300)
                    {
                        if (attackMode == 0)
                        {
                            attackMode = 1;
                            attackTimer = 0;
                        }
                        else attackMode = 0;

                        timer = 0;
                    }
                }
            }

            if (attackMode == 0)
            {
                npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                npc.velocity += wanderAcceleration * 0.1f;
                attackTimer = 0;
            }
            else if (attackMode == 1)
            {
                npc.velocity += wanderAcceleration * 0.5f;
                if (Vector2.Distance(npc.Center, lastKnownTargetPosition) <= 12 * 16)
                {
                    npc.velocity -= Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                }
                else if (Vector2.Distance(npc.Center, lastKnownTargetPosition) >= 36 * 16)
                {
                    npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                }

                if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.RedDevil)
                {
                    if (attackTimer == 80 && npc.type != NPCID.RedDevil || attackTimer == 90 && npc.type != NPCID.RedDevil || attackTimer == 100)
                    {
                        int spread = 100;
                        if (npc.type == NPCID.RedDevil)
                        {
                            spread /= 2;
                        }

                        Vector2 num712 = lastKnownTargetPosition - npc.Center + Main.rand.NextVector2Circular(spread, spread);

                        num712 *= 0.2f / num712.Length();

                        int num228;
                        if (npc.type == NPCID.RedDevil)
                        {
                            num228 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, num712, ProjectileID.UnholyTridentHostile, 21, 0f, Main.myPlayer);
                        }
                        else num228 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, num712, ProjectileID.DemonSickle, 21, 0f, Main.myPlayer);
                        Main.projectile[num228].timeLeft = 300;

                        npc.netUpdate = true;

                        if (attackTimer >= 100)
                        {
                            attackTimer = 0;
                        }
                    }
                }
                else if (npc.type == NPCID.Harpy)
                {
                    if (attackTimer >= 50)
                    {
                        int spread = 25;

                        Vector2 num712 = Vector2.Normalize(lastKnownTargetPosition - npc.Center + Main.rand.NextVector2Circular(spread, spread)) * 5;

                        int num228 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, num712, ProjectileID.HarpyFeather, 21, 0f, Main.myPlayer);
                        Main.projectile[num228].timeLeft = 300;

                        npc.netUpdate = true;

                        attackTimer = 0;
                    }
                }
                else if (HasTag(npc, hornet))
                {
                    if (attackTimer >= 150)
                    {
                        int spread = 25;

                        Vector2 num712 = Vector2.Normalize(lastKnownTargetPosition - npc.Center + Main.rand.NextVector2Circular(spread, spread)) * 7.5f;

                        int num21 = (int)(10f * npc.scale);
                        if (npc.type == NPCID.MossHornet)
                            num21 = (int)(30f * npc.scale);

                        int num228 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, num712, ProjectileID.Stinger, num21, 0f, Main.myPlayer);
                        Main.projectile[num228].timeLeft = 300;
                        npc.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.Item17, npc.Center);

                        attackTimer = 0;
                    }
                }
                else if (npc.type == NPCID.Corruptor)
                {
                    if (attackTimer >= 100)
                    {
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + npc.velocity.X), (int)(npc.Center.Y + npc.velocity.Y), 112);

                        attackTimer = 0;
                    }
                }
                else if (npc.type == NPCID.Probe)
                {
                    if (attackTimer >= 200 && Collision.CanHit(npc, Main.player[npc.target]))
                    {
                        Vector2 num712 = Vector2.Normalize(lastKnownTargetPosition - npc.Center) * 7.5f;

                        int num228 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, num712, ProjectileID.PinkLaser, 25, 0f, Main.myPlayer);
                        npc.netUpdate = true;

                        attackTimer = 0;
                    }
                }
                attackTimer++;
            }
        }

        public override void ConstantBehaviour(NPC npc)
        {
            if (npc.noTileCollide)
            {
                if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.alpha = 153;
                }
                else npc.alpha = 0;
            }

            wanderAcceleration += Main.rand.NextVector2Circular(speed, speed) * 0.2f;
            if (wanderAcceleration.Length() > speed)
            {
                wanderAcceleration = Vector2.Normalize(wanderAcceleration) * speed;
            }

            if (HasTag(npc, bat))
            {
                npc.velocity *= 0.97f;
            }
            else if (HasTag(npc, eye) || npc.type == NPCID.TheHungryII)
            {
                if (npc.type == NPCID.SleepyEye || npc.type == NPCID.SleepyEye2)
                {
                    npc.velocity *= 0.9925f;
                }
                else npc.velocity *= 0.985f;
            }
            else if (HasTag(npc, eater))
            {
                npc.velocity *= 0.99f;
            }
            else npc.velocity *= 0.98f;
        }

        public override bool WillSearchForPlayer(NPC npc)
        {
            return !HasTag(npc, eye) && npc.type != NPCID.Probe || !Main.dayTime;
        }

        public override void SetDirection(NPC npc)
        {
            bool radial = HasTag(npc, eye) || HasTag(npc, eater) || npc.type == NPCID.Probe || npc.type == NPCID.MeteorHead;
            bool point = HasTag(npc, ranged) || HasTag(npc, eater) || npc.aiStyle == 17 || npc.type == NPCID.MeteorHead;

            if (point)
            {
                if (radial)
                {
                    npc.rotation = (lastKnownTargetPosition - npc.Center).ToRotation();
                    if (npc.type != NPCID.Probe && npc.type != NPCID.MeteorHead)
                    {
                        npc.rotation -= MathHelper.PiOver2;
                    }
                }
                if (lastKnownTargetPosition.X < npc.Center.X)
                {
                    npc.direction = -1;
                    if (npc.type == NPCID.Probe || npc.type == NPCID.MeteorHead)
                    {
                        npc.rotation -= MathHelper.Pi;
                    }
                }
                else npc.direction = 1;
            }
            else if (npc.velocity != Vector2.Zero)
            {
                if (radial)
                {
                    npc.rotation = npc.velocity.ToRotation();
                    if (npc.type == NPCID.ServantofCthulhu)
                    {
                        npc.rotation -= MathHelper.PiOver2;
                    }
                    else npc.rotation += MathHelper.PiOver2;
                }
                if (npc.velocity.X < 0)
                {
                    npc.direction = -1;
                }
                else npc.direction = 1;
            }
            npc.spriteDirection = npc.direction;
        }

        private bool HasTag(NPC npc, bool[] tag)
        {
            return tag[npc.type];
        }

        #region tags
        bool[] melee;
        bool[] ranged;

        bool[] eye;
        bool[] bat;
        bool[] hornet;
        bool[] eater;
        #endregion

        public override void SetDefaults(NPC entity)
        {
            if (IsValidNPC(entity))
            {
                #region tags
                int arrayLength = NPCLoader.NPCCount;

                melee = new bool[arrayLength];
                ranged = new bool[arrayLength];

                eye = new bool[arrayLength];
                bat = new bool[arrayLength];
                hornet = new bool[arrayLength];
                eater = new bool[arrayLength];

                for (int id = 0; id < arrayLength; id++)
                {
                    if (entity.type == NPCID.DemonEye || entity.type == NPCID.DemonEye2 || entity.type == NPCID.CataractEye || entity.type == NPCID.CataractEye2 || entity.type == NPCID.SleepyEye || entity.type == NPCID.SleepyEye2 || entity.type == NPCID.DialatedEye || entity.type == NPCID.DialatedEye2 || entity.type == NPCID.GreenEye || entity.type == NPCID.GreenEye2 || entity.type == NPCID.PurpleEye || entity.type == NPCID.PurpleEye2 || entity.type == NPCID.ServantofCthulhu || entity.type == NPCID.WanderingEye || entity.type == NPCID.DemonEyeOwl || entity.type == NPCID.DemonEyeSpaceship)
                    {
                        eye[id] = true;
                        entity.noTileCollide = false;
                    }
                    if (entity.type == NPCID.CaveBat || entity.type == NPCID.JungleBat || entity.type == NPCID.Hellbat || entity.type == NPCID.GiantBat || entity.type == NPCID.IlluminantBat || entity.type == NPCID.IceBat || entity.type == NPCID.Lavabat || entity.type == NPCID.GiantFlyingFox || entity.type == NPCID.SporeBat)
                    {
                        bat[id] = true;
                    }
                    if (entity.type == NPCID.Hornet || entity.type == NPCID.MossHornet || entity.type >= 231 && entity.type <= 235)
                    {
                        hornet[id] = true;
                    }
                    if (entity.type == NPCID.EaterofSouls || entity.type == NPCID.Corruptor || entity.type == NPCID.Crimera || entity.type == NPCID.LittleEater || entity.type == NPCID.BigEater || entity.type == NPCID.LittleCrimera || entity.type == NPCID.BigCrimera)
                    {
                        eater[id] = true;
                    }

                    if (HasTag(entity, bat) || HasTag(entity, hornet) || entity.type == NPCID.Demon || entity.type == NPCID.VoodooDemon || entity.type == NPCID.Harpy || entity.type == NPCID.RedDevil || entity.type == NPCID.Corruptor || entity.type == NPCID.Probe)
                    {
                        ranged[id] = true;
                    }
                    if (!HasTag(entity, hornet) && entity.type != NPCID.Probe)
                    {
                        melee[id] = true;
                    }
                }
                #endregion

                entity.noGravity = true;

                if (HasTag(entity, melee) && !HasTag(entity, ranged))
                {
                    attackMode = 0;
                }
                else if (HasTag(entity, ranged) && !HasTag(entity, melee))
                {
                    attackMode = 1;
                }

                if (HasTag(entity, eye))
                {
                    speed = 0.075f;

                    if (entity.type == NPCID.DemonEye2 || entity.type == NPCID.CataractEye2 || entity.type == NPCID.SleepyEye2 || entity.type == NPCID.PurpleEye2)
                    {
                        speed *= 0.8f;
                    }
                    else if (entity.type == NPCID.DialatedEye2 || entity.type == NPCID.GreenEye2)
                    {
                        speed *= 1.25f;
                    }
                    if (entity.type == NPCID.SleepyEye || entity.type == NPCID.SleepyEye2)
                    {
                        speed *= 0.8f;
                        speed *= 0.5f;
                    }
                }
                else if (entity.type == NPCID.MeteorHead)
                {
                    speed = 0.025f;
                }
                else if (HasTag(entity, eater))
                {
                    speed = 0.05f;
                }
                else if (HasTag(entity, bat) || entity.type == NPCID.RedDevil)
                {
                    speed = 0.15f;
                }
                else if (entity.type == NPCID.TheHungryII)
                {
                    speed = 0.08f;
                }
                else if (entity.type == NPCID.Harpy)
                {
                    speed = 0.125f;
                }
                else// if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon)
                {
                    speed = 0.1f;
                }

                if (entity.aiStyle == 17)
                {
                    aiState = -1;
                }
                else if (entity.type == NPCID.PigronCorruption || entity.type == NPCID.PigronHallow || entity.type == NPCID.PigronCrimson)
                {
                    entity.noTileCollide = true;
                }
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (IsValidNPC(npc) && HasTag(npc, bat))
            {
                attackMode = 1;
                timer = 0;
            }
        }
    }

    public class Swimmer : EnemyAI
    {
        public override bool IsValidNPC(NPC npc)
        {
            return npc.aiStyle == 16 || npc.type == NPCID.FlyingFish || npc.aiStyle == 18;
        }

        public override void AIState_Passive(NPC npc)
        {
            if (npc.wet || npc.type == NPCID.FlyingFish && Main.raining)
            {
                if (npc.aiStyle == 18)
                {
                    npc.velocity += wanderAcceleration * 0.2f;
                    timer = 0;
                }
                else npc.velocity += wanderAcceleration * 0.5f;
            }
        }

        public override void AIState_Hostile(NPC npc)
        {
            if (npc.wet || npc.type == NPCID.FlyingFish && Main.raining)
            {
                if (npc.aiStyle == 18)
                {
                    npc.velocity += wanderAcceleration * 0.1f;
                    if (timer >= 120)
                    {
                        npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed * 20;
                        timer = 0;
                    }
                }
                else
                {
                    npc.velocity += Vector2.Normalize(lastKnownTargetPosition - npc.Center) * speed;
                    npc.velocity += wanderAcceleration * 0.1f;
                }
            }
        }

        public override void ConstantBehaviour(NPC npc)
        {
            npc.noGravity = npc.wet || npc.type == NPCID.FlyingFish && Main.raining;
            if (!npc.noGravity)
            {
                aiState = 0;
                if (npc.velocity.Y == 0)
                {
                    npc.velocity.X *= 0.8f;
                    float size = Main.rand.NextFloat(1);
                    npc.velocity.X += Main.rand.NextFloat(-3, 3) * size;
                    npc.velocity.Y -= size * 5;
                }
                else
                {
                    //npc.velocity.Y += 0.2f;
                    wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
                }
            }
            else
            {
                wanderAcceleration += Main.rand.NextVector2Circular(speed, speed) * 0.2f;
                if (wanderAcceleration.Length() > speed)
                {
                    wanderAcceleration = Vector2.Normalize(wanderAcceleration) * speed;
                }

                npc.velocity *= 0.96f;
            }
        }

        public override bool WillSearchForPlayer(NPC npc)
        {
            if (!Main.player[npc.target].wet && (npc.type != NPCID.FlyingFish || !Main.raining) || npc.type == NPCID.Shark && Main.player[npc.target].statLife >= Main.player[npc.target].statLifeMax2)
            {
                return false;
            }
            return true;
        }

        public override void SetDirection(NPC npc)
        {
            if (npc.aiStyle == 18 && (aiState == 1 || aiState == -1))
            {
                npc.rotation = (lastKnownTargetPosition - npc.Center).ToRotation();
                if (lastKnownTargetPosition.X < npc.Center.X)
                {
                    npc.direction = -1;
                }
                else npc.direction = 1;
            }
            else if (npc.velocity != Vector2.Zero)
            {
                npc.rotation = npc.velocity.ToRotation();
                if (npc.velocity.X < 0)
                {
                    npc.direction = -1;
                    if (npc.aiStyle != 18)
                    {
                        npc.rotation -= MathHelper.Pi;
                    }
                }
                else npc.direction = 1;
            }
            if (npc.aiStyle == 18)
            {
                npc.rotation += MathHelper.PiOver2;
            }
            npc.spriteDirection = npc.direction;
        }

        public override void SetDefaults(NPC entity)
        {
            if (IsValidNPC(entity))
            {
                speed = 0.5f;
                if (entity.type == NPCID.Goldfish || entity.type == NPCID.Shark || entity.type == NPCID.Arapaima)
                {
                    speed = 0.4f;
                }
                else if (entity.type == NPCID.AnglerFish)
                {
                    speed = 0.6f;
                }
                else if (entity.type == NPCID.FlyingFish)
                {
                    speed = 0.2f;
                }
                else if (entity.type == NPCID.BloodJelly)
                {
                    speed = 1;
                }
            }
        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (IsValidNPC(npc))
            {
                if (npc.aiStyle == 18)
                {
                    if (aiState == 1)
                    {
                        if (timer <= 30)
                        {
                            npc.frame.Y = frameHeight * 2;
                        }
                        else if (timer <= 105)
                        {
                            npc.frame.Y = frameHeight;
                        }
                        else npc.frame.Y = 0;
                    }
                }
            }
        }
    }
}