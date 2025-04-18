using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace RemnantsTemp.World.AltPlanetoids
{
    [JITWhenModsEnabled("CalamityMod")]
    public class ClearTileFix : GlobalTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringOreRunner[TileID.Meteorite] = true;

            if (ModLoader.HasMod("CalamityMod"))
            {
                if (ModContent.TryFind("CalamityMod", "ExodiumOre", out ModTile Exodium))
                {
                    TileID.Sets.CanBeClearedDuringOreRunner[Exodium.Type] = true;
                }
            }
        }
        public override bool CanReplace(int i, int j, int tile, int tileTypeBeingPlaced)
        {
            if (tile == ModContent.TileType<PlantyMush>() || tile == ModContent.TileType<AbyssGravel>())
            {
                if (NPC.downedBoss3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (tile == ModContent.TileType<Voidstone>())
            {
                if (DownedBossSystem.downedLeviathan)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return base.CanReplace(i, j, tile, tileTypeBeingPlaced);
        }
    }


    [JITWhenModsEnabled("CalamityMod")]
    public class AltPlanetoids : MicroBiome
    {
        private Rectangle _area;

        public static void GenerateNewPlanetoids(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Scattering Low Orbit Debris";
            var config2 = WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");

            int PlanetoidCount = Main.maxTilesX / 100;
            int i = 0;

            const int PlanetoidAttempts = 200000;
            i = 0;
            while (PlanetoidCount > 0 && i < PlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.05), (int)(Main.maxTilesX * 0.95));
                int y = WorldGen.genRand.Next(65, 100);


                bool placed = config2.CreateBiome<Planets>().Place(new Point(x, y), GenVars.structures);

                if (placed)
                    PlanetoidCount--;
                i++;
            }
        }

        public static bool InvalidSkyPlacementArea(Rectangle area)
        {
            for (int i = area.Left; i < area.Right; i++)
            {
                for (int j = area.Top; j < area.Bottom; j++)
                {
                    if (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == TileID.Sunplate)
                        return false;
                }
            }
            return true;
        }

        public bool CheckIfPlaceable(Point origin, int radius, StructureMap structures)
        {
            int gap = 22;
            int myRadius = radius + gap;
            int diameter = myRadius * 2;
            _area = new Rectangle(origin.X - myRadius, origin.Y - myRadius, diameter, diameter);

            if (!InvalidSkyPlacementArea(_area))
                return false;

            if (!structures.CanPlace(_area))
            {
                return false;
            }

            Dictionary<ushort, int> dict = new Dictionary<ushort, int>();
            CalamityMod.World.CustomActions.SolidScanner scanner = new CalamityMod.World.CustomActions.SolidScanner();
            WorldUtils.Gen(_area.Location, new Shapes.Rectangle(_area.Width, _area.Height), scanner);
            if (scanner.GetCount() > 2)
            {
                return false;
            }
            return true;
        }

        public override bool Place(Point origin, StructureMap structures)
        {
            structures.AddStructure(_area);

            //Optional, add the planetoid's center to a list, do something else etc.
            //Just have this option available

            return true;
        }
    }
}
