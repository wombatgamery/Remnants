using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Backgrounds;
using Remnants.Walls;
using Remnants.Walls.Parallax;
using Remnants.Worldgen;
using Remnants.Worldgen.Subworlds;
//using SubworldLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Remnants.Biomes
{
    public class Hive : ModBiome
	{
		public override int Music => MusicID.JungleUnderground;// MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambient_reformed");

		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<honeycomb>();

		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override string MapBackground => "Terraria/Images/MapBG16";

		public override string BestiaryIcon => "Remnants/Biomes/HiveIcon";
		public override string BackgroundPath => "Terraria/Images/MapBG16";
		public override Color? BackgroundColor => base.BackgroundColor;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Hive");
		}

		//public override void OnEnter(Player player)
		//{
		//	if (ModLoader.TryGetMod("RemnantsMusic", out Mod musicMod))
		//	{
		//		music = MusicLoader.GetMusicSlot(musicMod, "Sounds/Music/ambient_reformed");
		//	}
		//	else music = MusicID.JungleUnderground;

		//	//ambienceTimer = 0;
		//}

		//SoundStyle ambience = new SoundStyle("Remnants/Sounds/ambience/shoggoth_drone", SoundType.Ambient) with { Volume = 0.25f, IsLooped = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
		//ReLogic.Utilities.SlotId ambienceSlot;

		//public override void OnInBiome(Player player)
		//{
		//	ambienceSlot = SoundEngine.PlaySound(ambience);
		//}

		//public override void OnLeave(Player player)
		//{
		//	SoundEngine.StopTrackedSounds();
		//}

		public override bool IsBiomeActive(Player player)
		{
			return RemWorld.hiveTiles >= 3000;
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
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/music_dark_fog") : -1;
		}

        public override void OnInBiome(Player player)
        {
			player.ZoneGlowshroom = false;
			player.ZoneSnow = false;
			player.ZoneJungle = false;
		}

        public override bool IsBiomeActive(Player player)
		{
			return RemWorld.marbleTiles >= 5000;
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
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/music_dark_somethingominous") : -1;
		}

		public override void OnInBiome(Player player)
		{
			player.ZoneGlowshroom = false;
			player.ZoneSnow = false;
			player.ZoneJungle = false;
		}

		public override bool IsBiomeActive(Player player)
		{
			return RemWorld.graniteTiles >= 5000;
		}
	}

	public class OceanCave : ModBiome
	{
		public override int Music => -1;

		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ocean Cave");
		}

		public override bool IsBiomeActive(Player player)
		{
			return player.ZoneBeach && player.ZoneDirtLayerHeight && RemWorld.oceanCaveTiles >= 100;
		}
	}

	public class Vault : ModBiome
	{
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/Acid");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		//public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/spittingacid");

		public override string BestiaryIcon => "Remnants/Biomes/VaultIcon";
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

		//SoundStyle ambience = new SoundStyle("Remnants/Sounds/ambience/Bluezone_BC0240_background_machine_room_ambience_004", SoundType.Ambient) with { Volume = 0.5f, IsLooped = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
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

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/WBA Free Track - Drifter");

		public override string BestiaryIcon => "Remnants/Biomes/VaultIcon";
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

		SoundStyle ambience = new SoundStyle("Remnants/Sounds/ambience/Bluezone_BC0240_background_command_center_ambience_004", SoundType.Ambient) with { Volume = 0.75f, IsLooped = true, PlayOnlyIfFocused = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
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

		public override int Music => music;

		public override string BestiaryIcon => "Remnants/Biomes/MagicalLabIcon";
        public override string MapBackground => "Terraria/Images/MapBG32";
        public override string BackgroundPath => "Terraria/Images/MapBG32";
		public override Color? BackgroundColor => base.BackgroundColor;

		int music = -1;
		public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/archaic_vault") : MusicID.Shimmer;
		}

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<magicallab>() || wall == ModContent.WallType<EnchantedBrickWallUnsafe>() || wall == ModContent.WallType<Ascension>();
		}
	}

	public class MagicalLabAscension : ModBiome
    {
		public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Bluezone_BC0262_black_project_cinematic_ambience_014");

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

		public override int Music => music;

		public override string BestiaryIcon => base.BestiaryIcon;
        public override string MapBackground => "Terraria/Images/MapBG19";
        public override string BackgroundPath => "Terraria/Images/MapBG19";
        public override Color? BackgroundColor => base.BackgroundColor;

		int music = -1;
		public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/fog") : MusicID.Eerie;
		}
        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<forgottentomb>() || wall == ModContent.WallType<TombBrickWallUnsafe>();
		}
	}

	public class Undergrowth : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override int Music => music;

		public override string BestiaryIcon => base.BestiaryIcon;
        public override string MapBackground => "Terraria/Images/MapBG13";
        public override string BackgroundPath => "Terraria/Images/MapBG13";
        public override Color? BackgroundColor => base.BackgroundColor;

		int music = -1;
		public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/growth") : MusicID.JungleNight;
		}
		public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return player.position.Y / 16 > Main.worldSurface - (int)(Main.maxTilesY / 1200f * 2) * 42 && (wall == ModContent.WallType<undergrowth>() || wall == WallID.LivingWoodUnsafe);
		}
	}

	public class AerialGarden : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override int Music => music;

		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/GardenWater");

		public override string BestiaryIcon => base.BestiaryIcon;
        public override string MapBackground => "Terraria/Images/MapBG33";
        public override string BackgroundPath => "Terraria/Images/MapBG33";
        public override Color? BackgroundColor => base.BackgroundColor;

		int music = -1;
		public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/depthsOfDespair") : MusicID.JungleNight;
		}
		public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return RemWorld.gardenTiles > 500;
		}
	}

	public class EchoingHalls : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => music;

		public override string BestiaryIcon => "Remnants/Biomes/MazeIcon";
        public override string MapBackground => "Terraria/Images/MapBG32";
        public override string BackgroundPath => "Terraria/Images/MapBG32";
        public override Color? BackgroundColor => base.BackgroundColor;

		int music = -1;
        public override void OnEnter(Player player)
		{
			music = ModContent.GetInstance<Client>().CustomMusic ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/darkambient2b") : MusicID.Eerie;
		}

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<whisperingmaze>() || wall == ModContent.WallType<LabyrinthTileWall>() || wall == ModContent.WallType<LabyrinthBrickWall>();
		}

		//SoundStyle ambience = new SoundStyle("Remnants/Sounds/ambience/Myuu-The-Unknown-Horror-Ambience", SoundType.Ambient) with { Volume = 0.5f, IsLooped = true, PlayOnlyIfFocused = true, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, MaxInstances = 1 };
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
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override int Music => MusicID.Temple;

		public override string MapBackground => "Terraria/Images/MapBG15";

        public override void OnInBiome(Player player)
        {
			player.ZoneDesert = true;
        }

        public override bool IsBiomeActive(Player player)
		{
			int wall = Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].WallType;
			return wall == ModContent.WallType<pyramid>() || wall == ModContent.WallType<PyramidBrickWallUnsafe>();
		}
	}
}