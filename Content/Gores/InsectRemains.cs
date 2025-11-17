using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Gores
{
	public class InsectRemains1 : ModGore
	{
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Gore gore)
        {
            if (gore.velocity.Y == 0)
            {
                gore.rotation += gore.velocity.X / 4;
            }

            return true;
        }
    }

    public class InsectRemains2 : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Gore gore)
        {
            if (gore.velocity.Y == 0)
            {
                gore.rotation += gore.velocity.X / 3;
            }

            return true;
        }
    }

    public class InsectRemains3 : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Gore gore)
        {
            if (gore.velocity.Y == 0)
            {
                gore.rotation += gore.velocity.X / 2;
            }

            return true;
        }
    }
}