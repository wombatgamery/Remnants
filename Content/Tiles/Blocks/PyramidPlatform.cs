using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Content.Tiles.Blocks
{
    public class PyramidPlatform : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;

            TileID.Sets.Platforms[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.FullCopyFrom(TileID.Platforms);
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            MinPick = 65;
            DustType = DustID.Sand;
			HitSound = SoundID.Tink;
            AdjTiles = new int[] { TileID.Platforms };

            AddMapEntry(new Color(222, 148, 86));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}