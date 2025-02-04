using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Dusts
{
	public class growthblood : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
            UpdateType = DustID.Blood;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }

        public override bool Update(Dust dust)
		{
            Terraria.Lighting.AddLight(dust.position, 57 / 255 * dust.scale, 137 / 255 * dust.scale, 143 / 255 * dust.scale);
            return false;
		}
	}
}