using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Remnants.Content.Tiles.MusicBoxes;
using System.IO;

namespace Remnants.Content.Items.MusicBoxes;


#region Echoing Halls
public class EchoingHallsMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.EchoingHalls.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/EchoingHalls"), ModContent.ItemType<EchoingHallsMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.EchoingHallsMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.EchoingHallsMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion

#region Magical Labs
public class MagicalLabMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.MagicalLab.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MagicalLab"), ModContent.ItemType<MagicalLabMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.MagicalLabMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.MagicalLabMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion

#region Forgotten Tomb
[LegacyName("ForgottenTombMusicBox")]
public class SpiderNestMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.SpiderNest.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpiderNest"), ModContent.ItemType<SpiderNestMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.SpiderNestMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.SpiderNestMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion

#region Undergrowth
public class UndergrowthMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.Undergrowth.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Undergrowth"), ModContent.ItemType<UndergrowthMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.UndergrowthMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.UndergrowthMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion

#region Aerial Garden
public class GardenMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.AerialGarden.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/AerialGarden"), ModContent.ItemType<GardenMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.GardenMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.GardenMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion

#region Granite Cave
public class GraniteMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.GraniteCave.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/GraniteCave"), ModContent.ItemType<GraniteMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.GraniteMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.GraniteMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion

#region Marble Cave
public class MarbleMusicBox : ModItem
{
    public override LocalizedText DisplayName => Language.GetText("{0} ({1})").WithFormatArgs(Language.GetText("ItemName.MusicBox"), Language.GetText("Mods.Remnants.Biomes.MarbleCave.DisplayName"));

    public override LocalizedText Tooltip => null;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MarbleCave"), ModContent.ItemType<MarbleMusicBox>(), ModContent.TileType<Tiles.MusicBoxes.MarbleMusicBox>());
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.MusicBoxAltOverworldDay);
        Item.createTile = ModContent.TileType<Tiles.MusicBoxes.MarbleMusicBox>();
        Item.placeStyle = 0;
    }
}
#endregion