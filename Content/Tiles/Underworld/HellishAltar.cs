using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Underworld
{
	public class HellishAltar : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileTable[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 22 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

            MinPick = 110;

            DustType = DustID.Ash;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(160, 127, 127));
        }
	}
}