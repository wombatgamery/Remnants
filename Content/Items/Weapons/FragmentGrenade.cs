using Microsoft.Xna.Framework;
using Remnants.Content.Projectiles.Weapons;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Weapons
{
	public class FragmentGrenade : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
		{
			Item.CloneDefaults(ItemID.Grenade);
			Item.width = 16;
			Item.height = 22;
			Item.damage = 40;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.FragmentGrenade>();
		}

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}
