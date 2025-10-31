using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.Gores;
using Remnants.Content.Items.Consumable;
using Remnants.Content.Items.Placeable.Blocks;
using Remnants.Content.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Objects.Hazards
{
    public class SpiderCocoon : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
            Main.tileCut[Type] = true;

			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = DustID.Web;
            HitSound = SoundID.NPCHit18;

            AddMapEntry(new Color(160, 160, 178));

            VanillaFallbackOnModDeletion = TileID.Cobweb;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<Items.Placeable.Blocks.InsectRemains>(), Main.rand.Next(1, 4));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            fail = false;

            if (!fail)
            {
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDustPerfect(new Vector2(i + 0.5f, j + 0.5f) * 16, ModContent.DustType<Spiderling>());
                }

                for (int k = 0; k < 15; k++)
                {
                    int type = Main.rand.NextBool(3) ? ModContent.GoreType<InsectRemains1>() : Main.rand.NextBool(2) ? ModContent.GoreType<InsectRemains2>() : ModContent.GoreType<InsectRemains3>();
                    Gore.NewGorePerfect(new EntitySource_TileBreak(i, j), new Vector2(i + 0.5f, j + 0.5f) * 16, Main.rand.NextVector2Circular(4, 4), type);
                }
            }
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Lighting.GetColor(i, j) == Color.Black)
            {
                return false;
            }

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

            int frame = (i + j) % 2;
            float rotation = !Main.rand.NextBool(5) ? 0 : Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) / 8;
            SpriteEffects spriteEffects = (i + j) % 4 < 2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X + 8, j * 16 - (int)Main.screenPosition.Y + 8) + zero, new Rectangle(frame * 32, 0, 32, 32), Lighting.GetColor(i, j), rotation, new Vector2(16, 16), 1f, spriteEffects, 0f);

            return false;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath35, new Vector2(i + 0.5f, j + 0.5f) * 16);
            }
            return true;// false;
        }
    }
}