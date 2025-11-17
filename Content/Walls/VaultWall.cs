using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls
{
	[LegacyName("vaultwall")]
	public class VaultWall : ModWall
	{
		public override void SetStaticDefaults()		
		{
			Main.wallHouse[Type] = true;
            Main.wallLargeFrames[Type] = 2;

            DustType = DustID.Iron;
			HitSound = SoundID.Tink;

			AddMapEntry(new Color(44, 38, 51));
		}

		public override bool CanExplode(int i, int j) => false;
	}

	[LegacyName("vaultwallunsafe")]
	public class VaultWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/VaultWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            Main.wallLargeFrames[Type] = 2;

            DustType = DustID.Iron;
			HitSound = SoundID.Tink;

            AddMapEntry(new Color(44, 38, 51));
        }

		public override void KillWall(int i, int j, ref bool fail)
		{
			fail = !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3;
		}

        public override bool CanExplode(int i, int j) => false;
    }

    public class VaultBeamWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(26, 23, 36));
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class VaultBeamWallUnsafe : ModWall
    {
        public override string Texture => "Remnants/Content/Walls/VaultBeamWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(26, 23, 36));
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3;
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class VaultHazardWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLargeFrames[Type] = 2;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(76, 57, 47));
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class VaultHazardWallUnsafe : ModWall
    {
        public override string Texture => "Remnants/Content/Walls/VaultHazardWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallLargeFrames[Type] = 2;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(76, 57, 47));
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3;
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class VaultRailing : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(60, 53, 67));
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
