using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Underworld
{
	public class TuffBrick : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            DustType = DustID.Ash;
            HitSound = SoundID.Dig;

            AddMapEntry(new Color(99, 85, 84));

            VanillaFallbackOnModDeletion = TileID.IridescentBrick;
        }
    }
}