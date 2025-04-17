//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.Localization;
//using Remnants.Content.Tiles;
//using System.IO;
//using Remnants.Content.Worldgen;
//using Microsoft.Xna.Framework;

//namespace Remnants.Content.Buffs
//{
//	public class peaceful : ModBuff
//	{
//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("Peaceful Atmosphere");
//            // Description.SetDefault("Increased health and mana regeneration while standing still.");
//            Main.buffNoTimeDisplay[Type] = true;
//            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
//        }

//        public override void Update(Player player, ref int buffIndex)
//        {
//            if (!player.GetModPlayer<RemPlayer>().moving)
//            {
//                player.lifeRegen += 1;
//                player.manaRegen += 1;
//            }
//        }
//    }
//}