using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles.Objects;

namespace Remnants.Content.Items.Placeable
{
	public class Scrapper : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Extractinator);
			Item.width = 17 * 2;
			Item.height = 19 * 2;
            Item.createTile = ModContent.TileType<Tiles.Objects.Scrapper>();
		}
	}
}