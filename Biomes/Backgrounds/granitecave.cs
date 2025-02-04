using Terraria.ModLoader;

namespace Remnants.Backgrounds
{
	public class granitecave : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/granitecave");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/granitecave");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/granitecave");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Biomes/Backgrounds/granitecave");
		}
	}
}