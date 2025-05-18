using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Remnants.Content
{
    public class Worldgen : ModConfig
    {
        public static Worldgen Instance;

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

        //[Range(0f, 1f)]
        //[Increment(.25f)]
        //[DrawTicks]
        //[DefaultValue(1f)]
        //public float PlatformFrequency;

        [Range(0f, 1f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float CacheFrequency;

        [Range(0f, 1f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float FrozenRuinFrequency;

        //[DefaultValue(false)]
        //[BackgroundColor(150, 100, 125)]
        //public bool DoLivingTrees;

        [Header("LargerSky")]

        [Range(0f, 2400f)]
        [Increment(50f)]
        [DefaultValue(0f)]
        [BackgroundColor(100, 100, 150)]
        public float FlatSurfaceRatioIncrease;

        [Range(0f, 2400f)]
        [Increment(50f)]
        [DefaultValue(0f)]
        [BackgroundColor(100, 100, 150)]
        public float FlatUndergroundRatioIncrease;

        [Range(0f, 2400f)]
        [Increment(50f)]
        [DefaultValue(0f)]
        [BackgroundColor(100, 100, 150)]
        public float FlatLavaRatioIncrease;
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
