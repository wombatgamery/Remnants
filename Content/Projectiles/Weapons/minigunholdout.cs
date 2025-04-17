using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles.Weapons
{
    public class minigunholdout : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 15;
			//projectile.aiStyle = 75;
			Projectile.alpha = 0;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

			UpdateAnimation();
			UpdatePlayerVisuals(player, rrp);
		}

		private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
		{
			Projectile.Center = playerHandPos;
			// The beams emit from the tip of the Prism, not the side. As such, rotate the sprite by pi/2 (90 degrees).
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.spriteDirection = Projectile.direction;

			// The Prism is a holdout projectile, so change the player's variables to reflect that.
			// Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;

			// If you do not multiply by projectile.direction, the player's hand will point the wrong direction while facing left.
			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
		}

		private void UpdateAnimation()
		{
			Projectile.frameCounter++;

			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (++Projectile.frame >= 2)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}