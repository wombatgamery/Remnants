//using Terraria;
//using Terraria.ModLoader;
//using Terraria.ID;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria.ModLoader.Utilities;
//using Terraria.GameContent.Bestiary;
//using Terraria.Audio;
//using Remnants.Content.Walls;
//using Remnants.Content.Walls.Vanity;
//using Remnants.Content.Biomes;
//using Remnants.Content.Gores;

//namespace Remnants.Content.NPCs.Monsters.TheVault
//{
//    public class Gunner : ModNPC
//    {
//        public Color glowColor;

//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("Gunner");
//            Main.npcFrameCount[NPC.type] = 1;
//        }



//        public override void SetDefaults()
//        {
//            NPC.width = 20 * 2;
//            NPC.height = 18 * 2;

//            NPC.lifeMax = 96;
//            NPC.damage = 0;
//            NPC.defense = 30;

//            NPC.HitSound = SoundID.NPCHit4;
//            NPC.DeathSound = SoundID.NPCDeath37;
//            NPC.value = 200f;
//            NPC.knockBackResist = 0.85f;
//            NPC.aiStyle = -1;

//            NPC.noGravity = true;
//            NPC.lavaImmune = true;

//            NPC.buffImmune[BuffID.OnFire] = true;
//            NPC.buffImmune[BuffID.CursedInferno] = true;
//            NPC.buffImmune[BuffID.OnFire3] = true;
//            NPC.buffImmune[BuffID.Frostburn] = true;
//            NPC.buffImmune[BuffID.Frostburn2] = true;
//            NPC.buffImmune[BuffID.Poisoned] = true;
//            NPC.buffImmune[BuffID.Venom] = true;
//            NPC.buffImmune[BuffID.Bleeding] = true;

//            SpawnModBiomes = new int[] { ModContent.GetInstance<Vault>().Type };
//        }

//        Vector2 lastKnownTargetPosition;
//        Vector2 wanderVelocity;
//        int aiState;
//        int attackTimer;
//        float speed = 0.25f;
//        float bouncyness = 1;

//        public override void AI()
//        {
//            //bouncyness = aiState == 0 ? 1 : 0.5f;

//            if (NPC.collideX)
//            {
//                NPC.velocity.X *= -bouncyness;
//                wanderVelocity.X *= -bouncyness;
//                NPC.netUpdate = true;
//            }
//            if (NPC.collideY)
//            {
//                NPC.velocity.Y *= -bouncyness;
//                wanderVelocity.Y *= -bouncyness;
//                NPC.netUpdate = true;
//            }

//            if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead)
//            {
//                NPC.TargetClosest();
//            }

//            if (Main.rand.NextBool((int)(5 / speed)))
//            {
//                wanderVelocity = Main.rand.NextVector2Circular(speed, speed);
//            }
//            if (Main.rand.NextBool((int)(10 / speed)))
//            {
//                wanderVelocity = Vector2.Zero;
//            }

//            if (aiState == 0)
//            {
//                //wanderVelocity.X = NPC.velocity.X < 0 ? -speed : speed;

//                NPC.velocity += wanderVelocity;
//                //NPC.velocity *= 0.95f;

//                if (CanSeePlayer(NPC))
//                {
//                    aiState = 1;
//                    SoundEngine.PlaySound(new SoundStyle("Remnants/Content/Sounds/droid4"), NPC.Center);
//                }

//                glowColor = new Color(0, 1, 0);
//            }
//            else if (aiState == 1)
//            {
//                if (!CanSeePlayer(NPC))
//                {
//                    NPC.velocity += Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;

//                    if (Vector2.Distance(NPC.Center, lastKnownTargetPosition) <= 16 || !CanSeePlayerLastPosition(NPC))
//                    {
//                        aiState = 0;
//                        SoundEngine.PlaySound(new SoundStyle("Remnants/Content/Sounds/droid1"), NPC.Center);
//                    }

//                    glowColor = new Color(1, 1, 0);
//                }
//                else
//                {
//                    lastKnownTargetPosition = Main.player[NPC.target].Center;

//                    if (Vector2.Distance(NPC.Center, lastKnownTargetPosition) <= 12 * 16)
//                    {
//                        NPC.velocity -= Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;
//                    }
//                    else if (Vector2.Distance(NPC.Center, lastKnownTargetPosition) >= 16 * 16)
//                    {
//                        NPC.velocity += Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;
//                    }

//                    if (attackTimer >= 15)
//                    {
//                        Vector2 num712 = (Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 10f).RotatedByRandom(0.1f);

//                        int num228 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, num712, ProjectileID.BulletSnowman, 20, 0f, Main.myPlayer);
//                        NPC.netUpdate = true;

//                        //NPC.velocity -= num712 / 10;

//                        SoundEngine.PlaySound(SoundID.Item41, NPC.Center);

//                        attackTimer = 0;
//                    }
//                    attackTimer++;

//                    glowColor = new Color(1, 0, 0);
//                }
//            }

//            if (aiState == 0)
//            {
//                if (NPC.velocity.X < 0)
//                {
//                    NPC.direction = -1;
//                }
//                else if (NPC.velocity.X > 0)
//                {
//                    NPC.direction = 1;
//                }
//            }
//            else
//            {
//                if (lastKnownTargetPosition.X < NPC.Center.X)
//                {
//                    NPC.direction = -1;
//                }
//                else NPC.direction = 1;
//            }
//            NPC.spriteDirection = NPC.direction;

//            NPC.velocity *= 0.96f;

//            Lighting.AddLight(NPC.Center, glowColor.R, glowColor.G, glowColor.B);
//        }

//        //      public override float SpawnChance(NPCSpawnInfo spawnInfo)
//        //      {
//        //	Tile tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
//        //	if (spawnInfo.Player.InModBiome<Vault>() && (tile.WallType == ModContent.WallType<vault>() || tile.WallType == ModContent.WallType<vaultwallunsafe>()))
//        //          {
//        //		return 0.1f;
//        //          }
//        //	return 0;
//        //}

//        private bool CanSeePlayer(NPC npc)
//        {
//            return !Main.player[npc.target].DeadOrGhost && Collision.CanHit(npc.Center - Vector2.One / 2, 1, 1, Main.player[npc.target].Center - Vector2.One / 2, 1, 1);
//        }

//        private bool CanSeePlayerLastPosition(NPC npc)
//        {
//            return Collision.CanHit(npc.Center - Vector2.One / 2, 1, 1, lastKnownTargetPosition - Vector2.One / 2, 1, 1);
//        }

//        public override bool? CanFallThroughPlatforms()
//        {
//            return true;
//        }

//        public override void DrawEffects(ref Color drawColor)
//        {
//            if (NPC.life <= NPC.lifeMax / 2)
//            {
//                if (Main.rand.NextBool(10))
//                {
//                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke);
//                    Main.dust[dustIndex].velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-1, -2));
//                }
//                if (Main.rand.NextBool(50))
//                {
//                    int goreIndex = Gore.NewGore(Terraria.Entity.GetSource_None(), NPC.Center, default, Main.rand.Next(61, 64), 1f);
//                    Main.gore[goreIndex].position = new Vector2(NPC.Center.X - Main.gore[goreIndex].Width / 2, NPC.Center.Y - Main.gore[goreIndex].Height / 2);
//                    Main.gore[goreIndex].velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-1, -2));
//                }
//            }
//        }

//        public override void HitEffect(NPC.HitInfo hit)
//        {
//            for (int i = 0; i < 3; i++)
//            {
//                int dustIndex = Dust.NewDust(NPC.Center, NPC.width, NPC.height, ModContent.DustType<InsectRemains>(), hit.HitDirection * (float)hit.Damage / 10);
//                Main.dust[dustIndex].velocity += Main.rand.NextVector2Circular(4, 4);
//            }
//        }
//        public override void OnKill()
//        {
//            for (int i = 0; i < 10; i++)
//            {
//                int goreIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, default, Main.rand.Next(61, 64), 1f);
//                Main.gore[goreIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//            }
//            for (int i = 0; i < 5; i++)
//            {
//                int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Scale: Main.rand.Next(1, 3));
//                Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//            }
//        }

//        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
//        {
//            Vector2 position = NPC.position - Main.screenPosition;
//            //position.Y += 4;
//            Rectangle rect = new Rectangle(0, 0, NPC.width, NPC.height);
//            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/NPCs/Monsters/TheVault/gunnerglow").Value, position, rect, glowColor * 255, NPC.rotation, Vector2.Zero, 1f, NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
//        }

//        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
//        {
//            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

//                new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.Gunner"),
//                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Vault>().ModBiomeBestiaryInfoElement)
//            });
//        }
//    }
//}
