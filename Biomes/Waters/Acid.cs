using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Biomes.Waters
{
    public class Acid : ModWaterStyle
    {
        //public override string Texture => "Terraria/Images/Misc/water_3";

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Remnants/Acidfall").Slot;

        public override int GetSplashDust() => DustID.Water_Desert;

        public override int GetDropletGore() => ModContent.GoreType<AcidDroplet>();

        //public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        //{
        //    r = 1f;
        //    g = 1f;
        //    b = 1f;
        //}
    }
}