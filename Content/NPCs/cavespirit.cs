//using Terraria;
//using Terraria.ModLoader;
//using Terraria.ID;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Remnants.Content.Dusts;
//using Terraria.ModLoader.Utilities;
//using Terraria.GameContent.Bestiary;

//namespace Remnants.Content.NPCs
//{
//	public class cavespirit : ModNPC
//	{
//		public override void SetStaticDefaults()
//		{
//			DisplayName.SetDefault("Lost Spirit");
//			Main.npcFrameCount[NPC.type] = 4;
//		}

//        public override void FindFrame(int frameHeight)
//        {
//			if (++NPC.frameCounter > 5)
//            {
//				NPC.frameCounter = 0;

//				NPC.frame.Y += 18;
//				if (NPC.frame.Y >= 18 * Main.npcFrameCount[Type])
//                {
//					NPC.frame.Y = 0;
//                }
//			}
//        }

//        public float concentration = 1;

//		public override void SetDefaults()
//		{
//			NPC.width = 16;
//			NPC.height = 16;
//			NPC.lifeMax = 20;
//			NPC.damage = 35;
//			NPC.defense = 0;
//			NPC.HitSound = SoundID.NPCHit49;
//			NPC.DeathSound = SoundID.NPCDeath51;
//			NPC.value = 200f;
//			NPC.knockBackResist = 1f;
//			NPC.aiStyle = 10;
//			AIType = NPCID.CursedSkull;
//			//AnimationType = NPCID.Firefly;

//			NPC.noTileCollide = true;
//			NPC.noGravity = true;
//			NPC.lavaImmune = true;

//			NPC.buffImmune[BuffID.OnFire] = true;
//			NPC.buffImmune[BuffID.CursedInferno] = true;
//			NPC.buffImmune[BuffID.OnFire3] = true;
//			NPC.buffImmune[BuffID.Frostburn] = true;
//			NPC.buffImmune[BuffID.Frostburn2] = true;
//			NPC.buffImmune[BuffID.Poisoned] = true;
//			NPC.buffImmune[BuffID.Venom] = true;
//			NPC.buffImmune[BuffID.Bleeding] = true;
//			NPC.buffImmune[BuffID.Electrified] = true;
//			NPC.buffImmune[BuffID.Oiled] = true;
//		}

//        public override void DrawEffects(ref Color drawColor)
//        {
//			if (NPC.life < NPC.lifeMax)
//			{
//				concentration = 0;
//			}
//			else concentration = 1;

//			if (Main.rand.NextFloat(2 + (2 * concentration)) < 2)
//            {
//                int dustIndex = Dust.NewDust(NPC.Center + Main.rand.NextVector2Circular(8, 8) * concentration, 0, 0, ModContent.DustType<spiritenergy>(), Scale: Main.rand.NextFloat(0.5f, 2));
//				Main.dust[dustIndex].velocity = NPC.velocity / 2;
//            }
//			Terraria.Lighting.AddLight(NPC.Center, (150f / 255f), (255f / 255f), (255f / 255f));
//		}

//		public override void HitEffect(int hit.HitDirection, double damage)
//        {
//			for (int i = 0; i < 5; i++)
//            {
//				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<spiritenergy>(), Scale: Main.rand.NextFloat(0.5f, 2));
//				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//			}
//			for (int i = 0; i < 5; i++)
//			{
//				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Scale: Main.rand.Next(1, 3));
//				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//			}
//		}
//        public override void OnKill()
//		{
//			for (int i = 0; i < 10; i++)
//			{
//				int goreIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
//				Main.gore[goreIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//			}
//			for (int i = 0; i < 5; i++)
//			{
//				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Scale: Main.rand.Next(1, 3));
//				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//			}
//        }

//		public override void OnHitPlayer(Player target, int damage, bool crit)
//        {
//			int buffLength = (Main.expertMode ? 12 : 6) * 60;

//			target.AddBuff(BuffID.Slow, buffLength);
//			target.AddBuff(BuffID.Weak, buffLength);
//			target.AddBuff(BuffID.Blackout, buffLength);
//		}

//        public override float SpawnChance(NPCSpawnInfo spawnInfo)
//        {
//			return SpawnCondition.Cavern.Chance * 0.02f;
//        }

//        public override Color? GetAlpha(Color drawColor)
//		{
//			return Color.White;
//		}

//		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
//		{
//			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

//				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
//				new FlavorTextBestiaryInfoElement("An old resident of the underground kingdom, now a bodiless soul wandering the caves eternally, it drains life energy from its targets."),
//			});
//		}
//	}
//}
