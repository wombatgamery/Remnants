using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Remnants.Content.Biomes;
using Remnants.Content.NPCs.Monsters;
using Remnants.Content.Tiles;
using Remnants.Content.Tiles.AerialGarden;
using Remnants.Content.Tiles.DesertRuins;
using Remnants.Content.Tiles;
using Remnants.Content.Tiles;
using Remnants.Content.Tiles;
using Remnants.Content.Walls;
using Remnants.Content.Walls;
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
using Remnants.Content.Tiles.SulfuricVents;
using Remnants.Content.Walls.Underworld;

namespace Remnants.Content.World
{
    public class RemSystem : ModSystem
    {
        public static int whisperingMazeY;
        public static int whisperingMazeX;
        public static bool sightedWard = false;

        public static bool prototypesExist;
        public static bool vaultExhaustAlarm = false;
        public static float vaultExhaustIntensity;
        public static float vaultLightIntensity;
        public static float vaultLightFlicker;

        public static Dictionary<Point16, SlotId> ambience;

        public override void Load()
        {
            On_OverlayManager.Draw += VaultExhaustFog;
        }

        public override void PreWorldGen()
        {
            prototypesExist = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            //tag["savedX"] = Main.LocalPlayer.position.X;
            //tag["savedY"] = Main.LocalPlayer.position.Y;
            tag["whisperingMazeX"] = whisperingMazeX;
            tag["whisperingMazeY"] = whisperingMazeY;
            tag["sightedWard"] = sightedWard;

            tag["hasPrototypes"] = prototypesExist;
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

            if (tag.TryGet("hasPrototypes", out bool proto))
            {
                prototypesExist = proto;
            }
            if (tag.TryGet("geoWeatherIntensity", out float str))
            {
                vaultExhaustIntensity = str;
            }
        }

        static float ventsProximity =>MathHelper.Clamp(1 - MathHelper.Distance(Main.LocalPlayer.Center.Y / 16, MathHelper.Clamp(Main.LocalPlayer.Center.Y / 16, Main.maxTilesY - 300, Main.maxTilesY - 200)) / 50f, 0, 1);
        static float vaultsProximity => !prototypesExist ? 0 : Main.LocalPlayer.ZoneUnderworldHeight ? MathHelper.Clamp(1 - MathHelper.Distance(Main.LocalPlayer.Center.X / 16, MathHelper.Clamp(Main.LocalPlayer.Center.X / 16, Main.maxTilesX * 0.4f, Main.maxTilesX * 0.6f)) / (Main.maxTilesX * 0.05f), 0, 1) : 0;
        public static float vaultRumbleIntensity => MathHelper.Clamp(ventsProximity + vaultsProximity, 0, 1);

        SoundStyle rumble = new("Remnants/Content/Sounds/Ambience/Rumbling", SoundType.Ambient);
        SoundStyle debris1 = new("Remnants/Content/Sounds/Ambience/Debris1", SoundType.Ambient);
        SoundStyle debris2 = new("Remnants/Content/Sounds/Ambience/Debris2", SoundType.Ambient);
        SoundStyle debris3 = new("Remnants/Content/Sounds/Ambience/Debris3", SoundType.Ambient);
        SoundStyle debris4 = new("Remnants/Content/Sounds/Ambience/Debris4", SoundType.Ambient);

        public override void PostUpdateTime()
        {
            float time = Utils.GetDayTimeAs24FloatStartingFromMidnight();
            vaultExhaustAlarm = (time % 12 < 0.25f || time % 12 >= 7.75f) && Main.dayRate <= 1;

            float exhaustSpeed = 1f / 60 / 15;

            if (time % 12 >= 8f)
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
                float volume = (Main.LocalPlayer.Center.Y / 16 - (Main.maxTilesY - 400)) / 100f;
                if (volume > 0)
                {
                    volume = MathHelper.Clamp(volume, 0, 1);

                    if (Main.rand.NextBool(60))
                    {
                        debris1.Volume = volume * Main.rand.NextFloat(0.5f, 1) * 0.25f * vaultExhaustIntensity;
                        debris1.PitchVariance = 1;
                        debris1.MaxInstances = 1;
                        debris1.SoundLimitBehavior = SoundLimitBehavior.IgnoreNew;
                        SoundEngine.PlaySound(debris1);
                    }
                    if (Main.rand.NextBool(60))
                    {
                        debris2.Volume = volume * Main.rand.NextFloat(0.5f, 1) * 0.25f * vaultExhaustIntensity;
                        debris2.PitchVariance = 1;
                        debris2.MaxInstances = 1;
                        debris2.SoundLimitBehavior = SoundLimitBehavior.IgnoreNew;
                        SoundEngine.PlaySound(debris2);
                    }
                    if (Main.rand.NextBool(300))
                    {
                        debris3.Volume = volume * Main.rand.NextFloat(0.5f, 1) * 0.25f * vaultExhaustIntensity;
                        debris3.PitchVariance = 1;
                        debris3.MaxInstances = 1;
                        debris3.SoundLimitBehavior = SoundLimitBehavior.IgnoreNew;
                        SoundEngine.PlaySound(debris3);
                    }
                    if (Main.rand.NextBool(60))
                    {
                        debris4.Volume = volume * Main.rand.NextFloat(0.5f, 1) * 0.25f * vaultExhaustIntensity;
                        debris4.PitchVariance = 1;
                        debris4.MaxInstances = 1;
                        debris4.SoundLimitBehavior = SoundLimitBehavior.IgnoreNew;
                        SoundEngine.PlaySound(debris4);
                    }

                    time = (int)(Main.time + (Main.dayTime ? Main.nightLength : 0));

                    if (time % 300 == 0 && prototypesExist)
                    {
                        float distance = MathHelper.Distance(Main.LocalPlayer.Center.X / 16, Main.maxTilesX * 0.6f) / (Main.maxTilesX * 0.3f);
                        distance = MathHelper.Clamp(distance, 0, 1);
                        SoundStyle alarm = new SoundStyle("Remnants/Content/Sounds/Ambience/VaultExhaustAlarmL", SoundType.Ambient);
                        alarm.Volume = volume * 1.5f * (1 - distance);
                        alarm.MaxInstances = 0;
                        alarm.PauseBehavior = PauseBehavior.PauseWithGame;
                        SoundEngine.PlaySound(alarm);

                        distance = MathHelper.Distance(Main.LocalPlayer.Center.X / 16, Main.maxTilesX * 0.4f) / (Main.maxTilesX * 0.3f);
                        distance = MathHelper.Clamp(distance, 0, 1);
                        alarm = new SoundStyle("Remnants/Content/Sounds/Ambience/VaultExhaustAlarmR", SoundType.Ambient);
                        alarm.Volume = volume * 1.5f * (1 - distance);
                        alarm.MaxInstances = 0;
                        alarm.PauseBehavior = PauseBehavior.PauseWithGame;
                        SoundEngine.PlaySound(alarm);
                    }

                    if (time % 120 == 0)
                    {
                        rumble.PauseBehavior = PauseBehavior.PauseWithGame;
                        rumble.Volume = vaultRumbleIntensity * vaultExhaustIntensity;
                        rumble.MaxInstances = 0;
                        SoundEngine.PlaySound(rumble);
                    }
                }
            }
        }

        float exhaustFogIntensity;

        private void VaultExhaustFog(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.ForegroundWater)
            {
                float targetIntensity = Main.LocalPlayer.InModBiome<SulfuricVents>() ? 1 : false && (Main.LocalPlayer.ZoneUnderworldHeight || Main.LocalPlayer.InModBiome<PrototypeInterior>() && Main.LocalPlayer.Center.Y / 16 > Main.maxTilesY - 220) ? 0.5f : 0;
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
                    if (player.InModBiome<PrototypeInterior>())
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
