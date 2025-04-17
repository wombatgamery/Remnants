//using Remnants.Content.Buffs;
//using Remnants.Content.Projectiles;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace Remnants.Content.Items.Tools
//{
//	public class instaminer : ModItem
//	{
//		public override void SetStaticDefaults() 
//		{
//			// DisplayName.SetDefault("shreblgeblsword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
//		}

//		public override void SetDefaults() 
//		{
//			Item.CloneDefaults(ItemID.SolarFlarePickaxe);

//			Item.width = 17 * 2;
//			Item.height = 17 * 2;

//			Item.pick = 2147483647;

//			Item.useTime = 5;
//			Item.useAnimation = 5;

//			Item.value = 0;
//			Item.rare = 2;
//		}

//        public override bool AltFunctionUse(Player player)
//        {
//            WorldGen.KillTile((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);

//            return true;
//        }
//    }
//}