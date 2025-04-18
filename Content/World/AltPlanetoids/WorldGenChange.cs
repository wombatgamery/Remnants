using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using RemnantsTemp;

namespace RemnantsTemp.World.AltPlanetoids;

[JITWhenModsEnabled("CalamityMod")]
public class WorldGenChange : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        Worldgen PlanetConfig = Worldgen.Instance;
        if (ModLoader.HasMod("CalamityMod") && PlanetConfig.AltPlanetoids)
        {
          RemovePass(tasks, FindIndex(tasks, "Planetoids"));
          tasks.Insert(tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup")) - 1, new PassLegacy("Planetoids", AltPlanetoids.GenerateNewPlanetoids));
        }
    }

    private static void InsertPass(List<GenPass> tasks, GenPass item, int index, bool replace = false)
    {
        if (replace)
        {
            RemovePass(tasks, index);
        }
        if (index != -1)
        {
            tasks.Insert(index, item);
        }
    }

    private static void RemovePass(List<GenPass> tasks, int index, bool destroy = false)
    {
        if (index != -1)
        {
            if (destroy)
            {
                tasks.RemoveAt(index);
            }
            else
            {
                tasks[index].Disable();
            }
        }
    }
    private static int FindIndex(List<GenPass> tasks, string value)
    {
        return tasks.FindIndex((genpass) => genpass.Name.Equals(value));
    }

    private static Tile Tile(int x, int y)
    {
        return Framing.GetTileSafely(x, y);
    }
}