using System.Linq;
using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Remnants.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Accessories
{
    [LegacyName("WindCloak")]
    [AutoloadEquip(EquipType.Head)]
    public class SummonCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;

            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 13 * 2;
            Item.height = 9 * 2;
            Item.defense = 1;
            //Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) *= 1 + (float)(player.statLifeMax2 - player.statLife) / player.statLifeMax2 * 0.25f;
        }
    }

    //public class WindCloakPlayer : ModPlayer
    //{
    //    public bool equipped;
    //    public int direction;
    //    public float velocity = 6;

    //    const int dustType = DustID.Cloud;//DustID.DungeonSpirit
    //    public override void ResetEffects()
    //    {
    //        equipped = false;

    //        if (timer <= 0)
    //        {
    //            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[right] < 15)
    //            {
    //                direction = right;
    //            }
    //            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[left] < 15)
    //            {
    //                direction = left;
    //            }
    //            else direction = -1;
    //        }
    //    }

    //    public int delay = 0;
    //    public int timer = 0;

    //    const int left = 3;
    //    const int right = 2;

    //    public override void PreUpdateMovement()
    //    {
    //        // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
    //        if (CanDash() && direction != -1 && delay == 0)
    //        {
    //            Player.noFallDmg = true;

    //            delay = 60;
    //            timer = 15;

    //            for (int k = 0; k < 30; k++)
    //            {
    //                Dust dust = Dust.NewDustPerfect(Player.Center, dustType, Main.rand.NextVector2CircularEdge(2, 4), Scale: Main.rand.NextFloat(1, 2));
    //                dust.noGravity = true;
    //            }
    //        }

    //        if (delay > 0)
    //            delay--;

    //        if (timer > 0)
    //        { // dash is active
    //          // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
    //          // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.

    //            //Player.immune = true;

    //            Player.velocity.Y = 0;

    //            float dashDirection = (direction == right) ? 1 : -1;
    //            if (dashDirection == 1)
    //            {
    //                if (Player.velocity.X < velocity)
    //                {
    //                    Player.velocity.X = velocity;
    //                }
    //            }
    //            else if (dashDirection == -1)
    //            {
    //                if (Player.velocity.X > -velocity)
    //                {
    //                    Player.velocity.X = -velocity;
    //                }
    //            }

    //            Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, dustType);
    //            dust.velocity = Player.velocity / 2 + Main.rand.NextVector2Circular(1, 1);
    //            dust.noGravity = true;

    //            // count down frames remaining
    //            timer--;
    //        }
    //    }

    //    private bool CanDash()
    //    {
    //        return equipped
    //            && Player.dashType == 0 // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
    //            && !Player.setSolar // player isn't wearing solar armor
    //            && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
    //    }
    //}
}