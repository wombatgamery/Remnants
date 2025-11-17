using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	public class Lettering : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;

			TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
			
			MineResist = 0.5f;
			DustType = DustID.Paint;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(102, 102, 102));

            VanillaFallbackOnModDeletion = TileID.Bubble;
        }

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			Tile tile = Main.tile[i, j];

            if (tile.WallType == 0 || Main.wallLight[tile.WallType])
			{
				WorldGen.KillTile(i, j);
				return false;
			}

			return true;
        }

        public override bool CanPlace(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            return tile.WallType != 0 && !Main.wallLight[tile.WallType];
        }
	}
}