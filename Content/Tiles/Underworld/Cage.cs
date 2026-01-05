using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Underworld
{
    public class Cage : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = false;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = false;

            DustType = DustID.Iron;
            HitSound = SoundID.Item52;// Tink;

            AddMapEntry(new Color(92, 82, 83));

            VanillaFallbackOnModDeletion = TileID.Grate;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (WorldGen.gen)
            {
                fail = true;
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;
    }
}
