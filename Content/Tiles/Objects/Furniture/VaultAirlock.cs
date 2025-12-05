//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;
//using Terraria.ObjectData;
//using Terraria.Enums;

//namespace Remnants.Content.Tiles.Objects.Furniture
//{
//	public class VaultAirlock : ModTile
//	{
//        public override void SetStaticDefaults()
//		{
//            Main.tileFrameImportant[Type] = true;
//            Main.tileNoAttach[Type] = true;

//            TileID.Sets.DisableSmartCursor[Type] = true;
//            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
//            TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;
//            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

//            TileObjectData.newTile.Width = 7;
//            TileObjectData.newTile.Height = 11;
//            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
//            TileObjectData.newTile.CoordinateWidth = 16;
//            TileObjectData.newTile.CoordinatePadding = 0;
//            TileObjectData.newTile.Origin = new Point16(3, 16);
//            TileObjectData.newTile.UsesCustomCanPlace = true;
//            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
//            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
//            TileObjectData.addTile(Type);

//            MinPick = 210;
//            DustType = DustID.Iron;
//			HitSound = SoundID.Tink;

//            AddMapEntry(new Color(255, 255, 63), Language.GetText("MapObject.Door"));

//            VanillaFallbackOnModDeletion = TileID.IronBrick;
//        }
//    }
//}