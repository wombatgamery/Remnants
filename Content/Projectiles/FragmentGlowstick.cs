using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
	public class FragmentGlowstick : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Glowstick);

			AIType = ProjectileID.Glowstick;

			DrawOriginOffsetX = 8;
            DrawOriginOffsetY = 8;
        }
    }
}