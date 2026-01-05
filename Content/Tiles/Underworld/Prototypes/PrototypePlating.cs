using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Dusts;
using Remnants.Content.Walls.Underworld;
using Remnants.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Tiles.Underworld.Prototypes
{
	[LegacyName("vaultbrick")]
	[LegacyName("vaultbrickrusted")]
    [LegacyName("VaultPlating")]
    public class PrototypePlating : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Ash][Type] = true;
            Main.tileMerge[TileID.AshGrass][Type] = true;
            Main.tileMerge[TileID.Grate][Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
			TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = false;

            MinPick = 200;
			MineResist = 4;
			DustType = DustID.Iron;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(90, 79, 90));
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			if (BlendingTile(i - 1, j) && BlendingTile(i + 1, j) && BlendingTile(i, j - 1) && BlendingTile(i, j + 1))
			{
				bool top = !BlendingTile(i - 1, j - 1) || !BlendingTile(i + 1, j - 1);
				bool bottom = !BlendingTile(i - 1, j + 1) || !BlendingTile(i + 1, j + 1);

				if (top ^ bottom)
                {
					bool left = !BlendingTile(i - 1, j + (bottom ? 1 : -1));
					bool right = !BlendingTile(i + 1, j + (bottom ? 1 : -1));

					if (left ^ right)
					{
						Tile tile = Main.tile[i, j];

						tile.TileFrameX = (short)(Main.rand.Next(3) * 18 * 2);
						tile.TileFrameY = (short)(18 * 5 + (bottom ? 18 : 0));// (short)(Main.rand.Next(3) * 18);

						if (right)
                        {
							tile.TileFrameX += 18;
                        }

                        if (i % 2 == 1)
                        {
                            tile.TileFrameY += 18 * 2;
                        }

                        return false;
					}
				}
			}
            return true;
		}

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX <= 18 * 3)
            {
                if (i % 2 == 1)
                {
                    tile.TileFrameX += 18 * 5;
                    tile.TileFrameY += 18 * 5;
                }
            }
        }

		private bool BlendingTile(int x, int y)
        {
			Tile tile = Main.tile[x, y];
			return tile.HasTile && (tile.TileType == Type || Main.tileMerge[Type][tile.TileType]);
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

		public override bool CanExplode(int i, int j) => false;
    }

    [LegacyName("VaultProcessor")]
    public class PrototypeComputer : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

            Main.tileMerge[ModContent.TileType<PrototypePlating>()][Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = false;

            MinPick = 200;
            MineResist = 4;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(60, 53, 67));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (BlendingTile(i - 1, j) && BlendingTile(i + 1, j) && BlendingTile(i, j - 1) && BlendingTile(i, j + 1))
            {
                bool top = !BlendingTile(i - 1, j - 1) || !BlendingTile(i + 1, j - 1);
                bool bottom = !BlendingTile(i - 1, j + 1) || !BlendingTile(i + 1, j + 1);

                if (top ^ bottom)
                {
                    bool left = !BlendingTile(i - 1, j + (bottom ? 1 : -1));
                    bool right = !BlendingTile(i + 1, j + (bottom ? 1 : -1));

                    if (left ^ right)
                    {
                        Tile tile = Main.tile[i, j];

                        tile.TileFrameX = (short)(Main.rand.Next(3) * 18 * 2);
                        tile.TileFrameY = (short)(18 * 5 + (bottom ? 18 : 0));// (short)(Main.rand.Next(3) * 18);

                        if (right)
                        {
                            tile.TileFrameX += 18;
                        }

                        return false;
                    }
                }
            }
            return true;
        }

        private bool BlendingTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == Type || Main.tileBlendAll[tile.TileType]);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 + 8 - (int)Main.screenPosition.X, j * 16 + 8 - (int)Main.screenPosition.Y) + zero, new Rectangle(16, Main.rand.NextBool(2) ? 2 : 0, 2, 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            r = Main.rand.NextFloat(0.25f, 0.5f);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }

    [LegacyName("VaultConduit")]
    public class PrototypeConduits : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileMerge[ModContent.TileType<PrototypePlating>()][Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.BlockMergesWithMergeAllBlockOverride[Type] = false;

            MinPick = 200;
            MineResist = 4;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(106, 77, 85));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (BlendingTile(i - 1, j) && BlendingTile(i + 1, j) && BlendingTile(i, j - 1) && BlendingTile(i, j + 1))
            {
                bool top = !BlendingTile(i - 1, j - 1) || !BlendingTile(i + 1, j - 1);
                bool bottom = !BlendingTile(i - 1, j + 1) || !BlendingTile(i + 1, j + 1);

                if (top ^ bottom)
                {
                    bool left = !BlendingTile(i - 1, j + (bottom ? 1 : -1));
                    bool right = !BlendingTile(i + 1, j + (bottom ? 1 : -1));

                    if (left ^ right)
                    {
                        Tile tile = Main.tile[i, j];

                        tile.TileFrameX = (short)(Main.rand.Next(3) * 18 * 2);
                        tile.TileFrameY = (short)(18 * 5 + (bottom ? 18 : 0));// (short)(Main.rand.Next(3) * 18);

                        if (right)
                        {
                            tile.TileFrameX += 18;
                        }

                        return false;
                    }
                }
            }
            return true;
        }

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX <= 18 * 3)
            {
                if (i % 3 == 0)
                {
                    tile.TileFrameX = 18;
                }
                else tile.TileFrameX = (short)(18 * Main.rand.Next(2, 4));
            }
        }

        private bool BlendingTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == Type || Main.tileBlendAll[tile.TileType]);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }

    [LegacyName("VaultSupport")]
    public class PrototypeSupport : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;

            TileID.Sets.IsBeam[Type] = true;
            TileID.Sets.WallsMergeWith[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            MinPick = 200;
            MineResist = 1;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(54, 48, 61));
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }

    [LegacyName("VaultLight")]
    public class PrototypeLight : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            MinPick = 200;
            MineResist = 1;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(255, 255, 255));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            if (tile.WallType == 0)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }

        public override bool CanPlace(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            return tile.WallType != 0;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].WallType != ModContent.WallType<PrototypeWallUnsafe>())
            {
                r = g = b = 1;
            }
            else if (RemSystem.vaultLightIntensity > 0)
            {
                if (RemSystem.vaultLightIntensity == 1 || RemSystem.vaultLightFlicker < RemSystem.vaultLightIntensity)
                {
                    r = g = b = RemSystem.vaultLightIntensity;
                }
            }
        }
    }

    [LegacyName("VaultVent")]
    public class PrototypeVent : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            //Main.tileMerge[Type][ModContent.TileType<PrototypePlating>()] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            MinPick = 200;
            MineResist = 1;
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(36, 33, 46));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            if (tile.WallType == 0)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }

        public override bool CanPlace(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            return tile.WallType != 0;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => !WorldGen.gen;

        public override bool CanExplode(int i, int j) => false;
    }
}
