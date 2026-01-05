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
    public class PrototypeServer : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.IsBeam[Type] = true;

            MinPick = 200;
            MineResist = 1;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(54, 48, 61));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

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

            return true;
        }

        private bool BlendingTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == Type || Main.tileMerge[Type][tile.TileType]);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + 18 * 7, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (MiscTools.HasTile(i, j + 1, Type) && (MiscTools.HasTile(i - 1, j, Type) || MiscTools.HasTile(i + 1, j, Type)))
            {
                Tile tile = Main.tile[i, j];

                r = 17 / 255f / 4;
                g = 178 / 255f / 4;
                b = 177 / 255f / 4;
            }
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }
}
