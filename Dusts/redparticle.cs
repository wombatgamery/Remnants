using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Dusts
{
	public class redparticle : ModDust
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
            dust.velocity += Main.rand.NextVector2Circular(0.4f, 0.4f);

            Terraria.Lighting.AddLight(dust.position, (143f / 255f) * dust.scale , (57f / 255f) * dust.scale, (77f / 255f) * dust.scale);

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}