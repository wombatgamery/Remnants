using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
	public class SpiritLanceDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.scale *= 0.95f;
            dust.velocity *= 0.95f;

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            dust.frame.Y = Main.rand.Next(3) * 6;
            Lighting.AddLight(dust.position, 58f / 255f * dust.scale, 156f / 255f * dust.scale, 255f / 255f * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }

    public class SoulLanceDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            dust.scale *= 0.95f;
            dust.velocity *= 0.95f;

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            dust.frame.Y = Main.rand.Next(3) * 6;
            Lighting.AddLight(dust.position, 124f / 255f * dust.scale, 124f / 255f * dust.scale, 143f / 255f * dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}