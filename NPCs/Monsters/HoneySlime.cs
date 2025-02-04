using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Dusts;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Remnants.Biomes;

namespace Remnants.NPCs.Monsters
{
    public class HoneySlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Honey Slime");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.BlueSlime);

            NPC.lifeMax = 75;
            NPC.damage = 15;
            NPC.defense = 10;
            NPC.knockBackResist = 0.6f;

            NPC.scale = 1;// 1.2f;
            NPC.alpha = 50;
            NPC.color = default;

            NPC.value = 200f;

            NPC.buffImmune[20] = true;
            NPC.buffImmune[24] = true;

            AIType = NPCID.JungleSlime;
            AnimationType = NPCID.BlueSlime;

            SpawnModBiomes = new int[] { ModContent.GetInstance<Biomes.Hive>().Type };
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Honey, 15 * 60);
        }

        public override void AI()
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Honey, 0, 0, NPC.alpha, default, Scale: 1.2f);
            Main.dust[dust].velocity = NPC.velocity * 0.2f;
            if (Main.rand.NextBool(8))
            {
                Main.dust[dust].noGravity = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int num351 = 0; num351 < hit.Damage / NPC.lifeMax * 80.0; num351++)
                {
                    int num352 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Honey, hit.HitDirection * 2, -1f, NPC.alpha, default(Color), 1.2f);
                    if (Main.rand.NextBool(8))
                    {
                        Main.dust[num352].noGravity = true;
                    }
                }
                return;
            }
            for (int num353 = 0; num353 < 40; num353++)
            {
                int num354 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Honey, hit.HitDirection * 2, -2f, NPC.alpha, default(Color), 1.2f);
                if (Main.rand.NextBool(8))
                {
                    Main.dust[num354].noGravity = true;
                }
            }

            try
            {
                int num355 = (int)(NPC.Center.X / 16f);
                int num356 = (int)(NPC.Center.Y / 16f);
                if (!WorldGen.SolidTile(num355, num356) && Main.tile[num355, num356].LiquidAmount == 0)
                {
                    Main.tile[num355, num356].LiquidAmount = (byte)Main.rand.Next(50, 150);
                    Framing.GetTileSafely(num355, num356).LiquidType = 2;

                    WorldGen.SquareTileFrame(num355, num356);
                }
            }
            catch
            {
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("The honey absorbed by these slimes has greatly improved their vitality and endurance."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.Hive>().ModBiomeBestiaryInfoElement)
            });
        }
    }
}
