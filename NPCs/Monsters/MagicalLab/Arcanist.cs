using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Remnants.Biomes;
using Remnants.Projectiles.Enemy;
using Terraria.DataStructures;
using Terraria.Audio;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Remnants.NPCs.Monsters.MagicalLab
{
    public class Arcanist : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 24 * 2;
			NPC.height = 32 * 2;

			NPC.lifeMax = 160;
			NPC.damage = 0;
			NPC.defense = 4;

			NPC.HitSound = SoundID.NPCHit37;
			NPC.DeathSound = SoundID.NPCDeath40;
			NPC.value = 400f;
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
			Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.ShimmerTorch, 0, 0);
			dust.noGravity = true;
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
			for (int i = 0; i < hit.Damage; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.ShimmerTorch, 0, 0, 100);
				dust.velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(2, 2);
			}
			for (int i = 0; i < hit.Damage; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Blood, 0, 0, 100);
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
				Dust dust = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Smoke, Alpha: 100, Scale: Main.rand.Next(1, 3));
				dust.velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
			}
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.ShimmerTorch, Scale: Main.rand.Next(1, 3));
				dust.velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
			}
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height * 0.5f);
            Vector2 drawPos = NPC.position - Main.screenPosition + drawOrigin;// + new Vector2(0f, NPC.gfxOffY);

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Eyes").Value, drawPos, NPC.frame, Color.White, NPC.rotation, drawOrigin, NPC.scale, NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				new FlavorTextBestiaryInfoElement("Once brilliant scientists, disfigured and made ruthless by a failed experiment - a demise of their own making. Regardless, their aptitude for magic remains, and they will not hesitate to use it."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
			});
		}
	}

    public class ArcanistAI : EnemyAI
    {
        public override bool AffectsModdedNPCS => true;

        public override float bouncyness => 1;

        public override bool IsValidNPC(NPC npc)
        {
            return npc.type == ModContent.NPCType<Arcanist>();
        }

        public override void ConstantBehaviour(NPC npc)
        {
            speed = 0.1f;

            wanderAcceleration += Main.rand.NextVector2Circular(speed, speed) / 5;
            if (wanderAcceleration.Length() > speed)
            {
                wanderAcceleration = Vector2.Normalize(wanderAcceleration) * speed;
            }
            npc.velocity += wanderAcceleration;

            npc.velocity *= 0.98f;
        }

        public override void SetDirection(NPC npc)
        {
            if (Main.player[npc.target].Center.X < npc.Center.X)
            {
                npc.direction = -1;
            }
            else npc.direction = 1;
            npc.spriteDirection = npc.direction;

            npc.rotation = npc.velocity.X / 20;
        }

        bool aggro = false;

        public override void AIState_Passive(NPC npc)
        {
            if (!aggro)
            {
                if (CanSeeTarget(npc) || npc.life < npc.lifeMax)
                {
                    aggro = true;
                }
            }
            else if (attackTimer < 240)
            {
                int x = (int)(Main.player[npc.target].Center.X / 16) + Main.rand.Next(-32, 33);
                int y = (int)(Main.player[npc.target].Center.Y / 16) + Main.rand.Next(-32, 33);
                bool valid = true;

                for (int j = y; j <= y + 3; j++)
                {
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType] || Main.tile[i, j].LiquidAmount == 255)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (!valid)
                    {
                        break;
                    }
                }


                Vector2 pos = new Vector2(x - 1, y) * 16;
                if (valid && Vector2.Distance(pos + new Vector2(npc.width, npc.height) / 2, Main.player[npc.target].Center) > 8 * 16 && LineOfSight(pos + new Vector2(npc.width, npc.height) / 2, Main.player[npc.target].Center))
                {
                    for (int k = 0; k < 100; k++)
                    {
                        Dust dust = Dust.NewDustPerfect(npc.Center, DustID.ShimmerTorch, Main.rand.NextVector2Circular(10, 10), Scale: Main.rand.Next(1, 3));
                        dust.noGravity = true;
                    }
                    SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                    npc.position = pos;
                    for (int k = 0; k < 100; k++)
                    {
                        Dust dust = Dust.NewDustPerfect(npc.Center, DustID.ShimmerTorch, Main.rand.NextVector2Circular(10, 10), Scale: Main.rand.Next(1, 3));
                        dust.noGravity = true;
                    }
                    SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                }
            }
        }

        public override void AIState_Hostile(NPC npc)
        {
            if (CanSeeTarget(npc))
            {
                if (Vector2.Distance(npc.Center, Main.player[npc.target].Center) <= 16 * 16)
                {
                    npc.velocity -= Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * speed;
                }
                else if (Vector2.Distance(npc.Center, Main.player[npc.target].Center) >= 32 * 16)
                {
                    npc.velocity += Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * speed;
                }

                if (attackTimer >= 240)
                {
                    if (attackTimer == 240)
                    {
                        SoundStyle sound = SoundID.Item165;
                        sound.MaxInstances = Main.maxNPCs;
                        sound.PitchVariance = 1;
                        SoundEngine.PlaySound(sound, npc.Center);
                    }
                    else if (attackTimer == 300)
                    {
                        SoundStyle sound = SoundID.Item164;
                        sound.MaxInstances = Main.maxNPCs;
                        SoundEngine.PlaySound(sound, npc.Center);
                    }

                    if (attackTimer % 4 == 0)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, ModContent.ProjectileType<AetherButterfly>(), 20, 0f, Main.myPlayer);
                    }
                }
                if (++attackTimer >= 300)
                {
                    attackTimer = 0;
                }
            }
            else aiState = 0;
        }

        public override bool CanSeeTarget(NPC npc)
        {
            return !Main.player[npc.target].DeadOrGhost && !Main.player[npc.target].invis && Vector2.Distance(npc.Center, Main.player[npc.target].Center) <= 64 * 16 && LineOfSight(npc.Center, Main.player[npc.target].Center);
        }
    }
}
