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

namespace Remnants.Items.MusicBoxes;


#region Eching Halls
public class EchoingMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/darkambient2b"), ModContent.ItemType<EchoingMusicBox>(), ModContent.TileType<EchoingMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<EchoingMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}

public class EchoingMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<EchoingMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<EchoingMusicBox>();
    }
}
#endregion

#region Magical Labs
public class MagicalMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/archaic_vault"), ModContent.ItemType<MagicalMusicBox>(), ModContent.TileType<MagicalMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<MagicalMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}

public class MagicalMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<MagicalMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<MagicalMusicBox>();
    }
}
#endregion

#region Forgotten Tomb
public class ForgottenMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/fog"), ModContent.ItemType<ForgottenMusicBox>(), ModContent.TileType<ForgottenMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<ForgottenMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}

public class ForgottenMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<ForgottenMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<ForgottenMusicBox>();
    }
}
#endregion

#region Undergrowth
public class RootCoveredMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/growth"), ModContent.ItemType<RootCoveredMusicBox>(), ModContent.TileType<RootCoveredMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<RootCoveredMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}

public class RootCoveredMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<RootCoveredMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<RootCoveredMusicBox>();
    }
}
#endregion

#region Aerial Garden
public class CloudyMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/depthsOfDespair"), ModContent.ItemType<CloudyMusicBox>(), ModContent.TileType<CloudyMusicBoxTile>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<CloudyMusicBoxTile>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = 100000;
        Item.accessory = true;
    }
}

public class CloudyMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<CloudyMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<CloudyMusicBox>();
    }
}
#endregion

#region Granite Cavern
public class GraniteMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/music_dark_somethingominous"), ModContent.ItemType<GraniteMusicBox>(), ModContent.TileType<GraniteMusicBoxTile>());
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

public class GraniteMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<GraniteMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<GraniteMusicBox>();
    }
}
#endregion

#region Marble Cavern
public class MarbleMusicBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/music_dark_fog"), ModContent.ItemType<MarbleMusicBox>(), ModContent.TileType<MarbleMusicBoxTile>());
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

public class MarbleMusicBoxTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(200, 200, 200), name);
        RegisterItemDrop(ModContent.ItemType<MarbleMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<MarbleMusicBox>();
    }
}
#endregion