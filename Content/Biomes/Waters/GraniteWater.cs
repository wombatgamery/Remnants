using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Waters
{
    public class GraniteWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Remnants/GraniteWaterfall").Slot;

        public override int GetSplashDust() => DustID.Water_GlowingMushroom;

        public override int GetDropletGore() => ModContent.GoreType<GraniteDroplet>();

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 0.5f;
            b = 1f;
        }
    }
}