using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
	[LegacyName("poisonrock")]
	public class ToxicWaste : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileBlockLight[Type] = true;
			MineResist = 2f;
			DustType = 75;
			AddMapEntry(new Color(245, 255, 0));
			HitSound = SoundID.NPCHit18;// SoundID.Tink;

			//TileMerge((ushort)ModContent.TileType<poisonrock>(), TileID.Stone);
			//TileMerge((ushort)ModContent.TileType<poisonrock>(), TileID.Mud);
			//TileMerge((ushort)ModContent.TileType<poisonrock>(), TileID.Silt);
			//TileMerge((ushort)ModContent.TileType<poisonrock>(), TileID.Ash);
		}

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;

			if (Vector2.Distance(new Vector2(i * 16 + 8, j * 16 + 8), new Vector2(player.Center.X, player.Center.Y)) < 5 * 16)
		    {
				player.AddBuff(BuffID.Poisoned, 5);
			}
        }

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = (float)(131f / 255f);
			g = (float)(228f / 255f);
			b = (float)(16f / 255f);
		}

		public override bool IsTileDangerous(int i, int j, Player player)
        {
			return true;
        }
	}
}