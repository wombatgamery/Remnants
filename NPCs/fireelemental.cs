//using Terraria;
//using Terraria.ModLoader;
//using Terraria.ID;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Remnants.NPCs
//{
//	public class fireelemental : Terraria.ModLoader.ModNPC
//	{
//		public override void SetStaticDefaults()
//		{
//			DisplayName.SetDefault("Fire Elemental");
//			Main.npcFrameCount[NPC.type] = 2;
//		}

//		public override void SetDefaults()
//		{
//			NPC.width = 16;
//			NPC.height = 16;
//			NPC.lifeMax = 40;
//			NPC.damage = 30;
//			NPC.defense = 0;
//			NPC.HitSound = SoundID.NPCHit42;
//			NPC.DeathSound = SoundID.NPCDeath43;
//			NPC.value = 200f;
//			NPC.knockBackResist = 0f;
//			NPC.aiStyle = 10;
//			AIType = NPCID.CursedSkull;
//			AnimationType = NPCID.Firefly;

//			NPC.noTileCollide = true;
//			NPC.noGravity = true;
//			NPC.lavaImmune = true;

//			NPC.buffImmune[BuffID.Poisoned] = true;
//			NPC.buffImmune[BuffID.Venom] = true;
//			NPC.buffImmune[BuffID.Bleeding] = true;
//			NPC.buffImmune[BuffID.Electrified] = true;
//			NPC.buffImmune[BuffID.Oiled] = true;
//		}

//        public int frame = 0;
//		public int frameTimer = 0;

//        public override void DrawEffects(ref Color drawColor)
//        {
//			frameTimer++;
//			if (frameTimer > 2)
//			{
//				frameTimer = 0;
//				frame++;
//				if (frame > 1)
//				{
//					frame = 0;
//				}
//			}
//			NPC.frame.Y = frame * 20;

//			if (Main.rand.Next(2) == 0)
//            {
//                Dust.NewDust(new Vector2(NPC.Center.X - 2, NPC.Center.Y - 2), 4, 4, DustID.Torch, Scale: Main.rand.Next(1, 3));
//            }
//		}

//		public override void HitEffect(int hit.HitDirection, double damage)
//        {
//			for (int i = 0; i < 5; i++)
//            {
//				Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, Scale: Main.rand.Next(1, 4));
//			}
//        }
//        public override void OnKill()
//		{
//			for (int i = 0; i < 10; i++)
//			{
//				float dustvelocityX = NPC.velocity.X * Main.rand.NextFloat(0.8f, 1.2f);
//				float dustvelocityY = NPC.velocity.Y * Main.rand.NextFloat(0.8f, 1.2f);
//				Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, Scale: Main.rand.Next(1, 3), SpeedX: dustvelocityX, SpeedY: dustvelocityY);
//				dustvelocityX = NPC.velocity.X * Main.rand.NextFloat(0.8f, 1.2f);
//				dustvelocityY = NPC.velocity.Y * Main.rand.NextFloat(0.8f, 1.2f);
//				Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Scale: Main.rand.Next(1, 3), SpeedX: dustvelocityX, SpeedY: dustvelocityY);
//			}
//			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Gores/fireelemental").Type);
//		}

//		public override void OnHitPlayer(Player target, int damage, bool crit)
//        {
//			if (Main.expertMode)
//            {
//				target.AddBuff(BuffID.OnFire, 12 * 30);
//			}
//			else
//            {
//				target.AddBuff(BuffID.OnFire, 6 * 30);
//			}
//        }

//  //      public override float SpawnChance(NPCSpawnInfo spawnInfo)
//		//{
//		//	return SpawnCondition.Underworld.Chance * 0.5f;
//		//}

//		public override Color? GetAlpha(Color drawColor)
//		{
//			return Color.White;
//		}
//	}
//}
