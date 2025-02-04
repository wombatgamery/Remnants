//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.Localization;
//using Remnants.Tiles;
//using Remnants.Walls;
//using System.IO;
//using Remnants.Worldgen;
//using Microsoft.Xna.Framework;

//namespace Remnants.Buffs
//{
//	public class midas : ModBuff
//	{
//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("Midas");
//            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
//        }

//        int radius = 5;

//        public override void Update(Player player, ref int buffIndex)
//        {
//            Midas(player);
//        }
//        public override void Update(NPC npc, ref int buffIndex)
//        {
//            Midas(npc: npc);
//        }

//        private void Midas(Player player = null, NPC npc = null)
//        {
//            Vector2 position = player.Center;
//            if (player != null)
//            {
//                position = player.Center;
//            }
//            if (npc != null)
//            {
//                position = npc.Center;
//            }

//            for (int y = (int)position.Y / 16 - radius; y <= (int)position.Y / 16 + radius; y++)
//            {
//                for (int x = (int)position.X / 16 - radius; x <= (int)position.X / 16 + radius; x++)
//                {
//                    Tile tile = Main.tile[x, y];
//                    if (Vector2.Distance(position / 16, new Vector2(x, y)) <= radius && tile.HasTile)
//                    {
//                        bool midas = true;
//                        int type = -1;
//                        if (tile.TileType == TileID.MetalBars && tile.TileFrameX != 18 * 6)
//                        {
//                            tile.TileFrameX = 18 * 6;
//                        }
//                        else if (Main.tileBrick[tile.TileType])
//                        {
//                            type = TileID.GoldBrick;
//                        }
//                        else if (Main.tileSolid[tile.TileType] && tile.TileType != TileID.GoldBrick)
//                        {
//                            type = TileID.Gold;
//                        }
//                        else if (!Main.tileSolid[tile.TileType] && !Main.tileFrameImportant[tile.TileType])
//                        {
//                            type = TileID.GoldCoinPile;
//                        }
//                        else midas = false;

//                        if (type != -1 && tile.TileType != type)
//                        {
//                            tile.HasTile = false;
//                            WorldGen.PlaceTile(x, y, type, true, true);
//                        }

//                        if (midas)
//                        {
//                            int dustIndex = Dust.NewDust(new Vector2(x * 16, y * 16), 16, 16, DustID.YellowStarDust);
//                            Main.dust[dustIndex].noGravity = true;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}