using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using Remnants.Content.Items.MusicBoxes;

namespace Remnants.Content.Tiles.MusicBoxes;

#region Echoing Halls
public class EchoingHallsMusicBoxTile : ModTile
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
        RegisterItemDrop(ModContent.ItemType<EchoingHallsMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<EchoingHallsMusicBox>();
    }
}
#endregion

#region Magical Labs
public class MagicalLabMusicBoxTile : ModTile
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
        RegisterItemDrop(ModContent.ItemType<MagicalLabMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<MagicalLabMusicBox>();
    }
}
#endregion

#region Forgotten Tomb
public class ForgottenTombMusicBoxTile : ModTile
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
        RegisterItemDrop(ModContent.ItemType<ForgottenTombMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<ForgottenTombMusicBox>();
    }
}
#endregion

#region Undergrowth
public class UndergrowthMusicBoxTile : ModTile
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
        RegisterItemDrop(ModContent.ItemType<UndergrowthMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<UndergrowthMusicBox>();
    }
}
#endregion

#region Aerial Garden
public class GardenMusicBoxTile : ModTile
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
        RegisterItemDrop(ModContent.ItemType<GardenMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<GardenMusicBox>();
    }
}
#endregion

#region Granite Cave
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

#region Marble Cave
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