using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Biomes;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using Remnants.Items.Tools;
using Remnants.Buffs;
using Terraria.Audio;
using System;
using Remnants.Worldgen;
using Remnants.Walls;
using Terraria.DataStructures;

namespace Remnants
{
    public class RemPlayer : ModPlayer
    {
        public float critDamageMult;
        public bool magicShield;
        public float manaRegenStopTimer;

        public bool moving => !(Player.velocity.X < 0.05 && Player.velocity.Y < 0.05 && Player.velocity.X > -0.05 && Player.velocity.Y > -0.05);

        Vector2 lerpPosition;

        public bool ZoneVault;
        public bool ZoneJungleTemple;
        public bool ZoneGrowth;
        public bool ZoneFlesh;
        public bool ZoneMineshaft;
        public bool ZoneSkyJungle;

        public Vector2 savedPosition;

        public override void ResetEffects()
        {
            critDamageMult = 1;
            magicShield = false;
        }

        public int activeEssence;

        public override void PostUpdateEquips()
        {
            if (activeEssence > 0)
            {
                if (activeEssence == 1 || activeEssence == 2)
                {
                    Player.statLifeMax2 = (int)(Player.statLifeMax2 * 1.2f);
                }
                else Player.statLifeMax2 = (int)(Player.statLifeMax2 * 0.8f);

                if (activeEssence == 2 || activeEssence == 3)
                {
                    Player.moveSpeed *= 1.2f;
                    //Player.runAcceleration *= 1.2f;
                    Player.accRunSpeed *= 1.2f;
                    //Player.jumpSpeed *= 1.2f;
                    //Player.maxFallSpeed *= 1.25f;
                }
                else
                {
                    Player.moveSpeed *= 0.8f;
                    //Player.runAcceleration *= 0.8f;
                    Player.accRunSpeed *= 0.8f;
                    //Player.jumpSpeed *= 0.8f;
                    //Player.maxFallSpeed *= 0.75f;
                }


                if (activeEssence == 1 || activeEssence == 3)
                {
                    Player.GetDamage(DamageClass.Generic) *= 1.1f;
                }
                else Player.GetDamage(DamageClass.Generic) *= 0.9f;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (Player.miscEquips[4].type == ModContent.ItemType<LuminousHook>() && Player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Hooks.LuminousHook>()] == 0)
            {
                Lighting.AddLight(Player.Center, (117f / 255f), (77f / 255f), (31f / 255f));
            }

            if (ModContent.GetInstance<Server>().FreedomOfMovement && !Player.mount.Active)
            {
                //Player.runAcceleration *= 1.4f;
                //Player.maxRunSpeed *= 1.2f;

                Player.maxRunSpeed += 0.5f;
                Player.runAcceleration += 0.2f;
                Player.runSlowdown += 0.1f;
            }

            if (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight)
            {
                Player.ZoneGlowshroom = RemWorld.mushroomTiles >= 1000;
            }
            if (Player.InModBiome<Biomes.MagicalLab>())
            {
                Player.shimmerMonolithShader = true;
            }

            if (!Player.shimmering && Main.tile[(int)(Player.Center.X / 16), (int)((Player.position.Y + Player.height) / 16)].WallType == ModContent.WallType<Ascension>())
            {
                Player.gravity = 0;
                Player.velocity.Y *= 0.95f;
                Player.velocity.Y -= 0.2f;
            }

            if (manaRegenStopTimer > 0)
            {
                Player.manaRegenCount = 0;
                manaRegenStopTimer--;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Main.tile[(int)(Player.Center.X / 16), (int)((Player.position.Y + Player.height) / 16)].WallType == ModContent.WallType<Ascension>())
            {
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.TreasureSparkle);
                dust.velocity = new Vector2(0, -5);
            }
        }

        public static float shakeIntensity = 0;
        public static void ScreenShake(Vector2 position, float intensity)
        {
            shakeIntensity += intensity * 5 * (1 - MathHelper.Clamp(Vector2.Distance(position, Main.LocalPlayer.Center) / (64 * 16), 0, 1));
        }

        public override void ModifyScreenPosition()
        {
            if (shakeIntensity > 0)
            {
                Main.screenPosition += new Vector2(Main.rand.NextFloat(shakeIntensity), Main.rand.NextFloat(shakeIntensity));
                shakeIntensity *= 0.85f;
                if (shakeIntensity < 0.1f)
                {
                    shakeIntensity = 0;
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["activeEssence"] = activeEssence;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("activeEssence", out int essence))
            {
                activeEssence = essence;
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.InModBiome<Biomes.Vault>() && Player.wet && !Player.lavaWet)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 180;
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (magicShield && !Player.HasBuff(ModContent.BuffType<ManaShieldCooldown>()) && Player.statMana > 0)
            {
                modifiers.FinalDamage *= 0.75f;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (magicShield && !Player.HasBuff(ModContent.BuffType<ManaShieldCooldown>()) && Player.statMana > 0)
            {
                Player.CheckMana(Math.Min(info.Damage * 2, Player.statMana), true);

                if (Player.statMana <= 0)
                {
                    Player.AddBuff(ModContent.BuffType<ManaShieldCooldown>(), 60 * 30);
                    SoundEngine.PlaySound(new SoundStyle("Remnants/Sounds/omegac"), Player.Center);
                }
                Player.manaRegenDelay = (int)Player.maxRegenDelay;

                manaRegenStopTimer = 160;
            }
        }

        //      public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
        //      {
        //	Player player = Main.LocalPlayer;
        //	if (manaDamageAbsorption > 0 && !player.HasBuff(ModContent.BuffType<magicshieldcooldown>()))
        //	{
        //		int absorption = 0;
        //		int newDamage = modifiers.SourceDamage.;
        //		while (absorption < modifiers.FinalDamage.Flat * manaDamageAbsorption)
        //		{
        //			absorption++;
        //			if (absorption == player.statMana)
        //			{
        //				player.AddBuff(ModContent.BuffType<magicshieldcooldown>(), 30 * 60);
        //				SoundEngine.PlaySound(new SoundStyle("Remnants/omegac"), player.Center);
        //				break;
        //			}
        //		}
        //		player.statMana -= absorption;
        //		newDamage -= absorption;

        //		damage = Math.Max(1, newDamage);

        //		//Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<magicshieldhit>(), 0, 0, Main.myPlayer);
        //		for (int i = 0; i < 10; i++)
        //		{
        //			int dustIndex = Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<vaultflame>());
        //			Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(5f, 5f);
        //			Main.dust[dustIndex].noGravity = true;
        //		}

        //		//Main.PlaySound(SoundID.NPCHit, Style: mod.GetSoundSlot(SoundType.NPCHit, "Sounds/NPCHit/magicshieldhit"));
        //	}
        //	return true;
        //}

        //public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        //{
        //    if (crit)
        //    {
        //        damage /= 2;
        //        damage += target.defense / 2;
        //    }
        //}
        //public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hit.HitDirection)
        //{
        //    if (crit)
        //    {
        //        damage /= 2;
        //        damage += target.defense / 2;
        //    }
        //}

        private bool InFrontOfWall(int wallType)
        {
            return Main.tile[(int)Player.Center.X / 16, (int)Player.Center.Y / 16].WallType == wallType;
        }

        private bool InRectangle(Rectangle rectangle)
        {
            if (Player.Center.X > rectangle.Left * 16 && Player.Center.X < rectangle.Right * 16)
            {
                if (Player.Center.Y > rectangle.Top * 16 && Player.Center.Y < rectangle.Bottom * 16)
                {
                    return true;
                }
            }
            return false;
        }

        //     public override void PreUpdateBuffs()
        //     {
        //if (Player.InModBiome<EchoingMaze>())
        //         {
        //	Player.AddBuff(BuffID.Darkness, 2);
        //}
        //     }

        //public override void SendCustomBiomes(BinaryWriter writer)
        //{
        //	BitsByte flags = new BitsByte();
        //	flags[0] = ZoneGrowth;
        //	writer.Write(flags);
        //}

        //public override void ReceiveCustomBiomes(BinaryReader reader)
        //{
        //	BitsByte flags = reader.ReadByte();
        //	ZoneGrowth = flags[0];
        //}
    }

    //public class AerialGardenSceneEffect : ModSceneEffect
    //{
    //    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    //    public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Remnants/GardenWater");


    //    public override bool IsSceneEffectActive(Player player)
    //    {
    //        return RemWorld.gardenTiles >= 100;
    //    }
    //}
}