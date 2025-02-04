//using Microsoft.Xna.Framework;
//using Remnants.Tiles.Blocks;
//using Remnants.Walls;
//using Remnants.Walls.Backgrounds;
//////using SubworldLibrary;
//using Terraria;
//using Terraria.Audio;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Items
//{
//	public class devwand3 : ModItem
//	{
//		public override void SetStaticDefaults() 
//		{
//		}

//		public int wallType = 1;

//		public override void SetDefaults() 
//		{
//			Item.width = 15;
//			Item.height = 13;

//			Item.useStyle = ItemUseStyleID.Swing;
//			Item.useTime = 2;
//			Item.useAnimation = 2;
//			Item.shootSpeed = 16f;

//			Item.autoReuse = true;
			
//		}

//        public override bool CanRightClick()
//        {
//            return true;
//        }

//        public override void RightClick(Player player)
//        {
//			Main.slimeRain = false;
//			Main.slimeRainKillCount = 9999;
//			Main.slimeRainNPCSlots = 0;
//			Main.slimeRainTime = 9999 * 60;

//			SoundEngine.PlaySound(SoundID.NPCHit43, new Vector2((int)player.position.X, (int)player.position.Y));
//			SoundEngine.PlaySound(SoundID.NPCDeath42, new Vector2((int)player.position.X, (int)player.position.Y));
//		}
//    }
//}