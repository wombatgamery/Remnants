using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using System.Linq;
using static Remnants.Content.World.PrimaryBiomes;
using static Remnants.Content.World.SecondaryBiomes;
using Terraria.ModLoader.IO;
using Terraria.Chat;
using Terraria.Localization;
using System.Reflection;
using Remnants.Content.NPCs.Monsters;
using Remnants.Content.Walls;
using Remnants.Content.Tiles;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Plants;
using Remnants.Content.Tiles.Objects.Furniture;

namespace Remnants.Content.World
{
    public class RemWorld : ModSystem
    {
        public static Rectangle World => new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY);

        public static int whisperingMazeY;
        public static int whisperingMazeX;
        public static bool sightedWard = false;

        public static int mushroomTiles;
        public static int hiveTiles;
        public static int marbleTiles;
        public static int graniteTiles;
        public static int pyramidTiles;
        public static int oceanCaveTiles;
        public static int gardenTiles;

        public Rectangle skyJungle;

        public static float lavaLevel => GenVars.lavaLine - 50; // (int)(Main.rockLayer + Main.maxTilesY) / 2 + 15; //(int)((Main.maxTilesY - Main.rockLayer) / 2 + Main.rockLayer);

        public override void ResetNearbyTileEffects()
        {
            mushroomTiles = 0;
            hiveTiles = 0;
            marbleTiles = 0;
            graniteTiles = 0;
            pyramidTiles = 0;
            oceanCaveTiles = 0;
            gardenTiles = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            //tag["savedX"] = Main.LocalPlayer.position.X;
            //tag["savedY"] = Main.LocalPlayer.position.Y;
            tag["whisperingMazeX"] = whisperingMazeX;
            tag["whisperingMazeY"] = whisperingMazeY;
            tag["sightedWard"] = sightedWard;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            //if (tag.TryGet("savedX", out int x))
            //{
            //    Main.LocalPlayer.position.X = x;
            //}
            //if (tag.TryGet("savedY", out int y))
            //{
            //    Main.LocalPlayer.position.Y = y;
            //}
            if (tag.TryGet("whisperingMazeX", out int x))
            {
                whisperingMazeX = x;
            }
            if (tag.TryGet("whisperingMazeY", out int y))
            {
                whisperingMazeY = y;
            }
            if (tag.TryGet("sightedWard", out bool flag))
            {
                sightedWard = flag;
            }
        }

        public override void PostUpdateNPCs()
        {
            if (!sightedWard && NPC.AnyNPCs(ModContent.NPCType<Ward>()))
            {
                sightedWard = true;
            }
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            Main.SceneMetrics.SandTileCount += tileCounts[ModContent.TileType<PyramidBrick>()];

            mushroomTiles = tileCounts[TileID.MushroomGrass] + tileCounts[TileID.MushroomPlants] + tileCounts[TileID.MushroomBlock];
            hiveTiles = tileCounts[TileID.Hive];
            marbleTiles = tileCounts[TileID.Marble] + tileCounts[TileID.MarbleBlock] + tileCounts[TileID.MarbleColumn];
            graniteTiles = tileCounts[TileID.Granite] + tileCounts[TileID.GraniteBlock] + tileCounts[TileID.GraniteColumn];

            oceanCaveTiles = tileCounts[TileID.Coralstone];

            gardenTiles = tileCounts[ModContent.TileType<GardenBrick>()];

            //fleshTiles = tileCounts[ModContent.TileType<flesh>()] + tileCounts[ModContent.TileType<hangingeyeball>()] + tileCounts[ModContent.TileType<hangingeyeballvine>()];
        }

        public override void Load()
        {
            On_WorldGen.dropMeteor += Meteor;
            On_WorldGen.ShimmerCleanUp += CancelShimmerCleanup;
        }

        private void Meteor(On_WorldGen.orig_dropMeteor orig)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            int blocks = 0;
            for (int y = 40; y < Main.worldSurface; y++)
            {
                for (int x = 40; x <= Main.maxTilesX - 40; x++)
                {
                    if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Meteorite)
                    {
                        blocks++;

                        if (blocks >= Main.maxTilesX / 1050)
                        {
                            return;
                        }
                    }
                }
            }

            bool success = false;

            while (!success)
            {
                int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(400, (int)(Main.maxTilesX * 0.45f)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.55f), Main.maxTilesX - 400);
                int y = (int)(Main.worldSurface * 0.4f);

                while (!IgnoredByMeteor(x, y))
                {
                    y++;
                }

                bool falling = true;

                while (falling)
                {
                    y++;

                    falling = false;

                    for (int j = y - 5; j <= y + 5; j++)
                    {
                        for (int i = x - 5; i <= x + 5; i++)
                        {
                            if (IgnoredByMeteor(i, j))
                            {
                                falling = true;
                            }
                        }
                    }
                }

                if (y < Main.worldSurface)
                {
                    bool safe = true;
                    int radius = 25;

                    for (int j = y - radius * 2; j <= y + radius; j++)
                    {
                        for (int i = x - radius * 2; i <= x + radius * 2; i++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (TileID.Sets.BasicChest[tile.TileType] || TileID.Sets.AvoidedByMeteorLanding[tile.TileType])
                            {
                                safe = false;
                            }
                            else if (tile.TileType == TileID.Meteorite || Main.tileDungeon[tile.TileType] || tile.TileType == TileID.LivingWood || tile.TileType == TileID.LeafBlock)
                            {
                                safe = false;
                            }
                        }
                    }

                    if (safe)
                    {
                        typeof(WorldGen).GetField("stopDrops", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, true);

                        FastNoiseLite shaping = new FastNoiseLite();
                        shaping.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        shaping.SetFrequency(0.15f);
                        shaping.SetFractalType(FastNoiseLite.FractalType.None);

                        for (int j = y - radius * 2; j <= y; j++)
                        {
                            for (int i = x - radius; i <= x + radius; i++)
                            {
                                float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y - radius)) + shaping.GetNoise(i, j) * 10 + 10;

                                Tile tile = Main.tile[i, j];

                                if (distance < radius * 0.9f)
                                {
                                    WorldGen.KillTile(i, j, noItem: true);

                                    if (distance < radius * 0.8f)
                                    {
                                        tile.WallType = 0;
                                    }
                                    else if (tile.WallType != 0)
                                    {
                                        tile.WallType = WallID.LavaUnsafe1;
                                    }
                                }
                                else if (distance < radius)
                                {
                                    if (tile.HasTile)
                                    {
                                        if (Main.tileCut[tile.TileType])
                                        {
                                            WorldGen.KillTile(i, j, noItem: true);
                                        }
                                        else if (Main.tileSolid[tile.TileType])
                                        {
                                            SlopeType slope = tile.Slope;
                                            WorldGen.KillTile(i, j, noItem: true);
                                            WorldGen.PlaceTile(i, j, TileID.Ash, true);
                                            tile.Slope = slope;
                                        }
                                    }
                                }

                                WorldGen.SquareTileFrame(i, j);
                            }
                        }

                        FastNoiseLite lava = new FastNoiseLite();
                        lava.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
                        lava.SetFrequency(0.1f);
                        lava.SetFractalType(FastNoiseLite.FractalType.None);
                        lava.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);

                        for (int j = y - radius; j <= y + radius; j++)
                        {
                            for (int i = x - radius; i <= x + radius; i++)
                            {
                                float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + shaping.GetNoise(i, j) * 10 + 10;

                                Tile tile = Main.tile[i, j];

                                if (tile.HasTile)
                                {
                                    if (tile.TileType == ModContent.TileType<Nightglow>())
                                    {
                                        tile.HasTile = false;
                                    }
                                    else if (TileID.Sets.GetsDestroyedForMeteors[tile.TileType])
                                    {
                                        WorldGen.KillTile(i, j, noItem: true);
                                        tile.HasTile = false;
                                    }
                                }
                                else if (tile.LiquidAmount > 0)
                                {
                                    Main.tile[i + Main.rand.Next(-radius * 2, radius * 2 + 1), j - Main.rand.Next(radius, radius * 2)].LiquidAmount = tile.LiquidAmount;
                                    tile.LiquidAmount = 0;
                                }

                                if (distance < radius / 1.5f)
                                {
                                    WorldGen.KillTile(i, j, noItem: true);
                                    tile.LiquidType = LiquidID.Lava;

                                    if (distance < radius / 2f && lava.GetNoise(i, j) > -0.3f)
                                    {
                                        tile.LiquidAmount = 255;
                                    }
                                    else WorldGen.PlaceTile(i, j, TileID.Meteorite, true);

                                    if (tile.WallType != 0 || distance < radius / 1.75f)
                                    {
                                        tile.WallType = 0;
                                        WorldGen.PlaceWall(i, j, WallID.LavaUnsafe2);
                                    }
                                }
                                else if (distance < radius)
                                {
                                    if (tile.HasTile)
                                    {
                                        if (Main.tileCut[tile.TileType])
                                        {
                                            WorldGen.KillTile(i, j, noItem: true);
                                        }
                                        else if (Main.tileSolid[tile.TileType])
                                        {
                                            SlopeType slope = tile.Slope;
                                            WorldGen.KillTile(i, j, noItem: true);
                                            WorldGen.PlaceTile(i, j, TileID.Ash, true);
                                            tile.Slope = slope;
                                        }
                                    }

                                    if (tile.WallType != 0)
                                    {
                                        tile.WallType = WallID.LavaUnsafe1;
                                    }
                                }

                                WorldGen.SquareTileFrame(i, j);
                            }
                        }

                        for (int j = y - radius; j <= y + radius; j++)
                        {
                            for (int i = x - radius; i <= x + radius; i++)
                            {
                                Tile tile = Main.tile[i, j];

                                if (tile.HasTile && tile.TileType == TileID.Meteorite && Main.rand.NextBool(2))
                                {
                                    bool left = WorldGen.SolidOrSlopedTile(i - 1, j) && !WorldGen.SolidOrSlopedTile(i + 1, j);
                                    bool right = WorldGen.SolidOrSlopedTile(i + 1, j) && !WorldGen.SolidOrSlopedTile(i - 1, j);
                                    bool top = WorldGen.SolidOrSlopedTile(i, j - 1) && !WorldGen.SolidOrSlopedTile(i, j + 1);
                                    bool bottom = WorldGen.SolidOrSlopedTile(i, j + 1) && !WorldGen.SolidOrSlopedTile(i, j - 1);

                                    if (bottom)
                                    {
                                        if (left)
                                        {
                                            tile.Slope = SlopeType.SlopeDownLeft;
                                        }
                                        else if (right)
                                        {
                                            tile.Slope = SlopeType.SlopeDownRight;
                                        }
                                    }
                                    else if (top)
                                    {
                                        if (left)
                                        {
                                            tile.Slope = SlopeType.SlopeUpLeft;
                                        }
                                        else if (right)
                                        {
                                            tile.Slope = SlopeType.SlopeUpRight;
                                        }
                                    }
                                }
                            }
                        }

                        NetMessage.SendTileSquare(-1, x, y, radius * 2);

                        typeof(WorldGen).GetField("stopDrops", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false);

                        success = true;
                    }
                }
            }

            if (Main.netMode == 0)
            {
                Main.NewText(Lang.gen[59].Value, 50, byte.MaxValue, 130);
            }
            else if (Main.netMode == 2)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.gen[59].Key), new Color(50, 255, 130));
            }

            return;
        }

        private bool IgnoredByMeteor(int i, int j)
        {
            if (!Main.tile[i, j].HasTile || !Main.tileSolid[Main.tile[i, j].TileType] || TileID.Sets.Platforms[Main.tile[i, j].TileType])
            {
                return true;
            }
            if (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == ModContent.TileType<StarOre>())
            {
                return true;
            }
            return false;
        }

        private void CancelShimmerCleanup(On_WorldGen.orig_ShimmerCleanUp orig)
        {
            return;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex;

            //genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids"));
            //if (genIndex != -1)
            //{
            //    tasks.Insert(genIndex + 1, new GrowthPlants("Growth Plants", 1));
            //}

            #region terrain
            InsertPass(tasks, new Terrain("Terrain Improvement", 1), FindIndex(tasks, "Terrain") + 1);
            RemovePass(tasks, FindIndex(tasks, "Rocks In Dirt"));
            RemovePass(tasks, FindIndex(tasks, "Dirt In Rocks"));
            RemovePass(tasks, FindIndex(tasks, "Dirt To Mud"));

            RemovePass(tasks, FindIndex(tasks, "Silt"));
            RemovePass(tasks, FindIndex(tasks, "Slush"));

            //InsertPass(tasks, FindIndex(tasks, "Caves") + 1, new Aquifers("Aquifers", 1));
            RemovePass(tasks, FindIndex(tasks, "Small Holes"));
            RemovePass(tasks, FindIndex(tasks, "Dirt Layer Caves"));
            RemovePass(tasks, FindIndex(tasks, "Rock Layer Caves"));
            RemovePass(tasks, FindIndex(tasks, "Surface Caves"));

            //InsertPass(tasks, FindIndex(tasks, "Tunnels"), new TerrainFeatures("Terrain Features", 1), true);
            RemovePass(tasks, FindIndex(tasks, "Tunnels"));

            RemovePass(tasks, FindIndex(tasks, "Mount Caves"));
            RemovePass(tasks, FindIndex(tasks, "Mountain Caves"));
            RemovePass(tasks, FindIndex(tasks, "Lakes"));

            RemovePass(tasks, FindIndex(tasks, "Shinies"));
            if (ModContent.GetInstance<Worldgen>().OreFrequency > 0)
            {
                InsertPass(tasks, new Ores("Minerals", 1), FindIndex(tasks, "Dungeon") + 1);
            }

            //InsertPass(tasks, FindIndex(tasks, "Gems"), new Ores("Ores", 1));
            //InsertPass(tasks, new Boulders("Boulders", 1), FindIndex(tasks, "Gems"));
            RemovePass(tasks, FindIndex(tasks, "Surface Ore and Stone"));
            //InsertPass(tasks, FindIndex(tasks, "Surface Ore and Stone"), new Boulders("Boulders", 1), true);

            RemovePass(tasks, FindIndex(tasks, "Spreading Grass"));

            //InsertPass(tasks, FindIndex(tasks, "Clean Up Dirt") + 1, new DesertRocks("Desert Rocks", 1));

            RemovePass(tasks, FindIndex(tasks, "Clay"));
            //InsertPass(tasks, FindIndex(tasks, "Planting Trees"), new Clay("Clay", 1));

            if (ModContent.GetInstance<Worldgen>().CloudDensity > 0)
            {
                InsertPass(tasks, new Clouds("Clouds", 1), FindIndex(tasks, "Settle Liquids Again"));
            }
            #endregion
            #region biomes
            //InsertPass(tasks, new Wetlands("Wetlands", 1), FindIndex(tasks, "Generate Ice Biome") + 1);
            InsertPass(tasks, new Caves("Caves", 2), FindIndex(tasks, "Tunnels") + 1); InsertPass(tasks, new PrimaryBiomes("Primary Biomes", 1), FindIndex(tasks, "Generate Ice Biome"), true);
            RemovePass(tasks, FindIndex(tasks, "Jungle")); RemovePass(tasks, FindIndex(tasks, "Muds Walls In Jungle"));
            RemovePass(tasks, FindIndex(tasks, "Full Desert")); RemovePass(tasks, FindIndex(tasks, "Dunes"));

            //InsertPass(tasks, new Undergrowth("Undergrowth", 100), FindIndex(tasks, "Tunnels") + 1);

            //InsertPass(tasks, new Hell("Underworld", 0), FindIndex(tasks, "Underworld"), true);
            RemovePass(tasks, FindIndex(tasks, "Underworld"));
            RemovePass(tasks, FindIndex(tasks, "Corruption"));

            //InsertPass(tasks, new Corruption("Corruption", 0), FindIndex(tasks, "Terrain") + 2);
            //InsertPass(tasks, new Corruption("Infection", 0), FindIndex(tasks, "Dungeon") + 1);

            InsertPass(tasks, new SecondaryBiomes("Secondary Biomes", 20), FindIndex(tasks, "Mud Caves To Grass") + 1);
            //InsertPass(tasks, new Aether("Aether", 20), FindIndex(tasks, "Granite") + 1);


            //InsertPass(tasks, new Clouds("Clouds", 1), FindIndex(tasks, "Dungeon"));

            RemovePass(tasks, FindIndex(tasks, "Mushroom Patches"));
            RemovePass(tasks, FindIndex(tasks, "Marble")); RemovePass(tasks, FindIndex(tasks, "Granite"));
            RemovePass(tasks, FindIndex(tasks, "Hives"));
            RemovePass(tasks, FindIndex(tasks, "Shimmer"));
            RemovePass(tasks, FindIndex(tasks, "Gem Caves")); RemovePass(tasks, FindIndex(tasks, "Gems")); RemovePass(tasks, FindIndex(tasks, "Gems In Ice Biome")); RemovePass(tasks, FindIndex(tasks, "Random Gems"));

            InsertPass(tasks, new Gems("Gem Cave Gems", 20), FindIndex(tasks, "Planting Trees") + 1);
            InsertPass(tasks, new GemTrees("More Trees", 20), FindIndex(tasks, "Planting Trees") + 1);

            //InsertPass(tasks, new Heaven("Sky Lands", 0), FindIndex(tasks, "Secondary Biomes") + 1);

            RemovePass(tasks, FindIndex(tasks, "Sand Patches"));

            //InsertPass(tasks, FindIndex(tasks, "Beaches"), new Beaches("Beaches", 1), true);
            RemovePass(tasks, FindIndex(tasks, "Beaches"));
            RemovePass(tasks, FindIndex(tasks, "Ocean Sand"));
            RemovePass(tasks, FindIndex(tasks, "Create Ocean Caves"));

            RemovePass(tasks, FindIndex(tasks, "Spider Caves"));

            RemovePass(tasks, FindIndex(tasks, "Floating Islands"));
            RemovePass(tasks, FindIndex(tasks, "Floating Island Houses"));
            #endregion

            #region structures
            RemovePass(tasks, FindIndex(tasks, "Surface Chests"));
            RemovePass(tasks, FindIndex(tasks, "Buried Chests"));

            InsertPass(tasks, new Pyramid("Pyramid", 100), FindIndex(tasks, "Pyramids"), true);

            InsertPass(tasks, new TheDungeon("Dungeon", 100), FindIndex(tasks, "Dungeon"), true);
            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (genIndex != -1)
            {
                InsertPass(tasks, new FloatingIslands("Sky Islands", 1), genIndex + 1);
                InsertPass(tasks, new AerialGarden("Aerial Garden", 100), genIndex + 1);
                InsertPass(tasks, new Undergrowth("Undergrowth", 100), genIndex + 1);
                InsertPass(tasks, new ForgottenTomb("Forgotten Tomb", 100), genIndex + 1);

                if (ModContent.GetInstance<Worldgen>().ExperimentalWorldgen)
                {
                    InsertPass(tasks, new InfernalStronghold("Infernal Stronghold", 100), genIndex + 1);
                    InsertPass(tasks, new WaterTemple("Water Temple", 100), genIndex + 1);
                }

                InsertPass(tasks, new HoneySanctum("Honey Sanctum", 0), genIndex + 1);
                InsertPass(tasks, new MagicalLab("Magical Lab", 0), genIndex + 1);
                InsertPass(tasks, new Labyrinth("Echoing Halls", 0), genIndex + 1);
            }

            InsertPass(tasks, new JungleTemple("Jungle Pyramid", 100), FindIndex(tasks, "Jungle Temple"), true);
            RemovePass(tasks, FindIndex(tasks, "Temple"));
            RemovePass(tasks, FindIndex(tasks, "Altars"));

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (genIndex != -1)
            {
                InsertPass(tasks, new ThermalRigs("Thermal Engines", 0), genIndex + 1);
                InsertPass(tasks, new Microdungeons("Microdungeons", 0), genIndex);
                InsertPass(tasks, new IceTemples("Ice Temples", 0), genIndex);
                //tasks.Insert(genIndex, new JungleFortresses("REM-MD: Jungle Fortresses", 0));
                //tasks.Insert(genIndex, new TreasureVaults("Treasure Vaults", 0));
                //tasks.Insert(genIndex, new HoneyShrines("Honey Sanctums", 0));
            }

            //genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Quick Cleanup"));
            //if (genIndex != -1)
            //{
            //    InsertPass(tasks, new Observatories("Observatories", 0), genIndex + 1);
            //}

            InsertPass(tasks, new Mineshafts("Mineshafts", 1), FindIndex(tasks, "Living Trees") + 1);
            RemovePass(tasks, FindIndex(tasks, "Living Trees"));
            RemovePass(tasks, FindIndex(tasks, "Wood Tree Walls"));

            if (!ModContent.GetInstance<Worldgen>().ExperimentalWorldgen)
            {
                InsertPass(tasks, new HellStructures("Hell Structures", 0), FindIndex(tasks, "Underworld") + 1);
            }

            RemovePass(tasks, FindIndex(tasks, "Hellforge"));

            RemovePass(tasks, FindIndex(tasks, "Jungle Chests"));
            #endregion

            #region decoration
            InsertPass(tasks, new Moss("Moss", 1), FindIndex(tasks, "Moss"), true);

            InsertPass(tasks, new SpecialPlants("Special Plants", 1), FindIndex(tasks, "Weeds"));

            RemovePass(tasks, FindIndex(tasks, "Piles"));
            InsertPass(tasks, new Piles("Piles", 0), FindIndex(tasks, "Planting Trees") + 1);

            //InsertPass(tasks, new Bushes("Bushes", 1), FindIndex(tasks, "Final Cleanup"));

            RemovePass(tasks, FindIndex(tasks, "Flowers"));
            //InsertPass(tasks, new Meadows("Meadows", 0), FindIndex(tasks, "Flowers"), true);
            #endregion

            InsertPass(tasks, new BoulderTraps("Boulder Traps", 1), FindIndex(tasks, "Traps"), true);
            //RemovePass(tasks, FindIndex(tasks, "Traps"));

            InsertPass(tasks, new SpawnPointFix("Spawn Point Fix", 1), FindIndex(tasks, "Spawn Point") + 1);

            InsertPass(tasks, new WorldCleanup("Finishing Touches", 1), FindIndex(tasks, "Final Cleanup") + 1);

            ////genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Granite"));
            ////RemovePass(tasks, genIndex);
            ///


            //RemovePass(tasks, FindIndex(tasks, "Random Gems"));

            RemovePass(tasks, FindIndex(tasks, "Gravitating Sand"));
            RemovePass(tasks, FindIndex(tasks, "Shell Piles"));
            RemovePass(tasks, FindIndex(tasks, "Ice"));
            RemovePass(tasks, FindIndex(tasks, "Cave Walls"));
            RemovePass(tasks, FindIndex(tasks, "Wall Variety"));
            RemovePass(tasks, FindIndex(tasks, "Mushrooms"));
            RemovePass(tasks, FindIndex(tasks, "Micro Biomes"));

            if (ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                if (ModContent.GetInstance<Worldgen>().SunkenSeaRework)
                {
                    RemovePass(tasks, FindIndex(tasks, "Sunken Sea"));
                }

                RemovePass(tasks, FindIndex(tasks, "Giant Hive"));
                //if (!ModContent.GetInstance<Client>().ExperimentalWorldgen)
                //{
                //    RemovePass(tasks, FindIndex(tasks, "Vernal Pass"));
                //}
                RemovePass(tasks, FindIndex(tasks, "Evil Island"));

                RemovePass(tasks, FindIndex(tasks, "Gem Depth Adjustment"));

                RemovePass(tasks, FindIndex(tasks, "Growing garden"));
            }

            RemovePass(tasks, FindIndex(tasks, "PlentifulOres"), true);

            if (ModLoader.TryGetMod("FargowiltasSouls", out Mod far))
            {
                RemovePass(tasks, FindIndex(tasks, "GuaranteePyramid"));
                RemovePass(tasks, FindIndex(tasks, "GuaranteePyramidAgain"));
                RemovePass(tasks, FindIndex(tasks, "CursedCoffinArena"));
            }

            if (ModLoader.TryGetMod("AdvancedWorldGen", out Mod awg))
            {
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Dead Man's Chests"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Thin Ice"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Sword Shrines"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Campsites"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Explosive Traps"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Living Trees"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Minecart Tracks"));
                RemovePass(tasks, FindIndex(tasks, "Micro Biomes Lava Traps"));
            }

            if (ModLoader.TryGetMod("Stellamod", out Mod lv))
            {
                RemovePass(tasks, FindIndex(tasks, "World Gen Abysm"));
                RemovePass(tasks, FindIndex(tasks, "World Gen Other stones"));
                RemovePass(tasks, FindIndex(tasks, "World Gen Cinderspark"));
                RemovePass(tasks, FindIndex(tasks, "World Gen Veiled Spot"));
            }

            if (ModLoader.TryGetMod("VervCaves", out Mod bc))
            {
                RemovePass(tasks, FindIndex(tasks, "Post Terrain"), true);
                RemovePass(tasks, FindIndex(tasks, "Post Terrain"), true);
            }

            if (ModContent.GetInstance<Worldgen>().Safeguard)
            {
                InsertPass(tasks, new Safeguard("Safeguard", 1), 1);
            }
        }

        public static void InsertPass(List<GenPass> tasks, GenPass item, int index, bool replace = false)
        {
            if (replace)
            {
                RemovePass(tasks, index);
            }
            if (index != -1)
            {
                tasks.Insert(index, item);
            }

            item.Name = "[R] " + item.Name;
        }

        public static void RemovePass(List<GenPass> tasks, int index, bool destroy = false)
        {
            if (index != -1)
            {
                if (destroy)
                {
                    tasks.RemoveAt(index);
                }
                else tasks[index].Disable();
            }
        }

        public static int FindIndex(List<GenPass> tasks, string value)
        {
            return tasks.FindIndex(genpass => genpass.Name.Equals(value));
        }

        public static Tile Tile(int x, int y)
        {
            return Framing.GetTileSafely(x, y);
        }
    }

    public class WGTools : ModSystem
    {
        public static Tile Tile(float x, float y)
        {
            return Framing.GetTileSafely((int)x, (int)y);
        }

        public static bool SolidTileOf(float x, float y, int type)
        {
            if (!WorldGen.SolidTile((int)x, (int)y) || Main.tile[(int)x, (int)y].TileType != type)
            {
                return false;
            }
            return true;
        }

        public static void CustomTileRunner(float x, float y, float radius, FastNoiseLite noise, int type = -2, int wall = -2, float strength = 1, float xFrequency = 1, float yFrequency = 1, bool add = true, bool replace = true)
        {
            for (int j = (int)(y - radius * 1.5f / yFrequency); j <= y + radius * 1.5f / yFrequency; j++)
            {
                for (int i = (int)(x - radius * 1.5f / xFrequency); i <= x + radius * 1.5f / xFrequency; i++)
                {
                    if (noise.GetNoise(i * xFrequency, j * yFrequency) <= (1 - Vector2.Distance(new Vector2((i - x) * xFrequency + x, (j - y) * yFrequency + y), new Vector2(x, y)) / radius) * strength && WorldGen.InWorld(i, j))
                    {
                        if (type == -1)
                        {
                            if (Framing.GetTileSafely(i, j).HasTile)
                            {
                                Framing.GetTileSafely(i, j).HasTile = false;
                            }
                        }
                        else if (type != -2)
                        {
                            //Framing.GetTileSafely(x, y).Slope = 0;
                            //Framing.GetTileSafely(x, y).IsHalfBlock = false;
                            if (replace && Framing.GetTileSafely(i, j).HasTile || add && !Framing.GetTileSafely(i, j).HasTile)
                            {
                                WorldGen.KillTile(i, j);
                                WorldGen.PlaceTile(i, j, type);
                            }
                        }

                        if (wall == -1)
                        {
                            Framing.GetTileSafely(i, j).WallType = 0;
                        }
                        else if (wall != -2)
                        {
                            if (replace && Framing.GetTileSafely(i, j).WallType != 0 || add && Framing.GetTileSafely(i, j).WallType == 0)
                            {
                                Framing.GetTileSafely(i, j).WallType = (ushort)wall;
                            }
                        }
                    }
                }
            }
        }

        public static void Terraform(Vector2 position, float size, bool[] tilesToDestroy = null, float scaleX = 2, float scaleY = 1, bool killWall = false)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Value);
            noise.SetFrequency(0.5f / (size / 2));
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            //noise.SetFractalLacunarity(1.75f);

            if (tilesToDestroy == null)
            {
                tilesToDestroy = TileID.Sets.Factory.CreateBoolSet(false);

                tilesToDestroy[TileID.Dirt] = true;
                tilesToDestroy[TileID.Grass] = true;
                tilesToDestroy[TileID.Stone] = true;
                tilesToDestroy[TileID.ClayBlock] = true;
                tilesToDestroy[TileID.SnowBlock] = true;
                tilesToDestroy[TileID.IceBlock] = true;
                tilesToDestroy[TileID.Mud] = true;
                tilesToDestroy[TileID.JungleGrass] = true;
                tilesToDestroy[TileID.MushroomGrass] = true;
                tilesToDestroy[TileID.Sand] = true;
                tilesToDestroy[TileID.HardenedSand] = true;
                tilesToDestroy[TileID.Sandstone] = true;
                tilesToDestroy[TileID.DesertFossil] = true;
                tilesToDestroy[TileID.FossilOre] = true;
                tilesToDestroy[TileID.Silt] = true;
                tilesToDestroy[TileID.Slush] = true;
                tilesToDestroy[TileID.MushroomBlock] = true;
                tilesToDestroy[TileID.Granite] = true;
                tilesToDestroy[TileID.Hive] = true;
                tilesToDestroy[TileID.LivingWood] = true;

                tilesToDestroy[TileID.Copper] = true;
                tilesToDestroy[TileID.Tin] = true;
                tilesToDestroy[TileID.Iron] = true;
                tilesToDestroy[TileID.Lead] = true;
                tilesToDestroy[TileID.Silver] = true;
                tilesToDestroy[TileID.Tungsten] = true;
                tilesToDestroy[TileID.Gold] = true;
                tilesToDestroy[TileID.Platinum] = true;

                tilesToDestroy[TileID.Amethyst] = true;
                tilesToDestroy[TileID.Topaz] = true;
                tilesToDestroy[TileID.Sapphire] = true;
                tilesToDestroy[TileID.Emerald] = true;
                tilesToDestroy[TileID.Ruby] = true;
                tilesToDestroy[TileID.Diamond] = true;

                tilesToDestroy[TileID.Cobweb] = true;
            }

            for (int y = (int)(position.Y - size * 2 * scaleY); y <= position.Y + size * 2 * scaleY; y++)
            {
                for (int x = (int)(position.X - size * 2 * scaleX); x <= position.X + size * 2 * scaleX; x++)
                {
                    float threshold = (1 - Vector2.Distance(position, new Vector2((x - position.X) / scaleX + position.X, (y - position.Y) / scaleY + position.Y)) / size) * 2;
                    Tile tile = Framing.GetTileSafely(x, y);

                    if (WorldGen.InWorld(x, y))
                    {
                        if (noise.GetNoise(x / scaleX, y / scaleY) / 0.8f <= threshold && tilesToDestroy[tile.TileType])
                        {
                            tile.HasTile = false;
                            tile.LiquidAmount = 0;

                            if (killWall && !Main.wallDungeon[tile.WallType])
                            {
                                Framing.GetTileSafely(x - 1, y).WallType = 0;
                                Framing.GetTileSafely(x + 1, y).WallType = 0;
                                Framing.GetTileSafely(x, y + 1).WallType = 0;
                            }

                            if (Framing.GetTileSafely(x, y - 1).TileType == TileID.Sand)
                            {
                                Framing.GetTileSafely(x, y - 1).TileType = TileID.HardenedSand;
                            }
                        }
                    }
                    tile = Framing.GetTileSafely(x - 1, y - 1);
                    if (!SurroundingTilesActive(x - 1, y - 1))
                    {
                        if (biomes.FindBiome(x, y) == BiomeID.Jungle)
                        {
                            if (tile.TileType == TileID.Mud)
                            {
                                tile.TileType = TileID.JungleGrass;
                            }
                        }
                        else if (biomes.FindBiome(x, y) == BiomeID.Glowshroom)
                        {
                            if (tile.TileType == TileID.Mud)
                            {
                                tile.TileType = TileID.MushroomGrass;
                            }
                        }
                    }
                }
            }
        }

        //public static void MoonstoneRock(int x, int y) // made by wombat
        //{
        //    FastNoiseLite noise = new FastNoiseLite();
        //    noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        //    noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        //    noise.SetFrequency(0.05f);

        //    float radius = 16;
        //    float xFrequency = 1.5f;
        //    float yFrequency = 1;
        //    float strength = 1;

        //    for (int j = (int)(y - radius * 1.5f / yFrequency); j <= y + radius * 1.5f / yFrequency; j++)
        //    {
        //        for (int i = (int)(x - radius * 1.5f / xFrequency); i <= x + radius * 1.5f / xFrequency; i++)
        //        {
        //            if (noise.GetNoise(i * xFrequency, j * yFrequency) <= (1 - (Vector2.Distance(new Vector2((i - x) * xFrequency + x, (j - y) * yFrequency + y), new Vector2(x, y)) / radius)) * strength)
        //            {
        //                Framing.GetTileSafely(i, j).HasTile = true;
        //                Framing.GetTileSafely(i, j).TileType = TileID.BubblegumBlock; // replace with moonstone tile
        //            }
        //        }
        //    }
        //}

        public static bool Solid(float x, float y)
        {
            return Tile(x, y).HasTile && Main.tileSolid[Tile(x, y).TileType] && !TileID.Sets.Platforms[Tile(x, y).TileType] && Tile(x, y).Slope == SlopeType.Solid && !Tile(x, y).IsHalfBlock;
        }

        public static bool NoDoors(float x, float y, int width = 1)
        {
            return (!Tile(x - 1, y).HasTile || Tile(x - 1, y).TileType != TileID.ClosedDoor && Tile(x - 1, y).TileType != ModContent.TileType<LockedIronDoor>()) && (!Tile(x + width, y).HasTile || Tile(x + width, y).TileType != TileID.ClosedDoor && Tile(x + width, y).TileType != ModContent.TileType<LockedIronDoor>());
        }

        public static void Rectangle(int left, int top, int right, int bottom, int type = -2, int wall = -2, bool add = true, bool replace = true, int style = 0, int liquid = -1, int liquidType = -1)
        {
            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    if (WorldGen.InWorld(x, y))
                    {
                        Tile tile = Main.tile[x, y];

                        if (liquid != -1)
                        {
                            tile.LiquidAmount = (byte)liquid;
                        }
                        if (liquidType != -1)
                        {
                            tile.LiquidType = liquidType;
                        }

                        if (type == -1)
                        {
                            if (tile.HasTile)
                            {
                                tile.HasTile = false;
                            }
                        }
                        else if (type != -2)
                        {
                            if (replace && tile.HasTile && tile.TileType == TileID.ClosedDoor)
                            {
                                WorldGen.KillTile(x, y);
                            }
                            if (replace && tile.HasTile || add && !tile.HasTile)
                            {
                                tile.HasTile = false;
                                tile.Slope = 0;
                                tile.IsHalfBlock = false;
                                WorldGen.PlaceTile(x, y, type, true, style: style);
                            }
                        }

                        if (wall == -1)
                        {
                            tile.WallType = 0;
                        }
                        else if (wall != -2)
                        {
                            if (replace && tile.WallType != 0 || add && tile.WallType == 0)
                            {
                                tile.WallType = (ushort)wall;
                            }
                        }
                    }
                }
            }
        }

        public static void Circle(float x2, float y2, float radius, int type = -2, int wall = -2, float xMultiplier = 1f, float yMultiplier = 1f, bool add = true, bool replace = true, int liquid = -1, int liquidType = -1)
        {
            for (int y = (int)(y2 - radius * yMultiplier); y <= y2 + radius * yMultiplier; y++)
            {
                for (int x = (int)(x2 - radius * xMultiplier); x <= x2 + radius * xMultiplier; x++)
                {
                    if (Vector2.Distance(new Vector2(x2, y2), new Vector2((x - x2) / xMultiplier + x2, (y - y2) / yMultiplier + y2)) < radius && WorldGen.InWorld(x, y))
                    {
                        Tile tile = Main.tile[x, y];

                        if (liquid != -1)
                        {
                            tile.LiquidAmount = (byte)liquid;
                        }
                        if (liquidType != -1)
                        {
                            tile.LiquidType = liquidType;
                        }

                        if (type == -1)
                        {
                            if (tile.HasTile)
                            {
                                tile.HasTile = false;
                            }
                        }
                        else if (type != -2)
                        {
                            //tile.Slope = 0;
                            //tile.IsHalfBlock = false;
                            if (replace && tile.HasTile && tile.TileType != ModContent.TileType<devtile>() || add && !tile.HasTile)
                            {
                                tile.HasTile = false;
                                WorldGen.PlaceTile(x, y, type);
                            }
                        }

                        if (wall == -1)
                        {
                            tile.WallType = 0;
                        }
                        else if (wall != -2)
                        {
                            if (replace && tile.WallType != 0 || add && tile.WallType == 0)
                            {
                                tile.WallType = (ushort)wall;
                            }
                        }
                    }
                }
            }
        }

        public static void Wire(int startX, int startY, int finishX, int finishY, bool red = true, bool blue = false, bool green = false, bool yellow = false)
        {
            Tile tile;
            for (int x = startX; x != finishX; x += x < finishX ? 1 : -1)
            {
                tile = Framing.GetTileSafely(x, startY);
                tile.RedWire = red;
                tile.BlueWire = blue;
                tile.GreenWire = green;
                tile.YellowWire = yellow;
            }
            for (int y = startY; y != finishY; y += y < finishY ? 1 : -1)
            {
                tile = Framing.GetTileSafely(finishX, y);
                tile.RedWire = red;
                tile.BlueWire = blue;
                tile.GreenWire = green;
                tile.YellowWire = yellow;
            }
            tile = Framing.GetTileSafely(finishX, finishY);
            tile.RedWire = red;
            tile.BlueWire = blue;
            tile.GreenWire = green;
            tile.YellowWire = yellow;
        }

        public static bool SurroundingTilesActive(float x, float y, bool checkSolid = false)
        {
            if (Tile(x + 1, y).HasTile && Tile(x + 1, y + 1).HasTile && Tile(x, y + 1).HasTile && Tile(x - 1, y + 1).HasTile && Tile(x - 1, y).HasTile && Tile(x - 1, y - 1).HasTile && Tile(x, y - 1).HasTile && Tile(x + 1, y - 1).HasTile)
            {
                if (checkSolid)
                {
                    if (Solid(x + 1, y) && Solid(x + 1, y + 1) && Solid(x, y + 1) && Solid(x - 1, y + 1) && Solid(x - 1, y) && Solid(x - 1, y - 1) && Solid(x, y - 1) && Solid(x + 1, y - 1))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void MediumPile(int x, int y, int Xframe = 0, int Yframe = 0)
        {
            if (RemTile.SolidTop(x, y + 1) && RemTile.SolidTop(x + 1, y + 1))
            {
                if (!Framing.GetTileSafely(x, y).HasTile && !Framing.GetTileSafely(x + 1, y).HasTile)
                {
                    Framing.GetTileSafely(x, y).HasTile = true;
                    Framing.GetTileSafely(x + 1, y).HasTile = true;
                    Framing.GetTileSafely(x, y).TileType = TileID.SmallPiles;
                    Framing.GetTileSafely(x + 1, y).TileType = TileID.SmallPiles;

                    Framing.GetTileSafely(x, y).TileFrameX = (short)(Xframe * 36);
                    Framing.GetTileSafely(x + 1, y).TileFrameX = (short)(Xframe * 36 + 18);
                    Framing.GetTileSafely(x, y).TileFrameY = (short)(18 + Yframe * 18);
                    Framing.GetTileSafely(x + 1, y).TileFrameY = (short)(18 + Yframe * 18);
                }
            }
        }

        public static int AdjacentTiles(float x, float y, bool checkSolid = false)
        {
            int adjacentTiles = 0;
            if (checkSolid)
            {
                if (Tile(x + 1, y).HasTile && Main.tileSolid[Tile(x + 1, y).TileType] && Tile(x, y - 1).TileType != TileID.Platforms) { adjacentTiles++; }
                if (Tile(x - 1, y).HasTile && Main.tileSolid[Tile(x - 1, y).TileType] && Tile(x, y - 1).TileType != TileID.Platforms) { adjacentTiles++; }
                if (Tile(x, y + 1).HasTile && Main.tileSolid[Tile(x, y + 1).TileType] && Tile(x, y - 1).TileType != TileID.Platforms) { adjacentTiles++; }
                if (Tile(x, y - 1).HasTile && Main.tileSolid[Tile(x, y - 1).TileType] && Tile(x, y - 1).TileType != TileID.Platforms) { adjacentTiles++; }
            }
            else
            {
                if (Tile(x + 1, y).HasTile) { adjacentTiles++; }
                if (Tile(x - 1, y).HasTile) { adjacentTiles++; }
                if (Tile(x, y + 1).HasTile) { adjacentTiles++; }
                if (Tile(x, y - 1).HasTile) { adjacentTiles++; }
            }
            return adjacentTiles;
        }

        public static void WoodenBeam(int x, int y)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            while (true)
            {
                int type = biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.BorealBeam : biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive ? TileID.RichMahoganyBeam : biomes.FindBiome(x, y) == BiomeID.Desert ? TileID.SandstoneColumn : biomes.FindBiome(x, y) == BiomeID.Glowshroom ? TileID.MushroomBeam : biomes.FindBiome(x, y) == BiomeID.Granite ? TileID.GraniteColumn : TileID.WoodenBeam;

                if (Tile(x, y + 1).HasTile || Tile(x, y + 1).TileType == TileID.Cobweb)
                {
                    if ((Tile(x, y + 1).TileType == TileID.WoodBlock || Tile(x, y + 1).TileType == TileID.RichMahogany) && !WorldGen.SolidTile(x, y + 2))
                    {
                        WorldGen.PlaceTile(x, y + 2, type);
                        y++;
                    }
                    else break;
                }
                else WorldGen.PlaceTile(x, y + 1, type);

                y++;
            }

            if (Main.tileSolid[Tile(x, y + 1).TileType])
            {
                Tile(x, y + 1).TileType = biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.BorealWood : biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive ? TileID.RichMahogany : biomes.FindBiome(x, y) == BiomeID.Desert ? TileID.SmoothSandstone : biomes.FindBiome(x, y) == BiomeID.Glowshroom ? TileID.MushroomBlock : biomes.FindBiome(x, y) == BiomeID.Granite ? TileID.GraniteBlock : TileID.WoodBlock;
            }
        }

        public static void PlaceObjectsInArea(int left, int top, int right, int bottom, int tile, int style2 = 0, int count = 1, int attemptLimit = 1000)
        {
            while (count > 0)
            {
                bool success = false;
                int attempts = 0;
                while (!success && attempts <= attemptLimit)
                {
                    attempts++;

                    int x = WorldGen.genRand.Next(left, right + 1);
                    int y = WorldGen.genRand.Next(top, bottom + 1);

                    if (Framing.GetTileSafely(x, y).TileType != tile)
                    {
                        WorldGen.PlaceObject(x, y, tile, style: style2, direction: WorldGen.genRand.NextBool(2) ? 1 : -1);
                    }
                    success = Framing.GetTileSafely(x, y).TileType == tile;
                    if (success)
                    {
                        count--;
                    }
                }
                if (attempts > attemptLimit)
                {
                    break;
                }
            }
        }

        public static void CandleBunch(int left, int top, int right, int bottom, int count = 1)
        {
            if (ModLoader.TryGetMod("WombatQOL", out Mod wgi) && wgi.TryFind("CeremonialCandle", out ModTile candle))
            {
                while (count > 0)
                {
                    bool success = false;
                    int attempts = 0;
                    while (!success && attempts <= 1000)
                    {
                        attempts++;

                        int x = WorldGen.genRand.Next(left, right + 1);
                        int y = WorldGen.genRand.Next(top, bottom + 1);

                        if (Framing.GetTileSafely(x, y).TileType != candle.Type)
                        {
                            WorldGen.PlaceObject(x, y, candle.Type, style: WorldGen.genRand.Next(6), direction: WorldGen.genRand.NextBool(2) ? 1 : -1);
                        }
                        success = Framing.GetTileSafely(x, y).TileType == candle.Type;
                        if (success)
                        {
                            count--;
                        }
                    }
                    if (attempts > 1000)
                    {
                        break;
                    }
                }
            }
        }
    }

    public class Safeguard : GenPass
    {
        public Safeguard(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Safeguard");

            VerifyCompatibility();
        }

        public void VerifyCompatibility()
        {
            bool issuesFound = false;
            string message = Language.GetTextValue("Mods.Remnants.Safeguard.IssuesFound");

            if (Main.maxTilesX < 6300 || Main.maxTilesY < 1800)
            {
                issuesFound = true;
                message = message + "\n. " + Language.GetTextValue("Mods.Remnants.Safeguard.SmallWorlds");
            }
            if (Main.specialSeedWorld)
            {
                issuesFound = true;
                message = message + "\n. " + Language.GetTextValue("Mods.Remnants.Safeguard.SecretSeeds");
            }
            if (Main.maxTilesX != (int)(Main.maxTilesY * 84f/24f) && Main.maxTilesX != (int)(Main.maxTilesY * 64f / 18f))
            {
                issuesFound = true;
                message = message + "\n. " + Language.GetTextValue("Mods.Remnants.Safeguard.BadRatio");
            }
            if (ModLoader.TryGetMod("StartWithBase", out Mod swb))
            {
                issuesFound = true;
                message = message + "\n. " + Language.GetTextValue("Mods.Remnants.Safeguard.StartWithBase");
            }
            if (ModLoader.TryGetMod("ContinentOfJourney", out Mod hwj))
            {
                issuesFound = true;
                message = message + "\n. " + Language.GetTextValue("Mods.Remnants.Safeguard.HomewardJourney");
            }
            if (ModLoader.TryGetMod("Aequus", out Mod aq))
            {
                issuesFound = true;
                message = message + "\n. " + Language.GetTextValue("Mods.Remnants.Safeguard.Aequus");
            }
            message = message + "\n" + Language.GetTextValue("Mods.Remnants.Safeguard.Solution");

            if (issuesFound)
            {
                throw new Exception("\n \n" + message + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n --------------------------------- \n");
            }
        }
    }

    public class Caves : GenPass
    {
        public Caves(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Caves");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            float scale = 1.25f;// * (2 / 1.5f);
            float heightMultiplier = 2;

            float transit1 = (int)Main.worldSurface - 60;
            float transit2 = (int)Main.rockLayer;

            FastNoiseLite caves = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caves.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            caves.SetFrequency(0.01f / scale);
            caves.SetFractalType(FastNoiseLite.FractalType.FBm);
            caves.SetFractalOctaves(3);

            FastNoiseLite caveSize = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveSize.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveSize.SetFrequency(0.01f / scale);
            caveSize.SetFractalType(FastNoiseLite.FractalType.FBm);
            caveSize.SetFractalGain(0.7f);
            caveSize.SetFractalOctaves(4);

            FastNoiseLite caveOffset = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveOffset.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveOffset.SetFrequency(0.01f / scale);
            caveOffset.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite caveRoughness = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveRoughness.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveRoughness.SetFrequency(0.04f / scale);
            caveRoughness.SetFractalType(FastNoiseLite.FractalType.FBm);
            FastNoiseLite caveRoughness2 = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            caveRoughness2.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            caveRoughness2.SetFrequency(0.01f / scale);
            caveRoughness2.SetFractalType(FastNoiseLite.FractalType.FBm);

            FastNoiseLite background = new FastNoiseLite();
            background.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            background.SetFrequency(0.04f);
            background.SetFractalType(FastNoiseLite.FractalType.FBm);
            background.SetFractalOctaves(2);
            background.SetFractalWeightedStrength(-1);
            background.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            background.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);

            for (float y = 40; y < Main.maxTilesY - 100; y++)
            {
                progress.Set((y - ((int)Main.worldSurface - 40)) / (Main.maxTilesY - 200 - ((int)Main.worldSurface - 40)));

                //float divider = -((y - transit1) / (transit2 - transit1)) + 1;
                //float intensity = 0.6f;
                //if (divider < 1) {
                //    divider = 1;
                //}
                //if (divider > 1 + intensity) {
                //    divider = 1 + intensity;
                //}
                //divider = (divider - 1) * intensity + 1;
                for (float x = 325; x < Main.maxTilesX - 325; x++)
                {
                    if (WGTools.Tile(x, y).HasTile && biomes.FindLayer((int)x, (int)y) >= biomes.surfaceLayer)
                    {
                        //float transit = MathHelper.Clamp((y - transit1) / (transit2 - transit1), 0, 1);// / 2 + 0.5f;

                        float mult = 2;

                        float threshold = 1;
                        //threshold -= MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y * 2.5f), new Vector2(1, ((int)Main.worldSurface - 85) * 2.5f)) / 500) * 3, 0, 1);
                        //threshold -= MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y * 2.5f), new Vector2(Main.maxTilesX, ((int)Main.worldSurface - 85) * 2.5f)) / 500) * 3, 0, 1);

                        float _size = (caveSize.GetNoise(x * mult, y * mult * heightMultiplier) * 0.8f + 0.25f) * threshold;
                        float _caves = caves.GetNoise(x * mult, y * mult * heightMultiplier);
                        float _roughness = caveRoughness.GetNoise(x * mult, y * mult * heightMultiplier) * ((caveRoughness2.GetNoise(x * mult, y * mult * heightMultiplier) + 1) / 2);
                        float _offset = caveOffset.GetNoise(x * mult, y * mult * heightMultiplier);

                        float _overall = (_caves + _roughness / 3) / (1 + 1 / 2);

                        if (biomes.FindLayer((int)x, (int)y) < biomes.caveLayer)
                        {
                            if (_overall - _offset < -_size / 1 || _overall - _offset > _size / 1)
                            {
                                WGTools.Tile(x, y).HasTile = false;
                            }


                            if (background.GetNoise(x, y * 2) > -0.5f)// || WGTools.SurroundingTilesActive(x, y, true))
                            {
                                WGTools.Tile(x, y).WallType = WGTools.Tile(x, y).TileType == TileID.Stone ? WallID.RocksUnsafe1 : WallID.Cave6Unsafe;
                            }
                        }
                        else
                        {
                            if (_overall - _offset > -_size / 2 && _overall - _offset < _size / 2)
                            {
                                WGTools.Tile(x, y).HasTile = false;
                            }

                            if (biomes.FindLayer((int)x, (int)y) < biomes.height  - 6)
                            {
                                if (_overall * 4 - _offset < -_size || _overall * 4 - _offset > _size)
                                {
                                    WGTools.Tile(x, y).WallType = WGTools.Tile(x, y).TileType == TileID.Dirt ? WallID.Cave6Unsafe : biomes.FindLayer((int)x, (int)y) >= biomes.lavaLayer ? WallID.Cave8Unsafe : WallID.RocksUnsafe1;
                                }
                            }
                        }


                        if (y > GenVars.lavaLine)
                        {
                            WGTools.Tile(x, y).LiquidType = LiquidID.Lava;
                        }
                    }
                }
            }

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.FloodedCaves");

            int structureCount = Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 50000;
            while (structureCount > 0)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);

                if (!WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).LiquidAmount != 255 && biomes.FindBiome(x, y) != BiomeID.Jungle)
                {
                    int radius = WorldGen.genRand.Next(25, 50);

                    for (int j = Math.Max(y - radius, (int)Main.rockLayer); j <= Math.Min(y + radius, Main.maxTilesY - 300); j++)
                    {
                        for (int i = x - radius; i <= x + radius; i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) < radius && WorldGen.InWorld(i, j))
                            {
                                WGTools.Tile(i, j).LiquidAmount = 255;
                            }
                        }
                    }

                    structureCount--;
                }
            }
        }
    }

    public class Terrain : GenPass
    {
        public Terrain(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        private static float scaleX => Main.maxTilesX / 4200f;
        private static float scaleY => Main.maxTilesY / 1200f;

        public static int Minimum => (int)(Main.worldSurface - 60 - 150 * scaleY * ModContent.GetInstance<Worldgen>().TerrainAmplitude);
        public static int Maximum => (int)(Main.worldSurface - 60);
        public static int Middle => (Minimum + Maximum) / 2;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            Main.worldSurface = (int)(Main.maxTilesY / 3f / 6) * 6;
            Main.rockLayer = (int)(Main.maxTilesY / 2.25f / 6) * 6;
            //GenVars.lavaLine = (int)((Main.maxTilesY * 0.75f) / 6) * 6;

            //if (ModContent.GetInstance<Client>().LargerSky)
            //{
            //    Main.worldSurface += Main.maxTilesX / 21;
            //    Main.rockLayer += Main.maxTilesX / 21;
            //    GenVars.lavaLine += Main.maxTilesX / 21;
            //}

            #region terrain
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Terrain");

            FastNoiseLite altitude = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            altitude.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            altitude.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            altitude.SetCellularJitter(0f);
            altitude.SetFractalType(FastNoiseLite.FractalType.FBm);
            altitude.SetFrequency(0.003f / scaleX);
            altitude.SetFractalOctaves(3);
            altitude.SetFractalLacunarity(3);
            altitude.SetFractalGain(0.75f);
            float[] altitudes = new float[Main.maxTilesX];
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                altitudes[i] = altitude.GetNoise(i, 0);// ((int)((altitude.GetNoise(i, 0) * (Maximum - Minimum)) / 12f) * 12) / (Maximum - Minimum);
            }

            FastNoiseLite roughness = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            roughness.SetNoiseType(FastNoiseLite.NoiseType.Value);
            roughness.SetFrequency(0.015f / scaleX);
            roughness.SetFractalType(FastNoiseLite.FractalType.FBm);
            roughness.SetFractalOctaves(5);

            for (float y = MathHelper.Clamp(Minimum - (Maximum - Minimum) / 2, 0, (int)Main.worldSurface * 0.35f); y <= Main.maxTilesY - 200; y++)
            {
                progress.Set(y / (Main.maxTilesY - 200));

                for (float x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[(int)x, (int)y];

                    if (y < Main.worldSurface)
                    {
                        float beachMultiplier = MathHelper.Clamp(Vector2.Distance(new Vector2(x, 0), new Vector2(MathHelper.Clamp(x, 0, 350), 0)) / (150 * scaleX), 0, 1) * MathHelper.Clamp(Vector2.Distance(new Vector2(x, 0), new Vector2(MathHelper.Clamp(x, Main.maxTilesX - 350, Main.maxTilesX), 0)) / (150 * scaleX), 0, 1);

                        float mountainX = Tundra.X * biomes.scale + biomes.scale / 2;
                        float mountainMultiplier = MathHelper.Clamp(MathHelper.Distance(x, mountainX) / (200 * scaleX), 0, 1);
                        float mountainMultiplier2 = MathHelper.Clamp(MathHelper.Distance(x, mountainX) / (300 * scaleX), 0, 1);
                        float valleyX = Jungle.Center * biomes.scale + biomes.scale / 2;
                        float valleyMultiplier = MathHelper.Clamp(MathHelper.Distance(x, valleyX) / (100 * scaleX) - 0.5f, 0, 1);
                        float valleyMultiplier2 = MathHelper.Clamp(MathHelper.Distance(x, valleyX) / (150 * scaleX) - 0.5f, 0, 1);

                        float _altitude = 0;
                        for (int i = (int)(x - 10 * scaleX); i <= x + 10 * scaleX; i++)
                        {
                            _altitude += altitudes[(int)MathHelper.Clamp(i, 0, Main.maxTilesX - 1)];
                        }
                        _altitude /= 1 + 20 * scaleX;
                        _altitude -= 1f;
                        _altitude *= beachMultiplier;
                        _altitude *= 0.5f;
                        _altitude += 1f;

                        if (ModContent.GetInstance<Worldgen>().IceMountain)
                        {
                            _altitude -= 1f;
                            _altitude *= mountainMultiplier2;
                            _altitude += 1f;

                            _altitude += 0.5f;
                            _altitude *= mountainMultiplier;
                            _altitude -= 0.5f;
                        }

                        if (ModContent.GetInstance<Worldgen>().JungleValley)
                        {
                            _altitude -= 0.5f;
                            _altitude *= valleyMultiplier2;
                            _altitude += 0.5f;

                            _altitude -= 1f;
                            _altitude *= valleyMultiplier;
                            _altitude += 1f;
                        }

                        float _roughness = roughness.GetNoise(x, y * 2) / 0.8f;
                        _roughness *= 0.5f + beachMultiplier;
                        _roughness += 1f;
                        _roughness /= 2;
                        //_bumps *= (y - Minimum) / (Maximum - Minimum);

                        float _terrain = _altitude * 0.85f + _roughness * 0.2f;
                        _terrain -= 1 - 0.5f / ModContent.GetInstance<Worldgen>().TerrainAmplitude;
                        _terrain *= MathHelper.Clamp(Vector2.Distance(new Vector2(x, 0), new Vector2(MathHelper.Clamp(x, Main.maxTilesX / 2 - 50 * scaleX, Main.maxTilesX / 2 + 50 * scaleX), 0)) / (100 * scaleX), 0.25f, 1);
                        _terrain += 1 - 0.5f / ModContent.GetInstance<Worldgen>().TerrainAmplitude;

                        float threshold = _terrain * (Maximum - Minimum) + Minimum;
                        threshold -= (int)Main.worldSurface;
                        threshold *= MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, 0, 125)) / 125, 0.15f, 1);
                        threshold *= MathHelper.Clamp(MathHelper.Distance(x, MathHelper.Clamp(x, Main.maxTilesX - 125, Main.maxTilesX)) / 125, 0.15f, 1);
                        threshold += (int)Main.worldSurface;

                        if (y >= threshold) // + Math.Sin((y * MathHelper.TwoPi) / 10) * 2
                        {
                            tile.HasTile = true;
                        }
                        else
                        {
                            tile.HasTile = false;
                            if (y >= (int)Main.worldSurface - 60)
                            {
                                tile.LiquidAmount = 255;
                            }
                        }
                    }
                    else tile.HasTile = true;

                    float blendLow = (int)Main.worldSurface;
                    float blendHigh = (int)Main.rockLayer;
                    float num = MathHelper.Clamp((y - blendLow) / (blendHigh - blendLow) * 2 - 1, -1, 1);

                    int layer = biomes.FindLayer((int)x, (int)y);

                    if (layer >= biomes.caveLayer && biomes.MaterialBlend(x, y, true) <= -0.3f)
                    {
                        tile.TileType = TileID.Silt;
                    }
                    else if (biomes.MaterialBlend(x, y, frequency: 2) <= (layer >= biomes.caveLayer ? 0.2f : -0.2f) || layer >= biomes.lavaLayer)
                    {
                        tile.TileType = TileID.Stone;
                    }
                    else if (layer < biomes.surfaceLayer && biomes.MaterialBlend(x, y, true) >= 0.2f)
                    {
                        tile.TileType = TileID.ClayBlock;
                    }
                    else if (layer >= biomes.surfaceLayer && layer < biomes.caveLayer && biomes.MaterialBlend(x, y, frequency: 2) >= 0.3f && y > Main.worldSurface + 5)
                    {
                        tile.TileType = TileID.Sand;
                    }
                    else tile.TileType = TileID.Dirt;

                    if (tile.TileType == TileID.Stone || tile.TileType == TileID.ClayBlock)
                    {
                        for (int i = 1; i <= WorldGen.genRand.Next(4, 7); i++)
                        {
                            if (!WGTools.Tile(x, y - i).HasTile)
                            {
                                tile.TileType = TileID.Dirt;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region swamps
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Swamps");

            FastNoiseLite distribution = new FastNoiseLite(WorldGen.genRand.Next(int.MinValue, int.MaxValue));
            distribution.SetNoiseType(FastNoiseLite.NoiseType.Value);
            distribution.SetFrequency(0.015f);
            distribution.SetFractalType(FastNoiseLite.FractalType.FBm);
            distribution.SetFractalOctaves(3);

            FastNoiseLite islets = new FastNoiseLite();
            islets.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            islets.SetFrequency(0.1f);
            islets.SetFractalType(FastNoiseLite.FractalType.FBm);
            islets.SetFractalOctaves(3);
            //islets.SetFractalGain(0.6f);

            for (int y = 40; y < Main.rockLayer; y++)
            {
                progress.Set(y / Main.rockLayer);

                for (int x = 300; x < Main.maxTilesX - 300; x++)
                {
                    Vector2 point = new Vector2(MathHelper.Clamp(x, 600, Main.maxTilesX - 600), MathHelper.Clamp(y, Middle, (float)Main.worldSurface));
                    float multiplier = MathHelper.Clamp((1 - Vector2.Distance(new Vector2(x, y), point) / (Main.maxTilesY / 18)) * 2, 0, 1);
                    multiplier *= MathHelper.Clamp(Vector2.Distance(new Vector2(x, y), new Vector2(Main.maxTilesX / 2, y)) / (Main.maxTilesY / 6), 0, 1);

                    if (biomes.FindLayer(x, y) < biomes.surfaceLayer && (distribution.GetNoise(x, y * 2) + 1 / 2) * multiplier > 0.15f && biomes.FindBiome(x, y) != BiomeID.Desert && WGTools.Tile(x, y).HasTile)
                    {
                        //if ((noise.GetNoise(x, y * 3) + noise2.GetNoise(x * 1.5f, y)) / 2 > 0.06f)
                        //{
                        //    WGTools.GetTile(x, y).active(true);
                        //}
                        WGTools.Tile(x, y).WallType = biomes.FindBiome(x, y) == BiomeID.Tundra ? WallID.SnowWallUnsafe : WallID.DirtUnsafe;

                        if (islets.GetNoise(x, y * 2) > -0.7f)
                        {
                            WGTools.Tile(x, y).HasTile = true;
                            WGTools.Tile(x, y).TileType = biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.SnowBlock : biomes.FindBiome(x, y) == BiomeID.Jungle ? TileID.Mud : TileID.Dirt;
                        }
                        else
                        {
                            WGTools.Tile(x, y).HasTile = false;
                            WGTools.Tile(x, y).LiquidAmount = 85;

                            if (islets.GetNoise(x, y * 2) < WorldGen.genRand.NextFloat(0.9f, 0.95f))
                            {
                                WGTools.Tile(x, y).WallType = 0;
                            }
                        }

                        //if (WorldGen.genRand.Next(100) <= 60)
                        //{
                        //    WGTools.Tile(x, y).wall = (ushort)ModContent.WallType<devwall>();
                        //}
                        //else
                        //{
                        //    WGTools.Tile(x, y).LiquidAmount = 255;
                        //}
                    }
                }
            }
            #endregion

            #region borders
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                WorldGen.TileRunner(40, y, WorldGen.genRand.Next(7, 10), 1, ModContent.TileType<Hardstone>());
                WorldGen.TileRunner(Main.maxTilesX - 40, y, WorldGen.genRand.Next(7, 10), 1, ModContent.TileType<Hardstone>());
                for (int x = 0; x < 40; x++)
                {
                    WGTools.Tile(x, y).HasTile = true;
                    WGTools.Tile(x, y).TileType = (ushort)ModContent.TileType<Hardstone>();
                }
                for (int x = Main.maxTilesX - 40; x < Main.maxTilesX; x++)
                {
                    WGTools.Tile(x, y).HasTile = true;
                    WGTools.Tile(x, y).TileType = (ushort)ModContent.TileType<Hardstone>();
                }
            }
            for (int x = 40; x < Main.maxTilesX - 40; x++)
            {
                WorldGen.TileRunner(x, Main.maxTilesY - 42, WorldGen.genRand.Next(7, 10), 1, ModContent.TileType<Hardstone>(), true);
                for (int y = Main.maxTilesY - 40; y < Main.maxTilesY; y++)
                {
                    WGTools.Tile(x, y).HasTile = true;
                    WGTools.Tile(x, y).TileType = (ushort)ModContent.TileType<Hardstone>();
                }
            }
            #endregion

            //WorldGen.PlaceTile(300, (int)Main.worldSurface - 120, TileID.BubblegumBlock);
            //WorldGen.PlaceTile(Main.maxTilesX - 300, (int)Main.worldSurface - 120, TileID.BubblegumBlock);

            //Debris();

            //for (int i = 0; i < 10; i++)
            //{
            //    WGTools.MoonstoneRock(Main.maxTilesX / 2 + i * 32, (int)Main.worldSurface - 200);
            //}
        }

        private void DoBlend(float x, float y, int type, int chance, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (WorldGen.genRand.NextBool(chance * 10))
                {
                    WorldGen.TileRunner((int)x + WorldGen.genRand.Next(-20, 21), (int)y + WorldGen.genRand.Next(-20, 21), WorldGen.genRand.Next(4, 25), 5, type, false, WorldGen.genRand.NextFloat(-10, 10), WorldGen.genRand.NextFloat(-10, 10));
                }
            }
        }
    }

    public class Ores : GenPass
    {
        public Ores(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Ores");

            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int[] blocksToReplace = new int[] { TileID.Dirt, TileID.Grass, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.Stone, TileID.Ebonstone, TileID.Crimstone, TileID.SnowBlock, TileID.IceBlock, TileID.Mud, TileID.JungleGrass, TileID.MushroomGrass, TileID.Sand, TileID.HardenedSand, TileID.Sandstone };

            int rarity;
            int type;

            for (int y = 60; y < Main.maxTilesY - 60; y++)
            {
                progress.Set(((float)y - 60) / (Main.maxTilesY - 60 - 60));

                for (int x = 60; x < Main.maxTilesX - 60; x++)
                {
                    Tile tile = Main.tile[x, y];

                    if (biomes.FindBiome(x, y) != BiomeID.Hive && biomes.FindBiome(x, y) != BiomeID.SunkenSea)
                    {
                        float radius = 3;
                        while (radius < 15)
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                break;
                            }
                            else radius++;
                        }

                        if (WorldGen.SolidTile3(x, y) && !Main.wallDungeon[WGTools.Tile(x, y).WallType] && WGTools.Tile(x, y).WallType != ModContent.WallType<GardenBrickWall>() && WGTools.Tile(x, y).WallType != ModContent.WallType<undergrowth>() && WGTools.Tile(x, y).WallType != WallID.LivingWoodUnsafe && WGTools.Tile(x, y).WallType != ModContent.WallType<forgottentomb>() && WGTools.Tile(x, y).WallType != ModContent.WallType<TombBrickWallUnsafe>() && biomes.FindBiome(x, y) != BiomeID.GemCave)
                        {
                            if (y > Main.worldSurface * 0.5f)
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Underworld || biomes.FindBiome(x, y) == BiomeID.AshForest)
                                {
                                    if (y >= Main.maxTilesY - 140)
                                    {
                                        rarity = 10;
                                        OreVein(x, y, WorldGen.genRand.Next(16, 24), rarity, TileID.Hellstone, new int[] { TileID.Ash, TileID.AshGrass, TileID.Obsidian }, 5, 0.55f, 6, 3);
                                    }
                                }
                                else if (x >= 350 && x <= Main.maxTilesX - 350)
                                {
                                    if (y < Main.worldSurface && biomes.FindBiome(x, y) == BiomeID.Desert)
                                    {
                                        rarity = 50;

                                        AddFossil(x, y, rarity);
                                    }
                                    if (y > Main.worldSurface && (biomes.FindBiome(x, y) == BiomeID.Corruption || biomes.FindBiome(x, y) == BiomeID.Crimson))
                                    {
                                        rarity = 5;

                                        type = biomes.FindBiome(x, y) == BiomeID.Crimson ? TileID.Crimtane : TileID.Demonite;

                                        OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 5, 0.55f, 6, 3);
                                    }
                                    else
                                    {
                                        #region tin/copper
                                        if (y < GenVars.lavaLine)
                                        {
                                            rarity = y > Main.worldSurface && y < Main.rockLayer ? 10 : 20;

                                            type = biomes.FindBiome(x, y, false) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Tin : TileID.Copper;

                                            OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                                        }
                                        #endregion

                                        #region iron/lead
                                        if (y > Main.worldSurface)
                                        {
                                            rarity = y > Main.rockLayer && y < GenVars.lavaLine ? 20 : 40;

                                            type = biomes.FindBiome(x, y, false) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Lead : TileID.Iron;

                                            OreVein(x, y, WorldGen.genRand.Next(16, 24), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                                        }
                                        #endregion

                                        #region silver/tungsten
                                        if (y > Main.rockLayer)
                                        {
                                            rarity = y > GenVars.lavaLine ? 20 : 40;

                                            type = biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Tungsten : TileID.Silver;

                                            OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                #region gold/platinum
                                if (biomes.FindBiome(x, y) != BiomeID.Tundra)
                                {
                                    rarity = 5;

                                    type = biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y, false) == BiomeID.Desert ? TileID.Platinum : TileID.Gold; //TileID.Gold;

                                    OreVein(x, y, WorldGen.genRand.Next(12, 18), rarity, type, blocksToReplace, 3, 0.5f, 5, 3);
                                }
                                #endregion
                            }

                            //if (y < Main.worldSurface * 0.35f)
                            //{
                            //    if (tile.TileType == TileID.Cloud || tile.TileType == TileID.RainCloud)
                            //    {
                            //        rarity = 4;

                            //        type = ModContent.TileType<starore>();

                            //        OreVein(x, y, WorldGen.genRand.Next(12, 16), rarity, type, new int[] { TileID.Cloud, TileID.RainCloud }, 5, 0.55f, 6, 3);
                            //    }
                            //    //else
                            //    //{
                            //    //    rarity = 2;

                            //    //    type = TileID.Platinum;

                            //    //    OreVein(x, y, WorldGen.genRand.Next(8, 12), rarity, type, 3, 0.5f, 5, 3);
                            //    //}
                            //}
                        }
                    }
                }
            }
        }

        private void OreVein(int structureX, int structureY, int size, float rarity, int type, int[] blocksToReplace, int steps, float weight = 0.5f, int birthLimit = 4, int deathLimit = 4)
        {
            if (ModContent.GetInstance<Worldgen>().OreFrequency == 0) { return; }

            rarity /= ModContent.GetInstance<Worldgen>().OreFrequency;
            if (WorldGen.genRand.NextBool((int)(rarity * 100)) && !Main.wallDungeon[WGTools.Tile(structureX, structureY).WallType])
            {
                int width = size;
                int height = size;

                structureX -= width / 2;
                structureY -= height / 2;

                bool[,] cellMap = new bool[width, height];
                bool[,] cellMapNew = cellMap;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (WorldGen.genRand.NextFloat(0, 1) <= weight)
                        {
                            cellMap[x, y] = true;
                        }
                        else cellMap[x, y] = false;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    if (WGTools.Tile(structureX, structureY + y).HasTile)
                    {
                        cellMap[0, y] = false;
                    }
                    if (WGTools.Tile(structureX + width - 1, structureY + y).HasTile)
                    {
                        cellMap[width - 1, y] = false;
                    }
                }
                for (int x = 0; x < width; x++)
                {
                    if (WGTools.Tile(structureX + x, structureY).HasTile)
                    {
                        cellMap[x, 0] = false;
                    }
                    if (WGTools.Tile(structureX + x, structureY + height - 1).HasTile)
                    {
                        cellMap[x, height - 1] = false;
                    }
                }

                cellMapNew = cellMap;

                for (int i = 0; i < steps; i++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            bool left = cellMap[x - 1, y];
                            bool right = cellMap[x + 1, y];
                            bool top = cellMap[x, y - 1];
                            bool bottom = cellMap[x, y + 1];
                            bool topleft = cellMap[x - 1, y - 1];
                            bool topright = cellMap[x + 1, y - 1];
                            bool bottomleft = cellMap[x - 1, y + 1];
                            bool bottomright = cellMap[x + 1, y + 1];
                            int adjacentTiles = 0;
                            if (left) { adjacentTiles++; }
                            if (right) { adjacentTiles++; }
                            if (top) { adjacentTiles++; }
                            if (bottom) { adjacentTiles++; }
                            if (topleft) { adjacentTiles++; }
                            if (topright) { adjacentTiles++; }
                            if (bottomleft) { adjacentTiles++; }
                            if (bottomright) { adjacentTiles++; }

                            if (cellMap[x, y] == false && adjacentTiles > birthLimit)
                            {
                                cellMapNew[x, y] = true;
                            }
                            if (cellMap[x, y] && adjacentTiles < deathLimit)
                            {
                                cellMapNew[x, y] = false;
                            }
                        }
                    }
                    cellMap = cellMapNew;
                }

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (cellMap[x, y] && WorldGen.InWorld(x + structureX, y + structureY))
                        {
                            Tile tile = WGTools.Tile(x + structureX, y + structureY);
                            if (tile.HasTile && blocksToReplace.Contains(tile.TileType) && (WGTools.Solid(x + structureX, y + structureY - 1) || structureY > Main.worldSurface))
                            {
                                tile.TileType = (ushort)type;
                            }
                        }
                    }
                }
            }
        }

        private void AddFossil(int x, int y, float rarity)
        {
            if (ModContent.GetInstance<Worldgen>().OreFrequency == 0) { return; }

            rarity /= ModContent.GetInstance<Worldgen>().OreFrequency;
            if (WorldGen.genRand.NextBool((int)(rarity * 100)))
            {
                int lifetime = WorldGen.genRand.Next(12, 25);

                Vector2 position = new Vector2(x, y);
                Vector2 velocity = WorldGen.genRand.NextVector2Circular(1, 1);

                float cooldown = 0;

                while (lifetime > 0)
                {
                    Tile tile = WGTools.Tile(position.X, position.Y);
                    if (tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand)
                    {
                        WGTools.Tile(position.X, position.Y).TileType = TileID.FossilOre;
                    }

                    position += new Vector2(velocity.X, velocity.Y);

                    velocity += Main.rand.NextVector2Circular(10f, 10f) * 0.1f;
                    if (velocity.Length() > 1)
                    {
                        velocity = Vector2.Normalize(velocity) * 1;
                    }

                    cooldown -= velocity.Length();
                    if (cooldown <= 0)
                    {
                        FossilRib(position, Vector2.Normalize(velocity).RotatedBy(-MathHelper.PiOver2));
                        FossilRib(position, Vector2.Normalize(velocity).RotatedBy(MathHelper.PiOver2));
                        cooldown += 3;
                    }

                    lifetime--;
                }
            }
        }

        private void FossilRib(Vector2 position, Vector2 velocity)
        {
            int lifetime = WorldGen.genRand.Next(3, 6);

            while (lifetime > 0)
            {
                Tile tile = WGTools.Tile(position.X, position.Y);
                if (tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand)
                {
                    WGTools.Tile(position.X, position.Y).TileType = TileID.FossilOre;
                }

                position += new Vector2(velocity.X, velocity.Y);


                lifetime--;
            }
        }
    }

    //public class ColonyEntrance : GenPass
    //{
    //    public ColonyEntrance(string name, float loadWeight) : base(name, loadWeight)
    //    {
    //    }

    //    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    //    {
    //        BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

    //        progress.Message = "Sealing off a bee colony";

    //        #region setup
    //        int structureX;
    //        int structureY = (int)Main.rockLayer;
    //        Rectangle structure;

    //        while (true)
    //        {
    //            structureX = (int)(Jungle.Center + Main.rand.NextFloat(-Jungle.Size, Jungle.Size) / 2) * biomes.scale;
    //            structure = new Rectangle(structureX - 21, structureY - 30, 43, 34);
    //            if (GenVars.structures.CanPlace(structure, 25))
    //            {
    //                break;
    //            }
    //        }
    //        #endregion

    //        #region structure
    //        FastNoiseLite noise = new FastNoiseLite();
    //        noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
    //        noise.SetFrequency(0.2f);
    //        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
    //        noise.SetFractalLacunarity(1.75f);

    //        float radius = structure.Width / 1.5f;
    //        //WGTools.Terraform(new Vector2(structureX, structureY), radius, scaleX: 1.5f);
    //        //WGTools.Terraform(new Vector2(structureX, structureY - radius + 10), radius, true, 1.5f);

    //        //WGTools.Rectangle(structure.Left - 2, structureY - 3, structure.Right + 2, structureY, -1, liquid: 0);
    //        //WGTools.Terraform(new Vector2(structureX, structureY - structure.Width / 3), structure.Width / 2, true, scaleX: 1.5f);
    //        //WGTools.Terraform(new Vector2(structureX, structureY + 4), structure.Width / 4, scaleX: 2);

    //        //WGTools.Terraform(new Vector2(structure.Left, structureY - 3), 3.5f, true, scaleX: 1.5f);
    //        //WGTools.Terraform(new Vector2(structure.Right, structureY - 3), 3.5f, true, scaleX: 1.5f);

    //        //WGTools.DrawRectangle(structure.Left - 1, structureY + 1, structure.Right + 1, structureY + 4, TileID.JungleGrass, replace: false);

    //        StructureHelper.API.Generator.GenerateStructure("Content/World/Structures/colony-entrance", new Point16(structure.Left, structure.Top), ModContent.GetInstance<Remnants>());

    //        GenVars.structures.AddProtectedStructure(structure, 25);

    //        WorldGen.PlaceObject(structureX, structureY - 3, ModContent.TileType<hivegateway>());
    //        TileEntity.PlaceEntityNet(structureX - 1, structureY - 6, ModContent.GetInstance<TEhivegateway>().Type);
    //        //TEhivegateway.PlaceEntityNet(structureX - 1, structureY - 3);
    //        #endregion
    //    }
    //}

    //public class Boulders : GenPass
    //{
    //    public Boulders(string name, float loadWeight) : base(name, loadWeight)
    //    {
    //    }

    //    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    //    {
    //        BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

    //        progress.Message = "Placing boulders";

    //        int structures = Main.maxTilesX / 1400;

    //        while (structures > 0)
    //        {
    //            int x = Main.maxTilesX / 2 + WorldGen.genRand.Next(-50, 50) * (Main.maxTilesX / 4200);
    //            int y = (int)(Main.worldSurface * 0.4f);

    //            if (WGTools.Solid(x, y) && WGTools.Tile(x, y).TileType != TileID.Cloud)
    //            {
    //                while (WGTools.Solid(x, y))
    //                {
    //                    y--;
    //                }
    //            }
    //            else while (!WGTools.Solid(x, y) || WGTools.Tile(x, y).TileType == TileID.Cloud)
    //                {
    //                    y++;
    //                }

    //            if (GenVars.structures.CanPlace(new Rectangle(x - 12, y - 16, 25, 33)) && biomes.FindBiome(x, y) != BiomeID.Desert && biomes.FindBiome(x, y) != BiomeID.Corruption && biomes.FindBiome(x, y) != BiomeID.Crimson)
    //            {
    //                float radius = WorldGen.genRand.Next(3, 7);
    //                Boulder(x, y, radius);
    //                structures--;
    //            }
    //        }
    //    }

    //    public static void Boulder(float x, float y, float radius)
    //    {
    //        BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

    //        FastNoiseLite noise = new FastNoiseLite();
    //        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
    //        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
    //        noise.SetFrequency(0.075f);

    //        float xFrequency = 1;
    //        float yFrequency = 1;
    //        float strength = 1;

    //        for (int j = (int)(y - radius * 1.5f / yFrequency); j <= y + radius * 1.5f / yFrequency; j++)
    //        {
    //            for (int i = (int)(x - radius * 1.5f / xFrequency); i <= x + radius * 1.5f / xFrequency; i++)
    //            {
    //                if (noise.GetNoise(i * xFrequency, j * yFrequency) <= (1 - Vector2.Distance(new Vector2((i - x) * xFrequency + x, (j - y) * yFrequency + y), new Vector2(x, y)) / radius) * strength)
    //                {
    //                    Framing.GetTileSafely(i, j).HasTile = true;
    //                    Framing.GetTileSafely(i, j).TileType = biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.IceBlock : TileID.Stone;
    //                    Framing.GetTileSafely(i, j).Slope = 0;
    //                }
    //            }
    //        }
    //    }
    //}

    public class Piles : GenPass
    {
        public Piles(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Piles");

            //Main.tileSolid[229] = false;
            //Main.tileSolid[190] = false;
            //Main.tileSolid[196] = false;
            //Main.tileSolid[189] = false;
            //Main.tileSolid[202] = false;
            //Main.tileSolid[460] = false;

            Main.tileSolid[190] = true;
            Main.tileSolid[192] = true;
            Main.tileSolid[196] = true;
            Main.tileSolid[189] = true;
            Main.tileSolid[202] = true;
            Main.tileSolid[225] = true;
            Main.tileSolid[460] = true;

            int area = (int)(Hive.Size * biomes.scale) * (int)(Hive.Size * biomes.scale);

            int objects = area / 400;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next((int)(Hive.X - Hive.Size) * biomes.scale, (int)(Hive.X + Hive.Size) * biomes.scale);
                int y = WorldGen.genRand.Next((int)(Hive.Y - Hive.Size) * biomes.scale, (int)(Hive.Y + Hive.Size) * biomes.scale);

                if (biomes.FindBiome(x, y) == BiomeID.Hive && WGTools.Tile(x, y - 1).HasTile && !WGTools.Tile(x + 1, y).HasTile && WGTools.Tile(x, y - 1).TileType == TileID.Hive)
                {
                    WorldGen.PlaceObject(x, y, TileID.BeeHive);
                    if (Framing.GetTileSafely(x, y).TileType == TileID.BeeHive)
                    {
                        objects--;
                    }
                }
            }

            Main.tileSolid[ModContent.TileType<SacrificialAltar>()] = true;

            for (int y = 40; y < Main.maxTilesY - 40; y++)
            {
                progress.Set(((float)y - 40) / (Main.maxTilesY - 40 - 40));

                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                    {
                        tile = Main.tile[x, y + 1];
                        if (tile.HasTile && Main.tileSolid[tile.TileType])
                        {
                            if (WorldGen.genRand.NextBool(20) && tile.TileType == TileID.Sandstone)
                            {
                                WorldGen.PlaceObject(x, y, TileID.RollingCactus);
                            }
                            else
                            {
                                if (biomes.FindBiome(x, y) != BiomeID.OceanCave && WorldGen.genRand.NextBool(2) && WGTools.NoDoors(x - 1, y, 3) && WGTools.Tile(x, y + 1).TileType != TileID.Platforms && tile.TileType != ModContent.TileType<SacrificialAltar>())
                                {
                                    LargePile(x, y);
                                }
                                if (WorldGen.genRand.NextBool(4) && WGTools.NoDoors(x, y, 2))
                                {
                                    MediumPile(x, y);
                                }
                                if (WorldGen.genRand.NextBool(6) && WGTools.NoDoors(x, y))
                                {
                                    SmallPile(x, y);
                                }
                            }
                        }
                    }
                }
            }

            Main.tileSolid[ModContent.TileType<SacrificialAltar>()] = false;
        }

        private void LargePile(int x, int y)
        {
            if (!WGTools.Tile(x, y).HasTile && !WGTools.Tile(x - 1, y).HasTile && !WGTools.Tile(x + 1, y).HasTile)
            {
                Tile tile = Main.tile[x, y + 1];
                ushort type = TileID.LargePiles;
                int style = -1;

                if (Main.wallDungeon[tile.WallType] || tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<PyramidBrickWallUnsafe>() || y > Main.maxTilesY - 200 && tile.TileType != TileID.Ash || tile.TileType == TileID.BoneBlock)
                {
                    style = Main.rand.Next(7);
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    type = TileID.LargePiles2;
                    style = Main.rand.Next(9, 14);
                }
                else if (tile.TileType == TileID.Sand && (x < 400 || x > Main.maxTilesX - 400))
                {
                    style = Main.rand.Next(7, 13);
                }
                else if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Stone || tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                {
                    style = Main.rand.Next(7, 16);
                }
                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                {
                    style = Main.rand.Next(26, 32);
                }
                else if (tile.TileType == TileID.MushroomGrass)
                {
                    style = Main.rand.Next(32, 35);
                }
                else
                {
                    type = TileID.LargePiles2;

                    if (tile.TileType == TileID.JungleGrass)
                    {
                        style = Main.rand.Next(3);
                    }
                    else if (tile.TileType == TileID.LivingWood || tile.TileType == TileID.LeafBlock || tile.WallType == WallID.LivingWoodUnsafe)
                    {
                        style = Main.rand.Next(47, 50);
                    }
                    else if (tile.TileType == TileID.Grass)
                    {
                        style = Main.rand.Next(14, 17);
                    }
                    else if (tile.TileType == TileID.Ash)
                    {
                        style = Main.rand.Next(6, 9);
                    }
                    else if (tile.TileType == TileID.Sandstone)
                    {
                        style = Main.rand.Next(29, 35);
                    }
                    else if (tile.TileType == TileID.Granite)
                    {
                        style = Main.rand.Next(35, 41);
                    }
                    else if (tile.TileType == TileID.Marble || tile.TileType == TileID.MarbleBlock)
                    {
                        style = Main.rand.Next(41, 47);
                    }
                    else if (tile.TileType == TileID.Sand && (x > 400 || x < Main.maxTilesX - 400))
                    {
                        style = Main.rand.Next(52, 55);
                    }
                }

                if (style != -1)
                {
                    WorldGen.Place3x2(x, y, type, style);
                }
            }
        }

        private void MediumPile(int x, int y)
        {
            if (!WGTools.Tile(x, y).HasTile && !WGTools.Tile(x + 1, y).HasTile)
            {
                Tile tile = Main.tile[x, y + 1];
                int X = -1;
                int Y = 1;

                if (tile.TileType == ModContent.TileType<SacrificialAltar>())
                {
                    X = Main.rand.Next(11, 16);
                }
                else if (Main.wallDungeon[tile.WallType] || tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<PyramidBrickWallUnsafe>() || y > Main.maxTilesY - 200 || tile.TileType == TileID.BoneBlock)
                {
                    X = Main.rand.Next(6, 16);
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    X = Main.rand.Next(34, 38);
                }
                else if (tile.TileType == TileID.Stone || tile.TileType == TileID.Sand && (x < 400 || x > Main.maxTilesX - 400) || tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                {
                    X = Main.rand.Next(6);
                }
                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                {
                    X = Main.rand.Next(25, 31);
                }
                else if (tile.TileType == TileID.Grass && tile.WallType != WallID.LivingWoodUnsafe)
                {
                    X = Main.rand.Next(38, 41);
                }
                else if (tile.TileType == TileID.Sandstone)
                {
                    X = Main.rand.Next(41, 47);
                }
                else if (tile.TileType == TileID.Granite)
                {
                    X = Main.rand.Next(47, 53);
                }
                else
                {
                    Y = 2;
                    if (tile.TileType == TileID.Marble)
                    {
                        X = Main.rand.Next(6);
                    }
                    else if (tile.TileType == TileID.LivingWood || tile.TileType == TileID.LeafBlock || tile.WallType == WallID.LivingWoodUnsafe)
                    {
                        X = Main.rand.Next(6, 9);
                    }
                    else if (tile.TileType == TileID.Sand)
                    {
                        X = Main.rand.Next(9, 12);
                    }
                }

                if (X != -1)
                {
                    WGTools.MediumPile(x, y, X, Y - 1);
                }
            }
        }

        private void SmallPile(int x, int y)
        {
            if (!WGTools.Tile(x, y).HasTile)
            {
                Tile tile = Main.tile[x, y + 1];
                int X = -1;

                if (tile.TileType == ModContent.TileType<SacrificialAltar>())
                {
                    X = Main.rand.Next(20, 28);
                }
                else if (Main.wallDungeon[tile.WallType] || tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<PyramidBrickWallUnsafe>() && (tile.TileType == TileID.Ash || tile.TileType == TileID.Obsidian || tile.TileType == TileID.BoneBlock))
                {
                    X = Main.rand.NextBool(2) ? Main.rand.Next(12, 28) : Main.rand.Next(28, 36);
                }
                else if (y > Main.maxTilesY - 200)
                {
                    X = Main.rand.Next(12, 28);
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    X = Main.rand.Next(48, 54);
                }
                else if (tile.TileType == TileID.Stone || tile.TileType == TileID.Sand && (x < 400 || x > Main.maxTilesX - 400) || tile.TileType == TileID.BrownMoss || tile.TileType == TileID.GreenMoss || tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss || tile.TileType == TileID.PurpleMoss)
                {
                    X = Main.rand.Next(6);
                }
                else if (tile.TileType == TileID.Dirt)
                {
                    X = Main.rand.Next(6, 12);
                }
                else if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                {
                    X = Main.rand.Next(36, 41);
                }
                else if (tile.TileType == TileID.Sandstone)
                {
                    X = Main.rand.Next(54, 60);
                }
                else if (tile.TileType == TileID.Granite)
                {
                    X = Main.rand.Next(60, 66);
                }
                else if (tile.TileType == TileID.Marble)
                {
                    X = Main.rand.Next(66, 72);
                }
                else if (tile.TileType == TileID.Sand)
                {
                    X = Main.rand.Next(73, 77);
                }

                if (X != -1)
                {
                    WorldGen.PlaceSmallPile(x, y, X, 0);
                }
            }
        }
    }

    public class Moss : GenPass
    {
        public Moss(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Grass");

            for (int y = 40; y < Main.worldSurface; y++)
            {
                for (int x = 40; x < Main.maxTilesX - 40; x++)
                {
                    progress.Set((y - 40) / (float)(Main.worldSurface - 40));

                    if (biomes.FindLayer(x, y) < biomes.surfaceLayer)
                    {
                        if (!WGTools.SurroundingTilesActive(x, y, true))
                        {
                            Tile tile = Main.tile[x, y];

                            if (tile.TileType == TileID.Dirt || tile.TileType == TileID.ClayBlock)
                            {
                                tile.TileType = y > Main.worldSurface / 2 ? biomes.FindBiome(x, y) == BiomeID.Corruption ? TileID.CorruptGrass : biomes.FindBiome(x, y) == BiomeID.Crimson ? TileID.CrimsonGrass : TileID.Grass : TileID.Grass;
                            }
                            else if (tile.TileType == TileID.Mud)
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Jungle)
                                {
                                    tile.TileType = TileID.JungleGrass;
                                }
                            }
                            if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe)
                            {
                                tile.WallType = y > Main.worldSurface / 2 ? biomes.FindBiome(x, y) == BiomeID.Corruption ? WallID.CorruptGrassUnsafe : biomes.FindBiome(x, y) == BiomeID.Crimson ? WallID.CrimsonGrassUnsafe : WallID.GrassUnsafe : WallID.GrassUnsafe;
                            }
                            if (tile.WallType == WallID.GrassUnsafe && biomes.MaterialBlend(x, y, frequency: 2) < -0.2f)
                            {
                                tile.WallType = WallID.RocksUnsafe1;
                            }
                        }
                    }
                }
            }

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Grass");

            int[] mossToChoose = new int[5];
            mossToChoose[0] = TileID.GreenMoss;
            mossToChoose[1] = TileID.BrownMoss;
            mossToChoose[2] = TileID.RedMoss;
            mossToChoose[3] = TileID.BlueMoss;
            mossToChoose[4] = TileID.PurpleMoss;

            int[] moss = new int[3];
            int[] mossWall = new int[3];
            int[] mossBrick = new int[3];
            int index = 0;
            while (index < 3)
            {
                int randomMoss = mossToChoose[WorldGen.genRand.Next(mossToChoose.Length)];
                if (!moss.Contains(randomMoss))
                {
                    moss[index] = randomMoss;
                    mossWall[index] = randomMoss == TileID.GreenMoss ? WallID.CaveUnsafe : randomMoss == TileID.BrownMoss ? WallID.Cave2Unsafe : randomMoss == TileID.RedMoss ? WallID.Cave3Unsafe : randomMoss == TileID.BlueMoss ? WallID.Cave4Unsafe : WallID.Cave5Unsafe;
                    mossBrick[index] = randomMoss == TileID.GreenMoss ? TileID.GreenMossBrick : randomMoss == TileID.BrownMoss ? TileID.BrownMossBrick : randomMoss == TileID.RedMoss ? TileID.RedMossBrick : randomMoss == TileID.BlueMoss ? TileID.BlueMossBrick : TileID.PurpleMossBrick;
                    index++;
                }
            }

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetFrequency(0.1f);

            int structureCount = 0;
            while (structureCount < Main.maxTilesX * (Main.maxTilesY - 200 - GenVars.lavaLine) / 100000)
            {
                progress.Set(structureCount / (float)(Main.maxTilesX * (Main.maxTilesY - 200 - GenVars.lavaLine) / 100000) / 2);

                int x = WorldGen.genRand.Next(40, Main.maxTilesX - 200);
                int y = WorldGen.genRand.Next(Math.Min(GenVars.lavaLine, Main.maxTilesY - 201), Main.maxTilesY - 200);

                if (biomes.FindBiome(x, y) == BiomeID.None && !WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).LiquidAmount == 255 && WGTools.Tile(x, y).LiquidType == 1)
                {
                    int radius = WorldGen.genRand.Next(30, 90);

                    for (int j = Math.Max(y - radius, GenVars.lavaLine); j <= y + radius; j++)
                    {
                        for (int i = Math.Max(x - radius, 40); i <= Math.Min(x + radius, Main.maxTilesX - 40); i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + (noise.GetNoise(i, j) + 1) * 5 < radius && WorldGen.InWorld(i, j) && biomes.FindBiome(i, j) == BiomeID.None && !WGTools.SurroundingTilesActive(i, j, true))
                            {
                                Tile tile = Main.tile[i, j];

                                if (tile.HasTile && (tile.TileType == TileID.Stone || tile.TileType == TileID.GrayBrick))
                                {
                                    tile.TileType = tile.TileType == TileID.GrayBrick ? TileID.LavaMossBrick : TileID.LavaMoss;
                                }

                                if (tile.WallType == WallID.Cave8Unsafe)
                                {
                                    tile.WallType = WallID.LavaUnsafe2;
                                }
                            }
                        }
                    }

                    structureCount++;
                }
            }

            structureCount = 0;
            while (structureCount < Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 40000)
            {
                progress.Set(structureCount / (float)(Main.maxTilesX * (Main.maxTilesY - 200 - (int)Main.rockLayer) / 40000) / 2 + 0.5f);

                int x = WorldGen.genRand.Next(40, Main.maxTilesX - 40);
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);

                if (biomes.FindBiome(x, y, false) == BiomeID.None && !WGTools.Tile(x, y).HasTile && WGTools.Tile(x, y).LiquidAmount == 255 && WGTools.Tile(x, y).LiquidType == 0)
                {
                    int radius = WorldGen.genRand.Next(25, 75);

                    for (int j = Math.Max(y - radius, (int)Main.rockLayer); j <= y + radius; j++)
                    {
                        for (int i = Math.Max(x - radius, 40); i <= Math.Min(x + radius, Main.maxTilesX - 40); i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + (noise.GetNoise(i, j) + 1) * 5 < radius && WorldGen.InWorld(i, j) && biomes.FindBiome(i, j) == BiomeID.None && !WGTools.SurroundingTilesActive(i, j, true))
                            {
                                Tile tile = Main.tile[i, j];
                                int mossType = x + (noise.GetNoise(i, j) + 1) * 10 <= Main.maxTilesX / 3 + Math.Cos(j / (Main.maxTilesY / 120f)) * (Main.maxTilesX / 200f) ? 0 : x <= Main.maxTilesX / 1.5f + Math.Cos(j / (Main.maxTilesY / 120f)) * (Main.maxTilesX / 200f) ? 1 : 2;

                                if (tile.HasTile && (tile.TileType == TileID.Stone || tile.TileType == TileID.GrayBrick))
                                {
                                    tile.TileType = tile.TileType == TileID.GrayBrick ? (ushort)mossBrick[mossType] : (ushort)moss[mossType];
                                }
                                if (tile.WallType == WallID.RocksUnsafe1 || tile.WallType == WallID.Cave8Unsafe)
                                {
                                    tile.WallType = (ushort)mossWall[mossType];
                                }
                            }
                        }
                    }

                    structureCount++;
                }
            }
        }
    }

    public class WorldCleanup : GenPass
    {
        public WorldCleanup(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Touchups");

            Main.tileSolid[162] = false;
            Main.tileSolid[226] = true;
            Main.tileSolid[232] = true;

            Main.tileSolid[TileID.BreakableIce] = true;

            for (int x = 20; x < Main.maxTilesX - 20; x++)
            {
                progress.Set(x / (float)Main.maxTilesX);

                for (int y = 20; y < Main.maxTilesY - 20; y++)
                {
                    Tile tile = WGTools.Tile(x, y);

                    if (biomes.FindBiome(x, y) == BiomeID.Tundra && (!tile.HasTile || !Main.tileSolid[tile.TileType]) && y > Main.worldSurface * 0.5f && !Main.wallDungeon[tile.WallType])
                    {
                        if (Framing.GetTileSafely(x, y).LiquidAmount > 0 && Framing.GetTileSafely(x, y - 1).LiquidAmount == 0 && !WGTools.Solid(x, y - 1) && Framing.GetTileSafely(x - 1, y - 1).LiquidAmount == 0 && Framing.GetTileSafely(x + 1, y - 1).LiquidAmount == 0)
                        {
                            WorldGen.KillTile(x, y);
                            WorldGen.PlaceTile(x, y, TileID.BreakableIce);
                        }
                    }

                    if (tile.HasTile)
                    {
                        if (tile.TileType == ModContent.TileType<Tiles.nothing>())
                        {
                            tile.TileType = TileID.Dirt;
                            WorldGen.KillTile(x, y);
                        }
                        #region platformfix
                        if (tile.TileType == TileID.Platforms)
                        {
                            tile.IsHalfBlock = false;
                            tile.Slope = 0;
                            if (tile.TileFrameX == 10 * 18 || tile.TileFrameX == 20 * 18 || tile.TileFrameX == 22 * 18 || tile.TileFrameX == 24 * 18 || tile.TileFrameX == 26 * 18)
                            {
                                //WorldGen.KillTile(x, y, true);
                                tile.Slope = SlopeType.SlopeDownLeft;
                            }
                            if (tile.TileFrameX == 8 * 18 || tile.TileFrameX == 19 * 18 || tile.TileFrameX == 21 * 18 || tile.TileFrameX == 23 * 18 || tile.TileFrameX == 25 * 18)
                            {
                                //WorldGen.KillTile(x, y, true);
                                tile.Slope = SlopeType.SlopeDownRight;
                            }
                        }
                        #endregion
                        else
                        {
                            if (tile.TileType == TileID.Plants || tile.TileType == TileID.Plants2)
                            {
                                if (y < Main.worldSurface * 0.5f)
                                {
                                    tile.TileFrameX = (short)(Main.rand.Next(6, 20) * 18);
                                    if (tile.TileFrameX >= 8 * 18)
                                    {
                                        tile.TileFrameX++;
                                    }
                                }
                            }
                            else if (tile.TileType == TileID.MinecartTrack)
                            {
                                WGTools.Tile(x, y + 2).LiquidAmount = 0;

                                if (tile.TileFrameX == 1 && tile.TileFrameY == -1)
                                {
                                    bool left = !WGTools.Tile(x - 1, y).HasTile || WGTools.Tile(x - 1, y).TileType != TileID.MinecartTrack;
                                    bool right = !WGTools.Tile(x + 1, y).HasTile || WGTools.Tile(x + 1, y).TileType != TileID.MinecartTrack;
                                    if (left && right)
                                    {
                                        tile.TileFrameX = 0;
                                    }
                                    else if (left)
                                    {
                                        tile.TileFrameX = 14;
                                    }
                                    else if (right)
                                    {
                                        tile.TileFrameX = 15;
                                    }
                                }
                            }
                            else if (tile.TileType == TileID.WoodBlock || tile.TileType == TileID.GrayBrick || TileID.Sets.tileMossBrick[tile.TileType] || tile.TileType == TileID.Glass && tile.WallType == ModContent.WallType<magicallab>() || tile.TileType == ModContent.TileType<LockedIronDoor>() || tile.TileType == ModContent.TileType<VaultPipe>() || ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("IndustrialPanel", out ModTile IndustrialPanel) && tile.TileType == IndustrialPanel.Type)
                            {
                                tile.Slope = 0;
                                tile.IsHalfBlock = false;
                            }
                            else if (tile.TileType == TileID.Pots && WGTools.Tile(x - 1, y).TileType != TileID.Pots && WGTools.Tile(x, y - 1).TileType != TileID.Pots && biomes.FindBiome(x, y) == BiomeID.Tundra)
                            {
                                int style = Main.rand.Next(4, 7);
                                for (int j = 0; j < 2; j++)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        WGTools.Tile(x + i, y + j).TileFrameY = (short)(style * 36 + j * 18);
                                    }
                                }
                            }
                            else if (tile.TileType == TileID.DemonAltar)
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Crimson && tile.TileFrameX < 18 * 3)
                                {
                                    tile.TileFrameX += 18 * 3;
                                }
                                else if (biomes.FindBiome(x, y) == BiomeID.Corruption && tile.TileFrameX >= 18 * 3)
                                {
                                    tile.TileFrameX -= 18 * 3;
                                }
                            }
                            else if (tile.TileType == TileID.ShadowOrbs)
                            {
                                if (biomes.FindBiome(x, y) == BiomeID.Crimson && tile.TileFrameX < 18 * 2)
                                {
                                    tile.TileFrameX += 18 * 2;
                                }
                                else if (biomes.FindBiome(x, y) == BiomeID.Corruption && tile.TileFrameX >= 18 * 2)
                                {
                                    tile.TileFrameX -= 18 * 2;
                                }
                            }
                            else if (tile.TileType == ModContent.TileType<ArcaneChest>())
                            {
                                if (Main.tile[x - 1, y].TileType != ModContent.TileType<ArcaneChest>())
                                {
                                    WGTools.Tile(x + 3, y).HasTile = false;
                                }
                            }

                            //if (biomes.FindBiome(x, y) == BiomeID.OceanCave && tile.TileType == TileID.HardenedSand)
                            //{
                            //    tile.TileType = TileID.ArgonMoss;
                            //}

                            //if (tile.TileType == TileID.Dirt && y <= Main.worldSurface)
                            //{
                            //    tile.TileType = TileID.Grass;
                            //}
                            //else if (tile.TileType == TileID.JungleGrass)
                            //{
                            //    if (biomes.FindBiome(x, y) == BiomeID.Glowshroom && !Main.wallDungeon[tile.WallType])
                            //    {
                            //        tile.TileType = TileID.MushroomGrass;
                            //    }
                            //}

                            if (WGTools.SurroundingTilesActive(x, y, true))
                            {
                                if (tile.TileType == TileID.Grass)
                                {
                                    if (biomes.FindBiome(x, y) == BiomeID.Corruption)
                                    {
                                        tile.TileType = TileID.CorruptGrass;
                                    }
                                    else if (biomes.FindBiome(x, y) == BiomeID.Crimson)
                                    {
                                        tile.TileType = TileID.CrimsonGrass;
                                    }
                                }
                                else if (tile.TileType == TileID.LivingWood)
                                {
                                    tile.WallType = WallID.LivingWoodUnsafe;
                                }
                                else if (tile.TileType == TileID.LeafBlock)
                                {
                                    tile.WallType = WallID.LivingLeaf;
                                }
                                else if (tile.TileType == ModContent.TileType<Hardstone>() && tile.WallType == 0)
                                {
                                    tile.WallType = (ushort)ModContent.WallType<hardstonewall>();
                                }

                                if (tile.WallType == WallID.GrassUnsafe)
                                {
                                    tile.WallType = WallID.DirtUnsafe;
                                }
                                else if (tile.WallType == WallID.JungleUnsafe || tile.WallType == WallID.MushroomUnsafe)
                                {
                                    tile.WallType = WallID.MudUnsafe;
                                }
                            }

                            //if (tile.IsHalfBlock && WorldGen.SolidTile3(x, y - 1))
                            //{
                            //    tile.IsHalfBlock = false;
                            //}
                        }
                    }
                    else
                    {
                        if (biomes.FindBiome(x, y) == BiomeID.Hive && WorldGen.genRand.NextBool(4))
                        {
                            bool top = false;
                            bool bottom = false;
                            if (RemTile.SolidBottom(x, y - 1) && WGTools.Tile(x, y - 1).TileType == TileID.Hive)
                            {
                                top = true;
                            }
                            if (RemTile.SolidTop(x, y + 1) && WGTools.Tile(x, y + 1).TileType == TileID.Hive)
                            {
                                bottom = true;
                            }
                            if (top || bottom)
                            {
                                //WorldGen.PlaceTight(x, y);
                                tile.HasTile = true;
                                tile.TileType = TileID.Stalactite;
                                tile.TileFrameX = (short)(WorldGen.genRand.Next(9, 12) * 18);

                                if (top)
                                {
                                    tile.TileFrameY = 4 * 18;
                                }
                                else if (bottom)
                                {
                                    tile.TileFrameY = 5 * 18;
                                }
                            }
                        }
                        if (biomes.FindBiome(x, y) == BiomeID.Marble && WorldGen.genRand.NextBool(5))
                        {
                            if (WGTools.Tile(x, y - 1).HasTile && WGTools.Tile(x, y - 1).TileType == TileID.Marble)
                            {
                                WorldGen.PlaceTile(x, y, TileID.WaterDrip);
                            }
                        }

                        if (WGTools.Tile(x, y + 24).HasTile && WGTools.Tile(x, y + 24).TileType == ModContent.TileType<LabyrinthBrick>())
                        {
                            tile.LiquidAmount = 0;
                        }
                    }

                    if (tile.WallType != 0)
                    {
                        if (tile.WallType == ModContent.WallType<Walls.dev.nothing>())
                        {
                            tile.WallType = 0;
                        }
                        //else if (!WGTools.SurroundingTilesActive(x, y, true) && Framing.GetTileSafely(x + 1, y).WallType == 0 && Framing.GetTileSafely(x, y + 1).WallType == 0 && Framing.GetTileSafely(x - 1, y).WallType == 0 && Framing.GetTileSafely(x, y - 1).WallType == 0)
                        //{
                        //    if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.GrassUnsafe || tile.WallType == WallID.FlowerUnsafe || tile.WallType == WallID.SnowWallUnsafe || tile.WallType == WallID.JungleUnsafe)
                        //    {
                        //        tile.WallType = 0;
                        //    }
                        //}
                        if (tile.WallType == WallID.GrassUnsafe && y <= Main.worldSurface * 0.5f)
                        {
                            tile.WallType = WallID.FlowerUnsafe;
                        }
                        else if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe || tile.WallType == WallID.GrassUnsafe || tile.WallType == WallID.FlowerUnsafe)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.Jungle)
                            {
                                tile.WallType = biomes.FindLayer(x, y) < biomes.surfaceLayer && biomes.MaterialBlend(x, y, frequency: 2) < -0.2f ? WallID.JungleUnsafe3 : WallID.JungleUnsafe;
                            }
                            else if (biomes.FindBiome(x, y) == BiomeID.Tundra && y > Main.worldSurface * 0.5f)
                            {
                                tile.WallType = WallID.SnowWallUnsafe;
                            }
                            else if ((tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.Cave6Unsafe) && biomes.FindBiome(x, y) == BiomeID.Desert)
                            {
                                tile.WallType = WallID.HardenedSand;
                            }
                            else if (tile.TileType == TileID.Stone && tile.WallType == WallID.DirtUnsafe)
                            {
                                tile.WallType = WallID.Cave6Unsafe;
                            }
                        }
                        else if (tile.WallType == WallID.RocksUnsafe1)
                        {
                            if (biomes.FindBiome(x, y) == BiomeID.Jungle)
                            {
                                tile.WallType = WallID.JungleUnsafe3;
                            }
                        }
                        else if (y > Main.maxTilesY - 200 && tile.WallType == WallID.HellstoneBrickUnsafe)
                        {
                            tile.LiquidAmount = 255;
                        }
                        else if ((WGTools.Tile(x + 1, y).WallType == WallID.HardenedSand && WGTools.Tile(x - 1, y).WallType == WallID.Sandstone || WGTools.Tile(x - 1, y).WallType == WallID.HardenedSand && WGTools.Tile(x + 1, y).WallType == WallID.Sandstone) && tile.WallType == 0)
                        {
                            tile.WallType = WallID.HardenedSand;
                        }

                        if (tile.WallType == WallID.SnowWallUnsafe && biomes.FindLayer(x, y) < biomes.surfaceLayer && biomes.MaterialBlend(x, y, frequency: 2) < 0.2f)
                        {
                            tile.WallType = WallID.IceUnsafe;
                        }

                        //if (biomes.FindLayer(x, y) >= biomes.lavaLayer)
                        //{
                        //    if (tile.WallType == WallID.JungleUnsafe || tile.WallType == WallID.MushroomUnsafe)
                        //    {
                        //        tile.WallType = WallID.MudUnsafe;
                        //    }
                        //}

                        //if (WGTools.SurroundingTilesActive(x, y, true) || tile.LiquidAmount == 255 || Framing.GetTileSafely(x + 1, y).LiquidAmount == 255 || Framing.GetTileSafely(x - 1, y).LiquidAmount == 255 || Framing.GetTileSafely(x, y - 1).LiquidAmount == 255 || Framing.GetTileSafely(x - 1, y - 1).LiquidAmount == 255 || Framing.GetTileSafely(x + 1, y - 1).LiquidAmount == 255)
                        //{
                        //    if (tile.WallType == WallID.GrassUnsafe || tile.WallType == WallID.FlowerUnsafe)
                        //    {
                        //        tile.WallType = WallID.DirtUnsafe;
                        //    }
                        //    else if (tile.WallType == WallID.JungleUnsafe)
                        //    {
                        //        tile.WallType = WallID.MudUnsafe;
                        //    }
                        //}

                        //if (biomes.FindBiomeEfficient(x, y, BiomeID.Glowshroom))
                        //{
                        //    if (WGTools.SurroundingTilesActive(x, y, true) && (tile.wall == 0 || tile.wall == WallID.MushroomUnsafe))
                        //    {
                        //        tile.wall = WallID.MudUnsafe;
                        //    }
                        //}
                        //else 
                    }

                    if (tile.WallType == WallID.LihzahrdBrickUnsafe || tile.WallType == ModContent.WallType<temple>() || tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>() || tile.WallType == ModContent.WallType<whisperingmaze>() || tile.WallType == ModContent.WallType<LabyrinthTileWall>() || tile.WallType == ModContent.WallType<LabyrinthBrickWall>())
                    {
                        tile.LiquidAmount = 0;
                        if (tile.TileType == TileID.WaterDrip || tile.TileType == TileID.LavaDrip)
                        {
                            tile.HasTile = false;
                        }
                    }
                    if (biomes.FindBiome(x, y) == BiomeID.Hive || biomes.FindBiome(x, y, false) == BiomeID.Underworld || biomes.FindBiome(x, y) == BiomeID.Aether || tile.WallType == ModContent.WallType<whisperingmaze>() || tile.WallType == ModContent.WallType<LabyrinthTileWall>() || tile.WallType == ModContent.WallType<LabyrinthBrickWall>())
                    {
                        if (tile.TileType == TileID.Cobweb)
                        {
                            tile.HasTile = false;
                        }
                    }
                    else if (biomes.FindBiome(x, y) == BiomeID.Glowshroom || biomes.FindBiome(x, y) == BiomeID.Desert || biomes.FindBiome(x, y) == BiomeID.SunkenSea || biomes.FindBiome(x, y) == BiomeID.Marble || biomes.FindBiome(x, y) == BiomeID.Granite || biomes.FindBiome(x, y) == BiomeID.Corruption || biomes.FindBiome(x, y) == BiomeID.Crimson || biomes.FindBiome(x, y) == BiomeID.Beach && y < Main.worldSurface + 50)
                    {
                        tile.LiquidType = 0;
                        if (tile.TileType == TileID.Cobweb || tile.TileType == TileID.Obsidian)
                        {
                            tile.HasTile = false;
                            if (tile.TileType == TileID.Obsidian)
                            {
                                tile.LiquidAmount = 255;
                            }
                        }
                        if (tile.TileType == TileID.LavaDrip)
                        {
                            tile.TileType = TileID.WaterDrip;
                        }
                    }
                    //    if (!tile.HasTile)
                    //    {
                    //        if (WorldGen.genRand.NextBool(20) && RemTile.SolidBottom(x, y - 1))
                    //        {
                    //            WorldGen.PlaceTile(x, y, TileID.DyePlants, style: 7);
                    //        }
                    //        else if (WorldGen.genRand.NextBool(4) && RemTile.SolidTop(x, y + 1) && RemTile.SolidTop(x + 1, y + 1) && !WGTools.GetTile(x + 1, y).HasTile)
                    //        {
                    //            WGTools.MediumPile(x, y, Main.rand.Next(6, 16));
                    //        }
                    //        else if (WorldGen.genRand.NextBool(2) && RemTile.SolidTop(x, y + 1) && !tile.HasTile)
                    //        {
                    //            RemTile.SmallPile(x, y, Main.rand.Next(12, 28));
                    //        }
                    //    }
                    //}
                    //if (biomes.FindBiome(x, y, false) == BiomeID.Glowshroom)
                    //{
                    //    if (WorldGen.genRand.NextBool(2) && RemTile.SolidTop(x, y + 1) && (WGTools.GetTile(x, y + 1).type == ModContent.TileType<mudstone>() || WGTools.GetTile(x, y + 1).type == TileID.Silt))
                    //    {
                    //        if (WorldGen.genRand.NextBool(2))
                    //        {
                    //            WorldGen.PlaceTile(x, y, TileID.DyePlants);
                    //        }
                    //        else WorldGen.PlaceTile(x, y, TileID.DyePlants, style: 1);
                    //    }
                    //}

                }
            }

            #region lihzahrdaltar
            for (int x = 0; x <= 2; x++)
            {
                WGTools.Tile(GenVars.lAltarX + x, GenVars.lAltarY + 1).HasTile = true;
                WGTools.Tile(GenVars.lAltarX + x, GenVars.lAltarY + 1).TileType = TileID.LihzahrdBrick;

                for (int y = 0; y <= 1; y++)
                {
                    //WorldGen.KillTile(GenVars.lAltarX + x, GenVars.lAltarY + y);
                    Tile tile = WGTools.Tile(GenVars.lAltarX + x, GenVars.lAltarY + y);

                    tile.HasTile = true;
                    tile.TileType = TileID.LihzahrdAltar;
                    tile.TileFrameX = (short)(x * 18);
                    tile.TileFrameY = (short)(y * 18);
                }
            }
            WorldGen.KillTile(GenVars.lAltarX - 1, GenVars.lAltarY + 1);
            WorldGen.PlaceTile(GenVars.lAltarX - 1, GenVars.lAltarY + 2, TileID.LihzahrdBrick);
            WorldGen.PlaceTile(GenVars.lAltarX - 1, GenVars.lAltarY + 1, TileID.Torches, style: 6);
            WorldGen.KillTile(GenVars.lAltarX + 3, GenVars.lAltarY + 1);
            WorldGen.PlaceTile(GenVars.lAltarX + 3, GenVars.lAltarY + 2, TileID.LihzahrdBrick);
            WorldGen.PlaceTile(GenVars.lAltarX + 3, GenVars.lAltarY + 1, TileID.Torches, style: 6);

            if (ModLoader.TryGetMod("CalamityMod", out Mod cal))
            {
                for (int y = GenVars.lAltarY - 58; y <= GenVars.lAltarY + 2; y++)
                {
                    for (int x = GenVars.lAltarX - 78; x <= GenVars.lAltarX + 80; x++)
                    {
                        if (Main.tile[x, y].WallType == ModContent.WallType<temple>())
                        {
                            Main.tile[x, y].WallType = WallID.LihzahrdBrickUnsafe;
                        }
                    }
                }
            }
            #endregion

            for (int y = (int)Main.worldSurface - 40; y < Main.maxTilesY - 200; y++)
            {
                for (int x = GenVars.UndergroundDesertLocation.Left; x <= GenVars.UndergroundDesertLocation.Right; x++)
                {
                    if (!WGTools.Tile(x, y).HasTile && WorldGen.genRand.NextBool(30))
                    {
                        if (WGTools.Tile(x, y).WallType == WallID.Sandstone)
                        {
                            if (WGTools.AdjacentTiles(x, y, true) >= 1)
                            {
                                WorldGen.PlaceTile(x, y, TileID.ExposedGems, style: 6);
                            }
                        }
                    }
                }
            }

            for (int y = (int)Main.worldSurface; y < Main.rockLayer; y++)
            {
                for (int x = 20; x < Main.maxTilesX - 20; x++)
                {
                    if (biomes.FindBiome(x, y) == BiomeID.OceanCave)
                    {
                        if (!WGTools.Tile(x, y).HasTile && WorldGen.SolidTile(WGTools.Tile(x, y + 1)) && WGTools.Tile(x, y).LiquidAmount == 255)
                        {
                            if (WGTools.Tile(x, y + 1).TileType == TileID.Coralstone)
                            {
                                WorldGen.PlaceTile(x, y, TileID.Coral, style: Main.rand.Next(6));
                            }
                            else if (WorldGen.genRand.NextBool(15))
                            {
                                WorldGen.PlaceTile(x, y, TileID.DyePlants, style: 5);
                            }
                        }
                    }
                }
            }
        }
    }

    public class SpawnPointFix : GenPass
    {
        public SpawnPointFix(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.Remnants.WorldgenMessages.Spawnpoint");

            Main.spawnTileX = Main.maxTilesX / 2 + (WorldGen.genRand.NextBool(2) ? -25 : 25);
            if (Main.spawnTileY < Main.worldSurface * 0.5f)
            {
                Main.spawnTileY = (int)(Main.worldSurface * 0.5f);
            }

            if (SolidSpawn())
            {
                while (SolidSpawn())
                {
                    Main.spawnTileY--;
                }
                Main.spawnTileY++;
            }
            else
            {
                while (!SolidSpawn())
                {
                    Main.spawnTileY++;
                }
            }
        }

        private bool SolidSpawn()
        {
            for (int i = Main.spawnTileX - 1; i <= Main.spawnTileX + 1; i++)
            {
                Tile tile = WGTools.Tile(i, Main.spawnTileY + 1);
                if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.LivingWood && tile.TileType != TileID.LeafBlock)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
