using NVorbis.Contracts;
using Remnants.Content.Tiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Remnants.Content.Items.Placeable.Blocks
{
	public class InsectRemains : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType <Tiles.SpiderNest.InsectRemains>();
		}

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            resultStack = 1;

            if (Main.rand.NextBool(1000))
            {
                resultType = ItemID.SpiderEgg;
            }
            else if (Main.rand.NextBool(2))
            {
                resultType = ItemID.Cobweb;
                resultStack = Main.rand.Next(3) + 1;
            }
            else
            {
                if (Main.rand.NextBool(5))
                {
                    resultType = Main.rand.NextBool(3) ? ItemID.RedHusk : Main.rand.NextBool(2) ? ItemID.CyanHusk : ItemID.VioletHusk;
                }
                else
                {
                    resultStack = Main.rand.Next(2) + 1;

                    if (Main.rand.NextBool(2))
                    {
                        resultType = Main.rand.NextBool(2) ? ItemID.AntlionMandible : ItemID.Stinger;
                    }
                    else
                    {
                        resultType = ItemID.Firefly;
                    }
                }
            }
        }
	}
}