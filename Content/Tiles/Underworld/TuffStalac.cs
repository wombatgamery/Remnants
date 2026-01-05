using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Tiles.Underworld;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Underworld
{
	public class TuffStalac : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            //Main.tileMerge[ModContent.TileType<Tuff>()][Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.LavaDeath = false;
            //TileObjectData.newTile.RandomStyleRange = 3;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.newAlternate.AnchorTop = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(79, 66, 68));

            DustType = DustID.Ash;
		}
    }

    public class TuffStalacSmall : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            //Main.tileMerge[ModContent.TileType<Tuff>()][Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.LavaDeath = false;
            //TileObjectData.newTile.RandomStyleRange = 3;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(79, 66, 68));

            DustType = DustID.Ash;
        }
    }
}