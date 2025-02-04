using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Plants
{
	public class eldermushroom : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CoordinateWidth = 20;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(77, 55, 62));

			DustType = 8;
		}

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

			bool anchorLeft = true;
			bool anchorRight = true;

			if (!RemTile.SolidRight(i - 1, j))
			{
				anchorLeft = false;
			}
			if (!RemTile.SolidLeft(i + 1, j))
			{
				anchorRight = false;
			}

			if (anchorLeft && anchorRight)
            {
				tile.TileFrameX = (short)((short)Main.rand.Next(2) * 22);
			}
			else if (anchorLeft)
            {
				tile.TileFrameX = 0;
			}
			else if (anchorRight)
            {
				tile.TileFrameX = 22;
			}
			else
            {
				WorldGen.KillTile(i, j);
            }

            return false;
        }

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			tile.HasTile = false;
		}

		public override void PostSetDefaults()
		{
			Main.tileNoSunLight[Type] = false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}