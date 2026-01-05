using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Remnants.Content.Tiles;
using Remnants.Content.Dusts;
using Remnants.Content.Projectiles;
using Terraria.GameContent.Drawing;

namespace Remnants.Content.Tiles
{
    public class SinewCorruption : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.GreenBlood;
            HitSound = SoundID.NPCHit9;

            AddMapEntry(new Color(101, 74, 114));
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].LiquidAmount == 0 && !WorldGen.SolidOrSlopedTile(i, j + 2))
            {
                bool maxLength = true;

                for (int a = 0; a < 10; a++)
                {
                    if (Main.tile[i, j - a].TileType != Type)
                    {
                        maxLength = false;
                        break;
                    }
                }

                if (!maxLength)
                {
                    WorldGen.PlaceTile(i, j + 1, Type, true);
                }
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            bool anchor = true;

            if (!Main.tile[i, j - 1].HasTile)
            {
                anchor = false;
            }
            else if (!TileID.Sets.Corrupt[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].TileType != Type)
            {
                anchor = false;
            }
            else if (Main.tile[i, j - 1].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j - 1].Slope == SlopeType.SlopeUpRight)
            {
                anchor = false;
            }

            if (!anchor)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }
    }

    [LegacyName("Eyeball")]
    [LegacyName("EyeballVine")]
    public class SinewCrimson : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.BloodWater;
            HitSound = SoundID.NPCHit9;

            AddMapEntry(new Color(202, 50, 55));
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].LiquidAmount == 0 && !WorldGen.SolidOrSlopedTile(i, j + 2))
            {
                bool maxLength = true;

                for (int a = 0; a < 10; a++)
                {
                    if (Main.tile[i, j - a].TileType != Type)
                    {
                        maxLength = false;
                        break;
                    }
                }

                if (!maxLength)
                {
                    WorldGen.PlaceTile(i, j + 1, Type, true);
                }
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            bool anchor = true;

            if (!Main.tile[i, j - 1].HasTile)
            {
                anchor = false;
            }
            else if (!TileID.Sets.Crimson[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].TileType != Type)
            {
                anchor = false;
            }
            else if (Main.tile[i, j - 1].Slope == SlopeType.SlopeUpLeft || Main.tile[i, j - 1].Slope == SlopeType.SlopeUpRight)
            {
                anchor = false;
            }

            if (!anchor)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }
    }
}