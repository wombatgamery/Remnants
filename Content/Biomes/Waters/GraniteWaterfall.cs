using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Waters
{
    public class GraniteWaterfall : ModWaterfallStyle
    {
        //public override string Texture => "Terraria/Images/Waterfall_4";

        public override void AddLight(int i, int j)
        {
            Lighting.AddLight(i, j, (float)(32f / 255f), (float)(62f / 255f), (float)(103f / 255f));
        }
    }
}