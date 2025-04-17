using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
    public class pulseburst : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;

            dust.frame = new Rectangle(0, 0, 32, 32);
            dust.position -= Vector2.One * 16;
            dust.velocity = Vector2.Zero;

            dust.frame.X -= 34;
        }

        public override bool Update(Dust dust)
        {
            dust.frame.X += 34;
            if (dust.frame.X > 3 * 34)
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