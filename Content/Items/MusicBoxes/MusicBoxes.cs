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

namespace Remnants.Content.Items.MusicBoxes;


#region Echoing Halls
public class EchoingHallsMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/EchoingHalls"), ModContent.ItemType<EchoingHallsMusicBox>(), ModContent.TileType<EchoingHallsMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<EchoingHallsMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion

#region Magical Labs
public class MagicalLabMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MagicalLab"), ModContent.ItemType<MagicalLabMusicBox>(), ModContent.TileType<MagicalLabMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<MagicalLabMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion

#region Forgotten Tomb
public class ForgottenTombMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Tomb"), ModContent.ItemType<ForgottenTombMusicBox>(), ModContent.TileType<ForgottenTombMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<ForgottenTombMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion

#region Undergrowth
public class UndergrowthMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Growth"), ModContent.ItemType<UndergrowthMusicBox>(), ModContent.TileType<UndergrowthMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<UndergrowthMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion

#region Aerial Garden
public class GardenMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/AerialGarden"), ModContent.ItemType<GardenMusicBox>(), ModContent.TileType<GardenMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<GardenMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion

#region Granite Cave
public class GraniteMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/GraniteCave"), ModContent.ItemType<GraniteMusicBox>(), ModContent.TileType<GraniteMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<GraniteMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion

#region Marble Cave
public class MarbleMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/MarbleCave"), ModContent.ItemType<MarbleMusicBox>(), ModContent.TileType<MarbleMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<MarbleMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}
#endregion