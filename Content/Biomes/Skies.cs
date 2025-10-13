using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Biomes.Backgrounds;
using Remnants.Content.Buffs;
using Remnants.Content.Walls;
using Remnants.Content.Walls.Parallax;
using Terraria;
using Terraria.Graphics.Effects;

namespace Remnants.Content.Biomes
{
    public class SulfuricVentsSky : CustomSky
    {
        private bool isActive = false;
        private float intensity;

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {

        }

        //public override Color OnTileColor(Color inColor) => new(Vector4.Lerp(new Vector4(0.75f, 0.75f, 0.5f, 1f), inColor.ToVector4(), 1 - intensity));

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive || intensity > 0f;
        }
    }
}