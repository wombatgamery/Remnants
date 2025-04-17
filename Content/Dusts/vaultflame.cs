using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
	public class vaultflame : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return Color.White;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.15f;
			if (!dust.noGravity)
			{
				dust.velocity.Y += 0.08f;
			}
			else dust.velocity *= 0.98f;
			dust.scale *= 0.98f;
            Lighting.AddLight(dust.position, 96f / 255f * dust.scale, 107f / 255f * dust.scale, 191f / 255f * dust.scale);
			if (dust.scale < 0.1f)
			{
				dust.active = false;
			}
			return false;
		}
	}
}