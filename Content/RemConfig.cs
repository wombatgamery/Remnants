using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Remnants.Content
{
	public class Worldgen : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Header("General")]

        [DefaultValue(true)]
        [BackgroundColor(90, 160, 140)]
        public bool Safeguard;

        [DefaultValue(false)]
        [BackgroundColor(150, 100, 125)]
        public bool ExperimentalWorldgen;

        [Header("Terrain")]

        [Range(0f, 1f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float OreFrequency;

        [Range(0f, 1.5f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float CloudDensity;

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

        [DefaultValue(true)]
        [BackgroundColor(150, 150, 125)]
        public bool SunkenSeaRework;

        [DefaultValue(true)]
        public bool AltPlanetoids;

        [DefaultValue(false)]
        public bool DoLava;

        [Header("Structure")]

		[Range(0f, 2f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(1f)]
		public float TrapFrequency;

        [Range(0f, 1f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float CabinFrequency;

        [Range(0f, 1f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float RailroadFrequency;

        //[DefaultValue(true)]
        //[BackgroundColor(90, 160, 140)]
        //[ReloadRequired]
        //public float MeteorRework;

        //[DefaultValue(false)]
        //[BackgroundColor(150, 150, 125)]
        //public bool LargerSky;
    }
	public class Gameplay : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Functional")]

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

        [Header("Audio")]

        [DefaultValue(true)]
        public bool CustomMusic;
    }
}
