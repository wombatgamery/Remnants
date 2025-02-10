using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Remnants.Tiles;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Remnants.Worldgen;
//using SubworldLibrary;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Remnants
{
	public class Client : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("Worldgen")]

        [Range(0.5f, 1.5f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        //[BackgroundColor(125, 125, 100)]
        public float TerrainAmplitude;

        [DefaultValue(true)]
        public bool IceMountain;

        [DefaultValue(true)]
        public bool JungleValley;

        //[Label("Ore Frequency")]
        //[Tooltip("Controls the amount of ore generated in new worlds.")]
        [Range(0f, 1f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(1f)]
		public float OreFrequency;

		[Range(0f, 2f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(1f)]
		public float TrapFrequency;

        //[Label("Cloud Density")]
        //[Tooltip("Controls the amount of clouds generated in new worlds.")]
        [Range(0f, 1.5f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(1f)]
		public float CloudDensity;

        //[DefaultValue(false)]
        //[BackgroundColor(150, 150, 125)]
        //public bool LargerSky;

        [DefaultValue(false)]
        [BackgroundColor(150, 100, 125)]
        public bool ExperimentalWorldgen;

        [Header("Audio")]

		[DefaultValue(true)]
		public bool CustomMusic;

	}
	public class Server : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Header("Gameplay")]

		[DefaultValue(true)]
        [BackgroundColor(90, 160, 140)]
        public bool FreedomOfMovement;

		[DefaultValue(true)]
        [BackgroundColor(150, 150, 125)]
        public bool EnemyAI;

		[DefaultValue(false)]
        [BackgroundColor(150, 100, 125)]
        public bool ProjectileAI;

		[DefaultValue(true)]
        [BackgroundColor(150, 150, 125)]
        public bool HangingBats;
	}
}
