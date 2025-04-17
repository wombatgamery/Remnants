using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Content.Items.Consumable;

namespace Remnants.Content.Tiles.Objects.Decoration
{
	public class PyramidPot : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSpelunker[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileOreFinderPriority[Type] = 500;

			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.FriendlyFairyCanLureTo[Type] = true;

			TileObjectData.newTile.FullCopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(238, 194, 156), CreateMapEntryName());

			RegisterItemDrop(ModContent.ItemType<PyramidKey>());
			DustType = DustID.BeachShell;
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
			SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j) * 16);
        }

  //      public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
  //      {
		//	if (Main.tile[i, j].TileFrameY == 0)
		//	{
		//		r = 0.2f;
		//		g = 0.1f;
		//		b = 0f;
		//	}
		//}

  //      public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
  //      {
  //          if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
  //          {
		//		if (Main.rand.NextBool(2))
  //              {
		//			Dust dust = Dust.NewDustPerfect(new Vector2((i + 1) * 16 + Main.rand.Next(-4, 5), j * 16), DustID.Sandnado, new Vector2(0, -1), Scale: Main.rand.NextFloat(0.25f, 1));
		//			dust.noGravity = true;
  //              }
  //          }
  //      }
    }
}