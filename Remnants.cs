using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Remnants.Tiles;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Remnants.Worldgen;
//using SubworldLibrary;

namespace Remnants
{
	public class Remnants : Mod
	{
		public Remnants()
		{

		}

		public override void Load() //test
		{
            //TextureAssets.Item[ItemID.Shuriken].Value = GetTexture("Items/vanilla/shuriken");
			//TextureAssets.Item[ItemID.ThrowingKnife].Value = GetTexture("Items/vanilla/throwingknife");
			//TextureAssets.Item[ItemID.PoisonedKnife].Value = GetTexture("Items/vanilla/poisonknife");
		}

		//public override void AddRecipeGroups()/* tModPorter Note: Removed. Use ModSystem.AddRecipeGroups */
		//{
		//	RecipeGroup

		//	group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Husk", new int[]
		//	{
		//		ItemID.RedHusk,
		//		ItemID.CyanHusk,
		//		ItemID.VioletHusk
		//	});
		//	RecipeGroup.RegisterGroup("Remnants:BeetleHusk", group);
		//}
	}
}
