using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Remnants.Tiles;
using Remnants.Walls;
using System.IO;
using Remnants.Worldgen;
using Microsoft.Xna.Framework;

namespace Remnants.Buffs
{
    public class ManaShieldCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.manaRegenDelay < 1)
            {
                player.manaRegenDelay = 1;
            }
        }
    }
}