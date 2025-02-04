using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Dusts;

namespace Remnants.Biomes.Waters
{
    public class MarbleWater : ModWaterStyle
    {
        //public override string Texture => "Terraria/Images/Misc/water_3";

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Remnants/MarbleWaterfall").Slot;

        public override int GetSplashDust() => DustID.DesertWater2;

        public override int GetDropletGore() => ModContent.GoreType<MarbleDroplet>();

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 0.8f;
            g = 0.9f;
            b = 1f;
        }
    }
}