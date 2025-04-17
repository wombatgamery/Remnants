using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts.Environment
{
    public class granitespark : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.velocity += Main.rand.NextVector2Circular(1f, 1f) * 0.5f;

            dust.velocity.Y -= Main.rand.NextFloat(0.05f);
            dust.velocity *= 0.98f;

            dust.scale *= 0.995f;
            if (dust.scale <= 0.01f)
            {
                dust.active = false;
            }

            Lighting.AddLight(dust.position, 13f / 255f * dust.scale, 114f / 255f * dust.scale, 182f / 255f * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}