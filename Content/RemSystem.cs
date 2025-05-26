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
using Remnants.Content.Tiles.Objects;

namespace Remnants.Content.World
{
    public class RemSystem : ModSystem
    {
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
        public static bool pyramidPotNearby;

        public override void ResetNearbyTileEffects()
        {
            mushroomTiles = 0;
            hiveTiles = 0;
            marbleTiles = 0;
            graniteTiles = 0;
            pyramidTiles = 0;
            oceanCaveTiles = 0;
            gardenTiles = 0;
            pyramidPotNearby = false;
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            pyramidTiles = tileCounts[ModContent.TileType<PyramidBrick>()];
            //pyramidPotNearby = tileCounts[ModContent.TileType<PyramidPot>()] > 0;

            mushroomTiles = tileCounts[TileID.MushroomGrass] + tileCounts[TileID.MushroomPlants] + tileCounts[TileID.MushroomBlock];
            hiveTiles = tileCounts[TileID.Hive];

            marbleTiles = tileCounts[TileID.Marble] + tileCounts[TileID.MarbleBlock] + tileCounts[TileID.MarbleColumn];
            graniteTiles = tileCounts[TileID.Granite] + tileCounts[TileID.GraniteBlock] + tileCounts[TileID.GraniteColumn];

            oceanCaveTiles = tileCounts[TileID.Coralstone];

            gardenTiles = tileCounts[ModContent.TileType<GardenBrick>()];

            Main.SceneMetrics.SandTileCount += pyramidTiles;
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

        public override void PostUpdateEverything()
        {
            if (!sightedWard && NPC.AnyNPCs(ModContent.NPCType<Ward>()))
            {
                sightedWard = true;
            }
        }
    }
}
