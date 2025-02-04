using Terraria.ModLoader;

namespace Remnants.Backgrounds
{
	public class marblecave : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/marblecave");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/marblecave");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/marblecave");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/marblecave");
		}
	}
}