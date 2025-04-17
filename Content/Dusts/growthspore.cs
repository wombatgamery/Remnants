using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
	public class growthspore : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.velocity += Main.rand.NextVector2Circular(1f, 1f) * 0.05f;
            if (dust.velocity.X > 0.5)
            {
                dust.velocity.X = 0.5f;
            }
            if (dust.velocity.X < -0.5)
            {
                dust.velocity.X = -0.5f;
            }
            if (dust.velocity.Y > 0.5)
            {
                dust.velocity.Y = 0.5f;
            }
            if (dust.velocity.Y < -0.5)
            {
                dust.velocity.Y = -0.5f;
            }

            dust.rotation += dust.velocity.X * 0.15f;

            dust.scale *= 0.995f;
            if (dust.scale <= 0.01f)
            {
                dust.active = false;
            }

            Lighting.AddLight(dust.position, 57 / 255 * dust.scale, 95 / 255 * dust.scale, 143 / 255 * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}