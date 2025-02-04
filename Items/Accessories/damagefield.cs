//using Microsoft.Xna.Framework;
//using Remnants.Dusts;
//using Remnants.NPCs;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Items.accessory
//{
//    public class damagefield : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("Kinetic Warp Field");
//            // Tooltip.SetDefault("Tears enemies apart at a molecular level, ignoring their defense, more effective when not in motion.");
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 11;
//            Item.height = 8;
//            Item.accessory = true;
//            Item.maxStack = 1;
//            Item.value = 100;
//            Item.rare = 0;
//        }

//        public float damageRadius = 8;
//        public float timer = 0;

//        public override void UpdateAccessory(Player player, bool hideVisual)
//        {
//            int dustIndex;

//            for (int i = 0; i < 3; i++)
//            {
//                dustIndex = Dust.NewDust(player.Center + Main.rand.NextVector2Circular(16f, 16f) * damageRadius, 0, 0, ModContent.DustType<redparticle>());
//                Main.dust[dustIndex].velocity = player.velocity;
//            }

//            for (int i = 0; i < 6; i++)
//            {
//                dustIndex = Dust.NewDust(player.Center + Main.rand.NextVector2CircularEdge(16f, 16f) * damageRadius, 0, 0, ModContent.DustType<redparticle>());
//                Main.dust[dustIndex].velocity = player.velocity;
//            }
//        }

//        public override void UpdateEquip(Player player)
//        {
//            if (timer <= 0)
//            {
//                timer = 5;
//                for (int k = 0; k < Main.maxNPCs; k++)
//                {
//                    NPC target = Main.npc[k];
//                    if (target.active && !target.dontTakeDamage && !target.friendly && target.lifeMax > 5)
//                    {
//                        int dmg = 2;
//                        if (!player.GetModPlayer<RemPlayer>().moving)
//                        {
//                            dmg *= 2;
//                        }
//                        dmg += target.defense / 2;
//                        //float targetSize = (target.width + target.height) / 2;
//                        if (Vector2.Distance(player.Center, target.Center) < damageRadius * 16)
//                        {
//                            NPC.HitInfo hit = new NPC.HitInfo();
//                            hit.Damage = dmg;
//                            target.StrikeNPC(hit);
//                        }
//                    }
//                }
//            }
//            timer--;
//        }
//    }
//}