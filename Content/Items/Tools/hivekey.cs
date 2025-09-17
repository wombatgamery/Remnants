//using System;
//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Content.Items.Tools
//{
//    public class hivekey : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("Hive Key");
//            // Tooltip.SetDefault("A mysterious object with no apparent purpose, it has an insert on the back that would fit into a slot.");
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 10;
//            Item.height = 10;
//            Item.maxStack = 1;
//            Item.value = 10000;
//            Item.rare = ItemRarityID.Blue;

//            Item.useTime = 100;
//            Item.useStyle = ItemUseStyleID.Swing;
//        }

//        public override bool? UseItem(Player player)
//        {
//            Rectangle area = new Rectangle((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, 36 * 2, 36 * 2);
//            int padding = 26;

//            FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
//            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
//            noise.SetFrequency(0.04f);
//            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
//            noise.SetFractalOctaves(2);

//            for (int j = area.Top; j < area.Bottom; j++)
//            {
//                for (int i = area.Left; i < area.Right; i++)
//                {
//                    Point16 point = new Point16(Math.Clamp(i, area.Left + padding, area.Right - padding), Math.Clamp(j, area.Top + padding, area.Bottom - padding));
//                    float _noise = -noise.GetNoise(i * 2, j);
//                    float threshold = Math.Clamp(Vector2.Distance(new Vector2(i, j), point.ToVector2()) / 22f, 0, 1);

//                    Tile tile = Main.tile[i, j];
//                    if (_noise > threshold)
//                    {
//                        tile.HasTile = false;

//                        if (_noise - 0.2f > threshold)
//                        {
//                            tile.WallType = 0;
//                        }
//                        else
//                        {
//                            WorldGen.PlaceWall(i, j, WallID.RocksUnsafe1);
//                            tile.WallType = WallID.RocksUnsafe1;
//                        }
//                    }
//                    else
//                    {
//                        WorldGen.PlaceTile(i, j, TileID.Stone);
//                        tile.TileType = TileID.Stone;

//                        WorldGen.PlaceWall(i, j, WallID.RocksUnsafe1);
//                        tile.WallType = WallID.RocksUnsafe1;
//                    }
//                    WorldGen.SquareTileFrame(i, j);
//                    WorldGen.SquareWallFrame(i, j);
//                }
//            }

//            return true;
//        }
//    }
//}