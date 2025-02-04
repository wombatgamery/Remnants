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

			NPC.lifeMax = 200;
			NPC.damage = 0;
			NPC.defense = 4;

			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath56;
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

		Vector2 wanderAcceleration;
		ref float attackTimer => ref NPC.ai[0];
		float speed = 0.1f;
		bool aggro = false;

        public override void AI()
		{
			wanderAcceleration += Main.rand.NextVector2Circular(speed, speed) / 5;
			if (wanderAcceleration.Length() > speed)
			{
				wanderAcceleration = Vector2.Normalize(wanderAcceleration) * speed;
			}
			NPC.velocity += wanderAcceleration;

			NPC.TargetClosest();
			if (!aggro)
			{
				if (CanSeePlayer(NPC))
				{
					aggro = true;
				}
			}
			else if (!CanSeePlayer(NPC) || Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) > 64 * 16)
            {
				int x = (int)(Main.player[NPC.target].Center.X / 16) + Main.rand.Next(-32, 33);
				int y = (int)(Main.player[NPC.target].Center.Y / 16) + Main.rand.Next(-32, 33);
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
				if (valid && Vector2.Distance(pos + new Vector2(NPC.width, NPC.height) / 2, Main.player[NPC.target].Center) > 8 * 16 && Collision.CanHit(pos + new Vector2(NPC.width, NPC.height) / 2 - Vector2.One / 2, 1, 1, Main.player[NPC.target].Center - Vector2.One / 2, 1, 1))
                {
					for (int k = 0; k < 100; k++)
					{
						Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.ShimmerTorch, Main.rand.NextVector2Circular(10, 10), Scale: Main.rand.Next(1, 3));
						dust.noGravity = true;
					}
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
					NPC.position = pos;
					for (int k = 0; k < 100; k++)
					{
						Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.ShimmerTorch, Main.rand.NextVector2Circular(10, 10), Scale: Main.rand.Next(1, 3));
						dust.noGravity = true;
					}
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
				}
			}
			else
            {
				if (Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) <= 16 * 16)
				{
					NPC.velocity -= Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * speed;
				}
				else if (Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) >= 32 * 16)
				{
					NPC.velocity += Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * speed;
				}

				attackTimer++;
				if (attackTimer >= 240)
				{
					if (attackTimer == 240)
					{
						SoundStyle sound = SoundID.Item165;
						sound.MaxInstances = Main.maxNPCs;
						sound.PitchVariance = 1;
						SoundEngine.PlaySound(sound, NPC.Center);
					}
					else if (attackTimer == 300)
                    {
						SoundStyle sound = SoundID.Item164;
						sound.MaxInstances = Main.maxNPCs;
						SoundEngine.PlaySound(sound, NPC.Center);
					}

					if (attackTimer % 4 == 0)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, ModContent.ProjectileType<AetherButterfly>(), 20, 0f, Main.myPlayer);
					}
				}
				if (attackTimer >= 300)
				{
					attackTimer = 0;
				}
			}

            //for (int i = 0; i < 3; i++)
            //{
            //    for (int k = 0; k < 2; k++)
            //    {
            //        Vector2 offset = Vector2.UnitX * 48;
            //        Dust dust = Dust.NewDustDirect(NPC.Center - Vector2.One * 2.5f + offset.RotatedBy(Main.GameUpdateCount * 0.15f + i * MathHelper.TwoPi * (1 / 3f)), 5, 5, DustID.ShimmerTorch, 0, 0);
            //        dust.noGravity = true;
            //    }
            //}

            if (Main.player[NPC.target].Center.X < NPC.Center.X)
			{
				NPC.direction = -1;
			}
			else NPC.direction = 1;
			NPC.spriteDirection = NPC.direction;

			NPC.rotation = NPC.velocity.X / 20;

			NPC.velocity *= 0.98f;
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
			return !Main.player[npc.target].DeadOrGhost && !Main.player[npc.target].invis && Collision.CanHit(npc.Center - Vector2.One / 2, 1, 1, Main.player[npc.target].Center - Vector2.One / 2, 1, 1);
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

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				new FlavorTextBestiaryInfoElement("A once great scientist, disfigured and made savage by some failed experiment. While their intelligence and order remains, they will not hesitate to kill outsiders."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
			});
		}
	}
}
