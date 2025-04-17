using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remnants.Content.NPCs.Bosses
{
	//public class bloodpillar : ModNPC
	//{
	//	public override void SetStaticDefaults()
	//	{
	//		DisplayName.SetDefault("Blood Pillar");
	//	}

	//	public override void SetDefaults()
	//	{
	//		NPC.width = 82 * 2;
	//		NPC.height = 183 * 2;
	//		NPC.lifeMax = 4000;
	//		NPC.damage = 0;
	//		NPC.defense = 20;
	//		NPC.HitSound = SoundID.NPCHit4;
	//		NPC.DeathSound = SoundID.NPCDeath43;
	//		NPC.knockBackResist = 0f;
	//		NPC.aiStyle = -1;

	//		NPC.boss = true;
	//		NPC.dontTakeDamage = true;
	//		NPC.lavaImmune = true;
	//		NPC.behindTiles = true;
	//	}

 //       //public override void DrawBehind(int index)
 //       //{
 //       //    base.DrawBehind(index);
 //       //}

 //       public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
 //       {
	//		Vector2 position = NPC.position - Main.screenPosition;
	//		position.Y += 4;
	//		Rectangle rect = new Rectangle(0, 0, NPC.width, NPC.height);
	//		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/NPCs/bosses/bloodpillarglow").Value, position, rect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
	//		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/NPCs/bosses/bloodpillarglow2").Value, position, rect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
	//	}
	//}

	//public class bloodpillar : ModNPC
	//{
	//	public override void SetStaticDefaults()
	//	{
	//		DisplayName.SetDefault("Blood Pillar");
	//	}

	//	public override void SetDefaults()
	//	{
	//		NPC.width = 6 * 2;
	//		NPC.height = 9 * 2;
	//		NPC.lifeMax = 4000;
	//		NPC.damage = 0;
	//		NPC.defense = 0;
	//		NPC.HitSound = SoundID.NPCHit1;
	//		NPC.DeathSound = SoundID.NPCDeath1;
	//		NPC.value = 50000;
	//		NPC.knockBackResist = 0f;
	//		NPC.aiStyle = -1;

	//		NPC.lavaImmune = true;
	//		NPC.behindTiles = true;

	//		NPC.buffImmune[BuffID.OnFire] = true;
	//		NPC.buffImmune[BuffID.Poisoned] = true;
	//		NPC.buffImmune[BuffID.Venom] = true;
	//		NPC.buffImmune[BuffID.Bleeding] = true;
	//	}

	//	//public override void DrawBehind(int index)
	//	//{
	//	//    base.DrawBehind(index);
	//	//}
	//}
}
