using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Backgrounds
{
	public class honeycomb : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/honeycomb");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/honeycomb");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/honeycomb");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/honeycomb");
		}
	}
}