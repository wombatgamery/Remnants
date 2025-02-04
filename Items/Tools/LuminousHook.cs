using Remnants.Buffs;
using Remnants.Projectiles;
using Remnants.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Items.Tools
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