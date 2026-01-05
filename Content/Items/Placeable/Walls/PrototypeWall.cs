using Remnants.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Placeable.Walls
{
	public class PrototypeWall : ModItem
	{
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneWall);
			Item.createWall = ModContent.WallType <Content.Walls.Underworld.PrototypeWall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ModContent.ItemType<PrototypePlating>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
		}
	}

    public class PrototypeBeamWall : ModItem
    {
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.Underworld.PrototypeBeamWall>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type, 4);
            recipe.AddIngredient(ModContent.ItemType<PrototypePlating>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<PrototypeWall>());
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<PrototypeWall>());
            recipe.AddIngredient(Type);
            recipe.Register();
        }
    }

    public class PrototypeHazardWall : ModItem
    {
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.Underworld.PrototypeHazardWall>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<PrototypeWall>(), 2);
            recipe.AddIngredient(ItemID.OrangePaint);
            recipe.AddIngredient(ItemID.BlackPaint);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<PrototypeWall>());
            recipe.AddIngredient(Type);
            recipe.Register();
        }
    }

    public class PrototypeRailing : ModItem
    {
        public override LocalizedText Tooltip => null;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StoneWall);
            Item.createWall = ModContent.WallType<Content.Walls.Underworld.PrototypeRailing>();
        }

        public override void AddRecipes()
        {
            Recipe recipe;

            recipe = Recipe.Create(Type, 8);
            recipe.AddIngredient(ModContent.ItemType<PrototypePlating>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<PrototypePlating>());
            recipe.AddIngredient(Type, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}