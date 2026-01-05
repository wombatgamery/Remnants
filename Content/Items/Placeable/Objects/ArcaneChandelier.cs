using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class ArcaneChandelier : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GoldChandelier);
			Item.width = 16;
			Item.height = 16;
			Item.placeStyle = 0;
			Item.createTile = ModContent.TileType<Content.Tiles.Shimmer.ArcaneChandelier>();
		}
	}
}