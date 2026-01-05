using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Underworld.Prototypes
{
	public class PrototypeMonitorDiagram : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileMerge[ModContent.TileType<PrototypeMonitor>()][Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.Origin = new Point16(3, 9);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

            MinPick = 200;
            MineResist = 1;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(20, 75, 102));
		}

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
            {
                Tile tile = Main.tile[i, j];

                if (tile.TileFrameY >= 176)
                {
                    if (Main.rand.NextBool(10) || !Main.wallHouse[tile.WallType] && RemSystem.vaultLightIntensity < 1)
                    {
                        tile.TileFrameY -= 176;
                    }
                }
                else if (Main.rand.NextBool(100) && (Main.wallHouse[tile.WallType] || RemSystem.vaultLightIntensity == 1))
                {
                    if (tile.TileFrameY > 0 && tile.TileFrameY < 160 && tile.TileFrameX % 112 > 0 && tile.TileFrameX % 112 < 96)
                    {
                        tile.TileFrameY += 176;
                    }
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (Main.wallHouse[tile.WallType] || RemSystem.vaultLightIntensity == 1)
            {
                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                if (Main.drawToScreen)
                {
                    zero = Vector2.Zero;
                }

                int frame = tile.TileFrameX >= 7 * 16 ? 0 : (int)(Main.GameUpdateCount / 300) % 4 + 1;

                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + frame * 7 * 16, tile.TileFrameY % 176 + 11 * 16, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (tile.TileFrameY >= 176)
                {
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Pointer").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                fail = true;
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<PrototypeMonitor>();
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (Main.wallHouse[tile.WallType] || RemSystem.vaultLightIntensity == 1)
            {
                r = 17 / 255f;
                g = 178 / 255f;
                b = 177 / 255f;
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }
}