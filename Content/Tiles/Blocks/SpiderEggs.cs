using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
    public class SpiderEggs : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<InsectRemains>()] = true;
            Main.tileMerge[ModContent.TileType<InsectRemains>()][Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][TileID.ClayBlock] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Sulfurstone>()] = true;
            Main.tileMerge[ModContent.TileType<Sulfurstone>()][Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            DustType = DustID.GreenBlood;
			HitSound = SoundID.NPCHit18;

			AddMapEntry(new Color(160, 160, 178));

            VanillaFallbackOnModDeletion = TileID.Spider;
        }

        public override bool CanDrop(int i, int j) => false;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust dust = Dust.NewDustPerfect(new Vector2(i + 0.5f, j + 0.5f) * 16, ModContent.DustType<Spiderling>());
                }
            }
        }

        //public override bool HasWalkDust() => true;

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.Web;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, new Vector2(i + 0.5f, j + 0.5f) * 16);
            }
            return false;
        }
    }
}
