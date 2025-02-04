using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Walls.Vanity
{
	[LegacyName("wood")]
	public class Wood : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = DustID.Dirt;
			RegisterItemDrop(ItemID.WoodWall);
			AddMapEntry(new Color(66, 45, 35));
		}
	}
	public class WoodSafe : ModWall
	{
		public override string Texture => "Remnants/Walls/Vanity/Wood";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.Dirt;
			RegisterItemDrop(ItemID.WoodWall);
			AddMapEntry(new Color(66, 45, 35));
		}
	}

	[LegacyName("woodlattice")]
	public class WoodLattice : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;
			Main.wallBlend[Type] = 2;
			DustType = 0;
			RegisterItemDrop(ItemID.WoodenFence);
		}
	}

	[LegacyName("woodboreal")]
	public class WoodBoreal : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = DustID.BorealWood;
			RegisterItemDrop(ItemID.BorealWoodWall);
			AddMapEntry(new Color(79, 59, 51));
		}
	}
	public class WoodBorealSafe : ModWall
	{
		public override string Texture => "Remnants/Walls/Vanity/WoodBoreal";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.BorealWood;
			RegisterItemDrop(ItemID.BorealWoodWall);
			AddMapEntry(new Color(79, 59, 51));
		}
	}

	[LegacyName("woodmahogany")]
	public class WoodMahogany : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = DustID.RichMahogany;
			RegisterItemDrop(ItemID.RichMahoganyWall);
			AddMapEntry(new Color(69, 42, 34));
		}
	}

	public class WoodMahoganySafe : ModWall
	{
		public override string Texture => "Remnants/Walls/Vanity/WoodMahogany";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.RichMahogany;
			RegisterItemDrop(ItemID.RichMahoganyWall);
			AddMapEntry(new Color(69, 42, 34));
		}
	}

	[LegacyName("stonebrick")]
	[LegacyName("brickstone")]
	public class BrickStone : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			Main.wallBlend[Type] = 1;
			DustType = DustID.Stone;
			RegisterItemDrop(ItemID.GrayBrickWall);
			AddMapEntry(new Color(42, 41, 44));
		}
	}
	public class BrickStoneSafe : ModWall
	{
		public override string Texture => "Remnants/Walls/Vanity/BrickStone";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallBlend[Type] = 1;
			DustType = DustID.Stone;
			RegisterItemDrop(ItemID.GrayBrickWall);
			AddMapEntry(new Color(42, 41, 44));
		}
	}

	[LegacyName("icetemple")]
	[LegacyName("brickice")]
	public class BrickIce : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			Main.wallBlend[Type] = 1;
			DustType = DustID.Ice;
			RegisterItemDrop(ItemID.IceBrickWall);
			AddMapEntry(new Color(71, 95, 131));
		}
	}
	public class BrickIceSafe : ModWall
	{
		public override string Texture => "Remnants/Walls/Vanity/BrickIce";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallBlend[Type] = 1;
			DustType = DustID.Ice;
			RegisterItemDrop(ItemID.IceBrickWall);
			AddMapEntry(new Color(71, 95, 131));
		}
	}
}
