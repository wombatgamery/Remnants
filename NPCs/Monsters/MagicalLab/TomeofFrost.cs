using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Remnants.Biomes;
using Remnants.Projectiles.Enemy;
using Terraria.DataStructures;
using System;

namespace Remnants.NPCs.Monsters.MagicalLab
{
    public class TomeofFrost : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}



		public override void SetDefaults()
		{
			NPC.width = 22 * 2;
			NPC.height = 16 * 2;

			NPC.lifeMax = 100;
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

		Vector2 lastKnownTargetPosition;
		Vector2 wanderVelocity;
		ref float aiState => ref NPC.ai[0];
		ref float attackTimer => ref NPC.ai[1];
		float speed = 0.2f;
		float bouncyness = 1;

        public override void AI()
		{
			//bouncyness = aiState == 0 ? 1 : 0.5f;

			if (NPC.collideX)
			{
				NPC.velocity.X *= -bouncyness;
				//wanderVelocity.X *= -bouncyness;
                wanderVelocity = Main.rand.NextVector2Circular(speed, speed);
                NPC.netUpdate = true;
			}
			if (NPC.collideY)
			{
				NPC.velocity.Y *= -bouncyness;
				//wanderVelocity.Y *= -bouncyness;
                wanderVelocity = Main.rand.NextVector2Circular(speed, speed);
                NPC.netUpdate = true;
			}

			if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead)
			{
				NPC.TargetClosest();
			}

			if (Main.rand.NextBool((int)(3 / speed)))
			{
				wanderVelocity = Main.rand.NextVector2Circular(speed, speed);
			}
            if (Main.rand.NextBool((int)(3 / speed)))
            {
                wanderVelocity = Vector2.Zero;
            }

            if (aiState == 0)
			{
				NPC.velocity += wanderVelocity;

				if (CanSeePlayer(NPC))
				{
					aiState = 1;
				}
			}
			else if (aiState == 1)
			{
				if (!CanSeePlayer(NPC))
				{
					if (Vector2.Distance(NPC.Center, lastKnownTargetPosition) <= 16 || !CanSeePlayerLastPosition(NPC))
					{
						aiState = 0;
					}
					else NPC.velocity += Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;
				}
				else
				{
					lastKnownTargetPosition = Main.player[NPC.target].Center;

					NPC.velocity += wanderVelocity * 0.5f;
					if (Vector2.Distance(NPC.Center, lastKnownTargetPosition) <= 16 * 16)
					{
						NPC.velocity -= Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;
					}
					else if (Vector2.Distance(NPC.Center, lastKnownTargetPosition) >= 32 * 16)
					{
						NPC.velocity += Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;
					}

					if (attackTimer <= 60)
                    {
						if (attackTimer == 0 || attackTimer == 10 || attackTimer == 20)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + Vector2.UnitX * Main.rand.Next(-48, 49), Vector2.Zero, ModContent.ProjectileType<IceSpike>(), 30, 0f, Main.myPlayer);
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

            //for (int i = 0; i < 3; i++)
            //{
            //    for (int k = 0; k < 2; k++)
            //    {
            //        Vector2 offset = Vector2.UnitX * 32;
            //        Dust dust = Dust.NewDustDirect(NPC.Center - Vector2.One * 2.5f + offset.RotatedBy(Main.GameUpdateCount * 0.15f + (i + 0.5f) * MathHelper.TwoPi * (1 / 3f)), 5, 5, DustID.IceTorch, 0, 0);
            //        dust.noGravity = true;
            //    }
            //}

            if (aiState == 0)
            {
				if (NPC.velocity.X < 0)
				{
					NPC.direction = -1;
				}
				else if (NPC.velocity.X > 0)
				{
					NPC.direction = 1;
				}
			}
			else
            {
				if (lastKnownTargetPosition.X < NPC.Center.X)
				{
					NPC.direction = -1;
				}
				else NPC.direction = 1;
			}
			NPC.spriteDirection = NPC.direction;

			NPC.rotation = NPC.velocity.X / 10 + (float)Math.Sin(Main.GameUpdateCount / 10f + NPC.whoAmI * 7) / 5;

			NPC.velocity *= 0.97f;
		}

		//      public override float SpawnChance(NPCSpawnInfo spawnInfo)
		//      {
		//	Tile tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
		//	if (spawnInfo.Player.InModBiome<Vault>() && (tile.WallType == ModContent.WallType<vault>() || tile.WallType == ModContent.WallType<vaultwallunsafe>()))
		//          {
		//		return 0.1f;
		//          }
		//	return 0;
		//}

		private bool CanSeePlayer(NPC npc)
		{
			return !Main.player[npc.target].DeadOrGhost && !Main.player[npc.target].shimmering && Collision.CanHit(npc.Center - Vector2.One / 2, 1, 1, Main.player[npc.target].Center - Vector2.One / 2, 1, 1);
		}

		private bool CanSeePlayerLastPosition(NPC npc)
		{
			return Collision.CanHit(npc.Center - Vector2.One / 2, 1, 1, lastKnownTargetPosition - Vector2.One / 2, 1, 1);
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

				new FlavorTextBestiaryInfoElement("Created as both protectors of the magical lab and repositories of its knowledge, these enchanted books cast their own spells on intruders."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
			});
		}
	}
}
