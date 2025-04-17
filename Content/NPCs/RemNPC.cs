using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Remnants.Content.World;
using Remnants.Content.Items.Consumable;
using Remnants.Content.Items.Materials;
using Remnants.Content.Biomes;
using Remnants.Content.NPCs.Monsters;
using Remnants.Content.NPCs.Monsters.MagicalLab;
using Remnants.Content.NPCs.Monsters.TheVault;
using Remnants.Content.NPCs.Monsters.Undergrowth;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Remnants.Content.Tiles.Blocks;

namespace Remnants.Content.NPCs
{
    public class RemNPC : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            if (entity.type == NPCID.DemonEye || entity.type == NPCID.DemonEye2 || entity.type == NPCID.CataractEye || entity.type == NPCID.CataractEye2 || entity.type == NPCID.SleepyEye || entity.type == NPCID.SleepyEye2 || entity.type == NPCID.DialatedEye || entity.type == NPCID.DialatedEye2 || entity.type == NPCID.GreenEye || entity.type == NPCID.GreenEye2 || entity.type == NPCID.PurpleEye || entity.type == NPCID.PurpleEye2)
            {
                entity.lifeMax = 60;
                entity.damage = 18;
                entity.defense = 2;
                entity.knockBackResist = 0.9f;

                if (entity.type == NPCID.CataractEye || entity.type == NPCID.CataractEye2)
                {
                    //npc.GivenName = "Cataract Eye";
                    entity.damage -= 2;
                    entity.defense += 2;
                    entity.knockBackResist -= 0.1f;
                }
                else if (entity.type == NPCID.SleepyEye || entity.type == NPCID.SleepyEye2)
                {
                    //npc.GivenName = "Sleepy Eye";
                    entity.defense += 4;
                }
                else if (entity.type == NPCID.DialatedEye || entity.type == NPCID.DialatedEye2)
                {
                    //npc.GivenName = "Dilated Eye";
                    entity.damage += 2;
                    entity.defense -= 2;
                    entity.knockBackResist += 0.1f;
                }
                else if (entity.type == NPCID.GreenEye || entity.type == NPCID.GreenEye2)
                {
                    //npc.GivenName = "Green Eye";
                    entity.damage += 2;
                    entity.lifeMax -= 5;
                }
                else if (entity.type == NPCID.PurpleEye || entity.type == NPCID.PurpleEye2)
                {
                    //npc.GivenName = "Purple Eye";
                    entity.damage -= 2;
                    entity.lifeMax += 5;
                }

                if (entity.type == NPCID.DemonEye2 || entity.type == NPCID.CataractEye2 || entity.type == NPCID.SleepyEye2 || entity.type == NPCID.PurpleEye2)
                {
                    //npc.GivenName = "Large" + npc.GivenName;
                    entity.damage += 2;
                    entity.lifeMax += 10;
                    entity.knockBackResist -= 0.1f;
                }
                else if (entity.type == NPCID.DialatedEye2 || entity.type == NPCID.GreenEye2)
                {
                    //npc.GivenName = "Small" + npc.GivenName;
                    entity.damage -= 2;
                    entity.lifeMax -= 10;
                    entity.knockBackResist += 0.1f;
                }
            }
        }

        public override void PostAI(NPC npc)
        {
            if (npc.type != ModContent.NPCType<Arcanist>())
            {
                if (!npc.noTileCollide && Main.tile[(int)(npc.Center.X / 16), (int)((npc.position.Y + npc.height) / 16)].WallType == ModContent.WallType<Ascension>())
                {
                    npc.GravityMultiplier *= 0;
                    npc.velocity.Y *= 0.95f;
                    npc.velocity.Y -= 0.2f;
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (!npc.noTileCollide && Main.tile[(int)(npc.Center.X / 16), (int)((npc.position.Y + npc.height) / 16)].WallType == ModContent.WallType<Ascension>())
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TreasureSparkle);
                dust.velocity = new Vector2(0, -5);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (ModContent.GetInstance<Gameplay>().HangingBats && Main.netMode == NetmodeID.SinglePlayer)
            {
                if (npc.type == NPCID.CaveBat || npc.type == NPCID.JungleBat || npc.type == NPCID.Hellbat || npc.type == NPCID.GiantBat || npc.type == NPCID.IlluminantBat || npc.type == NPCID.IceBat || npc.type == NPCID.Lavabat || npc.type == NPCID.GiantFlyingFox || npc.type == NPCID.SporeBat)
                {
                    if (source is EntitySource_SpawnNPC && npc.ai[1] != 1 && !npc.SpawnedFromStatue)
                    {
                        npc.active = false;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            //if (npc.type == NPCID.FaceMonster || npc.type == NPCID.EaterofSouls || npc.type == NPCID.Herpling || npc.type == NPCID.Corruptor || npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.RedDevil || npc.type == NPCID.Unicorn)
            //{
            //	npcLoot.Add(ItemDropRule.Common(ItemID.Leather, 2));
            //}

            #region metalscraps
            if (npc.type == NPCID.Probe || npc.type == NPCID.DeadlySphere || npc.type == NPCID.MartianTurret)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SalvagedMetal>(), 2, 2, 4));
            }
            if (npc.type == NPCID.PrimeLaser || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeSaw || npc.type == NPCID.MartianWalker)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SalvagedMetal>(), 1, 4, 8));
            }
            if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism || npc.type == NPCID.TheDestroyer || npc.type == NPCID.MartianSaucer)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SalvagedMetal>(), 1, 8, 16));
            }
            #endregion

            npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.Binoculars);

            //if (npc.type == NPCID.Hornet)
            //{
            //	npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsPreHardmode(), ModContent.ItemType<hivekey>(), 50));
            //}
            if (npc.type == NPCID.AngryBones || npc.type >= 294 && npc.type <= 296 || npc.type == NPCID.DarkCaster || npc.type == NPCID.CursedSkull)
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.Valor, 200));

                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IronKey>(), 100));
            }
            else if (npc.type == NPCID.GiantTortoise)
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.TurtleShell, 1));
            }

            npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.AncientCobaltHelmet);
            npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.AncientCobaltBreastplate);
            npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.AncientCobaltLeggings);

            //if (npc.type == NPCID.GraniteGolem)
            //{
            //    npcLoot.Add(ItemDropRule.OneFromOptions(21, new int[] { ItemID.AncientCobaltHelmet, ItemID.AncientCobaltBreastplate, ItemID.AncientCobaltLeggings }));
            //}
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];

            if (!Main.wallHouse[Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].WallType])
            {
                if (tile.WallType == ModContent.WallType<whisperingmaze>() || tile.WallType == ModContent.WallType<LabyrinthTileWall>() || tile.WallType == ModContent.WallType<LabyrinthBrickWall>())
                {
                    pool.Clear();
                }
                else if (tile.TileType != TileID.LihzahrdBrick && (tile.WallType == WallID.LihzahrdBrickUnsafe || tile.WallType == ModContent.WallType<temple>()))
                {
                    pool.Clear();
                    pool[NPCID.Lihzahrd] = 2f;
                    pool[NPCID.FlyingSnake] = 1f;
                }
                else if (tile.WallType == ModContent.WallType<vault>() || tile.WallType == ModContent.WallType<VaultWallUnsafe>())
                {
                    pool.Clear();

                    pool[ModContent.NPCType<Shocker>()] = 1f;
                    pool[ModContent.NPCType<Gunner>()] = 1f;
                    pool[ModContent.NPCType<Blaster>()] = 1f;
                    pool[ModContent.NPCType<Flamer>()] = 1f;

                    pool[ModContent.NPCType<Icer>()] = 0.2f;
                }
                //else if (tile.WallType == WallID.LihzahrdBrickUnsafe || tile.WallType == ModContent.WallType<temple>())
                //{
                //	pool.Clear();

                //	if (player.InModBiome(ModContent.GetInstance<JungleTemple>()))
                //	{
                //		pool[NPCID.Lihzahrd] = 1f;
                //		pool[NPCID.FlyingSnake] = 0.5f;
                //	}
                //}
                else if (tile.WallType == ModContent.WallType<magicallab>() || tile.WallType == ModContent.WallType<EnchantedBrickWallUnsafe>() || tile.WallType == ModContent.WallType<Ascension>())
                {
                    pool.Clear();

                    if (tile.WallType != ModContent.WallType<Ascension>())
                    {
                        pool[ModContent.NPCType<Arcanist>()] = 1f;
                        pool[ModContent.NPCType<TomeofInferno>()] = 1f;
                        pool[ModContent.NPCType<TomeofFrost>()] = 1f;
                    }
                }
                else if (tile.WallType == ModContent.WallType<undergrowth>() || tile.WallType == WallID.LivingWoodUnsafe)
                {
                    pool.Clear();

                    pool[ModContent.NPCType<ResinSlime>()] = 3f;

                    if (spawnInfo.Player.InModBiome<Biomes.Undergrowth>())
                    {
                        pool[ModContent.NPCType<CentipedeHead>()] = 1f;
                    }
                }
                else if (tile.TileType == ModContent.TileType<PyramidBrick>() || tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<PyramidBrickWallUnsafe>())
                {
                    pool.Clear();

                    if (spawnInfo.Player.InModBiome<Biomes.Pyramid>())
                    {
                        if (tile.WallType == ModContent.WallType<pyramid>() || tile.WallType == ModContent.WallType<PyramidBrickWallUnsafe>())
                        {
                            pool[NPCID.Ghost] = 2f;
                        }
                        //if (player.InModBiome(ModContent.GetInstance<Pyramid>()))
                        //{
                        //	pool[NPCID.Ghost] = 2f;

                        //	pool[NPCID.Scorpion] = 1f;
                        //	pool[NPCID.ScorpionBlack] = 1f;
                        //}
                    }

                    pool[NPCID.SandSlime] = 2f;

                    pool[NPCID.Scorpion] = 1f;
                    pool[NPCID.ScorpionBlack] = 1f;
                }
                else if (tile.WallType == ModContent.WallType<forgottentomb>() || tile.WallType == ModContent.WallType<TombBrickWallUnsafe>())
                {
                    pool.Clear();

                    pool[Main.hardMode ? NPCID.Wraith : NPCID.Ghost] = 1f;
                    pool[Main.hardMode ? NPCID.BlackRecluseWall : NPCID.WallCreeperWall] = 3f;

                    if (!NPC.savedStylist && !NPC.AnyNPCs(NPCID.WebbedStylist))
                    {
                        pool[NPCID.WebbedStylist] = 1f;
                    }
                }
                else if (tile.WallType == ModContent.WallType<stronghold>() || tile.WallType == ModContent.WallType<HellishBrickWallUnsafe>())
                {
                    pool[NPCID.BlazingWheel] = 0.5f;
                }
                else if ((tile.TileType == TileID.Granite || tile.TileType == TileID.GraniteBlock) && spawnInfo.SpawnTileY > Main.worldSurface)
                {
                    pool.Clear();

                    pool[NPCID.GraniteGolem] = 2f;
                    pool[NPCID.GraniteFlyer] = 1f;
                }
                else if (spawnInfo.Player.InModBiome(ModContent.GetInstance<TheHive>()) || tile.TileType == TileID.Hive || tile.WallType == ModContent.WallType<hive>() || tile.WallType == WallID.HiveUnsafe)
                {
                    pool.Clear();

                    if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].WallType == 0)
                    {
                        pool[ModContent.NPCType<HoneySlime>()] = 3f;
                    }
                    else
                    {
                        pool[NPCID.LittleHornetHoney] = 1f;
                        pool[NPCID.HornetHoney] = 1f;
                        pool[NPCID.BigHornetHoney] = 1f;
                    }
                    //if (!spawnInfo.Water)
                    //{
                    //	pool[NPCID.LittleHornetHoney] = 0.3f / 3;
                    //	pool[NPCID.HornetHoney] = 0.3f / 3;
                    //	pool[NPCID.BigHornetHoney] = 0.3f / 3;
                    //}
                }
                else if (spawnInfo.Player.InModBiome(ModContent.GetInstance<OceanCave>()) && spawnInfo.SpawnTileY > Main.worldSurface)
                {
                    if (spawnInfo.Water)
                    {
                        pool.Clear();

                        pool[NPCID.PinkJellyfish] = 1.5f;
                        pool[NPCID.Shark] = 0.5f;

                        pool[NPCID.Goldfish] = 0.5f;
                        pool[NPCID.Seahorse] = 0.5f;
                    }
                }
            }

            //if (SubworldSystem.IsActive<MansionSubworld>())
            //{
            //	//pool.Clear();

            //	if (tile.WallType != 0)
            //             {
            //		pool[NPCID.BlackRecluse] = 0.5f;
            //		//pool[NPCID.Mimic] = 0.1f;
            //	}
            //}

            //if (player.InModBiome(ModContent.GetInstance<Growth>()))
            //{
            //	pool.Clear();
            //	if (spawnInfo.SpawnTileY <= Main.maxTilesY - 200 && spawnInfo.SpawnTileY > Main.worldSurface)
            //	{
            //		if (!spawnInfo.Water)
            //		{
            //			pool[NPCID.Firefly] = 4f;
            //		}
            //	}
            //}

            if (!NPC.savedMech && NPC.downedBoss3 && !NPC.AnyNPCs(NPCID.BoundMechanic) && Main.wallDungeon[tile.WallType] && spawnInfo.SpawnTileY > Main.worldSurface + 15 && tile.TileType != TileID.Spikes)
            {
                pool[NPCID.BoundMechanic] = 1f;
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.InModBiome(ModContent.GetInstance<Biomes.Undergrowth>()))
            {
                spawnRate /= 2;
            }
            if (player.InModBiome(ModContent.GetInstance<OceanCave>()))
            {
                spawnRate /= 2;
            }
        }

        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<Arcanist>()] = 3;
            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<TomeofInferno>()] = 3;
            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<TomeofFrost>()] = 3;
            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<TomeofMending>()] = 3;
            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<TomeofSummoning>()] = 5;
            //ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<IlluminantDagger>()] = 3;

            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<Ward>()] = 5;
        }

        //public override void SetupShop(int type, Chest shop, ref int nextSlot)
        //{
        //	base.SetupShop(type, shop, ref nextSlot);

        //	if (type == NPCID.ArmsDealer)
        //	{
        //		shop.item[nextSlot].SetDefaults(ItemID.ExplosivePowder);
        //		nextSlot++;
        //	}
        //	if (type == NPCID.Demolitionist)
        //	{
        //		shop.item[nextSlot].SetDefaults(ItemID.ExplosivePowder);
        //		nextSlot++;
        //	}
        //}
    }

    #region dropconditions
    public class YoyosValor : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if ((!Main.hardMode || !NPC.downedPlantBoss) && info.player.ZoneDungeon && info.npc.lifeMax > 5 && info.npc.HasPlayerTarget && !info.npc.friendly && info.npc.value > 0f)
            {
                return !info.IsInSimulation;
            }

            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Bestiary_ItemDropConditions.YoyosKraken");
        }
    }
    #endregion
}