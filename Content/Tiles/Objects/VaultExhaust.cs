using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.Gores;
using Remnants.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Objects
{
    public class VaultExhaust : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true; 
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = false;

            MinPick = 200;
            MineResist = 4;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(66, 64, 88));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            if (!WorldGen.SolidTile(i, j - 1) || !WorldGen.SolidTile(i, j + 1) || tile.TileFrameX == 0 && !WorldGen.SolidTile(i + 1, j) || tile.TileFrameX == 16 && !WorldGen.SolidTile(i - 1, j))
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }

        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (RemSystem.exhaustIntensity > 0)
            {
                bool left = Main.tile[i, j].TileFrameX == 0;

                for (int k = 0; k < 4; k++)
                {
                    if (Main.rand.NextFloat(1f) < RemSystem.exhaustIntensity)
                    {
                        Dust dust = Dust.NewDustPerfect(new Vector2((i * 16) + Main.rand.NextFloat(7f, 9f), j * 16 + 8), DustID.Smoke, new Vector2(Main.rand.NextFloat(0, RemSystem.exhaustIntensity * 24) * (left ? -1 : 1), Main.rand.NextFloat(-2, 0)), 255 - (int)(RemSystem.exhaustIntensity * 25), Scale: Main.rand.NextFloat(2, 4));
                        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                    if (Main.rand.NextFloat(1f) < RemSystem.exhaustIntensity)
                    {
                        Gore gore = Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), new Vector2((i * 16) - 16 + (left ? -16 : 16) + Main.rand.NextFloat(7f, 9f), j * 16 - 8), new Vector2(Main.rand.NextFloat(0, RemSystem.exhaustIntensity * 24) * (left ? -1 : 1), 0), Main.rand.Next(220, 222), Main.rand.NextFloat(1, 2));
                        gore.alpha = 255 - (int)(RemSystem.exhaustIntensity * 50);
                        gore.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                }

                //if (Main.rand.NextFloat(64f) < RemSystem.exhaustIntensity)
                //{
                //    Vector2 position = new Vector2(i + 0.5f, j + 0.5f) * 16;
                //    Vector2 velocity = Main.rand.NextVector2Circular(2f, 1f);
                //    Gore gore = Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), position, velocity, ModContent.GoreType<ToxicFog>(), Main.rand.NextFloat(4, 8));
                //    gore.position -= new Vector2(6f * gore.scale, 3f * gore.scale);
                //}
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (RemSystem.exhaustAlarm)
            {
                r = 1;
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

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, RemSystem.exhaustAlarm ? 16 : 32, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }
}
