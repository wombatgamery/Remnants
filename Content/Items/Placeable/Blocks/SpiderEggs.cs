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
	public class SpiderEggs : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
        }

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = ModContent.TileType <Tiles.Blocks.SpiderEggs>();
		}

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(ItemID.Cobweb, 21);
            recipe.AddIngredient(Type);
            recipe.Register();

            recipe = Recipe.Create(ItemID.Silk, 3);
            recipe.AddIngredient(Type);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }
}