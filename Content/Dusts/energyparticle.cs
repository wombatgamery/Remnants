using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
	public class energyparticle : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;

            dust.scale *= 0.95f;
            dust.velocity *= 0.95f;
            dust.velocity += Main.rand.NextVector2Circular(0.2f, 0.2f);

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            Lighting.AddLight(dust.position, 85f / 255f * dust.scale, 111f / 255f * dust.scale, 171f / 255f * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}