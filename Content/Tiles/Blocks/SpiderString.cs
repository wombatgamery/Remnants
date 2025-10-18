//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Remnants.Content.Tiles.Blocks;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace Remnants.Content.Tiles.Blocks
//{
//	public class SpiderString : ModTile
//	{
//		public override void SetStaticDefaults()
//		{
//			Main.tileFrameImportant[Type] = false;
//			Main.tileLavaDeath[Type] = true;

//			TileID.Sets.DisableSmartCursor[Type] = true;

//			AddMapEntry(new Color(135, 132, 150));
//			DustType = DustID.Web;

//            VanillaFallbackOnModDeletion = TileID.Cobweb;
//        }

//        public override bool CanDrop(int i, int j)
//        {
//			return false;
//        }

//        public override bool CanPlace(int i, int j)
//        {
//            if (!Main.tile[i, j - 1].HasTile)
//            {
//                return false;
//            }
//            else if (!Main.tileSolid[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].TileType != Type)
//            {
//                return false;
//            }
//            else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeUpRight)
//            {
//                return false;
//            }

//			return true;
//        }

//		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
//		{
//			Tile tile = Main.tile[i, j];

//			bool openBottom = false;

//            if (!Main.tile[i, j + 1].HasTile)
//            {
//                openBottom = true;
//            }
//            else if (!Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].TileType != Type)
//            {
//                openBottom = true;
//            }
//            else if (Main.tile[i, j + 1].Slope == SlopeType.SlopeDownLeft || Main.tile[i, j + 1].Slope == SlopeType.SlopeDownRight || Main.tile[i, j + 1].IsHalfBlock)
//            {
//                openBottom = true;
//            }

//            if (openBottom)
//            {
//                tile.TileFrameX = (short)(18 * Main.rand.Next(9, 12));
//                tile.TileFrameY = 18 * 4;

//                return false;
//            }

//            return true;
//		}

//        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
//        {
//            if (Lighting.GetColor(i, j) == Color.Black)
//            {
//                return;
//            }

//            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

//            if (WorldGen.SolidTile(i, j - 1))
//            {
//                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Blocks/SpiderEggMerge").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, (j - 1) * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(16, 0, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//            }
//            if (WorldGen.SolidTile(i, j + 1))
//            {
//                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Blocks/SpiderEggMerge").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, (j + 1) * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(16, 32, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//            }
//        }

//        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
//        {
//            if (i % 2 == 1)
//            {
//                spriteEffects = SpriteEffects.FlipHorizontally;
//            }
//        }
//    }
//}