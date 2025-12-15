using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Remnants.Content.Biomes;
using Remnants.Content.NPCs.Monsters;
using Remnants.Content.Tiles;
using Remnants.Content.Tiles.Blocks;
using Remnants.Content.Tiles.Objects;
using Remnants.Content.Tiles.Objects.Furniture;
using Remnants.Content.Tiles.Plants;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Walls.Vanity;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;
using static Remnants.Content.World.BiomeGeneration;
using static Remnants.Content.World.BiomeMap;

namespace Remnants.Content.World
{
    public class RemSystem : ModSystem
    {
        public static int whisperingMazeY;
        public static int whisperingMazeX;
        public static bool sightedWard = false;

        public static bool vaultExhaustAlarm = false;
        public static float vaultExhaustIntensity;
        public static float vaultLightIntensity;
        public static float vaultLightFlicker;

        public static Dictionary<Point16, SlotId> ambience;

        public override void Load()
        {
            On_OverlayManager.Draw += VaultExhaustFog;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            //tag["savedX"] = Main.LocalPlayer.position.X;
            //tag["savedY"] = Main.LocalPlayer.position.Y;
            tag["whisperingMazeX"] = whisperingMazeX;
            tag["whisperingMazeY"] = whisperingMazeY;
            tag["sightedWard"] = sightedWard;

            tag["geoWeatherIntensity"] = vaultExhaustIntensity;
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

            if (tag.TryGet("geoWeatherIntensity", out float str))
            {
                vaultExhaustIntensity = str;
            }
        }

        public override void PostUpdateTime()
        {
            float time = Utils.GetDayTimeAs24FloatStartingFromMidnight();
            vaultExhaustAlarm = time % 6 < 0.25f || time % 6 >= 3.75f;

            float exhaustSpeed = 1f / 60 / 15;

            if (time % 6 >= 4f)
            {
                if (vaultExhaustIntensity < 1)
                {
                    vaultExhaustIntensity += exhaustSpeed;
                }
                else if (vaultExhaustIntensity > 1)
                {
                    vaultExhaustIntensity = 1;
                }
            }
            else
            {
                if (vaultExhaustIntensity > 0)
                {
                    vaultExhaustIntensity -= exhaustSpeed;
                }
                else if (vaultExhaustIntensity < 0)
                {
                    vaultExhaustIntensity = 0;
                }
            }

            if (vaultExhaustAlarm)
            {
                float sirenVolume = (Main.LocalPlayer.Center.Y / 16 - (Main.maxTilesY - 400)) / 100f;
                //sirenVolume *= MathHelper.Distance(Main.LocalPlayer.Center.X / 16)
                if (sirenVolume > 0)
                {
                    sirenVolume = MathHelper.Clamp(sirenVolume, 0, 1);
                    //time *= 60;
                    //time = (int)time;

                    if (Main.GameUpdateCount % 300 == 0)
                    {
                        float distance = MathHelper.Distance(Main.LocalPlayer.Center.X / 16, Main.maxTilesX * 0.6f) / (Main.maxTilesX * 0.2f);
                        distance = MathHelper.Clamp(distance, 0, 1);
                        SoundStyle alarm = new SoundStyle("Remnants/Content/Sounds/Ambience/VaultExhaustAlarmLeft");
                        alarm.Volume = sirenVolume * 0.75f * (1 - distance);
                        SoundEngine.PlaySound(alarm);

                        distance = MathHelper.Distance(Main.LocalPlayer.Center.X / 16, Main.maxTilesX * 0.4f) / (Main.maxTilesX * 0.2f);
                        distance = MathHelper.Clamp(distance, 0, 1);
                        alarm = new SoundStyle("Remnants/Content/Sounds/Ambience/VaultExhaustAlarmRight");
                        alarm.Volume = sirenVolume * 0.75f * (1 - distance);
                        SoundEngine.PlaySound(alarm);
                    }
                }
            }
        }

        float exhaustFogIntensity;

        private void VaultExhaustFog(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.ForegroundWater)
            {
                float targetIntensity = Main.LocalPlayer.InModBiome<SulfuricVents>() ? 1 : Main.LocalPlayer.ZoneUnderworldHeight ? 0.5f : 0;
                targetIntensity += (vaultExhaustIntensity - 1) * 0.75f;

                if (exhaustFogIntensity > 0 || targetIntensity > 0)
                {
                    if (exhaustFogIntensity != targetIntensity)
                    {
                        if (exhaustFogIntensity < targetIntensity)
                        {
                            exhaustFogIntensity += 0.005f;

                            if (exhaustFogIntensity > targetIntensity)
                            {
                                exhaustFogIntensity = targetIntensity;
                            }
                        }
                        else if (exhaustFogIntensity > targetIntensity)
                        {
                            exhaustFogIntensity -= 0.005f;

                            if (exhaustFogIntensity < targetIntensity)
                            {
                                exhaustFogIntensity = targetIntensity;
                            }
                        }
                    }

                    float opacity = exhaustFogIntensity * 0.25f;

                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), new Color(opacity, opacity, opacity / 2, opacity));
                    spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), new Color(0, 0, 0, opacity));
                }
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

        public override void PostUpdatePlayers()
        {
            bool lightsActive = false;
            foreach (var player in Main.ActivePlayers)
            {
                if (!player.DeadOrGhost)
                {
                    if (player.InModBiome<VaultInterior>())
                    {
                        int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
                        if (wall != ModContent.WallType<vault>())
                        {
                            lightsActive = true;
                            break;
                        }
                    }
                }
            }

            if (lightsActive)
            {
                if (vaultLightIntensity < 1)
                {
                    vaultLightIntensity += 0.025f;
                    vaultLightFlicker = Main.rand.NextFloat(0, 1);
                }
                else vaultLightIntensity = 1;
            }
            else
            {
                if (vaultLightIntensity > 0)
                {
                    vaultLightIntensity -= 0.025f;
                    vaultLightFlicker = Main.rand.NextFloat(0, 1);
                }
                else vaultLightIntensity = 0;
            }
        }
    }
}
