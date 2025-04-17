using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ModLoader;
using Remnants.Content.Dusts;

namespace Remnants.Content.Projectiles.Effects
{
	public class MazeGuardianTeleport : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 9999;

			Projectile.width = 180;
			Projectile.height = 180;

			Projectile.friendly = false;
			Projectile.hostile = false;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

        public override void AI()
		{
			Projectile.velocity = Vector2.Zero;

			if (Projectile.alpha < 255)
			{
				for (int i = 0; i < (255 - Projectile.alpha) / 17; i++)
				{
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<spiritenergy>());
				}

				Projectile.alpha += 17;
			}
			else Projectile.Kill();
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}

        public override bool PreDraw(ref Color lightColor)
        {
			lightColor = Color.White;
			return base.PreDraw(ref lightColor);
        }
    }
}