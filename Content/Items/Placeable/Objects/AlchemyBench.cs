using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class AlchemyBench : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fireplace);
			Item.rare = 0;
			Item.createTile = ModContent.TileType<Tiles.Objects.Furniture.AlchemyBench>();
		}
	}
}