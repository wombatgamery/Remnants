using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
    public class pulsering : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;

            dust.frame = new Rectangle(0, 0, 16, 16);
            dust.velocity = Vector2.Zero;
            dust.rotation = 0;

            dust.frame.X -= 18;
        }

        public override bool Update(Dust dust)
        {
            dust.frame.X += 18;
            if (dust.frame.X > 3 * 18)
            {
                dust.active = false;
            }

            //dust.frame.X = frameNumber * 18;
            //if (++frameCounter >= 5)
            //{
            //    frameCounter = 0;
            //    if (++frameNumber >= 3)
            //    {
            //        dust.active = false;
            //    }
            //}
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}