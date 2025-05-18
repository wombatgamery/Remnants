using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using System.Linq;
using static Remnants.Content.World.BiomeGeneration;
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
using Terraria.GameContent.Generation;
using System.Threading;
using static Remnants.Content.World.BiomeMap;
using SteelSeries.GameSense;
using Remnants.Content.Walls.Vanity;
using Terraria.DataStructures;

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
            On_WorldGen.ShimmerCleanUp += CancelShimmerCleanup;
        }

        private void CancelShimmerCleanup(On_WorldGen.orig_ShimmerCleanUp orig)
        {
            return;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex;

            bool spiritReforged = ModLoader.TryGetMod("SpiritReforged", out Mod sr);

            #region biomes
            InsertPass(tasks, new BiomeGeneration("Biomes", 1), FindIndex(tasks, "Generate Ice Biome"), true);
            RemovePass(tasks, FindIndex(tasks, "Jungle")); RemovePass(tasks, FindIndex(tasks, "Muds Walls In Jungle"));
            RemovePass(tasks, FindIndex(tasks, "Full Desert")); RemovePass(tasks, FindIndex(tasks, "Dunes"));

            //InsertPass(tasks, new Undergrowth("Undergrowth", 100), FindIndex(tasks, "Tunnels") + 1);

            //InsertPass(tasks, new Hell("Underworld", 0), FindIndex(tasks, "Underworld"), true);
            RemovePass(tasks, FindIndex(tasks, "Underworld"));
            RemovePass(tasks, FindIndex(tasks, "Corruption"));

            //InsertPass(tasks, new Corruption("Corruption", 0), FindIndex(tasks, "Terrain") + 2);
            //InsertPass(tasks, new Corruption("Infection", 0), FindIndex(tasks, "Dungeon") + 1);

            //InsertPass(tasks, new SecondaryBiomes("Secondary Biomes", 20), FindIndex(tasks, "Mud Caves To Grass") + 1);
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

            if (spiritReforged)
            {
                RemovePass(tasks, FindIndex(tasks, "Beaches"), true);
                RemovePass(tasks, FindIndex(tasks, "Create Ocean Caves"), true);
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

            if (ModLoader.TryGetMod("Consolaria", out Mod Console))
            {
                RemovePass(tasks, FindIndex(tasks, "Jungle Sanctum"), true);
                RemovePass(tasks, FindIndex(tasks, "Heart Shrine"), true);
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

        public static int FindIndex(List<GenPass> tasks, string value, bool last = false)
        {
            return last ? tasks.FindLastIndex(genpass => genpass.Name.Equals(value)) : tasks.FindIndex(genpass => genpass.Name.Equals(value));
        }

        public static Tile Tile(int x, int y)
        {
            return Framing.GetTileSafely(x, y);
        }
    }

    public class MiscTools : ModSystem
    {
        public static Tile Tile(float x, float y)
        {
            return Framing.GetTileSafely((int)x, (int)y);
        }

        public static bool HasTile(int x, int y, int type)
        {
            return Main.tile[x, y].TileType == type && Main.tile[x, y].HasTile;
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

                if (Tile(x, y + 1).HasTile && Tile(x, y + 1).TileType == TileID.Cobweb || Tile(x, y + 1).TileType == ModContent.TileType<nothing>())
                {
                    WorldGen.KillTile(x, y + 1);
                }
                if (Tile(x, y + 1).HasTile)
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

                    if (Framing.GetTileSafely(x, y).TileType != tile && !HasTile(x, y + 1, ModContent.TileType<nothing>()))
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

    public class StructureTools
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

                targetCell = Point.Zero;
            }

            public int X;
            public int Y;

            int _roomWidth;
            int _roomHeight;

            public Point targetCell;
            public bool[,,] layout;



            public Point16 roomPos => new Point16((int)(X + targetCell.X * _roomWidth), (int)(Y + targetCell.Y * _roomHeight));

            public Rectangle room => new Rectangle(roomPos.X, roomPos.Y, _roomWidth, _roomHeight);

            public Rectangle grid;

            public Rectangle area => new Rectangle(X, Y, _roomWidth * grid.Width, _roomHeight * grid.Height);

            public void AddMarker(int cellX, int cellY, int layer = 0)
            {
                if (cellX < grid.Left || cellX >= grid.Right || cellY < grid.Top || cellY >= grid.Bottom)
                {
                    return;
                }
                layout[cellX, cellY, layer] = true;
            }
            public bool FindMarker(int cellX, int cellY, int layer = 0)
            {
                if (cellX < grid.Left || cellX >= grid.Right || cellY < grid.Top || cellY >= grid.Bottom)
                {
                    return layer == 0;
                }
                else return layout[cellX, cellY, layer];
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
                else return open ^ FindMarker(i, j, (direction + 1) % 4 + 1);
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

            bool magicalLab = MiscTools.Tile(x, y).WallType == ModContent.WallType<EnchantedBrickWallUnsafe>() || MiscTools.Tile(x, y).WallType == ModContent.WallType<magicallab>();
            bool pyramid = MiscTools.Tile(x, y).WallType == ModContent.WallType<PyramidBrickWallUnsafe>() || MiscTools.Tile(x, y).WallType == ModContent.WallType<pyramid>();
            bool tomb = MiscTools.Tile(x, y).WallType == ModContent.WallType<TombBrickWallUnsafe>() || MiscTools.Tile(x, y).WallType == ModContent.WallType<forgottentomb>();
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
                        itemsToAdd.Add((tomb ? ItemID.WebRope : Main.wallDungeon[MiscTools.Tile(x, y).WallType] || y >= Main.maxTilesY - 200 ? ItemID.Chain : ItemID.Rope, Main.rand.Next(50, 100)));

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
                    itemsToAdd.Add((MiscTools.Tile(x, y).LiquidAmount > 0 ? ItemID.Glowstick : magicalLab ? ItemID.ShimmerTorch : biomes.FindBiome(x, y) == BiomeID.Tundra ? ItemID.IceTorch : biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Hive ? ItemID.JungleTorch : biomes.FindBiome(x, y) == BiomeID.Desert ? ItemID.DesertTorch : biomes.FindBiome(x, y) == BiomeID.Glowshroom ? ItemID.MushroomTorch : ItemID.Torch, grade > 0 ? Main.rand.Next(10, 20) : Main.rand.Next(2, 5)));
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
                for (int k = 0; k < (MiscTools.Tile(x, y).LiquidAmount > 0 ? Main.rand.Next(6, 12) : Main.rand.Next(0, 6)); k++)
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
                for (int x = left - radius; x <= right + radius; x = x == left - 1 && y >= top && y <= bottom ? right + 1 : x + 1)
                {
                    Vector2 point = new Vector2(MathHelper.Clamp(x, left, right), MathHelper.Clamp(y, top, bottom));

                    float noise = borders.GetNoise(x, y) / 2 + 0.5f;
                    if (noise > Vector2.Distance(new Vector2(x, y), point) / (radius / 2) && innerTile != -1)
                    {
                        MiscTools.Tile(x, y).HasTile = true;
                        MiscTools.Tile(x, y).TileType = (ushort)innerTile;
                    }
                    else if (!MiscTools.Tile(x, y).HasTile || MiscTools.Tile(x, y).TileType != innerTile && MiscTools.Tile(x, y).TileType != ModContent.TileType<Hardstone>())
                    {
                        if (ignoreTop)
                        {
                            point.Y = MathHelper.Clamp(point.Y, top + radius, bottom);
                        }
                        if (noise + 1 > Vector2.Distance(new Vector2(x, y), point) / (radius / 2))
                        {
                            MiscTools.Tile(x, y).HasTile = true;
                            MiscTools.Tile(x, y).TileType = biomes.FindLayer(x, y) >= biomes.Height - 4 ? TileID.Ash : biomes.FindLayer(x, y) >= biomes.Height - 6 ? TileID.Obsidian : biomes.FindBiome(x, y) == BiomeID.Granite ? TileID.Granite : biomes.FindBiome(x, y) == BiomeID.Jungle || biomes.FindBiome(x, y) == BiomeID.Glowshroom ? TileID.Mud : biomes.FindBiome(x, y) == BiomeID.Tundra ? TileID.IceBlock : biomes.FindLayer(x, y) < biomes.caveLayer && biomes.FindLayer(x, y) >= biomes.surfaceLayer && biomes.MaterialBlend(x, y, frequency: 2) < 0.2f ? TileID.Dirt : TileID.Stone;
                        }
                        else if (!MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y - 1).HasTile && MiscTools.Tile(x, y - 1).TileType == TileID.Mud)
                        {
                            MiscTools.Tile(x, y - 1).TileType = biomes.FindBiome(x, y) == BiomeID.Jungle ? TileID.JungleGrass : TileID.MushroomGrass;
                        }
                    }
                }
            }
        }

        public static void AddTraps(Rectangle location, float multiplier = 1, bool temple = false)
        {
            BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

            int count = (int)(location.Width * location.Height * multiplier / 2000 * ModContent.GetInstance<Worldgen>().TrapFrequency);
            while (count > 0)
            {
                int x = WorldGen.genRand.Next(location.Left, location.Right);
                int y = WorldGen.genRand.Next(location.Top + 5, location.Bottom);

                if (!MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount == 0 && !MiscTools.Solid(x, y - 1) && !MiscTools.Solid(x, y - 2) && MiscTools.Solid(x, y + 1) && TileID.Sets.TouchDamageImmediate[MiscTools.Tile(x, y + 1).TileType] == 0 && MiscTools.Tile(x, y + 1).TileType != TileID.TrapdoorClosed)
                {
                    if (!temple)
                    {
                        int trapX = x;
                        int trapY = y - WorldGen.genRand.Next(3);
                        int direction = WorldGen.genRand.NextBool(2) ? -1 : 1;
                        int length = 0;

                        while (!MiscTools.Solid(trapX, trapY))
                        {
                            trapX += direction;
                            length++;
                        }
                        if (length > 12 && length <= 64 && MiscTools.Tile(trapX, trapY).TileType != TileID.Traps && MiscTools.Tile(trapX, trapY).TileType != TileID.ClosedDoor && MiscTools.Tile(trapX, trapY).TileType != ModContent.TileType<LockedIronDoor>() && TileID.Sets.TouchDamageImmediate[MiscTools.Tile(trapX, trapY).TileType] == 0 && (MiscTools.Tile(trapX, trapY - 1).TileType != TileID.Traps && MiscTools.Tile(trapX, trapY + 1).TileType != TileID.Traps || WorldGen.genRand.NextBool(5)))
                        {
                            WorldGen.PlaceTile(x, y, TileID.PressurePlates, style: 2);

                            MiscTools.Tile(trapX, trapY).Slope = SlopeType.Solid; MiscTools.Tile(trapX, trapY).TileType = TileID.Traps;
                            MiscTools.Tile(trapX, trapY).TileFrameX = (short)((direction == 1 ? 0 : 1) * 18); MiscTools.Tile(trapX, trapY).TileFrameY = 0;

                            MiscTools.Wire(x, y + (Main.wallDungeon[MiscTools.Tile(x, y).WallType] ? 2 : 1), trapX, trapY);
                            MiscTools.Tile(x, y).RedWire = true; MiscTools.Tile(x, y + 1).RedWire = true;

                            count--;
                        }
                    }
                    else
                    {
                        if (MiscTools.Tile(x, y).WallType == WallID.LihzahrdBrickUnsafe || MiscTools.Tile(x, y).WallType == ModContent.WallType<temple>())
                        {
                            int trapLeft = x - WorldGen.genRand.Next(3);
                            int trapRight = x + WorldGen.genRand.Next(3);
                            int trapY = y;
                            int length = 0;

                            while (!MiscTools.Solid(x, trapY))
                            {
                                trapY--;
                                length++;
                            }

                            if (length > 7)
                            {
                                bool valid = true;
                                for (int i = trapLeft; i <= trapRight; i++)
                                {
                                    if (!MiscTools.Solid(i, trapY) || WorldGen.SolidOrSlopedTile(i, trapY + 1))
                                    {
                                        valid = false;
                                        break;
                                    }
                                    else if (MiscTools.Tile(i, trapY).TileType == TileID.Traps || MiscTools.Tile(i, trapY).TileType == TileID.Platforms || TileID.Sets.TouchDamageImmediate[MiscTools.Tile(i, trapY).TileType] > 0)
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
                                        MiscTools.Tile(i, trapY).Slope = SlopeType.Solid; MiscTools.Tile(i, trapY).TileType = TileID.Traps;
                                        MiscTools.Tile(i, trapY).TileFrameX = 0; MiscTools.Tile(i, trapY).TileFrameY = (short)((length > 19 ? 3 : 4) * 18);
                                        MiscTools.Tile(i, trapY).YellowWire = true;
                                    }

                                    int offset = 0;
                                    if (MiscTools.Tile(x, y).WallType != WallID.LihzahrdBrickUnsafe)
                                    {
                                        int left = 0;
                                        int right = 0;
                                        while (MiscTools.Tile(x - left, y).WallType != WallID.LihzahrdBrickUnsafe)
                                        {
                                            left++;
                                        }
                                        while (MiscTools.Tile(x + right, y).WallType != WallID.LihzahrdBrickUnsafe)
                                        {
                                            right++;
                                        }

                                        if (left == right)
                                        {
                                            offset = WorldGen.genRand.NextBool(2) ? -left : right;
                                        }
                                        else offset = left < right ? -left : right;
                                    }

                                    MiscTools.Tile(x, y).YellowWire = true;
                                    MiscTools.Wire(x, y + (offset != 0 ? 1 : 0), x + offset, trapY + 1, false, yellow: true);
                                    if (offset != 0)
                                    {
                                        MiscTools.Wire(x + offset, trapY, x, trapY, false, yellow: true);
                                    }

                                    count--;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddVariation(Rectangle location, int padding = 10)
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

            for (int y = location.Top - padding; y <= location.Bottom + padding; y++)
            {
                for (int x = location.Left - padding; x <= location.Right + padding; x++)
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
                        if (tile.TileType == TileID.HangingLanterns && MiscTools.Tile(x, y - 1).TileType != TileID.HangingLanterns && WorldGen.genRand.NextBool(2))
                        {
                            MiscTools.Tile(x, y).TileFrameX = 18;
                            MiscTools.Tile(x, y + 1).TileFrameX = 18;
                        }
                        else if (tile.TileType == TileID.Banners)
                        {
                            if (MiscTools.Tile(x, y - 1).TileType != TileID.Banners && tile.TileFrameX == 18 * 4)
                            {
                                int style = Main.rand.Next(3);
                                MiscTools.Tile(x, y).TileFrameX += (short)(style * 18);
                                MiscTools.Tile(x, y + 1).TileFrameX += (short)(style * 18);
                                MiscTools.Tile(x, y + 2).TileFrameX += (short)(style * 18);
                            }
                        }
                        else if (tile.TileType == TileID.Books)
                        {
                            if (tile.TileFrameX != 18 * 5)
                            {
                                int style = Main.rand.Next(5);
                                MiscTools.Tile(x, y).TileFrameX = (short)(style * 18);
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
                    else if (tile.WallType != ModContent.WallType<undergrowth>() && tile.WallType != WallID.RocksUnsafe1 && tile.WallType != WallID.Cave6Unsafe)
                    {
                        bool tomb = tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>();

                        if ((WorldGen.genRand.NextBool(2) || tomb) && (y > Main.worldSurface || !Main.wallLight[tile.WallType]) && biomes.FindBiome(x, y) != BiomeID.Tundra && biomes.FindBiome(x, y) != BiomeID.Desert && biomes.FindBiome(x, y) != BiomeID.Marble && biomes.FindBiome(x, y) != BiomeID.Granite && biomes.FindBiome(x, y) != BiomeID.Hive)
                        {
                            if (MiscTools.Solid(x, y - 1) && MiscTools.Tile(x, y - 1).TileType != TileID.Spikes && (WorldGen.SolidOrSlopedTile(x - 1, y) || WorldGen.SolidOrSlopedTile(x + 1, y)))
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
                        if (MiscTools.Tile(x, y).HasTile && WorldGen.genRand.NextBool(chance) && MiscTools.AdjacentTiles(x, y) < 4 && MiscTools.Tile(x, y).WallType != ModContent.WallType<GardenBrickWall>())
                        {
                            if (tilesToErode.Contains(Framing.GetTileSafely(x, y).TileType) && !Main.tileFrameImportant[Framing.GetTileSafely(x, y - 1).TileType])
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
            if (ModLoader.TryGetMod("WombatQOL", out Mod wgi) && wgi.TryFind("CeremonialCandle", out ModTile candle))
            {
                BiomeMap biomes = ModContent.GetInstance<BiomeMap>();

                for (int y = location.Top; y <= location.Bottom; y++)
                {
                    for (int x = location.Left; x <= location.Right; x++)
                    {
                        Tile tile = Main.tile[x, y];

                        if (!tile.HasTile && MiscTools.Tile(x, y + 1).HasTile)
                        {
                            if (MiscTools.Tile(x, y + 1).TileType == TileID.PlanterBox)
                            {
                                if (WorldGen.genRand.NextBool(2))
                                {
                                    WorldGen.PlaceTile(x, y, TileID.ImmatureHerbs, style: 2);
                                }
                            }
                            else
                            {
                                if (MiscTools.Tile(x, y + 1).TileType == TileID.Bookcases)// || WGTools.Tile(x, y + 1).TileType == TileID.Pianos)
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
                                else if (MiscTools.Tile(x, y + 1).TileType == ModContent.TileType<SacrificialAltar>())
                                {
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceTile(x, y, candle.Type, style: WorldGen.genRand.Next(6));
                                    }
                                }
                                else if (MiscTools.Tile(x, y + 1).TileType == ModContent.TileType<AlchemyBench>())
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
                                else if (MiscTools.Tile(x, y + 1).TileType == TileID.Tables || MiscTools.Tile(x, y + 1).TileType == TileID.WorkBenches || MiscTools.Tile(x, y + 1).TileType == TileID.Dressers || MiscTools.Tile(x, y + 1).TileType == TileID.Platforms && (MiscTools.Tile(x, y + 1).TileFrameY == 18 * 9 || MiscTools.Tile(x, y + 1).TileFrameY == 18 * 10 || MiscTools.Tile(x, y + 1).TileFrameY == 18 * 11 || MiscTools.Tile(x, y + 1).TileFrameY == 18 * 12))
                                {
                                    if (WorldGen.genRand.NextBool(5) && MiscTools.Tile(x, y + 1).TileType != TileID.Platforms && MiscTools.Tile(x, y + 1).TileType != ModContent.TileType<SacrificialAltar>() && !MiscTools.Tile(x + 1, y).HasTile && MiscTools.Tile(x + 1, y + 1).HasTile)
                                    {
                                        WorldGen.PlaceTile(x, y, TileID.Bowls);
                                    }
                                    if (!tile.HasTile && (WorldGen.genRand.NextBool(2) || tile.WallType == ModContent.WallType<EnchantedBrickWallUnsafe>()))
                                    {
                                        if (WorldGen.genRand.NextBool(3) && (MiscTools.Tile(x, y + 1).TileType == TileID.Platforms || !Main.wallDungeon[tile.WallType]) && biomes.FindBiome(x, y) != BiomeID.Granite)
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
                            if (!MiscTools.SurroundingTilesActive(x, y, true))
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

                    if (MiscTools.Tile(x - 1, y - 1).TileType == TileID.Mud)
                    {
                        if (!MiscTools.SurroundingTilesActive(x - 1, y - 1, true))
                        {
                            MiscTools.Tile(x - 1, y - 1).TileType = TileID.JungleGrass;
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
    //            structureX = (int)(Jungle.Center + Main.rand.NextFloat(-Jungle.Width, Jungle.Width) / 2) * biomes.scale;
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

            int area = (int)(Hive.Size * biomes.Scale) * (int)(Hive.Size * biomes.Scale);

            int objects = area / 400;
            while (objects > 0)
            {
                int x = WorldGen.genRand.Next((int)(Hive.X - Hive.Size) * biomes.Scale, (int)(Hive.X + Hive.Size) * biomes.Scale);
                int y = WorldGen.genRand.Next((int)(Hive.Y - Hive.Size) * biomes.Scale, (int)(Hive.Y + Hive.Size) * biomes.Scale);

                if (biomes.FindBiome(x, y) == BiomeID.Hive && MiscTools.Tile(x, y - 1).HasTile && !MiscTools.Tile(x + 1, y).HasTile && MiscTools.Tile(x, y - 1).TileType == TileID.Hive)
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
                                if (biomes.FindBiome(x, y) != BiomeID.OceanCave && WorldGen.genRand.NextBool(4) && MiscTools.NoDoors(x - 1, y, 3) && MiscTools.Tile(x, y + 1).TileType != TileID.Platforms && tile.TileType != ModContent.TileType<SacrificialAltar>())
                                {
                                    LargePile(x, y);
                                    x++;
                                }
                                if (WorldGen.genRand.NextBool(3) && MiscTools.NoDoors(x, y, 2))
                                {
                                    MediumPile(x, y);
                                    x++;
                                }
                                if (WorldGen.genRand.NextBool(2) && MiscTools.NoDoors(x, y))
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
            if (!MiscTools.Tile(x, y).HasTile && !MiscTools.Tile(x - 1, y).HasTile && !MiscTools.Tile(x + 1, y).HasTile)
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
                    else if (tile.TileType == TileID.LeafBlock)
                    {
                        style = Main.rand.Next(50, 52);
                    }
                    else if (tile.TileType == TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
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
            if (!MiscTools.Tile(x, y).HasTile && !MiscTools.Tile(x + 1, y).HasTile)
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
                    else if (tile.TileType == TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
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
                    MiscTools.MediumPile(x, y, X, Y - 1);
                }
            }
        }

        private void SmallPile(int x, int y)
        {
            if (!MiscTools.Tile(x, y).HasTile)
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

                    if (biomes.FindBiome(x, y) != BiomeID.Beach && biomes.FindLayer(x, y) < biomes.surfaceLayer)
                    {
                        if (!MiscTools.SurroundingTilesActive(x, y, true))
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

                if (biomes.FindBiome(x, y) == BiomeID.None && !MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount == 255 && MiscTools.Tile(x, y).LiquidType == 1)
                {
                    int radius = WorldGen.genRand.Next(30, 90);

                    for (int j = Math.Max(y - radius, GenVars.lavaLine); j <= y + radius; j++)
                    {
                        for (int i = Math.Max(x - radius, 40); i <= Math.Min(x + radius, Main.maxTilesX - 40); i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + (noise.GetNoise(i, j) + 1) * 5 < radius && WorldGen.InWorld(i, j) && biomes.FindBiome(i, j) == BiomeID.None && !MiscTools.SurroundingTilesActive(i, j, true))
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

                if (biomes.FindBiome(x, y, false) == BiomeID.None && !MiscTools.Tile(x, y).HasTile && MiscTools.Tile(x, y).LiquidAmount == 255 && MiscTools.Tile(x, y).LiquidType == 0)
                {
                    int radius = WorldGen.genRand.Next(25, 75);

                    for (int j = Math.Max(y - radius, (int)Main.rockLayer); j <= y + radius; j++)
                    {
                        for (int i = Math.Max(x - radius, 40); i <= Math.Min(x + radius, Main.maxTilesX - 40); i++)
                        {
                            if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + (noise.GetNoise(i, j) + 1) * 5 < radius && WorldGen.InWorld(i, j) && biomes.FindBiome(i, j) == BiomeID.None && !MiscTools.SurroundingTilesActive(i, j, true))
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
                    Tile tile = MiscTools.Tile(x, y);

                    if (biomes.FindBiome(x, y) == BiomeID.Tundra && (!tile.HasTile || !Main.tileSolid[tile.TileType]) && y > Main.worldSurface * 0.5f && !Main.wallDungeon[tile.WallType])
                    {
                        if (Framing.GetTileSafely(x, y).LiquidAmount > 0 && Framing.GetTileSafely(x, y - 1).LiquidAmount == 0 && !MiscTools.Solid(x, y - 1) && Framing.GetTileSafely(x - 1, y - 1).LiquidAmount == 0 && Framing.GetTileSafely(x + 1, y - 1).LiquidAmount == 0)
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
                            else if (tile.TileType == ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>())
                            {
                                if (MiscTools.Tile(x, y - 1).TileType != ModContent.TileType<Tiles.Objects.Hazards.WoodenSpike>())
                                {
                                    int style = Main.rand.Next(4);
                                    MiscTools.Tile(x, y).TileFrameX = (short)(style * 16);
                                    MiscTools.Tile(x, y + 1).TileFrameX = (short)(style * 16);
                                }
                            }
                            else if (tile.TileType == TileID.MinecartTrack)
                            {
                                MiscTools.Tile(x, y + 2).LiquidAmount = 0;

                                if (tile.TileFrameX == 1 && tile.TileFrameY == -1)
                                {
                                    bool left = !MiscTools.Tile(x - 1, y).HasTile || MiscTools.Tile(x - 1, y).TileType != TileID.MinecartTrack;
                                    bool right = !MiscTools.Tile(x + 1, y).HasTile || MiscTools.Tile(x + 1, y).TileType != TileID.MinecartTrack;
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
                            else if (tile.TileType == TileID.Pots && MiscTools.Tile(x - 1, y).TileType != TileID.Pots && MiscTools.Tile(x, y - 1).TileType != TileID.Pots && biomes.FindBiome(x, y) == BiomeID.Tundra)
                            {
                                int style = Main.rand.Next(4, 7);
                                for (int j = 0; j < 2; j++)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        MiscTools.Tile(x + i, y + j).TileFrameY = (short)(style * 36 + j * 18);
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
                                    MiscTools.Tile(x + 3, y).HasTile = false;
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

                            if (MiscTools.SurroundingTilesActive(x, y, true))
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
                            if (RemTile.SolidBottom(x, y - 1) && MiscTools.Tile(x, y - 1).TileType == TileID.Hive)
                            {
                                top = true;
                            }
                            if (RemTile.SolidTop(x, y + 1) && MiscTools.Tile(x, y + 1).TileType == TileID.Hive)
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
                            if (MiscTools.Tile(x, y - 1).HasTile && MiscTools.Tile(x, y - 1).TileType == TileID.Marble)
                            {
                                WorldGen.PlaceTile(x, y, TileID.WaterDrip);
                            }
                        }

                        if (MiscTools.Tile(x, y + 24).HasTile && MiscTools.Tile(x, y + 24).TileType == ModContent.TileType<LabyrinthBrick>())
                        {
                            tile.LiquidAmount = 0;

                            if (tile.TileType == TileID.Obsidian)
                            {
                                tile.HasTile = false;
                            }
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
                        else if ((MiscTools.Tile(x + 1, y).WallType == WallID.HardenedSand && MiscTools.Tile(x - 1, y).WallType == WallID.Sandstone || MiscTools.Tile(x - 1, y).WallType == WallID.HardenedSand && MiscTools.Tile(x + 1, y).WallType == WallID.Sandstone) && tile.WallType == 0)
                        {
                            tile.WallType = WallID.HardenedSand;
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
                MiscTools.Tile(GenVars.lAltarX + x, GenVars.lAltarY + 1).HasTile = true;
                MiscTools.Tile(GenVars.lAltarX + x, GenVars.lAltarY + 1).TileType = TileID.LihzahrdBrick;

                for (int y = 0; y <= 1; y++)
                {
                    //WorldGen.KillTile(GenVars.lAltarX + x, GenVars.lAltarY + y);
                    Tile tile = MiscTools.Tile(GenVars.lAltarX + x, GenVars.lAltarY + y);

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
                    if (!MiscTools.Tile(x, y).HasTile && WorldGen.genRand.NextBool(30))
                    {
                        if (MiscTools.Tile(x, y).WallType == WallID.Sandstone)
                        {
                            if (MiscTools.AdjacentTiles(x, y, true) >= 1)
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
                        if (!MiscTools.Tile(x, y).HasTile && WorldGen.SolidTile(MiscTools.Tile(x, y + 1)) && MiscTools.Tile(x, y).LiquidAmount == 255)
                        {
                            if (MiscTools.Tile(x, y + 1).TileType == TileID.Coralstone)
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

            Main.spawnTileX = Main.maxTilesX / 2 + (WorldGen.genRand.NextBool(2) ? -50 : 50);
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
                Tile tile = MiscTools.Tile(i, Main.spawnTileY + 1);
                if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.LivingWood && tile.TileType != TileID.LeafBlock)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
