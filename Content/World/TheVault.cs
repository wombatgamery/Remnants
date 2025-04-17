using Microsoft.Xna.Framework;
using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Tools;
using Remnants.Content.Tiles;
using Remnants.Content.Tiles.Objects.Hazards;
using Remnants.Content.Walls.dev;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects.Decoration;

namespace Remnants.Content.World
{
    public class VaultWorld : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex;

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (genIndex != -1 && ModContent.GetInstance<Worldgen>().ExperimentalWorldgen)
            {
                tasks.Insert(genIndex + 1, new Vault("The Vault", 0));
            }
        }
    }

    public class Vault : GenPass
    {
        public Vault(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Vault");

            bool devMode = false;

            #region setup
            Structures.Dungeon vault = new Structures.Dungeon(0, 0, (int)(Main.maxTilesX / 4200f * 12), Main.maxTilesY / 600, 60, 60, 5);

            vault.X = PrimaryBiomes.Jungle.Center * biomes.scale - vault.area.Width / 2;
            vault.Y = Main.maxTilesY - 210 - vault.area.Height;

            WGTools.Rectangle(vault.area.Left - 1, vault.area.Top - 1, vault.area.Right, vault.area.Bottom, ModContent.TileType<VaultPlating>());
            WGTools.Rectangle(vault.area.Left, vault.area.Top, vault.area.Right - 1, vault.area.Bottom - 1, wall: ModContent.WallType<VaultWallUnsafe>(), liquid: 0);
            Structures.FillEdges(vault.area.Left - 1, vault.area.Top - 1, vault.area.Right, vault.area.Bottom, ModContent.TileType<VaultPlating>());
            GenVars.structures.AddProtectedStructure(vault.area, 22);
            #endregion

            #region rooms

            int roomCount;

            #region special

            #endregion

            #region standard
            int roomID = 0;
            int attempts = 0;
            while (attempts < 10000)
            {
                vault.targetCell.X = WorldGen.genRand.Next(vault.grid.Left, vault.grid.Right - (roomID == 0 || roomID == 1 ? 1 : 0));
                vault.targetCell.Y = WorldGen.genRand.Next(0, vault.grid.Bottom - (roomID == 2 || roomID == 3 ? 1 : 0));
                if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
                {
                    if (roomID == 0 || roomID == 1)
                    {
                        if (!vault.FindMarker(vault.targetCell.X + 1, vault.targetCell.Y) && vault.CheckConnection(4, true) && vault.CheckConnection(2, true, 1))
                        {
                            if (roomID == 0)
                            {
                                if (vault.CheckConnection(1, true) && vault.CheckConnection(3, false) && vault.CheckConnection(1, false, 1) && vault.CheckConnection(3, true, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 3); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y, 1);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/2x1", 0, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 1)
                            {
                                if (vault.CheckConnection(1, false) && vault.CheckConnection(3, true) && vault.CheckConnection(1, true, 1) && vault.CheckConnection(3, false, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 1); vault.AddMarker(vault.targetCell.X + 1, vault.targetCell.Y, 3);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/2x1", 1, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                    }
                    else if (roomID == 2 || roomID == 3)
                    {
                        if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y + 1) && vault.CheckConnection(1, true) && vault.CheckConnection(3, true, 0, 1))
                        {
                            if (roomID == 2)
                            {
                                if (vault.CheckConnection(2, true) && vault.CheckConnection(4, false) && vault.CheckConnection(2, false, 0, 1) && vault.CheckConnection(4, true, 0, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 4); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1, 2);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x2", 0, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                            else if (roomID == 3)
                            {
                                if (vault.CheckConnection(2, false) && vault.CheckConnection(4, true) && vault.CheckConnection(2, true, 0, 1) && vault.CheckConnection(4, false, 0, 1))
                                {
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1);
                                    vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 2); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y + 1, 4);

                                    StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x2", 1, vault.roomPos, ModContent.GetInstance<Remnants>());
                                }
                            }
                        }
                    }

                    if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 4)
                {
                    roomID = 0;
                }
            }

            roomID = 0;
            attempts = 0;
            while (attempts < 10000)
            {
                vault.targetCell.X = WorldGen.genRand.Next(vault.grid.Left, vault.grid.Right);
                vault.targetCell.Y = WorldGen.genRand.Next(0, vault.grid.Bottom);

                if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
                {
                    if (roomID == 0)
                    {
                        if (vault.CheckConnection(1, false) && vault.CheckConnection(2, true) && vault.CheckConnection(3, true) && vault.CheckConnection(4, false))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 1); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 0, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 1)
                    {
                        if (vault.CheckConnection(1, false) && vault.CheckConnection(2, false) && vault.CheckConnection(3, true) && vault.CheckConnection(4, true))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 1); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 2);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 1, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 2)
                    {
                        if (vault.CheckConnection(1, true) && vault.CheckConnection(2, false) && vault.CheckConnection(3, false) && vault.CheckConnection(4, true))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 2); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 3);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 2, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }
                    else if (roomID == 3)
                    {
                        if (vault.CheckConnection(1, true) && vault.CheckConnection(2, true) && vault.CheckConnection(3, false) && vault.CheckConnection(4, false))
                        {
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y);
                            vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 3); vault.AddMarker(vault.targetCell.X, vault.targetCell.Y, 4);

                            StructureHelper.API.MultiStructureGenerator.GenerateMultistructureSpecific("Content/World/Structures/Special/TheVault/1x1", 3, vault.roomPos, ModContent.GetInstance<Remnants>());
                        }
                    }

                    if (!vault.FindMarker(vault.targetCell.X, vault.targetCell.Y))
                    {
                        attempts++;
                    }
                    else
                    {
                        attempts = 0;
                    }
                }
                else attempts++;

                if (attempts % 100 == 0)
                {
                    roomID++;
                }
                if (roomID >= 6)
                {
                    roomID = 0;
                }
            }
            #endregion
            #endregion

            #region cleanup
            if (!devMode)
            {
                FastNoiseLite ores = new FastNoiseLite();
                ores.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                ores.SetFrequency(0.04f);
                ores.SetFractalType(FastNoiseLite.FractalType.FBm);
                ores.SetFractalOctaves(3);

                for (int y = vault.area.Top; y <= vault.area.Bottom; y++)
                {
                    for (int x = vault.area.Left; x <= vault.area.Right; x++)
                    {
                        Tile tile = WGTools.Tile(x, y);

                        if (WGTools.Solid(x, y) && tile.TileType != ModContent.TileType<VaultPipe>() && tile.TileType != TileID.Ash && tile.TileType != ModContent.TileType<VaultPlatform>())
                        {
                            if (tile.TileType == ModContent.TileType<VaultPlating>() || tile.TileType == TileID.ConveyorBeltLeft || tile.TileType == TileID.ConveyorBeltRight)
                            {
                                tile.WallType = (ushort)ModContent.WallType<VaultWallUnsafe>();
                            }
                            else tile.WallType = (ushort)ModContent.WallType<vault>();
                        }

                        if (tile.TileType == ModContent.TileType<Hardstone>())
                        {
                            if (ores.GetNoise(x, y) > -0.4f && ores.GetNoise(x, y) < -0.25f)
                            {
                                tile.TileType = TileID.Adamantite;
                            }
                            else if (ores.GetNoise(x, y) > 0.25f && ores.GetNoise(x, y) < 0.4f)
                            {
                                tile.TileType = TileID.Titanium;
                            }
                        }
                        else if (tile.TileType == TileID.Ash)
                        {
                            if (tile.LiquidAmount > 0)
                            {
                                WorldGen.KillTile(x, y);
                            }
                            else if (!WGTools.Tile(x, y - 1).HasTile || WGTools.Tile(x, y - 1).TileType != TileID.Ash)
                            {
                                bool left = !WGTools.Tile(x - 1, y).HasTile || WGTools.Tile(x - 1, y).TileType == ModContent.TileType<VaultPipe>();
                                bool right = !WGTools.Tile(x + 1, y).HasTile || WGTools.Tile(x + 1, y).TileType == ModContent.TileType<VaultPipe>();

                                if (left && right)
                                {
                                    WGTools.Tile(x, y).IsHalfBlock = true;
                                }
                                else if (left)
                                {
                                    WGTools.Tile(x, y).Slope = SlopeType.SlopeDownRight;
                                }
                                else if (right)
                                {
                                    WGTools.Tile(x, y).Slope = SlopeType.SlopeDownLeft;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region objects
            if (!devMode)
            {
                int objects;

                objects = vault.grid.Height * vault.grid.Width * 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(vault.area.Left, vault.area.Right);
                    int y = WorldGen.genRand.Next(vault.area.Top, vault.area.Bottom);

                    bool valid = true;
                    if (WGTools.Tile(x, y + 1).TileType != ModContent.TileType<VaultPlating>() || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneLarge>())
                    {
                        valid = false;
                    }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, ModContent.TileType<DeadDroneLarge>(), style: WorldGen.genRand.NextBool(10) ? Main.rand.Next(4, 6) : Main.rand.Next(4));
                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneLarge>())
                        {
                            objects--;
                        }
                    }
                }
                objects = vault.grid.Height * vault.grid.Width * 2;
                while (objects > 0)
                {
                    int x = WorldGen.genRand.Next(vault.area.Left, vault.area.Right);
                    int y = WorldGen.genRand.Next(vault.area.Top, vault.area.Bottom);

                    bool valid = true;
                    if (WGTools.Tile(x, y + 1).TileType != ModContent.TileType<VaultPlating>() || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneSmall>())
                    {
                        valid = false;
                    }

                    if (valid)
                    {
                        WorldGen.PlaceObject(x, y, ModContent.TileType<DeadDroneSmall>(), style: Main.rand.Next(4));
                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<DeadDroneSmall>())
                        {
                            objects--;
                        }
                    }
                }
            }
            #endregion
        }

        #region functions
        //private void AshPiles()
        //{
        //    int structureCount = vault.grid.Height * vault.grid.Width;

        //    while (structureCount > 0)
        //    {
        //        int x = WorldGen.genRand.Next(location.Left + 21, location.Right - 20);
        //        int y = WorldGen.genRand.Next(location.Top + 21, location.Bottom - 20);

        //        if (!WGTools.Tile(x, y).HasTile && WGTools.Solid(x, y + 1) && WGTools.Tile(x, y).LiquidAmount == 0 && WGTools.Tile(x, y + 1).TileType != ModContent.TileType<VaultPipe>())
        //        {
        //            WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 9) * 2, 1, TileID.Ash, addTile: true, overRide: false);

        //            structureCount--;
        //        }
        //    }

        //    for (int y = location.Bottom; y >= location.Top; y--)
        //    {
        //        for (int x = location.Left; x <= location.Right; x++)
        //        {
        //            if (WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).TileType == TileID.Ash)
        //            {
        //                bool killTile = false;
        //                for (int i = -1; i <= 1; i++)
        //                {
        //                    if (!WGTools.Tile(x + i, y + 1).HasTile || !Main.tileSolid[WGTools.Tile(x + i, y + 1).TileType] || WGTools.Tile(x + i, y + 1).TileType == ModContent.TileType<VaultPipe>())
        //                    {
        //                        killTile = true;
        //                        break;
        //                    }
        //                }
        //                if (killTile)
        //                {
        //                    WorldGen.KillTile(x, y);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        private bool SpikeValidation(int x, int y)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (!WGTools.Tile(i, y).HasTile || WGTools.Tile(i, y).TileType == ModContent.TileType<VaultPlatform>())
                {
                    return false;
                }
            }
            return WGTools.Solid(x, y) && !WGTools.Solid(x, y - 1) && WGTools.Tile(x, y - 1).LiquidAmount == 0;
        }
    }
}
