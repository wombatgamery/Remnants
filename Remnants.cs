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

		public override void Load()
		{
			On_WorldGen.ShimmerCleanUp += CancelShimmerCleanup;
		}

		private void CancelShimmerCleanup(On_WorldGen.orig_ShimmerCleanUp orig)
		{
			return;
		}
	}
}
