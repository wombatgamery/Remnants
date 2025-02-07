using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Remnants.Biomes;
using Terraria.Audio;
using Remnants.Projectiles.Enemy;
using Terraria.DataStructures;
using System;

namespace Remnants.NPCs.Monsters.MagicalLab
{
    public class TomeofSummoning : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

			NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
		}



		public override void SetDefaults()
		{
			NPC.width = 22 * 2;
			NPC.height = 16 * 2;

			NPC.lifeMax = 60;
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

		Vector2 wanderAcceleration;
		ref float attackTimer => ref NPC.ai[0];
		float speed = 0.2f;
		float bouncyness = 1;

        public override void AI()
		{
            //bouncyness = aiState == 0 ? 1 : 0.5f;

            if (NPC.collideX)
            {
                NPC.velocity.X *= -bouncyness;
                //wanderVelocity.X *= -bouncyness;
                wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
                NPC.netUpdate = true;
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y *= -bouncyness;
                //wanderVelocity.Y *= -bouncyness;
                wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
                NPC.netUpdate = true;
            }

			if (Main.rand.NextBool((int)(3 / speed)))
			{
				wanderAcceleration = Main.rand.NextVector2Circular(speed, speed);
			}
            if (Main.rand.NextBool((int)(3 / speed)))
            {
                wanderAcceleration = Vector2.Zero;
            }
            NPC.velocity += wanderAcceleration;


            NPC.TargetClosest();
            if (!CanSeePlayer(NPC) || Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) > 64 * 16)
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
                if (Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) <= 8 * 16)
                {
                    NPC.velocity -= Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * speed;
                }
                else if (Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) >= 16 * 16)
                {
                    NPC.velocity += Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * speed;
                }


                if (++attackTimer == 120)
                {

					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC npc = Main.npc[i];

						if (npc.active && npc.life > 5 && !npc.friendly && !npc.noTileCollide && !NPCID.Sets.TeleportationImmune[npc.type] && !Collision.CanHit(Main.player[NPC.target].Center, 1, 1, npc.Center, 1, 1))
						{
                            for (int k = 0; k < 100; k++)
                            {
                                Dust dust = Dust.NewDustPerfect(npc.Center, DustID.ShimmerTorch, Main.rand.NextVector2Circular(10, 10), Scale: Main.rand.Next(1, 3));
                                dust.noGravity = true;
                            }
                            SoundEngine.PlaySound(SoundID.Item8, npc.Center);

                            npc.position = new Vector2(NPC.Center.X - npc.width / 2, NPC.Center.Y - npc.height / 2);

                            for (int k = 0; k < 100; k++)
                            {
                                Dust dust = Dust.NewDustPerfect(npc.Center, DustID.ShimmerTorch, Main.rand.NextVector2Circular(10, 10), Scale: Main.rand.Next(1, 3));
                                dust.noGravity = true;
                            }
                            SoundEngine.PlaySound(SoundID.Item8, npc.Center);

                            break;
						}
					}

                    attackTimer = 0;
                }
            }

            //for (int i = 0; i < 3; i++)
            //{
            //    for (int k = 0; k < 2; k++)
            //    {
            //        Vector2 offset = Vector2.UnitX * 32;
            //        Dust dust = Dust.NewDustDirect(NPC.Center - Vector2.One * 2.5f + offset.RotatedBy(Main.GameUpdateCount * 0.15f + i * MathHelper.TwoPi * (1 / 3f)), 5, 5, DustID.Torch, 0, 0);
            //        dust.noGravity = true;
            //    }
            //}

            if (Main.player[NPC.target].Center.X < NPC.Center.X)
            {
                NPC.direction = -1;
            }
            else NPC.direction = 1;
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
			return Collision.CanHit(npc.Center - Vector2.One / 2, 1, 1, Main.player[NPC.target].Center - Vector2.One / 2, 1, 1);
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
				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, Scale: Main.rand.Next(1, 3));
				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.TomeofSummoning"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
			});
		}
	}
}
