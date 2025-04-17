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
using Remnants.Content.Items.Consumable;

namespace Remnants.Content.Tiles.Objects.Furniture
{
    public class PyramidChest : ModTile
    {
        public override void SetStaticDefaults()
        {
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
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(158, 167, 191), CreateMapEntryName(), MapChestName);
            AddMapEntry(new Color(158, 167, 191), this.GetLocalization("MapEntry0"), MapChestName);

            DustType = DustID.Lead;
            AdjTiles = new int[] { TileID.Containers };
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CreateMapEntryName();

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX >= 36;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int DustType, ref bool manual)
        {
            DustType = this.DustType;
            return true;
        }

        public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual)
        {
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            // We need to return true only if the tile style is the unlocked variant of a chest that supports locking. 
            if (style == 0)
            {
                // We can check other conditions as well, such as how biome chests can't be locked until Plantera is defeated
                return true;
            }
            return false;
        }

        public string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }
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

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            player.CloseSign();
            player.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }

            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }

            bool isLocked = Chest.IsLocked(left, top);
            if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
            {
                if (left == player.chestX && top == player.chestY && player.chest != -1)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                if (isLocked)
                {
                    // Make sure to change the code in UnlockChest if you don't want the chest to only unlock at night.
                    int key = ModContent.ItemType<PyramidKey>();
                    if (player.ConsumeItem(key, includeVoidBag: true) && Chest.Unlock(left, top))
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
                        }
                    }
                    else Main.NewText(Language.GetTextValue("Mods.Remnants.Content.Tiles.PyramidChest.Messages.KeysinUnderground"), new Color(255, 204, 102));
                }
                else
                {
                    int chest = Chest.FindChest(left, top);
                    if (chest != -1)
                    {
                        Main.stackSplit = 600;
                        if (chest == player.chest)
                        {
                            player.chest = -1;
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }
                        else
                        {
                            SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                            player.OpenChest(left, top, chest);
                        }

                        Recipe.FindRecipes();
                    }
                }
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }
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
                string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); // This gets the ContainerName text for the currently selected language
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Objects.PyramidChest>();
                    if (Main.tile[left, top].TileFrameX / 36 == 1)
                    {
                        player.cursorItemIconID = ModContent.ItemType<PyramidKey>();
                    }

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
