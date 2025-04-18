using CalamityMod;
using CalamityMod.NPCs.CalClone;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Ores;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RemnantsTemp.World
{
    public class MeteorSafeguard : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int tile, ref bool blockDamaged)
        {
            if (tile == TileID.Meteorite)
            {
                if (NPC.downedBoss2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == TileID.ShimmerBlock)
            {
                if (NPC.downedBoss2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == TileID.Hellstone)
            {
                if (NPC.downedBoss3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == TileID.LunarOre)
            {
                if (NPC.downedMoonlord)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return base.CanKillTile(i, j, tile, ref blockDamaged);
        }
        public override bool CanReplace(int i, int j, int tile, int tileTypeBeingPlaced)
        {
            if (tile == TileID.Meteorite)
            {
                if (NPC.downedBoss2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == TileID.ShimmerBlock)
            {
                if (NPC.downedBoss2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == TileID.Hellstone)
            {
                if (NPC.downedBoss3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == TileID.LunarOre)
            {
                if (NPC.downedMoonlord)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return base.CanReplace(i, j, tile, tileTypeBeingPlaced);
        }
        public override bool KillSound(int i, int j, int tile, bool fail)
        {
            if (tile == TileID.ShimmerBlock)
            {
                SoundEngine.PlaySound(SoundID.Shatter);
            }
            return base.KillSound(i, j, tile, fail);
        }
    }
}
