using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Tiles;
using Remnants.Items.Placeable.Walls;
using Remnants.Items.Materials;

namespace Remnants.Items.Placeable.Blocks
{
	[LegacyName("vaultbrick")]
	public class VaultPlating : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = ModContent.TileType <Tiles.Blocks.VaultPlating>();
		}

		public override void AddRecipes()
		{
			Recipe recipe;

			if (ModLoader.TryGetMod("WombatQOL", out Mod wombatqol) && wombatqol.TryFind("concrete", out ModItem concrete))
            {
				recipe = Recipe.Create(Type, 50);
				recipe.AddIngredient(concrete.Type, 50);
                recipe.AddIngredient(ItemID.AdamantiteBar);
                recipe.AddIngredient(ItemID.TitaniumBar);
                recipe.AddTile(TileID.AdamantiteForge);
                recipe.Register();
			}

            recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<VaultWall>(), 4);
			recipe.Register();
		}
	}
}