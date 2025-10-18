using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Blocks
{
    public class InsectRemains : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][TileID.ClayBlock] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Sulfurstone>()] = true;
            Main.tileMerge[ModContent.TileType<Sulfurstone>()][Type] = true;

            DustType = DustID.Shadewood;
			HitSound = SoundID.Item48;

			AddMapEntry(new Color(124, 107, 92));

            VanillaFallbackOnModDeletion = TileID.Stone;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Lighting.GetColor(i, j) == Color.Black)
            {
                return;
            }

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x != 0 || y != 0)
                    {
                        if (CanMerge(i + x, j + y))
                        {
                            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Remnants/Content/Tiles/Blocks/SpiderEggMerge").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle((-x + 1) * 16, (-y + 1) * 16, 16, 16), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }

        private bool CanMerge(int i, int j)
        {
            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<SpiderEggs>())
            {
                return true;
            }
            return false;
        }

        public override bool HasWalkDust() => true;

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.Shadewood;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.Item51, new Vector2(i + 0.5f, j + 0.5f) * 16);
            }
            return true;// false;
        }
	}
}
