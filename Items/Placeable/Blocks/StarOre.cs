using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles;

namespace Remnants.Items.Placeable.Blocks
{
	[LegacyName("starore")]
	public class StarOre : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.width = 8;
			Item.height = 8;
			Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<Tiles.Blocks.StarOre>();
		}

        public override void AddRecipes()
        {
			Recipe recipe;

			recipe = Recipe.Create(ItemID.FallenStar);
			recipe.AddIngredient(Type, 6);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
    }
}