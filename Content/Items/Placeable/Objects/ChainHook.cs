using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Remnants.Content.Tiles.Objects;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class ChainHook : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ChainLantern);
			Item.width = 14;
			Item.height = 20;
			Item.createTile = ModContent.TileType<Tiles.Objects.Decoration.ChainHook>();
		}
	}
}