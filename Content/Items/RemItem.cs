using Microsoft.Xna.Framework;
using Remnants.Content.Biomes;
using Remnants.Content.Items.Materials;
using Remnants.Content.Items.Placeable.Blocks;
using Remnants.Content.Projectiles.vanilla;
using Remnants.Content.Tiles;
using Remnants.Content.Dusts;
using Remnants.Content.World;
//using SubworldLibrary;
using System.Collections.Generic;
using System.Media;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Remnants.Content.Walls.EchoingHalls;
using Remnants.Content.Walls.Shimmer;

namespace Remnants.Content.Items
{
    public class RemItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            //if (ModContent.GetInstance<Gameplay>().ProjectileAI)
            //{
            //    if (item.type == ItemID.Shuriken)
            //    {
            //        item.crit = 4;
            //        item.shootSpeed *= 1.2f;
            //        item.autoReuse = true;
            //    }
            //    else if (item.type == ItemID.PoisonedKnife)
            //    {
            //        item.CloneDefaults(ItemID.ThrowingKnife);
            //        item.damage += 2;
            //        item.shoot = ProjectileID.PoisonedKnife;
            //    }
            //    //else if (item.type == ItemID.MolotovCocktail)
            //    //{
            //    //	item.shoot = ModContent.ProjectileType<molotovcocktail>();
            //    //	item.shootSpeed *= 1.2f;
            //    //}
            //    else if (item.type == ItemID.ShadowFlameKnife)
            //    {
            //        item.shootSpeed *= 1.4f;
            //    }
            //    else if (item.type == ItemID.Javelin || item.type == ItemID.BoneJavelin)
            //    {
            //        item.crit = 8;
            //    }
            //    else if (item.type == ItemID.BoneDagger)
            //    {
            //        item.damage += 2;
            //        item.crit = 4;
            //        item.knockBack = 4;
            //    }

            //    if (item.type == ItemID.Flamethrower || item.type == ItemID.ElfMelter)
            //    {
            //        item.damage = item.damage * 2;
            //        item.shoot = ModContent.ProjectileType<flamejet>();
            //    }
            //}

            //if (item.type == ItemID.CobaltNaginata || item.type == ItemID.PalladiumPike || item.type == ItemID.MythrilHalberd || item.type == ItemID.OrichalcumHalberd || item.type == ItemID.AdamantiteGlaive || item.type == ItemID.TitaniumTrident || item.type == ItemID.Gungnir)
            //{
            //	item.useTime += 5;
            //	item.autoReuse = true;
            //}
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (Main.tile[(int)(item.Center.X / 16), (int)((item.position.Y + item.height) / 16)].WallType == ModContent.WallType<Ascension>())
            {
                gravity = 0;
                item.velocity.Y *= 0.95f;
                item.velocity.Y -= 0.2f;
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Tile tile = Main.tile[x, y];
            if (tile.WallType == ModContent.WallType<LabyrinthTileWall>() || tile.WallType == ModContent.WallType<LabyrinthBrickWall>() || tile.WallType == ModContent.WallType<whisperingmaze>())
            {
                if (item.type == ItemID.RodofDiscord || item.type == ItemID.RodOfHarmony)
                {
                    Main.NewText(Language.GetTextValue("Mods.Remnants.Chat.LabyrinthTeleportation"), 120, 242, 255);
                    return false;
                }
            }
            return true;
        }

        //   public override bool? UseItem(Item item, Player player)
        //   {
        //       if (player.InModBiome<EchoingHalls>())
        //       {
        //           if (item.type == ItemID.RodofDiscord || item.type == ItemID.RodOfHarmony || item.type == ItemID.WormholePotion)
        //           {
        ////for (int k = 0; k < 20; k++)
        ////{
        ////    Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<spiritenergy>());
        ////}

        ////player.position.X = RemWorld.whisperingMazeX * 16 - player.width / 2;
        ////player.position.Y = (RemWorld.whisperingMazeY + 1) * 16 - player.height;

        ////for (int k = 0; k < 20; k++)
        ////{
        ////    Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<spiritenergy>());
        ////}
        //return false;
        //           }
        //       }

        //       return null;
        //   }

        //public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        //{
        //	if (item.type == ItemID.ExplosivePowder)
        //	{
        //		var line = new TooltipLine(Mod, "Face", "A volatile substance ideal for guns and explosives");
        //		tooltips.Add(line);
        //	}
        //	//if (item.type == ItemID.CursedFlame)
        //	//{
        //	//	var line = new TooltipLine(mod, "Face", "A magic fire with properties specialized for death and destruction.");
        //	//	tooltips.Add(line);
        //	//}
        //}
    }

    //public class NoGrind : GlobalItem
    //{
    //	public override void AddRecipes()
    //	{
    //		DeleteRecipes(ItemID.GrayBrick);
    //		DeleteRecipes(ItemID.RedBrick);

    //		DeleteRecipes(ItemID.SandstoneBrick);
    //		DeleteRecipes(ItemID.MudstoneBlock);
    //		DeleteRecipes(ItemID.ObsidianBrick);
    //		DeleteRecipes(ItemID.IridescentBrick);

    //		DeleteRecipes(ItemID.EbonstoneBrick);
    //		DeleteRecipes(ItemID.PearlstoneBrick);

    //		DeleteRecipes(ItemID.StoneSlab);
    //		DeleteRecipes(ItemID.SandstoneSlab);

    //		ModRecipe

    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.GrayBrick, 5);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.ClayBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.RedBrick, 5);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.SandBlock);
    //		modrecipe.AddIngredient(ItemID.Sandstone);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.SandstoneBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.MudBlock);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.MudstoneBlock, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.Obsidian);
    //		modrecipe.AddIngredient(mod, "lavastone");
    //		modrecipe.AddTile(TileID.Hellforge); modrecipe.SetResult(ItemID.ObsidianBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.AshBlock);
    //		modrecipe.AddIngredient(mod, "lavastone");
    //		modrecipe.AddTile(TileID.Hellforge); modrecipe.SetResult(ItemID.IridescentBrick, 10);
    //		modrecipe.Register();

    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.EbonstoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.EbonstoneBrick, 5);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.PearlstoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.PearlstoneBrick, 5);
    //		modrecipe.Register();

    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.StoneSlab, 5);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.SandBlock);
    //		modrecipe.AddIngredient(ItemID.Sandstone);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.SandstoneSlab, 10);
    //		modrecipe.Register();

    //		DeleteRecipes(ItemID.CopperBrick);
    //		DeleteRecipes(ItemID.TinBrick);
    //		DeleteRecipes(ItemID.CopperPlating);
    //		DeleteRecipes(ItemID.TinPlating);
    //		DeleteRecipes(ItemID.SilverBrick);
    //		DeleteRecipes(ItemID.TungstenBrick);
    //		DeleteRecipes(ItemID.GoldBrick);
    //		DeleteRecipes(ItemID.PlatinumBrick);
    //		DeleteRecipes(ItemID.MeteoriteBrick);
    //		DeleteRecipes(ItemID.DemoniteBrick);
    //		DeleteRecipes(ItemID.CrimtaneBrick);
    //		DeleteRecipes(ItemID.HellstoneBrick);
    //		DeleteRecipes(ItemID.CobaltBrick);
    //		DeleteRecipes(ItemID.PalladiumColumn);
    //		DeleteRecipes(ItemID.MythrilBrick);
    //		DeleteRecipes(ItemID.BubblegumBlock);
    //		DeleteRecipes(ItemID.AdamantiteBeam);
    //		DeleteRecipes(ItemID.TitanstoneBlock);
    //		DeleteRecipes(ItemID.ChlorophyteBrick);
    //		DeleteRecipes(ItemID.LunarBrick);

    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.CopperBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.CopperBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.TinBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.TinBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.CopperBar);
    //		modrecipe.AddTile(TileID.HeavyWorkBench); modrecipe.SetResult(ItemID.CopperPlating, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.TinBar);
    //		modrecipe.AddTile(TileID.HeavyWorkBench); modrecipe.SetResult(ItemID.TinPlating, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.SilverBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.SilverBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.TungstenBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.TungstenBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.GoldBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.GoldBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.PlatinumBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.PlatinumBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.MeteoriteBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.MeteoriteBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.DemoniteBar);
    //		modrecipe.AddIngredient(ItemID.EbonstoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.DemoniteBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.CrimtaneBar);
    //		modrecipe.AddIngredient(ItemID.CrimstoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.CrimtaneBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.HellstoneBar);
    //		modrecipe.AddIngredient(mod, "lavastone");
    //		modrecipe.AddTile(TileID.Hellforge); modrecipe.SetResult(ItemID.HellstoneBrick, 10);
    //		modrecipe.Register();

    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.CobaltBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.CobaltBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.PalladiumBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.PalladiumColumn, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.MythrilBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.MythrilBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.OrichalcumBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.BubblegumBlock, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.AdamantiteBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.AdamantiteBeam, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.TitaniumBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.TitanstoneBlock, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.ChlorophyteBar);
    //		modrecipe.AddIngredient(ModContent.ItemType<mudstone>());
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.ChlorophyteBrick, 10);
    //		modrecipe.Register();
    //		modrecipe = Recipe.Create(mod);
    //		modrecipe.AddIngredient(ItemID.LunarBar);
    //		modrecipe.AddIngredient(ItemID.StoneBlock);
    //		modrecipe.AddTile(TileID.Furnaces); modrecipe.SetResult(ItemID.LunarBrick, 10);
    //		modrecipe.Register();
    //	}

    //	private void DeleteRecipes(int item)
    //       {
    //		RecipeFinder finder = new RecipeFinder();
    //		finder.SetResult(item);
    //		foreach (Recipe recipefinder in finder.SearchRecipes())
    //		{ 
    //			RecipeEditor editor = new RecipeEditor(recipefinder);
    //			editor.DeleteRecipe();
    //		}
    //	}
    //}

    public class BetterRecipes : ModSystem
    {
        public override void PostAddRecipes()
        {
            Recipe recipe;

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
            }

            #region misc
            recipe = Recipe.Create(ItemID.Bottle);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.Register();
            recipe = Recipe.Create(ItemID.Bottle);
            recipe.AddIngredient(ItemID.BottledHoney);
            recipe.Register();

            recipe = Recipe.Create(ItemID.MusketBall, 99);
            recipe.AddIngredient(ItemID.LeadBar, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            //recipe = Recipe.Create(ItemID.Grenade, 6);
            //recipe.AddIngredient(ItemID.IronBar, 1);
            //recipe.AddIngredient(ItemID.ExplosivePowder, 1);
            //recipe.AddTile(TileID.Anvils);
            //recipe.Register();

            //recipe = Recipe.Create(ItemID.Bomb, 9);
            //recipe.AddIngredient(ItemID.IronBar, 2);
            //recipe.AddIngredient(ItemID.ExplosivePowder, 2);
            //recipe.AddTile(TileID.Anvils);
            //recipe.Register();

            //recipe = Recipe.Create(ItemID.Hook);
            //recipe.AddRecipeGroup(RecipeGroupID.IronBar);
            //recipe.AddTile(TileID.Anvils);
            //recipe.Register();

            DisableRecipes(ItemID.Keg);
            recipe = Recipe.Create(ItemID.Keg);
            recipe.AddIngredient(ItemID.Barrel);
            recipe.AddRecipeGroup(RecipeGroupID.Wood, 5);
            recipe.AddTile(TileID.Sawmill);
            recipe.Register();

            recipe = Recipe.Create(ItemID.ThrowingKnife, 30);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar);
            recipe.AddRecipeGroup(RecipeGroupID.Wood);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            recipe = Recipe.Create(ItemID.DivingHelmet);
            recipe.AddIngredient(ItemID.CopperBar, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            #endregion

            recipe = Recipe.Create(ItemID.LihzahrdBrick, 25);
            recipe.AddIngredient(ModContent.ItemType<Placeable.Blocks.Hardstone>(), 5);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddIngredient(ItemID.MudBlock, 5);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();

            #region armor
            DisableRecipes(ItemID.FrostHelmet);
            DisableRecipes(ItemID.FrostBreastplate);
            DisableRecipes(ItemID.FrostLeggings);

            recipe = Recipe.Create(ItemID.FrostHelmet); recipe.AddIngredient(ItemID.AdamantiteHelmet); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostHelmet); recipe.AddIngredient(ItemID.AdamantiteMask); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostHelmet); recipe.AddIngredient(ItemID.AdamantiteHeadgear); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostBreastplate); recipe.AddIngredient(ItemID.AdamantiteBreastplate); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostLeggings); recipe.AddIngredient(ItemID.AdamantiteLeggings); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();

            recipe = Recipe.Create(ItemID.FrostHelmet); recipe.AddIngredient(ItemID.TitaniumHelmet); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostHelmet); recipe.AddIngredient(ItemID.TitaniumMask); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostHelmet); recipe.AddIngredient(ItemID.TitaniumHeadgear); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostBreastplate); recipe.AddIngredient(ItemID.TitaniumBreastplate); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.FrostLeggings); recipe.AddIngredient(ItemID.TitaniumLeggings); recipe.AddIngredient(ItemID.FrostCore); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();

            DisableRecipes(ItemID.AncientBattleArmorHat);
            DisableRecipes(ItemID.AncientBattleArmorShirt);
            DisableRecipes(ItemID.AncientBattleArmorPants);

            recipe = Recipe.Create(ItemID.AncientBattleArmorHat); recipe.AddIngredient(ItemID.AdamantiteHelmet); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorHat); recipe.AddIngredient(ItemID.AdamantiteMask); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorHat); recipe.AddIngredient(ItemID.AdamantiteHeadgear); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorShirt); recipe.AddIngredient(ItemID.AdamantiteBreastplate); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorPants); recipe.AddIngredient(ItemID.AdamantiteLeggings); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorHat); recipe.AddIngredient(ItemID.TitaniumHelmet); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorHat); recipe.AddIngredient(ItemID.TitaniumMask); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorHat); recipe.AddIngredient(ItemID.TitaniumHeadgear); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorShirt); recipe.AddIngredient(ItemID.TitaniumBreastplate); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.AncientBattleArmorPants); recipe.AddIngredient(ItemID.TitaniumLeggings); recipe.AddIngredient(ItemID.AncientBattleArmorMaterial); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            DisableRecipes(ItemID.TurtleHelmet);
            DisableRecipes(ItemID.TurtleScaleMail);
            DisableRecipes(ItemID.TurtleLeggings);

            recipe = Recipe.Create(ItemID.TurtleHelmet); recipe.AddIngredient(ItemID.ChlorophyteMask); recipe.AddIngredient(ItemID.TurtleShell, 6); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.TurtleHelmet); recipe.AddIngredient(ItemID.ChlorophyteHelmet); recipe.AddIngredient(ItemID.TurtleShell, 6); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.TurtleHelmet); recipe.AddIngredient(ItemID.ChlorophyteHeadgear); recipe.AddIngredient(ItemID.TurtleShell, 6); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.TurtleScaleMail); recipe.AddIngredient(ItemID.ChlorophytePlateMail); recipe.AddIngredient(ItemID.TurtleShell, 12); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            recipe = Recipe.Create(ItemID.TurtleLeggings); recipe.AddIngredient(ItemID.ChlorophyteGreaves); recipe.AddIngredient(ItemID.TurtleShell, 9); recipe.AddTile(TileID.MythrilAnvil); recipe.Register();
            #endregion
        }

        private void DisableRecipes(int item)
        {
            for (int index = 0; index < Main.recipe.Length; index++)
            {
                if (Main.recipe[index] != null && Main.recipe[index].TryGetResult(item, out Item result))
                {
                    Main.recipe[index].DisableRecipe();
                }
            }
        }
    }
}
