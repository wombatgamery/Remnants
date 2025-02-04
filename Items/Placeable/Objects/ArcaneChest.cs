using Microsoft.Xna.Framework;
using Remnants.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles.Objects.Furniture;

namespace Remnants.Items.Placeable.Objects
{
	public class ArcaneChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Chest);
			Item.width = 44;
            Item.createTile = ModContent.TileType<Tiles.Objects.Furniture.ArcaneChest>();
		}
	}
}