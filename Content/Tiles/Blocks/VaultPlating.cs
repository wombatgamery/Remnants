using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Remnants.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	[LegacyName("vaultbrick")]
	[LegacyName("vaultbrickrusted")]
	public class VaultPlating : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			//Main.tileMerge[Type][ModContent.TileType<vaultplatform>()] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

			MinPick = 180;
			MineResist = 4;
			DustType = DustID.Iron;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(90, 79, 90));
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			if (BlendingTile(i - 1, j) && BlendingTile(i + 1, j) && BlendingTile(i, j - 1) && BlendingTile(i, j + 1))
			{
				bool top = !BlendingTile(i - 1, j - 1) || !BlendingTile(i + 1, j - 1);
				bool bottom = !BlendingTile(i - 1, j + 1) || !BlendingTile(i + 1, j + 1);

				if (top ^ bottom)
                {
					bool left = !BlendingTile(i - 1, j + (bottom ? 1 : -1));
					bool right = !BlendingTile(i + 1, j + (bottom ? 1 : -1));

					if (left ^ right)
					{
						Tile tile = Main.tile[i, j];

						tile.TileFrameX = (short)(Main.rand.Next(3) * 18 * 2);
						tile.TileFrameY = (short)(18 * 5 + (bottom ? 18 : 0));// (short)(Main.rand.Next(3) * 18);

						if (right)
                        {
							tile.TileFrameX += 18;
                        }

						return false;
					}
				}
			}
			return true;
		}

		private bool BlendingTile(int x, int y)
        {
			Tile tile = Main.tile[x, y];
			return tile.HasTile && (tile.TileType == Type || Main.tileBlendAll[tile.TileType]);
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;
    }

	[LegacyName("vaultpipe")]
	public class VaultPipe : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = false;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

			MinPick = 180;
			MineResist = 4;
			DustType = DustID.Silver;
			HitSound = SoundID.Item52;

			AddMapEntry(new Color(134, 131, 153), CreateMapEntryName());
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;

		public override bool Slope(int i, int j)
		{
			return false;
		}
	}
}
