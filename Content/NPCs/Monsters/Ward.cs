using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using Remnants.Content.Tiles.Objects.Hazards;
using Remnants.Content.Projectiles.Effects;
using Remnants.Content.Biomes;
using Remnants.Content.Walls;
using Remnants.Content.World;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects;
using Remnants.Content.Tiles.Plants;
using Remnants.Content.Tiles.Objects.Furniture;

namespace Remnants.Content.NPCs.Monsters
{
    public class Ward : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.TeleportationImmune[Type] = true;
            NPCID.Sets.NeedsExpertScaling[Type] = false;
        }

        public override void SetDefaults()
        {
            NPC.width = 176;
            NPC.height = 172;

            NPC.aiStyle = -1;
            //NPC.boss = true;

            NPC.lifeMax = 888888;
            NPC.damage = 8888;
            NPC.defense = 88;
            NPC.knockBackResist = 0f;
            NPC.value = 0f;

            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.chaseable = false;

            NPC.ShowNameOnHover = false;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath43;

            SpawnModBiomes = new int[] { ModContent.GetInstance<EchoingHalls>().Type };

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.TeleportationImmune[Type] = true;
            NPCID.Sets.NeedsExpertScaling[Type] = false;
        }

        public override bool CheckActive()
        {
            Player target = Main.player[NPC.target];
            return direction == 1 && target.Center.Y > NPC.Center.Y || direction == 3 && target.Center.Y < NPC.Center.Y || direction == 2 && target.Center.X < NPC.Center.X || direction == 4 && target.Center.X > NPC.Center.X;
        }

        ref float direction => ref NPC.ai[0];
        ref float collisionFrames => ref NPC.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            collisionFrames = -30;

            TeleportEffect(NPC, NPC.height/2);

            Tile tile = Main.tile[(NPC.Center / 16).ToPoint16()];
            if (tile.WallType != ModContent.WallType<LabyrinthTileWall>() && tile.WallType != ModContent.WallType<LabyrinthBrickWall>() && tile.WallType != ModContent.WallType<whisperingmaze>())
            {
                NPC.active = false;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest();

            if (NPC.collideX && (direction == 2 || direction == 4) || NPC.collideY && (direction == 1 || direction == 3) || direction == -1 || direction == 0)
            {
                collisionFrames++;
                if (collisionFrames == 1)
                {
                    if (direction > 0)
                    {
                        if (direction != 1)
                        {
                            NPC.velocity = Vector2.Zero;
                        }
                        else NPC.velocity.Y -= 4;

                        RemPlayer.ScreenShake(NPC.Center, 2);

                        if (Vector2.Distance(NPC.Center, Main.LocalPlayer.Center) <= 64 * 16)
                        {
                            SoundStyle impactSound = SoundID.Item62;
                            impactSound.PitchVariance = 0.5f;
                            SoundEngine.PlaySound(impactSound, position: NPC.Center);

                            //if (direction == 1)
                            //{
                            //    NPC.position.Y -= 8;
                            //}

                            for (int i = 0; i < 33; i++)
                            {
                                Dust dust = new Dust();
                                if (direction == 1)
                                {
                                    dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, 8, DustID.Smoke);
                                }
                                else if (direction == 2)
                                {
                                    dust = Dust.NewDustDirect(new Vector2(NPC.position.X + NPC.width, NPC.position.Y), 8, NPC.height, DustID.Smoke);
                                }
                                else if (direction == 3)
                                {
                                    dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y + NPC.height), NPC.width, 8, DustID.Smoke);
                                }
                                else if (direction == 4)
                                {
                                    dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), 8, NPC.height, DustID.Smoke);
                                }
                                dust.velocity = Main.rand.NextVector2Circular(1, 1);
                                dust.alpha = 102;
                                dust.scale = Main.rand.NextFloat(1, 2);
                            }
                        }
                    }
                    ChooseDirection();

                    if (direction == 2 || direction == 4)
                    {
                        if (NPC.width < 176)
                        {
                            NPC.width = 172; NPC.position.X -= 2;
                        }
                    }
                    else
                    {
                        if (NPC.width > 172)
                        {
                            NPC.width = 172; NPC.position.X += 2;
                        }
                    }
                }
                else if (collisionFrames >= 60)
                {
                    TeleportEffect(NPC);
                    NPC.active = false;
                    //if (pastCollisions.Contains(NPC.position))
                    //{
                    //    TeleportAway(NPC);
                    //}
                    //else ChooseDirection(false);
                }
            }
            else collisionFrames = 0;

            float speed = Main.masterMode ? 0.4f : Main.expertMode ? 0.35f : 0.3f + (Main.hardMode ? 0.1f : 0) + (NPC.downedMoonlord ? 0.1f : 0);
            if (direction == 1)
            {
                NPC.velocity.Y -= speed;
            }
            else if (direction == 2)
            {
                NPC.velocity.X += speed;
            }
            else if (direction == 3)
            {
                NPC.velocity.Y += speed;
            }
            else if (direction == 4)
            {
                NPC.velocity.X -= speed;
            }
            
            NPC.velocity *= 0.97f;
        }

        public override bool PreAI()
        {
            Rectangle area = new Rectangle((int)NPC.position.X / 16, (int)NPC.position.Y / 16, 11, 11);

            if (NPC.velocity != Vector2.Zero)
            {
                Rectangle target = new Rectangle((int)(NPC.position.X + NPC.velocity.X) / 16 - 1, (int)(NPC.position.Y + NPC.velocity.Y) / 16 - 1, 13, 13);

                //Dust.NewDustPerfect(target.TopLeft() * 16, DustID.Torch, Vector2.Zero);
                //Dust.NewDustPerfect(target.TopRight() * 16, DustID.Torch, Vector2.Zero);
                //Dust.NewDustPerfect(target.BottomLeft() * 16, DustID.Torch, Vector2.Zero);
                //Dust.NewDustPerfect(target.BottomRight() * 16, DustID.Torch, Vector2.Zero);

                //for (int j = (NPC.velocity.Y > 0 ? area.Bottom : target.Top); j < (NPC.velocity.Y < 0 ? area.Top : target.Bottom); j++)
                //{
                //    for (int i = (NPC.velocity.X > 0 ? area.Right : target.Left); i < (NPC.velocity.X < 0 ? area.Left : target.Right); i++)
                //    {
                //        AttemptDestroyTile(i, j);
                //    }
                //}
                for (int j = target.Top; j < area.Top; j++)
                {
                    for (int i = target.Left + 1; i < target.Right - 1; i++)
                    {
                        AttemptDestroyTile(i, j);
                    }
                }
                for (int j = area.Bottom; j < target.Bottom; j++)
                {
                    for (int i = target.Left + 1; i < target.Right - 1; i++)
                    {
                        AttemptDestroyTile(i, j);
                    }
                }

                for (int j = target.Top; j < target.Bottom; j++)
                {
                    for (int i = target.Left; i < area.Left; i++)
                    {
                        AttemptDestroyTile(i, j);
                    }
                }
                for (int j = target.Top; j < target.Bottom; j++)
                {
                    for (int i = area.Right; i < target.Right; i++)
                    {
                        AttemptDestroyTile(i, j);
                    }
                }
            }
            else
            {
                for (int j = area.Top - 1; j < area.Bottom + 1; j++)
                {
                    for (int i = area.Left - 1; i < area.Right + 1; i++)
                    {
                        AttemptDestroyTile(i, j);
                    }
                }
            }

            return true;
        }

        private void AttemptDestroyTile(int i, int j)
        {
            if (CanDestroyTile(Main.tile[i, j]))
            {
                WorldGen.KillTile(i, j);
            }
            else
            {
                Tile tile = Main.tile[i, j];
                if (tile.HasTile && tile.TileType == ModContent.TileType<LabyrinthWallLamp>())
                {
                    tile.TileFrameY = 24;
                }
            }
        }

        private bool CanDestroyTile(Tile tile)
        {
            if (!tile.HasTile || tile.WallType != ModContent.WallType<LabyrinthTileWall>() && tile.WallType != ModContent.WallType<LabyrinthBrickWall>() && tile.WallType != ModContent.WallType<whisperingmaze>())
            {
                return false;
            }
            if (Main.tileSolid[tile.TileType] && tile.TileType != ModContent.TileType<LabyrinthBrick>() && tile.TileType != ModContent.TileType<LabyrinthPlatform>())
            {
                return true;
            }
            if (Main.tileCut[tile.TileType] && tile.TileType != ModContent.TileType<LabyrinthVine>() && tile.TileType != ModContent.TileType<LabyrinthGrass>())
            {
                return true;
            }
            if (Main.tileLighted[tile.TileType] && tile.TileType != ModContent.TileType<LabyrinthWallLamp>() && tile.TileType != ModContent.TileType<LabyrinthAltar>())
            {
                return true;
            }

            return tile.TileType == TileID.Heart;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            NPC.life = NPC.lifeMax - 1;
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            NPC.life = NPC.lifeMax - 1;
        }

        private void ChooseDirection(bool stop = true)
        {
            List<int> directions = new List<int>();

            Rectangle location = new Rectangle((int)NPC.position.X / 16, (int)NPC.position.Y / 16, 11, 11);

            if ((direction != 3 || !stop) && !CollisionTop())//!Collision.SolidTiles(location.Left + 1, location.Right - 1, location.Top - 2, location.Top - 2))
            {
                directions.Add(1);
            }
            if ((direction != 4 || !stop) && !CollisionRight())
            {
                directions.Add(2);
            }
            if ((direction != 1 || !stop) && !CollisionBottom())
            {
                directions.Add(3);
            }
            if ((direction != 2 || !stop) && !CollisionLeft())
            {
                directions.Add(4);
            }

            if (directions.Count > 0)
            {
                if (directions.Contains(1) && directions.Contains(3))
                {
                    directions.Remove(Main.player[NPC.target].Center.Y > NPC.Center.Y ? 1 : 3);
                }
                if (directions.Contains(2) && directions.Contains(4))
                {
                    directions.Remove(Main.player[NPC.target].Center.X < NPC.Center.X ? 2 : 4);
                }

                direction = directions[WorldGen.genRand.Next(directions.Count)];
            }
            else
            {
                direction = -1;
            }
        }

        private void TeleportEffect(NPC npc, int yOffset = 0)
        {
            Projectile.NewProjectileDirect(Terraria.Entity.GetSource_None(), npc.Center + Vector2.UnitY * yOffset, Vector2.Zero, ModContent.ProjectileType<MazeGuardianTeleport>(), 0, 0);
            SoundEngine.PlaySound(new SoundStyle("Remnants/Content/Sounds/starspawnspellcharge"), npc.Center);
        }

        private bool CollisionLeft()
        {
            return Collision.SolidCollision(new Vector2(NPC.getRect().Left - 16, NPC.position.Y + 16), 16, NPC.height - 32);
        }
        private bool CollisionRight()
        {
            return Collision.SolidCollision(new Vector2(NPC.getRect().Right, NPC.position.Y + 16), 16, NPC.height - 32);
        }
        private bool CollisionTop()
        {
            return Collision.SolidCollision(new Vector2(NPC.position.X + 16, NPC.getRect().Top - 16), NPC.width - 32, 16);
        }
        private bool CollisionBottom()
        {
            return Collision.SolidCollision(new Vector2(NPC.position.X + 16, NPC.getRect().Bottom), NPC.width - 32, 16);
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int num351 = 0; num351 < hit.Damage / 4; num351++)
                {
                    int num352 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, hit.HitDirection * 2, -1f);
                    //if (Main.rand.NextBool(8))
                    //{
                    //    Main.dust[num352].noGravity = true;
                    //}
                }
                return;
            }
            for (int num353 = 0; num353 < 40; num353++)
            {
                int num354 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, hit.HitDirection * 2, -2f, NPC.alpha, default, 1.2f);
                if (Main.rand.NextBool(8))
                {
                    Main.dust[num354].noGravity = true;
                }
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.Ward"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<EchoingHalls>().ModBiomeBestiaryInfoElement)
            });

            bestiaryEntry.UIInfoProvider = new MazeGuardianInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);
        }
    }

    internal class MazeGuardianInfoProvider : IBestiaryUICollectionInfoProvider
    {
        private readonly string _persistentIdentifierToCheck;

        public MazeGuardianInfoProvider(string persistentId)
        {
            _persistentIdentifierToCheck = persistentId;
        }

        public BestiaryUICollectionInfo GetEntryUICollectionInfo()
        {
            BestiaryEntryUnlockState unlockState = GetUnlockState(Main.BestiaryTracker.Kills.GetKillCount(_persistentIdentifierToCheck));
            BestiaryUICollectionInfo result = default;
            result.UnlockState = unlockState;
            return result;
        }

        public static BestiaryEntryUnlockState GetUnlockState(int kills)
        {
            //if (kills > 0)
            //{
            //    return BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
            //}
            //else
            if (RemWorld.sightedWard)
            {
                return BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
                //return BestiaryEntryUnlockState.CanShowPortraitOnly_1;
            }

            return BestiaryEntryUnlockState.NotKnownAtAll_0;
        }
    }
}
