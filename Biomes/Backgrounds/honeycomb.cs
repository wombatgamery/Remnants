using Terraria.ModLoader;

namespace Remnants.Backgrounds
{
	public class honeycomb : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/honeycomb");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/honeycomb");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/honeycomb");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/honeycomb");
		}
	}
}