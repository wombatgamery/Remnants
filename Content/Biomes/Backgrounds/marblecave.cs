using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Backgrounds
{
	public class marblecave : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/marblecave");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/marblecave");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/marblecave");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Biomes/Backgrounds/marblecave");
		}
	}
}