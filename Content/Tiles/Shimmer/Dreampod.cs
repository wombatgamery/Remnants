using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Content.Tiles;
using Remnants.Content.Dusts;
using Remnants.Content.Items.Placeable.Plants;
using System.Collections.Generic;
using Remnants.Content.Items.Materials;
using Remnants.Content.Projectiles;

namespace Remnants.Content.Tiles.Shimmer
{
	[LegacyName("dreampodhead")]
	public class Dreampod : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CoordinateHeights = new[] { 20 };
			TileObjectData.newTile.CoordinateWidth = 20;
			TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(255, 255, 255), CreateMapEntryName());

			DustType = DustID.VioletMoss;
			HitSound = SoundID.Grass;
		}

        //    public override void RandomUpdate(int i, int j)
        //    {
        //        Tile tile = Main.tile[i, j];

        //        if (!Main.tile[i, j + 1].HasTile && Main.rand.NextBool(10))
        //        {
        //            bool maxLength = true;

        //            for (int a = 0; a < 10; a++)
        //            {
        //                if (Main.tile[i, j - a].TileType != ModContent.TileType<DreampodVine>() && Main.tile[i, j - a].TileType != ModContent.TileType<Dreampod>())
        //                {
        //                    maxLength = false;
        //		break;
        //                }
        //            }

        //            if (!maxLength)
        //            {
        //	tile.TileType = (ushort)ModContent.TileType<DreampodVine>();
        //	WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Dreampod>(), true);

        //	tile.TileFrameX = (short)(Main.rand.Next(3) * 18);
        //	Main.tile[i, j + 1].TileFrameX = (short)(Main.rand.Next(3) * 20);
        //}
        //        }
        //    }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            bool anchor = true;

            if (!Main.tile[i, j - 1].HasTile)
            {
                anchor = false;
            }
            else if (Main.tile[i, j - 1].TileType != TileID.VioletMoss && !Main.tileSolid[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].TileType != ModContent.TileType<DreampodVine>())
            {
                anchor = false;
            }
            else if (Main.tile[i, j - 1].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j - 1].Slope == SlopeType.SlopeUpRight)
            {
                anchor = false;
            }

            if (!anchor)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			//Color color = RemTile.MagicalLabLightColour(i);
			//r = color.R / 255f;
			//g = color.G / 255f;
			//b = color.B / 255f;
            r = (float)(255f / 255f);
            g = (float)(119f / 255f);
            b = (float)(232f / 255f);
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
			Projectile proj = Projectile.NewProjectileDirect(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16 + Vector2.One * 8 + Vector2.UnitY * 2, Vector2.Zero, ModContent.ProjectileType<FallingDreampod>(), 0, 0);
			proj.frame = Main.tile[i, j].TileFrameX / 20;
			NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			SpriteEffects effect = SpriteEffects.None;
			if (i % 2 == 1)
			{
				effect = SpriteEffects.FlipHorizontally;
			}
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X - 2, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 20, 20), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
		}
	}

	[LegacyName("dreampodvine")]
	public class DreampodVine : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;

            Main.tileMerge[Type][ModContent.TileType<Dreampod>()] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

			AddMapEntry(new Color(181, 68, 206));
			AddMapEntry(new Color(255, 119, 232));

			DustType = DustID.VioletMoss;
			HitSound = SoundID.Grass;
		}

        public override bool CanDrop(int i, int j)
        {
            return false;
        }

        public override ushort GetMapOption(int i, int j)
		{
			return (ushort)(!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != ModContent.TileType<DreampodVine>() ? 1 : 0);
		}

        public override bool CanPlace(int i, int j)
        {
            if (!Main.tile[i, j - 1].HasTile)
            {
                return false;
            }
            else if (Main.tile[i, j - 1].TileType != TileID.VioletMoss)
            {
                return false;
            }
            else if (Main.tile[i, j - 1].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j - 1].Slope == SlopeType.SlopeUpRight)
            {
                return false;
            }

            return true;
        }

        public override void RandomUpdate(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			if (Main.rand.NextBool(20) && Main.tile[i, j].LiquidAmount == 0)
			{
				if (!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != Type && Main.tile[i, j + 1].TileType != ModContent.TileType<Dreampod>())
				{
                    tile.HasTile = false;
                    WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<Dreampod>(), style: Main.rand.Next(3));
                }
			}
			else if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].LiquidAmount == 0)
			{
				bool maxLength = true;

				for (int a = 0; a < 20; a++)
				{
					if (Main.tile[i, j - a].TileType != Type)
					{
						maxLength = false;
						break;
					}
				}

				if (!maxLength)
				{
					WorldGen.PlaceTile(i, j + 1, Type, true);
				}
			}
		}

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            bool anchor = true;

            if (!Main.tile[i, j - 1].HasTile)
            {
                anchor = false;
            }
            else if (Main.tile[i, j - 1].TileType != TileID.VioletMoss && !Main.tileSolid[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].TileType != ModContent.TileType<DreampodVine>())
            {
                anchor = false;
            }
            else if (Main.tile[i, j - 1].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j - 1].Slope == SlopeType.SlopeUpRight)
            {
                anchor = false;
            }

            if (!anchor)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = (float)(181f / 255f);
			g = (float)(68f / 255f);
			b = (float)(206f / 255f);
		}

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            SpriteEffects effect = SpriteEffects.None;
            if (i % 2 == 1)
            {
                effect = SpriteEffects.FlipHorizontally;
            }
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
        }
    }
}