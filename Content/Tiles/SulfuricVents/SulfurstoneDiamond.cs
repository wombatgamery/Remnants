using Microsoft.Xna.Framework;
using Remnants.Content.Biomes;
using Remnants.Content.Items.Consumable;
using Remnants.Content.Walls;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.SulfuricVents
{
	public class SulfurstoneDiamond : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileShine[Type] = 450;
            //Main.tileShine2[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Sulfurstone>()] = true;
            Main.tileMerge[ModContent.TileType<Sulfurstone>()][Type] = true;

            DustType = DustID.Stone;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(187, 198, 233), Language.GetText("ItemName.Diamond"));

            VanillaFallbackOnModDeletion = TileID.Diamond;
        }

        public override void PostSetDefaults()
        {
            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                if (Main.tileMerge[ModContent.TileType<Sulfurstone>()][i])
                {
                    Main.tileMerge[Type][i] = true;
                }
                if (Main.tileMerge[i][ModContent.TileType<Sulfurstone>()])
                {
                    Main.tileMerge[i][Type] = true;
                }
            }
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ItemID.Diamond);
        }

        public override bool IsTileSpelunkable(int i, int j) => true;
    }
}