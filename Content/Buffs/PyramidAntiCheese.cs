using Terraria;
using Terraria.ModLoader;

namespace Remnants.Content.Buffs
{
	public class PyramidAntiCheese : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
    }
}