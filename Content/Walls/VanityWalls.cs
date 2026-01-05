using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Walls.Underworld;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
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

            VanillaFallbackOnModDeletion = WallID.Wood;
        }
	}
	public class WoodSafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/Wood";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.Dirt;
			RegisterItemDrop(ItemID.WoodWall);
			AddMapEntry(new Color(66, 45, 35));

            VanillaFallbackOnModDeletion = WallID.Wood;
        }
	}

	[LegacyName("woodlattice")]
	public class WoodLattice : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;
			DustType = 0;
			RegisterItemDrop(ItemID.WoodenFence);

			//AddMapEntry(new Color(66, 45, 35));
   //         AddMapEntry(new Color(0, 0, 0, 0));
        }

   //     public override ushort GetMapOption(int i, int j)
   //     {
			//return (ushort)((i + j) % 2);
   //     }
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

            VanillaFallbackOnModDeletion = WallID.BorealWood;
        }
	}
	public class WoodBorealSafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/WoodBoreal";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.BorealWood;
			RegisterItemDrop(ItemID.BorealWoodWall);
			AddMapEntry(new Color(79, 59, 51));

            VanillaFallbackOnModDeletion = WallID.BorealWood;
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

            VanillaFallbackOnModDeletion = WallID.RichMaogany;
        }
	}

	public class WoodMahoganySafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/WoodMahogany";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.RichMahogany;
			RegisterItemDrop(ItemID.RichMahoganyWall);
			AddMapEntry(new Color(69, 42, 34));

            VanillaFallbackOnModDeletion = WallID.RichMaogany;
        }
	}

    [LegacyName("stonebrick")]
	[LegacyName("brickstone")]
	public class BrickStone : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = DustID.Stone;
			RegisterItemDrop(ItemID.GrayBrickWall);
			AddMapEntry(new Color(42, 41, 44));

            VanillaFallbackOnModDeletion = WallID.GrayBrick;
        }
	}
	public class BrickStoneSafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/BrickStone";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.Stone;
			RegisterItemDrop(ItemID.GrayBrickWall);
			AddMapEntry(new Color(42, 41, 44));

            VanillaFallbackOnModDeletion = WallID.GrayBrick;
        }
	}

	[LegacyName("icetemple")]
	[LegacyName("brickice")]
	public class BrickIce : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = DustID.Ice;
			RegisterItemDrop(ItemID.IceBrickWall);
			AddMapEntry(new Color(71, 95, 131));

            VanillaFallbackOnModDeletion = WallID.IceBrick;
        }
	}
	public class BrickIceSafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/BrickIce";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = DustID.Ice;
			RegisterItemDrop(ItemID.IceBrickWall);
			AddMapEntry(new Color(71, 95, 131));

            VanillaFallbackOnModDeletion = WallID.IceBrick;
        }
	}
}
