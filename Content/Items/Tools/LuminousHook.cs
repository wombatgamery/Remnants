using Remnants.Content.Buffs;
using Remnants.Content.Projectiles;
using Remnants.Content.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Tools
{
	public class LuminousHook : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() 
		{
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shoot = ModContent.ProjectileType<Projectiles.Hooks.LuminousHook>();
		}
    }
}