using Microsoft.Xna.Framework;
using Remnants.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Walls.Underworld
{
	[LegacyName("VaultWall")]
	public class PrototypeWall : ModWall
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

	[LegacyName("VaultWallUnsafe")]
	public class PrototypeWallUnsafe : ModWall
	{
		public override string Texture => "Remnants/Content/Walls/Underworld/PrototypeWall";

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

    [LegacyName("VaultBeamWall")]
    public class PrototypeBeamWall : ModWall
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

    [LegacyName("VaultBeamWallUnsafe")]
    public class PrototypeBeamWallUnsafe : ModWall
    {
        public override string Texture => "Remnants/Content/Walls/Underworld/PrototypeBeamWall";

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

    [LegacyName("VaultHazardWall")]
    public class PrototypeHazardWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLargeFrames[Type] = 2;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(83, 44, 42));
        }

        public override bool CanExplode(int i, int j) => false;
    }

    [LegacyName("VaultHazardWallUnsafe")]
    public class PrototypeHazardWallUnsafe : ModWall
    {
        public override string Texture => "Remnants/Content/Walls/Underworld/PrototypeHazardWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallLargeFrames[Type] = 2;

            DustType = DustID.Iron;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(83, 44, 42));
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3;
        }

        public override bool CanExplode(int i, int j) => false;
    }

    [LegacyName("VaultRailing")]
    public class PrototypeRailing : ModWall
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
