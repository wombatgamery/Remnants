using Microsoft.Xna.Framework;
using Remnants.Content.Biomes;
using Remnants.Content.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.DesertRuins
{
	public class PyramidBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.AvoidedByMeteorLanding[Type] = true;

			MinPick = 65;
			MineResist = 2;
			DustType = DustID.Sand;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(207, 136, 79));

            VanillaFallbackOnModDeletion = TileID.SandstoneBrick;
        }

		//public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;

   //     public override void NearbyEffects(int i, int j, bool closer)
   //     {
			//if (Main.tile[i, j].WallType == ModContent.WallType<PyramidBrickWallUnsafe>())
			//{
   //             if (Main.tile[i, j].IsActuated)
   //             {
   //                 if (closer)
   //                 {
   //                     return;
   //                 }

   //                 foreach (var player in Main.ActivePlayers)
   //                 {
   //                     if (player.HasBuff(BuffID.Suffocation))
   //                     {
   //                         Framing.GetTileSafely(i, j).IsActuated = false;
   //                         NetMessage.SendTileSquare(-1, i, j);
   //                     }
   //                 }
   //             }
   //         }
   //     }
    }
}