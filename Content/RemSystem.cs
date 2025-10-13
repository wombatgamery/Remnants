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

        public static int mushroomTiles;
        public static int hiveTiles;
        public static int marbleTiles;
        public static int graniteTiles;
        public static int sulfuricTiles;
        public static int oceanCaveTiles;

        public static int pyramidTiles;
        public static int gardenTiles;

        public override void ResetNearbyTileEffects()
        {
            mushroomTiles = 0;
            hiveTiles = 0;
            marbleTiles = 0;
            graniteTiles = 0;
            sulfuricTiles = 0;
            oceanCaveTiles = 0;

            pyramidTiles = 0;
            gardenTiles = 0;
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            mushroomTiles = tileCounts[TileID.MushroomGrass] + tileCounts[TileID.MushroomPlants] + tileCounts[TileID.MushroomBlock];
            hiveTiles = tileCounts[TileID.Hive];
            marbleTiles = tileCounts[TileID.Marble] + tileCounts[TileID.MarbleBlock];
            graniteTiles = tileCounts[TileID.Granite] + tileCounts[TileID.GraniteBlock];
            sulfuricTiles = tileCounts[ModContent.TileType<Sulfurstone>()] + tileCounts[ModContent.TileType<SulfuricVent>()];
            oceanCaveTiles = tileCounts[TileID.Coralstone];

            pyramidTiles = tileCounts[ModContent.TileType<PyramidBrick>()];
            gardenTiles = tileCounts[ModContent.TileType<GardenBrick>()];
            //pyramidPotNearby = tileCounts[ModContent.TileType<PyramidPot>()] > 0;

            //Main.SceneMetrics.SandTileCount += pyramidTiles;
        }
    }
}
