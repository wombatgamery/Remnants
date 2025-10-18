using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace Remnants.Content.Tiles.MusicBoxes;

#region Echoing Halls
public class EchoingHallsMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.EchoingHallsMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.EchoingHallsMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion

#region Magical Labs
public class MagicalLabMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.MagicalLabMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.MagicalLabMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion

#region Forgotten Tomb
[LegacyName("ForgottenTombMusicBox")]
public class SpiderNestMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.SpiderNestMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.SpiderNestMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion

#region Undergrowth
public class UndergrowthMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.UndergrowthMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.UndergrowthMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion

#region Aerial Garden
public class GardenMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.GardenMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.GardenMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion

#region Granite Cave
public class GraniteMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.GraniteMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.GraniteMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion

#region Marble Cave
public class MarbleMusicBox : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.FullCopyFrom(TileID.MusicBoxes);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        RegisterItemDrop(ModContent.ItemType<Items.MusicBoxes.MarbleMusicBox>());
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.MusicBoxes.MarbleMusicBox>();
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
        {
            return;
        }

        int MusicNote = Main.rand.Next(570, 573);
        Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
        Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
        NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
        NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
        switch (MusicNote)
        {
            case 572:
                SpawnPosition.X -= 8f;
                break;
            case 571:
                SpawnPosition.X -= 4f;
                break;
        }

        Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
    }
}
#endregion