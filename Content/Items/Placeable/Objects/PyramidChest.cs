using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Content.Tiles.Objects.Furniture;

namespace Remnants.Content.Items.Placeable.Objects
{
	public class PyramidChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Chest);
			Item.width = 16;
			Item.height = 14;
            Item.createTile = ModContent.TileType<Tiles.Objects.Furniture.PyramidChest>();
		}
	}
}