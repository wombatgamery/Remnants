using CalamityMod.DataStructures;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Remnants.Content;
using RemnantsTemp;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace RemnantsTemp.World.AltPlanetoids;

[JITWhenModsEnabled("CalamityMod")]
public class Planets : AltPlanetoids
{
    private ushort[] OreTypes = new ushort[]
    {
        TileID.LunarOre,
        TileID.ShimmerBlock
    };
    private ushort[] MagmaOreTypes = new ushort[]
    {
        TileID.LunarOre,
        TileID.Obsidian
    };

    public override bool Place(Point origin, StructureMap structures)
    {
        int radius = _random.Next(7, 26);

        if (!CheckIfPlaceable(origin, radius, structures))
        {
            return false;
        }

        if (!ModContent.GetInstance<Worldgen>().DoLava)
        {
            PlacePlanet(origin, radius, _random.Next(OreTypes));
        }
        else
        {
            PlacePlanet(origin, radius, _random.Next(MagmaOreTypes));
        }

        return base.Place(origin, structures);
    }

    public void PlacePlanet(Point origin, int radius, ushort oreType)
    {
        int Varient = _random.Next(1, 7);

        Circle planetoid = new Circle(origin.ToVector2() * 16f + new Vector2(8f), radius * 16f);

        ShapeData crust = new ShapeData();
        ShapeData core = new ShapeData();

        int outerRadius = (int)(radius * WorldGen.genRand.NextFloat(0.74f, 0.82f));

        GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
        WorldUtils.Gen(origin, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
        {
                blotchMod.Output(crust)
        }));
        WorldUtils.Gen(origin, new Shapes.Circle(outerRadius), Actions.Chain(new GenAction[]
        {
                blotchMod.Output(core)
        }));

        crust.Subtract(core, origin, origin);

        WorldUtils.Gen(origin, new ModShapes.All(core), Actions.Chain(new GenAction[]
        {
                new Actions.PlaceTile(Main.getGoodWorld ? TileID.WoodenSpikes : (ushort)ModContent.TileType<ExodiumOre>()),
                new Actions.PlaceWall(WallID.ShimmerBlockWall)
        }));
        WorldUtils.Gen(origin, new ModShapes.All(crust), Actions.Chain(new GenAction[]
        {
                new Actions.PlaceTile(TileID.Meteorite),
                new Actions.PlaceWall(WallID.ShimmerBlockWall)
        }));

        int randDirt = (int)(radius * 0.4f);
        int randStone = (int)(randDirt * 0.6f);
        for (int i = 0; i < randStone; i++)
        {
            int x = origin.X;
            int y = origin.Y;
            while (Vector2.Distance(origin.ToVector2(), new Vector2(x, y)) < outerRadius)
            {
                x = _random.Next(origin.X - radius, origin.X + radius + 1);
                y = _random.Next(origin.Y - radius, origin.Y + radius + 1);
            }
            WorldGen.TileRunner(x, y, _random.NextFloat(4.6f, 7.6f), _random.Next(7, 16), Main.getGoodWorld ? TileID.WoodenSpikes : (ushort)ModContent.TileType<ExodiumOre>());
        }
        for (int i = 0; i < randDirt; i++)
        {
            int x = _random.Next(origin.X - outerRadius, origin.X + outerRadius + 1);
            int y = _random.Next(origin.Y - outerRadius, origin.Y + outerRadius + 1);

            WorldGen.TileRunner(x, y, _random.NextFloat(5f, 8f), _random.Next(8, 18), TileID.Meteorite);
        }

        int numStrokes = radius > 20 ? 3 : 2;
        for (int i = 0; i < numStrokes; i++)
        {
            Vector2 start = planetoid.RandomPointOnCircleEdge();
            Vector2 end = planetoid.Center - (start - planetoid.Center);
            Vector2 control = planetoid.RandomPointOnCircleEdge();
            float min = radius * 0.6f * 16f;
            while (Vector2.Distance(control, start) < min || Vector2.Distance(control, end) < min)
            {
                control = planetoid.RandomPointOnCircleEdge();
            }

            BezierCurve curve = new BezierCurve(start, control, end);

            int strokeSteps = 50;
            double baseStrength = (double)_random.NextFloat(1.2f, 2.4f);

            List<Vector2> tilePoints = curve.GetPoints(strokeSteps);
            for (int k = 0; k < strokeSteps; k++)
            {
                float progress = k / (float)strokeSteps * (float)Math.PI;
                double strengthMultiplier = 1.0 + Math.Sin(progress) * baseStrength * 1.1f;

                Vector2 nextPoint = tilePoints[k];
                int x = (int)(nextPoint.X / 16f);
                int y = (int)(nextPoint.Y / 16f);

                WorldGen.OreRunner(x, y, baseStrength * strengthMultiplier, 1, oreType);
            }
        }

        if (Varient == 2)
        {
            if (!ModContent.GetInstance<Worldgen>().DoLava)
            {
                WorldUtils.Gen(origin, new Shapes.Circle((int)(radius * 0.5f)), Actions.Chain(new GenAction[]
                {
                 new Modifiers.Blotches(2, 0.3),
                 new Actions.SetTile(TileID.ShimmerBlock, true),
                 new Actions.PlaceWall(WallID.ShimmerBlockWall)
                }));

                WorldUtils.Gen(origin, new Shapes.Circle((int)(radius * 0.3f)), Actions.Chain(new GenAction[]
                {
                 new Modifiers.Blotches(2, 0.3).Output(core),
                 new Actions.ClearTile(true),
                 new Actions.SetLiquid(3, 255)
                }));
            }
            else
            {
                WorldUtils.Gen(origin, new Shapes.Circle((int)(radius * 0.5f)), Actions.Chain(new GenAction[]
                {
                 new Modifiers.Blotches(2, 0.3),
                 new Actions.SetTile(TileID.Obsidian, true),
                 new Actions.PlaceWall(WallID.ObsidianBackUnsafe)
                }));

                WorldUtils.Gen(origin, new Shapes.Circle((int)(radius * 0.3f)), Actions.Chain(new GenAction[]
                {
                 new Modifiers.Blotches(2, 0.3).Output(core),
                 new Actions.ClearTile(true),
                 new Actions.SetLiquid(1, 255)
                }));
            }
        }

        if (Varient == 3 && radius <= 17)
        {
            if (!ModContent.GetInstance<Worldgen>().DoLava)
            {
                ShapeData hive = new ShapeData();
                int blobs = _random.Next(3, 6);
                float smallerRadius = radius * 0.55f;
                int minSize = (int)(radius * 0.4f);
                int maxSize = (int)(radius * 0.6f);
                for (int k = 0; k < blobs; k++)
                {
                    int moveX = _random.Next(-(int)smallerRadius, (int)smallerRadius);
                    int moveY = _random.Next(-(int)smallerRadius, (int)smallerRadius);
                    WorldUtils.Gen(origin,
                    new Shapes.Circle(_random.Next(minSize, maxSize), _random.Next(minSize, maxSize)),
                    Actions.Chain(new GenAction[]
                    {
                     new Modifiers.Offset(moveX, moveY),
                     new Modifiers.Blotches(2, 0.12),
                     new Actions.ClearTile(true).Output(hive),
                     new Actions.PlaceWall(WallID.ShimmerBlockWall)
                    }));
                }
                ShapeData hiveOutline = new ShapeData();
                WorldUtils.Gen(origin, new ModShapes.InnerOutline(hive, true), Actions.Chain(new GenAction[]
                {
                 new Actions.PlaceTile(TileID.ShimmerBlock),
                 new Actions.ClearWall().Output(hiveOutline),
                 new Modifiers.Conditions(new CalamityMod.World.CustomConditions.IsNotTouchingAir(false)),
                 new Actions.PlaceWall(WallID.ShimmerBlockWall)
                }));
                hive.Subtract(hiveOutline, origin, origin);
                WorldUtils.Gen(origin, new ModShapes.InnerOutline(hive, true), new Actions.PlaceTile(TileID.ShimmerBlock).Output(hiveOutline));
                hive.Subtract(hiveOutline, origin, origin);
                WorldUtils.Gen(origin, new ModShapes.All(hive), Actions.Chain(new GenAction[]
                {
                 new Actions.SetLiquid(3, 255)
                }));
            }
            else
            {
                ShapeData hive = new ShapeData();
                int blobs = _random.Next(3, 6);
                float smallerRadius = radius * 0.55f;
                int minSize = (int)(radius * 0.4f);
                int maxSize = (int)(radius * 0.6f);
                for (int k = 0; k < blobs; k++)
                {
                    int moveX = _random.Next(-(int)smallerRadius, (int)smallerRadius);
                    int moveY = _random.Next(-(int)smallerRadius, (int)smallerRadius);
                    WorldUtils.Gen(origin,
                    new Shapes.Circle(_random.Next(minSize, maxSize), _random.Next(minSize, maxSize)),
                    Actions.Chain(new GenAction[]
                    {
                     new Modifiers.Offset(moveX, moveY),
                     new Modifiers.Blotches(2, 0.12),
                     new Actions.ClearTile(true).Output(hive),
                     new Actions.PlaceWall(WallID.ObsidianBackUnsafe)
                    }));
                }
                ShapeData hiveOutline = new ShapeData();
                WorldUtils.Gen(origin, new ModShapes.InnerOutline(hive, true), Actions.Chain(new GenAction[]
                {
                 new Actions.PlaceTile(TileID.Obsidian),
                 new Actions.ClearWall().Output(hiveOutline),
                 new Modifiers.Conditions(new CalamityMod.World.CustomConditions.IsNotTouchingAir(false)),
                 new Actions.PlaceWall(WallID.ObsidianBackUnsafe)
                }));
                hive.Subtract(hiveOutline, origin, origin);
                WorldUtils.Gen(origin, new ModShapes.InnerOutline(hive, true), new Actions.PlaceTile(TileID.Obsidian).Output(hiveOutline));
                hive.Subtract(hiveOutline, origin, origin);
                WorldUtils.Gen(origin, new ModShapes.All(hive), Actions.Chain(new GenAction[]
                {
                 new Actions.SetLiquid(1, 255)
                }));
            }

            //Clear dirt walls on outer edge because of stone / ore
            WorldUtils.Gen(origin, new ModShapes.InnerOutline(crust), Actions.Chain(new GenAction[]
            {
             new Modifiers.OnlyTiles(new ushort[] { Main.getGoodWorld ? TileID.Obsidian : TileID.Meteorite, TileID.LunarOre, (ushort)ModContent.TileType<ExodiumOre>(), oreType }),
             new Modifiers.IsTouchingAir(true),
             new Modifiers.OnlyWalls(WallID.ObsidianBackUnsafe, WallID.ShimmerBlockWall),
             new Actions.Smooth(),
             new Actions.ClearWall(true)
            }));
        }

        //Clear dirt walls on outer edge because of stone / ore
        WorldUtils.Gen(origin, new ModShapes.InnerOutline(crust), Actions.Chain(new GenAction[]
        {
             new Modifiers.OnlyTiles(new ushort[] { Main.getGoodWorld ? TileID.ShimmerBlock : TileID.Meteorite, TileID.LunarOre, (ushort)ModContent.TileType<ExodiumOre>(), oreType }),
             new Modifiers.IsTouchingAir(true),
             new Modifiers.OnlyWalls(WallID.ShimmerBlockWall),
             new Actions.Smooth(),
             new Actions.ClearWall(true)
        }));
    }
}

