using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Items.Accessories;
using Remnants.Items.Placeable.Plants;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Items.Materials
{
	public class DreamJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<DreampodSeeds>();
		}

		public override void SetDefaults()
		{
			Item.width = 10 * 2;
			Item.height = 8 * 2;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(copper: 4);
			Item.rare = ItemRarityID.Green;
		}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);
            Vector2 origin = itemFrame.Size() / 2f;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPosition, itemFrame, Color.White, rotation, origin, scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AddRecipes()
		{
            CreateDuplicationRecipe(ItemID.Daybloom);
            CreateDuplicationRecipe(ItemID.Moonglow);
            CreateDuplicationRecipe(ItemID.Blinkroot);
            CreateDuplicationRecipe(ItemID.Deathweed);
            CreateDuplicationRecipe(ItemID.Waterleaf);
            CreateDuplicationRecipe(ItemID.Fireblossom);
            CreateDuplicationRecipe(ItemID.Shiverthorn);
        }

		private void CreateDuplicationRecipe(int type)
		{
            Recipe recipe;

            recipe = Recipe.Create(type, 2);
            recipe.AddIngredient(type);
            recipe.AddIngredient(Type, 10);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
	}
}