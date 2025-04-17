using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects
{
	[LegacyName("placedminecart")]
	public class Minecart : ModTile
	{
        public override string Texture => "Terraria/Images/Mount_Minecart";

        public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.CanDropFromRightClick[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(138, 121, 102), Language.GetText("ItemName.Minecart"));

			RegisterItemDrop(ItemID.Minecart);
			DustType = 8;
			HitSound = SoundID.Dig;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];

			player.cursorItemIconID = ItemID.Minecart;
			player.cursorItemIconText = "";

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}

	public class Shroomcart : ModTile
	{
		public override string Texture => "Terraria/Images/Extra_115";

		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.CanDropFromRightClick[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(138, 121, 102), Language.GetText("ItemName.ShroomMinecart"));

			RegisterItemDrop(ItemID.ShroomMinecart);
			DustType = DustID.GlowingMushroom;
			HitSound = SoundID.Dig;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];

			player.cursorItemIconID = ItemID.ShroomMinecart;
			player.cursorItemIconText = "";

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}