//using Terraria;
//using Terraria.ModLoader;
//using Terraria.ID;
//using Microsoft.Xna.Framework;
//using Terraria.GameContent.Bestiary;
//using Remnants.Content.Biomes;
//using Terraria.Audio;
//using Terraria.DataStructures;

//namespace Remnants.Content.NPCs.Monsters.MagicalLab
//{
//    public class IlluminantDagger : ModNPC
//	{
//        public override void SetStaticDefaults()
//        {
//			NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
//		}

//        public override void SetDefaults()
//		{
//			NPC.width = 22 * 2;
//			NPC.height = 16 * 2;

//			NPC.lifeMax = 150;
//			NPC.damage = 40;
//			NPC.defense = 16;
//			NPC.knockBackResist = 0.5f;

//			NPC.HitSound = SoundID.NPCHit4;
//			NPC.DeathSound = SoundID.NPCDeath6;
//			NPC.value = 200f;
//			NPC.aiStyle = -1;

//			NPC.noGravity = true;
//			NPC.noTileCollide = true;

//			SpawnModBiomes = new int[] { ModContent.GetInstance<Biomes.MagicalLab>().Type };
//		}

//		Vector2 lastKnownTargetPosition;
//		int attackTimer;
//		float speed = 0.1f;
//		float bouncyness = 1;

//        public override void AI()
//		{
//			if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead)
//			{
//				NPC.TargetClosest();
//			}

//			lastKnownTargetPosition = Main.player[NPC.target].Center;

//			if (++attackTimer >= 60)
//			{
//				if (attackTimer == 60)
//				{
//					NPC.velocity *= 0.5f;
//					NPC.velocity += Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * 8;
//					NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;

//					SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
//				}

//				if (attackTimer >= 120)
//				{
//					attackTimer = 0;
//				}
//				NPC.velocity *= 0.99f;
//			}
//			else
//            {
//				NPC.velocity += Vector2.Normalize(lastKnownTargetPosition - NPC.Center) * speed;
//				NPC.velocity *= 0.98f;

//				NPC.rotation += attackTimer / 100f;
//			}
//		}

//		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
//		{
//			attackTimer = 0;
//		}
//		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
//		{
//			attackTimer = 0;
//		}

//		public override void DrawEffects(ref Color drawColor)
//		{
//			Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.ShimmerTorch, 0, 0);
//			dust.noGravity = true;

//			drawColor = Color.White;
//		}

//        public override void HitEffect(NPC.HitInfo hit)
//        {
//			for (int i = 0; i < hit.Damage / 2; i++)
//			{
//				Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.ShimmerTorch, 0, 0);
//				dust.velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(2, 2);
//			}
//		}
//        public override void OnKill()
//		{
//			for (int i = 0; i < 5; i++)
//			{
//				int goreIndex = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
//				Main.gore[goreIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//			}
//			for (int i = 0; i < 10; i++)
//			{
//				int dustIndex = Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, Alpha: 100);
//				Main.dust[dustIndex].velocity = NPC.velocity / 2 + Main.rand.NextVector2Circular(4, 4);
//			}
//		}

//		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
//		{
//			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

//				new FlavorTextBestiaryInfoElement("Infused with sentience formed from the chaos aspect of Hallowed magic, the illuminant dagger seeks to sow chaos and mayhem by spilling the blood of those unfortunate enough to stumble into its warpath."),
//				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.MagicalLab>().ModBiomeBestiaryInfoElement)
//			});
//		}
//	}
//}
