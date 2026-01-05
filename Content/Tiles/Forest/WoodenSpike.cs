using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Forest
{
    public class WoodenSpike : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;

			TileID.Sets.TouchDamageImmediate[Type] = 40;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 34 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 4;
			TileObjectData.newTile.DrawYOffset = -18;
			TileObjectData.addTile(Type);

			DustType = DustID.WoodFurniture;

			AddMapEntry(new Color(160, 107, 80));

            VanillaFallbackOnModDeletion = TileID.Spikes;
        }

        public override bool CanDrop(int i, int j) => false;

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}