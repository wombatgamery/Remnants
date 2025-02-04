using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;

namespace Remnants.Tiles.Objects.Furniture
{
    //public class ArcaneChest2 : ModTile
    //{
    //    public override void SetStaticDefaults()
    //    {
    //        Main.tileLighted[Type] = true;
    //        Main.tileSpelunker[Type] = true;
    //        Main.tileContainer[Type] = true;
    //        Main.tileShine2[Type] = true;
    //        Main.tileShine[Type] = 1200;
    //        Main.tileFrameImportant[Type] = true;
    //        Main.tileNoAttach[Type] = true;
    //        Main.tileOreFinderPriority[Type] = 500;

    //        TileID.Sets.HasOutlines[Type] = true;
    //        TileID.Sets.BasicChest[Type] = true;
    //        TileID.Sets.DisableSmartCursor[Type] = true;
    //        TileID.Sets.AvoidedByNPCs[Type] = true;
    //        TileID.Sets.InteractibleByNPCs[Type] = true;
    //        TileID.Sets.IsAContainer[Type] = true;

    //        TileObjectData.newTile.FullCopyFrom(TileID.Containers);
    //        TileObjectData.addTile(Type);

    //        AddMapEntry(new Color(186, 186, 211), CreateMapEntryName(), MapChestName);

    //        DustType = DustID.Iron;
    //        AdjTiles = new int[] { TileID.Containers };
    //    }

    //    public override LocalizedText DefaultContainerName(int frameX, int frameY)
    //    {
    //        return CreateMapEntryName();
    //    }

    //    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

    //    public string MapChestName(string name, int i, int j)
    //    {
    //        Tile tile = Main.tile[i, j];
    //        int left = i - tile.TileFrameX / 18;
    //        int top = j;
    //        if (tile.TileFrameY != 0)
    //        {
    //            top--;
    //        }
    //        int chest = Chest.FindChest(left, top);
    //        if (chest < 0)
    //        {
    //            return Language.GetTextValue("LegacyChestType.0");
    //        }
    //        else if (Main.chest[chest].name == "")
    //        {
    //            return name;
    //        }
    //        else
    //        {
    //            return name + ": " + Main.chest[chest].name;
    //        }
    //    }

    //    public override void NumDust(int i, int j, bool fail, ref int num)
    //    {
    //        num = 1;
    //    }

    //    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    //    {
    //        Tile tile = Main.tile[i, j];

    //        if (tile.TileFrameX == 18 * 2)
    //        {
    //            int left = i - 2;
    //            int top = j;
    //            if (Main.tile[left, j].TileFrameY != 0)
    //            {
    //                top--;
    //            }

    //            int chest = Chest.FindChest(left, top);

    //            if (Main.chest[chest].frame != 0)
    //            {
    //                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
    //                if (Main.drawToScreen)
    //                {
    //                    zero = Vector2.Zero;
    //                }
    //                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(36, Main.chest[chest].frame * 38 + (top == j ? 0 : 18), 16, (top == j ? 16 : 18)), Lighting.GetColor(i, j) * 1.75f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    //    {
    //        Tile tile = Main.tile[i, j];

    //        if (tile.TileFrameY == 18)
    //        {
    //            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
    //            if (Main.drawToScreen)
    //            {
    //                zero = Vector2.Zero;
    //            }
    //            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + 18 * 3, 18, 16, 16), RemTile.MagicalLabLightColour(i), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    //            //Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + 18 * 3, 56, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    //        }
    //    }

    //    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    //    {
    //        Tile tile = Main.tile[i, j];
    //        if (tile.TileFrameY == 18)
    //        {
    //            Color color = RemTile.MagicalLabLightColour(i, true);
    //            //RemTile.RGBLight(r, g, b, 112, 93, 133);
    //            r = color.R / 255f;// (241f / 255f) * mult;
    //            g = color.G / 255f;// (195f / 255f) * mult;
    //            b = color.B / 255f;// (233f / 255f) * mult;
    //        }
    //    }

    //    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    //    {
    //        Chest.DestroyChest(i, j);
    //    }

    //    public override bool CanKillTile(int i, int j, ref bool blockDamaged)
    //    {
    //        if (Main.tile[i, j].TileFrameX == 18 * 2)
    //        {
    //            int left = i - 2;
    //            int top = j;
    //            if (Main.tile[left, j].TileFrameY != 0)
    //            {
    //                top--;
    //            }

    //            int chest = Chest.FindChest(left, top);

    //            for (int k = 0; k < 40; k++)
    //            {
    //                if (!Main.chest[chest].item[k].IsAir)
    //                {
    //                    return false;
    //                }
    //            }
    //        }
    //        return true;
    //    }

    //    public override bool RightClick(int i, int j)
    //    {
    //        Player player = Main.LocalPlayer;
    //        Tile tile = Main.tile[i, j];
    //        Main.mouseRightRelease = false;
    //        int left = i - tile.TileFrameX / 18;
    //        int top = j;
    //        if (tile.TileFrameY != 0)
    //        {
    //            top--;
    //        }
    //        if (player.sign >= 0)
    //        {
    //            SoundEngine.PlaySound(SoundID.MenuClose);
    //            player.sign = -1;
    //            Main.editSign = false;
    //            Main.npcChatText = "";
    //        }
    //        if (Main.editChest)
    //        {
    //            SoundEngine.PlaySound(SoundID.MenuOpen);
    //            Main.editChest = false;
    //            Main.npcChatText = "";
    //        }
    //        if (player.editedChestName)
    //        {
    //            NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
    //            player.editedChestName = false;
    //        }
    //        // bool isLocked = IsLockedChest(left, top);
    //        bool isLocked = false;
    //        if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
    //        {
    //            if (left == player.chestX && top == player.chestY && player.chest >= 0)
    //            {
    //                player.chest = -1;
    //                Recipe.FindRecipes();
    //                SoundEngine.PlaySound(SoundID.MenuClose);
    //            }
    //            else
    //            {
    //                NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top, 0f, 0f, 0, 0, 0);
    //                Main.stackSplit = 600;
    //            }
    //        }
    //        else
    //        {
    //            int chest = Chest.FindChest(left, top);
    //            if (chest >= 0)
    //            {
    //                Main.stackSplit = 600;
    //                if (chest == player.chest)
    //                {
    //                    player.chest = -1;
    //                    SoundEngine.PlaySound(SoundID.MenuClose);
    //                }
    //                else
    //                {
    //                    player.chest = chest;
    //                    Main.playerInventory = true;
    //                    Main.recBigList = false;
    //                    player.chestX = left;
    //                    player.chestY = top;
    //                    SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
    //                }
    //                Recipe.FindRecipes();
    //            }
    //        }
    //        return true;
    //    }

    //    public override void MouseOver(int i, int j)
    //    {
    //        Player player = Main.LocalPlayer;
    //        Tile tile = Main.tile[i, j];
    //        int left = i - tile.TileFrameX / 18;
    //        int top = j;
    //        if (tile.TileFrameY != 0)
    //        {
    //            top--;
    //        }
    //        int chest = Chest.FindChest(left, top);
    //        player.cursorItemIconID = -1;
    //        if (chest < 0)
    //        {
    //            player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
    //        }
    //        else
    //        {
    //            player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Arcane Chest";
    //            if (player.cursorItemIconText == "Arcane Chest")
    //            {
    //                player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Objects.ArcaneChest>();
    //                player.cursorItemIconText = "";
    //            }
    //        }
    //        player.noThrow = 2;
    //        player.cursorItemIconEnabled = true;
    //    }

    //    public override void MouseOverFar(int i, int j)
    //    {
    //        MouseOver(i, j);
    //        Player player = Main.LocalPlayer;
    //        if (player.cursorItemIconText == "")
    //        {
    //            player.cursorItemIconEnabled = false;
    //            player.cursorItemIconID = 0;
    //        }
    //    }
    //}

    public class ArcaneChest : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileOreFinderPriority[Type] = 500;

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.BasicChest[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;

            TileObjectData.newTile.FullCopyFrom(TileID.Containers);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(186, 186, 211), CreateMapEntryName(), MapChestName);

            DustType = DustID.Iron;
            AdjTiles = new int[] { TileID.Containers };
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            return CreateMapEntryName();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public string MapChestName(string name, int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j;
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            if (chest < 0)
            {
                return Language.GetTextValue("LegacyChestType.0");
            }
            else if (Main.chest[chest].name == "")
            {
                return name;
            }
            else
            {
                return name + ": " + Main.chest[chest].name;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX == 18 * 2)
            {
                int left = i - 2;
                int top = j;
                if (Main.tile[left, j].TileFrameY != 0)
                {
                    top--;
                }

                int chest = Chest.FindChest(left, top);

                if (Main.chest[chest].frame != 0)
                {
                    Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                    if (Main.drawToScreen)
                    {
                        zero = Vector2.Zero;
                    }
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(36, Main.chest[chest].frame * 38 + (top == j ? 0 : 18), 16, (top == j ? 16 : 18)), Lighting.GetColor(i, j) * 1.75f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                    return false;
                }
            }
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameY == 18)
            {
                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                if (Main.drawToScreen)
                {
                    zero = Vector2.Zero;
                }
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + 18 * 3, 18, 16, 16), RemTile.MagicalLabLightColour(i), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX + 18 * 3, 56, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 18)
            {
                Color color = RemTile.MagicalLabLightColour(i, true);
                //RemTile.RGBLight(r, g, b, 112, 93, 133);
                r = color.R / 255f;// (241f / 255f) * mult;
                g = color.G / 255f;// (195f / 255f) * mult;
                b = color.B / 255f;// (233f / 255f) * mult;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (Main.tile[i, j].TileFrameX == 18 * 2)
            {
                int left = i - 2;
                int top = j;
                if (Main.tile[left, j].TileFrameY != 0)
                {
                    top--;
                }

                int chest = Chest.FindChest(left, top);

                for (int k = 0; k < 40; k++)
                {
                    if (!Main.chest[chest].item[k].IsAir)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i - tile.TileFrameX / 18;
            int top = j;
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }
            // bool isLocked = IsLockedChest(left, top);
            bool isLocked = false;
            if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top, 0f, 0f, 0, 0, 0);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                    }
                    Recipe.FindRecipes();
                }
            }
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j;
            if (tile.TileFrameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Arcane Chest";
                if (player.cursorItemIconText == "Arcane Chest")
                {
                    player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Objects.ArcaneChest>();
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
    }
}