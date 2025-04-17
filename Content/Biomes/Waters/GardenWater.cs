using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Waters
{
    public class GardenWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Remnants/GardenWaterfall").Slot;

        public override int GetSplashDust() => DustID.Water_Snow;

        public override int GetDropletGore() => ModContent.GoreType<GardenDroplet>();

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 0.8f;
            g = 0.8f;
            b = 1f;
        }
    }
}