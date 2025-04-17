using Terraria;
using Terraria.ModLoader;

namespace Remnants.Content.Buffs
{
	public class RunestalkBlessing : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.pickSpeed /= 1.5f;
        }
    }
}