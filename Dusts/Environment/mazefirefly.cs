using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Dusts.Environment
{
    public class mazefirefly : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = 1;
            dust.noGravity = true;
            dust.velocity = Main.rand.NextVector2Circular(1f, 1f) * 1f;
        }

        public override bool Update(Dust dust)
        {
            if (Main.rand.NextBool(200))
            {
                dust.active = false;
            }
            else
            {
                dust.position += dust.velocity;

                dust.velocity *= 0.98f;
                dust.velocity += Main.rand.NextVector2Circular(1f, 1f) * 0.1f;

                Lighting.AddLight(dust.position, 65f / 255f * dust.scale * 0.1f, 103f / 255f * dust.scale * 0.1f, 155f / 255f * dust.scale * 0.1f);
            }

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}