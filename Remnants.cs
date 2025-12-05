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
            Filters.Scene["Remnants:SulfuricVents"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.75f, 0.75f, 0.5f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["Remnants:SulfuricVents"] = new SulfuricVentsSky();

            //Filters.Scene["Remnants:VaultExhaustFog"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.15f, 0.15f, 0.05f).UseOpacity(0.75f), EffectPriority.VeryHigh);
            //SkyManager.Instance["Remnants:VaultExhaustFog"] = new VaultExhaustFog();
        }
	}
}
