using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Remnants.Items.Placeable.Objects
{
	public class LabyrinthWallLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Switch);
			Item.width = 9 * 2;
			Item.height = 9 * 2;
			Item.createTile = ModContent.TileType<Tiles.Objects.Furniture.LabyrinthWallLamp>();
		}
	}
}