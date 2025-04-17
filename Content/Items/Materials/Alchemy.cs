using System.Collections.Generic;
using System.Media;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Materials
{

    public class Alchemy : GlobalItem
    {
        public override void AddRecipes()
        {
            //ModRecipe

            //modrecipe = Recipe.Create(Mod);

        }

        //      public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        //{
        //	if (item.type == ItemID.WaterWalkingPotion)
        //	{
        //		var line = new TooltipLine(mod, "Tooltip#", "A volatile substance ideal for guns and explosives.");
        //		tooltips.Add(line);
        //	}
        //}
        //public override void OnConsumeItem(Item item, Player player)
        //{
        //	if (item.type == ItemID.WaterWalkingPotion)
        //	{
        //		player.AddBuff(BuffID.Flipper, 8 * 60 * 60);
        //	}
        //}
    }
}
