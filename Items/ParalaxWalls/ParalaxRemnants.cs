using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Remnants.Walls.Parallax;
using Remnants.Items.Placeable.Blocks;
using Remnants.Tiles.Blocks;
using Remnants.Walls;
using Terraria;

namespace Remnants.Items.ParalaxWalls;

public class ParalaxDungeonB : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<dungeonblue>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.AncientBlueDungeonBrick).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class ParalaxDungeonG : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<dungeongreen>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.AncientGreenDungeonBrick).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class ParalaxDungeonP : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<dungeonpink>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.AncientPinkDungeonBrick).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class Tomb : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<forgottentomb>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient<Placeable.Blocks.TombBrick>().
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class ParaHive : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<hive>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.Hive).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class LabPara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<magicallab>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient<Placeable.Blocks.StarOre>().
            AddIngredient(ItemID.StoneBlock).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class LabFly : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<Ascension>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient<Placeable.Blocks.StarOre>().
            AddIngredient(ItemID.Cloud).
            AddIngredient(ItemID.StoneBlock).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class PyramidPara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<pyramid>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient<Placeable.Blocks.PyramidBrick>().
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class StrongholdPara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<stronghold>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.DirtBlock).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class TemplePara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<temple>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.LihzahrdBrick).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class TreePara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<undergrowth>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.Wood).
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class VaultPara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<vault>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient<Placeable.Blocks.VaultPlating>().
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class WaterPara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<watertemple>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient<WaterBrick>().
            AddTile(TileID.WorkBenches).
            Register();
    }
}
public class MazePara : ModItem
{
    public override void SetDefaults()
    {
        Item.createWall = ModContent.WallType<whisperingmaze>();
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).
            AddIngredient(ItemID.Wood).
            AddTile(TileID.WorkBenches).
            Register();
    }
}

public class BreakWalls : GlobalWall
{
    public override void KillWall(int i, int j, int type, ref bool fail)
    {
        if (type == ModContent.WallType<Ascension>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<vault>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<whisperingmaze>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<dungeonblue>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<dungeongreen>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<dungeonpink>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<forgottentomb>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<goldenconsole>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<goldenmonitor>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<goldenserver>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<GoldenCity>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<hive>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<magicallab>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<pyramid>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<stronghold>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<temple>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<watertemple>())
        {
            fail = false;
        }
        if (type == ModContent.WallType<undergrowth>())
        {
            fail = false;
        }
    }
}