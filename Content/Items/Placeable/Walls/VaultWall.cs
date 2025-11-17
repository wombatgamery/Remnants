using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Remnants.Content.Items.Placeable.Blocks;

namespace Remnants.Content.Items.Placeable.Walls
{
	public class VaultWall : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
			Item.createWall = ModContent.WallType <Content.Walls.VaultWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
		}
	}

    public class VaultBeamWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.VaultBeamWall>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type, 4);
            recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<VaultWall>());
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<VaultWall>());
            recipe.AddIngredient(Type);
            recipe.Register();
        }
    }

    public class VaultHazardWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.VaultHazardWall>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<VaultWall>(), 2);
            recipe.AddIngredient(ItemID.YellowPaint);
            recipe.AddIngredient(ItemID.BlackPaint);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<VaultWall>());
            recipe.AddIngredient(Type);
            recipe.Register();
        }
    }

    public class VaultRailing : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.VaultRailing>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 8);
            recipe.AddIngredient(ModContent.ItemType<VaultPlating>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<VaultPlating>());
            recipe.AddIngredient(Type, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}