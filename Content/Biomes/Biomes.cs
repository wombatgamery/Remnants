using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Biomes.Backgrounds;
using Remnants.Content.Buffs;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.World;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Remnants.Content.Biomes
{
    public class Beehive : ModBiome
	{
        public override int Music => -1;

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<honeycomb>();

		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override string MapBackground => "Terraria/Images/MapBG16";

		public override string BestiaryIcon => "Remnants/Content/Biomes/HiveIcon";
		public override string BackgroundPath => "Terraria/Images/MapBG16";
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            player.ZoneHive = true;
        }

        public override bool IsBiomeActive(Player player)
		{
			return RemSystem.hiveTiles >= 5000;
		}
	}

	public class MarbleCave : ModBiome
	{
        public override int Music => music;

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<marblecave>();

		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/MarbleWater");

		public override string MapBackground => "Terraria/Images/MapBG18";

		public override string BackgroundPath => "Terraria/Images/MapBG18";

		int music = -1;
		public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Gameplay>().MarbleMusic ? MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MarbleCave") : -1;
		}

        public override void OnInBiome(Player player)
        {
            player.ZoneGlowshroom = false;
            player.ZoneSnow = false;
            player.ZoneJungle = false;
        }

        public override bool IsBiomeActive(Player player)
		{
			return (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight) && RemSystem.marbleTiles >= 5000;
		}
	}

	public class GraniteCave : ModBiome
	{
		public override int Music => music;

		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<granitecave>();

		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/GraniteWater");

		public override string MapBackground => "Terraria/Images/MapBG17";

		public override string BackgroundPath => "Terraria/Images/MapBG17";

		int music = -1;
		public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Gameplay>().GraniteMusic ? MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/GraniteCave") : -1;
		}

        public override void OnInBiome(Player player)
        {
            player.ZoneGlowshroom = false;
            player.ZoneSnow = false;
            player.ZoneJungle = false;
        }

        public override bool IsBiomeActive(Player player)
		{
			return player.position.Y / 16 > Main.worldSurface + 50 && RemSystem.graniteTiles >= 5000;
		}
	}

	public class OceanCave : ModBiome
	{
		public override int Music => music;

		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ocean Cave");
		}

        int music = -1;

        public override void OnEnter(Player player)
        {
            music = ModContent.GetInstance<Gameplay>().OceanMusic ? MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/OceanCave") : -1;
        }

        public override bool IsBiomeActive(Player player)
		{
			return player.ZoneBeach && player.ZoneDirtLayerHeight && RemSystem.oceanCaveTiles >= 100;
		}
	}

	public class Vault : ModBiome
	{
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/Acid");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		//public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/spittingacid");

		public override string BestiaryIcon => "Remnants/Content/Biomes/VaultIcon";
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Vault");
		}

		public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<vault>() || wall == ModContent.WallType<VaultWallUnsafe>();
		}

		//SoundStyle ambience = new SoundStyle("Remnants/Content/Sounds/ambience/Bluezone_BC0240_background_machine_room_ambience_004", SoundType.Ambient) with { Volume = 0.5f, IsLooped = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
		//ReLogic.Utilities.SlotId ambienceSlot;

		//public override void OnInBiome(Player player)
		//{
		//	ambienceSlot = SoundEngine.PlaySound(ambience);
		//}

  //      public override void OnLeave(Player player)
  //      {
		//	SoundEngine.StopTrackedSounds();
  //      }
    }

	public class GoldenCity : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/WBA Free Track - Drifter");

		public override string BestiaryIcon => "Remnants/Content/Biomes/VaultIcon";
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Golden City");
		}

		public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<Walls.Parallax.GoldenCity>() || wall == ModContent.WallType<GoldenPanelWallUnsafe>();
		}

		SoundStyle ambience = new SoundStyle("Remnants/Content/Sounds/ambience/Bluezone_BC0240_background_command_center_ambience_004", SoundType.Ambient) with { Volume = 0.75f, IsLooped = true, PlayOnlyIfFocused = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
		ReLogic.Utilities.SlotId ambienceSlot;

		public override void OnInBiome(Player player)
		{
			ambienceSlot = SoundEngine.PlaySound(ambience);
		}

		public override void OnLeave(Player player)
		{
			SoundEngine.StopTrackedSounds();
		}
	}

	public class MagicalLab : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MagicalLab");

		public override string BestiaryIcon => "Remnants/Content/Biomes/MagicalLabIcon";
        public override string MapBackground => "Terraria/Images/MapBG32";
        public override string BackgroundPath => "Terraria/Images/MapBG32";
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<magicallab>() || wall == ModContent.WallType<EnchantedBrickWallUnsafe>() || wall == ModContent.WallType<Ascension>();
		}
	}

	public class MagicalLabAscension : ModBiome
    {
		public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MagicElevator");

		public override bool IsBiomeActive(Player player)
        {
			if (!player.shimmering)
            {
				int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
				return wall == ModContent.WallType<Ascension>();
			}
			return false;
		}

        public override void OnEnter(Player player)
        {
			SoundStyle sound = SoundID.Item163;
			sound.MaxInstances = 0;
			SoundEngine.PlaySound(sound, player.Center);
        }
    }

	public class ForgottenTomb : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Tomb");

		public override string BestiaryIcon => base.BestiaryIcon;
        public override string MapBackground => "Terraria/Images/MapBG19";
        public override string BackgroundPath => "Terraria/Images/MapBG19";
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<forgottentomb>() || wall == ModContent.WallType<TombBrickWallUnsafe>();
		}
	}

	public class Undergrowth : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Undergrowth");

		public override string BestiaryIcon => "Remnants/Content/Biomes/UndergrowthIcon";
        public override string MapBackground => "Terraria/Images/MapBG13";
        public override string BackgroundPath => "Terraria/Images/MapBG13";
        public override Color? BackgroundColor => base.BackgroundColor;

		public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return player.position.Y / 16 > Main.worldSurface - Main.maxTilesY / 12 && (wall == ModContent.WallType<undergrowth>() || wall == WallID.LivingWoodUnsafe);
		}
	}

	public class AerialGarden : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/AerialGarden");

		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/GardenWater");

		public override string BestiaryIcon => base.BestiaryIcon;
        public override string MapBackground => "Terraria/Images/MapBG33";
        public override string BackgroundPath => "Terraria/Images/MapBG33";
        public override Color? BackgroundColor => base.BackgroundColor;

		public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return RemSystem.gardenTiles > 500;
		}
	}

	public class EchoingHalls : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/EchoingHalls");

		public override string BestiaryIcon => "Remnants/Content/Biomes/MazeIcon";
        public override string MapBackground => "Terraria/Images/MapBG32";
        public override string BackgroundPath => "Terraria/Images/MapBG32";
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<whisperingmaze>() || wall == ModContent.WallType<LabyrinthTileWall>() || wall == ModContent.WallType<LabyrinthBrickWall>();
		}

		//SoundStyle ambience = new SoundStyle("Remnants/Content/Sounds/ambience/Myuu-The-Unknown-Horror-Ambience", SoundType.Ambient) with { Volume = 0.5f, IsLooped = true, PlayOnlyIfFocused = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
		//ReLogic.Utilities.SlotId ambienceSlot;

		//public override void OnInBiome(Player player)
		//{
		//	ambienceSlot = SoundEngine.PlaySound(ambience);
		//}

		//public override void OnLeave(Player player)
		//{
		//	SoundEngine.StopTrackedSounds();
		//}
	}

	public class JungleTemple : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => MusicID.Temple;

		public override string MapBackground => "Terraria/Images/MapBG14";

		public override void OnInBiome(Player player)
		{
			player.ZoneJungle = true;
			player.ZoneLihzhardTemple = true;
		}

		public override bool IsBiomeActive(Player player)
		{
			return InFrontOfWall(WallID.LihzahrdBrickUnsafe) || InFrontOfWall(ModContent.WallType<temple>());
		}

		private bool InFrontOfWall(int wallType)
		{
			Player player = Main.LocalPlayer;
			return Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType == wallType;
		}
    }

	public class Pyramid : ModBiome
	{
        public override int Music => -1;

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override string MapBackground => "Terraria/Images/MapBG15";

        public override void OnInBiome(Player player)
        {
			player.ZoneDesert = true;

			if (player.GetBestPickaxe().pick < 65)
			{
                player.AddBuff(ModContent.BuffType<PyramidAntiCheese>(), 2);
            }
        }

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
            return player.position.Y / 16 > Main.worldSurface - 48 * 2 - 6 && player.position.Y / 16 < Main.worldSurface + 6 && (wall == ModContent.WallType<pyramid>() || wall == ModContent.WallType<PyramidBrickWallUnsafe>() || wall == ModContent.WallType<PyramidRuneWall>());
		}
	}
}