using Terraria;
using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Backgrounds
{
	public class GrowthUG : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Remnants/Content/Biomes/Backgrounds/GrowthUG0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Remnants/Content/Biomes/Backgrounds/Honeycomb");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Remnants/Content/Biomes/Backgrounds/GrowthUG2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Remnants/Content/Biomes/Backgrounds/GrowthUG1");
			textureSlots[4] = BackgroundTextureLoader.GetBackgroundSlot("Remnants/Content/Biomes/Backgrounds/GrowthUG5");
			textureSlots[5] = BackgroundTextureLoader.GetBackgroundSlot("Remnants/Content/Biomes/Backgrounds/GrowthUG4");
		}
	}
}