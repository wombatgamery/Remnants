using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.Walls.Underworld;
using Remnants.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Underworld.Prototypes
{
    public class PrototypeMonitor : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            MinPick = 200;
            MineResist = 1;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(20, 75, 102));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            if (tile.WallType == 0)
            {
                WorldGen.KillTile(i, j);
                return false;
            }
            else
            {
                if (BlendingTile(i - 1, j) && BlendingTile(i + 1, j) && BlendingTile(i, j - 1) && BlendingTile(i, j + 1))
                {
                    bool top = !BlendingTile(i - 1, j - 1) || !BlendingTile(i + 1, j - 1);
                    bool bottom = !BlendingTile(i - 1, j + 1) || !BlendingTile(i + 1, j + 1);

                    if (top ^ bottom)
                    {
                        bool left = !BlendingTile(i - 1, j + (bottom ? 1 : -1));
                        bool right = !BlendingTile(i + 1, j + (bottom ? 1 : -1));

                        if (left ^ right)
                        {
                            tile.TileFrameX = (short)(Main.rand.Next(3) * 18 * 2);
                            tile.TileFrameY = (short)(18 * 5 + (bottom ? 18 : 0));// (short)(Main.rand.Next(3) * 18);

                            if (right)
                            {
                                tile.TileFrameX += 18;
                            }

                            if (i % 2 == 1)
                            {
                                tile.TileFrameY += 18 * 2;
                            }

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool BlendingTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == Type || Main.tileMerge[Type][tile.TileType]);
        }

        public override bool CanPlace(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            return tile.WallType != 0;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
            {
                Tile tile = Main.tile[i, j];

                if (tile.TileFrameY == 18)
                {
                    if (tile.TileFrameX >= 18 && tile.TileFrameX <= 18 * 3 && Main.rand.NextBool(20))
                    {
                        tile.TileFrameX = (short)(18 * Main.rand.Next(1, 4));
                        NetMessage.SendTileSquare(-1, i, j);
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

                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 18 * 7, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (tile.TileFrameY == 18)
                {
                    if (tile.TileFrameX == 18)
                    {
                        bool connect = Main.tile[i - 1, j].TileFrameX == 18 && Main.tile[i - 1, j].TileFrameY == 18;

                        Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 + (!connect ? 2 : 0) - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(18 * 6 + ((int)(Main.GameUpdateCount / 4 + (i - j) * 16) * 2 + (!connect ? 2 : 0)) % 96, 18 * 5, !connect ? 14 : 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    else if (tile.TileFrameX == 18 * 3)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 + 2 + x * 4 - (int)Main.screenPosition.X, j * 16 + 2 + y * 4 - (int)Main.screenPosition.Y) + zero, new Rectangle(16, 0, 2, 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                    }
                }
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
