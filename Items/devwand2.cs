//using Microsoft.Xna.Framework;
//using Remnants.Tiles.Blocks;
//using Remnants.Tiles.Objects.Furniture;
//using Remnants.Tiles.Plants;
//using Remnants.Walls;
//using Remnants.Walls.Parallax;
//using Remnants.Worldgen;
//////using SubworldLibrary;
//using Terraria;
//using Terraria.Audio;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Items
//{
//    public class devwand2 : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//        }

//        public int wallType = 1;

//        public override void SetDefaults()
//        {
//            Item.width = 15;
//            Item.height = 13;

//            Item.useStyle = ItemUseStyleID.Swing;
//            Item.useTime = 2;
//            Item.useAnimation = 2;
//            Item.shootSpeed = 16f;

//            Item.autoReuse = true;

//        }

//        public bool was;

//        //public override bool CanUseItem(Player player)
//        //{
//        //	if (player.altFunctionUse == 2)
//        //	{
//        //		item.useStyle = ItemUseStyleID.Stabbing;
//        //		item.useTime = 20;
//        //		item.useAnimation = 20;
//        //		item.damage = 50;
//        //		item.shoot = ProjectileID.Bee;
//        //	}
//        //	else
//        //	{
//        //		item.useStyle = ItemUseStyleID.SwingThrow;
//        //		item.useTime = 40;
//        //		item.useAnimation = 40;
//        //		item.damage = 100;
//        //		item.shoot = ProjectileID.None;
//        //	}
//        //	return base.CanUseItem(player);
//        //}

//        public override bool CanRightClick()
//        {
//            return true;
//        }

//        public override void RightClick(Player player)
//        {
//            //ModContent.GetInstance<Maze>().BuildMaze();
//            //Main.NewText(player.maxRunSpeed);
//            //Main.NewText(player.runAcceleration);
//            //Main.NewText(player.runSlowdown);
//            for (int i = 40; i < Main.maxTilesX - 40; i++)
//            {
//                for (int j = 40; j < Main.maxTilesY; j++)
//                {
//                    if (WGTools.Tile(i, j).TileType == TileID.Platforms && WGTools.Tile(i, j).WallType == ModContent.WallType<MarineSlabWallUnsafe>())
//                    {
//                        WGTools.Tile(i, j).TileType = (ushort)ModContent.TileType<MarinePlatform>();
//                    }
//                    if (WGTools.Tile(i, j).TileType == ModContent.TileType<MarinePlatform>())
//                    {
//                        WGTools.Tile(i, j).TileFrameY = 0;
//                    }
//                    //if (WGTools.Tile(i, j).TileType == ModContent.TileType<VaultPlating>())
//                    //{
//                    //    WGTools.Tile(i, j).WallType = (ushort)ModContent.WallType<VaultWallUnsafe>();
//                    //}
//                    //if (WGTools.GetTile(i, j + 1).WallType == (ushort)ModContent.WallType<mineshaft2>())
//                    //{
//                    //    if (WGTools.GetTile(i - 1, j + 1).WallType != 0 && WGTools.GetTile(i + 1, j + 1).WallType != 0)
//                    //    {
//                    //        WGTools.GetTile(i, j + 1).WallType = WallID.Wood;
//                    //    }
//                    //}
//                    //if (WGTools.GetTile(i, j).WallType == WallID.Dirt)
//                    //{
//                    //    WGTools.GetTile(i, j).WallType = 0;
//                    //}
//                    ////else if (WGTools.GetTile(i, j).WallType == WallID.RichMaogany || WGTools.GetTile(i, j).TileType == TileID.RichMahogany && WGTools.GetTile(i, j).WallType == 0)
//                    ////{
//                    ////    WGTools.GetTile(i, j).WallType = (ushort)ModContent.WallType<junglefortress1>();
//                    ////}
//                    ////if (WGTools.GetTile(i, j).WallType == ModContent.WallType<vaultwallunsafe>() || WGTools.GetTile(i, j).WallType == ModContent.WallType<hardstonewallvault>())
//                    ////{
//                    ////    WGTools.GetTile(i, j).WallType = (ushort)ModContent.WallType<vault>();
//                    ////}
//                    ///
//                    //if (WGTools.Tile(i, j).TileType == ModContent.TileType<LabyrinthWallLamp>())
//                    //{
//                    //    WGTools.Tile(i, j).WallType = (ushort)ModContent.WallType<LabyrinthTileWall>();
//                    //}


//                    //ModContent.GetInstance<Maze>().BuildMaze();
//                    //if (WGTools.Tile(i, j).TileType == ModContent.TileType<mazegrass>() || WGTools.Tile(i, j).TileType == ModContent.TileType<mazevine>())
//                    //{
//                    //    WorldGen.KillTile(i, j);
//                    //    WGTools.Tile(i, j).TileType = (ushort)ModContent.TileType<goldenpanel>();
//                    //}
//                }
//            }

//            //SubworldSystem.Enter<VaultSubworld>();

//            SoundEngine.PlaySound(SoundID.NPCHit42, new Vector2((int)player.position.X, (int)player.position.Y));
//            SoundEngine.PlaySound(SoundID.NPCDeath43, new Vector2((int)player.position.X, (int)player.position.Y));
//        }
//    }
//}