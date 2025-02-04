using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Items.Documents
{
    public class ScientistNotes1 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.width = 11 * 2;
            Item.height = 11 * 2;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class ScientistNotes2 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ModContent.ItemType<ScientistNotes1>());
        }
    }

    public class ScientistNotes3 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ModContent.ItemType<ScientistNotes1>());
        }
    }

    public class ScientistNotes4 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ModContent.ItemType<ScientistNotes1>());
        }
    }

    public class ScientistNotes5 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ModContent.ItemType<ScientistNotes1>());
        }
    }

    public class ScientistNotes6 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ModContent.ItemType<ScientistNotes1>());
        }
    }

    public class ScientistNotes7 : ModItem
    {
        public override string Texture => "Remnants/Items/Documents/ScientistNotes";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ModContent.ItemType<ScientistNotes1>());
        }
    }
}