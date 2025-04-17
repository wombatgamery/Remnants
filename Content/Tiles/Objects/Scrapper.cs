using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Placeable;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects
{
	public class Scrapper : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(107, 97, 92), CreateMapEntryName());
			DustType = 8;
		}

		//public override bool RightClick(int i, int j)
		//{
		//	Player player = Main.LocalPlayer;
		//	int item = player.HeldItem.type;

		//	if (item != 0)
  //          {
		//		int output = 0;
		//		bool outputIron = false;

		//		if (item == ItemID.MechanicalWorm || item == ItemID.MechanicalEye || item == ItemID.MechanicalSkull)
		//		{
		//			output = 5;
		//		}
		//		if (item == ItemID.IronShortsword || item == ItemID.LeadShortsword || item == ItemID.IronBow || item == ItemID.LeadBow)
		//		{
		//			output = 7;
		//		}
		//		if (item == ItemID.IronBroadsword || item == ItemID.LeadBroadsword || item == ItemID.IronAxe || item == ItemID.LeadAxe || item == ItemID.IronHammer || item == ItemID.LeadHammer)
		//		{
		//			output = 8;
		//		}
		//		if (item == ItemID.IronPickaxe || item == ItemID.LeadPickaxe)
		//		{
		//			output = 10;
		//		}
		//		if (item == ItemID.IronHelmet || item == ItemID.LeadHelmet)
		//		{
		//			output = 15;
		//		}
		//		if (item == ItemID.IronGreaves || item == ItemID.LeadGreaves || item == ItemID.Flamethrower)
		//		{
		//			output = 20;
		//		}
		//		if (item == ItemID.IronChainmail || item == ItemID.LeadChainmail)
		//		{
		//			output = 25;
		//		}

		//		if (output > 0)
		//		{
		//			player.HeldItem.stack -= 1;
		//			if (player.HeldItem.stack <= 0)
		//			{
		//				player.HeldItem.TurnToAir();
		//			}

		//			Item.NewItem(i * 16, j * 16, 48, 32, ModContent.ItemType<metalscraps>(), output * 2);
		//			SoundEngine.PlaySound(SoundID.Grab, i * 16, j * 16);
		//		}
		//	}

		//	return true;
  //      }

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];

			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<SalvagedMetal>();
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            //if (Main.drawToScreen)
            //{
            //    zero = Vector2.Zero;
            //}
            //int height = tile.TileFrameY == 36 ? 18 : 16;
            //Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/recyclerglow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 + 2 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
			Color color = new Color(100, 100, 100, 0);
			int frameX = Main.tile[i, j].TileFrameX;
			int frameY = Main.tile[i, j].TileFrameY;
			int offsetY = 2;
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			for (int k = 0; k < 7; k++)
			{
				float x = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float y = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Objects/recyclerflame").Value, new Vector2(i * 16 - (int)Main.screenPosition.X + x, j * 16 - (int)Main.screenPosition.Y + offsetY + y) + zero, new Rectangle(frameX, frameY, 16, 16), color, 0f, default, 1f, SpriteEffects.None, 0f);
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.8f;
			g = 0.45f;
			b = 0.15f;
			//r = 0.8f;
			//g = 0.4f;
			//b = 0.4f;
		}
	}
}