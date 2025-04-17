using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
	public class spiritenergy : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
            dust.velocity = Vector2.Zero;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 12, 10, 10);
            dust.rotation = 0;
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.velocity += Main.rand.NextVector2Circular(1f, 1f) * 0.25f;
            dust.velocity *= 0.98f;

            dust.scale *= 0.97f;
            if (dust.scale <= 0.1f)
            {
                dust.active = false;
            }

            Lighting.AddLight(dust.position, 65f / 255f * dust.scale, 103f / 255f * dust.scale, 155f / 255f * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}