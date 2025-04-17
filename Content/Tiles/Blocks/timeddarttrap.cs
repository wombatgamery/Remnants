//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.ID;
//using Terraria.DataStructures;
//using Terraria.Enums;
//using Terraria.ModLoader;
//using Terraria.ObjectData;
//using Remnants.Content.Tiles.Objects;
//using Remnants.Content.Dusts;

//namespace Remnants.Content.Tiles.Blocks
//{
//	public class timeddarttrapA : ModTile
//	{
//		public override void SetStaticDefaults()
//		{
//			Main.tileSolid[Type] = true;
//			Main.tileFrameImportant[Type] = true;
//			Main.tileBlockLight[Type] = false;
			
//			TileID.Sets.DisableSmartCursor[Type] = true;
//			TileID.Sets.CanBeSloped[Type] = false;

//			TileObjectData.newTile.FullCopyFrom(TileID.Traps);
//			TileObjectData.addTile(Type);

//			DustType = DustID.Dirt;
//			HitSound = SoundID.Tink;

//			AddMapEntry(new Color(89, 80, 75));
//		}

//        public override void NearbyEffects(int i, int j, bool closer)
//        {
//			if (Main.GameUpdateCount % 120 == 0)
//            {
//				Tile tile = Main.tile[i, j];

//				Vector2 position = new Vector2(i * 16 + 8, j * 16 + 9);
//				Vector2 velocity = Vector2.Zero;

//				int num142 = tile.TileFrameX != 0 ? 1 : -1;

//				velocity.X = 12 * num142;
//				position.X += 10 * num142;

//				Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), position, velocity, ProjectileID.PoisonDart, 20, 2f, Main.myPlayer);
//			}
//        }

//		public override bool IsTileDangerous(int i, int j, Player player)
//		{
//			return true;
//		}
//	}

//	public class timeddarttrapB : ModTile
//	{
//		public override void SetStaticDefaults()
//		{
//			Main.tileSolid[Type] = true;
//			Main.tileFrameImportant[Type] = true;
//			Main.tileBlockLight[Type] = false;

//			TileID.Sets.DisableSmartCursor[Type] = true;
//			TileID.Sets.CanBeSloped[Type] = false;

//			TileObjectData.newTile.FullCopyFrom(TileID.Traps);
//			TileObjectData.addTile(Type);

//			DustType = DustID.Dirt;
//			HitSound = SoundID.Tink;

//			AddMapEntry(new Color(89, 80, 75));
//		}

//		public override void NearbyEffects(int i, int j, bool closer)
//		{
//			if ((Main.GameUpdateCount + 60) % 120 == 0)
//			{
//				Tile tile = Main.tile[i, j];

//				Vector2 position = new Vector2(i * 16 + 8, j * 16 + 9);
//				Vector2 velocity = Vector2.Zero;

//				int num142 = tile.TileFrameX != 0 ? 1 : -1;

//				velocity.X = 12 * num142;
//				position.X += 10 * num142;

//				Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), position, velocity, ProjectileID.PoisonDart, 20, 2f, Main.myPlayer);
//			}
//		}

//        public override bool IsTileDangerous(int i, int j, Player player)
//		{
//			return true;
//		}
//	}
//}