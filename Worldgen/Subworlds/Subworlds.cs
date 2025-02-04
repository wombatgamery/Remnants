//using Microsoft.Xna.Framework;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.ID;
//using Terraria.IO;
//using Terraria.ModLoader;
//using Terraria.WorldBuilding;
////using SubworldLibrary;
//using Remnants.Tiles.blocks;
//using Remnants.Walls;
//using Remnants.Walls.dev;
//using Remnants.Tiles;
//using StructureHelper;
//using Terraria.DataStructures;
//using Remnants.Walls.bg;
//using System;

//namespace Remnants
//{
//    public class VaultSubworld : Subworld
//    {
//        public override int Width => 2100;
//        public override int Height => 1200;

//        public override bool ShouldSave => false; //set this to false for testing new worldgen

//        public override void Load()
//        {
//            Main.bloodMoon = false;
//            Main.pumpkinMoon = false;
//            Main.snowMoon = false;
//            Main.eclipse = false;

//            base.Load();
//        }

//        public override void Unload()
//        {
//            base.Unload();
//        }

//        public override List<GenPass> Tasks => new List<GenPass>()
//        {
//            //new SubworldGenPass(progress => WorldSetup(progress)),
//            //new SubworldGenPass(progress => TheVault(progress))
//        };

//        private void WorldSetup(GenerationProgress progress)
//        {
//            progress.Message = "Setting things up";

//            Main.worldSurface = 1; //Hides the underground layer just out of bounds
//            Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds

//            Main.spawnTileX = Main.maxTilesX / 2;
//            Main.spawnTileY = 50;
//        }

//        public static int X;
//        public static int Y;
//        public Rectangle location;

//        public int[,,] layout;

//        public int roomsVertical;
//        public int roomsHorizontal;

//        public int roomWidth => 40;
//        public int roomHeight => 40;
//        public int roomsLeft => 0 - (roomsHorizontal - 1) / 2;
//        public int roomsRight => (roomsHorizontal - 1) / 2;

//        private void TheVault(GenerationProgress progress)
//        {
//            progress.Message = "Building the vault";

//            X = Main.maxTilesX / 2;
//            Y = 120;

//            roomsVertical = 12;
//            roomsHorizontal = 8 + 1;

//            location = new Rectangle(X - (roomWidth * roomsHorizontal) / 2 - 60, Y - roomHeight / 2 - 60, (roomWidth * roomsHorizontal) + 120, (roomHeight * roomsVertical) + 120);
//            //WorldGen.structures.AddProtectedStructure(location);

//            WGTools.DrawRectangle(location.Left, location.Top, location.Right, location.Bottom, ModContent.TileType<hardstone>(), add: true, replace: true, liquid: 0);
//            WGTools.DrawRectangle(location.Left + 1, location.Top + 1, location.Right - 1, location.Bottom - 1, wall: ModContent.WallType<vault>(), add: true, replace: true);

//            SpecialRooms();
//            StandardRooms(4);
//            FillerRooms(3);
//            Caves();

//            Roughness();

//            #region cleanup
//            for (int x = location.Left + 1; x <= location.Right - 1; x++)
//            {
//                for (int y = location.Top + 1; y <= location.Bottom - 1; y++)
//                {
//                    if (WGTools.GetTile(x, y).TileType == ModContent.TileType<vaultbrick>() || WGTools.GetTile(x, y).TileType == ModContent.TileType<VaultPlating>())
//                    {
//                        WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<vaultwallunsafe>();
//                        if (WGTools.GetTile(x, y).TileType == ModContent.TileType<vaultbrick>())
//                        {
//                            WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<VaultPlating>();
//                        }
//                    }
//                    else if (WGTools.GetTile(x, y).TileType == ModContent.TileType<hardstone>() && WGTools.SurroundingTilesActive(x, y, true))
//                    {
//                        WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<hardstonewallvault>();
//                    }
//                    else if (WGTools.GetTile(x, y).WallType == ModContent.WallType<hardstonewallvault>())
//                    {
//                        WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<vault>();
//                    }
//                }
//            }
//            #endregion

//            AshPiles();
//            Ores();
//            RandomBricks();
//        }

//        #region rooms
//        public void StandardRooms(int ids)
//        {
//            int roomID = 1;
//            int attempts = 1000;
//            while (attempts > 0)
//            {
//                attempts--;

//                if (roomID == 1)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY) || FindMarker(cellX + 1, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenLeft(cellX, cellY) || !OpenTop(cellX, cellY) || !OpenRight(cellX + 1, cellY) || !OpenBottom(cellX + 1, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY + 1) && OpenBottom(cellX, cellY) || FindMarker(cellX + 1, cellY - 1) && OpenTop(cellX + 1, cellY))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//                        AddMarker(cellX, cellY, 3);
//                        AddMarker(cellX + 1, cellY, 1);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom1c", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        attempts = 1000;
//                    }
//                }
//                else if (roomID == 2)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY) || FindMarker(cellX, cellY + 1))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenLeft(cellX, cellY) || !OpenRight(cellX, cellY) || !OpenRight(cellX, cellY + 1))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX - 1, cellY + 1) && OpenLeft(cellX, cellY + 1) || FindMarker(cellX, cellY + 2) && OpenBottom(cellX, cellY + 1))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);

//                        AddMarker(cellX, cellY, 1);
//                        AddMarker(cellX, cellY + 1, 3);
//                        AddMarker(cellX, cellY + 1, 4);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom1b", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        attempts = 1000;
//                    }
//                }
//                else if (roomID == 3)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY) || FindMarker(cellX + 1, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenLeft(cellX, cellY) || !OpenTop(cellX, cellY) || !OpenRight(cellX + 1, cellY) || !OpenBottom(cellX + 1, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY + 1) && OpenBottom(cellX, cellY) || FindMarker(cellX + 1, cellY - 1) && OpenTop(cellX + 1, cellY))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//                        AddMarker(cellX, cellY, 3);
//                        AddMarker(cellX + 1, cellY, 1);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom2c", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        attempts = 1000;
//                    }
//                }
//                else if (roomID == 4)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY) || FindMarker(cellX, cellY + 1))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenLeft(cellX, cellY) || !OpenRight(cellX, cellY) || !OpenRight(cellX, cellY + 1))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX - 1, cellY + 1) && OpenLeft(cellX, cellY + 1) || FindMarker(cellX, cellY + 2) && OpenBottom(cellX, cellY + 1))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);

//                        AddMarker(cellX, cellY, 1);
//                        AddMarker(cellX, cellY + 1, 3);
//                        AddMarker(cellX, cellY + 1, 4);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom2b", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        //VaultRoom(0, 15, roomWidth, 26, cellX, cellY);
//                        //VaultRoom(24, 15, roomWidth, 26, cellX, cellY + 1);

//                        //VaultRoom(16, 8, 32, 12, cellX, cellY);
//                        //VaultRoom(6, 22, 18, 26, cellX, cellY);
//                        //VaultRoom(10, 41, 26, 54, cellX, cellY);
//                        //VaultRoom(16, 22, 26, 41, cellX, cellY);

//                        //VaultRoom(20, 8, 34, 12, cellX, cellY);
//                        //VaultRoom(8, 22, 20, 26, cellX, cellY);
//                        //VaultRoom(20, 22, 30, 42, cellX, cellY);
//                        //VaultRoom(10, 42, 30, 54, cellX, cellY);

//                        attempts = 1000;
//                    }
//                }

//                roomID++;
//                if (roomID > ids)
//                {
//                    roomID = 1;
//                }
//            }
//        }

//        public void FillerRooms(int ids)
//        {
//            int roomID = 1;
//            int attempts = 1000;
//            while (attempts > 0)
//            {
//                attempts--;

//                if (roomID == 1)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenRight(cellX, cellY) || !OpenBottom(cellX, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX - 1, cellY) && OpenLeft(cellX, cellY))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY);

//                        AddMarker(cellX, cellY, 1);
//                        AddMarker(cellX, cellY, 4);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom1a", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        attempts = 1000;
//                    }
//                }
//                else if (roomID == 3)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft + 1, roomsRight + 1);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenLeft(cellX, cellY) || !OpenBottom(cellX, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX + 1, cellY) && OpenRight(cellX, cellY))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY);

//                        AddMarker(cellX, cellY, 1);
//                        AddMarker(cellX, cellY, 2);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom2a", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        attempts = 1000;
//                    }
//                }
//                else if (roomID == 2)
//                {
//                    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                    int cellY = WorldGen.genRand.Next(0, roomsVertical);

//                    bool valid = true;
//                    if (FindMarker(cellX, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (!OpenLeft(cellX, cellY) || !OpenTop(cellX, cellY))
//                    {
//                        valid = false;
//                    }
//                    else if (FindMarker(cellX, cellY + 1) && OpenBottom(cellX, cellY) || FindMarker(cellX + 1, cellY) && OpenRight(cellX, cellY))
//                    {
//                        valid = false;
//                    }

//                    if (valid)
//                    {
//                        AddMarker(cellX, cellY);

//                        AddMarker(cellX, cellY, 2);
//                        AddMarker(cellX, cellY, 3);

//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        Generator.GenerateStructure("Structures/vaultroom3a", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                        attempts = 1000;
//                    }
//                }

//                roomID++;
//                if (roomID > ids)
//                {
//                    roomID = 1;
//                }
//            }
//        }

//        public void SpecialRooms()
//        {
//            //int roomCount = roomsVertical * roomsHorizontal / 12;

//            //while (roomCount > 0)
//            //{
//            //    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//            //    int cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//            //    bool valid = true;
//            //    if (FindMarker(cellX, cellY) || FindMarker(cellX + 1, cellY))
//            //    {
//            //        valid = false;
//            //    }
//            //    if (!OpenLeft(cellX, cellY) || !OpenRight(cellX + 1, cellY))
//            //    {
//            //        valid = false;
//            //    }

//            //    if (valid)
//            //    {
//            //        AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//            //        AddMarker(cellX, cellY, 1);
//            //        AddMarker(cellX, cellY, 3);
//            //        AddMarker(cellX + 1, cellY, 1);
//            //        AddMarker(cellX + 1, cellY, 3);

//            //        VaultRoom(1, 15, roomWidth * 2, 26, cellX, cellY);
//            //        VaultRoom(5, 5, roomWidth * 2 - 4, roomHeight - 4, cellX, cellY);

//            //        roomCount--;
//            //    }
//            //}
//        }
//        #endregion

//        #region extras
//        public void Caves()
//        {
//            for (int cellY = 0; cellY < roomsVertical; cellY++)
//            {
//                for (int cellX = roomsLeft; cellX <= roomsRight; cellX++)
//                {
//                    if (!FindMarker(cellX, cellY))
//                    {
//                        //DungeonRoom(1, 1, roomWidth, roomHeight, -1, cellX, cellY);
//                        DungeonRoom(1, 1, roomWidth, roomHeight, (ushort)ModContent.TileType<devtile>(), cellX, cellY);
//                    }
//                }
//            }
//            FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(-2147483648, 2147483647));
//            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
//            noise.SetFrequency(0.1f);
//            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
//            //noise.SetFractalOctaves(3);

//            for (int x = location.Left + 2; x <= location.Right - 1; x++)
//            {
//                for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//                {
//                    if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<devtile>())
//                    {
//                        WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<hardstone>();

//                        if (noise.GetNoise(x, y) < -0.7f)
//                        {
//                            Framing.GetTileSafely(x, y).HasTile = false;
//                        }
//                    }
//                }
//            }
//        }

//        public void Roughness()
//        {
//            for (int i = 0; i < 3; i++)
//            {
//                for (int x = location.Left + 2; x <= location.Right - 1; x++)
//                {
//                    for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//                    {
//                        if (WorldGen.SolidTile(x, y) && WorldGen.genRand.NextBool(3))
//                        {
//                            if (!Framing.GetTileSafely(x + 1, y).HasTile || !Framing.GetTileSafely(x, y + 1).HasTile || !Framing.GetTileSafely(x - 1, y).HasTile || !Framing.GetTileSafely(x, y - 1).HasTile)
//                            {
//                                if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<hardstone>())
//                                {
//                                    Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<devtile2>();
//                                }
//                            }
//                        }
//                    }
//                }
//                for (int x = location.Left + 2; x <= location.Right - 1; x++)
//                {
//                    for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//                    {
//                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<devtile2>())
//                        {
//                            WorldGen.KillTile(x, y);
//                        }
//                    }
//                }
//            }

//            for (int x = location.Left + 2; x <= location.Right - 1; x++)
//            {
//                for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//                {
//                    if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<hardstone>() && WGTools.AdjacentTiles(x, y) <= 1)
//                    {
//                        WorldGen.KillTile(x, y);
//                    }
//                }
//            }
//        }

//        public void Ores()
//        {
//            FastNoiseLite ores = new FastNoiseLite();
//            ores.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//            ores.SetFrequency(0.04f);
//            ores.SetFractalType(FastNoiseLite.FractalType.FBm);
//            ores.SetFractalOctaves(3);

//            for (int x = location.Left + 1; x <= location.Right - 1; x++)
//            {
//                for (int y = location.Top + 1; y <= location.Bottom - 1; y++)
//                {
//                    if (WGTools.GetTile(x, y).TileType == ModContent.TileType<hardstone>())
//                    {
//                        if (ores.GetNoise(x, y) > -0.4f && ores.GetNoise(x, y) < -0.25f)
//                        {
//                            WGTools.GetTile(x, y).TileType = TileID.Adamantite;
//                        }
//                        else if (ores.GetNoise(x, y) > 0.25f && ores.GetNoise(x, y) < 0.4f)
//                        {
//                            WGTools.GetTile(x, y).TileType = TileID.Titanium;
//                        }
//                    }
//                }
//            }
//        }

//        public void RandomBricks()
//        {
//            int structureCount = roomsVertical * roomsHorizontal * 8;

//            while (structureCount > 0)
//            {
//                int structureX = WorldGen.genRand.Next(location.Left, location.Right);
//                int structureY = WorldGen.genRand.Next(location.Top, location.Bottom);
//                int radius = 5;

//                for (int y = structureY - radius; y <= structureY + radius; y++)
//                {
//                    for (int x = structureX - radius; x <= structureX + radius; x++)
//                    {
//                        if (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) < WorldGen.genRand.Next(0, radius) && WorldGen.genRand.NextBool(2))
//                        {
//                            if (Framing.GetTileSafely(x, y).HasTile && Framing.GetTileSafely(x, y).TileType == ModContent.TileType<hardstone>())
//                            {
//                                Framing.GetTileSafely(x, y).TileType = (ushort)ModContent.TileType<VaultPlating>();
//                            }
//                        }
//                        if (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) < WorldGen.genRand.Next(0, radius) && WorldGen.genRand.NextBool(2))
//                        {
//                            if (Framing.GetTileSafely(x, y).WallType == ModContent.WallType<hardstonewallvault>())
//                            {
//                                Framing.GetTileSafely(x, y).WallType = (ushort)ModContent.WallType<vaultwallunsafe>();
//                            }
//                        }
//                    }
//                }

//                structureCount--;
//            }
//        }

//        public void AshPiles()
//        {
//            int structureCount = roomsVertical * roomsHorizontal * 5;

//            while (structureCount > 0)
//            {
//                int structureX = WorldGen.genRand.Next(location.Left + 21, location.Right - 20);
//                int structureY = WorldGen.genRand.Next(location.Top + 21, location.Bottom - 20);

//                if (WGTools.GetTile(structureX, structureY + 1).HasTile && Main.tileSolid[WGTools.GetTile(structureX, structureY + 1).TileType] && WGTools.GetTile(structureX, structureY + 1).TileType != ModContent.TileType<vaultpipe>())
//                {
//                    WorldGen.TileRunner(structureX, structureY, WorldGen.genRand.Next(3, 13) * 2, 1, TileID.Ash, addTile: true, overRide: false);

//                    structureCount--;
//                }
//            }

//            for (int y = location.Bottom; y >= location.Top; y--)
//            {
//                for (int x = location.Left; x <= location.Right; x++)
//                {
//                    if (WGTools.GetTile(x, y).HasTile && WGTools.GetTile(x, y).TileType == TileID.Ash)
//                    {
//                        bool killTile = false;
//                        for (int i = -1; i <= 1; i++)
//                        {
//                            if (!WGTools.GetTile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.GetTile(x + i, y + 1).TileType] || WGTools.GetTile(x + i, y + 1).TileType == ModContent.TileType<vaultpipe>())
//                            {
//                                killTile = true;
//                                break;
//                            }
//                        }
//                        if (killTile)
//                        {
//                            WorldGen.KillTile(x, y);
//                        }
//                    }
//                }
//            }

//            for (int x = location.Left + 1; x <= location.Right - 1; x++)
//            {
//                for (int y = location.Top + 1; y <= location.Bottom - 1; y++)
//                {
//                    if (WGTools.GetTile(x, y).TileType == TileID.Ash)
//                    {
//                        if (!WGTools.GetTile(x, y - 1).HasTile || WGTools.GetTile(x, y - 1).TileType != TileID.Ash)
//                        {
//                            bool left = !WGTools.GetTile(x - 1, y).HasTile || WGTools.GetTile(x - 1, y).TileType == ModContent.TileType<vaultpipe>();
//                            bool right = !WGTools.GetTile(x + 1, y).HasTile || WGTools.GetTile(x + 1, y).TileType == ModContent.TileType<vaultpipe>();

//                            if (left && right)
//                            {
//                                WGTools.GetTile(x, y).IsHalfBlock = true;
//                            }
//                            else if (left)
//                            {
//                                WGTools.GetTile(x, y).Slope = SlopeType.SlopeDownRight;
//                            }
//                            else if (right)
//                            {
//                                WGTools.GetTile(x, y).Slope = SlopeType.SlopeDownLeft;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        #endregion

//        #region functions
//        public void DungeonRoom(int left, int top, int right, int bottom, int cellX, int cellY, int tile = -2, int wall = -2, bool add = true, bool replace = true, int style = 0, int liquid = -1, int liquidType = -1)
//        {
//            WGTools.DrawRectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top - roomHeight / 2, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom - roomHeight / 2, tile, wall, add, replace, style, liquid, liquidType);
//        }

//        public void AddMarker(int cellX, int cellY, int xOffset = 0)
//        {
//            //layout[cellX - roomsLeft, cellY, xOffset] = 1;
//            int markerX = X + cellX * roomWidth - roomWidth / 2;
//            int markerY = cellY + 1;
//            Framing.GetTileSafely(markerX + xOffset, markerY).WallType = (ushort)ModContent.WallType<devwall>();
//        }
//        public bool FindMarker(int cellX, int cellY, int xOffset = 0)
//        {
//            int markerX = X + cellX * roomWidth - roomWidth / 2;
//            int markerY = cellY + 1;
//            return Framing.GetTileSafely(markerX + xOffset, markerY).WallType == ModContent.WallType<devwall>();
//            //return layout[cellX - roomsLeft, cellY, xOffset] == 1;
//        }

//        public bool OpenLeft(int cellX, int cellY)
//        {
//            return !FindMarker(cellX - 1, cellY, 2);
//        }
//        public bool OpenRight(int cellX, int cellY)
//        {
//            return !FindMarker(cellX + 1, cellY, 4);
//        }
//        public bool OpenTop(int cellX, int cellY)
//        {
//            return !FindMarker(cellX, cellY - 1, 3);
//        }
//        public bool OpenBottom(int cellX, int cellY)
//        {
//            return !FindMarker(cellX, cellY + 1, 1);
//        }

//        private void DungeonObject(int x, int y, int tile, int objstyle, int cellX, int cellY)
//        {
//            var replaceX = X + cellX * roomWidth + x - roomWidth / 2;
//            var replaceY = Y + cellY * roomHeight + y - roomHeight / 2;

//            WorldGen.PlaceObject(replaceX, replaceY, tile, style: objstyle);
//        }

//        private void DungeonChest(int x, int y, int tile, int objstyle, int cellX, int cellY)
//        {
//            var replaceX = X + cellX * roomWidth + x - roomWidth / 2;
//            var replaceY = Y + cellY * roomHeight + y - roomHeight / 2;

//            WorldGen.PlaceChest(replaceX, replaceY, (ushort)tile, style: objstyle);
//        }

//        private void DungeonObjectFraming(int x, int y, int frameX, int frameY, int cellX, int cellY)
//        {
//            var replaceX = X + cellX * roomWidth + x - roomWidth / 2;
//            var replaceY = Y + cellY * roomHeight + y - roomHeight / 2;

//            if (frameX != -1)
//            {
//                Framing.GetTileSafely(replaceX, replaceY).TileFrameX = (short)frameX;
//            }
//            if (frameY != -1)
//            {
//                Framing.GetTileSafely(replaceX, replaceY).TileFrameY = (short)frameY;
//            }
//        }
//        #endregion
//    }

//    #region vaultgenpass
//    //public class TheVault : GenPass
//    //{
//    //    public TheVault(string name, float loadWeight) : base(name, loadWeight)
//    //    {
//    //    }

//    //    public static int X;
//    //    public static int Y;
//    //    public Rectangle location;

//    //    public int[,,] layout;

//    //    public int roomWidth = 40;
//    //    public int roomHeight = 40;

//    //    public int roomsVertical;
//    //    public int roomsHorizontal;
//    //    public int roomsLeft => 0 - (roomsHorizontal - 1) / 2;
//    //    public int roomsRight => (roomsHorizontal - 1) / 2;

//    //    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//    //    {
//    //        X = WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width / 2;
//    //        Y = WorldGen.UndergroundDesertLocation.Y + WorldGen.UndergroundDesertLocation.Height + 50;

//    //        roomsVertical = 12;
//    //        roomsHorizontal = 8 + 1;

//    //        location = new Rectangle(X - (roomWidth * roomsHorizontal) / 2 - 20, Y - roomHeight / 2 - 20, (roomWidth * roomsHorizontal) + 40, (roomHeight * roomsVertical) + 40);
//    //        WorldGen.structures.AddProtectedStructure(location);

//    //        WGTools.DrawRectangle(location.Left, location.Top, location.Right, location.Bottom, ModContent.TileType<hardstone>(), true, true, false, liquid: 0);
//    //        WGTools.DrawRectangle(location.Left + 1, location.Top + 1, location.Right - 1, location.Bottom - 1, ModContent.WallType<vault>(), true, true, true);

//    //        //SpecialRooms();
//    //        //StandardRooms(4);
//    //        //FillerRooms(3);
//    //        //Caves();

//    //        //Roughness();

//    //        //#region cleanup
//    //        //for (int x = location.Left + 1; x <= location.Right - 1; x++)
//    //        //{
//    //        //    for (int y = location.Top + 1; y <= location.Bottom - 1; y++)
//    //        //    {
//    //        //        if (WGTools.GetTile(x, y).type == ModContent.TileType<vaultbrick>() || WGTools.GetTile(x, y).type == ModContent.TileType<VaultPlating>())
//    //        //        {
//    //        //            WGTools.GetTile(x, y).wall = (ushort)ModContent.WallType<vaultwallunsafe>();
//    //        //            if (WGTools.GetTile(x, y).type == ModContent.TileType<vaultbrick>())
//    //        //            {
//    //        //                WGTools.GetTile(x, y).type = (ushort)ModContent.TileType<VaultPlating>();
//    //        //            }
//    //        //        }
//    //        //        else if (WGTools.GetTile(x, y).type == ModContent.TileType<hardstone>() && WGTools.SurroundingTilesActive(x, y, true))
//    //        //        {
//    //        //            WGTools.GetTile(x, y).wall = (ushort)ModContent.WallType<hardstonewallvault>();
//    //        //        }
//    //        //        else if (WGTools.GetTile(x, y).wall == ModContent.WallType<hardstonewallvault>())
//    //        //        {
//    //        //            WGTools.GetTile(x, y).wall = (ushort)ModContent.WallType<vault>();
//    //        //        }
//    //        //    }
//    //        //}
//    //        //#endregion

//    //        //AshPiles();
//    //        //Ores();
//    //        //RandomBricks();
//    //    }

//    //    #region rooms
//    //    public void StandardRooms(int ids)
//    //    {
//    //        int roomID = 1;
//    //        int attempts = 1000;
//    //        while (attempts > 0)
//    //        {
//    //            attempts--;

//    //            if (roomID == 1)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY) || FindMarker(cellX + 1, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenLeft(cellX, cellY) || !OpenTop(cellX, cellY) || !OpenRight(cellX + 1, cellY) || !OpenBottom(cellX + 1, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY + 1) && OpenBottom(cellX, cellY) || FindMarker(cellX + 1, cellY - 1) && OpenTop(cellX + 1, cellY))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//    //                    AddMarker(cellX, cellY, 3);
//    //                    AddMarker(cellX + 1, cellY, 1);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom1c", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    attempts = 1000;
//    //                }
//    //            }
//    //            else if (roomID == 2)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY) || FindMarker(cellX, cellY + 1))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenLeft(cellX, cellY) || !OpenRight(cellX, cellY) || !OpenRight(cellX, cellY + 1))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX - 1, cellY + 1) && OpenLeft(cellX, cellY + 1) || FindMarker(cellX, cellY + 2) && OpenBottom(cellX, cellY + 1))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);

//    //                    AddMarker(cellX, cellY, 1);
//    //                    AddMarker(cellX, cellY + 1, 3);
//    //                    AddMarker(cellX, cellY + 1, 4);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom1b", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    attempts = 1000;
//    //                }
//    //            }
//    //            else if (roomID == 3)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY) || FindMarker(cellX + 1, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenLeft(cellX, cellY) || !OpenTop(cellX, cellY) || !OpenRight(cellX + 1, cellY) || !OpenBottom(cellX + 1, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY + 1) && OpenBottom(cellX, cellY) || FindMarker(cellX + 1, cellY - 1) && OpenTop(cellX + 1, cellY))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//    //                    AddMarker(cellX, cellY, 3);
//    //                    AddMarker(cellX + 1, cellY, 1);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom2c", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    attempts = 1000;
//    //                }
//    //            }
//    //            else if (roomID == 4)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY) || FindMarker(cellX, cellY + 1))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenLeft(cellX, cellY) || !OpenRight(cellX, cellY) || !OpenRight(cellX, cellY + 1))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX - 1, cellY + 1) && OpenLeft(cellX, cellY + 1) || FindMarker(cellX, cellY + 2) && OpenBottom(cellX, cellY + 1))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);

//    //                    AddMarker(cellX, cellY, 1);
//    //                    AddMarker(cellX, cellY + 1, 3);
//    //                    AddMarker(cellX, cellY + 1, 4);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom2b", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    //VaultRoom(0, 15, roomWidth, 26, cellX, cellY);
//    //                    //VaultRoom(24, 15, roomWidth, 26, cellX, cellY + 1);

//    //                    //VaultRoom(16, 8, 32, 12, cellX, cellY);
//    //                    //VaultRoom(6, 22, 18, 26, cellX, cellY);
//    //                    //VaultRoom(10, 41, 26, 54, cellX, cellY);
//    //                    //VaultRoom(16, 22, 26, 41, cellX, cellY);

//    //                    //VaultRoom(20, 8, 34, 12, cellX, cellY);
//    //                    //VaultRoom(8, 22, 20, 26, cellX, cellY);
//    //                    //VaultRoom(20, 22, 30, 42, cellX, cellY);
//    //                    //VaultRoom(10, 42, 30, 54, cellX, cellY);

//    //                    attempts = 1000;
//    //                }
//    //            }

//    //            roomID++;
//    //            if (roomID > ids)
//    //            {
//    //                roomID = 1;
//    //            }
//    //        }
//    //    }

//    //    public void FillerRooms(int ids)
//    //    {
//    //        int roomID = 1;
//    //        int attempts = 1000;
//    //        while (attempts > 0)
//    //        {
//    //            attempts--;

//    //            if (roomID == 1)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenRight(cellX, cellY) || !OpenBottom(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX - 1, cellY) && OpenLeft(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY);

//    //                    AddMarker(cellX, cellY, 1);
//    //                    AddMarker(cellX, cellY, 4);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom1a", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    attempts = 1000;
//    //                }
//    //            }
//    //            else if (roomID == 3)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft + 1, roomsRight + 1);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenLeft(cellX, cellY) || !OpenBottom(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY - 1) && OpenTop(cellX, cellY) || FindMarker(cellX + 1, cellY) && OpenRight(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY);

//    //                    AddMarker(cellX, cellY, 1);
//    //                    AddMarker(cellX, cellY, 2);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom2a", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    attempts = 1000;
//    //                }
//    //            }
//    //            else if (roomID == 2)
//    //            {
//    //                int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//    //                int cellY = WorldGen.genRand.Next(0, roomsVertical);

//    //                bool valid = true;
//    //                if (FindMarker(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (!OpenLeft(cellX, cellY) || !OpenTop(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }
//    //                else if (FindMarker(cellX, cellY + 1) && OpenBottom(cellX, cellY) || FindMarker(cellX + 1, cellY) && OpenRight(cellX, cellY))
//    //                {
//    //                    valid = false;
//    //                }

//    //                if (valid)
//    //                {
//    //                    AddMarker(cellX, cellY);

//    //                    AddMarker(cellX, cellY, 2);
//    //                    AddMarker(cellX, cellY, 3);

//    //                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//    //                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//    //                    Generator.GenerateStructure("Structures/vaultroom3a", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//    //                    attempts = 1000;
//    //                }
//    //            }

//    //            roomID++;
//    //            if (roomID > ids)
//    //            {
//    //                roomID = 1;
//    //            }
//    //        }
//    //    }

//    //    public void SpecialRooms()
//    //    {
//    //        //int roomCount = roomsVertical * roomsHorizontal / 12;

//    //        //while (roomCount > 0)
//    //        //{
//    //        //    int cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//    //        //    int cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//    //        //    bool valid = true;
//    //        //    if (FindMarker(cellX, cellY) || FindMarker(cellX + 1, cellY))
//    //        //    {
//    //        //        valid = false;
//    //        //    }
//    //        //    if (!OpenLeft(cellX, cellY) || !OpenRight(cellX + 1, cellY))
//    //        //    {
//    //        //        valid = false;
//    //        //    }

//    //        //    if (valid)
//    //        //    {
//    //        //        AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//    //        //        AddMarker(cellX, cellY, 1);
//    //        //        AddMarker(cellX, cellY, 3);
//    //        //        AddMarker(cellX + 1, cellY, 1);
//    //        //        AddMarker(cellX + 1, cellY, 3);

//    //        //        VaultRoom(1, 15, roomWidth * 2, 26, cellX, cellY);
//    //        //        VaultRoom(5, 5, roomWidth * 2 - 4, roomHeight - 4, cellX, cellY);

//    //        //        roomCount--;
//    //        //    }
//    //        //}
//    //    }
//    //    #endregion

//    //    #region extras
//    //    public void Caves()
//    //    {
//    //        for (int cellY = 0; cellY < roomsVertical; cellY++)
//    //        {
//    //            for (int cellX = roomsLeft; cellX <= roomsRight; cellX++)
//    //            {
//    //                if (!FindMarker(cellX, cellY))
//    //                {
//    //                    //DungeonRoom(1, 1, roomWidth, roomHeight, -1, cellX, cellY);
//    //                    DungeonRoom(1, 1, roomWidth, roomHeight, (ushort)ModContent.TileType<devtile>(), cellX, cellY);
//    //                }
//    //            }
//    //        }
//    //        FastNoiseLite noise = new FastNoiseLite(WorldGen.genRand.Next(-2147483648, 2147483647));
//    //        noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
//    //        noise.SetFrequency(0.1f);
//    //        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
//    //        //noise.SetFractalOctaves(3);

//    //        for (int x = location.Left + 2; x <= location.Right - 1; x++)
//    //        {
//    //            for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//    //            {
//    //                if (Framing.GetTileSafely(x, y).type == ModContent.TileType<devtile>())
//    //                {
//    //                    WGTools.GetTile(x, y).type = (ushort)ModContent.TileType<hardstone>();

//    //                    if (noise.GetNoise(x, y) < -0.7f)
//    //                    {
//    //                        Framing.GetTileSafely(x, y).active(false);
//    //                    }
//    //                }
//    //            }
//    //        }
//    //    }

//    //    public void Roughness()
//    //    {
//    //        for (int i = 0; i < 3; i++)
//    //        {
//    //            for (int x = location.Left + 2; x <= location.Right - 1; x++)
//    //            {
//    //                for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//    //                {
//    //                    if (WorldGen.SolidTile(x, y) && WorldGen.genRand.NextBool(3)
//    //                    {
//    //                        if (!Framing.GetTileSafely(x + 1, y).active() || !Framing.GetTileSafely(x, y + 1).active() || !Framing.GetTileSafely(x - 1, y).active() || !Framing.GetTileSafely(x, y - 1).active())
//    //                        {
//    //                            if (Framing.GetTileSafely(x, y).type == ModContent.TileType<hardstone>())
//    //                            {
//    //                                Framing.GetTileSafely(x, y).type = (ushort)ModContent.TileType<devtile2>();
//    //                            }
//    //                        }
//    //                    }
//    //                }
//    //            }
//    //            for (int x = location.Left + 2; x <= location.Right - 1; x++)
//    //            {
//    //                for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//    //                {
//    //                    if (Framing.GetTileSafely(x, y).type == ModContent.TileType<devtile2>())
//    //                    {
//    //                        WorldGen.KillTile(x, y);
//    //                    }
//    //                }
//    //            }
//    //        }

//    //        for (int x = location.Left + 2; x <= location.Right - 1; x++)
//    //        {
//    //            for (int y = location.Top + 2; y <= location.Bottom - 1; y++)
//    //            {
//    //                if (Framing.GetTileSafely(x, y).type == ModContent.TileType<hardstone>() && WGTools.AdjacentTiles(x, y) <= 1)
//    //                {
//    //                    WorldGen.KillTile(x, y);
//    //                }
//    //            }
//    //        }
//    //    }

//    //    public void Ores()
//    //    {
//    //        FastNoiseLite ores = new FastNoiseLite();
//    //        ores.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//    //        ores.SetFrequency(0.04f);
//    //        ores.SetFractalType(FastNoiseLite.FractalType.FBm);
//    //        ores.SetFractalOctaves(3);

//    //        for (int x = location.Left + 1; x <= location.Right - 1; x++)
//    //        {
//    //            for (int y = location.Top + 1; y <= location.Bottom - 1; y++)
//    //            {
//    //                if (WGTools.GetTile(x, y).type == ModContent.TileType<hardstone>())
//    //                {
//    //                    if (ores.GetNoise(x, y) > -0.4f && ores.GetNoise(x, y) < -0.25f)
//    //                    {
//    //                        WGTools.GetTile(x, y).type = TileID.Adamantite;
//    //                    }
//    //                    else if (ores.GetNoise(x, y) > 0.25f && ores.GetNoise(x, y) < 0.4f)
//    //                    {
//    //                        WGTools.GetTile(x, y).type = TileID.Titanium;
//    //                    }
//    //                }
//    //            }
//    //        }
//    //    }

//    //    public void RandomBricks()
//    //    {
//    //        int structureCount = roomsVertical * roomsHorizontal * 8;

//    //        while (structureCount > 0)
//    //        {
//    //            int structureX = WorldGen.genRand.Next(location.Left, location.Right);
//    //            int structureY = WorldGen.genRand.Next(location.Top, location.Bottom);
//    //            int radius = 5;

//    //            for (int y = structureY - radius; y <= structureY + radius; y++)
//    //            {
//    //                for (int x = structureX - radius; x <= structureX + radius; x++)
//    //                {
//    //                    if (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) < WorldGen.genRand.Next(0, radius) && WorldGen.genRand.NextBool(2))
//    //                    {
//    //                        if (Framing.GetTileSafely(x, y).active() && Framing.GetTileSafely(x, y).type == ModContent.TileType<hardstone>())
//    //                        {
//    //                            Framing.GetTileSafely(x, y).type = (ushort)ModContent.TileType<VaultPlating>();
//    //                        }
//    //                    }
//    //                    if (Vector2.Distance(new Vector2(structureX, structureY), new Vector2(x, y)) < WorldGen.genRand.Next(0, radius) && WorldGen.genRand.NextBool(2))
//    //                    {
//    //                        if (Framing.GetTileSafely(x, y).wall == ModContent.WallType<hardstonewallvault>())
//    //                        {
//    //                            Framing.GetTileSafely(x, y).wall = (ushort)ModContent.WallType<vaultwallunsafe>();
//    //                        }
//    //                    }
//    //                }
//    //            }

//    //            structureCount--;
//    //        }
//    //    }

//    //    public void AshPiles()
//    //    {
//    //        int structureCount = roomsVertical * roomsHorizontal * 5;

//    //        while (structureCount > 0)
//    //        {
//    //            int structureX = WorldGen.genRand.Next(location.Left + 21, location.Right - 20);
//    //            int structureY = WorldGen.genRand.Next(location.Top + 21, location.Bottom - 20);

//    //            if (WGTools.GetTile(structureX, structureY + 1).active() && Main.tileSolid[WGTools.GetTile(structureX, structureY + 1).type] && WGTools.GetTile(structureX, structureY + 1).type != ModContent.TileType<vaultpipe>())
//    //            {
//    //                WorldGen.TileRunner(structureX, structureY, WorldGen.genRand.Next(3, 13) * 2, 1, TileID.Ash, addTile: true, overRide: false);

//    //                structureCount--;
//    //            }
//    //        }

//    //        for (int y = location.Bottom; y >= location.Top; y--)
//    //        {
//    //            for (int x = location.Left; x <= location.Right; x++)
//    //            {
//    //                if (WGTools.GetTile(x, y).active() && WGTools.GetTile(x, y).type == TileID.Ash)
//    //                {
//    //                    bool killTile = false;
//    //                    for (int i = -1; i <= 1; i++)
//    //                    {
//    //                        if (!WGTools.GetTile(x + i, y + 1).active() || !Main.tileSolid[WGTools.GetTile(x + i, y + 1).type] || WGTools.GetTile(x + i, y + 1).type == ModContent.TileType<vaultpipe>())
//    //                        {
//    //                            killTile = true;
//    //                            break;
//    //                        }
//    //                    }
//    //                    if (killTile)
//    //                    {
//    //                        WorldGen.KillTile(x, y);
//    //                    }
//    //                }
//    //            }
//    //        }

//    //        for (int x = location.Left + 1; x <= location.Right - 1; x++)
//    //        {
//    //            for (int y = location.Top + 1; y <= location.Bottom - 1; y++)
//    //            {
//    //                if (WGTools.GetTile(x, y).type == TileID.Ash)
//    //                {
//    //                    if (!WGTools.GetTile(x, y - 1).active() || WGTools.GetTile(x, y - 1).type != TileID.Ash)
//    //                    {
//    //                        bool left = !WGTools.GetTile(x - 1, y).active() || WGTools.GetTile(x - 1, y).type == ModContent.TileType<vaultpipe>();
//    //                        bool right = !WGTools.GetTile(x + 1, y).active() || WGTools.GetTile(x + 1, y).type == ModContent.TileType<vaultpipe>();

//    //                        if (left && right)
//    //                        {
//    //                            WGTools.GetTile(x, y).halfBrick(true);
//    //                        }
//    //                        else if (left)
//    //                        {
//    //                            WGTools.GetTile(x, y).slope(2);
//    //                        }
//    //                        else if (right)
//    //                        {
//    //                            WGTools.GetTile(x, y).slope(1);
//    //                        }
//    //                    }
//    //                }
//    //            }
//    //        }
//    //    }
//    //    #endregion

//    //    #region functions
//    //    public void DungeonRoom(int left, int top, int right, int bottom, int tile, int cellX, int cellY, bool add = true, bool replace = true, bool wall = false, int style = 0, int liquid = -1, int liquidType = -1)
//    //    {
//    //        WGTools.DrawRectangle(X + cellX * roomWidth + left - roomWidth / 2, Y + cellY * roomHeight + top - roomHeight / 2, X + cellX * roomWidth + right - roomWidth / 2, Y + cellY * roomHeight + bottom - roomHeight / 2, tile, add, replace, wall, style, liquid, liquidType);
//    //    }

//    //    public void AddMarker(int cellX, int cellY, int xOffset = 0)
//    //    {
//    //        //layout[cellX - roomsLeft, cellY, xOffset] = 1;
//    //        int markerX = X + cellX * roomWidth - roomWidth / 2;
//    //        int markerY = cellY + 1;
//    //        Framing.GetTileSafely(markerX + xOffset, markerY).wall = (ushort)ModContent.WallType<devwall>();
//    //    }
//    //    public bool FindMarker(int cellX, int cellY, int xOffset = 0)
//    //    {
//    //        int markerX = X + cellX * roomWidth - roomWidth / 2;
//    //        int markerY = cellY + 1;
//    //        return Framing.GetTileSafely(markerX + xOffset, markerY).wall == ModContent.WallType<devwall>();
//    //        //return layout[cellX - roomsLeft, cellY, xOffset] == 1;
//    //    }

//    //    public bool OpenLeft(int cellX, int cellY)
//    //    {
//    //        return !FindMarker(cellX - 1, cellY, 2);
//    //    }
//    //    public bool OpenRight(int cellX, int cellY)
//    //    {
//    //        return !FindMarker(cellX + 1, cellY, 4);
//    //    }
//    //    public bool OpenTop(int cellX, int cellY)
//    //    {
//    //        return !FindMarker(cellX, cellY - 1, 3);
//    //    }
//    //    public bool OpenBottom(int cellX, int cellY)
//    //    {
//    //        return !FindMarker(cellX, cellY + 1, 1);
//    //    }

//    //    private void DungeonObject(int x, int y, int tile, int objstyle, int cellX, int cellY)
//    //    {
//    //        var replaceX = X + cellX * roomWidth + x - roomWidth / 2;
//    //        var replaceY = Y + cellY * roomHeight + y - roomHeight / 2;

//    //        WorldGen.PlaceObject(replaceX, replaceY, tile, style: objstyle);
//    //    }

//    //    private void DungeonChest(int x, int y, int tile, int objstyle, int cellX, int cellY)
//    //    {
//    //        var replaceX = X + cellX * roomWidth + x - roomWidth / 2;
//    //        var replaceY = Y + cellY * roomHeight + y - roomHeight / 2;

//    //        WorldGen.PlaceChest(replaceX, replaceY, (ushort)tile, style: objstyle);
//    //    }

//    //    private void DungeonObjectFraming(int x, int y, int frameX, int frameY, int cellX, int cellY)
//    //    {
//    //        var replaceX = X + cellX * roomWidth + x - roomWidth / 2;
//    //        var replaceY = Y + cellY * roomHeight + y - roomHeight / 2;

//    //        if (frameX != -1)
//    //        {
//    //            Framing.GetTileSafely(replaceX, replaceY).frameX = (short)frameX;
//    //        }
//    //        if (frameY != -1)
//    //        {
//    //            Framing.GetTileSafely(replaceX, replaceY).frameY = (short)frameY;
//    //        }
//    //    }
//    //    #endregion
//    //}
//    #endregion

//    public class MansionSubworld : Subworld
//    {
//        public override int Width => 800;
//        public override int Height => 600;

//        public override bool ShouldSave => false;

//        public override bool NormalUpdates => false;

//        public override List<GenPass> Tasks => new List<GenPass>()
//        {
//            //new SubworldGenPass(progress => WorldSetup(progress)),
//            //new SubworldGenPass(progress => Environment(progress)),
//            //new SubworldGenPass(progress => Mansion(progress)),
//        };

//        private void WorldSetup(GenerationProgress progress)
//        {
//            progress.Message = "Setting things up";

//            Main.worldSurface = Main.maxTilesY - 300;
//            Main.rockLayer = Main.maxTilesY;

//            Main.spawnTileX = Main.maxTilesX / 2;
//            Main.spawnTileY = 50;

//            Main.dayTime = false;
//            Main.time = 14000;

//            Main.hardMode = true;
//            Main.halloween = true;
//        }

//        public FastNoiseLite elevation = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
//        public FastNoiseLite terrain = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));

//        private void Environment(GenerationProgress progress)
//        {
//            progress.Message = "Terraforming";

//            elevation.SetNoiseType(FastNoiseLite.NoiseType.Value);
//            elevation.SetFrequency(0.003f);
//            elevation.SetFractalOctaves(3);

//            terrain.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//            terrain.SetFrequency(0.005f);
//            terrain.SetFractalType(FastNoiseLite.FractalType.FBm);
//            terrain.SetFractalOctaves(5);

//            int terrainMin = (int)Main.worldSurface - 150;
//            int terrainMax = (int)Main.worldSurface - 30;

//            for (float y = (int)Math.Min((int)Main.worldSurface - 250, Main.worldSurface * 0.35f); y < Main.maxTilesY; y++)
//            {
//                for (float x = 20; x < Main.maxTilesX - 20; x++)
//                {
//                    progress.Set(y / (Main.maxTilesY - 200) / 2);

//                    if (y <= Main.worldSurface)
//                    {
//                        Vector2 point = new Vector2(MathHelper.Clamp(x, location.Left - 6, location.Right + 6), y);

//                        float _height = (elevation.GetNoise(x, y / 2) * 160);
//                        _height *= MathHelper.Clamp(Vector2.Distance(new Vector2(x, y), point) / 200, 0, 1);

//                        float _terrain = terrain.GetNoise(x * 2 + WorldGen.genRand.Next(-1, 2), y) / 0.9f;
//                        _terrain *= MathHelper.Clamp(Vector2.Distance(new Vector2(x, y), point) / 100, 0, 1);

//                        float threshold = MathHelper.Clamp((y + _height - terrainMin) / (terrainMax - terrainMin), 0, 1);

//                        if (_terrain <= threshold * 2 - 1)
//                        {
//                            WGTools.GetTile(x, y).HasTile = true;
//                            //WGTools.GetTile(x, y).frameX = -1;
//                            //WGTools.GetTile(x, y).frameY = -1;
//                        }
//                        else
//                        {
//                            WGTools.GetTile(x, y).HasTile = false;
//                            //WGTools.GetTile(x, y).frameX = -1;
//                            //WGTools.GetTile(x, y).frameY = -1;
//                        }
//                    }
//                    else WGTools.GetTile(x, y).HasTile = true;
//                }
//            }

//            Main.spawnTileX = Main.maxTilesX / 2;
//            if (WorldGen.genRand.NextBool(2))
//            {
//                Main.spawnTileX -= 250;
//            }
//            else Main.spawnTileX += 250;

//            Main.spawnTileY = 50;
//            while (true)
//            {       
//                if (WGTools.GetTile(Main.spawnTileX, Main.spawnTileY + 1).HasTile)
//                {
//                    break;
//                }
//                Main.spawnTileY++;
//            }

//            FastNoiseLite blending = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
//            blending.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
//            blending.SetFrequency(0.04f);
//            blending.SetFractalType(FastNoiseLite.FractalType.FBm);
//            blending.SetFractalGain(0.6f);
//            blending.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);
//            blending.SetCellularJitter(1.2f);

//            float blendLow = (int)Main.worldSurface;
//            float blendHigh = (int)Main.worldSurface + 100;

//            for (float y = 20; y < Main.maxTilesY; y++)
//            {
//                for (float x = 20; x < Main.maxTilesX - 20; x++)
//                {
//                    progress.Set(y / (Main.maxTilesY - 200) / 2 + 0.5f);

//                    float _blending = blending.GetNoise(x, y);
//                    float num = MathHelper.Clamp((y + (float)Math.Cos(x / 100) * 50 - blendLow) / (blendHigh - blendLow) * 2 - 1, -1, 1);
//                    if (_blending <= num / 10 - 0.6f)
//                    {
//                        WGTools.GetTile(x, y).TileType = TileID.Stone;
//                    }
//                    else
//                    {
//                        WGTools.GetTile(x, y).TileType = TileID.Dirt;
//                    }

//                    if (y <= Main.worldSurface && WGTools.GetTile(x, y).TileType == TileID.Dirt && (!WGTools.SurroundingTilesActive(x, y) || WorldGen.genRand.NextBool(100)))
//                    {
//                        WGTools.GetTile(x, y).TileType = TileID.Grass;
//                    }
//                    if (WGTools.SurroundingTilesActive(x, y))
//                    {
//                        WGTools.GetTile(x, y).WallType = WallID.DirtUnsafe;
//                    }

//                    if (WGTools.AdjacentTiles(x, y) == 2)
//                    {
//                        if (WorldGen.SolidTile(WGTools.GetTile(x, y + 1)))
//                        {
//                            if (WorldGen.genRand.NextBool(2) && !WorldGen.SolidTile(WGTools.GetTile(x - 1, y - 1)) && !WorldGen.SolidTile(WGTools.GetTile(x + 1, y - 1)))
//                            {
//                                WGTools.GetTile(x, y).IsHalfBlock = true;
//                            }
//                            else if (WorldGen.SolidTile(WGTools.GetTile(x - 1, y)))
//                            {
//                                WGTools.GetTile(x, y).Slope = SlopeType.SlopeDownLeft;
//                            }
//                            else if (WorldGen.SolidTile(WGTools.GetTile(x + 1, y)))
//                            {
//                                WGTools.GetTile(x, y).Slope = SlopeType.SlopeDownRight;
//                            }
//                        }
//                    }
//                    else if (WGTools.AdjacentTiles(x, y) == 1 && !WorldGen.SolidTile(WGTools.GetTile(x, y - 1)))
//                    {
//                        WGTools.GetTile(x, y).IsHalfBlock = true;
//                    }
//                }
//            }

//            #region bushes
//            for (int y = 40; y < Main.worldSurface; y++)
//            {
//                for (int x = 40; x < Main.maxTilesX - 40; x++)
//                {
//                    if (Framing.GetTileSafely(x, y).HasTile && !WGTools.SurroundingTilesActive(x, y, true) && WorldGen.genRand.NextBool(4))
//                    {
//                        Tile tile = Framing.GetTileSafely(x, y);
//                        if (tile.TileType == TileID.Grass || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.CorruptGrass || tile.TileType == TileID.CrimsonGrass)
//                        {
//                            int count = 3;
//                            while (count > 0)
//                            {
//                                int i = WorldGen.genRand.Next(x - 4, x + 5);
//                                int j = WorldGen.genRand.Next(y - 4, y + 5);
//                                if (Framing.GetTileSafely(i, j).HasTile && !WGTools.SurroundingTilesActive(i, j, true))
//                                {
//                                    tile = Framing.GetTileSafely(i, j);
//                                    if (tile.TileType == TileID.Grass || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.CorruptGrass || tile.TileType == TileID.CrimsonGrass)
//                                    {
//                                        int grass = WallID.GrassUnsafe;
//                                        if (tile.TileType == TileID.JungleGrass)
//                                        {
//                                            grass = WallID.JungleUnsafe;
//                                        }
//                                        else if (tile.TileType == TileID.CorruptGrass)
//                                        {
//                                            grass = WallID.CorruptGrassUnsafe;
//                                        }
//                                        else if (tile.TileType == TileID.CrimsonGrass)
//                                        {
//                                            grass = WallID.CrimsonGrassUnsafe;
//                                        }

//                                        WGTools.DrawCircle(i, j, WorldGen.genRand.NextFloat(0.5f, 3.5f), wall: grass, xMultiplier: 1, yMultiplier: 1, add: true, replace: false);

//                                        count--;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            #endregion
//        }

//                private void DoBlend(float x, float y, int type, int chance, int count = 1)
//        {
//            for (int i = 0; i < count; i++)
//            {
//                if (WorldGen.genRand.NextBool(chance * 10))
//                {
//                    WorldGen.TileRunner((int)x + WorldGen.genRand.Next(-20, 21), (int)y + WorldGen.genRand.Next(-20, 21), WorldGen.genRand.Next(4, 25), WorldGen.genRand.Next(12, 19), type, false, WorldGen.genRand.NextFloat(-10, 10), WorldGen.genRand.NextFloat(-10, 10));
//                }
//            }
//        }

//        #region mansion
//        public int X => Main.maxTilesX / 2;
//        public int Y => (int)Main.worldSurface - 190 - 8 + roomHeight * 2;

//        int[,,] layout;

//        int roomWidth => 8;
//        int roomHeight => 14;

//        int roomsVertical => 8;
//        int roomsHorizontal => 26;

//        int roomsLeft => 0 - (roomsHorizontal - 1) / 2;
//        int roomsRight => (roomsHorizontal - 1) / 2;

//        public Rectangle location => new Rectangle(X - (roomWidth * roomsHorizontal) / 2, Y - roomHeight / 2, (roomWidth * roomsHorizontal) + 2, (roomHeight * roomsVertical));

//        private void Mansion(GenerationProgress progress)
//        {
//            progress.Message = "Building the mansion";

//            bool devMode = false;

//            #region setup
//            layout = new int[roomsHorizontal, roomsVertical, 5];

//            int cellX;
//            int cellY;

//            for (cellY = 0; cellY < 2; cellY++)
//            {
//                for (cellX = roomsLeft + 7; cellX <= roomsRight - 7; cellX++)
//                {
//                    if (cellX < -3 || cellX > 3)
//                    {
//                        AddMarker(cellX, cellY, 1);
//                    }
//                }
//            }

//            for (cellY = 0; cellY <= roomsVertical; cellY++)
//            {
//                for (cellX = roomsLeft; cellX <= roomsRight; cellX++)
//                {
//                    if (!FindMarker(cellX, cellY))
//                    {
//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        if (!FindMarker(cellX, cellY, 1))
//                        {
//                            if (cellY == 0)
//                            {
//                                if (FindMarker(cellX - 1, cellY, 1))
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/roof-left", new Point16(posX - 1, posY - roomHeight), ModContent.GetInstance<Remnants>());
//                                }
//                                else if (FindMarker(cellX + 1, cellY, 1))
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/roof-right", new Point16(posX, posY - roomHeight), ModContent.GetInstance<Remnants>());
//                                }
//                                else Generator.GenerateStructure("Structures/mansion/roof-middle", new Point16(posX, posY - roomHeight), ModContent.GetInstance<Remnants>());
//                            }
//                        }
//                        else if (!FindMarker(cellX, cellY + 1, 1))
//                        {
//                            Generator.GenerateStructure("Structures/mansion/balcony-middle", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                        }

//                        if (!FindMarker(cellX, cellY, 1))
//                        {
//                            if (FindMarker(cellX - 1, cellY, 1))
//                            {
//                                AddMarker(cellX, cellY);
//                                AddMarker(cellX, cellY, 2);
//                                if (cellY >= roomsVertical - 2)
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/wall-left", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                                }
//                                else if (!FindMarker(cellX - 1, cellY + 1, 1) || cellY == roomsVertical - 3)
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/door-left", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                                    if (cellY == roomsVertical - 3)
//                                    {
//                                        Generator.GenerateStructure("Structures/mansion/entrance-left", new Point16(posX - 4, posY), ModContent.GetInstance<Remnants>());
//                                    }
//                                }
//                                else Generator.GenerateStructure("Structures/mansion/window-left", new Point16(posX - 1, posY), ModContent.GetInstance<Remnants>());
//                            }
//                            if (FindMarker(cellX + 1, cellY, 1))
//                            {
//                                AddMarker(cellX, cellY);
//                                AddMarker(cellX, cellY, 2);
//                                if (cellY >= roomsVertical - 2)
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/wall-right", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                                }
//                                else if (!FindMarker(cellX + 1, cellY + 1, 1) || cellY == roomsVertical - 3)
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/door-right", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                                    if (cellY == roomsVertical - 3)
//                                    {
//                                        Generator.GenerateStructure("Structures/mansion/entrance-right", new Point16(posX + roomWidth, posY), ModContent.GetInstance<Remnants>());
//                                    }
//                                }
//                                else Generator.GenerateStructure("Structures/mansion/window-right", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                            }
//                        }
//                    }
//                }
//            }

//            #endregion

//            #region rooms
//            int roomCount;

//            #region special
//            #region 2x2
//            roomCount = 2;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                cellY = WorldGen.genRand.Next(1, roomsVertical - 2);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 2, 2) || !FreeSpace(cellX + 1, cellY + 2, 1, 1))
//                {
//                    valid = false;
//                }
//                else if (FindMarker(cellX - 1, cellY) || FindMarker(cellX + 2, cellY) || FindMarker(cellX - 1, cellY + 1) || FindMarker(cellX + 2, cellY + 1))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)) || (!FindMarker(cellX - 1, cellY + 1) && !FindMarker(cellX - 2, cellY + 1) && FindMarker(cellX - 3, cellY + 1)) || (!FindMarker(cellX, cellY + 2) && !FindMarker(cellX - 1, cellY + 2) && FindMarker(cellX - 2, cellY + 2)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)) || (!FindMarker(cellX + 1, cellY + 1) && !FindMarker(cellX + 2, cellY + 1) && FindMarker(cellX + 3, cellY + 1)) || (!FindMarker(cellX + 1, cellY + 2) && !FindMarker(cellX + 2, cellY + 2) && FindMarker(cellX + 3, cellY + 2)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX + 1, cellY + 1); AddMarker(cellX + 1, cellY + 2);
//                    AddMarker(cellX + 1, cellY + 1, 2);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/c3", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    Generator.GenerateStructure("Structures/mansion/c5", new Point16(posX + roomWidth, posY + roomHeight * 2), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }

//            roomCount = 2;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                cellY = WorldGen.genRand.Next(1, roomsVertical - 2);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 2, 2) || !FreeSpace(cellX, cellY + 2, 1, 1))
//                {
//                    valid = false;
//                }
//                else if (FindMarker(cellX - 1, cellY) || FindMarker(cellX + 2, cellY) || FindMarker(cellX - 1, cellY + 1) || FindMarker(cellX + 2, cellY + 1))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)) || (!FindMarker(cellX - 1, cellY + 1) && !FindMarker(cellX - 2, cellY + 1) && FindMarker(cellX - 3, cellY + 1)) || (!FindMarker(cellX - 1, cellY + 2) && !FindMarker(cellX - 2, cellY + 2) && FindMarker(cellX - 3, cellY + 2)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)) || (!FindMarker(cellX + 1, cellY + 1) && !FindMarker(cellX + 2, cellY + 1) && FindMarker(cellX + 3, cellY + 1)) || (!FindMarker(cellX, cellY + 2) && !FindMarker(cellX + 1, cellY + 2) && FindMarker(cellX + 2, cellY + 2)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX + 1, cellY + 1); AddMarker(cellX, cellY + 2);
//                    AddMarker(cellX, cellY + 1, 2);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/c4", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    Generator.GenerateStructure("Structures/mansion/c5", new Point16(posX, posY + roomHeight * 2), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }

//            roomCount = 2;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                cellY = WorldGen.genRand.Next(1, roomsVertical - 1);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 2, 2))
//                {
//                    valid = false;
//                }
//                else if (FindMarker(cellX - 1, cellY) || FindMarker(cellX + 2, cellY) || FindMarker(cellX - 1, cellY + 1) || FindMarker(cellX + 2, cellY + 1))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX + 1, cellY + 1);
//                    AddMarker(cellX + 1, cellY + 1, 2);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/c1", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }
//            roomCount = 2;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                cellY = WorldGen.genRand.Next(1, roomsVertical - 1);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 2, 2))
//                {
//                    valid = false;
//                }
//                else if (FindMarker(cellX - 1, cellY) || FindMarker(cellX + 2, cellY) || FindMarker(cellX - 1, cellY + 1) || FindMarker(cellX + 2, cellY + 1))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY); AddMarker(cellX, cellY + 1); AddMarker(cellX + 1, cellY + 1);
//                    AddMarker(cellX, cellY + 1, 2);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/c2", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }
//            #endregion
//            #region 1x2
//            roomCount = roomsVertical + 1;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                cellY = WorldGen.genRand.Next(0, roomsVertical - 1);

//                if (roomCount < roomsVertical + 2)
//                {
//                    cellY = Math.Max(0, roomCount - 1 - 2);
//                    if (roomCount == 1)
//                    {
//                        cellX = WorldGen.genRand.Next(-2, 3);
//                    }
//                    else if (roomCount == 2)
//                    {
//                        cellX = WorldGen.genRand.Next(roomsLeft + 1, roomsLeft + 6);
//                    }
//                    else if (roomCount == 3)
//                    {
//                        cellX = WorldGen.genRand.Next(roomsRight - 6, roomsRight - 1);
//                    }
//                }

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 1, 2))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)) || (!FindMarker(cellX - 1, cellY + 1) && !FindMarker(cellX - 2, cellY + 1) && FindMarker(cellX - 3, cellY + 1)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)) || (!FindMarker(cellX + 1, cellY + 1) && !FindMarker(cellX + 2, cellY + 1) && FindMarker(cellX + 3, cellY + 1)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY); AddMarker(cellX, cellY + 1);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;

//                    if (cellY >= roomsVertical - 2)
//                    {
//                        Generator.GenerateStructure("Structures/mansion/b2", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                    }
//                    else Generator.GenerateStructure("Structures/mansion/b1", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }
//            #endregion
//            #region 2x1
//            roomCount = 4;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight);
//                cellY = WorldGen.genRand.Next(0, roomsVertical - 2);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 2, 1))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 2, cellY) && !FindMarker(cellX + 3, cellY) && FindMarker(cellX + 4, cellY)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY); AddMarker(cellX + 1, cellY);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateMultistructureRandom("Structures/mansion/kitchen", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }
//            #endregion
//            #region 1x1
//            roomCount = 16;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                cellY = WorldGen.genRand.Next(0, roomsVertical);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 1, 1))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/table", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    PlacePainting(5, 7, cellX, cellY);

//                    roomCount--;
//                }
//            }

//            roomCount = 12;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                cellY = WorldGen.genRand.Next(0, roomsVertical);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 1, 1))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/bookshelves", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }

//            //roomCount = 8;
//            //while (roomCount > 0)
//            //{
//            //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//            //    cellY = WorldGen.genRand.Next(0, roomsVertical);

//            //    bool valid = true;

//            //    if (!FreeSpace(cellX, cellY, 1, 1))
//            //    {
//            //        valid = false;
//            //    }
//            //    else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)))
//            //    {
//            //        valid = false;
//            //    }
//            //    else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)))
//            //    {
//            //        valid = false;
//            //    }

//            //    if (valid)
//            //    {
//            //        AddMarker(cellX, cellY);

//            //        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//            //        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//            //        Generator.GenerateMultistructureRandom("Structures/mansion/bed", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//            //        roomCount--;
//            //    }
//            //}

//            roomCount = 8;
//            while (roomCount > 0)
//            {
//                cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//                cellY = WorldGen.genRand.Next(0, roomsVertical - 2);

//                bool valid = true;

//                if (!FreeSpace(cellX, cellY, 1, 1))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)))
//                {
//                    valid = false;
//                }
//                else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)))
//                {
//                    valid = false;
//                }

//                if (valid)
//                {
//                    AddMarker(cellX, cellY);

//                    int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                    int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                    Generator.GenerateStructure("Structures/mansion/sofas", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//                    roomCount--;
//                }
//            }

//            //roomCount = 4;
//            //while (roomCount > 0)
//            //{
//            //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//            //    cellY = WorldGen.genRand.Next(0, roomsVertical);

//            //    bool valid = true;

//            //    if (!FreeSpace(cellX, cellY, 1, 1))
//            //    {
//            //        valid = false;
//            //    }
//            //    else if ((!FindMarker(cellX - 1, cellY) && !FindMarker(cellX - 2, cellY) && FindMarker(cellX - 3, cellY)))
//            //    {
//            //        valid = false;
//            //    }
//            //    else if ((!FindMarker(cellX + 1, cellY) && !FindMarker(cellX + 2, cellY) && FindMarker(cellX + 3, cellY)))
//            //    {
//            //        valid = false;
//            //    }

//            //    if (valid)
//            //    {
//            //        AddMarker(cellX, cellY);

//            //        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//            //        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//            //        Generator.GenerateStructure("Structures/mansion/piano", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//            //        roomCount--;
//            //    }
//            //}
//            #endregion
//            #endregion

//            #region filler
//            //roomCount = 24;
//            //while (roomCount > 0)
//            //{
//            //    cellX = WorldGen.genRand.Next(roomsLeft, roomsRight + 1);
//            //    cellY = WorldGen.genRand.Next(0, roomsVertical);

//            //    bool valid = true;

//            //    if (!FreeSpace(cellX, cellY, 1, 1))
//            //    {
//            //        valid = false;
//            //    }
//            //    if (FindMarker(cellX - 1, cellY, 2) || FindMarker(cellX + 1, cellY, 2))
//            //    {
//            //        valid = false;
//            //    }

//            //    if (valid)
//            //    {
//            //        AddMarker(cellX, cellY);
//            //        AddMarker(cellX, cellY, 2);

//            //        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//            //        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//            //        Generator.GenerateStructure("Structures/mansion/door", new Point16(posX, posY), ModContent.GetInstance<Remnants>());

//            //        roomCount--;
//            //    }
//            //}
//            for (cellY = 0; cellY < roomsVertical; cellY++)
//            {
//                for (cellX = roomsLeft; cellX <= roomsRight; cellX++)
//                {
//                    if (!FindMarker(cellX, cellY) && !FindMarker(cellX, cellY, 1))
//                    {
//                        int posX = X + cellX * roomWidth - roomWidth / 2 + 1;
//                        int posY = Y + cellY * roomHeight - roomHeight / 2 + 1;
//                        if ((FindMarker(cellX - 1, cellY) || FindMarker(cellX + 1, cellY)) && !FindMarker(cellX - 1, cellY, 2) && !FindMarker(cellX + 1, cellY, 2))
//                        {
//                            Generator.GenerateStructure("Structures/mansion/door", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                        }
//                        else
//                        {
//                            if (WorldGen.genRand.NextBool(2))
//                            {
//                                if (cellY >= roomsVertical - 2)
//                                {
//                                    Generator.GenerateStructure("Structures/mansion/a3", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                                }
//                                else Generator.GenerateStructure("Structures/mansion/a2", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                            }
//                            else Generator.GenerateStructure("Structures/mansion/a1", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                        }
//                        //if (WorldGen.genRand.NextBool(2))
//                        //{
//                        //    Generator.GenerateStructure("Structures/mansion/a1", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                        //}
//                        //else Generator.GenerateStructure("Structures/mansion/a2", new Point16(posX, posY), ModContent.GetInstance<Remnants>());
//                    }
//                }
//            }
//            #endregion

//            #endregion

//            #region objects
//            if (!devMode)
//            {
//                int objects;

//                objects = 8;
//                while (objects > 0)
//                {
//                    int x = WorldGen.genRand.Next(location.Left, location.Right);
//                    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

//                    if (Framing.GetTileSafely(x, y).TileType != TileID.Containers && WGTools.GetTile(x, y + 1).TileType == TileID.WoodBlock && WGTools.GetTile(x + 1, y + 1).TileType == TileID.WoodBlock)
//                    {
//                        int chestIndex = WorldGen.PlaceChest(x, y, style: 0);
//                        if (Framing.GetTileSafely(x, y).TileType == TileID.Containers)
//                        {
//                            ChestLoot(chestIndex, objects);

//                            objects--;
//                        }
//                    }
//                }

//                objects = roomsVertical * roomsHorizontal / 16;
//                while (objects > 0)
//                {
//                    int x = WorldGen.genRand.Next(location.Left, location.Right);
//                    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

//                    bool valid = true;
//                    if (Framing.GetTileSafely(x, y).TileType == TileID.GrandfatherClocks)
//                    {
//                        valid = false;
//                    }
//                    else for (int i = 0; i <= 1; i++)
//                        {
//                            if (WGTools.GetTile(x + i, y + 1).TileType == TileID.Platforms || WGTools.GetTile(x + i, y + 1).TileType == TileID.RedDynastyShingles || !WGTools.GetTile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.GetTile(x + i, y + 1).TileType])
//                            {
//                                valid = false;
//                                break;
//                            }
//                        }

//                    if (valid)
//                    {
//                        WorldGen.PlaceObject(x, y, TileID.GrandfatherClocks);
//                        if (Framing.GetTileSafely(x, y).TileType == TileID.GrandfatherClocks)
//                        {
//                            objects--;
//                        }
//                    }
//                }

//                objects = roomsVertical * roomsHorizontal / 4;
//                while (objects > 0)
//                {
//                    int x = WorldGen.genRand.Next(location.Left, location.Right);
//                    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

//                    bool valid = true;
//                    if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot || WGTools.GetTile(x, y).WallType == WallID.Wood)
//                    {
//                        valid = false;
//                    }
//                    else if (WGTools.GetTile(x, y + 1).TileType == TileID.Platforms && WGTools.GetTile(x, y + 1).TileFrameY != 18 * 11 || WGTools.GetTile(x, y + 1).TileType == TileID.RedDynastyShingles)
//                    {
//                        valid = false;
//                    }
//                    else if (WGTools.GetTile(x, y - 1).HasTile || WGTools.GetTile(x - 1, y).TileType == TileID.ClosedDoor || WGTools.GetTile(x + 1, y).TileType == TileID.ClosedDoor)
//                    {
//                        valid = false;
//                    }
//                    else for (int i = -1; i <= 1; i++)
//                        {
//                            if (!WGTools.GetTile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.GetTile(x + i, y + 1).TileType])
//                            {
//                                valid = false;
//                                break;
//                            }
//                        }

//                    if (valid)
//                    {
//                        WorldGen.PlaceObject(x, y, TileID.ClayPot);
//                        if (Framing.GetTileSafely(x, y).TileType == TileID.ClayPot)
//                        {
//                            WorldGen.PlaceTile(x, y - 1, TileID.MatureHerbs, style: 2);
//                            objects--;
//                        }
//                    }
//                }

//                objects = roomsVertical * roomsHorizontal / 8;
//                while (objects > 0)
//                {
//                    int x = WorldGen.genRand.Next(location.Left, location.Right);
//                    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

//                    if (Framing.GetTileSafely(x, y).TileType != TileID.LargePiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.Platforms && Framing.GetTileSafely(x, y + 1).TileType != TileID.RedDynastyShingles)
//                    {
//                        WorldGen.PlaceObject(x, y, TileID.LargePiles, style: Main.rand.Next(22, 26));
//                        if (Framing.GetTileSafely(x, y).TileType == TileID.LargePiles)
//                        {
//                            objects--;
//                        }
//                    }
//                }

//                objects = roomsVertical * roomsHorizontal / 4;
//                while (objects > 0)
//                {
//                    int x = WorldGen.genRand.Next(location.Left, location.Right);
//                    int y = WorldGen.genRand.Next(location.Top, location.Bottom);

//                    if (Framing.GetTileSafely(x, y).TileType != TileID.SmallPiles && Framing.GetTileSafely(x, y + 1).TileType != TileID.RedDynastyShingles)
//                    {
//                        WGTools.MediumPile(x, y, Main.rand.Next(31, 34));
//                        if (Framing.GetTileSafely(x, y).TileType == TileID.SmallPiles)
//                        {
//                            objects--;
//                        }
//                    }
//                }
//            }

//            #endregion

//            #region cleanup
//            if (!devMode)
//            {
//                FastNoiseLite weathering = new FastNoiseLite();
//                weathering.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//                weathering.SetFrequency(0.1f);
//                weathering.SetFractalType(FastNoiseLite.FractalType.FBm);
//                weathering.SetFractalOctaves(3);

//                for (int y = location.Top - 8; y <= location.Bottom + 9; y++)
//                {
//                    for (int x = location.Left - 8; x <= location.Right + 8; x++)
//                    {
//                        Tile tile = WGTools.GetTile(x, y);

//                        if (!WGTools.GetTile(x, y).HasTile && WGTools.GetTile(x, y).WallType != 0 && WGTools.GetTile(x, y - 1).HasTile && WGTools.AdjacentTiles(x, y, true) >= 2)
//                        {
//                            WorldGen.TileRunner(x, y, WorldGen.genRand.Next(1, 3) * 2, 1, TileID.Cobweb, true, overRide: false);
//                        }

//                        if (y >= location.Bottom - roomHeight * 2 && x > location.Left + 5 && x < location.Right - 5)
//                        {
//                            if (WGTools.GetTile(x, y).TileType == TileID.WoodBlock)
//                            {
//                                WGTools.GetTile(x, y).TileType = TileID.GrayBrick;
//                            }
//                            if (WGTools.GetTile(x, y).WallType == WallID.Wood)
//                            {
//                                WGTools.GetTile(x, y).WallType = WallID.GrayBrick;
//                            }
//                            if (WGTools.GetTile(x, y).WallType == ModContent.WallType<mansion1>() || WGTools.GetTile(x, y).WallType == ModContent.WallType<oceanruins1>())
//                            {
//                                WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<mansion4>();
//                            }

//                            if (WGTools.GetTile(x, y).TileType == TileID.Platforms && WGTools.GetTile(x, y).TileFrameY == 18 * 11)
//                            {
//                                WGTools.GetTile(x, y).TileFrameY = 18 * 9;
//                            }
//                        }
//                        else
//                        {
//                            if (WGTools.GetTile(x, y).TileType == ModContent.TileType<hardstonebrick>())
//                            {
//                                WGTools.GetTile(x, y).TileType = (ushort)ModContent.TileType<hardstonetiles>();
//                            }
//                            if (WGTools.GetTile(x, y).WallType == ModContent.WallType<hardstonebrickwall>())
//                            {
//                                WGTools.GetTile(x, y).WallType = (ushort)ModContent.WallType<hardstonetilewall>();
//                            }
//                        }

//                        //float _weathering = weathering.GetNoise(x, y);
//                        //if (_weathering > -0.05f && _weathering < 0.05f)
//                        //{
//                        //    if (tile.type == TileID.WoodBlock || tile.type == TileID.GrayBrick)
//                        //    {
//                        //        if (WGTools.GetTile(x, y - 1).type == TileID.PlanterBox || !WGTools.SurroundingTilesActive(x, y, true))
//                        //        {
//                        //            tile.type = TileID.Grass;
//                        //        }
//                        //        else tile.type = TileID.Dirt;
//                        //    }
//                        //    if (tile.wall == WallID.Wood || tile.wall == WallID.GrayBrick || tile.wall == ModContent.WallType<mansion1>() || tile.wall == ModContent.WallType<mansion2>() || tile.wall == ModContent.WallType<mansion3>())
//                        //    {
//                        //        if (!WGTools.SurroundingTilesActive(x, y, true))
//                        //        {
//                        //            tile.wall = WallID.GrassUnsafe;
//                        //        }
//                        //        else tile.wall = WallID.DirtUnsafe;
//                        //    }
//                        //}
//                    }
//                }
//            }
//            #endregion
//        }

//        #region functions

//        private void ChestLoot(int chestIndex, int objects)
//        {
//            Chest chest = Main.chest[chestIndex];

//            var itemsToAdd = new List<(int type, int stack)>();

//            itemsToAdd.Add((ItemID.HerbBag, 1));

//            if (Main.rand.NextBool(2))
//            {
//                itemsToAdd.Add((ItemID.HealingPotion, Main.rand.Next(3, 6)));
//            }
//            if (Main.rand.NextBool(2))
//            {
//                itemsToAdd.Add((ItemID.ManaPotion, Main.rand.Next(3, 6)));
//            }

//            itemsToAdd.Add((StructureTools.RarePotion(), Main.rand.Next(1, 3)));
//            itemsToAdd.Add((StructureTools.UncommonPotion(), Main.rand.Next(1, 3)));

//            itemsToAdd.Add((ItemID.Book, Main.rand.Next(2, 8)));
//            if (Main.rand.NextBool(2))
//            {
//                itemsToAdd.Add((ItemID.Bone, Main.rand.Next(2, 8)));
//            }

//            itemsToAdd.Add((ItemID.GoldCoin, Main.rand.Next(6, 12)));

//            int chestItemIndex = 0;
//            foreach (var itemToAdd in itemsToAdd)
//            {
//                Item item = new Item();
//                item.SetDefaults(itemToAdd.type);
//                item.stack = itemToAdd.stack;
//                chest.item[chestItemIndex] = item;
//                chestItemIndex++;
//                if (chestItemIndex >= 40)
//                    break;
//            }
//        }

//        private void DresserLoot(int chestIndex)
//        {
//            Chest chest = Main.chest[chestIndex];
//            // itemsToAdd will hold type and stack data for each item we want to add to the chest
//            var itemsToAdd = new List<(int type, int stack)>();

//            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
//            int specialItem = new Terraria.Utilities.WeightedRandom<int>(
//                Tuple.Create((int)ItemID.CobaltShield, 1.0),
//                Tuple.Create((int)ItemID.ShadowKey, 1.0),
//                Tuple.Create((int)ItemID.Muramasa, 1.0),
//                Tuple.Create((int)ItemID.Handgun, 1.0),
//                Tuple.Create((int)ItemID.AquaScepter, 1.0),
//                Tuple.Create((int)ItemID.MagicMissile, 1.0),
//                Tuple.Create((int)ItemID.BlueMoon, 1.0)
//            );

//            if (Main.rand.NextBool(2))
//            {
//                itemsToAdd.Add((ItemID.MolotovCocktail, Main.rand.Next(10, 20)));
//            }

//            itemsToAdd.Add((StructureTools.CommonPotion(), Main.rand.Next(1, 3)));

//            if (Main.rand.NextBool(2))
//            {
//                itemsToAdd.Add((ItemID.RecallPotion, Main.rand.Next(1, 3)));
//            }

//            itemsToAdd.Add((ItemID.Book, Main.rand.Next(2, 8)));

//            if (Main.rand.NextBool(2))
//            {
//                itemsToAdd.Add((ItemID.Bone, Main.rand.Next(2, 8)));
//            }

//            itemsToAdd.Add((ItemID.SilverCoin, Main.rand.Next(50, 100)));

//            int chestItemIndex = 0;
//            foreach (var itemToAdd in itemsToAdd)
//            {
//                Item item = new Item();
//                item.SetDefaults(itemToAdd.type);
//                item.stack = itemToAdd.stack;
//                chest.item[chestItemIndex] = item;
//                chestItemIndex++;
//                if (chestItemIndex >= 40)
//                    break;
//            }
//        }

//        private void PlacePainting(int x, int y, int cellX, int cellY)
//        {
//            int posX = X + cellX * roomWidth - roomWidth / 2;
//            int posY = Y + cellY * roomHeight - roomHeight / 2;

//            int style2 = Main.rand.Next(10);

//            if (style2 == 0)
//            {
//                style2 = 20;
//            }
//            else if (style2 == 1)
//            {
//                style2 = 21;
//            }
//            else if (style2 == 2)
//            {
//                style2 = 22;
//            }
//            else if (style2 == 3)
//            {
//                style2 = 24;
//            }
//            else if (style2 == 4)
//            {
//                style2 = 25;
//            }
//            else if (style2 == 5)
//            {
//                style2 = 26;
//            }
//            else if (style2 == 6)
//            {
//                style2 = 28;
//            }
//            else if (style2 == 7)
//            {
//                style2 = 33;
//            }
//            else if (style2 == 8)
//            {
//                style2 = 34;
//            }
//            else if (style2 == 9)
//            {
//                style2 = 35;
//            }

//            WorldGen.PlaceObject(posX + x, posY + y, TileID.Painting3X3, style: style2);
//        }

//        private bool FreeSpace(int cellX, int cellY, int width, int height)
//        {
//            for (int j = cellY; j < cellY + height; j++)
//            {
//                for (int i = cellX; i < cellX + width; i++)
//                {
//                    if (FindMarker(i, j) || FindMarker(i, j, 1))
//                    {
//                        return false;
//                    }
//                }
//            }
//            return true;
//        }

//        private void AddMarker(int cellX, int cellY, int layer = 0)
//        {
//            layout[cellX - roomsLeft, cellY, layer] = -1;
//        }
//        private bool FindMarker(int cellX, int cellY, int layer = 0)
//        {
//            if (cellX < roomsLeft || cellX > roomsRight || cellY < 0 || cellY >= roomsVertical)
//            {
//                if (layer == 1 && cellY < roomsVertical)
//                {
//                    return true;
//                }
//                else return false;
//            }
//            else return layout[cellX - roomsLeft, cellY, layer] == -1;
//        }
//        #endregion
//        #endregion
//    }

//    public class TheTower : Subworld
//    {
//        public override int Width => 2100;
//        public override int Height => 1200;

//        public override List<GenPass> Tasks => new List<GenPass>()
//        {
//            //new SubworldGenPass(progress => WorldSetup(progress)),
//        };

//        private void WorldSetup(GenerationProgress progress)
//        {
//            progress.Message = "Setting things up";

//            Main.worldSurface = 1; //Hides the underground layer just out of bounds
//            Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds

//            Main.spawnTileX = Main.maxTilesX / 2;
//            Main.spawnTileY = 50;
//        }
//    }

//    public class ArenaSubworld : Subworld
//    {
//        public override int Width => 1600;
//        public override int Height => 800;

//        public override bool ShouldSave => false; //set this to false for testing new worldgen

//        public override bool NormalUpdates => false;

//        public override void Load()
//        {
//            Main.bloodMoon = false;
//            Main.pumpkinMoon = false;
//            Main.snowMoon = false;
//            Main.eclipse = false;

//            base.Load();
//        }

//        public override void Unload()
//        {
//            base.Unload();
//        }

//        public override List<GenPass> Tasks => new List<GenPass>()
//        {
//            //new SubworldGenPass(progress => WorldSetup(progress)),
//            //new SubworldGenPass(progress => TheArena(progress))
//        };

//        private void WorldSetup(GenerationProgress progress)
//        {
//            progress.Message = "Setting things up";

//            Main.worldSurface = Main.maxTilesY - 80; //Hides the underground layer just out of bounds
//            Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds

//            Main.spawnTileX = Main.maxTilesX / 2;
//            Main.spawnTileY = 50;
//        }

//        private void TheArena(GenerationProgress progress)
//        {
//            progress.Message = "Building the arena";

//            int platformSpacing = 25;

//            for (int y = Main.maxTilesY - 80 - platformSpacing; y > 80; y -= platformSpacing)
//            {
//                WGTools.DrawRectangle(1, y, Main.maxTilesX, y, TileID.Platforms, style: 29);
//            }

//            //for (int x = 40; x < Main.maxTilesX - 40; x += 40)
//            //{
//            //    for (int y = Main.maxTilesY - 80 - platformSpacing; y > 80; y -= platformSpacing)
//            //    {
//            //        Generator.GenerateStructure("Structures/arenapillar", new Point16(x - 2, y), ModContent.GetInstance<Remnants>());
//            //        WorldGen.PlaceObject(x, y - 1, TileID.Campfire, style: 1);
//            //    }
//            //}

//            WGTools.DrawRectangle(1, Main.maxTilesY - 80, Main.maxTilesX, Main.maxTilesY, TileID.MarbleBlock);

//            for (int x = 60; x < Main.maxTilesX - 60; x += 40)
//            {
//                WGTools.DrawRectangle(x - 14, Main.maxTilesY - 80, x + 14, Main.maxTilesY - 79, TileID.Dirt);
//            }

//            FastNoiseLite weathering = new FastNoiseLite();
//            weathering.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//            weathering.SetFrequency(0.05f);
//            weathering.SetFractalType(FastNoiseLite.FractalType.FBm);
//            weathering.SetFractalOctaves(3);

//            FastNoiseLite bushes = new FastNoiseLite();
//            bushes.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
//            bushes.SetFrequency(0.2f);
//            bushes.SetFractalType(FastNoiseLite.FractalType.FBm);
//            bushes.SetFractalOctaves(3);

//            for (int y = 1; y < Main.maxTilesY; y++)
//            {
//                for (int x = 1; x < Main.maxTilesX; x++)
//                {
//                    if (WGTools.GetTile(x, y).TileType == TileID.MarbleBlock || WGTools.GetTile(x, y).TileType == TileID.Platforms)
//                    {
//                        WGTools.GetTile(x, y).TileColor = PaintID.OrangePaint;
//                    }
//                    if (WGTools.GetTile(x, y).WallType == WallID.MarbleBlock)
//                    {
//                        WGTools.GetTile(x, y).WallColor = PaintID.OrangePaint;
//                    }

//                    float _weathering = weathering.GetNoise(x, y);
//                    if (_weathering > -0.1f && _weathering < 0.1f)
//                    {
//                        if (WGTools.GetTile(x, y).TileType == TileID.GrayBrick || WGTools.GetTile(x, y).TileType == TileID.StoneSlab)
//                        {
//                            WGTools.GetTile(x, y).TileType = TileID.Stone;
//                        }
//                        if (WGTools.GetTile(x, y).TileType == TileID.MarbleBlock)
//                        {
//                            WGTools.GetTile(x, y).TileType = TileID.Marble;
//                        }
//                        if (WGTools.GetTile(x, y).WallType == WallID.MarbleBlock)
//                        {
//                            WGTools.GetTile(x, y).WallType = WallID.Marble;
//                        }
//                    }

//                    if (WGTools.SurroundingTilesActive(x, y))
//                    {
//                        if (WGTools.GetTile(x, y).TileType == TileID.Dirt || WGTools.GetTile(x, y).TileType == TileID.Grass)
//                        {
//                            WGTools.GetTile(x, y).WallType = WallID.DirtUnsafe;
//                        }
//                        else if (WGTools.GetTile(x, y).TileType == TileID.Marble)
//                        {
//                            WGTools.GetTile(x, y).WallType = WallID.Marble;
//                        }
//                        else if (WGTools.GetTile(x, y).TileType == TileID.MarbleBlock)
//                        {
//                            WGTools.GetTile(x, y).WallType = WallID.MarbleBlock;
//                        }
//                    }
//                    else if (WGTools.GetTile(x, y).TileType == TileID.Dirt)
//                    {
//                        WGTools.GetTile(x, y).TileType = TileID.Grass;
//                    }

//                    Vector2 point = new Vector2(x, Main.maxTilesY - 81);
//                    float multiplier = MathHelper.Clamp((1 - (Vector2.Distance(new Vector2(x, y), point) / 4)), 0, 1);

//                    if (((bushes.GetNoise(x, y) + 1) / 2) * multiplier > 0.25f && WGTools.GetTile(x, y).WallType == 0)
//                    {
//                        WGTools.GetTile(x, y).WallType = WallID.Grass;
//                    }
//                }
//            }
//        }
//    }
//}
