using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Remnants.Content.Items.Consumable;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Terraria.Audio;
using Remnants.Content.Projectiles.Enemy;
using Remnants.Content.Items.Materials;

namespace Remnants.Content.NPCs.Monsters.Undergrowth
{
    public class ResinSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.NeedsExpertScaling[Type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(48f, 0f),
                PortraitPositionXOverride = 48f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.BlueSlime);

            NPC.width = 32;

            NPC.lifeMax = 60;
            NPC.damage = 10;
            NPC.defense = 4;
            NPC.knockBackResist = 0.7f;

            NPC.alpha = 50;
            NPC.color = default;

            NPC.value = 50;

            NPC.DeathSound = SoundID.NPCDeath15;

            AIType = NPCID.JungleSlime;
            AnimationType = NPCID.BlueSlime;

            SpawnModBiomes = new int[] { ModContent.GetInstance<Biomes.Undergrowth>().Type };
        }

        int oldLife = 0;

        public override void AI()
        {
            if ((int)deathTimer > 1)
            {
                deathTimer--;
                if ((int)deathTimer == 1)
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    NPC.netUpdate = true;
                }
                else if (NPC.velocity.Y == 0)
                {
                    NPC.velocity.X *= 0.75f;
                }
            }
            else if (NPC.life <= NPC.lifeMax / 3f)
            {
                NPC.defense = 12;
            }
            else if (NPC.life <= NPC.lifeMax / 1.5f)
            {
                NPC.defense = 8;
            }
            else NPC.defense = 4;

            #region hpeffects
            if (NPC.life <= NPC.lifeMax / 3f && oldLife > NPC.lifeMax / 3f || NPC.life <= NPC.lifeMax / 1.5f && oldLife > NPC.lifeMax / 1.5f || deathTimer == 60)
            {
                SoundEngine.PlaySound(SoundID.Item51, NPC.Center);

                for (int num351 = 0; num351 < 10; num351++)
                {
                    int num352 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemAmber, 0, -1f, NPC.alpha);
                    if (Main.rand.NextBool(8))
                    {
                        Main.dust[num352].noGravity = true;
                    }
                }
            }

            oldLife = NPC.life;
            #endregion
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ResinGel>(), 1, 1, 2));
        }

        int deathTimer;

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone >= NPC.life)
            {
                DeathSequence();
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone >= NPC.life)
            {
                DeathSequence();
            }
        }

        private void DeathSequence()
        {
            deathTimer = 61;

            NPC.life = 1;
            NPC.damage = 0;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;

            NPC.netUpdate = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 32;

            if (deathTimer > 0)
            {
                NPC.frame.X = 96;
                NPC.frame.Y = Main.GameUpdateCount % 8 < 4 ? 0 : 26;
            }
            else if (NPC.life <= NPC.lifeMax / 3f)
            {
                NPC.frame.X = 64;
            }
            else if (NPC.life <= NPC.lifeMax / 1.5f)
            {
                NPC.frame.X = 32;
            }
            else NPC.frame.X = 0;
        }

        public override void OnKill()
        {
            for (int k = 0; k < 5; k++)
            {
                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ResinShrapnel>(), 10, 0);
                proj.velocity = Main.rand.NextVector2Circular(6, 6);
                if (proj.velocity.Y > 0)
                {
                    proj.velocity.Y *= -1;
                }
                proj.netUpdate = true;
            }

            for (int k = 0; k < 40; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemAmber, 0, -2f, NPC.alpha);
                if (Main.rand.NextBool(8))
                {
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                Vector2 drawPos = NPC.Center - Main.screenPosition;

                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPos - Vector2.UnitY * NPC.height / 2 + Vector2.UnitY * 3, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height / 2), NPC.scale, SpriteEffects.None, 0f);

                return false;
            }
            
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int k = 0; k < hit.Damage / 2; k++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemAmber, hit.HitDirection * 2, -1f, NPC.alpha);
                    if (Main.rand.NextBool(8))
                    {
                        Main.dust[dust].noGravity = true;
                    }
                }
                return;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Mods.Remnants.Bestiary.ResinSlime"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.Undergrowth>().ModBiomeBestiaryInfoElement)
            });
        }
    }
}
