using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
//using SubworldLibrary;
using Remnants.Tiles.Blocks;
using Remnants.Walls;
using Remnants.Walls.dev;
using Remnants.Tiles;
using StructureHelper;
using Terraria.DataStructures;
using Remnants.Walls.Vanity;
using System;

namespace Remnants.World.Subworlds
{
    public class SubworldGenPass : GenPass
    {
        private Action<GenerationProgress> method;

        public SubworldGenPass(Action<GenerationProgress> method) : base("", 1)
        {
            this.method = method;
        }

        public SubworldGenPass(float weight, Action<GenerationProgress> method) : base("", weight)
        {
            this.method = method;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            method(progress);
        }
    }
}