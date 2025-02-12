using Microsoft.Xna.Framework;
using Remnants.Items.Accessories;
using Remnants.Items.Tools;
using Remnants.Items.Weapons;
using Remnants.Tiles;
using Remnants.Tiles.Blocks;
using Remnants.Tiles.Objects;
using Remnants.Tiles.Objects.Decoration;
using Remnants.Tiles.Objects.Furniture;
using Remnants.Tiles.Objects.Hazards;
using Remnants.Tiles.Plants;
using Remnants.Walls;
using Remnants.Walls.Vanity;
using Remnants.Walls.Parallax;
using Remnants.Walls.dev;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Remnants.Worldgen.PrimaryBiomes;
using static Remnants.Worldgen.SecondaryBiomes;
using Remnants.Items.Documents;

namespace Remnants.Worldgen
{
    public class Structures : ModSystem
    {
        public struct Dungeon
        {
            public Dungeon(int x, int y, int width, int height, int roomWidth, int roomHeight, int layers = 1, bool pyramid = false)
            {
                X = x;
                Y = y;

                grid = new Rectangle(0, 0, width, height);
                layout = new bool[width, height, layers];

                _roomWidth = roomWidth;
                _roomHeight = roomHeight;

                if (pyramid)
                {
                    for (targetCell.Y = grid.Top; targetCell.Y < grid.Bottom; targetCell.Y++)
                    {
                        for (targetCell.X = grid.Left; targetCell.X < grid.Right; targetCell.X++)
                        {
                            if (targetCell.X - targetCell.Y > grid.Center.X || targetCell.X + targetCell.Y < grid.Center.X)
                            {
                                AddMarker(targetCell.X, targetCell.Y);
                            }
                        }
                    }
                }

                targetCell = Vector2.Zero;
            }

            public int X;
            public int Y;

            int _roomWidth;
            int _roomHeight;

            public Vector2 targetCell;
            public bool[,,] layout;



            public Point16 roomPos => new Point16((int)(X + targetCell.X * _roomWidth), (int)(Y + targetCell.Y * _roomHeight));

            public Rectangle room => new Rectangle(roomPos.X, roomPos.Y, _roomWidth, _roomHeight);

            public Rectangle grid;

            public Rectangle area => new Rectangle(X, Y, (_roomWidth * grid.Width), (_roomHeight * grid.Height));

            public void AddMarker(float cellX, float cellY, int layer = 0)
            {
                if ((int)cellX < grid.Left || (int)cellX >= grid.Right || (int)cellY < grid.Top || (int)cellY >= grid.Bottom)
                {
                    return;
                }
                layout[(int)cellX, (int)cellY, layer] = true;
            }
            public bool FindMarker(float cellX, float cellY, int layer = 0)
            {
                if ((int)cellX < grid.Left || (int)cellX >= grid.Right || (int)cellY < grid.Top || (int)cellY >= grid.Bottom)
                {
                    return layer == 0;
                }
                else return layout[(int)cellX, (int)cellY, layer];
            }

            public bool AddRoom(int width = 1, int height = 1, bool condition = true)
            {
                //cell.X = x == -1 ? WorldGen.genRand.Next(rooms.Left, rooms.Right - width + 2) : x;
                //cell.Y = y == -1 ? WorldGen.genRand.Next(0, rooms.Height - height + 1) : y;

                if (!condition)
                {
                    return false;
                }
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (FindMarker(targetCell.X + i, targetCell.Y + j))
                        {
                            return false;
                        }
                    }
                }

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        AddMarker(targetCell.X + i, targetCell.Y + j);
                    }
                }

                return true;
            }

            public bool CheckConnection(int direction, bool open, int offsetX = 0, int offsetY = 0)
            {
                int i = (int)targetCell.X + offsetX;
                int j = (int)targetCell.Y + offsetY;
                switch (direction)
                {
                    case 1:
                        j--; break;
                    case 2:
                        i++; break;
                    case 3:
                        j++; break;
                    case 4:
                        i--; break;
                }

                if (!FindMarker(i, j))
                {
                    return true;
                }
                else return open ^ FindMarker(i, j, ((direction + 1) % 4) + 1);
            }
        }

        public static void FillChest(int chestIndex, List<(int type, int stack)> itemsToAdd)
        {
            Chest chest = Main.chest[chestIndex];

            if (chest != null)
            {
                int itemsAdded = 0;
                int chestItemIndex = 0;
                foreach (var itemToAdd in itemsToAdd)
                {
                    Item item = new Item(itemToAdd.type, itemToAdd.stack);

                    //chestItemIndex = WorldGen.genRand.Next(Chest.maxItems);
                    //while (!chest.item[chestItemIndex].IsAir)
                    //{
                    //    chestItemIndex = WorldGen.genRand.Next(Chest.maxItems);
                    //}

                    if (item.accessory || item.damage > 0)
                    {
                        item.Prefix(-1);
                    }

                    chest.item[chestItemIndex] = item;
                    itemsAdded++;
                    chestItemIndex++;
                    if (itemsAdded >= Chest.maxItems)
                        break;
                }
            }
        }

        public static bool AvoidsBiomes(Rectangle location, int[] biomesToAvoid)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();
            for (int y = location.Top - 5; y <= location.Bottom + 5; y++)
            {
                for (int x = location.Left - 5; x <= location.Right + 5; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (biomesToAvoid.Contains(biomes.FindBiome(x, y)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool InsideBiome(Rectangle location, int biome)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();
            for (int y = location.Top; y <= location.Bottom; y++)
            {
                for (int x = location.Left; x <= location.Right; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (biomes.FindBiome(x, y) != biome)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void GenericLoot(int chestIndex, List<(int type, int stack)> itemsToAdd, int grade = 0, int[] uniquePotions = null, bool haveRestorationPotions = false)
        {
            Chest chest = Main.chest[chestIndex];

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int x = chest.x;
            int y = chest.y;

            bool magicalLab = WGTools.Tile(x, y).WallType == ModContent.WallType<EnchantedBrickWallUnsafe>() || WGTools.Tile(x, y).WallType == ModContent.WallType<magicallab>();
            bool pyramid = WGTools.Tile(x, y).WallType == ModContent.WallType<PyramidBrickWallUnsafe>() || WGTools.Tile(x, y).WallType == ModContent.WallType<pyramid>();
            bool tomb = WGTools.Tile(x, y).WallType == ModContent.WallType<TombBrickWallUnsafe>() || WGTools.Tile(x, y).WallType == ModContent.WallType<forgottentomb>();
            bool manaPotions = y <= Main.worldSurface * 0.5;

            if (grade > 0)
            {
                int odds = biomes.FindBiome(x, y) == BiomeID.Marble || biomes.FindBiome(x, y) == BiomeID.Granite ? 2 : 3;

                if (grade < 3 && Main.rand.NextBool(odds) && !magicalLab)
                {
                    int piece = Main.rand.Next(4);

                    if (biomes.FindBiome(x, y) == BiomeID.Marble)
                    {
                        itemsToAdd.Add((piece == 3 ? ItemID.Gladius : piece == 2 ? ItemID.GladiatorLeggings : piece == 1 ? ItemID.GladiatorBreastplate : ItemID.GladiatorHelmet, 1));
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Granite)
                    {
                        //piece = Main.rand.Next(3);
                        //itemsToAdd.Add((piece == 3 ? ItemID.Gladius : piece == 2 ? ItemID.AncientCobaltLeggings : piece == 1 ? ItemID.AncientCobaltBreastplate : ItemID.AncientCobaltHelmet, 1));
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Desert || biomes.FindBiome(x, y) == BiomeID.Hive)
                    {
                        if (y > GenVars.lavaLine || Main.wallDungeon[Main.tile[x, y].WallType])
                        {
                            itemsToAdd.Add((piece == 3 ? ItemID.TungstenBroadsword : piece == 2 ? ItemID.TungstenGreaves : piece == 1 ? ItemID.TungstenChainmail : ItemID.TungstenHelmet, 1));
                        }
                        else if (y > Main.rockLayer)
                        {
                            itemsToAdd.Add((piece == 3 ? ItemID.LeadBroadsword : piece == 2 ? ItemID.LeadGreaves : piece == 1 ? ItemID.LeadChainmail : ItemID.LeadHelmet, 1));
                        }
                        else if (y > Main.worldSurface * 0.5)
                        {
                            itemsToAdd.Add((piece == 3 ? ItemID.TinBroadsword : piece == 2 ? ItemID.TinGreaves : piece == 1 ? ItemID.TinChainmail : ItemID.TinHelmet, 1));
                        }
                    }
                    else
                    {
                        if (y > GenVars.lavaLine || Main.wallDungeon[Main.tile[x, y].WallType])
                        {
                            itemsToAdd.Add((piece == 3 ? ItemID.SilverBroadsword : piece == 2 ? ItemID.SilverGreaves : piece == 1 ? ItemID.SilverChainmail : ItemID.SilverHelmet, 1));
                        }
                        else if (y > Main.rockLayer)
                        {
                            itemsToAdd.Add((piece == 3 ? ItemID.IronBroadsword : piece == 2 ? ItemID.IronGreaves : piece == 1 ? ItemID.IronChainmail : ItemID.IronHelmet, 1));
                        }
                        else if (y > Main.worldSurface * 0.5)
                        {
                            itemsToAdd.Add((piece == 3 ? ItemID.CopperBroadsword : piece == 2 ? ItemID.CopperGreaves : piece == 1 ? ItemID.CopperChainmail : ItemID.CopperHelmet, 1));
                        }
                    }
                }
                //if (grade == 1 && y > Main.worldSurface * 0.5 && y < Main.rockLayer)
                //{
                //    if (Main.rand.NextBool(2))
                //        itemsToAdd.Add((y > Main.worldSurface ? ItemID.CanOfWorms : ItemID.HerbBag, Main.rand.Next(1, 3)));
                //}

                if (Main.rand.NextBool(2))
                {
                    if (magicalLab)
                    {
                        itemsToAdd.Add((ItemID.MolotovCocktail, Main.rand.Next(10, 20)));
                    }
                    else if (y > Main.rockLayer)
                    {
                        itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                    }
                    else itemsToAdd.Add((biomes.FindBiome(x, y) == BiomeID.Desert ? ItemID.ScarabBomb : y < Main.worldSurface ? ItemID.Grenade : ItemID.Bomb, Main.rand.Next(5, 10)));
                }

                if (grade < 3)
                {
                    if (y > Main.worldSurface * 0.5 && Main.rand.NextBool(2) && !magicalLab)
                        itemsToAdd.Add((tomb ? ItemID.WebRope : Main.wallDungeon[WGTools.Tile(x, y).WallType] || y >= Main.maxTilesY - 200 ? ItemID.Chain : biomes.FindBiome(x, y) == BiomeID.None && y < Main.worldSurface ? ItemID.VineRope : ItemID.Rope, Main.rand.Next(50, 100)));

                    if (Main.rand.NextBool(2))
                    {
                        if (Main.tile[x, y].TileType == TileID.Dressers)
                        {
                            itemsToAdd.Add((Main.rand.NextBool(2) ? ItemID.Shuriken : ItemID.ThrowingKnife, Main.rand.Next(50, 100)));
                        }
                        else if (grade <= 1)
                        {
                            itemsToAdd.Add((ItemID.WoodenArrow, Main.rand.Next(50, 100)));
                        }
                        else itemsToAdd.Add((magicalLab ? ItemID.ShimmerArrow : y >= Main.maxTilesY - 200 ? ItemID.HellfireArrow : y < Main.worldSurface * 0.5f ? ItemID.JestersArrow : biomes.FindBiome(x, y) == BiomeID.Tundra ? ItemID.FrostburnArrow : ItemID.FlamingArrow, Main.rand.Next(25, 50)));
                    }
                }

                if (biomes.FindBiome(x, y) == BiomeID.Hive)
                {
                    itemsToAdd.Add((ItemID.BottledHoney, Main.rand.Next(6, 12)));
                }
                else if (Main.rand.NextBool(2) || magicalLab)
                    itemsToAdd.Add((grade > 2 ? haveRestorationPotions ? ItemID.RestorationPotion : manaPotions ? ItemID.GreaterManaPotion : ItemID.GreaterHealingPotion : grade == 2 ? haveRestorationPotions ? ItemID.RestorationPotion : manaPotions ? ItemID.ManaPotion : ItemID.HealingPotion : haveRestorationPotions ? ItemID.LesserRestorationPotion : manaPotions ? ItemID.LesserManaPotion : ItemID.LesserHealingPotion, Main.rand.Next(3, 6)));
            }

            if (uniquePotions != null)
            {
                itemsToAdd.Add((uniquePotions[Main.rand.Next(uniquePotions.Length)], WorldGen.genRand.Next(2, 4)));
            }


            if (grade > 2)
            {
                itemsToAdd.Add((MythicPotion(), Main.rand.Next(1, 3)));
            }
            if (grade > 1)
            {
                itemsToAdd.Add((RarePotion(), Main.rand.Next(1, 3)));
            }
            itemsToAdd.Add((CommonPotion(), Main.rand.Next(2, 4)));

            if (grade < 3)
            {
                if (Main.rand.NextBool(2) || magicalLab)
                    itemsToAdd.Add((magicalLab ? ItemID.TeleportationPotion : y >= Main.maxTilesY - 200 ? ItemID.PotionOfReturn : ItemID.RecallPotion, Main.rand.Next(2, 4)));
            }

            if (y > Main.worldSurface && y < Main.maxTilesY - 200 || pyramid)
            {
                if (Main.rand.NextBool(2))
                {
                    itemsToAdd.Add((WGTools.Tile(x, y).LiquidAmount > 0 ? ItemID.Glowstick : magicalLab ? ItemID.ShimmerTorch : biomes.FindBiome(x, y) == BiomeID.Tundra ? ItemID.IceTorch : biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive ? ItemID.JungleTorch : biomes.FindBiome(x, y) == BiomeID.Desert ? ItemID.DesertTorch : biomes.FindBiome(x, y) == BiomeID.Glowshroom ? ItemID.MushroomTorch : ItemID.Torch, grade > 0 ? Main.rand.Next(10, 20) : Main.rand.Next(2, 5)));
                }
            }

            if (grade <= 1)
            {
                itemsToAdd.Add((ItemID.SilverCoin, grade > 0 ? Main.rand.Next(20, 41) : Main.rand.Next(4, 9)));
            }
            else itemsToAdd.Add((ItemID.GoldCoin, grade > 2 ? Main.rand.Next(5, 11) : Main.rand.Next(1, 3)));

            if (Main.tile[x, y].TileType == TileID.Dressers)
            {
                itemsToAdd.Add((ItemID.Bottle, Main.rand.Next(3, 6)));
                itemsToAdd.Add((ItemID.Silk, Main.rand.Next(6, 12)));
                itemsToAdd.Add((ItemID.Book, Main.rand.Next(3, 6)));
            }

            if (Main.wallDungeon[Main.tile[x, y].WallType])
            {
                for (int k = 0; k < (WGTools.Tile(x, y).LiquidAmount > 0 ? Main.rand.Next(6, 12) : Main.rand.Next(0, 6)); k++)
                {
                    itemsToAdd.Add((ItemID.Bone, Main.rand.Next(1, 4)));
                }
            }

            if (magicalLab)
            {
                itemsToAdd.Add((ItemID.SpellTome, 1));
            }
            else
            {
                if (pyramid)
                {
                    for (int k = 0; k < Main.rand.Next(0, 6); k++)
                    {
                        itemsToAdd.Add((ItemID.AncientCloth, Main.rand.Next(1, 4)));
                    }
                }
                else if (biomes.FindBiome(x, y) == BiomeID.Hive)
                {
                    for (int k = 0; k < Main.rand.Next(0, 6); k++)
                    {
                        itemsToAdd.Add((ItemID.BeeWax, Main.rand.Next(1, 4)));
                    }
                }
                if (y < Main.maxTilesY - 200 && Main.tile[x, y].LiquidAmount == 0)
                {
                    for (int k = 0; k < Main.rand.Next(3, 6); k++)
                    {
                        itemsToAdd.Add((ItemID.Cobweb, Main.rand.Next(1, 4)));
                    }
                }
            }
        }

        public static void FillEdges(int left, int top, int right, int bottom, int innerTile = -1, bool ignoreTop = true)
        {
            int radius = 25;

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            FastNoiseLite borders = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            borders.SetFractalType(FastNoiseLite.FractalType.FBm);
            borders.SetFrequency(0.05f);

            for (int y = top - (!ignoreTop ? radius : 0); y <= bottom + radius; y++)
            {
                for (int x = left - radius; x <= right + radius; x = (x == left - 1 && y >= top && y <= bottom) ? right + 1 : x + 1)
                {
                    Vector2 point = new Vector2(MathHelper.Clamp(x, left, right), MathHelper.Clamp(y, top, bottom));

                    float noise = borders.GetNoise(x, y) / 2 + 0.5f;
                    if (noise > Vector2.Distance(new Vector2(x, y), point) / (radius / 2) && innerTile != -1)
                    {
                        WGTools.Tile(x, y).HasTile = true;
                        WGTools.Tile(x, y).TileType = (ushort)innerTile;
                    }
                    else if (!WGTools.Tile(x, y).HasTile || WGTools.Tile(x, y).TileType != innerTile && WGTools.Tile(x, y).TileType != ModContent.TileType<Hardstone>())
                    {
                        if (ignoreTop)
                        {
                            point.Y = MathHelper.Clamp(point.Y, top + radius, bottom);
                        }
                        if (noise + 1 > Vector2.Distance(new Vector2(x, y), point) / (radius / 2))
                        {
                            WGTools.Tile(x, y).HasTile = true;
                            WGTools.Tile(x, y).TileType = biomes.FindLayer(x, y) >= biomes.height - 4 ? TileID.Ash : biomes.FindLayer(x, y) >= biomes.height - 6 ? TileID.Obsidian : biomes.FindBiome(x, y) == BiomeID.Granite ? TileID.Granite : biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Glowshroom ? TileID.Mud : biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.IceBlock : biomes.FindLayer(x, y) < biomes.caveLayer ? TileID.Dirt : TileID.Stone;
                        }
                        else if (!WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y - 1).HasTile && WGTools.Tile(x, y - 1).TileType == TileID.Mud)
                        {
                            WGTools.Tile(x, y - 1).TileType = biomes.FindBiome(x, y) == BiomeID.Jungle ? TileID.JungleGrass : TileID.MushroomGrass;
                        }
                    }
                }
            }
        }

        public static void AddTraps(Rectangle location, float multiplier = 1, bool temple = false)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int count = (int)((location.Width * location.Height * multiplier / 2000) * ModContent.GetInstance<Client>().TrapFrequency);
            while (count > 0)
            {
                int x = WorldGen.genRand.Next(location.Left, location.Right);
                int y = WorldGen.genRand.Next(location.Top + 5, location.Bottom);

                if (!WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).LiquidAmount == 0 && !WGTools.Solid(x, y - 1) && !WGTools.Solid(x, y - 2) && WGTools.Solid(x, y + 1) && TileID.Sets.TouchDamageImmediate[WGTools.Tile(x, y + 1).TileType] == 0 && WGTools.Tile(x, y + 1).TileType != TileID.TrapdoorClosed)
                {
                    if (!temple)
                    {
                        int trapX = x;
                        int trapY = y - WorldGen.genRand.Next(3);
                        int direction = WorldGen.genRand.NextBool(2) ? -1 : 1;
                        int length = 0;

                        while (!WGTools.Solid(trapX, trapY))
                        {
                            trapX += direction;
                            length++;
                        }
                        if (length > 12 && length <= 64 && WGTools.Tile(trapX, trapY).TileType != TileID.Traps && WGTools.Tile(trapX, trapY).TileType != TileID.ClosedDoor && WGTools.Tile(trapX, trapY).TileType != ModContent.TileType<LockedIronDoor>() && TileID.Sets.TouchDamageImmediate[WGTools.Tile(trapX, trapY).TileType] == 0 && (WGTools.Tile(trapX, trapY - 1).TileType != TileID.Traps && WGTools.Tile(trapX, trapY + 1).TileType != TileID.Traps || WorldGen.genRand.NextBool(5)))
                        {
                            WorldGen.PlaceTile(x, y, TileID.PressurePlates, style: 2);

                            WGTools.Tile(trapX, trapY).Slope = SlopeType.Solid; WGTools.Tile(trapX, trapY).TileType = TileID.Traps;
                            WGTools.Tile(trapX, trapY).TileFrameX = (short)((direction == 1 ? 0 : 1) * 18); WGTools.Tile(trapX, trapY).TileFrameY = 0;

                            WGTools.Wire(x, y + (Main.wallDungeon[WGTools.Tile(x, y).WallType] ? 2 : 1), trapX, trapY);
                            WGTools.Tile(x, y).RedWire = true; WGTools.Tile(x, y + 1).RedWire = true;

                            count--;
                        }
                    }
                    else
                    {
                        if (WGTools.Tile(x, y).WallType == WallID.LihzahrdBrickUnsafe || WGTools.Tile(x, y).WallType == ModContent.WallType<temple>())
                        {
                            int trapLeft = x - WorldGen.genRand.Next(3);
                            int trapRight = x + WorldGen.genRand.Next(3);
                            int trapY = y;
                            int length = 0;

                            while (!WGTools.Solid(x, trapY))
                            {
                                trapY--;
                                length++;
                            }

                            if (length > 7)
                            {
                                bool valid = true;
                                for (int i = trapLeft; i <= trapRight; i++)
                                {
                                    if (!WGTools.Solid(i, trapY) || WorldGen.SolidOrSlopedTile(i, trapY + 1))
                                    {
                                        valid = false;
                                        break;
                                    }
                                    else if (WGTools.Tile(i, trapY).TileType == TileID.Traps || WGTools.Tile(i, trapY).TileType == TileID.Platforms || TileID.Sets.TouchDamageImmediate[WGTools.Tile(i, trapY).TileType] > 0)
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                                if (valid)
                                {
                                    WorldGen.PlaceTile(x, y, TileID.PressurePlates, style: 6);

                                    for (int i = trapLeft; i <= trapRight; i++)
                                    {
                                        WGTools.Tile(i, trapY).Slope = SlopeType.Solid; WGTools.Tile(i, trapY).TileType = TileID.Traps;
                                        WGTools.Tile(i, trapY).TileFrameX = 0; WGTools.Tile(i, trapY).TileFrameY = (short)((length > 19 ? 3 : 4) * 18);
                                        WGTools.Tile(i, trapY).YellowWire = true;
                                    }

                                    int offset = 0;
                                    if (WGTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe)
                                    {
                                        int left = 0;
                                        int right = 0;
                                        while (WGTools.Tile(x - left, y).WallType != WallID.LihzahrdBrickUnsafe)
                                        {
                                            left++;
                                        }
                                        while (WGTools.Tile(x + right, y).WallType != WallID.LihzahrdBrickUnsafe)
                                        {
                                            right++;
                                        }

                                        if (left == right)
                                        {
                                            offset = WorldGen.genRand.NextBool(2) ? -left : right;
                                        }
                                        else offset = (left < right) ? -left : right;
                                    }

                                    WGTools.Tile(x, y).YellowWire = true;
                                    WGTools.Wire(x, y + (offset != 0 ? 1 : 0), x + offset, trapY + 1, false, yellow: true);
                                    if (offset != 0)
                                    {
                                        WGTools.Wire(x + offset, trapY, x, trapY, false, yellow: true);
                                    }

                                    count--;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddVariation(Rectangle location)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            FastNoiseLite weathering = new FastNoiseLite();
            weathering.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            weathering.SetFrequency(0.2f);
            weathering.SetFractalType(FastNoiseLite.FractalType.FBm);
            weathering.SetFractalOctaves(3);

            int statue = WorldGen.genRand.Next(4);

            //ushort rockWall = (ushort)WorldGen.genRand.Next(3);
            //if (rockWall == 0) { rockWall = WallID.Cave8Unsafe; }
            //else if (rockWall == 1) { rockWall = WallID.RocksUnsafe1; }
            //else if (rockWall == 2) { rockWall = WallID.RocksUnsafe2; }

            ushort dirtWall = (ushort)WorldGen.genRand.Next(4);
            if (dirtWall == 0) { dirtWall = WallID.DirtUnsafe1; }
            else if (dirtWall == 1) { dirtWall = WallID.DirtUnsafe2; }
            else if (dirtWall == 2) { dirtWall = WallID.DirtUnsafe3; }
            else { dirtWall = WallID.DirtUnsafe4; }

            ushort jungleWall = (ushort)WorldGen.genRand.Next(3);
            if (jungleWall == 0) { jungleWall = WallID.JungleUnsafe1; }
            else if (jungleWall == 1) { jungleWall = WallID.JungleUnsafe2; }
            else { jungleWall = WallID.JungleUnsafe4; }

            ushort lavaWall = (ushort)WorldGen.genRand.Next(4);
            if (lavaWall == 0) { lavaWall = WallID.LavaUnsafe1; }
            else if (lavaWall == 1) { lavaWall = WallID.LavaUnsafe2; }
            else if (lavaWall == 2) { lavaWall = WallID.LavaUnsafe3; }
            else if (lavaWall == 3) { lavaWall = WallID.LavaUnsafe4; }

            for (int y = location.Top - 10; y <= location.Bottom + 10; y++)
            {
                for (int x = location.Left - 10; x <= location.Right + 10; x++)
                {
                    Tile tile = Main.tile[x, y];
                    //bool lavaLevel = y >= GenVars.lavaLine + WorldGen.genRand.Next(-5, 6);

                    if (tile.HasTile)
                    {
                        //if (tile.TileType == TileID.ClosedDoor && WGTools.Tile(x, y - 1).TileType != TileID.ClosedDoor && WorldGen.genRand.NextBool(3))
                        //{
                        //    WorldGen.OpenDoor(x, y, WorldGen.genRand.NextBool(2) ? -1 : 1);
                        //}
                        //else
                        if (tile.TileType == TileID.HangingLanterns && WGTools.Tile(x, y - 1).TileType != TileID.HangingLanterns && WorldGen.genRand.NextBool(2))
                        {
                            WGTools.Tile(x, y).TileFrameX = 18;
                            WGTools.Tile(x, y + 1).TileFrameX = 18;
                        }
                        else if (tile.TileType == TileID.Banners)
                        {
                            if (WGTools.Tile(x, y - 1).TileType != TileID.Banners && tile.TileFrameX == 18 * 4)
                            {
                                int style = Main.rand.Next(3);
                                WGTools.Tile(x, y).TileFrameX += (short)(style * 18);
                                WGTools.Tile(x, y + 1).TileFrameX += (short)(style * 18);
                                WGTools.Tile(x, y + 2).TileFrameX += (short)(style * 18);
                            }
                        }
                        else if (tile.TileType == TileID.Books)
                        {
                            if (tile.TileFrameX != 18 * 5)
                            {
                                int style = Main.rand.Next(5);
                                WGTools.Tile(x, y).TileFrameX = (short)(style * 18);
                            }
                        }
                        else if (tile.TileType == TileID.Statues)
                        {
                            if (tile.TileFrameX <= 18 * 2)
                            {
                                if (statue == 1)
                                {
                                    tile.TileFrameX += 36 * 1;
                                }
                                if (statue == 2)
                                {
                                    tile.TileFrameX += 36 * 11;
                                }
                                if (statue == 3)
                                {
                                    tile.TileFrameX += 36 * 15;
                                }
                            }
                        }
                        else if (tile.TileType == TileID.Painting3X3)
                        {
                            if (tile.TileFrameX == 54 * 16 && tile.TileFrameY == 0)
                            {
                                int style = Main.rand.Next(16, 18);
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Main.tile[x + i, y + j].TileFrameX = (short)(style * 54 + i * 18);
                                        Main.tile[x + i, y + j].TileFrameY = (short)(j * 18);
                                    }
                                }
                            }
                        }
                        else if (tile.TileType == TileID.Painting4X3)
                        {
                            if (tile.TileFrameX == 0 && tile.TileFrameY % 54 == 0)
                            {
                                int style = Main.rand.Next(9);
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        Main.tile[x + i, y + j].TileFrameX = (short)(i * 18);
                                        Main.tile[x + i, y + j].TileFrameY = (short)(style * 54 + j * 18);
                                    }
                                }
                            }
                        }
                    }
                    else if (tile.WallType != ModContent.WallType<undergrowth>())
                    {
                        bool tomb = tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>();

                        if ((WorldGen.genRand.NextBool(2) || tomb) && (y > Main.worldSurface || !Main.wallLight[tile.WallType]) && biomes.FindBiome(x, y) != BiomeID.Tundra && biomes.FindBiome(x, y) != BiomeID.Desert && biomes.FindBiome(x, y) != BiomeID.Marble && biomes.FindBiome(x, y) != BiomeID.Granite && biomes.FindBiome(x, y) != BiomeID.Hive)
                        {
                            if (WGTools.Solid(x, y - 1) && WGTools.Tile(x, y - 1).TileType != TileID.Spikes && (WorldGen.SolidOrSlopedTile(x - 1, y) || WorldGen.SolidOrSlopedTile(x + 1, y)))
                            {
                                WorldGen.TileRunner(x, y, tomb ? Main.rand.Next(4, 9) : 4, 1, TileID.Cobweb, true, overRide: false);
                            }
                        }
                    }

                    if (weathering.GetNoise(x + WorldGen.genRand.Next(-3, 4), y + WorldGen.genRand.Next(-3, 4)) > 0)
                    {
                        if (tile.TileType == TileID.GrayBrick || tile.TileType == TileID.WoodBlock || tile.TileType == TileID.RichMahogany)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive)
                            {
                                tile.TileType = TileID.JungleGrass;
                            }
                            else if (y < Main.worldSurface)
                            {
                                tile.TileType = TileID.Grass;
                            }
                            else if (tile.TileType == TileID.GrayBrick)
                            {
                                tile.TileType = TileID.Stone;
                            }
                        }
                        else if (tile.TileType == TileID.IceBrick)
                        {
                            tile.TileType = TileID.IceBlock;
                        }
                        else if (tile.TileType == TileID.MarbleBlock)
                        {
                            tile.TileType = TileID.Marble;
                        }
                        else if (tile.TileType == TileID.GraniteBlock)
                        {
                            tile.TileType = TileID.Granite;
                        }
                        else if (tile.TileType == TileID.SmoothSandstone)
                        {
                            tile.TileType = TileID.Sandstone;
                        }
                        else if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("honeybrick", out ModTile honeybrick) && tile.TileType == honeybrick.Type)
                        {
                            tile.TileType = TileID.Hive;
                        }

                        //if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Dirt)
                        //{
                        //    tile.WallType = dirtWall;
                        //}
                        //else
                        if (tile.WallType == ModContent.WallType<BrickStone>() || tile.WallType == ModContent.WallType<Wood>() || tile.WallType == ModContent.WallType<WoodMahogany>())
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive)
                            {
                                tile.WallType = WallID.JungleUnsafe;
                            }
                            else if (y < Main.worldSurface)
                            {
                                tile.WallType = WallID.GrassUnsafe;
                            }
                            else if (tile.WallType == ModContent.WallType<BrickStone>())
                            {
                                tile.WallType = biomes.FindLayer(x, y) >= biomes.lavaLayer ? WallID.Cave8Unsafe : WallID.RocksUnsafe1;
                            }
                        }
                        else if (tile.WallType == ModContent.WallType<BrickIce>())
                        {
                            tile.WallType = WallID.IceUnsafe;
                        }
                        else if (tile.WallType == WallID.MarbleBlock)
                        {
                            tile.WallType = WallID.MarbleUnsafe;
                        }
                        else if (tile.WallType == WallID.GraniteBlock)
                        {
                            tile.WallType = WallID.GraniteUnsafe;
                        }
                        else if (tile.WallType == WallID.SmoothSandstone)
                        {
                            tile.WallType = WallID.Sandstone;
                        }
                        else if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("honeybrickwall", out ModWall honeybrickwall) && tile.WallType == honeybrickwall.Type)
                        {
                            tile.WallType = WallID.HiveUnsafe;
                        }
                    }
                }
            }
        }

        public static void AddErosion(Rectangle location, ushort[] tilesToErode, int steps = 3, int chance = 4)
        {
            for (int i = 0; i < steps; i++)
            {
                for (int y = location.Top - 10; y <= location.Bottom + 10; y++)
                {
                    for (int x = location.Left - 10; x <= location.Right + 10; x++)
                    {
                        if (WGTools.Tile(x, y).HasTile && WorldGen.genRand.NextBool(chance) && WGTools.AdjacentTiles(x, y) < 4 && WGTools.Tile(x, y).WallType != ModContent.WallType<GardenBrickWall>())
                        {
                            if (tilesToErode.Contains(Framing.GetTileSafely(x, y).TileType))
                            {
                                Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<devtile2>();
                            }
                        }

                        //if (Framing.GetTileSafely(x, y).type == TileID.Platforms)
                        //{
                        //    if (Framing.GetTileSafely(x, y).slope() == 0)
                        //    {
                        //        bool slopeLeft = false;
                        //        bool slopeRight = false;
                        //        if (Framing.GetTileSafely(x - 1, y).active() && Main.tileSolid[Framing.GetTileSafely(x - 1, y).type])
                        //        {
                        //            slopeLeft = true;
                        //        }
                        //        if (Framing.GetTileSafely(x + 1, y).active() && Main.tileSolid[Framing.GetTileSafely(x + 1, y).type])
                        //        {
                        //            slopeRight = true;
                        //        }
                        //        if (slopeLeft && !slopeRight)
                        //        {
                        //            Framing.GetTileSafely(x, y).slope(1);
                        //        }
                        //        else if (!slopeLeft && slopeRight)
                        //        {
                        //            Framing.GetTileSafely(x, y).slope(2);
                        //        }
                        //    }

                        //    if (!Framing.GetTileSafely(x + 1, y + 1).active() && Framing.GetTileSafely(x, y).slope() == 1)
                        //    {
                        //        WorldGen.PlaceTile(x + 1, y + 1, TileID.Platforms);
                        //        Framing.GetTileSafely(x + 1, y + 1).slope(1);
                        //    }
                        //    if (!Framing.GetTileSafely(x - 1, y + 1).active() && Framing.GetTileSafely(x, y).slope() == 2)
                        //    {
                        //        WorldGen.PlaceTile(x - 1, y + 1, TileID.Platforms);
                        //        Framing.GetTileSafely(x - 1, y + 1).slope(2);
                        //    }
                        //}
                    }
                }

                for (int y = location.Top - 10; y <= location.Bottom + 10; y++)
                {
                    for (int x = location.Left - 10; x <= location.Right + 10; x++)
                    {
                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<devtile2>())
                        {
                            WorldGen.KillTile(x, y);
                            Framing.GetTileSafely(x, y).TileType = 0;
                        }
                    }
                }
            }

            for (int y = location.Top - 10; y <= location.Bottom + 10; y++)
            {
                for (int x = location.Left - 10; x <= location.Right + 10; x++)
                {
                    if (Framing.GetTileSafely(x, y).HasTile)
                    {
                        int adjacentTiles = 0;
                        if (Framing.GetTileSafely(x + 1, y).HasTile)
                        {
                            adjacentTiles++;
                        }
                        if (Framing.GetTileSafely(x - 1, y).HasTile)
                        {
                            adjacentTiles++;
                        }
                        if (Framing.GetTileSafely(x, y + 1).HasTile)
                        {
                            adjacentTiles++;
                        }
                        if (Framing.GetTileSafely(x, y - 1).HasTile)
                        {
                            adjacentTiles++;
                        }
                        if (adjacentTiles <= 1)
                        {
                            if (tilesToErode.Contains(Framing.GetTileSafely(x, y).TileType))
                            {
                                WorldGen.KillTile(x, y);
                            }
                        }
                    }
                }
            }
        }

        public static void AddDecorations(Rectangle location)
        {
            if (ModLoader.TryGetMod("WombatQOL", out Mod wgi) && wgi.TryFind<ModTile>("CeremonialCandle", out ModTile candle))
            {
                BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

                for (int y = location.Top; y <= location.Bottom; y++)
                {
                    for (int x = location.Left; x <= location.Right; x++)
                    {
                        Tile tile = Main.tile[x, y];

                        if (!tile.HasTile && WGTools.Tile(x, y + 1).HasTile)
                        {
                            if (WGTools.Tile(x, y + 1).TileType == TileID.PlanterBox)
                            {
                                if (WorldGen.genRand.NextBool(2))
                                {
                                    WorldGen.PlaceTile(x, y, TileID.ImmatureHerbs, style: 2);
                                }
                            }
                            else
                            {
                                if (WGTools.Tile(x, y + 1).TileType == TileID.Bookcases)// || WGTools.Tile(x, y + 1).TileType == TileID.Pianos)
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        WorldGen.PlaceTile(x, y, TileID.Books);
                                    }
                                    //else if (WGTools.Tile(x, y + 1).TileType == TileID.Pianos)
                                    //{
                                    //    WorldGen.PlaceTile(x, y, candle.Type, style: WorldGen.genRand.Next(6));
                                    //}
                                }
                                else if (WGTools.Tile(x, y + 1).TileType == ModContent.TileType<SacrificialAltar>())
                                {
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceTile(x, y, candle.Type, style: WorldGen.genRand.Next(6));
                                    }
                                }
                                else if (WGTools.Tile(x, y + 1).TileType == ModContent.TileType<AlchemyBench>())
                                {
                                    if (!WorldGen.genRand.NextBool(3))
                                    {
                                        if (WorldGen.genRand.NextBool(2))
                                        {
                                            WorldGen.PlaceTile(x, y, TileID.Bottles);
                                            if (WorldGen.genRand.NextBool(2))
                                            {
                                                tile.TileFrameX = (short)(WorldGen.genRand.NextBool(2) ? 18 : 36);
                                            }
                                        }
                                        else WorldGen.PlaceTile(x, y, TileID.Books);
                                    }
                                }
                                else if (WGTools.Tile(x, y + 1).TileType == TileID.Tables || WGTools.Tile(x, y + 1).TileType == TileID.WorkBenches || WGTools.Tile(x, y + 1).TileType == TileID.Dressers || WGTools.Tile(x, y + 1).TileType == TileID.Platforms && (WGTools.Tile(x, y + 1).TileFrameY == 18 * 9 || WGTools.Tile(x, y + 1).TileFrameY == 18 * 10 || WGTools.Tile(x, y + 1).TileFrameY == 18 * 11 || WGTools.Tile(x, y + 1).TileFrameY == 18 * 12))
                                {
                                    if (WorldGen.genRand.NextBool(5) && WGTools.Tile(x, y + 1).TileType != TileID.Platforms && WGTools.Tile(x, y + 1).TileType != ModContent.TileType<SacrificialAltar>() && !WGTools.Tile(x + 1, y).HasTile && WGTools.Tile(x + 1, y + 1).HasTile)
                                    {
                                        WorldGen.PlaceTile(x, y, TileID.Bowls);
                                    }
                                    if (!tile.HasTile && (WorldGen.genRand.NextBool(2) || tile.WallType == ModContent.WallType<EnchantedBrickWallUnsafe>()))
                                    {
                                        if (WorldGen.genRand.NextBool(3) && (WGTools.Tile(x, y + 1).TileType == TileID.Platforms || !Main.wallDungeon[tile.WallType]) && biomes.FindBiome(x, y) != BiomeID.Granite)
                                        {
                                            WorldGen.PlaceTile(x, y, candle.Type, style: WorldGen.genRand.Next(6));
                                        }
                                        else if (WorldGen.genRand.NextBool(2))
                                        {
                                            WorldGen.PlaceTile(x, y, TileID.Bottles);
                                            if (WorldGen.genRand.NextBool(2))
                                            {
                                                tile.TileFrameX = (short)(WorldGen.genRand.NextBool(2) ? 18 : 36);
                                            }
                                        }
                                        else WorldGen.PlaceTile(x, y, TileID.Books);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddTheming(Rectangle location)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            ushort dirtWall = (ushort)WorldGen.genRand.Next(4);
            if (dirtWall == 0) { dirtWall = WallID.DirtUnsafe1; }
            else if (dirtWall == 1) { dirtWall = WallID.DirtUnsafe2; }
            else if (dirtWall == 2) { dirtWall = WallID.DirtUnsafe3; }
            else { dirtWall = WallID.DirtUnsafe4; }

            ushort jungleWall = (ushort)WorldGen.genRand.Next(3);
            if (jungleWall == 0) { jungleWall = WallID.JungleUnsafe1; }
            else if (jungleWall == 1) { jungleWall = WallID.JungleUnsafe2; }
            else { jungleWall = WallID.JungleUnsafe4; }

            for (int y = location.Top - 10; y <= location.Bottom + 10; y++)
            {
                for (int x = location.Left - 10; x <= location.Right + 10; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (tile.WallType == WallID.Wood)
                    {
                        tile.WallType = (ushort)ModContent.WallType<Wood>();
                    }

                    if (biomes.FindBiome(x, y) == BiomeID.Tundra)
                    {
                        if (tile.TileType == TileID.Dirt)
                        {
                            tile.TileType = TileID.SnowBlock;
                        }
                        else if (tile.TileType == TileID.Stone)
                        {
                            tile.TileType = TileID.IceBlock;
                        }
                        else if (tile.TileType == TileID.GrayBrick)
                        {
                            tile.TileType = TileID.IceBrick;
                        }
                        else if (tile.TileType == TileID.WoodBlock)
                        {
                            tile.TileType = TileID.BorealWood;
                        }
                        else if (tile.TileType == TileID.Platforms)
                        {
                            if (tile.TileFrameY == 0)
                            {
                                tile.TileFrameY = 18 * 19;
                            }
                            else if (tile.TileFrameY == 18 * 43)
                            {
                                tile.TileFrameY = 18 * 35;
                            }
                        }
                        else if (tile.TileType == TileID.WoodenBeam)
                        {
                            tile.TileType = TileID.BorealBeam;
                        }
                        if (tile.WallType == ModContent.WallType<Wood>())
                        {
                            tile.WallType = (ushort)ModContent.WallType<WoodBoreal>();
                        }
                        else if (tile.WallType == ModContent.WallType<BrickStone>())
                        {
                            tile.WallType = (ushort)ModContent.WallType<BrickIce>();
                        }
                        else if (tile.WallType == WallID.Dirt)
                        {
                            tile.WallType = WallID.IceUnsafe;
                        }
                        else if (tile.WallType == WallID.WoodenFence)
                        {
                            tile.WallType = WallID.BorealWoodFence;
                        }

                        if (tile.HasTile)
                        {
                            if (tile.TileType == TileID.Banners)
                            {
                                if (tile.TileFrameX == 0)
                                {
                                    FurnitureFraming(x, y, 18 * 2, 0);
                                }
                            }
                            else if (tile.TileType == TileID.WorkBenches)
                            {
                                FurnitureFraming(x, y, 36 * 23, 0);
                            }
                            else if (tile.TileType == TileID.ClosedDoor || tile.TileType == TileID.OpenDoor)
                            {
                                FurnitureFraming(x, y, 0, 54 * 30);
                            }
                            else if (tile.TileType == TileID.Chairs)
                            {
                                FurnitureFraming(x, y, 0, 40 * 30);
                            }
                            else if (tile.TileType == TileID.Tables)
                            {
                                FurnitureFraming(x, y, 54 * 28, 0);
                            }
                            else if (tile.TileType == TileID.Beds)
                            {
                                FurnitureFraming(x, y, 0, 36 * 24);
                            }
                            else if (tile.TileType == TileID.Benches && tile.TileFrameX >= 18 * 3)
                            {
                                FurnitureFraming(x, y, 54 * 24, 0);
                            }
                            else if (tile.TileType == TileID.Bookcases)
                            {
                                FurnitureFraming(x, y, 54 * 25, 0);
                            }
                            else if (tile.TileType == TileID.GrandfatherClocks)
                            {
                                FurnitureFraming(x, y, 36 * 6, 0);
                            }
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive)
                    {
                        if (tile.TileType == TileID.Dirt)
                        {
                            if (!WGTools.SurroundingTilesActive(x, y, true))
                            {
                                tile.TileType = TileID.JungleGrass;
                            }
                            else tile.TileType = TileID.Mud;
                        }
                        else if (tile.TileType == TileID.WoodBlock)
                        {
                            tile.TileType = TileID.RichMahogany;
                        }
                        else if (tile.TileType == TileID.Platforms)
                        {
                            if (tile.TileFrameY == 0)
                            {
                                tile.TileFrameY = 18 * 2;
                            }
                        }
                        else if (tile.TileType == TileID.WoodenBeam)
                        {
                            tile.TileType = TileID.RichMahoganyBeam;
                        }
                        else if (tile.TileType == TileID.CopperBrick)
                        {
                            tile.TileType = TileID.TinBrick;
                        }

                        if (tile.WallType == ModContent.WallType<Wood>())
                        {
                            tile.WallType = (ushort)ModContent.WallType<WoodMahogany>();
                        }
                        else if (tile.WallType == WallID.Dirt)
                        {
                            tile.WallType = jungleWall;
                        }
                        else if (tile.WallType == WallID.WoodenFence)
                        {
                            tile.WallType = WallID.RichMahoganyFence;
                        }
                        else if (tile.WallType == WallID.CopperBrick)
                        {
                            tile.WallType = WallID.TinBrick;
                        }

                        if (tile.HasTile)
                        {
                            if (tile.TileType == TileID.Banners)
                            {
                                if (tile.TileFrameX == 0)
                                {
                                    FurnitureFraming(x, y, 18 * 1, 0);
                                }
                            }
                            else if (tile.TileType == TileID.WorkBenches)
                            {
                                FurnitureFraming(x, y, 36 * 2, 0);
                            }
                            else if (tile.TileType == TileID.ClosedDoor || tile.TileType == TileID.OpenDoor)
                            {
                                FurnitureFraming(x, y, 0, 54 * 2);
                            }
                            else if (tile.TileType == TileID.Chairs)
                            {
                                FurnitureFraming(x, y, 0, 40 * 3);
                            }
                            else if (tile.TileType == TileID.Tables)
                            {
                                FurnitureFraming(x, y, 54 * 2, 0);
                            }
                            else if (tile.TileType == TileID.Beds)
                            {
                                FurnitureFraming(x, y, 0, 36 * 2);
                            }
                            else if (tile.TileType == TileID.Benches && tile.TileFrameX >= 18 * 3)
                            {
                                FurnitureFraming(x, y, 54 * 3, 0);
                            }
                            else if (tile.TileType == TileID.Bookcases)
                            {
                                FurnitureFraming(x, y, 54 * 12, 0);
                            }
                            else if (tile.TileType == TileID.GrandfatherClocks)
                            {
                                FurnitureFraming(x, y, 36 * 14, 0);
                            }
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Desert)
                    {
                        if (tile.TileType == TileID.WoodBlock)
                        {
                            tile.TileType = TileID.SmoothSandstone;
                        }
                        else if (tile.TileType == TileID.Platforms)
                        {
                            if (tile.TileFrameY == 0)
                            {
                                tile.TileFrameY = 18 * 42;
                            }
                        }
                        else if (tile.TileType == TileID.WoodenBeam)
                        {
                            tile.TileType = TileID.SandstoneColumn;
                        }
                        else if (tile.TileType == TileID.CopperBrick)
                        {
                            tile.TileType = TileID.TinBrick;
                        }

                        if (tile.WallType == ModContent.WallType<Wood>())
                        {
                            tile.WallType = WallID.Sandstone;
                        }
                        else if (tile.WallType == ModContent.WallType<BrickStone>())
                        {
                            tile.WallType = WallID.HardenedSand;
                        }
                        else if (tile.WallType == WallID.CopperBrick)
                        {
                            tile.WallType = WallID.TinBrick;
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Glowshroom)
                    {
                        if (tile.TileType == TileID.WoodBlock)
                        {
                            tile.TileType = TileID.MushroomBlock;
                        }
                        else if (tile.TileType == TileID.Platforms)
                        {
                            if (tile.TileFrameY == 0)
                            {
                                tile.TileFrameY = 18 * 18;
                            }
                        }
                        else if (tile.TileType == TileID.WoodenBeam)
                        {
                            tile.TileType = TileID.MushroomBeam;
                        }
                        if (tile.WallType == WallID.Dirt || tile.WallType == ModContent.WallType<Wood>())
                        {
                            tile.WallType = WallID.MushroomUnsafe;
                        }

                        if (tile.HasTile)
                        {
                            if (tile.TileType == TileID.WorkBenches)
                            {
                                FurnitureFraming(x, y, 36 * 7, 0);
                            }
                            else if (tile.TileType == TileID.ClosedDoor || tile.TileType == TileID.OpenDoor)
                            {
                                FurnitureFraming(x, y, 0, 54 * 6);
                            }
                            else if (tile.TileType == TileID.Chairs)
                            {
                                FurnitureFraming(x, y, 0, 40 * 9);
                            }
                            else if (tile.TileType == TileID.Tables)
                            {
                                FurnitureFraming(x, y, 54 * 27, 0);
                            }
                            else if (tile.TileType == TileID.HangingLanterns)
                            {
                                FurnitureFraming(x, y, 0, 36 * 28);
                            }
                            else if (tile.TileType == TileID.Beds)
                            {
                                FurnitureFraming(x, y, 0, 36 * 24);
                            }
                            else if (tile.TileType == TileID.Benches)
                            {
                                if (tile.TileFrameX >= 18 * 3)
                                {
                                    FurnitureFraming(x, y, 54 * 17, 0);
                                }
                                else FurnitureFraming(x, y, 54 * 23, 0);
                            }
                            else if (tile.TileType == TileID.Bookcases)
                            {
                                FurnitureFraming(x, y, 54 * 24, 0);
                            }
                            else if (tile.TileType == TileID.GrandfatherClocks)
                            {
                                FurnitureFraming(x, y, 36 * 16, 0);
                            }
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Granite)
                    {
                        if (tile.TileType == TileID.Stone)
                        {
                            tile.TileType = TileID.Granite;
                        }
                        else if (tile.TileType == TileID.WoodBlock)
                        {
                            tile.TileType = TileID.GraniteBlock;
                        }
                        else if (tile.TileType == TileID.Platforms)
                        {
                            if (tile.TileFrameY == 0)
                            {
                                tile.TileFrameY = 18 * 28;
                            }
                        }
                        else if (tile.TileType == TileID.WoodenBeam)
                        {
                            tile.TileType = TileID.GraniteColumn;
                        }
                        if (tile.WallType == ModContent.WallType<Wood>() || tile.WallType == ModContent.WallType<BrickStone>())
                        {
                            tile.WallType = WallID.GraniteBlock;
                        }
                        else if (tile.WallType == WallID.Dirt)
                        {
                            tile.WallType = WallID.GraniteUnsafe;
                        }
                    }
                    else if (tile.WallType == WallID.Dirt)
                    {
                        tile.WallType = dirtWall;
                    }

                    if (WGTools.Tile(x - 1, y - 1).TileType == TileID.Mud)
                    {
                        if (!WGTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            WGTools.Tile(x - 1, y - 1).TileType = TileID.JungleGrass;
                        }
                    }
                }
            }
        }

        public static void FurnitureFraming(int x, int y, short frameX = 0, short frameY = 0)
        {
            int type = Main.tile[x, y].TileType;
            int width = 1;
            int height = 1;

            if (type == TileID.WorkBenches)
            {
                width = 2;
            }
            else if (type == TileID.ClosedDoor || type == TileID.Banners)
            {
                height = 3;
            }
            else if (type == TileID.OpenDoor)
            {
                width = 2;
                height = 3;
            }
            else if (type == TileID.Chairs || type == TileID.HangingLanterns)
            {
                height = 2;
            }
            else if (type == TileID.Tables || type == TileID.Pianos)
            {
                width = 3;
                height = 2;
            }
            else if (type == TileID.Beds)
            {
                width = 4;
                height = 2;
            }
            else if (type == TileID.Benches)
            {
                width = 3;
                height = 2;
            }
            else if (type == TileID.Bookcases)
            {
                width = 3;
                height = 4;
            }
            else if (type == TileID.GrandfatherClocks)
            {
                width = 2;
                height = 5;
            }

            if ((width == 1 || !Main.tile[x - 1, y].HasTile || Main.tile[x - 1, y].TileType != type) && (height == 1 || !Main.tile[x, y - 1].HasTile || Main.tile[x, y - 1].TileType != type))
            {
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        Tile tile = Main.tile[x + i, y + j];

                        if (frameX != 0)
                        {
                            tile.TileFrameX = (short)(frameX + i * 18);
                        }
                        if (frameY != 0)
                        {
                            tile.TileFrameY = (short)(frameY + j * (type == TileID.Chairs ? 18 : 18));
                        }
                    }
                }
            }
        }

        public static int CommonPotion()
        {
            int[] potions = new int[4];
            potions[0] = ItemID.IronskinPotion;
            potions[1] = ItemID.SwiftnessPotion;
            potions[2] = ItemID.RegenerationPotion;
            potions[3] = ItemID.GillsPotion;

            return potions[Main.rand.Next(potions.Length)];
        }

        public static int RarePotion()
        {
            int[] potions = new int[4];
            potions[0] = ItemID.ArcheryPotion;
            potions[1] = ItemID.WaterWalkingPotion;
            potions[2] = ItemID.InvisibilityPotion;
            potions[3] = ItemID.SpelunkerPotion;

            return potions[Main.rand.Next(potions.Length)];
        }

        public static int MythicPotion()
        {
            int[] potions = new int[4];
            potions[0] = ItemID.WrathPotion;
            potions[1] = ItemID.RagePotion;
            potions[2] = ItemID.EndurancePotion;
            potions[3] = ItemID.LifeforcePotion;

            return potions[Main.rand.Next(potions.Length)];
        }
    }

    public class Mineshafts : GenPass
    {
        public Mineshafts(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        int X;
        int Y;

        bool[,,] layout;

        int roomsHorizontal;
        int roomsVertical;
        int roomWidth = 12;
        int roomHeight = 18;

        int roomsLeft => 0 - (roomsHorizontal - 1) / 2;
        int roomsRight => (roomsHorizontal - 1) / 2;

        public Rectangle location => new Rectangle(X - (roomWidth * roomsHorizontal) / 2, Y, (roomWidth * roomsHorizontal), (roomHeight * roomsVertical));

        int cellX;
        int cellY;

        Point16 roomPos => new Point16(X + cellX * roomWidth - roomWidth / 2, Y + cellY * roomHeight);

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Building mineshafts";

            int structureCount = 0;
            while (structureCount < Main.maxTilesX / 2100)
            {
                #region spawnconditions
                //X = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(600, Main.maxTilesX / 2 - 100) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 100, Main.maxTilesX - 600);
                X = structureCount == 1 ? (int)((Tundra.X + WorldGen.genRand.NextFloat(-Tundra.Size, Tundra.Size) / 2) * biomes.scale) : structureCount == 0 ? (int)((Jungle.Center + WorldGen.genRand.NextFloat(-Jungle.Size, Jungle.Size) / 2) * biomes.scale + biomes.scale / 2) : WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(600, (int)(Main.maxTilesX * 0.45f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.55f), Main.maxTilesX - 600);
                Y = (int)Main.worldSurface - 30;

                bool valid = false;
                while (!valid)
                {
                    Y--;

                    valid = true;
                    for (int i = -5; i <= 5; i++)
                    {
                        if (WorldGen.SolidTile3(X + i, Y) || Framing.GetTileSafely(X + i, Y).WallType != 0)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                int entranceY = Y + 1;
                int entranceLength = 0;
                while (Y < Main.worldSurface + 2)
                {
                    Y++;
                    entranceLength++;
                }

                roomsHorizontal = 15;// WorldGen.genRand.Next(4, 8) * 2 + 1;
                roomsVertical = Main.maxTilesY / 300;

                bool[] validTiles = TileID.Sets.GeneralPlacementTiles;
                validTiles[TileID.MushroomGrass] = true;
                validTiles[TileID.Granite] = false;
                validTiles[TileID.Sandstone] = false;

                valid = true;
                if (entranceY < Terrain.Middle && !WorldGen.genRand.NextBool(100) && structureCount != 1)//entranceY <= Main.worldSurface * 0.4f)
                {
                    valid = false;
                }
                else if (!GenVars.structures.CanPlace(location, validTiles, 25) || !GenVars.structures.CanPlace(new Rectangle(X - 7, entranceY, roomWidth + 3, Y - entranceY)))
                {
                    valid = false;
                }
                //else if (!Structures.AvoidsBiomes(location, new int[] { BiomeID.Desert, BiomeID.Granite }));
                //{
                //    valid = false;
                //}
                #endregion

                if (valid)
                {
                    #region setup
                    layout = new bool[roomsHorizontal, roomsVertical, 4];

                    GenVars.structures.AddProtectedStructure(location);

                    FastNoiseLite caves = new FastNoiseLite();
                    caves.SetNoiseType(FastNoiseLite.NoiseType.Value);
                    caves.SetFrequency(0.075f);
                    caves.SetFractalType(FastNoiseLite.FractalType.FBm);
                    caves.SetFractalOctaves(3);
                    //caves.SetFractalGain(0.6f);

                    for (int y = location.Top; y <= location.Bottom; y++)
                    {
                        for (int x = location.Left; x <= location.Right; x++)
                        {
                            float _caves = caves.GetNoise(x, y * 2);
                            if (_caves > 0)
                            {
                                WGTools.Tile(x, y).HasTile = false;
                            }
                            else
                            {
                                if (WGTools.Tile(x, y).TileType == TileID.Stalactite)
                                {
                                    WorldGen.KillTile(x, y);
                                }
                                else WGTools.Tile(x, y).HasTile = true;

                                WGTools.Tile(x, y).Slope = 0;
                            }
                            WGTools.Tile(x, y).LiquidAmount = 0;
                            WGTools.Tile(x, y).WallType = 0;
                        }
                    }
                    #endregion

                    #region rooms
                    int roomCount;

                    cellX = 0;
                    cellY = 0;
                    AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX, cellY + 1, 2);
                    Generator.GenerateStructure("Structures/common/mineshaft/entrance-room", roomPos, ModContent.GetInstance<Remnants>());
                    //WorldGen.PlaceObject(roomPos.X + 2, roomPos.Y + roomHeight - 1, TileID.Benches);
                    //WorldGen.PlaceObject(roomPos.X + 10, roomPos.Y + roomHeight - 1, TileID.Benches);

                    roomCount = 0;
                    while (roomCount < roomsVertical)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                        cellY = WorldGen.genRand.Next(1, roomsVertical);
                        //if (roomCount == 0)
                        //{
                        //    cellX = 0;
                        //    cellY = 0;
                        //}
                        if (roomCount < roomsVertical - 1)
                        {
                            cellY = roomCount + 1;
                        }

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY - 1, 2))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX, cellY, 1);

                            Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 0);
                            Generator.GenerateStructure("Structures/common/mineshaft/ladder", new Point16(roomPos.X + 3, roomPos.Y), ModContent.GetInstance<Remnants>());

                            roomCount++;
                        }
                    }

                    roomCount = 1;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                        cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY) && !FindMarker(cellX, cellY + 1) && !FindMarker(cellX + 1, cellY + 1))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX + 1, cellY + 1);
                            AddMarker(cellX + 1, cellY + 1, 2);

                            Generator.GenerateStructure("Structures/common/mineshaft/staircase-right", roomPos, ModContent.GetInstance<Remnants>());

                            roomCount--;
                        }
                    }

                    roomCount = 1;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                        cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY) && !FindMarker(cellX, cellY + 1) && !FindMarker(cellX + 1, cellY + 1))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX + 1, cellY + 1);
                            AddMarker(cellX, cellY + 1, 2);

                            Generator.GenerateStructure("Structures/common/mineshaft/staircase-left", roomPos, ModContent.GetInstance<Remnants>());

                            roomCount--;
                        }
                    }

                    roomCount = roomsVertical / 2;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                        cellY = WorldGen.genRand.Next(1, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY) && !FindMarker(cellX, cellY - 1, 2) && !FindMarker(cellX, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);
                            AddMarker(cellX, cellY, 1);
                            AddMarker(cellX, cellY, 2);

                            Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/2x1", roomPos, ModContent.GetInstance<Remnants>(), 1);

                            roomCount--;
                        }
                    }

                    roomCount = roomsVertical / 2;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                        cellY = WorldGen.genRand.Next(1, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 1, cellY - 1, 2) && !FindMarker(cellX + 1, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);
                            AddMarker(cellX + 1, cellY, 1);
                            AddMarker(cellX + 1, cellY, 2);

                            Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/2x1", roomPos, ModContent.GetInstance<Remnants>(), 2);

                            roomCount--;
                        }
                    }

                    //roomCount = 2;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                    //    cellY = WorldGen.genRand.Next(1, roomsVertical);

                    //    if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY))
                    //    {
                    //        AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);
                    //        Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/2x1", roomPos, ModContent.GetInstance<Remnants>(), 1);

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = roomsVertical / 2;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);
                            Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/2x1", roomPos, ModContent.GetInstance<Remnants>(), 0);

                            roomCount--;
                        }
                    }

                    //roomCount = roomsVertical * 2;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                    //    cellY = WorldGen.genRand.Next(1, roomsVertical);

                    //    if (!FindMarker(cellX, cellY))
                    //    {
                    //        AddMarker(cellX, cellY);

                    //        Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 2);

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = roomsVertical / 2;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY) && !FindMarker(cellX, cellY + 1, 1) && !FindMarker(cellX + 1, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);
                            AddMarker(cellX, cellY, 3);

                            roomCount--;
                        }
                    }

                    roomCount = roomsVertical * 4;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY);
                            //AddMarker(cellX, cellY, 2);

                            WGTools.Rectangle(roomPos.X, roomPos.Y + 9, roomPos.X + 1, roomPos.Y + roomHeight, TileID.WoodBlock, ModContent.WallType<Wood>());
                            WGTools.Rectangle(roomPos.X, roomPos.Y + 10, roomPos.X + 1, roomPos.Y + roomHeight - 1, -1);
                            WGTools.Rectangle(roomPos.X, roomPos.Y + 10, roomPos.X, roomPos.Y + roomHeight - 2, TileID.WoodenBeam);

                            WGTools.Rectangle(roomPos.X + roomWidth - 1, roomPos.Y + 9, roomPos.X + roomWidth, roomPos.Y + roomHeight, TileID.WoodBlock, ModContent.WallType<Wood>());
                            WGTools.Rectangle(roomPos.X + roomWidth - 1, roomPos.Y + 10, roomPos.X + roomWidth, roomPos.Y + roomHeight - 1, -1);
                            WGTools.Rectangle(roomPos.X + roomWidth, roomPos.Y + 10, roomPos.X + roomWidth, roomPos.Y + roomHeight - 1, TileID.WoodenBeam);

                            WGTools.Rectangle(roomPos.X, roomPos.Y, roomPos.X + 2, roomPos.Y + 8, TileID.Stone);
                            WGTools.Rectangle(roomPos.X + roomWidth - 2, roomPos.Y, roomPos.X + roomWidth, roomPos.Y + 8, TileID.Stone);

                            roomCount--;
                        }
                    }

                    //roomCount = roomsVertical * roomsHorizontal / 8;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                    //    cellY = WorldGen.genRand.Next(0, roomsVertical);

                    //    if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY + 1, 1))
                    //    {
                    //        AddMarker(cellX, cellY);

                    //        Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 2);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = roomsVertical * roomsHorizontal / 8;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                    //    cellY = WorldGen.genRand.Next(0, roomsVertical);

                    //    if (!FindMarker(cellX, cellY))
                    //    {
                    //        AddMarker(cellX, cellY);

                    //        WGTools.DrawRectangle(roomPos.X + 1, roomPos.Y + 1, roomPos.X + roomWidth - 2, roomPos.Y + roomHeight - 2, -1);

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = roomsVertical;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY))
                        {
                            AddMarker(cellX, cellY);

                            //if (cellY < roomsVertical - 1 && FindMarker(cellX, cellY + 1, 1))
                            //{
                            //    Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 1);
                            //}
                            //else Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 0);

                            Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 1);

                            roomCount--;
                        }
                    }

                    for (cellY = 0; cellY < roomsVertical; cellY++)
                    {
                        for (cellX = roomsLeft; cellX <= roomsRight; cellX++)
                        {
                            //if (FindMarker(cellX, cellY, 2))
                            //{
                            //    if (FindMarker(cellX - 1, cellY))
                            //    {
                            //        WGTools.Rectangle(roomPos.X + 1, roomPos.Y + 10, roomPos.X + 1, roomPos.Y + roomHeight - 1, TileID.WoodBlock);
                            //        WGTools.Rectangle(roomPos.X + 1, roomPos.Y + 11, roomPos.X + 1, roomPos.Y + roomHeight - 2, -1, ModContent.WallType<wood>());
                            //    }
                            //    if (FindMarker(cellX + 1, cellY))
                            //    {
                            //        WGTools.Rectangle(roomPos.X + roomWidth - 1, roomPos.Y + 10, roomPos.X + roomWidth - 1, roomPos.Y + roomHeight - 1, TileID.WoodBlock);
                            //        WGTools.Rectangle(roomPos.X + roomWidth - 1, roomPos.Y + 11, roomPos.X + roomWidth - 1, roomPos.Y + roomHeight - 2, -1, ModContent.WallType<wood>());
                            //    }
                            //}
                            if (!FindMarker(cellX, cellY))
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/mineshaft/1x1", roomPos, ModContent.GetInstance<Remnants>(), 0);
                                //WGTools.DrawRectangle(roomPos.X + 1, roomPos.Y + 11, roomPos.X + roomWidth - 1, roomPos.Y + roomHeight - 1, -1);
                            }
                            if (FindMarker(cellX - 2, cellY, 3))
                            {
                                cellX -= 2;
                                Generator.GenerateStructure("Structures/common/mineshaft/machine", roomPos, ModContent.GetInstance<Remnants>());

                                //WorldGen.PlaceObject(roomPos.X + 6, roomPos.Y + roomHeight - 1, TileID.Extractinator); //biomes.FindBiome(roomPos.X + 6, roomPos.Y) == BiomeID.Jungle ? ModContent.TileType<recycler>() : TileID.Extractinator);

                                cellX += 2;
                            }
                            if (FindMarker(cellX, cellY, 1))
                            {
                                WGTools.Rectangle(roomPos.X + 4, roomPos.Y, roomPos.X + 8, roomPos.Y, TileID.WoodBlock);
                                WGTools.Rectangle(roomPos.X + 5, roomPos.Y, roomPos.X + 7, roomPos.Y, TileID.Platforms);

                                if (cellY > 0 && FindMarker(cellX, cellY - 1))
                                {
                                    WGTools.Rectangle(roomPos.X + 5, roomPos.Y - 2, roomPos.X + 7, roomPos.Y - 1, -1);
                                }
                            }
                        }
                    }

                    roomCount = roomsVertical;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY, 1) && !FindMarker(cellX, cellY, 2))
                        {
                            valid = true;

                            for (int x = roomPos.X + 2; x <= roomPos.X + 10; x++)
                            {
                                if (WGTools.Tile(x, roomPos.Y + roomHeight - 2).HasTile || WGTools.Tile(x, roomPos.Y + roomHeight - 1).HasTile)
                                {
                                    valid = false;
                                    break;
                                }
                                else if (WGTools.Tile(x, roomPos.Y + roomHeight).TileType != TileID.WoodBlock && WGTools.Tile(x, roomPos.Y + roomHeight).TileType != TileID.Platforms || !WGTools.Tile(x, roomPos.Y + roomHeight).HasTile)
                                {
                                    valid = false;
                                    break;
                                }
                            }

                            if (valid)
                            {
                                WorldGen.PlaceObject(roomPos.X + 4, roomPos.Y + roomHeight - 1, TileID.Benches);
                                WorldGen.PlaceObject(roomPos.X + 8, roomPos.Y + roomHeight - 1, TileID.Benches);
                                roomCount--;
                            }
                        }
                    }

                    roomCount = roomsVertical / 2;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY, 1) && !FindMarker(cellX, cellY, 2))
                        {
                            valid = true;

                            for (int x = roomPos.X + 2; x <= roomPos.X + 10; x++)
                            {
                                if (WGTools.Tile(x, roomPos.Y + roomHeight - 2).HasTile || WGTools.Tile(x, roomPos.Y + roomHeight - 1).HasTile)
                                {
                                    valid = false;
                                    break;
                                }
                                else if (WGTools.Tile(x, roomPos.Y + roomHeight).TileType != TileID.WoodBlock && WGTools.Tile(x, roomPos.Y + roomHeight).TileType != TileID.Platforms || !WGTools.Tile(x, roomPos.Y + roomHeight).HasTile)
                                {
                                    valid = false;
                                    break;
                                }
                            }

                            if (valid)
                            {
                                WorldGen.PlaceObject(roomPos.X + 6, roomPos.Y + roomHeight - 1, TileID.Sawmill);
                                roomCount--;
                            }
                        }
                    }
                    #endregion

                    #region entrance
                    int height = 18;
                    int index = WorldGen.genRand.Next(2);

                    for (int y = entranceY + 1; y <= Y; y++)
                    {
                        WorldGen.TileRunner(X - 3, y, WorldGen.genRand.Next(6, 12), 1, TileID.Dirt, true, overRide: false);
                        WorldGen.TileRunner(X + 3, y, WorldGen.genRand.Next(6, 12), 1, TileID.Dirt, true, overRide: false);
                    }
                    WorldGen.TileRunner(X - 7, entranceY + 16, 32, 1, TileID.Dirt, true, overRide: false);
                    WorldGen.TileRunner(X + 7, entranceY + 16, 32, 1, TileID.Dirt, true, overRide: false);

                    //WGTools.Rectangle(X - 7, entranceY, X + 7, Y, TileID.Dirt);
                    WGTools.Rectangle(X - 2, entranceY, X + 2, Y, TileID.GrayBrick);
                    WGTools.Rectangle(X - 1, entranceY, X + 1, Y, -1, ModContent.WallType<BrickStone>());
                    WGTools.Rectangle(X, entranceY, X, Y, TileID.Rope);
                    GenVars.structures.AddProtectedStructure(new Rectangle(X - 8, entranceY, roomWidth + 3, Y - entranceY));

                    //WGTools.Rectangle(X - 6, Y + height / 2 + 1, X - 6, Y + height - 1, TileID.WoodenBeam);
                    //WGTools.Rectangle(X + 6, Y + height / 2 + 1, X + 6, Y + height - 1, TileID.WoodenBeam);
                    Generator.GenerateStructure("Structures/common/mineshaft/entrance", new Point16(X - 7, entranceY - 8), ModContent.GetInstance<Remnants>());

                    //WorldGen.PlaceTile(X - 8, entranceY, TileID.Platforms);
                    //WorldGen.PlaceTile(X + 8, entranceY, TileID.Platforms);

                    #endregion

                    #region cleanup

                    Structures.AddErosion(location, new[] {TileID.Dirt, TileID.Stone, TileID.Mud, TileID.JungleGrass });

                    FastNoiseLite background = new FastNoiseLite();
                    background.SetNoiseType(FastNoiseLite.NoiseType.Value);
                    background.SetFrequency(0.2f);
                    background.SetFractalType(FastNoiseLite.FractalType.FBm);
                    background.SetFractalOctaves(2);
                    background.SetFractalLacunarity(1.75f);

                    ushort dirtWall = (ushort)WorldGen.genRand.Next(4);
                    if (dirtWall == 0) { dirtWall = WallID.DirtUnsafe1; }
                    else if (dirtWall == 1) { dirtWall = WallID.DirtUnsafe2; }
                    else if (dirtWall == 2) { dirtWall = WallID.DirtUnsafe3; }
                    else { dirtWall = WallID.DirtUnsafe4; }

                    ushort jungleWall = (ushort)WorldGen.genRand.Next(3);
                    if (jungleWall == 0) { jungleWall = WallID.JungleUnsafe1; }
                    else if (jungleWall == 1) { jungleWall = WallID.JungleUnsafe2; }
                    else { jungleWall = WallID.JungleUnsafe4; }

                    for (int y = entranceY - 3; y <= location.Bottom + 10; y++)
                    {
                        for (int x = location.Left - 10; x <= location.Right + 10; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (y >= location.Top && (tile.TileType == TileID.Dirt || tile.TileType == TileID.Stone))
                            {
                                if (biomes.MaterialBlend(x, y, frequency: 2) > 0)
                                {
                                    tile.TileType = TileID.Stone;
                                }
                                else tile.TileType = TileID.Dirt;
                            }
                            else if (tile.HasTile && tile.TileType == TileID.HangingLanterns && WGTools.Tile(x, y + 1).HasTile && WGTools.Tile(x, y + 1).TileType == TileID.HangingLanterns && WorldGen.genRand.NextBool(2))
                            {
                                WGTools.Tile(x, y).TileFrameX = 18;
                                WGTools.Tile(x, y + 1).TileFrameX = 18;
                            }

                            if (tile.HasTile && tile.TileType == TileID.WoodenBeam && (!WGTools.Tile(x, y + 1).HasTile || WGTools.Tile(x, y + 1).TileType != TileID.WoodenBeam))
                            {
                                int j = y;

                                WGTools.WoodenBeam(x, j);
                            }

                            Vector2 point = new Vector2(MathHelper.Clamp(x, location.Left, location.Right), MathHelper.Clamp(y, location.Top, location.Bottom));
                            float multiplier = MathHelper.Clamp(1 - (Vector2.Distance(new Vector2(x, y), point) / 10), 0, 1);
                            if (((background.GetNoise(x * 2, y) + 1) / 2 * multiplier) > 0.5f && (tile.WallType == 0 || tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.JungleUnsafe || tile.WallType == WallID.WoodenFence || tile.WallType == WallID.IronFence))
                            {
                                tile.WallType = WallID.Dirt;
                            }
                        }
                    }
                    #endregion

                    #region objects
                    int objects;

                    objects = 5;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(location.Left, location.Right);
                        int y = WorldGen.genRand.Next(location.Top, location.Bottom + 1);

                        if (Framing.GetTileSafely(x, y).TileType != ModContent.TileType<RustedChest>() && WGTools.Tile(x, y + 1).TileType == TileID.WoodBlock && WGTools.Tile(x + 1, y + 1).TileType == TileID.WoodBlock && WGTools.Tile(x - 1, y).TileType != TileID.WoodenBeam && WGTools.Tile(x + 2, y).TileType != TileID.WoodenBeam)
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<RustedChest>(), notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<RustedChest>())
                            {
                                #region chest
                                var itemsToAdd = new List<(int type, int stack)>();

                                int[] specialItems = new int[5];
                                specialItems[0] = ItemID.ShoeSpikes;
                                specialItems[1] = ItemID.Aglet;
                                specialItems[2] = ItemID.Radar;
                                specialItems[3] = ItemID.WoodenBoomerang;
                                specialItems[4] = ItemID.PortableStool;

                                int specialItem = specialItems[(objects - 1) % specialItems.Length];
                                itemsToAdd.Add((specialItem, 1));

                                Structures.GenericLoot(chestIndex, itemsToAdd, 1, new int[] { ItemID.MiningPotion, ItemID.ShinePotion });

                                if (Main.rand.NextBool(2))
                                {
                                    itemsToAdd.Add((biomes.FindBiome(x, y) == BiomeID.Jungle ? ItemID.LeadOre : ItemID.IronOre, Main.rand.Next(15, 45)));
                                }
                                else itemsToAdd.Add((biomes.FindBiome(x, y) == BiomeID.Jungle ? ItemID.TinOre : ItemID.CopperOre, Main.rand.Next(30, 90)));

                                Structures.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }

                    //objects = roomsVertical * roomsHorizontal / 32;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    //    int y = WorldGen.genRand.Next(location.Center.Y, location.Bottom + 1);

                    //    valid = true;

                    //    if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<Tiles.Objects.Minecart>())
                    //    {
                    //        valid = false;
                    //    }
                    //    else for (int i = -1; i < 2; i++)
                    //        {
                    //            if (Framing.GetTileSafely(x + i, y + 1).TileType == TileID.Platforms)
                    //            {
                    //                valid = false;
                    //            }
                    //        }


                    //    if (valid)
                    //    {
                    //        WorldGen.PlaceObject(x, y, ModContent.TileType<Tiles.Objects.Minecart>());
                    //        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<Tiles.Objects.Minecart>())
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}

                    objects = roomsVertical * roomsHorizontal / 16;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(location.Left, location.Right);
                        int y = WorldGen.genRand.Next(location.Top + roomHeight / 2, location.Bottom + 1);

                        valid = true;

                        if (Framing.GetTileSafely(x, y).TileType == TileID.Campfire)
                        {
                            valid = false;
                        }
                        else for (int i = -1; i < 2; i++)
                            {
                                if (Framing.GetTileSafely(x + i, y - 2).HasTile)
                                {
                                    valid = false;
                                }
                                else if (Framing.GetTileSafely(x + i, y + 1).TileType == TileID.WoodBlock || Framing.GetTileSafely(x + i, y + 1).TileType == TileID.Platforms)
                                {
                                    valid = false;
                                }
                                else if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("IndustrialPanel", out ModTile IndustrialPanel) && Framing.GetTileSafely(x + i, y + 1).TileType == IndustrialPanel.Type)
                                {
                                    valid = false;
                                }
                                if (!valid) { break; }
                            }


                        if (valid)
                        {
                            WorldGen.PlaceObject(x, y, TileID.Campfire, style: biomes.FindBiome(x, y) == BiomeID.Tundra ? 3 : biomes.FindBiome(x, y) == BiomeID.Jungle ? 13 : 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Campfire)
                            {
                                objects--;
                            }
                        }
                    }

                    //objects = roomsVertical * roomsHorizontal / 16;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    //    int y = WorldGen.genRand.Next(location.Top + roomHeight / 2, location.Bottom);

                    //    if (!WGTools.GetTile(x, y).HasTile && WGTools.FullTile(x, y + 1))
                    //    {
                    //        WGTools.PlaceObjectsInArea(x - 6, y - 5, x + 5, y + 2, TileID.FishingCrate, count: WorldGen.genRand.Next(3, 7));
                    //        if (objects <= roomsVertical * roomsHorizontal / 32)
                    //        {
                    //            WGTools.PlaceObjectsInArea(x - 6, y - 5, x + 5, y + 2, TileID.AmmoBox);
                    //        }
                    //        WGTools.PlaceObjectsInArea(x - 6, y - 6, x + 6, y + 2, TileID.WaterCandle);

                    //        WGTools.PlaceObjectsInArea(x - 6, y - 5, x + 6, y + 2, TileID.Chairs, count: WorldGen.genRand.Next(4));
                    //        objects--;
                    //    }
                    //}
                    objects = roomsVertical * roomsHorizontal / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(location.Left, location.Right);
                        int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.LargePiles)
                        {
                            WorldGen.PlaceObject(x, y, TileID.LargePiles, style: Main.rand.Next(22, 26));
                            if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = roomsVertical * roomsHorizontal;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(location.Left, location.Right);
                        int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && WGTools.NoDoors(x, y, 2))
                        {
                            WGTools.MediumPile(x, y, Main.rand.Next(31, 34));
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }

                    objects = roomsVertical * roomsHorizontal * 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(location.Left, location.Right);
                        int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    objects = roomsVertical * roomsHorizontal * 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(location.Left, location.Right);
                        int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    #endregion

                    Structures.AddTheming(new Rectangle(location.X, entranceY, location.Width, location.Height + (location.Top - entranceY)));
                    Structures.AddVariation(new Rectangle(location.X, entranceY, location.Width, location.Height + (location.Top - entranceY)));

                    structureCount++;
                }
            }
        }

        #region functions

        private void DungeonRoom(int left, int top, int right, int bottom, int cellX, int cellY, int tile = -2, int wall = -2, bool add = true, bool replace = true, int style = 0, int liquid = -1, int liquidType = -1)
        {
            WGTools.Rectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom, tile, wall, add, replace, style, liquid, liquidType);
        }

        private void AddMarker(int cellX, int cellY, int layer = 0)
        {
            layout[cellX - roomsLeft, cellY, layer] = true;
        }
        private bool FindMarker(int cellX, int cellY, int layer = 0)
        {
            if (cellX < roomsLeft || cellX > roomsRight || cellY < 0 || cellY >= roomsVertical)
            {
                return false;
            }
            else return layout[cellX - roomsLeft, cellY, layer];
        }
        #endregion

        internal struct Marker
        {
            public Vector2 position;
            public int direction;
            public bool hallway;
            public Marker(Vector2 _position, int _direction, bool _hallway)
            {
                position = _position;
                direction = _direction;
                hallway = _hallway;
            }
        }
    }

    public class Microdungeons : GenPass
    {
        public Microdungeons(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Building microdungeons";
            int structureCount;

            int uniqueStructures = 6;
            int progressCounter = 0;

            structureCount = 0; // MARBLE BATHHOUSE
            while (structureCount < (Main.maxTilesX) / 1050)
            {
                progress.Set((progressCounter + (structureCount / (float)(Main.maxTilesX / 1050))) / (float)uniqueStructures);

                #region spawnconditions
                Structures.Dungeon temple = new Structures.Dungeon(0, 0, 3, structureCount == 0 ? 4 : structureCount == 1 ? 2 : Math.Max(WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 4)), 18, 12, 3);
                temple.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.425f), (int)(Main.maxTilesX * 0.575f) - temple.area.Width);
                temple.Y = (MarbleCave.Y + 1) * biomes.scale - temple.area.Height;

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(false, TileID.Marble, TileID.Obsidian);

                bool valid = true;
                if (!GenVars.structures.CanPlace(temple.area, validTiles, 10))
                {
                    valid = false;
                }
                //else if (!Structures.InsideBiome(temple.area, BiomeID.Marble))
                //{
                //    valid = false;
                //}
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(temple.area, 10);

                    for (int j = 0; j < 20; j += 3)
                    {
                        WorldGen.TileRunner(temple.area.Left - 7, temple.area.Bottom + 2 + j, WorldGen.genRand.Next(2, 7) * 2 + j / 2, 1, TileID.Marble, true, 0, 0, false, false);
                        WorldGen.TileRunner(temple.area.Right + 7, temple.area.Bottom + 2 + j, WorldGen.genRand.Next(2, 7) * 2 + j / 2, 1, TileID.Marble, true, 0, 0, false, false);
                    }

                    #region structure
                    bool[] marble = TileID.Sets.Factory.CreateBoolSet(false, TileID.Marble);
                    WGTools.Terraform(new Vector2(temple.area.Left - 5, temple.area.Bottom - 3), 5, marble);
                    WGTools.Terraform(new Vector2(temple.area.Right + 5, temple.area.Bottom - 3), 5, marble);

                    #region rooms
                    int roomCount;

                    for (int i = 0; i < temple.grid.Width; i++)
                    {
                        for (int j = 1; j < temple.grid.Height; j++)
                        {
                            temple.AddMarker(i, j);
                        }
                    }
                    int width = structureCount == 2 ? 2 : WorldGen.genRand.Next(1, temple.grid.Width + 1);
                    int pos = WorldGen.genRand.Next(0, temple.grid.Width - width);
                    for (int i = pos; i < pos + width; i++)
                    {
                        temple.AddMarker(i, 0);
                    }

                    //temple.AddMarker(0, 0, 1);
                    //roomCount = (temple.grid.Height - 1) * temple.grid.Width / 2;
                    //while (roomCount > 0)
                    //{
                    //    temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width);
                    //    temple.targetCell.Y = WorldGen.genRand.Next(0, temple.grid.Height);
                    //    if (roomCount < temple.grid.Height)
                    //    {
                    //        temple.targetCell.Y = roomCount;
                    //    }

                    //    if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = temple.grid.Height * temple.grid.Width / 8;
                    //while (roomCount > 0)
                    //{
                    //    temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width - 1);
                    //    temple.targetCell.Y = WorldGen.genRand.Next(0, temple.grid.Height);

                    //    if (!temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y + 1))
                    //    {
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y); temple.AddMarker(temple.targetCell.X, temple.targetCell.Y + 1);
                    //        temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                    //        if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                    //        {
                    //            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y + 1, 1);
                    //        }

                    //        Generator.GenerateMultistructureRandom("Structures/common/thermalrig/solid", temple.roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < temple.grid.Height - 1)
                    {
                        temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width);
                        temple.targetCell.Y = roomCount + 1;

                        if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && temple.FindMarker(temple.targetCell.X, temple.targetCell.Y - 1))
                        {
                            temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 1);

                            roomCount++;
                        }
                    }

                    #endregion

                    for (temple.targetCell.Y = temple.grid.Height - 1; temple.targetCell.Y >= 0; temple.targetCell.Y--)
                    {
                        while (true)
                        {
                            temple.targetCell.X = WorldGen.genRand.Next(0, temple.grid.Width);

                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/MarbleBathhouse/room", temple.roomPos, ModContent.GetInstance<Remnants>(), 2);
                                temple.AddMarker(temple.targetCell.X, temple.targetCell.Y, 2);

                                int chestIndex = WorldGen.PlaceChest(temple.roomPos.X + 9, temple.roomPos.Y + temple.room.Height - 1, TileID.Dressers, style: 27);

                                var itemsToAdd = new List<(int type, int stack)>();

                                Structures.GenericLoot(chestIndex, itemsToAdd);

                                Structures.FillChest(chestIndex, itemsToAdd);

                                break;
                            }
                        }

                        for (temple.targetCell.X = 0; temple.targetCell.X < temple.grid.Width; temple.targetCell.X++)
                        {
                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 2))
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/MarbleBathhouse/room", temple.roomPos, ModContent.GetInstance<Remnants>(), WorldGen.genRand.Next(2));
                            }

                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && (temple.targetCell.X == 0 || !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y)))
                            {
                                Generator.GenerateStructure("Structures/common/MarbleBathhouse/left", new Point16(temple.roomPos.X - 5, temple.roomPos.Y), ModContent.GetInstance<Remnants>());
                                WorldGen.PlaceTile(temple.roomPos.X - 6, temple.roomPos.Y, TileID.Platforms, style: 29);
                            }
                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y) && (temple.targetCell.X == temple.grid.Width - 1 || !temple.FindMarker(temple.targetCell.X + 1, temple.targetCell.Y)))
                            {
                                Generator.GenerateStructure("Structures/common/MarbleBathhouse/right", new Point16(temple.roomPos.X + temple.room.Width, temple.roomPos.Y), ModContent.GetInstance<Remnants>());
                                WorldGen.PlaceTile(temple.roomPos.X + temple.room.Width + 6, temple.roomPos.Y, TileID.Platforms, style: 29);
                            }

                            if (temple.targetCell.Y == temple.grid.Height - 1)
                            {
                                if (temple.targetCell.X == 0)
                                {
                                    WorldGen.PlaceTile(temple.roomPos.X - 6, temple.roomPos.Y + temple.room.Height, TileID.Platforms, style: 29);
                                }
                                if (temple.targetCell.X == temple.grid.Width - 1)
                                {
                                    WorldGen.PlaceTile(temple.roomPos.X + temple.room.Width + 6, temple.roomPos.Y + temple.room.Height, TileID.Platforms, style: 29);
                                }
                            }
                        }
                    }

                    for (temple.targetCell.Y = 0; temple.targetCell.Y < temple.grid.Height; temple.targetCell.Y++)
                    {
                        for (temple.targetCell.X = 0; temple.targetCell.X < temple.grid.Width; temple.targetCell.X++)
                        {
                            if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y))
                            {
                                if (temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                                {
                                    Generator.GenerateStructure("Structures/common/MarbleBathhouse/ladder", temple.roomPos, ModContent.GetInstance<Remnants>());

                                    //if (temple.targetCell.Y < temple.grid.Height - 1 && temple.targetCell.X == 0)
                                    //{
                                    //    WGTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + 7, temple.roomPos.X, temple.roomPos.Y + 11, TileID.MarbleBlock);
                                    //    WGTools.Rectangle(temple.roomPos.X - 1, temple.roomPos.Y + 6, temple.roomPos.X - 1, temple.roomPos.Y + 12, TileID.Marble);
                                    //}
                                    //else if (temple.targetCell.Y < temple.grid.Height - 1 && temple.targetCell.X == temple.grid.Width - 1)
                                    //{
                                    //    WGTools.Rectangle(temple.roomPos.X + temple.room.Width, temple.roomPos.Y + 7, temple.roomPos.X + temple.room.Width + 2, temple.roomPos.Y + 11, TileID.MarbleBlock);
                                    //    WGTools.Rectangle(temple.roomPos.X + temple.room.Width + 1, temple.roomPos.Y + 6, temple.roomPos.X + temple.room.Width + 1, temple.roomPos.Y + 12, TileID.Marble);
                                    //}
                                }

                                if (temple.targetCell.X > 0 && temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y) && !temple.FindMarker(temple.targetCell.X - 1, temple.targetCell.Y, 1) && !temple.FindMarker(temple.targetCell.X, temple.targetCell.Y, 1))
                                {
                                    WGTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + 4, temple.roomPos.X + 2, temple.roomPos.Y + temple.room.Height - 1, -1, WallID.MarbleBlock);

                                    WGTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + 3, temple.roomPos.X + 2, temple.roomPos.Y + 3, TileID.MarbleBlock);
                                    WGTools.Rectangle(temple.roomPos.X - 2, temple.roomPos.Y + temple.room.Height, temple.roomPos.X + 2, temple.roomPos.Y + temple.room.Height, TileID.MarbleBlock);

                                    WorldGen.PlaceObject(temple.roomPos.X, temple.roomPos.Y + 11, TileID.Pianos, style: 29);
                                    //WorldGen.PlaceObject(temple.roomPos.X, temple.roomPos.Y + 8, TileID.Painting3X3, style: 45);

                                    PlacePainting(temple.roomPos.X, temple.roomPos.Y + 7);
                                }
                            }
                        }
                    }

                    #region cleanup

                    for (int y = temple.area.Top - 4; y <= temple.area.Bottom + 50; y++)
                    {
                        for (int x = temple.area.Left - 6; x <= temple.area.Right + 6; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (tile.WallType == ModContent.WallType<WoodLattice>())
                            {
                                tile.WallType = 0;
                                Main.tile[x, y - 1].WallType = 0;
                            }

                            if (y > temple.area.Bottom && biomes.FindBiome(x, y) == BiomeID.Marble)
                            {
                                if (!tile.HasTile)
                                {
                                    WorldGen.PlaceTile(x, y, TileID.Marble);
                                    tile.LiquidAmount = 0;
                                }
                            }
                        }
                    }

                    Structures.AddVariation(temple.area);
                    Structures.AddDecorations(temple.area);
                    #endregion

                    #region objects
                    int objects;

                    objects = 1;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(temple.area.Left, temple.area.Right);
                        int y = WorldGen.genRand.Next(temple.area.Top, temple.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType != TileID.Platforms && WGTools.Tile(x + 1, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y, 2))
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, style: 51, notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                            {
                                #region chestloot
                                var itemsToAdd = new List<(int type, int stack)>();

                                itemsToAdd.Add((structureCount % 2 == 0 ? ItemID.HermesBoots : ItemID.AncientChisel, 1));

                                Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.TitanPotion });

                                Structures.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }

                    //objects = temple.grid.Height * temple.grid.Width;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(temple.area.Left, temple.area.Right);
                    //    int y = WorldGen.genRand.Next(temple.area.Top, temple.area.Bottom + 2);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                    //    {
                    //        WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    //objects = temple.grid.Height * temple.grid.Width;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(temple.area.Left, temple.area.Right);
                    //    int y = WorldGen.genRand.Next(temple.area.Top, temple.area.Bottom + 2);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                    //    {
                    //        WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // MINECART RAIL
            while (structureCount < Main.maxTilesY / 150)
            {
                progress.Set((progressCounter + (structureCount / (float)(Main.maxTilesY / 120))) / (float)uniqueStructures);

                #region spawnconditions
                Structures.Dungeon rail = new Structures.Dungeon(0, WorldGen.genRand.Next((int)Main.rockLayer, GenVars.lavaLine - 50), WorldGen.genRand.Next(15, 30) * (Main.maxTilesX / 4200), 2, 12, 6, 2);
                rail.X = WorldGen.genRand.Next(400, Main.maxTilesX - 400 - rail.area.Width);// (structureCount < Main.maxTilesY / 240 ^ Tundra.X > biomes.width / 2) ? WorldGen.genRand.Next(400, Main.maxTilesX / 2 - rail.area.Width / 2) : WorldGen.genRand.Next(Main.maxTilesX / 2 - rail.area.Width / 2, Main.maxTilesX - 400 - rail.area.Width);
                rail.X = (int)(rail.X / 4) * 4;

                bool[] invalidTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.Ash, TileID.Ebonstone, TileID.Crimstone, TileID.LihzahrdBrick, TileID.LivingWood);

                bool valid = true;
                if (!GenVars.structures.CanPlace(rail.area, invalidTiles, 25))
                {
                    valid = false;
                }
                //else if (Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Granite }) && structureCount < Main.maxTilesY / 600f || !Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Granite }) && structureCount >= Main.maxTilesY / 600f)
                //{
                //    valid = false;
                //}
                else if (!Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Tundra, BiomeID.Desert, BiomeID.Marble, BiomeID.Hive, BiomeID.GemCave, BiomeID.Toxic }))
                {
                    valid = false;
                }
                else if (!Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Jungle }) ^ structureCount % 5 == 0)
                {
                    valid = false;
                }
                else
                {
                    for (int i = rail.area.Left; i <= rail.area.Right; i++)
                    {
                        for (int j = rail.area.Y - 100; j <= rail.area.Y + 100; j++)
                        {
                            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.MinecartTrack)
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (!valid)
                        {
                            break;
                        }
                    }
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(rail.area, 10);

                    #region structure
                    #region rooms

                    for (int i = rail.area.Left; i <= rail.area.Right; i++)
                    {
                        for (int j = rail.area.Y - 5; j <= rail.area.Y - 1; j++)
                        {
                            if (!TileID.Sets.IsBeam[Main.tile[i, j].TileType])
                            {
                                WorldGen.KillTile(i, j);
                            }
                        }

                        WGTools.Terraform(new Vector2(i, rail.area.Y - 3), 5);
                        WorldGen.PlaceTile(i, rail.area.Y - 1, TileID.MinecartTrack);
                        WorldGen.TileFrame(i, rail.area.Y - 1);
                    }

                    bool hasStation = Structures.AvoidsBiomes(rail.area, new int[] { BiomeID.Desert, BiomeID.Granite });
                    int stationWidth = 2;
                    int stationX = WorldGen.genRand.Next(1, rail.grid.Width - stationWidth);
                    if (hasStation)
                    {
                        int ladderX = WorldGen.genRand.Next(stationX, stationX + stationWidth);

                        rail.targetCell.X = stationX;
                        rail.targetCell.Y = 0;

                        WGTools.Terraform(new Vector2(rail.room.Left, rail.room.Bottom - 3), 5);
                        WGTools.Terraform(new Vector2(rail.room.Right + rail.room.Width, rail.room.Bottom - 3), 5);

                        for (rail.targetCell.X = stationX; rail.targetCell.X < stationX + stationWidth; rail.targetCell.X++)
                        {
                            rail.AddMarker(rail.targetCell.X, 0);

                            if (rail.targetCell.X == ladderX)
                            {
                                Generator.GenerateStructure("Structures/common/MinecartRail/ladder", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                            }
                            else Generator.GenerateStructure("Structures/common/MinecartRail/room", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                        }

                        rail.targetCell.X = stationX;
                        Generator.GenerateStructure("Structures/common/MinecartRail/wall", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                        rail.targetCell.X = stationX + stationWidth;
                        Generator.GenerateStructure("Structures/common/MinecartRail/wall", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());
                    }

                    int roomCount = rail.grid.Width / 10;
                    while (roomCount > 0)
                    {
                        rail.targetCell.X = WorldGen.genRand.Next(1, rail.grid.Width - 1);

                        if (!rail.FindMarker(rail.targetCell.X - 1, 0) && !rail.FindMarker(rail.targetCell.X, 0) && !rail.FindMarker(rail.targetCell.X + 1, 0) && !rail.FindMarker(rail.targetCell.X, 0, 1))
                        {
                            rail.AddMarker(rail.targetCell.X, 0, 1);

                            roomCount--;
                        }
                    }

                    for (rail.targetCell.Y = 0; rail.targetCell.Y < rail.grid.Height; rail.targetCell.Y++)
                    {
                        for (rail.targetCell.X = 0; rail.targetCell.X < rail.grid.Width; rail.targetCell.X++)
                        {
                            if (rail.FindMarker(rail.targetCell.X, rail.targetCell.Y) || rail.FindMarker(rail.targetCell.X, rail.targetCell.Y, 1))
                            {

                            }
                            else if (rail.targetCell.Y == 0 || rail.FindMarker(rail.targetCell.X, 0))
                            {
                                Generator.GenerateStructure("Structures/common/MinecartRail/bottom", new Point16(rail.roomPos.X, rail.roomPos.Y), ModContent.GetInstance<Remnants>());

                                for (int i = rail.roomPos.X; i <= rail.room.Right; i += 4)
                                {
                                    WGTools.WoodenBeam(i, rail.roomPos.Y + 1);
                                }
                            }
                        }
                    }

                    bool[] tiles = TileID.Sets.Factory.CreateBoolSet(false, TileID.WoodBlock, TileID.WoodenBeam, TileID.BorealBeam, TileID.RichMahoganyBeam, TileID.MushroomBeam, TileID.GraniteColumn);

                    for (rail.targetCell.X = 0; rail.targetCell.X < rail.grid.Width; rail.targetCell.X++)
                    {
                        if (rail.FindMarker(rail.targetCell.X, 0, 1))
                        {
                            for (int i = rail.roomPos.X + 1; i < rail.roomPos.X + rail.room.Width; i++)
                            {
                                WorldGen.KillTile(i, rail.area.Y - 1);
                            }
                            WGTools.Tile(rail.roomPos.X, rail.area.Y - 1).TileFrameX = 1; WGTools.Tile(rail.roomPos.X + rail.room.Width, rail.area.Y - 1).TileFrameX = 1;

                            WGTools.Terraform(new Vector2(rail.roomPos.X + 6, rail.area.Y + 1), 6, tiles);
                            WGTools.Terraform(new Vector2(rail.roomPos.X + 6, rail.area.Y - 9), 8);
                        }
                    }

                    #endregion

                    #region cleanup
                    if (hasStation)
                    {
                        rail.targetCell.X = stationX;
                        rail.targetCell.Y = 0;

                        WGTools.Rectangle(rail.roomPos.X - 2, rail.room.Bottom, rail.roomPos.X - 1, rail.room.Bottom, TileID.Platforms, replace: false);
                        WGTools.Rectangle(rail.room.Right + rail.room.Width + 1, rail.room.Bottom, rail.room.Right + rail.room.Width + 2, rail.room.Bottom, TileID.Platforms, replace: false);

                        WGTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, biomes.FindBiome(rail.room.Right, rail.room.Bottom) == BiomeID.Glowshroom ? ModContent.TileType<Shroomcart>() : ModContent.TileType<Tiles.Objects.Minecart>());
                        //WGTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, TileID.GrandfatherClocks); 
                        WGTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, TileID.WorkBenches);
                        WGTools.PlaceObjectsInArea(rail.roomPos.X + 1, rail.room.Bottom - 1, rail.room.Right + rail.room.Width - 1, rail.room.Bottom - 1, TileID.Chairs);
                    }

                    Structures.AddDecorations(rail.area);
                    Structures.AddTheming(rail.area);
                    Structures.AddVariation(rail.area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // GRANITE TOWER
            int missingPiece = WorldGen.genRand.NextBool(3) ? ItemID.AncientCobaltLeggings : WorldGen.genRand.NextBool(2) ? ItemID.AncientCobaltBreastplate : ItemID.AncientCobaltHelmet;
            while (structureCount < Main.maxTilesY / 300)
            {
                progress.Set((progressCounter + (structureCount / (float)(Main.maxTilesY / 300))) / (float)uniqueStructures);

                #region spawnconditions
                Structures.Dungeon tower = new Structures.Dungeon(0, 0, 2, structureCount == 0 ? 5 : structureCount == 1 ? 3 : Math.Max(WorldGen.genRand.Next(4, 7), WorldGen.genRand.Next(4, 6)), 17, 18, 3);
                bool left = WorldGen.genRand.NextBool(2);
                tower.X = Tundra.X * biomes.scale + biomes.scale / 2 + (left ? -tower.area.Width - 35 : 35);
                tower.Y = WorldGen.genRand.Next((int)Main.worldSurface + 50, Main.maxTilesY - 500 - tower.area.Height);

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true);

                bool valid = true;
                if (!GenVars.structures.CanPlace(tower.area, validTiles, 10))
                {
                    valid = false;
                }
                else if (tower.Y + tower.area.Height < Main.rockLayer && WorldGen.genRand.NextBool(2))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(tower.area, 10);

                    WGTools.Rectangle(tower.area.Left - tower.room.Width * (left ? 1 : -1), tower.area.Top, tower.area.Right - tower.room.Width * (left ? 1 : -1), tower.area.Bottom, TileID.Granite);
                    Structures.FillEdges(tower.area.Left - tower.room.Width * (left ? 1 : -1), tower.area.Top, tower.area.Right - tower.room.Width * (left ? 1 : -1), tower.area.Bottom, TileID.Granite, false);

                    #region structure

                    #region rooms
                    for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Height; tower.targetCell.Y++)
                    {
                        tower.AddMarker(left ? 1 : 0, tower.targetCell.Y);
                        WGTools.Terraform(new Vector2(left ? tower.area.Right + 6 : tower.area.Left - 7, tower.roomPos.Y + 11), 6.5f, scaleX: 1);
                    }
                    int height = WorldGen.genRand.Next(3, tower.grid.Height + 1);
                    int pos = WorldGen.genRand.Next(0, tower.grid.Height - height);
                    for (int j = pos; j < pos + height; j++)
                    {
                        tower.AddMarker(left ? 0 : 1, j);
                    }

                    int roomCount;

                    roomCount = 0;
                    while (roomCount < tower.grid.Height - 1)
                    {
                        tower.targetCell.X = WorldGen.genRand.Next(0, tower.grid.Width);
                        tower.targetCell.Y = roomCount + 1;

                        if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && tower.FindMarker(tower.targetCell.X, tower.targetCell.Y - 1))
                        {
                            tower.AddMarker(tower.targetCell.X, tower.targetCell.Y, 1);

                            roomCount++;
                        }
                    }

                    #endregion

                    for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Height; tower.targetCell.Y++)
                    {
                        for (tower.targetCell.X = 0; tower.targetCell.X < tower.grid.Width; tower.targetCell.X++)
                        {
                            if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && !tower.FindMarker(tower.targetCell.X, tower.targetCell.Y, 1))
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/GraniteTower/room", tower.roomPos, ModContent.GetInstance<Remnants>(), !tower.FindMarker(tower.targetCell.X, tower.targetCell.Y - 1, 1) && tower.targetCell.Y > 0 && tower.FindMarker(tower.targetCell.X, tower.targetCell.Y - 1) ? 1 : 0);

                                if (WorldGen.genRand.NextBool(2))
                                {
                                    WorldGen.PlaceObject(tower.roomPos.X + 3, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: 1);
                                    WorldGen.PlaceObject(tower.roomPos.X + 5, tower.roomPos.Y + tower.room.Height - 1, TileID.Tables, style: 33);
                                    WorldGen.PlaceObject(tower.roomPos.X + 5, tower.roomPos.Y + tower.room.Height - 3, TileID.Candles, style: 28);
                                    WorldGen.PlaceObject(tower.roomPos.X + 7, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: -1);

                                    WorldGen.PlaceObject(tower.roomPos.X + 9, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: 1);
                                    WorldGen.PlaceObject(tower.roomPos.X + 11, tower.roomPos.Y + tower.room.Height - 1, TileID.Tables, style: 33);
                                    WorldGen.PlaceObject(tower.roomPos.X + 11, tower.roomPos.Y + tower.room.Height - 3, TileID.Candles, style: 28);
                                    WorldGen.PlaceObject(tower.roomPos.X + 13, tower.roomPos.Y + tower.room.Height - 1, TileID.Chairs, style: 34, direction: -1);
                                }
                                else
                                {
                                    WorldGen.PlaceObject(tower.roomPos.X + 4, tower.roomPos.Y + tower.room.Height - 1, TileID.Beds, style: 29, direction: 1);
                                    WorldGen.PlaceObject(tower.roomPos.X + 11, tower.roomPos.Y + tower.room.Height - 1, TileID.Beds, style: 29, direction: -1);

                                    int chestIndex = WorldGen.PlaceChest(tower.roomPos.X + 8, tower.roomPos.Y + tower.room.Height - 1, TileID.Dressers, style: 26);
                                    var itemsToAdd = new List<(int type, int stack)>();
                                    Structures.GenericLoot(chestIndex, itemsToAdd);
                                    Structures.FillChest(chestIndex, itemsToAdd);
                                }
                            }
                        }
                    }

                    for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Height; tower.targetCell.Y++)
                    {
                        for (tower.targetCell.X = 0; tower.targetCell.X < tower.grid.Width; tower.targetCell.X++)
                        {
                            if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y))
                            {
                                if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y, 1))
                                {
                                    Generator.GenerateStructure("Structures/common/GraniteTower/ladder", tower.roomPos, ModContent.GetInstance<Remnants>());
                                }

                                if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && (tower.targetCell.X == 0 || !tower.FindMarker(tower.targetCell.X - 1, tower.targetCell.Y)))
                                {
                                    if (!left)
                                    {
                                        Generator.GenerateStructure("Structures/common/GraniteTower/left", new Point16(tower.roomPos.X - 6, tower.roomPos.Y), ModContent.GetInstance<Remnants>());
                                        WorldGen.PlaceTile(tower.roomPos.X - 7, tower.roomPos.Y, TileID.Platforms, style: 28);
                                    }

                                    if (left || !tower.FindMarker(tower.targetCell.X - 1, tower.targetCell.Y))
                                    {
                                        WGTools.Rectangle(tower.roomPos.X - 2, tower.roomPos.Y + 12, tower.roomPos.X - 1, tower.roomPos.Y + tower.room.Height, TileID.GraniteBlock, WallID.GraniteBlock);
                                        WGTools.Rectangle(tower.roomPos.X - 1, tower.roomPos.Y + 13, tower.roomPos.X - 1, tower.roomPos.Y + tower.room.Height - 1, -1);
                                    }
                                }
                                if (tower.FindMarker(tower.targetCell.X, tower.targetCell.Y) && (tower.targetCell.X == tower.grid.Width - 1 || !tower.FindMarker(tower.targetCell.X + 1, tower.targetCell.Y)))
                                {
                                    if (left)
                                    {
                                        Generator.GenerateStructure("Structures/common/GraniteTower/right", new Point16(tower.roomPos.X + tower.room.Width, tower.roomPos.Y), ModContent.GetInstance<Remnants>());
                                        WorldGen.PlaceTile(tower.roomPos.X + tower.room.Width + 6, tower.roomPos.Y, TileID.Platforms, style: 28);
                                    }

                                    if (!left || !tower.FindMarker(tower.targetCell.X + 1, tower.targetCell.Y))
                                    {
                                        WGTools.Rectangle(tower.roomPos.X + tower.room.Width, tower.roomPos.Y + 12, tower.roomPos.X + tower.room.Width + 1, tower.roomPos.Y + tower.room.Height, TileID.GraniteBlock, WallID.GraniteBlock);
                                        WGTools.Rectangle(tower.roomPos.X + tower.room.Width, tower.roomPos.Y + 13, tower.roomPos.X + tower.room.Width, tower.roomPos.Y + tower.room.Height - 1, -1);
                                    }
                                }
                                if (tower.targetCell.Y == tower.grid.Height - 1)
                                {
                                    WGTools.Rectangle(tower.roomPos.X - (tower.targetCell.X == 0 ? 6 : 0), tower.roomPos.Y + tower.room.Height + 1, tower.roomPos.X + tower.room.Width + (tower.targetCell.X == tower.grid.Width - 1 ? 5 : -1), tower.roomPos.Y + tower.room.Height + 4, TileID.Granite);

                                    if (tower.targetCell.X == 0)
                                    {
                                        WorldGen.PlaceTile(tower.roomPos.X - 7, tower.roomPos.Y + tower.room.Height, TileID.Platforms, style: 28);
                                    }
                                    if (tower.targetCell.X == tower.grid.Width - 1)
                                    {
                                        WorldGen.PlaceTile(tower.roomPos.X + tower.room.Width + 6, tower.roomPos.Y + tower.room.Height, TileID.Platforms, style: 28);
                                    }
                                }
                            }
                        }
                    }

                    #region cleanup

                    //for (int y = temple.area.Top - 4; y <= temple.area.Bottom + 50; y++)
                    //{
                    //    for (int x = temple.area.Left - 6; x <= temple.area.Right + 6; x++)
                    //    {
                    //        Tile tile = Main.tile[x, y];
                    //    }
                    //}

                    Structures.AddVariation(tower.area);
                    Structures.AddDecorations(tower.area);
                    #endregion

                    #region objects
                    int objects;

                    objects = 1;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(tower.area.Left, tower.area.Right);
                        int y = WorldGen.genRand.Next(tower.area.Top, tower.area.Bottom);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType != TileID.Platforms && WGTools.Tile(x + 1, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y, 2))
                        {
                            int chestIndex = WorldGen.PlaceChest(x, y, style: 50, notNearOtherChests: true);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                            {
                                #region chestloot
                                var itemsToAdd = new List<(int type, int stack)>();

                                int piece = structureCount % 4 == 3 ? ItemID.AncientCobaltLeggings : structureCount % 4 == 2 ? ItemID.AncientCobaltBreastplate : structureCount % 4 == 1 ? ItemID.AncientCobaltHelmet : ItemID.CelestialMagnet;
                                if (piece != missingPiece)
                                {
                                    itemsToAdd.Add((piece, 1));
                                }

                                Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                                Structures.FillChest(chestIndex, itemsToAdd);
                                #endregion

                                objects--;
                            }
                        }
                    }
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // OVERGROWN CABIN
            while (structureCount < (Main.maxTilesX * Main.maxTilesY / 1200f) / (ModContent.GetInstance<Client>().ExperimentalWorldgen ? 840 : 420))
            {
                progress.Set((progressCounter + (structureCount / (float)(Main.maxTilesX * Main.maxTilesY / 1200f) / (ModContent.GetInstance<Client>().ExperimentalWorldgen ? 840 : 420))) / (float)uniqueStructures);

                #region spawnconditions
                Structures.Dungeon cabin = new Structures.Dungeon(WorldGen.genRand.Next(400, Main.maxTilesX - 400), 0, WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(1, 3), 12, 9, 3);

                cabin.Y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200 - cabin.area.Height);

                bool valid = true;
                if (!GenVars.structures.CanPlace(cabin.area, 25))
                {
                    valid = false;
                }
                else if (!Structures.InsideBiome(cabin.area, BiomeID.Jungle))
                {
                    valid = false;
                }
                else if (cabin.Y > GenVars.lavaLine && WorldGen.genRand.NextBool(2))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(cabin.area, 10);

                    #region structure
                    WGTools.Rectangle(cabin.area.Left, cabin.area.Top, cabin.area.Right, cabin.area.Bottom, -1);

                    WGTools.Terraform(new Vector2(cabin.area.Left, cabin.area.Bottom - 3), 5);
                    WGTools.Terraform(new Vector2(cabin.area.Right, cabin.area.Bottom - 3), 5);

                    WGTools.Rectangle(cabin.area.Left - 2, cabin.area.Bottom, cabin.area.Left - 1, cabin.area.Bottom, TileID.Platforms, replace: false);
                    WGTools.Rectangle(cabin.area.Right + 1, cabin.area.Bottom, cabin.area.Right + 2, cabin.area.Bottom, TileID.Platforms, replace: false);

                    #region rooms
                    int roomCount;

                    for (int i = 0; i < cabin.grid.Width; i++)
                    {
                        cabin.AddMarker(i, cabin.grid.Height - 1, 1);
                    }
                    if (cabin.grid.Height > 1)
                    {
                        int width = WorldGen.genRand.Next(1, cabin.grid.Width + 1);
                        int x = WorldGen.genRand.Next(0, cabin.grid.Width - width);
                        for (int i = x; i < x + width; i++)
                        {
                            cabin.AddMarker(i, cabin.grid.Height - 2, 1);
                        }
                    }

                    //AddMarker(0, 0, 1);
                    //roomCount = (cabin.grid.Height - 1) * cabin.grid.Width / 2;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);
                    //    if (roomCount < cabin.grid.Height)
                    //    {
                    //        cabin.targetCell.Y = roomCount;
                    //    }

                    //    if (!FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //    {
                    //        AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = cabin.grid.Height * cabin.grid.Width / 8;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width - 1);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);

                    //    if (!FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && !FindMarker(cabin.targetCell.X + 1, cabin.targetCell.Y + 1))
                    //    {
                    //        AddMarker(cabin.targetCell.X, cabin.targetCell.Y); AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1);
                    //        AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                    //        if (FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //        {
                    //            AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1, 1);
                    //        }

                    //        Generator.GenerateMultistructureRandom("Structures/common/thermalrig/solid", roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < cabin.grid.Height - 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = roomCount + 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = cabin.grid.Height - 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            int index = WorldGen.genRand.Next(2);
                            Generator.GenerateMultistructureSpecific("Structures/common/OvergrownCabin/bathroom", cabin.roomPos, ModContent.GetInstance<Remnants>(), index);

                            PlacePainting(cabin.roomPos.X + 6, cabin.roomPos.Y + 4);

                            int chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + (index == 0 ? 3 : 9), cabin.roomPos.Y + 8, TileID.Dressers, style: 2);

                            var itemsToAdd = new List<(int type, int stack)>();

                            itemsToAdd.Add((ItemID.FlareGun, 1));
                            itemsToAdd.Add((ItemID.Flare, WorldGen.genRand.Next(15, 30)));

                            Structures.GenericLoot(chestIndex, itemsToAdd);

                            Structures.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = 0;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            int index = WorldGen.genRand.Next(2);
                            Generator.GenerateMultistructureSpecific("Structures/common/OvergrownCabin/bed", cabin.roomPos, ModContent.GetInstance<Remnants>(), index);

                            int chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + (index == 0 ? 2 : 9), cabin.roomPos.Y + 8, style: 10);

                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[5];
                            specialItems[0] = ItemID.AnkletoftheWind;
                            specialItems[1] = ItemID.FeralClaws;
                            specialItems[2] = ItemID.Boomstick;
                            specialItems[3] = ItemID.StaffofRegrowth;
                            specialItems[4] = ItemID.FiberglassFishingPole;

                            int specialItem = specialItems[structureCount % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));
                            if (specialItem == ItemID.Boomstick)
                            {
                                itemsToAdd.Add((ItemID.MusketBall, Main.rand.Next(30, 60)));
                            }

                            Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                            Structures.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    #endregion

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width)
                            {
                                if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                                {
                                    Generator.GenerateMultistructureRandom("Structures/common/OvergrownCabin/seat", cabin.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                if (cabin.targetCell.Y == cabin.grid.Height - 1)
                                {
                                    Generator.GenerateStructure("Structures/common/OvergrownCabin/bottom", new Point16(cabin.roomPos.X, cabin.roomPos.Y + cabin.room.Height), ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && (cabin.targetCell.Y == 0 || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1)))
                            {
                                Generator.GenerateStructure("Structures/common/OvergrownCabin/roof-middle", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || (!cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X < cabin.grid.Width - 1 && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1)))
                            {
                                Generator.GenerateStructure("Structures/common/OvergrownCabin/roof-left", new Point16(cabin.roomPos.X - 2, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X > 0 && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1)))
                            {
                                Generator.GenerateStructure("Structures/common/OvergrownCabin/roof-right", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                            {
                                Generator.GenerateMultistructureRandom("Structures/common/OvergrownCabin/ladder", cabin.roomPos, ModContent.GetInstance<Remnants>());
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1)) || (!cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1)))
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/OvergrownCabin/wall", new Point16(cabin.roomPos.X, cabin.roomPos.Y), ModContent.GetInstance<Remnants>(), cabin.targetCell.Y == cabin.grid.Height - 1 ? 0 : 1);
                            }
                        }
                    }

                    #region objects
                    int objects;

                    //objects = 1;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                    //    int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType == TileID.GrayBrick && WGTools.Tile(x + 1, y + 1).TileType == TileID.GrayBrick && WGTools.NoDoors(x, y, 2))
                    //    {
                    //        int chestIndex = WorldGen.PlaceChest(x, y, style: 1, notNearOtherChests: true);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    //        {
                    //            #region chestloot
                    //            var itemsToAdd = new List<(int type, int stack)>();

                    //            int[] specialItems = new int[3];
                    //            specialItems[0] = ItemID.HermesBoots;
                    //            specialItems[1] = ItemID.CloudinaBottle;
                    //            specialItems[2] = ItemID.MagicMirror;

                    //            int specialItem = specialItems[structureCount % specialItems.Length];
                    //            itemsToAdd.Add((specialItem, 1));

                    //            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                    //            StructureTools.FillChest(chestIndex, itemsToAdd);
                    //            #endregion

                    //            objects--;
                    //        }
                    //    }
                    //}

                    objects = cabin.grid.Height * cabin.grid.Width / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.ClayPot && !WGTools.Tile(x, y - 1).HasTile && Framing.GetTileSafely(x, y + 1).TileType == TileID.RichMahogany && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceObject(x, y, TileID.ClayPot);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                            {
                                WorldGen.PlaceTile(x, y - 1, TileID.ImmatureHerbs, style: 1);
                                objects--;
                            }
                        }
                    }

                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    #endregion

                    #region cleanup
                    for (int y = cabin.area.Top - 4; y <= cabin.area.Bottom + 2; y++)
                    {
                        for (int x = cabin.area.Left - 2; x <= cabin.area.Right + 2; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (y == cabin.area.Bottom + 1 && tile.HasTile && TileID.Sets.IsBeam[tile.TileType] && (!WGTools.Tile(x, y + 1).HasTile || WGTools.Tile(x, y + 1).TileType != TileID.RichMahoganyBeam))
                            {
                                int j = y;
                                WGTools.WoodenBeam(x, j);
                            }
                        }
                    }

                    Structures.AddDecorations(cabin.area);
                    Structures.AddTheming(cabin.area);
                    Structures.AddVariation(cabin.area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // BURIED CABIN
            while (structureCount < (Main.maxTilesX * Main.maxTilesY / 1200f) / 300)
            {
                progress.Set((progressCounter + (structureCount / (float)((Main.maxTilesX * Main.maxTilesY / 1200f) / 300))) / (float)uniqueStructures);

                #region spawnconditions
                Structures.Dungeon cabin = new Structures.Dungeon(0, 0, WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(1, 3), 8, 9, 3);

                cabin.X = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f) - cabin.area.Width);
                cabin.Y = (structureCount > (Main.maxTilesX * Main.maxTilesY / 1200) / 630) ? WorldGen.genRand.Next((int)Main.rockLayer, GenVars.lavaLine - cabin.area.Height) : WorldGen.genRand.Next(GenVars.lavaLine, Main.maxTilesY - 200 - cabin.area.Height);

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.MushroomGrass, TileID.SnowBlock, TileID.IceBlock, TileID.Mud, TileID.JungleGrass, TileID.Sand, TileID.HardenedSand, TileID.Sandstone, TileID.Ebonstone, TileID.Crimstone, TileID.Ash, TileID.Marble, TileID.LihzahrdBrick);

                bool valid = true;
                if (!GenVars.structures.CanPlace(cabin.area, validTiles, 25))
                {
                    valid = false;
                }
                else if (!Structures.AvoidsBiomes(cabin.area, new int[] { BiomeID.Granite, BiomeID.Hive, BiomeID.Toxic, BiomeID.Obsidian }))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(cabin.area, 10);

                    #region structure
                    WGTools.Rectangle(cabin.area.Left, cabin.area.Top, cabin.area.Right, cabin.area.Bottom, -1);

                    WGTools.Terraform(new Vector2(cabin.area.Left, cabin.area.Bottom - 3), 5);
                    WGTools.Terraform(new Vector2(cabin.area.Right, cabin.area.Bottom - 3), 5);

                    WGTools.Rectangle(cabin.area.Left - 2, cabin.area.Bottom, cabin.area.Left - 1, cabin.area.Bottom, TileID.Platforms, replace: false);
                    WGTools.Rectangle(cabin.area.Right + 1, cabin.area.Bottom, cabin.area.Right + 2, cabin.area.Bottom, TileID.Platforms, replace: false);

                    #region rooms
                    int roomCount;

                    for (int i = 0; i < cabin.grid.Width; i++)
                    {
                        cabin.AddMarker(i, cabin.grid.Height - 1, 1);
                    }
                    if (cabin.grid.Height > 1)
                    {
                        int width = WorldGen.genRand.Next(2, cabin.grid.Width + 1);
                        int x = WorldGen.genRand.Next(0, cabin.grid.Width - width);
                        for (int i = x; i < x + width; i++)
                        {
                            cabin.AddMarker(i, cabin.grid.Height - 2, 1);
                        }
                    }

                    //cabin.AddMarker(0, 0, 1);
                    //roomCount = (cabin.grid.Height - 1) * cabin.grid.Width / 2;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);
                    //    if (roomCount < cabin.grid.Height)
                    //    {
                    //        cabin.targetCell.Y = roomCount;
                    //    }

                    //    if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //    {
                    //        cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = cabin.grid.Height * cabin.grid.Width / 8;
                    //while (roomCount > 0)
                    //{
                    //    cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width - 1);
                    //    cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);

                    //    if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && !cabin.FindMarker(cabin.targetCell.X + 1, cabin.targetCell.Y + 1))
                    //    {
                    //        cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y); cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1);
                    //        cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                    //        if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                    //        {
                    //            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y + 1, 1);
                    //        }

                    //        Generator.GenerateMultistructureRandom("Structures/common/thermalrig/solid", cabin.roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < cabin.grid.Height - 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = roomCount + 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y, 2);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = 0;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            int index = WorldGen.genRand.Next(2);
                            Generator.GenerateMultistructureSpecific("Structures/common/BuriedCabin/bed", cabin.roomPos, ModContent.GetInstance<Remnants>(), index);

                            int chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + (index == 0 ? 2 : 6), cabin.roomPos.Y + 8, TileID.Dressers, style: biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Jungle || biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Hive ? 2 : biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Tundra ? 18 : 0);

                            var itemsToAdd = new List<(int type, int stack)>();

                            itemsToAdd.Add((ItemID.FlareGun, 1));
                            itemsToAdd.Add((ItemID.Flare, WorldGen.genRand.Next(15, 30)));

                            Structures.GenericLoot(chestIndex, itemsToAdd);

                            Structures.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = cabin.grid.Height - 1;

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            Generator.GenerateStructure("Structures/common/BuriedCabin/blank", cabin.roomPos, ModContent.GetInstance<Remnants>());

                            WorldGen.PlaceObject(cabin.roomPos.X + 4, cabin.roomPos.Y + 5, TileID.Painting3X3, style: 45);

                            int chestIndex = 0;
                            int style = biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Jungle || biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Hive ? 10 : biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) == BiomeID.Tundra ? 11 : 1;
                            if (WorldGen.genRand.NextBool(2))
                            {
                                chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + 2, cabin.roomPos.Y + 8, style: style);

                                //if (structureCount % 2 == 0)
                                //{
                                //    WorldGen.PlaceObject(cabin.roomPos.X + 5, cabin.roomPos.Y + 8, TileID.Anvils);
                                //}
                                //else
                                 WorldGen.PlaceObject(cabin.roomPos.X + 5, cabin.roomPos.Y + 8, TileID.SharpeningStation);
                            }
                            else
                            {
                                chestIndex = WorldGen.PlaceChest(cabin.roomPos.X + 5, cabin.roomPos.Y + 8, style: style);

                                //if (structureCount % 2 == 0)
                                //{
                                //    WorldGen.PlaceObject(cabin.roomPos.X + 2, cabin.roomPos.Y + 8, TileID.Anvils);
                                //}
                                //else
                                WorldGen.PlaceObject(cabin.roomPos.X + 3, cabin.roomPos.Y + 8, TileID.SharpeningStation);
                            }
                            //if (biomes.FindBiome(cabin.roomPos.X + 4, cabin.roomPos.Y + 8) != BiomeID.Jungle)
                            //{
                            //    WorldGen.PlaceTile(cabin.roomPos.X + 4, cabin.roomPos.Y + 8, TileID.MetalBars, style: 2);
                            //    WorldGen.PlaceTile(cabin.roomPos.X + 4, cabin.roomPos.Y + 7, TileID.MetalBars, style: 2);
                            //}

                            var itemsToAdd = new List<(int type, int stack)>();

                            int[] specialItems = new int[5];
                            specialItems[0] = ItemID.MagicMirror;
                            specialItems[1] = ItemID.CloudinaBottle;
                            specialItems[2] = ItemID.BandofRegeneration;
                            specialItems[3] = ItemID.Mace;
                            specialItems[4] = ItemID.TreasureMagnet;

                            int specialItem = specialItems[structureCount % specialItems.Length];
                            itemsToAdd.Add((specialItem, 1));

                            Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                            Structures.FillChest(chestIndex, itemsToAdd);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < 1)
                    {
                        cabin.targetCell.X = WorldGen.genRand.Next(0, cabin.grid.Width);
                        cabin.targetCell.Y = WorldGen.genRand.Next(0, cabin.grid.Height);

                        if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1))
                        {
                            cabin.AddMarker(cabin.targetCell.X, cabin.targetCell.Y);

                            Generator.GenerateMultistructureRandom("Structures/common/BuriedCabin/books", cabin.roomPos, ModContent.GetInstance<Remnants>());

                            roomCount++;
                        }
                    }

                    #endregion

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width)
                            {
                                if (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        Generator.GenerateMultistructureSpecific("Structures/common/BuriedCabin/seat", cabin.roomPos, ModContent.GetInstance<Remnants>(), WorldGen.genRand.Next(1, 3));
                                        PlacePainting(cabin.roomPos.X + 4, cabin.roomPos.Y + 4);
                                    }
                                    else Generator.GenerateMultistructureSpecific("Structures/common/BuriedCabin/seat", cabin.roomPos, ModContent.GetInstance<Remnants>(), 0);
                                }
                                if (cabin.targetCell.Y == cabin.grid.Height - 1)
                                {
                                    Generator.GenerateStructure("Structures/common/BuriedCabin/bottom", new Point16(cabin.roomPos.X, cabin.roomPos.Y + cabin.room.Height), ModContent.GetInstance<Remnants>());
                                }
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && (cabin.targetCell.Y == 0 || !cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y - 1, 1)))
                            {
                                Generator.GenerateStructure("Structures/common/BuriedCabin/roof-middle", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || (!cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X < cabin.grid.Width - 1 && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1)))
                            {
                                Generator.GenerateStructure("Structures/common/BuriedCabin/roof-left", new Point16(cabin.roomPos.X - 2, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                            if (cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X > 0 && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1)))
                            {
                                Generator.GenerateStructure("Structures/common/BuriedCabin/roof-right", new Point16(cabin.roomPos.X, cabin.roomPos.Y - 4), ModContent.GetInstance<Remnants>());
                            }
                        }
                    }

                    for (cabin.targetCell.Y = cabin.grid.Height - 1; cabin.targetCell.Y >= 0; cabin.targetCell.Y--)
                    {
                        for (cabin.targetCell.X = 0; cabin.targetCell.X <= cabin.grid.Width; cabin.targetCell.X++)
                        {
                            if (cabin.targetCell.X < cabin.grid.Width && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 2))
                            {
                                Generator.GenerateMultistructureRandom("Structures/common/BuriedCabin/ladder", cabin.roomPos, ModContent.GetInstance<Remnants>());
                            }

                            if (cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.targetCell.X == 0 || cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.targetCell.X == cabin.grid.Width || (!cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1)) || (!cabin.FindMarker(cabin.targetCell.X - 1, cabin.targetCell.Y, 1) && cabin.FindMarker(cabin.targetCell.X, cabin.targetCell.Y, 1)))
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/BuriedCabin/wall", new Point16(cabin.roomPos.X, cabin.roomPos.Y), ModContent.GetInstance<Remnants>(), cabin.targetCell.Y == cabin.grid.Height - 1 ? 0 : 1);
                            }
                        }
                    }

                    #region objects
                    int objects;

                    //objects = 1;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                    //    int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom);

                    //    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.Tile(x, y + 1).TileType == TileID.GrayBrick && WGTools.Tile(x + 1, y + 1).TileType == TileID.GrayBrick && WGTools.NoDoors(x, y, 2))
                    //    {
                    //        int chestIndex = WorldGen.PlaceChest(x, y, style: 1, notNearOtherChests: true);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
                    //        {
                    //            #region chestloot
                    //            var itemsToAdd = new List<(int type, int stack)>();

                    //            int[] specialItems = new int[3];
                    //            specialItems[0] = ItemID.HermesBoots;
                    //            specialItems[1] = ItemID.CloudinaBottle;
                    //            specialItems[2] = ItemID.MagicMirror;

                    //            int specialItem = specialItems[structureCount % specialItems.Length];
                    //            itemsToAdd.Add((specialItem, 1));

                    //            StructureTools.GenericLoot(chestIndex, itemsToAdd, 2);

                    //            StructureTools.FillChest(chestIndex, itemsToAdd);
                    //            #endregion

                    //            objects--;
                    //        }
                    //    }
                    //}

                    objects = cabin.grid.Height * cabin.grid.Width / 2;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.ClayPot && !WGTools.Tile(x, y - 1).HasTile && Framing.GetTileSafely(x, y + 1).TileType == TileID.WoodBlock && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceObject(x, y, TileID.ClayPot);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
                            {
                                WorldGen.PlaceTile(x, y - 1, TileID.ImmatureHerbs, style: 2);
                                objects--;
                            }
                        }
                    }

                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(6), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    objects = cabin.grid.Height * cabin.grid.Width;
                    while (objects > 0)
                    {
                        int x = WorldGen.genRand.Next(cabin.area.Left, cabin.area.Right);
                        int y = WorldGen.genRand.Next(cabin.area.Top, cabin.area.Bottom + 2);

                        if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && WGTools.NoDoors(x, y))
                        {
                            WorldGen.PlaceSmallPile(x, y, Main.rand.Next(28, 35), 0);
                            if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
                            {
                                objects--;
                            }
                        }
                    }
                    #endregion

                    #region cleanup
                    for (int y = cabin.area.Top - 4; y <= cabin.area.Bottom + 2; y++)
                    {
                        for (int x = cabin.area.Left - 2; x <= cabin.area.Right + 2; x++)
                        {
                            Tile tile = Main.tile[x, y];

                            if (y == cabin.area.Bottom + 1 && tile.HasTile && TileID.Sets.IsBeam[tile.TileType] && (!WGTools.Tile(x, y + 1).HasTile || WGTools.Tile(x, y + 1).TileType != TileID.WoodenBeam))
                            {
                                int j = y;
                                WGTools.WoodenBeam(x, j);
                            }
                        }
                    }

                    Structures.AddDecorations(cabin.area);
                    Structures.AddTheming(cabin.area);
                    Structures.AddVariation(cabin.area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }

            progressCounter++;

            structureCount = 0; // MINING PLATFORM
            while (structureCount < (Main.maxTilesX * Main.maxTilesY / 1200f) / 175)
            {
                progress.Set((progressCounter + (structureCount / (float)((Main.maxTilesX * Main.maxTilesY / 1200f) / 175))) / (float)uniqueStructures);

                #region spawnconditions

                int x = structureCount < (Main.maxTilesX * Main.maxTilesY / 1200f) / (175 * 5) ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.4f), (int)(Main.maxTilesX * 0.6f)) : WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.4f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.6f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200);
                int height = Math.Max(WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 7));
                Rectangle area = new Rectangle(x - 3, y - height * 6 - 8, 7, height * 6 + 8);

                bool[] validTiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.Sand, TileID.HardenedSand, TileID.Sandstone, TileID.Ebonstone, TileID.Crimstone, TileID.Ash, TileID.LihzahrdBrick);

                bool valid = true;
                if (!GenVars.structures.CanPlace(area, validTiles, 25))
                {
                    valid = false;
                }
                else if (!Structures.AvoidsBiomes(area, new int[] { BiomeID.Glowshroom, BiomeID.Granite, BiomeID.Hive, BiomeID.Toxic, BiomeID.Obsidian }))
                {
                    valid = false;
                }
                else
                {
                    for (int j = y - height * 6 - 6; j <= y + 3; j++)
                    {
                        for (int i = x - 3; i <= x + 3; i++)
                        {
                            if (j < y - 1)
                            {
                                if (WGTools.Solid(i, j))
                                {
                                    valid = false;
                                }
                            }
                            else if (j > y)
                            {
                                if (!WGTools.Solid(i, j))
                                {
                                    valid = false;
                                }
                            }
                            else if (WGTools.Tile(i, j).LiquidAmount > 0 && WGTools.Tile(i, j).LiquidType == LiquidID.Lava)
                            {
                                valid = false;
                            }
                        }
                    }

                    if (valid)
                    {
                        int length = 0;
                        for (int i = x + 2; !WGTools.Solid(i, y - height * 6); i++)
                        {
                            length++;
                        }
                        for (int i = x - 2; !WGTools.Solid(i, y - height * 6); i--)
                        {
                            length++;
                        }
                        if (length > 40)
                        {
                            valid = false;
                        }
                    }
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(area, 10);

                    #region structure
                    WGTools.Rectangle(x - 3, y, x + 3, y, TileID.WoodBlock);
                    for (int k = 0; k < height; k++)
                    {
                        y -= 6;
                        Generator.GenerateStructure("Structures/common/Platform/ladder", new Point16(x - 3, y), ModContent.GetInstance<Remnants>());
                    }
                    for (int i = x + 2; !WGTools.Solid(i, y); i++)
                    {
                        WGTools.Tile(i, y).TileType = biomes.FindBiome(i, y) == BiomeID.Glowshroom ? TileID.MushroomBlock : biomes.FindBiome(i, y) == BiomeID.Jungle ? TileID.RichMahogany : biomes.FindBiome(i, y) == BiomeID.Tundra ? TileID.BorealWood : TileID.WoodBlock;
                        WGTools.Tile(i, y).HasTile = true;
                    }
                    for (int i = x - 2; !WGTools.Solid(i, y); i--)
                    {
                        WGTools.Tile(i, y).TileType = biomes.FindBiome(i, y) == BiomeID.Glowshroom ? TileID.MushroomBlock : biomes.FindBiome(i, y) == BiomeID.Jungle ? TileID.RichMahogany : biomes.FindBiome(i, y) == BiomeID.Tundra ? TileID.BorealWood : TileID.WoodBlock;
                        WGTools.Tile(i, y).HasTile = true;
                    }

                    Generator.GenerateStructure("Structures/common/Platform/cabin", new Point16(x - 5, y - 8), ModContent.GetInstance<Remnants>());

                    #region cleanup

                    Structures.AddTheming(area);
                    Structures.AddVariation(area);
                    #endregion
                    #endregion

                    structureCount++;
                }
            }
        }

        private void PlacePainting(int x, int y)
        {
            int style2 = Main.rand.Next(10);

            if (style2 == 0)
            {
                style2 = 20;
            }
            else if (style2 == 1)
            {
                style2 = 21;
            }
            else if (style2 == 2)
            {
                style2 = 22;
            }
            else if (style2 == 3)
            {
                style2 = 24;
            }
            else if (style2 == 4)
            {
                style2 = 25;
            }
            else if (style2 == 5)
            {
                style2 = 26;
            }
            else if (style2 == 6)
            {
                style2 = 28;
            }
            else if (style2 == 7)
            {
                style2 = 33;
            }
            else if (style2 == 8)
            {
                style2 = 34;
            }
            else if (style2 == 9)
            {
                style2 = 35;
            }

            WorldGen.PlaceObject(x, y, TileID.Painting3X3, style: style2);
        }
    }

    public class FloatingIslands : GenPass
    {
        public FloatingIslands(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Raising islands";

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int countInitial = (int)(Main.maxTilesX / 525f);
            int count = countInitial;
            while (count > 0)
            {
                bool valid = true;

                int height = WorldGen.genRand.Next((int)(30 * (Main.maxTilesY / 1200f)), (int)(60 * (Main.maxTilesY / 1200f)) + 1);
                if (count < countInitial / 2)
                {
                    height /= 4;
                }

                int width = height * 4;

                int padding = 40;

                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(500 + padding, Main.maxTilesX / 2 - 200 - width) : WorldGen.genRand.Next(Main.maxTilesX / 2 + 200, Main.maxTilesX - 500 - width - padding);
                int y = WorldGen.genRand.Next(100, (int)(Main.worldSurface * 0.5f) - height);

                if (!Structures.AvoidsBiomes(new Rectangle(x, y, width, height), new[] { BiomeID.Tundra }))
                {
                    valid = false;
                }
                else if (count == countInitial && biomes.FindBiome(x + width / 2, (int)Main.worldSurface - 50, false) != BiomeID.Jungle)
                {
                    valid = false;
                }
                else if (count == countInitial - 1 && biomes.FindBiome(x + width / 2, (int)Main.worldSurface - 50, false) == BiomeID.Jungle)
                {
                    valid = false;
                }
                //else if (MathHelper.Distance(x + width / 2, Main.dungeonX) < width / 2 + 50)
                //{
                //    valid = false;
                //}
                else
                {
                    for (int j = y - padding; j <= y + height * 1.5f + padding; j++)
                    {
                        for (int i = x - padding; i <= x + width + padding; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.HasTile && Main.tileSolid[tile.TileType])
                            {
                                valid = false;
                            }
                        }
                    }
                }

                if (valid)
                {
                    GenVars.structures.AddStructure(new Rectangle(x, y, width, height), padding);

                    CreateIsland(x + width / 2, y + height / 2, width / 3, true);
                    List<int> rooms = new List<int>() { 0, 1, 2 };

                    if (count >= countInitial / 2)
                    {

                        Structures.Dungeon tower = new Structures.Dungeon(x + width / 2, y, 1, count % 3 + 2, 23, 12);

                        while (true)
                        {
                            tower.Y++;

                            if (WGTools.Tile(tower.X - 11, tower.Y + 1).HasTile || WGTools.Tile(tower.X + 11, tower.Y + 1).HasTile)
                            {
                                break;
                            }
                        }

                        tower.X -= 11;
                        tower.Y -= tower.area.Height;

                        int type = biomes.FindBiome(tower.area.Center.X, tower.area.Bottom) == BiomeID.Jungle ? TileID.Mud : TileID.Dirt;
                        WGTools.Rectangle(tower.area.Left, tower.area.Bottom, tower.area.Right - 1, tower.area.Bottom + width / 15, type, replace: false);
                        WorldGen.TileRunner(tower.area.Left, tower.area.Bottom + 2, 8, 1, type, true, overRide: false);
                        WorldGen.TileRunner(tower.area.Right, tower.area.Bottom + 2, 8, 1, type, true, overRide: false);

                        GenVars.structures.AddProtectedStructure(tower.area, 25);

                        for (tower.targetCell.Y = 0; tower.targetCell.Y < tower.grid.Bottom; tower.targetCell.Y++)
                        {
                            if (tower.targetCell.Y == 0)
                            {
                                int index = WorldGen.genRand.Next(2);
                                Generator.GenerateMultistructureSpecific("Structures/common/SkyObservatory/top", tower.roomPos, ModContent.GetInstance<Remnants>(), index);

                                #region chest
                                int chestIndex;
                                if (index == 0)
                                {
                                    chestIndex = WorldGen.PlaceChest(tower.roomPos.X + 9, tower.roomPos.Y + 11, style: 13);
                                }
                                else chestIndex = WorldGen.PlaceChest(tower.roomPos.X + 12, tower.roomPos.Y + 11, style: 13);

                                var itemsToAdd = new List<(int type, int stack)>();

                                int[] specialItems = new int[4];
                                specialItems[0] = ItemID.Starfury;
                                specialItems[1] = ItemID.ShinyRedBalloon;
                                specialItems[2] = ItemID.LuckyHorseshoe;
                                specialItems[3] = ItemID.Binoculars;

                                int specialItem = specialItems[(count - 1) % specialItems.Length];

                                itemsToAdd.Add((specialItem, 1));

                                Structures.GenericLoot(chestIndex, itemsToAdd, 2, new int[] { ItemID.GravitationPotion, ItemID.FeatherfallPotion });

                                Structures.FillChest(chestIndex, itemsToAdd);
                                #endregion
                            }
                            else
                            {
                                int room = rooms[WorldGen.genRand.Next(0, rooms.Count)];
                                rooms.Remove(room);

                                if (room == 0)
                                {
                                    Generator.GenerateMultistructureRandom("Structures/common/SkyObservatory/study", tower.roomPos, ModContent.GetInstance<Remnants>());
                                }
                                else if (room == 1)
                                {
                                    int index = WorldGen.genRand.Next(2);
                                    Generator.GenerateMultistructureSpecific("Structures/common/SkyObservatory/bedroom", tower.roomPos, ModContent.GetInstance<Remnants>(), index);

                                    #region dresser
                                    int chestIndex = WorldGen.PlaceChest(tower.roomPos.X + (index == 0 ? 13 : 9), tower.roomPos.Y + 11, TileID.Dressers, style: 13);

                                    var itemsToAdd = new List<(int type, int stack)>();

                                    Structures.GenericLoot(chestIndex, itemsToAdd);

                                    Structures.FillChest(chestIndex, itemsToAdd);
                                    #endregion

                                    WorldGen.PlaceTile(tower.roomPos.X + (index == 0 ? 13 : 9), tower.roomPos.Y + 9, TileID.Candles, style: 12);
                                    WGTools.Tile(tower.roomPos.X + (index == 0 ? 13 : 9), tower.roomPos.Y + 9).TileFrameX = 18;
                                }
                                else if (room == 2)
                                {
                                    Generator.GenerateMultistructureRandom("Structures/common/SkyObservatory/bathroom", tower.roomPos, ModContent.GetInstance<Remnants>());
                                }

                                if (tower.targetCell.Y == tower.grid.Bottom - 1)
                                {
                                    WGTools.Rectangle(tower.area.Left - 2, tower.roomPos.Y + 9, tower.area.Left, tower.roomPos.Y + 11, -1);
                                    WGTools.Rectangle(tower.area.Right - 1, tower.roomPos.Y + 9, tower.area.Right + 1, tower.roomPos.Y + 11, -1);

                                    WorldGen.PlaceObject(tower.area.Left, tower.roomPos.Y + 10, TileID.ClosedDoor, style: 9);
                                    WorldGen.PlaceObject(tower.area.Right - 1, tower.roomPos.Y + 10, TileID.ClosedDoor, style: 9);
                                }

                                int objects = 3;
                                while (objects > 0)
                                {
                                    int pos = WorldGen.genRand.Next(tower.area.Left + 1, tower.area.Right - 1);
                                    if (WGTools.Tile(pos, tower.roomPos.Y + 5).TileType != TileID.Banners)
                                    {
                                        WorldGen.PlaceObject(pos, tower.roomPos.Y + 4, TileID.Banners, style: 6 + objects);
                                        if (WGTools.Tile(pos, tower.roomPos.Y + 4).TileType == TileID.Banners)
                                        {
                                            objects--;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int islands = 0;
                    while (islands < ModContent.GetInstance<Client>().CloudDensity * 4)
                    {
                        int size = WorldGen.genRand.Next(width / 12, width / 6);

                        if (CreateIsland(WorldGen.genRand.Next(x + size / 2, x + width - size / 2), WorldGen.genRand.Next(y, y + height), size))
                        {
                            islands++;
                        }
                    }

                    count--;
                }
            }
        }

        private bool CreateIsland(int x, int y, int size, bool forced = false)
        {
            if (!forced)
            {
                for (int j = y - size / 2; j <= y + size; j++)
                {
                    for (int i = x - size / 2; i <= x + size / 2; i++)
                    {
                        Tile tile = Main.tile[i, j];
                        if (tile.HasTile && Main.tileSolid[tile.TileType])
                        {
                            return false;
                        }
                    }
                }
            }

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetFrequency(1f / size);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int thickness = size / 2;

            for (int j = y - thickness; j <= y + size / 2 + thickness; j++)
            {
                Vector2 point = new Vector2(x, Math.Clamp(j, y, y + size / 2));

                thickness = (int)((0.25f + (point.Y - y) / (size / 2)) / 1.25f * size / 2);

                for (int i = x - size / 2 - thickness; i <= x + size / 2 + thickness; i++)
                {
                    point.X = Math.Clamp(i, x - size / 2 + (point.Y - y), x + size / 2 - (point.Y - y));

                    if (noise.GetNoise(i * 2, j) / 2 + 0.5f > Vector2.Distance(new Vector2(i, j), point) / thickness)
                    {
                        Tile tile = Main.tile[i, j];

                        tile.HasTile = true;

                        if (biomes.MaterialBlend(i, j, frequency: 2) <= -0.2f)
                        {
                            tile.TileType = TileID.Stone;
                        }
                        else if (biomes.FindBiome(i, j) == BiomeID.Jungle)
                        {
                            tile.TileType = TileID.Mud;
                        }
                        else if (biomes.MaterialBlend(i, j, true) >= 0.2f)
                        {
                            tile.TileType = TileID.ClayBlock;
                        }
                        else tile.TileType = TileID.Dirt;

                        if (tile.TileType == TileID.Stone || tile.TileType == TileID.ClayBlock)
                        {
                            for (int k = 1; k <= WorldGen.genRand.Next(4, 7); k++)
                            {
                                if (!WGTools.Tile(i, j - k).HasTile)
                                {
                                    tile.TileType = biomes.FindBiome(i, j) == BiomeID.Jungle ? TileID.Mud : TileID.Dirt;
                                    break;
                                }
                            }
                        }
                    }

                    if (WGTools.SurroundingTilesActive(i - 1, j - 1))
                    {
                        WGTools.Tile(i - 1, j - 1).WallType = WallID.DirtUnsafe;
                    }
                }
            }

            GenVars.structures.AddProtectedStructure(new Rectangle(x - size / 2, y, size, size));

            return true;
        }
    }

    public class BoulderTraps : GenPass
    {
        public BoulderTraps(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Setting boulders";

            int count = 0;

            while (count < ((Main.maxTilesX * Main.maxTilesY / 1200f) / 21) * ModContent.GetInstance<Client>().TrapFrequency)
            {
                progress.Set(count / (float)(((Main.maxTilesX * Main.maxTilesY / 1200f) / 21) * ModContent.GetInstance<Client>().TrapFrequency));

                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next((int)(Main.worldSurface), Main.maxTilesY - 300);

                if ((WorldGen.genRand.NextBool(2) || y > Main.rockLayer) && (biomes.FindBiome(x, y) == BiomeID.None || biomes.FindBiome(x, y) == BiomeID.Jungle) && GenVars.structures.CanPlace(new Rectangle(x - 2, y - 3, 6, 6), 6))
                {
                    bool valid = true;
                    for (int j = y - 3; j <= y + 2; j++)
                    {
                        for (int i = x - 2; i <= x + 3; i++)
                        {
                            if (!WGTools.Solid(i, j) || Main.tileOreFinderPriority[Main.tile[i, j].TileType] > 0)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        int length = 0;
                        while (true)
                        {
                            if (!WorldGen.SolidOrSlopedTile(Main.tile[x, y + 3 + length]) && !WorldGen.SolidOrSlopedTile(Main.tile[x + 1, y + 3 + length]))
                            {
                                length++;
                            }
                            else break;
                        }
                        if (length > 10)
                        {
                            int plateX = x + WorldGen.genRand.Next(2);
                            int plateY = y + 3;

                            while (!WGTools.Solid(plateX, plateY + 1))
                            {
                                plateY++;
                            }

                            if (!WGTools.Solid(plateX - 1, plateY) && !WGTools.Solid(plateX + 1, plateY) && WGTools.Tile(plateX, plateY).LiquidAmount < 255 && WGTools.Tile(plateX, plateY).TileType != TileID.MinecartTrack)
                            {
                                WorldGen.KillTile(plateX, plateY);
                                WorldGen.PlaceTile(plateX, plateY, TileID.PressurePlates, style: 3);
                                WGTools.Wire(plateX, plateY, plateX, y + 3);

                                WGTools.Rectangle(x - 1, y - 2, x + 2, y + 1, TileID.GrayBrick);
                                WGTools.Rectangle(x, y - 1, x + 1, y, -1, ModContent.WallType<BrickStone>());
                                WorldGen.PlaceTile(x + 1, y, TileID.Boulder);

                                for (int j = y + 1; j <= y + 2; j++)
                                {
                                    for (int i = x; i <= x + 1; i++)
                                    {
                                        WGTools.Tile(i, j).RedWire = true;
                                        WGTools.Tile(i, j).HasActuator = true;
                                    }
                                }

                                GenVars.structures.AddProtectedStructure(new Rectangle(x - 2, y - 3, 6, 6));

                                count++;
                            }
                        }
                    }
                }
            }
        }
    }

    public class IceTemples : GenPass
    {
        public IceTemples(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        List<Marker> markers;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Microdungeons: Ice Temples";

            int structureCount;

            structureCount = 0;
            while (structureCount <= Main.maxTilesX * (Main.maxTilesY / 1200) / 1400)
            {
                #region spawnconditions
                int structureX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int structureY = WorldGen.genRand.Next((int)(Main.worldSurface + 50), Main.maxTilesY - 300);

                bool valid = true;
                if (!GenVars.structures.CanPlace(new Rectangle(structureX - 13, structureY - 25, 28, 27), 50))
                {
                    valid = false;
                }
                else if (biomes.FindBiome(structureX, structureY) != BiomeID.Tundra)
                {
                    valid = false;
                }
                else if (ModLoader.TryGetMod("Stellamod", out Mod lv) && lv.TryFind<ModTile>("AbyssalDirt", out ModTile aDirt) && Main.tile[structureX, structureY].TileType == aDirt.Type)
                {
                    valid = false;
                }
                #endregion

                #region structure
                if (valid)
                {
                    markers = new List<Marker>();

                    int chestIndex;
                    if (WorldGen.genRand.NextBool(2))
                    {
                        Generator.GenerateMultistructureSpecific("Structures/common/icetemple/treasureroom", new Point16(structureX - 13, structureY - 33), ModContent.GetInstance<Remnants>(), 0);
                        chestIndex = WorldGen.PlaceChest(structureX + 2, structureY - 2, style: 11);
                    }
                    else
                    {
                        Generator.GenerateMultistructureSpecific("Structures/common/icetemple/treasureroom", new Point16(structureX - 13, structureY - 33), ModContent.GetInstance<Remnants>(), 1);
                        chestIndex = WorldGen.PlaceChest(structureX - 2, structureY - 2, style: 11);
                    }
                    #region chestloot
                    var itemsToAdd = new List<(int type, int stack)>();

                    int[] specialItems = new int[6];
                    specialItems[0] = ItemID.IceSkates;
                    specialItems[1] = ItemID.BlizzardinaBottle;
                    specialItems[2] = ItemID.IceBoomerang;
                    specialItems[3] = ItemID.SnowballCannon;
                    specialItems[4] = ItemID.IceBlade;
                    specialItems[5] = ItemID.IceMirror;

                    int specialItem = specialItems[WorldGen.genRand.Next(0, specialItems.Length)];
                    if (structureCount < specialItems.Length)
                    {
                        specialItem = specialItems[structureCount];
                    }

                    itemsToAdd.Add((specialItem, 1));
                    if (specialItem == ItemID.SnowballCannon)
                    {
                        itemsToAdd.Add((ItemID.Snowball, Main.rand.Next(25, 50)));
                    }

                    if (WorldGen.genRand.NextBool(25))
                    {
                        itemsToAdd.Add((ItemID.Fish, 1));
                    }

                    Structures.GenericLoot(chestIndex, itemsToAdd, 2);

                    Structures.FillChest(chestIndex, itemsToAdd);
                    #endregion
                    GenVars.structures.AddProtectedStructure(new Rectangle(structureX - 13, structureY - 33, 28, 35));
                    AddMarker(structureX - 14, structureY - 2, 4, true);
                    AddMarker(structureX + 15, structureY - 2, 2, true);
                    AddMarker(structureX - 14, structureY - 19, 4, true);
                    AddMarker(structureX + 15, structureY - 19, 2, true);

                    #region rooms
                    int index = WorldGen.genRand.Next(0, markers.Count);
                    //PlaceRoom(4);
                    for (int i = 0; i < 20; i++)
                    {
                        index = WorldGen.genRand.Next(0, markers.Count);
                        PlaceRoom(index);
                    }
                    #endregion

                    while (markers.Count > 0)
                    {
                        int x = (int)markers[0].position.X;
                        int y = (int)markers[0].position.Y;
                        if (markers[0].direction == 2 || markers[0].direction == 4)
                        {
                            Tunnel(new Vector2(x, y - 5.5f), 7f, 3);

                            if (markers[0].direction == 2)
                            {
                                WorldGen.PlaceTile(x, y + 2, TileID.Platforms, style: 35);
                            }
                            else WorldGen.PlaceTile(x, y + 2, TileID.Platforms, style: 35);
                        }
                        else Tunnel(new Vector2(x, y), 1.5f, scaleY: 2);

                        RemoveMarker(0);
                    }

                    structureCount++;
                }
                #endregion
            }
            //Spikes();
            #region cleanup
            FastNoiseLite weathering = new FastNoiseLite();
            weathering.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            weathering.SetFrequency(0.2f);
            weathering.SetFractalType(FastNoiseLite.FractalType.FBm);
            weathering.SetFractalOctaves(3);

            for (int y = (int)Main.worldSurface; y < Main.maxTilesY - 200; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = WGTools.Tile(x, y);

                    if (WGTools.Tile(x, y + 1).TileType == TileID.PlanterBox && WGTools.Tile(x, y + 1).TileFrameY == 6 * 18 && WorldGen.genRand.NextBool(2))
                    {
                        WorldGen.PlaceTile(x, y, TileID.ImmatureHerbs, style: 6);
                    }

                    if (tile.WallType == ModContent.WallType<BrickIce>())
                    {
                        if (tile.TileType == TileID.Platforms)
                        {
                            WorldGen.KillTile(x, y, true);
                        }
                        else if (WGTools.Solid(x, y) && !WGTools.Tile(x, y + 1).HasTile && weathering.GetNoise(x + WorldGen.genRand.Next(-3, 4), y + WorldGen.genRand.Next(-3, 4)) > 0)
                        {
                            if (tile.TileType == TileID.IceBrick)
                            {
                                tile.TileType = TileID.IceBlock;
                            }
                        }
                        if (WGTools.Tile(x, y - 1).TileType == TileID.Torches && !WorldGen.genRand.NextBool(4))
                        {
                            WGTools.Tile(x, y - 1).TileFrameX += 18 * 3;
                        }
                    }
                }
            }
            #endregion
        }

        #region functions
        private void PlaceRoom(int index, int id = -1)
        {
            Marker savedMarker = markers[index];

            #region setup
            int x = (int)savedMarker.position.X;
            int y = (int)savedMarker.position.Y;

            if (id == -1)
            {
                if (savedMarker.direction == 3)
                {
                    id = 2;
                }
                else if (savedMarker.direction == 1)
                {
                    id = 3;
                }
                else if (savedMarker.door)
                {
                    id = 0;
                }
                else if (WorldGen.genRand.NextBool(2))
                {
                    id = WorldGen.genRand.Next(2, 4);
                }
                //else if (WorldGen.genRand.NextBool(3))
                //{
                //    id = 4;
                //}
                else id = 1;
            }

            Rectangle room = new Rectangle(x, y, 7, 18);
            if (id == 4)
            {
                room.Width *= 2;
            }

            int doorY = 0;

            doorY += room.Height - 4;

            if (savedMarker.direction == 4)
            {
                room.X -= (room.Width - 1);
            }
            if (savedMarker.direction == 1)
            {
                room.Y -= (room.Height - 1);
            }
            if (savedMarker.direction == 1 || savedMarker.direction == 3)
            {
                room.X -= (room.Width - 1) / 2;
            }
            else room.Y -= doorY;
            #endregion

            bool[] tiles = TileID.Sets.Factory.CreateBoolSet(true, TileID.Granite);
            if (GenVars.structures.CanPlace(new Rectangle(room.X + 1, room.Y + 1, room.Width - 2, room.Height - 2), tiles))
            {
                Point16 position = new Point16(room.X, room.Y);

                //Fill(room.Center.ToVector2(), room.Height);

                RemoveMarker(index);
                if (id == 0)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/icetemple/doorway", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 1)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/icetemple/basic", position, ModContent.GetInstance<Remnants>());
                }
                else if (id == 2)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/icetemple/shaft-bottom", position, ModContent.GetInstance<Remnants>());
                    if (savedMarker.direction != 3)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y, 1, false);
                    }
                }
                else if (id == 3)
                {
                    Generator.GenerateMultistructureRandom("Structures/common/icetemple/shaft-top", position, ModContent.GetInstance<Remnants>());
                    if (savedMarker.direction != 1)
                    {
                        AddMarker(room.X + (room.Width - 1) / 2, room.Y + (room.Height - 1), 3, false);
                    }
                }
                else if (id == 4)
                {
                    Generator.GenerateStructure("Structures/common/icetemple/pool", position, ModContent.GetInstance<Remnants>());
                    //WGTools.DrawRectangle(position.X + 5, position.Y + 11, position.X + 8, position.Y + 11, liquid: 255, liquidType: 0);
                }

                if (savedMarker.direction != 2)
                {
                    AddMarker(room.X - 1, room.Y + doorY, 4, id != 0 && WorldGen.genRand.NextBool(2));
                }
                if (savedMarker.direction != 4)
                {
                    AddMarker(room.X + room.Width, room.Y + doorY, 2, id != 0 && WorldGen.genRand.NextBool(2));
                }

                GenVars.structures.AddProtectedStructure(room);
            }
        }

        private void Tunnel(Vector2 position, float size, float scaleX = 1, float scaleY = 1)
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFrequency(0.2f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalLacunarity(1.75f);

            for (int y = (int)(position.Y - size * 2 * scaleY); y <= position.Y + size * 2 * scaleY; y++)
            {
                for (int x = (int)(position.X - size * 2 * scaleX); x <= position.X + size * 2 * scaleX; x++)
                {
                    float threshold = (Vector2.Distance(position, new Vector2((x - position.X) / scaleX + position.X, (y - position.Y) / scaleY + position.Y)) / size);
                    Tile tile = WGTools.Tile(x, y);
                    if (GenVars.structures.CanPlace(new Rectangle(x, y, 1, 1)) && WGTools.Solid(x, y) && tile.TileType != TileID.IceBrick && tile.TileType != TileID.ClosedDoor)
                    {
                        if (noise.GetNoise(x, y) <= 1 - threshold)
                        {
                            tile.HasTile = false;
                        }
                    }
                }
            }
        }

        private void AddMarker(int x, int y, int direction, bool shaft)
        {
            markers.Add(new Marker(new Vector2(x, y), direction, shaft));

            if (x <= Main.maxTilesX * 0.1f || x >= Main.maxTilesX * 0.9f)
            {
                RemoveMarker(markers.Count - 1);
            }
        }

        private void RemoveMarker(int index)
        {
            int x = (int)markers[index].position.X;
            int y = (int)markers[index].position.Y;

            if (markers[index].direction == 1)
            {
                WGTools.Rectangle(x - 1, y + 1, x + 1, y + 1, type: -1, style: 35);
                WorldGen.PlaceTile(x, y + 1, TileID.Chain);
            }
            else if (markers[index].direction == 3)
            {
                WGTools.Rectangle(x - 1, y - 1, x + 1, y - 1, TileID.Platforms, style: 35, replace: false);
            }

            markers.RemoveAt(index);
        }
        #endregion

        internal struct Marker
        {
            public Vector2 position;
            public int direction;
            public bool door;
            public Marker(Vector2 _position, int _direction, bool _shaft)
            {
                position = _position;
                direction = _direction;
                door = _shaft;
            }
        }
    }

    public class ThermalRigs : GenPass
    {
        public ThermalRigs(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        int X;
        int Y;

        int[,,] layout;

        int roomsHorizontal;
        int roomsVertical;
        int roomWidth = 8;
        int roomHeight = 12;

        Rectangle location => new Rectangle(X, Y, (roomWidth * roomsHorizontal) - 1, (roomHeight * roomsVertical) - 1);

        int cellX;
        int cellY;

        Point16 roomPos => new Point16(X + cellX * roomWidth, Y + cellY * roomHeight);

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = "Building geothermal rigs";

            int structureCount;

            structureCount = Main.maxTilesX / 525;
            while (structureCount > 0)
            {
                #region spawnconditions
                roomsHorizontal = WorldGen.genRand.Next(3, 7);
                roomsVertical = WorldGen.genRand.Next(2, 5);

                X = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(100, (int)(Main.maxTilesX * 0.25f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.75f), Main.maxTilesX - 100) - location.Width;
                Y = WorldGen.genRand.Next(Main.maxTilesY - 120 - location.Height, Main.maxTilesY - 100 - location.Height);

                bool valid = true;
                if (!GenVars.structures.CanPlace(location, 25))
                {
                    valid = false;
                }
                #endregion

                if (valid)
                {
                    GenVars.structures.AddProtectedStructure(location);

                    #region structure
                    layout = new int[roomsHorizontal, roomsVertical, 3];
                    WGTools.Rectangle(location.Left, location.Top, location.Right, location.Bottom, -1);

                    for (cellY = roomsVertical - 1; cellY >= 0; cellY--)
                    {
                        for (cellX = 0; cellX < roomsHorizontal; cellX++)
                        {
                            if (cellX == 0)
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/thermalrig/side", new Point16(roomPos.X - 2, roomPos.Y), ModContent.GetInstance<Remnants>(), 0);
                            }
                            if (cellX == roomsHorizontal - 1)
                            {
                                Generator.GenerateMultistructureSpecific("Structures/common/thermalrig/side", new Point16(roomPos.X + roomWidth, roomPos.Y), ModContent.GetInstance<Remnants>(), 1);
                            }
                        }
                    }

                    #region rooms
                    int roomCount;

                    //AddMarker(0, 0, 1);
                    //roomCount = (roomsVertical - 1) * roomsHorizontal / 2;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                    //    cellY = WorldGen.genRand.Next(0, roomsVertical);
                    //    if (roomCount < roomsVertical)
                    //    {
                    //        cellY = roomCount;
                    //    }

                    //    if (!FindMarker(cellX, cellY, 1))
                    //    {
                    //        AddMarker(cellX, cellY, 1);

                    //        roomCount--;
                    //    }
                    //}

                    //roomCount = roomsVertical * roomsHorizontal / 8;
                    //while (roomCount > 0)
                    //{
                    //    cellX = WorldGen.genRand.Next(0, roomsHorizontal - 1);
                    //    cellY = WorldGen.genRand.Next(0, roomsVertical);

                    //    if (!FindMarker(cellX, cellY) && !FindMarker(cellX + 1, cellY + 1))
                    //    {
                    //        AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);
                    //        AddMarker(cellX, cellY, 2);

                    //        if (FindMarker(cellX, cellY, 1))
                    //        {
                    //            AddMarker(cellX, cellY + 1, 1);
                    //        }

                    //        Generator.GenerateMultistructureRandom("Structures/common/thermalrig/solid", roomPos, ModContent.GetInstance<Remnants>());

                    //        roomCount--;
                    //    }
                    //}

                    roomCount = 0;
                    while (roomCount < roomsVertical - 1)
                    {
                        cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                        cellY = roomCount;

                        if (!FindMarker(cellX, cellY))
                        {
                            AddMarker(cellX, cellY);
                            AddMarker(cellX, cellY, 1);

                            Generator.GenerateMultistructureSpecific("Structures/common/thermalrig/open", roomPos, ModContent.GetInstance<Remnants>(), 1);

                            roomCount++;
                        }
                    }

                    roomCount = 0;
                    while (roomCount < roomsVertical * roomsHorizontal / 8 + roomsHorizontal)
                    {
                        cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                        if (roomCount < roomsHorizontal)
                        {
                            cellY = roomsVertical - 1;
                        }
                        else cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY);

                            Generator.GenerateMultistructureRandom("Structures/common/thermalrig/solid", roomPos, ModContent.GetInstance<Remnants>());

                            roomCount++;
                        }
                    }

                    roomCount = roomsVertical * roomsHorizontal / 4;
                    while (roomCount > 0)
                    {
                        cellX = WorldGen.genRand.Next(0, roomsHorizontal);
                        cellY = WorldGen.genRand.Next(0, roomsVertical);

                        if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY + 1, 1))
                        {
                            AddMarker(cellX, cellY);
                            AddMarker(cellX, cellY, 2);

                            Generator.GenerateMultistructureRandom("Structures/common/thermalrig/partial", roomPos, ModContent.GetInstance<Remnants>());

                            roomCount--;
                        }
                    }
                    #endregion

                    for (cellY = roomsVertical - 1; cellY >= 0; cellY--)
                    {
                        for (cellX = 0; cellX < roomsHorizontal; cellX++)
                        {
                            if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("IndustrialPanel", out ModTile IndustrialPanel))
                            {
                                if (!FindMarker(cellX, cellY))
                                {
                                    Generator.GenerateMultistructureSpecific("Structures/common/thermalrig/open", roomPos, ModContent.GetInstance<Remnants>(), 0);
                                }
                                if (FindMarker(cellX, cellY, 2))
                                {
                                    WorldGen.PlaceObject(roomPos.X + 4, roomPos.Y + 11, TileID.Hellforge);
                                }

                                if (!FindMarker(cellX, cellY - 1) || FindMarker(cellX, cellY - 1, 1))
                                {
                                    //if (FindMarker(cellX, cellY) && !FindMarker(cellX, cellY, 1))
                                    //{
                                    //    for (int i = roomPos.X + 1; i < roomPos.X + roomWidth - 1; i++)
                                    //    {
                                    //        if (WGTools.Solid(i - 1, roomPos.Y) && WGTools.Solid(i, roomPos.Y) && WGTools.Solid(i + 1, roomPos.Y))
                                    //        {
                                    //            WGTools.Tile(i, roomPos.Y).TileType = TileID.IronBrick;
                                    //        }
                                    //    }
                                    //}
                                    if (wombatqol.TryFind("IndustrialPlatform", out ModTile IndustrialPlatform))
                                    {
                                        WorldGen.PlaceTile(roomPos.X, roomPos.Y, IndustrialPanel.Type); WorldGen.PlaceTile(roomPos.X + 8, roomPos.Y, IndustrialPanel.Type);
                                        WGTools.Rectangle(roomPos.X + 1, roomPos.Y, roomPos.X + 7, roomPos.Y, IndustrialPlatform.Type, replace: false);
                                    }
                                }
                            }
                        }
                    }

                    #region objects
                    int objects;
                    //objects = 6;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    //    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                    //    valid = true;

                    //    if (Framing.GetTileSafely(x, y).TileType == TileID.FishingCrate)
                    //    {
                    //        valid = false;
                    //    }
                    //    else for (int i = 0; i <= 1; i++)
                    //        {
                    //            if (WGTools.GetTile(x + i, y + 1).TileType == ModContent.TileType<factoryplatform>() || WGTools.GetTile(x + i, y + 1).TileType == TileID.Platforms || WGTools.GetTile(x + i, y + 1).TileType == TileID.Tables)
                    //            {
                    //                valid = false;
                    //                break;
                    //            }
                    //        }

                    //    if (valid)
                    //    {
                    //        WorldGen.PlaceObject(x, y, TileID.FishingCrate, style: 0);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.FishingCrate)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    //objects = 3;
                    //while (objects > 0)
                    //{
                    //    int x = WorldGen.genRand.Next(location.Left, location.Right);
                    //    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

                    //    valid = true;

                    //    if (Framing.GetTileSafely(x, y).TileType == TileID.AmmoBox)
                    //    {
                    //        valid = false;
                    //    }
                    //    else for (int i = 0; i <= 1; i++)
                    //        {
                    //            if (WGTools.GetTile(x + i, y + 1).TileType == ModContent.TileType<factoryplatform>() || WGTools.GetTile(x + i, y + 1).TileType == TileID.Platforms || WGTools.GetTile(x + i, y + 1).TileType == TileID.Tables)
                    //            {
                    //                valid = false;
                    //                break;
                    //            }
                    //        }

                    //    if (valid)
                    //    {
                    //        WorldGen.PlaceObject(x, y, TileID.AmmoBox);
                    //        if (Framing.GetTileSafely(x, y).TileType == TileID.AmmoBox)
                    //        {
                    //            objects--;
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion

                    structureCount--;
                }
            }
        }

        private void DungeonRoom(int left, int top, int right, int bottom, int cellX, int cellY, int tile = -2, int wall = -2, bool add = true, bool replace = true, int style = 0, int liquid = -1, int liquidType = -1)
        {
            WGTools.Rectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top - roomHeight / 2, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom - roomHeight / 2, tile, wall, add, replace, style, liquid, liquidType);
        }
        private void AddMarker(int cellX, int cellY, int layer = 0)
        {
            layout[cellX, cellY, layer] = -1;
        }
        private bool FindMarker(int cellX, int cellY, int layer = 0)
        {
            //if (cellY >= roomsVertical)
            //{
            //    return true;
            //}
            if (cellX < 0 || cellX >= roomsHorizontal || cellY < 0 || cellY >= roomsVertical)
            {
                return false;
            }
            return layout[cellX, cellY, layer] == -1;
        }
    }
}