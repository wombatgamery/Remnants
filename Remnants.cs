using Remnants.Content.Biomes;
using System.Security.Cryptography.X509Certificates;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Remnants
{
	public class Remnants : Mod
	{
		public Remnants()
		{
			
		}

        public override void Load()
        {
            Filters.Scene["Remnants:SulfuricVents"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.75f, 0.8f, 0.5f).UseOpacity(0.7f), EffectPriority.VeryHigh);
            SkyManager.Instance["Remnants:SulfuricVents"] = new SulfuricVentsSky();
        }
	}
}
