using Microsoft.Xna.Framework;
using Remnants.Content.Biomes;
using Remnants.Content.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	public class SulfurstoneBrick : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Sulfurstone>()] = true;
            Main.tileMerge[ModContent.TileType<Sulfurstone>()][Type] = true;

            DustType = DustID.Stone;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(136, 130, 109));

            VanillaFallbackOnModDeletion = TileID.GrayBrick;
        }

        public override void PostSetDefaults()
        {
            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                if (Main.tileMerge[ModContent.TileType<Sulfurstone>()][i])
                {
                    Main.tileMerge[Type][i] = true;
                }
                if (Main.tileMerge[i][ModContent.TileType<Sulfurstone>()])
                {
                    Main.tileMerge[i][Type] = true;
                }
            }
        }
    }
}