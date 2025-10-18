using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Dusts
{
	public class Spiderling : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;

            dust.frame.Width = 10;
            dust.frame.Height = 10;
            dust.frame.Y = Main.rand.Next(3) * 12;
        }

        public override bool Update(Dust dust)
        {
            if (Collision.SolidCollision(dust.position, 5, 5))
            {
                dust.active = false;
            }
            else
            {
                dust.velocity += Main.rand.NextVector2CircularEdge(1, 1);
                dust.velocity *= 0.95f;

                dust.position += dust.velocity;
                dust.rotation = dust.velocity.ToRotation();

                dust.frame.Y += 12;
                if (dust.frame.Y > 24)
                {
                    dust.frame.Y = 0;
                }
            }

            return false;
        }
    }
}